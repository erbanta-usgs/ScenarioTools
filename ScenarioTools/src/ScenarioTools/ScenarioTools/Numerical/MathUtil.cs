using System;
using System.Collections.Generic;
using System.Text;

namespace ScenarioTools.Numerical
{
    public class MathUtil
    {
        public static float NanAwareMin(float value1, float value2)
        {
            // If either value is NaN, return the other value.
            if (float.IsNaN(value1))
            {
                return value2;
            }
            else if (float.IsNaN(value2))
            {
                return value1;
            }

            // Return the lesser of the two values.
            return Math.Min(value1, value2);
        }
        public static double NanAwareMin(double value1, double value2)
        {
            // If either value is NaN, return the other value.
            if (double.IsNaN(value1))
            {
                return value2;
            }
            else if (double.IsNaN(value2))
            {
                return value1;
            }

            // Return the lesser of the two values.
            return Math.Min(value1, value2);
        }
        public static float NanAwareMax(float value1, float value2)
        {
            // If either value is NaN, return the other value.
            if (float.IsNaN(value1))
            {
                return value2;
            }
            else if (float.IsNaN(value2))
            {
                return value1;
            }

            // Return the greater of the two values.
            return Math.Max(value1, value2);
        }
        public static double NanAwareMax(double value1, double value2)
        {
            // If either value is NaN, return the other value.
            if (double.IsNaN(value1))
            {
                return value2;
            }
            else if (double.IsNaN(value2))
            {
                return value1;
            }

            // Return the greater of the two values.
            return Math.Max(value1, value2);
        }

        public static float ArrayMin(float[] array)
        {
            if (array == null)
            {
                return float.NaN;
            }

            float value = float.NaN;
            for (int i = 0; i < array.Length; i++)
            {
                value = NanAwareMin(value, array[i]);
            }
            return value;
        }
        public static float ArrayMax(float[] array)
        {
            if (array == null)
            {
                return float.NaN;
            }

            float value = float.NaN;
            for (int i = 0; i < array.Length; i++)
            {
                value = NanAwareMax(value, array[i]);
            }
            return value;
        }

        public static bool Fuzzy_Equals(double A, double B, double Eps)
        {
            bool C = false;
            double epsLocal = Math.Abs(Eps);
            if (B != 0.0d)
            {
                if (Math.Abs((A - B) / B) < epsLocal)
                {
                    C = true;
                }
            }
            else
            {
                if (Math.Abs(A) < epsLocal)
                {
                    C = true;
                }
            }
            return C;
        }
        public static bool IsOdd(int value)
        {
            return value % 2 != 0;
        }
    }
}
