using System;
using System.Collections.Generic;

using System.Text;
using ScenarioTools.Numerical;

namespace ScenarioTools.DataClasses
{
    public class TimeSeries
    {
        private TimeSeriesSample[] samples;
        private float minValue;
        private float maxValue;
        private long[] dateTicks;

        public TimeSeries(TimeSeriesSample[] samples)
        {
            this.samples = samples;
            this.minValue = float.NaN;
            this.maxValue = float.NaN;
        }
        public int Length
        {
            get
            {
                return samples.Length;
            }
        }
        public TimeSeriesSample this[int index]
        {
            get
            {
                if (samples.Length == 0)
                {
                    return new TimeSeriesSample();
                }
                else
                {
                    return samples[index];
                }
            }
        }

        public float GetMinValue()
        {
            if (float.IsNaN(minValue))
            {
                for (int i = 0; i < samples.Length; i++)
                {
                    minValue = MathUtil.NanAwareMin(minValue, samples[i].Value);
                }
            }
            return minValue;
        }

        public float GetMaxValue()
        {
            if (float.IsNaN(maxValue))
            {
                for (int i = 0; i < samples.Length; i++)
                {
                    maxValue = MathUtil.NanAwareMax(maxValue, samples[i].Value);
                }
            }
            return maxValue;
        }

        public float GetValue(DateTime date)
        {
            // Get the index of the date in the array.
            int index = getIndex(date);

            // If the index is valid, return the appropriate value.
            if (index >= 0 && index < this.Length)
            {
                // If the date is right on the index, return the value at that index.
                if (date.Ticks == samples[index].Date.Ticks)
                {
                    return samples[index].Value;
                }
                // This is the case for when the date is after the date at the index.
                else
                {
                    // If the index is the last in the series, return NaN (the date is after the end of the series).
                    if (date.Ticks > samples[samples.Length - 1].Date.Ticks)
                    {
                        return float.NaN;
                    }
                    // Otherwise, return the interpolation of the points.
                    else
                    {
                        double r2 = (date.Ticks - samples[index].Date.Ticks) / (double)(samples[index + 1].Date.Ticks - samples[index].Date.Ticks);
                        double r1 = 1.0 - r2;
                        return (float)(r1 * samples[index].Value + r2 * samples[index + 1].Value);
                    }
                }
            }

            // Otherwise, return NaN.
            else
            {
                return float.NaN;
            }
        }

        
        private int getIndex(DateTime date)
        {
            int low = 0;
            int high = 0;
            int mid = 0;

            try
            {
                // If the ticks array is null, make it.
                if (dateTicks == null)
                {
                    dateTicks = new long[this.Length];
                    for (int i = 0; i < this.Length; i++)
                    {
                        dateTicks[i] = this[i].Date.Ticks;
                    }
                }

                // Get the ticks of the comparison date.
                long ticks = date.Ticks;

                // This is a modified binary search that will find the largest index for which the value at the index is less than the search value.
                low = 0;
                high = dateTicks.Length - 1;
                while (low <= high)
                {
                    mid = (low + high) / 2;

                    // This is the case for when the midpoint is too low.
                    if (dateTicks[mid] < ticks)
                    {
                        // If the point is contained between the mid index and index immediately following, return mid.
                        if (mid < dateTicks.Length - 1)
                        {
                            if (dateTicks[mid + 1] >= ticks)
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
                    else if (dateTicks[mid] > ticks)
                    {
                        // If the point is contained between mid index and previous index, return mid - 1.
                        if (dateTicks[mid - 1] <= ticks)
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
                //Console.WriteLine("low: " + low + ", high: " + high + ", mid: " + mid + ", length: " + dateTicks.Length);
                return -1;
            }
        }
    }
}
