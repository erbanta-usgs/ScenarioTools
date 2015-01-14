using System;
using System.Collections.Generic;

using System.Text;
using System.Drawing;
using ScenarioTools.Geometry;
using USGS.Puma.FiniteDifference;

namespace ScenarioTools.DataClasses
{
    public class GeoMap
    {
        #region Fields
        float[,] _values;
        double[] _xCoords;
        double[] _yCoords;
        double _rotation;
        Extent _extent;
        #endregion Fields

        #region Constructor
        public GeoMap(float[,] values, double[] xCoords, double[] yCoords)
        {
            // Store references to the value grid and the coordinate arrays.
            this._values = values;
            this._xCoords = xCoords;
            this._yCoords = yCoords;

            _extent = new Extent(xCoords[0], yCoords[0], xCoords[xCoords.Length - 1], yCoords[yCoords.Length - 1]);
        }
        #endregion Constructor

        #region Properties
        public int NCols
        {
            get
            {
                return _xCoords.Length - 1;
            }
        }
        public int NRows
        {
            get
            {
                return _yCoords.Length - 1;
            }
        }
        #endregion Properties

        #region Static methods
        public static GeoMap FixedGridMap(float[,] values, Extent extent, bool convertFlowToFlux)
        {
            // Ned TODO:  Need an alternative method for variable DELR/DELC. 
            // As is, this method assumes DELR and DELC are uniform.
            // If the values array is null, return null.
            if (values == null)
            {
                return null;
            }
            if (extent == null)
            {
                return null;
            }

            // Calculate the x-coordinate array.
            int nCols = values.GetLength(1);
            double[] xCoords = new double[nCols + 1];
            xCoords[0] = extent.West;
            double xDelta = extent.Width / nCols;
            for (int i = 1; i < xCoords.Length; i++)
            {
                xCoords[i] = xCoords[i - 1] + xDelta;
            }

            // Calculate the y-coordinate array.
            int nRows = values.GetLength(0);
            double[] yCoords = new double[nRows + 1];
            yCoords[0] = extent.South;
            double yDelta = extent.Height / nRows;
            for (int i = 1; i < yCoords.Length; i++)
            {
                yCoords[i] = yCoords[i - 1] + yDelta;
            }

            // If need to convert flow values to flux values, divide by (uniform) cell area
            if (convertFlowToFlux)
            {
                float cellArea = Convert.ToSingle(xDelta * yDelta);
                for (int i = 0; i < nRows; i++)
                {
                    for (int j = 0; j < nCols; j++)
                    {
                        values[i, j] = values[i, j] / cellArea;
                    }
                }
            }

            // Return the new GeoMap.
            return new GeoMap(values, xCoords, yCoords);
        }
        public static GeoMap GridMap(float[,] values, CellCenteredArealGrid modelGrid, bool convertFlowToFlux)
        {
            if (values == null)
            {
                return null;
            }

            // Currently, GridMap does not support rotated grid
            if (modelGrid.Angle != 0.0)
            {
                return null;
            }

            // The origin of a CellCenteredArealGrid is at the lower left corner
            double xMin = modelGrid.OriginX;
            double yMin = modelGrid.OriginY;
            double yMax = yMin + modelGrid.TotalRowHeight;
            
            // Rows and columns are in Modflow order (row 1 at top)
            USGS.Puma.Core.Array1d<double> rowHeights = modelGrid.GetRowSpacing();
            USGS.Puma.Core.Array1d<double> columnWidths = modelGrid.GetColumnSpacing();

            int nCols = values.GetLength(1);
            int nRows = values.GetLength(0);

            if (nCols != columnWidths.ElementCount || nRows != rowHeights.ElementCount)
            {
                // Dimension mismatch
                return null;
            }

            // Calculate the x-coordinate array (nCols+1 column boundaries).
            // Populate xCoords from left to right
            double[] xCoords = new double[nCols + 1];
            xCoords[0] = xMin;
            for (int i = 0; i < nCols; i++)
            {
                xCoords[i+1] = xCoords[i] + columnWidths[i + 1];
            }

            // Calculate the y-coordinate array (nRows+1 row boundaries).
            // The yCoords array contains coordinates starting at the bottom of the model (bottom of row nRows)
            // Populate yCoords from top to bottom
            double[] yCoords = new double[nRows + 1];
            yCoords[nRows] = yMax;
            for (int i = nRows - 1; i >= 0; i--)
            {
                yCoords[i] = yCoords[i + 1] - rowHeights[nRows - i];
            }
            
            // If flow values are to be converted to flux values, divide by (uniform) cell area
            if (convertFlowToFlux)
            {
                float cellArea;
                for (int i = 0; i < nRows; i++)
                {
                    for (int j = 0; j < nCols; j++)
                    {
                        cellArea = Convert.ToSingle(columnWidths[j+1] * rowHeights[i+1]);
                        values[i, j] = values[i, j] / cellArea;
                    }
                }
            }

            // Return the new GeoMap.
            return new GeoMap(values, xCoords, yCoords);
        }
        #endregion Static methods

        #region Public methods
        public PointF GetCentroidOfCell(int iRow0Base, int jCol0Base)
        {
            // If the row or column index is invalid, return a NaN point.
            int iRowInv = NRows - iRow0Base - 1;
            if (jCol0Base < 0 || jCol0Base >= NCols || iRowInv < 0 || iRowInv >= NRows)
            {
                return new PointF(float.NaN, float.NaN);
            }

            // Otherwise, return the centroid point.
            else
            {
                float x = Convert.ToSingle((_xCoords[jCol0Base] + _xCoords[jCol0Base + 1]) / 2.0f);
                float y = Convert.ToSingle((_yCoords[iRowInv] + _yCoords[iRowInv + 1]) / 2.0f);
                return new PointF(x, y);
            }
        }
        public float GetValueAtPoint(PointF samplePoint)
        {
            try
            {
                // Find the indices of the x and y coordinates.
                int xIndex = getIndex(samplePoint.X, _xCoords); // column index, 0-based
                int yIndex = getIndex(samplePoint.Y, _yCoords); // row index, 0-based

                // If either index is outside the array, return NaN.
                if (xIndex < 0 || yIndex < 0 || xIndex > (NCols - 1) || yIndex > (NRows - 1))
                {
                    return float.NaN;
                }

                // Otherwise, return the value of the specified cell.
                else
                {
                    return Convert.ToSingle(_values[NRows - yIndex - 1, xIndex]);
                }
            }
            catch
            {
                return float.NaN;
            }
        }
        public GeoMap CreateMapFromValueArray(float[,] values)
        {
            return new GeoMap(values, this._xCoords, this._yCoords);
        }
        public float GetValueAtCell(int iRow0Base, int jCol0Base)
        {
            if (iRow0Base < 0 || iRow0Base >= NRows || jCol0Base < 0 || jCol0Base >= NCols)
            {
                return float.NaN;
            }
            else
            {
                return Convert.ToSingle(_values[iRow0Base, jCol0Base]);
            }
        }
        public float[,] GetValueArray()
        {
            return _values;
        }
        public Extent GetExtent()
        {
            return _extent;
        }
        public double GetXCellBound(int i)
        {
            return this._xCoords[i];
        }
        public double GetYCellBound(int i)
        {
            return this._yCoords[i];
        }
        #endregion Public methods

        #region Private methods
        private int getIndex(double value, double[] array)
        {
            int low = 0;
            int high = 0;
            int mid = 0;

            try
            {
                // This is a modified binary search that will find the largest index for which the value at the index is less than or 
                // equal to the search value.
                low = 0;
                high = array.Length - 1;
                while (low <= high)
                {
                    mid = (low + high) / 2;

                    // This is the case for when the midpoint is too low.
                    if (array[mid] < value)
                    {
                        // If the value is contained between the mid index and index immediately following, return mid.
                        if (mid < array.Length - 1)
                        {
                            if (array[mid + 1] >= value)
                            {
                                return mid;
                            }
                            // Otherwise, move low to mid + 1.
                            else
                            {
                                low = mid + 1;
                            }
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    // This is the case for when the midpoint is too high.
                    else if (array[mid] > value)
                    {
                        // If mid is zero, return -1 (outside the array).
                        if (mid == 0)
                        {
                            return -1;
                        }

                        // If the point is contained between mid index and previous index, return mid - 1.
                        if (array[mid - 1] <= value)
                        {
                            return mid - 1;
                        }
                        // Otherwise, move high to mid - 1.
                        else
                        {
                            high = mid - 1;
                        }
                    }
                    // This is the case for when the midpoint equals the point for which we are looking. Return mid.
                    else
                    {
                        return mid;
                    }
                }

                // At this point, the value must be outside the array, so return -1.
                return -1;
            }
            catch
            {
                Console.WriteLine("low: " + low + ", high: " + high + ", mid: " + mid + ", length: " + array.Length);
                return -1;
            }
        }
        #endregion Private methods
    }
}
