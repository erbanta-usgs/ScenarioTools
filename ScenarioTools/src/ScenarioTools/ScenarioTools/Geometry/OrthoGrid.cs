using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using ScenarioTools.Spatial;

namespace ScenarioTools.Geometry
{
    public class OrthoGrid
    {
        public const string KEY_COL = "column";
        public const string KEY_ROW = "row";

        private double[] xWidths;
        private double[] yWidths;
        private double[] xCoordinates;
        private double[] yCoordinates;
        private Point2D anchor;
        private double rotation;
        private SpatialReference spatialReference;

        private Polygon2D[] polygons;

        public OrthoGrid(double[] xWidths, double[] yWidths, Point2D anchor, double rotation, SpatialReference spatialReference)
        {
            // Store the data.
            this.xWidths = xWidths;
            this.yWidths = yWidths;
            this.anchor = anchor;
            this.rotation = rotation;
            this.spatialReference = spatialReference;
        }
        public int NCols
        {
            get
            {
                return xWidths.Length;
            }
        }
        public int NRows
        {
            get
            {
                return yWidths.Length;
            }
        }
        public double[] XCoordinates
        {
            get
            {
                if (xCoordinates == null)
                {
                    calculateXCoordinates();
                }
                return xCoordinates;
            }
        }
        public double[] YCoordinates
        {
            get
            {
                if (yCoordinates == null)
                {
                    calculateYCoordinates();
                }
                return yCoordinates;
            }
        }
        private void calculateXCoordinates()
        {
            int nCols = this.NCols;
            xCoordinates = new double[nCols + 1];

            xCoordinates[0] = anchor.X;
            for (int i = 1; i < nCols + 1; i++)
            {
                xCoordinates[i] = xCoordinates[i - 1] + xWidths[i - 1];
            }
        }
        private void calculateYCoordinates()
        {
            int nRows = this.NRows;
            yCoordinates = new double[nRows + 1];

            yCoordinates[nRows] = anchor.Y;
            for (int i = nRows - 1; i >= 0; i--)
            {
                yCoordinates[i] = yCoordinates[i + 1] + yWidths[i];
            }
        }

        public Polygon2D[] GetPolygons()
        {
            if (polygons == null)
            {
                makePolygons();
            }
            return polygons;
        }
        private void makePolygons()
        {

            int nCols = this.NCols;
            int nRows = this.NRows;

            // Get the cell coordinates.
            double[] xCoordinates = this.XCoordinates;
            double[] yCoordinates = this.YCoordinates;

            // Make an array for the polygons.
            polygons = new Polygon2D[nCols * nRows];

            // Make a polygon for each cell.
            int cellIndex = 0;
            for (int j = 0; j < nRows; j++)
            {
                for (int i = 0; i < nCols; i++)
                {

                    // Make the polygon.
                    double x0 = xCoordinates[i];
                    double x1 = xCoordinates[i + 1];
                    double y0 = yCoordinates[j];
                    double y1 = yCoordinates[j + 1];
                    Point2D p0 = new Point2D(x0, y0);
                    Point2D p1 = new Point2D(x1, y0);
                    Point2D p2 = new Point2D(x1, y1);
                    Point2D p3 = new Point2D(x0, y1);
                    if (rotation != 0.0)
                    {
                        p0 = p0.RotateAbout(anchor, rotation);
                        p1 = p1.RotateAbout(anchor, rotation);
                        p2 = p2.RotateAbout(anchor, rotation);
                        p3 = p3.RotateAbout(anchor, rotation);
                    }

                    Polygon2D polygon = new Polygon2D(new Point2D[] { p0, p1, p2, p3 });

                    // Attach the row and column.
                    polygon.SetAttribute(KEY_COL, i + 1);
                    polygon.SetAttribute(KEY_ROW, j + 1);

                    // Store the polygon in the array.
                    polygons[cellIndex++] = polygon;
                }
            }
        }

        public void ToFile(string filename)
        {
            Polygon2D.WriteShapefile(filename, this.GetPolygons(), spatialReference);
        }
    }
}
