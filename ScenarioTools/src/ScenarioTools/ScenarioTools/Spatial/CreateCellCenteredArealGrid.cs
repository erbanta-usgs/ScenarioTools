using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using ScenarioTools.Numerical;

using USGS.Puma.Core;
using USGS.Puma.FiniteDifference;
using USGS.Puma.NTS.Features;
using USGS.Puma.NTS.Geometries;

namespace ScenarioTools.Spatial
{
    public class CreateCellCenteredArealGrid
    {
        private static FeatureCollection _featureCollection;
        public delegate void MyDelegate();
        public static MyDelegate doEvents;
        public static ToolStripProgressBar ProgressBar;

        /// <summary>
        /// Generate a CellCenteredArealGrid from a polygon 
        /// shapefile containing a finite-difference grid.
        /// </summary>
        /// <param name="ShapefilePath"></param>
        /// <returns></returns>
        public static CellCenteredArealGrid CreateGrid(string ShapefilePath, GridInfo gridInfo)
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);

            // If a common error is detected, null is returned.
            // Ned TODO: Need to add string argument ErrMsg which will describe any recognized error.
            CellCenteredArealGrid grid = null;
            bool isGood = true;
            int numatts = 0;
            int nrow = 0;
            int ncol = 0;
            int ncells = -1;
            int[,] cellPtr;
            Array1d<double> delr;
            Array1d<double> delc;
            int modflowOriginIndex;
            double originX = 0.0d;
            double originY = 0.0d;
            string rowAttName = "";
            string colAttName = "";
            Feature[] featureArray = null;
            try
            {
                // Read ESRI shapefile and import features to a FeatureCollection
                //FeatureCollection featureList = USGS.Puma.IO.EsriShapefileIO.Import(ShapefilePath);
                _featureCollection = null;
                backgroundWorker.RunWorkerAsync(ShapefilePath);
                while (backgroundWorker.IsBusy && _featureCollection == null)
                {
                    try
                    {
                        Thread.Sleep(1);
                        if (doEvents != null)
                        {
                            doEvents();
                        }
                    }
                    catch (Exception e)
                    {
                        string error = e.ToString();
                    }
                }
                FeatureCollection featureList = _featureCollection;

                // Attempt to define grid geometry from shapefile features.
                ncells = featureList.Count;
                featureArray = new Feature[ncells];
                featureArray = featureList.ToArray();
                numatts = featureArray[0].Attributes.Count;
                string errmsg = " Error encountered in defining grid geometry from shapefile '" + ShapefilePath + "': ";
                if (ncells < 1)
                {
                    isGood = false;
                    errmsg = errmsg + "Shapefile contains no features.";
                }
                //
                if (isGood)
                {
                    IdentifyRowColAttributes(ref isGood, numatts, ref rowAttName, ref colAttName, featureArray);
                    if (!isGood)
                    {
                        errmsg = errmsg + "Shapefile does not contain attributes identifying row and column numbers. "
                            + "An attribute identifying row number needs to be named 'row' or 'rownum'. "
                            + "An attribute identifying column number needs to be named 'col', 'colnum', 'column', or 'columnnum'.";
                    }
                }
                //
                if (isGood)
                {
                    string err = "";
                    DefineNrowNcol(ref isGood, ref nrow, ref ncol, ncells, rowAttName, colAttName, featureArray, ref err);
                    if (!isGood)
                    {
                        errmsg = errmsg + err;
                    }
                }
                //
                if (isGood)
                {
                    // Allocate and populate arrays to hold DELR, DELC, and cell pointers
                    delr = new Array1d<double>(ncol);
                    delc = new Array1d<double>(nrow);
                    cellPtr = new int[nrow + 1, ncol + 1];
                    PopulateCellPtr(ncells, cellPtr, rowAttName, colAttName, featureArray);
                    double angleDeg;
                    angleDeg = RotationAngle(ncol, cellPtr, featureArray);
                    //
                    Polygon polygon = (Polygon)featureArray[cellPtr[1, 1]].Geometry;
                    Polygon[] polygons = new Polygon[2];
                    polygons[0] = (Polygon)featureArray[cellPtr[1, 2]].Geometry;
                    polygons[1] = (Polygon)featureArray[cellPtr[2, 1]].Geometry;
                    modflowOriginIndex = -1;
                    // Assign MODFLOW grid origin (top left vertex of cell (1,1))
                    // Origin is the only vertex of cell(1,1) not shared with 
                    // neighboring cells
                    FindUnsharedVertex(ref modflowOriginIndex, ref originX, ref originY, polygon, polygons);
                    PopulateDelrDelc(nrow, ncol, cellPtr, delr, delc, modflowOriginIndex, featureArray);
                    // Redefine origin (origin for CellCenteredArealGrid is lower left corner of grid).  
                    polygon = (Polygon)featureArray[cellPtr[nrow, 1]].Geometry;
                    polygons[0] = (Polygon)featureArray[cellPtr[nrow - 1, 1]].Geometry;
                    polygons[1] = (Polygon)featureArray[cellPtr[nrow, 2]].Geometry;
                    int originIndex = -1;
                    FindUnsharedVertex(ref originIndex, ref originX, ref originY, polygon, polygons);
                    // Construct CellCenteredArealGrid.  Constructor takes rotationAngle in degrees.
                    grid = new CellCenteredArealGrid(delc, delr, originX, originY, angleDeg);
                    gridInfo.ShapefileAbsolutePath = ShapefilePath;
                }
                else
                {
                    MessageBox.Show(errmsg);
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred creating the CellCenteredArealGrid object.");
            }
            return grid;
        }

        private static void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // The argument will be the shapefile path (string)
            BackgroundWorker worker = (BackgroundWorker)sender;
            string shapefilePath = (string)e.Argument;
            FeatureCollection featureList = USGS.Puma.IO.EsriShapefileIO.Import(shapefilePath, worker);
            _featureCollection = featureList;
            e.Result  = featureList;
        }
        private static void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (ProgressBar != null)
            {
                ProgressBar.Visible = true;
                if (e.ProgressPercentage < ProgressBar.Minimum)
                {
                    ProgressBar.Value = ProgressBar.Minimum;
                }
                else if (e.ProgressPercentage > ProgressBar.Maximum)
                {
                    ProgressBar.Value = ProgressBar.Maximum;
                }
                else
                {
                    ProgressBar.Value = e.ProgressPercentage;
                }
            }
        }
        private static void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //EnableControls(true);
            //UseWaitCursor = false;
            //this.statusLabelPrimary.Text = "";
            if (e.Result is FeatureCollection)
            {
                _featureCollection = (FeatureCollection)e.Result;
            }
            if (ProgressBar != null)
            {
                ProgressBar.Visible = false;
            }
        }


        private static void PopulateDelrDelc(int nrow, int ncol, int[,] cellPtr, 
            Array1d<double> delr, Array1d<double> delc, int modflowOriginIndex, 
            Feature[] featureArray)
        {
            // Assign row and column spacing
            // Assumption: index identified as moflowOriginIndex consistently
            // points to upper left vertex of each cell in grid.
            // Assign delr, cell widths in row direction
            double x0, x1, y0, y1;
            for (int j = 1; j < ncol; j++)
            {
                x0 = featureArray[cellPtr[1, j]].Geometry.Coordinates[modflowOriginIndex].X;
                y0 = featureArray[cellPtr[1, j]].Geometry.Coordinates[modflowOriginIndex].Y;
                x1 = featureArray[cellPtr[1, j + 1]].Geometry.Coordinates[modflowOriginIndex].X;
                y1 = featureArray[cellPtr[1, j + 1]].Geometry.Coordinates[modflowOriginIndex].Y;
                delr[j] = Math.Sqrt(Math.Pow((x1 - x0), 2.0d) +
                                    Math.Pow((y1 - y0), 2.0d));
            }
            x0 = featureArray[cellPtr[1, ncol - 1]].Geometry.Centroid.X;
            y0 = featureArray[cellPtr[1, ncol - 1]].Geometry.Centroid.Y;
            x1 = featureArray[cellPtr[1, ncol]].Geometry.Centroid.X;
            y1 = featureArray[cellPtr[1, ncol]].Geometry.Centroid.Y;
            double centroidLength = Math.Sqrt(Math.Pow((x1 - x0), 2.0d) +
                                              Math.Pow((y1 - y0), 2.0d));
            delr[ncol] = 2.0d * (centroidLength - 0.5d * delr[ncol - 1]);
            // Assign delc, cell heights in column direction
            // ii converts Modflow index to index order
            // consistent with geographic coordinates
            int ii;
            for (int i = 1; i < nrow; i++)
            {
                x0 = featureArray[cellPtr[i, 1]].Geometry.Coordinates[modflowOriginIndex].X;
                y0 = featureArray[cellPtr[i, 1]].Geometry.Coordinates[modflowOriginIndex].Y;
                x1 = featureArray[cellPtr[i + 1, 1]].Geometry.Coordinates[modflowOriginIndex].X;
                y1 = featureArray[cellPtr[i + 1, 1]].Geometry.Coordinates[modflowOriginIndex].Y;
                ii = nrow - i + 1;
                delc[ii] = Math.Sqrt(Math.Pow((x1 - x0), 2.0d) +
                                    Math.Pow((y1 - y0), 2.0d));
            }
            x0 = featureArray[cellPtr[nrow - 1, 1]].Geometry.Centroid.X;
            y0 = featureArray[cellPtr[nrow - 1, 1]].Geometry.Centroid.Y;
            x1 = featureArray[cellPtr[nrow, 1]].Geometry.Centroid.X;
            y1 = featureArray[cellPtr[nrow, 1]].Geometry.Centroid.Y;
            centroidLength = Math.Sqrt(Math.Pow((x1 - x0), 2.0d) +
                                       Math.Pow((y1 - y0), 2.0d));
            delc[1] = 2.0d * (centroidLength - 0.5d * delc[nrow - 1]);
        }

        private static void FindUnsharedVertex(ref int originIndex, ref double originX, 
            ref double originY, Polygon polygon, Polygon[] polygons)
        {
            // Find vertex of polygon that is not shared by two neighboring polygons
            bool shared;
            for (int i = 0; i < 5; i++) // Iterate through vertices of polygon
            {
                shared = false;
                double vertexX = polygon.Coordinates[i].X;
                double vertexY = polygon.Coordinates[i].Y;
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        if (polygons[j].Coordinates[k].X == vertexX &&
                            polygons[j].Coordinates[k].Y == vertexY)
                        {
                            shared = true;
                            break;
                        }
                        if (shared)
                        {
                            break;
                        }
                    }
                }
                if (!shared)
                {
                    originIndex = i;
                    originX = vertexX;
                    originY = vertexY;
                    break;
                }
            }
        }

        private static double RotationAngle(int ncol, int[,] cellPtr, Feature[] featureArray)
        {
            // Assign rotation angle in degrees
            double angleDegLocal;
            double x11 = featureArray[cellPtr[1, 1]].Geometry.Centroid.X;
            double y11 = featureArray[cellPtr[1, 1]].Geometry.Centroid.Y;
            double x1Ncol = featureArray[cellPtr[1, ncol]].Geometry.Centroid.X;
            double y1Ncol = featureArray[cellPtr[1, ncol]].Geometry.Centroid.Y;
            double delx = x1Ncol - x11;
            double dely = y1Ncol - y11;
            double hypot = Math.Sqrt(delx * delx + dely * dely);
            double sine = dely / hypot;
            double arcsine = Math.Asin(sine);
            double angleRad;
            if (delx >= 0.0d)
            {
                angleRad = arcsine;
            }
            else
            {
                if (dely >= 0.0d)
                {
                    angleRad = 2 * Math.PI - arcsine;
                }
                else
                {
                    angleRad = -2 * Math.PI + arcsine;
                }
            }
            angleDegLocal = angleRad * (180.0d / Math.PI);
            return angleDegLocal;
        }

        private static void PopulateCellPtr(int ncells, int[,] cellPtr, string rowAttName, 
            string colAttName, Feature[] featureArray)
        {
            // Populate a 2-D array of pointers to cells, 
            // referenced by Modflow grid indices.
            // Do not use elements of cellPtr with index = 0, because Modflow doesn't.
            int row, col;
            for (int i = 0; i < ncells; i++)
            {
                row = Convert.ToInt32(((double)featureArray[i].Attributes[rowAttName]));
                col = Convert.ToInt32(((double)featureArray[i].Attributes[colAttName]));
                cellPtr[row, col] = i;
            }
        }

        private static void DefineNrowNcol(ref bool isGood, ref int nrow, ref int ncol, 
            int ncells, string rowAttName, string colAttName, Feature[] featureArray, ref string errmsg)
        {
            // Define NROW and NCOL
            int row, col;
            errmsg = "";
            USGS.Puma.NTS.Geometries.Geometry geometry;
            for (int i = 0; i < ncells; i++)
            {
                geometry = (USGS.Puma.NTS.Geometries.Geometry)featureArray[i].Geometry;
                if (!IsGeneralRectange(ref geometry)) 
                { 
                    isGood = false; 
                } 
                row = Convert.ToInt32(((double)featureArray[i].Attributes[rowAttName]));
                if (row > nrow) 
                { 
                    nrow = row; 
                }
                col = Convert.ToInt32(((double)featureArray[i].Attributes[colAttName]));
                if (col > ncol) 
                { 
                    ncol = col; 
                }
            }
            int rcproduct = nrow * ncol;
            if (rcproduct != ncells)
            {
                isGood = false;
                errmsg = "The number of polygons does not equal the product NROW x NCOL.";
            }
        }

        private static void IdentifyRowColAttributes(ref bool isGood, int numatts, 
            ref string rowAttName, ref string colAttName, Feature[] featureArray)
        {
            int i;
            // Identify attributes containing Row and Column numbers
            {
                USGS.Puma.NTS.Geometries.Geometry geometry;
                Attribute[] attributes;
                if (numatts >= 2)
                {
                    attributes = new Attribute[numatts];
                    string[] attributeNames = new string[numatts];
                    attributeNames = featureArray[0].Attributes.GetNames();
                    for (i = 0; i < numatts; i++)
                    {
                        if (attributeNames[i].ToLower() == "row" | attributeNames[i].ToLower() == "rownum")
                        {
                            rowAttName = attributeNames[i];
                        }
                        if (attributeNames[i].ToLower() == "col" | attributeNames[i].ToLower() == "column" |
                            attributeNames[i].ToLower() == "colnum" | attributeNames[i].ToLower() == "columnnum")
                        {
                            colAttName = attributeNames[i];
                        }
                    }
                    if (rowAttName == "" || colAttName == "")
                    {
                        isGood = false;
                    }
                }
                else
                {
                    isGood = false;
                }
            }
        }

        private static bool IsGeneralRectange(ref USGS.Puma.NTS.Geometries.Geometry geometry)
        {
            if (geometry.NumPoints != 5)
            {
                return false;
            }
            // Check that end point equals start point
            if (geometry.Coordinates[0].X != geometry.Coordinates[4].X ||
                geometry.Coordinates[0].Y != geometry.Coordinates[4].Y)
            {
                return false;
            }
            // Check that opposite sides are of equal length
            Point point0 = new Point(geometry.Coordinates[0]);
            Point point1 = new Point(geometry.Coordinates[1]);
            Point point2 = new Point(geometry.Coordinates[2]);
            Point point3 = new Point(geometry.Coordinates[3]);
            double side0 = Distance(ref point0, ref point1);
            double side1 = Distance(ref point1, ref point2);
            double side2 = Distance(ref point2, ref point3);
            double side3 = Distance(ref point3, ref point0);
            double epsilon = 1.0d-12;
            if (!MathUtil.Fuzzy_Equals(side0, side2, epsilon))
            {
                return false;
            }
            if (!MathUtil.Fuzzy_Equals(side1, side3, epsilon))
            {
                return false;
            }
            // check that diagonals are of equal length
            double diag0 = Distance(ref point0, ref point2);
            double diag1 = Distance(ref point1, ref point3);
            if (!MathUtil.Fuzzy_Equals(diag0, diag1, epsilon))
            {
                return false;
            }
            return true;
        }

        private static double Distance(ref USGS.Puma.NTS.Geometries.Point pointA, 
            ref USGS.Puma.NTS.Geometries.Point pointB)
        {
            double delx = pointB.X - pointA.X;
            double dely = pointB.Y - pointA.Y;
            return Math.Sqrt(delx * delx + dely * dely);
        }
    }
}
