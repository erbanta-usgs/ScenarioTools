using System;
using System.Collections.Generic;
using System.Text;

namespace ScenarioTools.Numerical
{
    public class ArrayOps
    {
        public static float[] Difference(float[] minuend, float[] subtrahend)
        {
            // Determine the width and height of the result.
            int length = Math.Min(minuend.Length, subtrahend.Length);

            // Make and populate the result.
            float[] difference = new float[length];
            for (int i = 0; i < length; i++)
            {
                difference[i] = minuend[i] - subtrahend[i];
            }

            // Return the result.
            return difference;
        }
        public static float[,] Difference(float[,] minuend, float[,] subtrahend)
        {
            // Determine the width and height of the result.
            int width = Math.Min(minuend.GetLength(0), subtrahend.GetLength(0));
            int height = Math.Min(minuend.GetLength(1), subtrahend.GetLength(1));

            // Make and populate the result.
            float[,] difference = new float[width, height];
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    difference[i, j] = minuend[i, j] - subtrahend[i, j];
                }
            }

            // Return the result.
            return difference;
        }
        public static void SetArrayElements(ref int[,] array, int val)
        {
            int nrow = array.GetLength(0);
            int ncol = array.GetLength(1);
            for (int i = 0; i < nrow; i++)
            {
                for (int j = 0; j < ncol; j++)
                {
                    array[i, j] = val;
                }
            }
        }
        public static void SetArrayElements(ref double[,] array, double val)
        {
            int nrow = array.GetLength(0);
            int ncol = array.GetLength(1);
            for (int i = 0; i < nrow; i++)
            {
                for (int j = 0; j < ncol; j++)
                {
                    array[i, j] = val;
                }
            }
        }
        /// <summary>
        /// Peform element-by-element addition.  Two-argument version adds second array (array1) to first array (result).
        /// Three-argument version populates result array with sum of array1 and array2.
        /// </summary>
        /// <param name="result">Sum of [array1] and [array2]</param>
        /// <param name="array1">Array to be added</param>
        /// <param name="array2">Array to be added</param>
        public static void AddArrays(ref double[,] result, double[,] array1, double[,] array2)
        {
            // Find array dimensions
            int nrow = array1.GetLength(0);
            int ncol = array1.GetLength(1);

            // Ensure array dimensions match
            if (nrow != array2.GetLength(0))
            {
                throw new Exception("Array dimension mismatch in AddArrayElements");
            }
            if (ncol != array2.GetLength(1))
            {
                throw new Exception("Array dimension mismatch in AddArrayElements");
            }
            if (nrow != result.GetLength(0))
            {
                throw new Exception("Array dimension mismatch in AddArrayElements");
            }
            if (ncol != result.GetLength(1))
            {
                throw new Exception("Array dimension mismatch in AddArrayElements");
            }

            // Perform element-by-element addition
            for (int i = 0; i < nrow; i++)
            {
                for (int j = 0; j < ncol; j++)
                {
                    result[i, j] = array1[i, j] + array2[i, j];
                }
            }            
        }
        /// <summary>
        /// Peform element-by-element addition.  Two-argument version adds second array (array1) to first array (result).
        /// Three-argument version populates result array with sum of array1 and array2.
        /// </summary>
        /// <param name="result">On return, result contains sum of [result] + [array1]</param>
        /// <param name="array1">Array to be added to [result]</param>
        public static void AddArrays(ref double[,] result, double[,] array1)
        {
            // Find array dimensions
            int nrow = array1.GetLength(0);
            int ncol = array1.GetLength(1);

            // Ensure array dimensions match
            if (nrow != result.GetLength(0))
            {
                throw new Exception("Array dimension mismatch in AddArrayElements");
            }
            if (ncol != result.GetLength(1))
            {
                throw new Exception("Array dimension mismatch in AddArrayElements");
            }

            // Perform element-by-element addition
            for (int i = 0; i < nrow; i++)
            {
                for (int j = 0; j < ncol; j++)
                {
                    result[i, j] = result[i, j] + array1[i, j];
                }
            }
        }
        public static void MultiplyArrayByScalar(ref double[,] result, double[,] array1, double multiplier)
        {
            // Find array dimensions
            int nrow = array1.GetLength(0);
            int ncol = array1.GetLength(1);

            // Ensure array dimensions match
            if (nrow != result.GetLength(0))
            {
                throw new Exception("Array dimension mismatch in MultArrayScalar");
            }
            if (ncol != result.GetLength(1))
            {
                throw new Exception("Array dimension mismatch in MultArrayScalar");
            }

            // Perform multiplication on each element
            for (int i = 0; i < nrow; i++)
            {
                for (int j = 0; j < ncol; j++)
                {
                    result[i, j] = array1[i, j] * multiplier;
                }
            }
        }
        public static void ShowMinAndMax(float[,] array)
        {
            float min = float.NaN;
            float max = float.NaN;

            int nCols = array.GetLength(0);
            int nRows = array.GetLength(1);
            for (int j = 0; j < nRows; j++)
            {
                for (int i = 0; i < nCols; i++)
                {
                    min = MathUtil.NanAwareMin(min, array[i, j]);
                    max = MathUtil.NanAwareMax(max, array[i, j]);
                }
            }

            Console.WriteLine("min: " + min + " -- max: " + max);
        }
        public static bool IsConstant(double[,] array, ref double constantValue)
        {
            double value = array[0, 0];
            int n = array.GetLength(0);
            int m = array.GetLength(1);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (!array[i, j].Equals(value))
                    {
                        return false;
                    }
                }
            }
            constantValue = value;
            return true;
        }
        public static bool IsConstant(int[,] array, ref int constantValue)
        {
            int value = array[0, 0];
            int n = array.GetLength(0);
            int m = array.GetLength(1);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (!array[i, j].Equals(value))
                    {
                        return false;
                    }
                }
            }
            constantValue = value;
            return true;
        }
    }
}
