using System;

using ScenarioTools.Geometry;

namespace ScenarioTools.ModflowReaders
{
    public class ModelLayer
    {
        DiscretizationFile dis;
        double[,] top, bottom, confiningBed;

        public ModelLayer(DiscretizationFile dis, double[,] top, double[,] bottom, double[,] confiningBed)
        {
            // Store references.
            this.top = top;
            this.bottom = bottom;
            this.confiningBed = confiningBed;
        }
        public bool hasConfiningBed()
        {
            return confiningBed != null;
        }
        public double[,] getTop()
        {
            return top;
        }
        public double[,] getBottom()
        {
            return bottom;
        }
        public double[,] getConfiningBed()
        {
            return confiningBed;
        }
        public double getCellThickness(int i, int j)
        {
            return top[i, j] - bottom[i, j];
        }

        /// <summary>
        /// Return cell height
        /// </summary>
        /// <param name="row">1-based row index</param>
        /// <param name="column">1-based column index</param>
        /// <returns>Height of Modflow cell (row, column)</returns>
        public double GetCellHeight(int row, int column)
        {
            int i = column - 1;
            int j = row - 1;
            try
            {
                return getCellThickness(i, j);
            }
            catch
            {
                return double.NaN;
            }
        }
        public double getTop(Point2D location)
        {
            throw new NotImplementedException();
            // Get the x and y cells of the location.
            int x = 0; // dis.getXCellIndex(location.getX());
            int y = 0; // dis.getYCellIndex(location.getY());

            // If either of the indices are invalid, return NaN.
            if (x < 0 || y < 0)
                return Double.NaN;

            // Return the value of the top array at the appropriate cell.
            return top[x, y];
        }

        /// <summary>
        /// Return cell top elevation
        /// </summary>
        /// <param name="row">1-based row index</param>
        /// <param name="column">1-based column index</param>
        /// <returns></returns>
        public double getTop(int row, int column)
        {
            try
            {
                int i = row - 1;
                int j = column - 1;
                return top[i, j];
            }
            catch
            {
                return double.NaN;
            }
        }
        public double getBottom(Point2D location)
        {
            throw new NotImplementedException();
            // Get the x and y cells of the location.
            int x = 0; // dis.getXCellIndex(location.getX());
            int y = 0; // dis.getYCellIndex(location.getY());

            // If either of the indices are invalid, return NaN.
            if (x < 0 || y < 0)
            {
                return double.NaN;
            }

            // Return the value of the bottom array at the appropriate cell.
            return bottom[x, y];
        }

        /// <summary>
        /// Return cell bottom elevation
        /// </summary>
        /// <param name="row">1-based row index</param>
        /// <param name="column">1-based column index</param>
        /// <returns></returns>
        public double getBottom(int row, int column)
        {
            try
            {
                int i = row - 1;
                int j = column - 1;
                return bottom[i, j];
            }
            catch
            {
                return double.NaN;
            }
        }
        public double getConfiningBed(Point2D location)
        {
            // If the confining bed is null, return NaN.
            if (confiningBed == null)
                return double.NaN;

            // Get the x and y cells of the location.
            int x = 0; // dis.getXCellIndex(location.getX());
            int y = 0; // dis.getYCellIndex(location.getY());

            // If either of the indices are invalid, return NaN.
            if (x < 0 || y < 0)
            {
                return double.NaN;
            }

            // Return the value of the top array at the appropriate cell.
            return confiningBed[x, y];
        }
    }
}
