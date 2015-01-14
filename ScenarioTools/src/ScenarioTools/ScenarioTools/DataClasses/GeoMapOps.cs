using System;
using System.Collections.Generic;

using System.Text;
using System.Drawing;

namespace ScenarioTools.DataClasses
{
    public class GeoMapOps
    {
        public static GeoMap Sum(GeoMap addend1, GeoMap addend2, GeoMap sampleGrid)
        {
            // Make the result value array.
            int nCols = sampleGrid.NCols;
            int nRows = sampleGrid.NRows;
            float[,] values = new float[nRows, nCols];

            // Query both grids at the locations specified by the sample grid and store the result in the values array.
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    PointF samplePoint = sampleGrid.GetCentroidOfCell(i, j);
                    values[i, j] = addend1.GetValueAtPoint(samplePoint) + addend2.GetValueAtPoint(samplePoint);
                }
            }

            // Return a map with the values from the value array and the spatial referencing of the sample grid.
            return sampleGrid.CreateMapFromValueArray(values);
        }
        public static GeoMap Difference(GeoMap minuend, GeoMap subtrahend, GeoMap sampleGrid)
        {
            // Make the result value array.
            int nCols = sampleGrid.NCols;
            int nRows = sampleGrid.NRows;
            float[,] values = new float[nRows, nCols];

            // Query both grids at the locations specified by the sample grid and store the result in the values array.
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    PointF samplePoint = sampleGrid.GetCentroidOfCell(i, j);
                    values[i, j] = minuend.GetValueAtPoint(samplePoint) - subtrahend.GetValueAtPoint(samplePoint);
                }
            }

            // Return a map with the values from the value array and the spatial referencing of the sample grid.
            return sampleGrid.CreateMapFromValueArray(values);
        }
        public static GeoMap Product(GeoMap factor1, GeoMap factor2, GeoMap sampleGrid)
        {
            // Make the result value array.
            int nCols = sampleGrid.NCols;
            int nRows = sampleGrid.NRows;
            float[,] values = new float[nRows, nCols];

            // Query both grids at the locations specified by the sample grid and store the result in the values array.
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    PointF samplePoint = sampleGrid.GetCentroidOfCell(i, j);
                    values[i, j] = factor1.GetValueAtPoint(samplePoint) * factor2.GetValueAtPoint(samplePoint);
                }
            }

            // Return a map with the values from the value array and the spatial referencing of the sample grid.
            return sampleGrid.CreateMapFromValueArray(values);
        }
        public static GeoMap Quotient(GeoMap dividend, GeoMap divisor, GeoMap sampleGrid)
        {
            // Make the result value array.
            int nCols = sampleGrid.NCols;
            int nRows = sampleGrid.NRows;
            float[,] values = new float[nRows, nCols];

            // Query both grids at the locations specified by the sample grid and store the result in the values array.
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    PointF samplePoint = sampleGrid.GetCentroidOfCell(i, j);
                    values[i, j] = dividend.GetValueAtPoint(samplePoint) / divisor.GetValueAtPoint(samplePoint);
                }
            }

            // Return a map with the values from the value array and the spatial referencing of the sample grid.
            return sampleGrid.CreateMapFromValueArray(values);
        }

        public static GeoMap Sum(GeoMap addend1, float addend2)
        {
            // Make the result value array.
            int nCols = addend1.NCols;
            int nRows = addend1.NRows;
            float[,] values = new float[nRows, nCols];

            // Calculate the result and store in the values array.
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    values[i, j] = addend1.GetValueAtCell(i, j) + addend2;
                }
            }

            // Return a map with the values from the value array and the spatial referencing of the sample grid.
            return addend1.CreateMapFromValueArray(values);
        }
        public static GeoMap Difference(GeoMap minuend, float subtrahend)
        {
            // Make the result value array.
            int nCols = minuend.NCols;
            int nRows = minuend.NRows;
            float[,] values = new float[nRows, nCols];

            // Calculate the result and store in the values array.
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    values[i, j] = minuend.GetValueAtCell(i, j) - subtrahend;
                }
            }

            // Return a map with the values from the value array and the spatial referencing of the sample grid.
            return minuend.CreateMapFromValueArray(values);
        }
        public static GeoMap Product(GeoMap factor1, float factor2)
        {
            // Make the result value array.
            int nCols = factor1.NCols;
            int nRows = factor1.NRows;
            float[,] values = new float[nRows, nCols];

            // Calculate the result and store in the values array.
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    values[i, j] = factor1.GetValueAtCell(i, j) * factor2;
                }
            }

            // Return a map with the values from the value array and the spatial referencing of the sample grid.
            return factor1.CreateMapFromValueArray(values);
        }
        public static GeoMap Quotient(GeoMap dividend, float divisor)
        {
            // Make the result value array.
            int nCols = dividend.NCols;
            int nRows = dividend.NRows;
            float[,] values = new float[nRows, nCols];

            // Calculate the result and store in the values array.
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    values[i, j] = dividend.GetValueAtCell(i, j) / divisor;
                }
            }

            // Return a map with the values from the value array and the spatial referencing of the sample grid.
            return dividend.CreateMapFromValueArray(values);
        }

        public static GeoMap Sum(float addend1, GeoMap addend2)
        {
            return Sum(addend2, addend1);
        }
        public static GeoMap Difference(float minuend, GeoMap subtrahend)
        {
            // Make the result value array.
            int nCols = subtrahend.NCols;
            int nRows = subtrahend.NRows;
            float[,] values = new float[nRows, nCols];

            // Calculate the result and store in the values array.
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    values[i, j] = minuend - subtrahend.GetValueAtCell(i, j);
                }
            }

            // Return a map with the values from the value array and the spatial referencing of the sample grid.
            return subtrahend.CreateMapFromValueArray(values);
        }
        public static GeoMap Product(float factor1, GeoMap factor2)
        {
            return Product(factor2, factor1);
        }
        public static GeoMap Quotient(float dividend, GeoMap divisor)
        {
            // Make the result value array.
            int nCols = divisor.NCols;
            int nRows = divisor.NRows;
            float[,] values = new float[nRows, nCols];

            // Calculate the result and store in the values array.
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    values[i, j] = dividend / divisor.GetValueAtCell(i, j);
                }
            }

            // Return a map with the values from the value array and the spatial referencing of the sample grid.
            return divisor.CreateMapFromValueArray(values);
        }
    }
}
