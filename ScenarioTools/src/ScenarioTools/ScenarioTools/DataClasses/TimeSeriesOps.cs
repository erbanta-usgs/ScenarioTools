using System;
using System.Collections.Generic;

using System.Text;
using ScenarioTools.Numerical;

namespace ScenarioTools.DataClasses
{
    public class TimeSeriesOps
    {
        public static TimeSeries Sum(TimeSeries addend1, TimeSeries addend2, TimeSeries sampleLocations)
        {
            // Make the list for the result samples.
            List<TimeSeriesSample> sumSamples = new List<TimeSeriesSample>();

            // Sample at the points in the sample locations series.
            for (int i = 0; i < sampleLocations.Length; i++)
            {
                // Get the sample point.
                DateTime samplePoint = sampleLocations[i].Date;

                // Get the value of both series at the sample point and find the sum.
                float sum = addend1.GetValue(samplePoint) + addend2.GetValue(samplePoint);

                // If the value is valid or if the difference list contains points, add the value to the list.
                if (sumSamples.Count > 0 || !float.IsNaN(sum))
                {
                    sumSamples.Add(new TimeSeriesSample(samplePoint, sum));
                }
            }

            // Remove all of the NaN samples from the end of the result series.
            while (sumSamples.Count > 0 && float.IsNaN(sumSamples[sumSamples.Count - 1].Value))
            {
                sumSamples.RemoveAt(sumSamples.Count - 1);
            }

            // Return the result as a time series.
            return new TimeSeries(sumSamples.ToArray());
        }
        public static TimeSeries Sum(TimeSeries addend1, float addend2)
        {
            // Make the list for the result samples.
            List<TimeSeriesSample> sumSamples = new List<TimeSeriesSample>();

            // Sample at the points in the time series.
            for (int i = 0; i < addend1.Length; i++)
            {
                // Get the sample point.
                DateTime samplePoint = addend1[i].Date;

                // Find the sum of the series point and the constant addend.
                float sum = addend1[i].Value + addend2;

                // If the value is valid or if the difference list contains points, add the value to the list.
                if (sumSamples.Count > 0 || !float.IsNaN(sum))
                {
                    sumSamples.Add(new TimeSeriesSample(samplePoint, sum));
                }
            }

            // Remove all of the NaN samples from the end of the result series.
            while (sumSamples.Count > 0 && float.IsNaN(sumSamples[sumSamples.Count - 1].Value))
            {
                sumSamples.RemoveAt(sumSamples.Count - 1);
            }

            // Return the result as a time series.
            return new TimeSeries(sumSamples.ToArray());
        }
        public static TimeSeries Sum(float addend1, TimeSeries addend2)
        {
            return Sum(addend2, addend1);
        }
        
        public static TimeSeries Difference(TimeSeries minuend, TimeSeries subtrahend, TimeSeries sampleLocations)
        {
            // Make the list for the result samples.
            List<TimeSeriesSample> differenceSamples = new List<TimeSeriesSample>();

            // Sample at the points in the sample locations series.
            for (int i = 0; i < sampleLocations.Length; i++)
            {
                // Get the sample point.
                DateTime samplePoint = sampleLocations[i].Date;

                // Get the value of both series at the sample point and find the difference.
                float difference = minuend.GetValue(samplePoint) - subtrahend.GetValue(samplePoint);

                // If the value is valid or if the difference list contains points, add the value to the list.
                if (differenceSamples.Count > 0 || !float.IsNaN(difference))
                {
                    differenceSamples.Add(new TimeSeriesSample(samplePoint, difference));
                }
            }

            // Remove all of the NaN samples from the end of the result series.
            while (differenceSamples.Count > 0 && float.IsNaN(differenceSamples[differenceSamples.Count - 1].Value))
            {
                differenceSamples.RemoveAt(differenceSamples.Count - 1);
            }

            // Return the result as a time series.
            return new TimeSeries(differenceSamples.ToArray());
        }
        public static TimeSeries Difference(TimeSeries minuend, float subtrahend)
        {
            // Make the list for the result samples.
            List<TimeSeriesSample> differenceSamples = new List<TimeSeriesSample>();

            // Sample at the points in the time series.
            for (int i = 0; i < minuend.Length; i++)
            {
                // Get the sample point.
                DateTime samplePoint = minuend[i].Date;

                // Find the difference of the series minuend and the constant subtrahend.
                float difference = minuend[i].Value - subtrahend;

                // If the value is valid or if the difference list contains points, add the value to the list.
                if (differenceSamples.Count > 0 || !float.IsNaN(difference))
                {
                    differenceSamples.Add(new TimeSeriesSample(samplePoint, difference));
                }
            }

            // Remove all of the NaN samples from the end of the result series.
            while (differenceSamples.Count > 0 && float.IsNaN(differenceSamples[differenceSamples.Count - 1].Value))
            {
                differenceSamples.RemoveAt(differenceSamples.Count - 1);
            }

            // Return the result as a time series.
            return new TimeSeries(differenceSamples.ToArray());
        }
        public static TimeSeries Difference(float minuend, TimeSeries subtrahend)
        {
            // Make the list for the result samples.
            List<TimeSeriesSample> differenceSamples = new List<TimeSeriesSample>();

            // Sample at the points in the time series.
            for (int i = 0; i < subtrahend.Length; i++)
            {
                // Get the sample point.
                DateTime samplePoint = subtrahend[i].Date;

                // Find the difference of the constant minuend and the series subtrahend.
                float difference = minuend - subtrahend[i].Value;

                // If the value is valid or if the difference list contains points, add the value to the list.
                if (differenceSamples.Count > 0 || !float.IsNaN(difference))
                {
                    differenceSamples.Add(new TimeSeriesSample(samplePoint, difference));
                }
            }

            // Remove all of the NaN samples from the end of the result series.
            while (differenceSamples.Count > 0 && float.IsNaN(differenceSamples[differenceSamples.Count - 1].Value))
            {
                differenceSamples.RemoveAt(differenceSamples.Count - 1);
            }

            // Return the result as a time series.
            return new TimeSeries(differenceSamples.ToArray());
        }
        
        public static TimeSeries Product(TimeSeries factor1, TimeSeries factor2, TimeSeries sampleLocations)
        {
            // Make the list for the result samples.
            List<TimeSeriesSample> productSamples = new List<TimeSeriesSample>();

            // Sample at the points in the sample locations series.
            for (int i = 0; i < sampleLocations.Length; i++)
            {
                // Get the sample point.
                DateTime samplePoint = sampleLocations[i].Date;

                // Get the value of both series at the sample point and find the difference.
                float product = factor1.GetValue(samplePoint) * factor2.GetValue(samplePoint);

                // If the value is valid or if the difference list contains points, add the value to the list.
                if (productSamples.Count > 0 || !float.IsNaN(product))
                {
                    productSamples.Add(new TimeSeriesSample(samplePoint, product));
                }
            }

            // Remove all of the NaN samples from the end of the result series.
            while (productSamples.Count > 0 && float.IsNaN(productSamples[productSamples.Count - 1].Value))
            {
                productSamples.RemoveAt(productSamples.Count - 1);
            }

            // Return the result as a time series.
            return new TimeSeries(productSamples.ToArray());
        }
        public static TimeSeries Product(TimeSeries factor1, float factor2)
        {
            // Make the list for the result samples.
            List<TimeSeriesSample> productSamples = new List<TimeSeriesSample>();

            // Sample at the points in the time series.
            for (int i = 0; i < factor1.Length; i++)
            {
                // Get the sample point.
                DateTime samplePoint = factor1[i].Date;

                // Find the product of the series factor and the constant factor.
                float product = factor1[i].Value * factor2;

                // If the value is valid or if the product list contains points, add the value to the list.
                if (productSamples.Count > 0 || !float.IsNaN(product))
                {
                    productSamples.Add(new TimeSeriesSample(samplePoint, product));
                }
            }

            // Remove all of the NaN samples from the end of the result series.
            while (productSamples.Count > 0 && float.IsNaN(productSamples[productSamples.Count - 1].Value))
            {
                productSamples.RemoveAt(productSamples.Count - 1);
            }

            // Return the result as a time series.
            return new TimeSeries(productSamples.ToArray());
        }
        public static TimeSeries Product(float factor1, TimeSeries factor2)
        {
            return Product(factor2, factor1);
        }
        
        public static TimeSeries Quotient(TimeSeries dividend, TimeSeries divisor, TimeSeries sampleLocations)
        {
            // Make the list for the result samples.
            List<TimeSeriesSample> quotientSamples = new List<TimeSeriesSample>();

            // Sample at the points in the sample locations series.
            for (int i = 0; i < sampleLocations.Length; i++)
            {
                // Get the sample point.
                DateTime samplePoint = sampleLocations[i].Date;

                // Get the value of both series at the sample point and find the difference.
                float quotient = dividend.GetValue(samplePoint) / divisor.GetValue(samplePoint);

                // If the value is valid or if the difference list contains points, add the value to the list.
                if (quotientSamples.Count > 0 || !float.IsNaN(quotient))
                {
                    quotientSamples.Add(new TimeSeriesSample(samplePoint, quotient));
                }
            }

            // Remove all of the NaN samples from the end of the result series.
            while (quotientSamples.Count > 0 && float.IsNaN(quotientSamples[quotientSamples.Count - 1].Value))
            {
                quotientSamples.RemoveAt(quotientSamples.Count - 1);
            }

            // Return the result as a time series.
            return new TimeSeries(quotientSamples.ToArray());
        }
        public static TimeSeries Quotient(TimeSeries dividend, float divisor)
        {
            Console.WriteLine("The divisor is " + divisor);

            // Make the list for the result samples.
            List<TimeSeriesSample> quotientSamples = new List<TimeSeriesSample>();

            // Sample at the points in the time series.
            for (int i = 0; i < dividend.Length; i++)
            {
                // Get the sample point.
                DateTime samplePoint = dividend[i].Date;

                // Find the quotient of the series dividend and the constant divisor.
                float quotient = dividend[i].Value / divisor;

                // If the value is valid or if the quotient list contains points, add the value to the list.
                if (quotientSamples.Count > 0 || !float.IsNaN(quotient))
                {
                    quotientSamples.Add(new TimeSeriesSample(samplePoint, quotient));
                }
            }

            // Remove all of the NaN samples from the end of the result series.
            while (quotientSamples.Count > 0 && float.IsNaN(quotientSamples[quotientSamples.Count - 1].Value))
            {
                quotientSamples.RemoveAt(quotientSamples.Count - 1);
            }

            // Return the result as a time series.
            return new TimeSeries(quotientSamples.ToArray());
        }
        public static TimeSeries Quotient(float dividend, TimeSeries divisor)
        {
            // Make the list for the result samples.
            List<TimeSeriesSample> quotientSamples = new List<TimeSeriesSample>();

            // Sample at the points in the time series.
            for (int i = 0; i < divisor.Length; i++)
            {
                // Get the sample point.
                DateTime samplePoint = divisor[i].Date;

                // Find the quotient of the constant dividend and the series divisor.
                float quotient = dividend / divisor[i].Value;

                // If the value is valid or if the quotient list contains points, add the value to the list.
                if (quotientSamples.Count > 0 || !float.IsNaN(quotient))
                {
                    quotientSamples.Add(new TimeSeriesSample(samplePoint, quotient));
                }
            }

            // Remove all of the NaN samples from the end of the result series.
            while (quotientSamples.Count > 0 && float.IsNaN(quotientSamples[quotientSamples.Count - 1].Value))
            {
                quotientSamples.RemoveAt(quotientSamples.Count - 1);
            }

            // Return the result as a time series.
            return new TimeSeries(quotientSamples.ToArray());
        }
        
        public float Minimum(TimeSeries timeSeries)
        {
            // Find the minimum value in the time series.
            float minValue = float.NaN;
            for (int i = 0; i < timeSeries.Length; i++)
            {
                minValue = MathUtil.NanAwareMin(timeSeries[i].Value, minValue);
            }

            // Return the result.
            return minValue;
        }
        public float Maximum(TimeSeries timeSeries)
        {
            // Find the maximum value in the time series.
            float maxValue = float.NaN;
            for (int i = 0; i < timeSeries.Length; i++)
            {
                maxValue = MathUtil.NanAwareMin(timeSeries[i].Value, maxValue);
            }

            // Return the result.
            return maxValue;
        }
        public float Average(TimeSeries timeSeries)
        {
            return Sum(timeSeries) / timeSeries.Length;
        }
        public float Sum(TimeSeries timeSeries)
        {
            // Find the sum of all values in the time series.
            float sum = 0.0f;
            for (int i = 0; i < timeSeries.Length; i++)
            {
                if (!float.IsNaN(timeSeries[i].Value))
                {
                    sum += timeSeries[i].Value;
                }
            }

            // Return the result.
            return sum;
        }
    }
}
