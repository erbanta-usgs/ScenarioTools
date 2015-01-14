using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScenarioTools.DataClasses
{

    public enum ModflowTimeUnit
    {
        Undefined = 0,
        Seconds = 1,
        Minutes = 2,
        Hours = 3,
        Days = 4,
        Years = 5
    }

    public class TimeHelper
    {
        /// <summary>
        /// Convert a length of time to a time span using a Modflow Time Unit
        /// </summary>
        /// <param name="timeLength"></param>
        /// <param name="modflowTimeUnit"></param>
        /// <returns></returns>
        public static TimeSpan GetTimeSpan(double timeLength, ModflowTimeUnit modflowTimeUnit)
        {
            long ticks;
            long ticksPerYear = TimeSpan.TicksPerDay * 365 + TimeSpan.TicksPerHour * 6;
            switch (modflowTimeUnit)
            {
                case ModflowTimeUnit.Seconds:
                    ticks = Convert.ToInt64(timeLength * Convert.ToDouble(TimeSpan.TicksPerSecond));
                    break;
                case ModflowTimeUnit.Minutes:
                    ticks = Convert.ToInt64(timeLength * Convert.ToDouble(TimeSpan.TicksPerMinute));
                    break;
                case ModflowTimeUnit.Hours:
                    ticks = Convert.ToInt64(timeLength * Convert.ToDouble(TimeSpan.TicksPerHour));
                    break;
                case ModflowTimeUnit.Days:
                    ticks = Convert.ToInt64(timeLength * Convert.ToDouble(TimeSpan.TicksPerDay));
                    break;
                case ModflowTimeUnit.Years:
                    ticks = Convert.ToInt64(timeLength * Convert.ToDouble(ticksPerYear));
                    break;
                default:
                    ticks = 0;
                    break;
            }
            return new TimeSpan(ticks);
        }

        public static double GetTimeLength(TimeSpan timeSpan, ModflowTimeUnit modflowTimeUnit)
        {
            long ticks;
            long ticksPerYear = TimeSpan.TicksPerDay * 365 + TimeSpan.TicksPerHour * 6;
            double timeLength = 0.0d;
            switch (modflowTimeUnit)
            {
                case ModflowTimeUnit.Seconds:
                    timeLength = Convert.ToDouble(timeSpan.Ticks) / Convert.ToDouble(TimeSpan.TicksPerSecond);
                    break;
                case ModflowTimeUnit.Minutes:
                    timeLength = Convert.ToDouble(timeSpan.Ticks) / Convert.ToDouble(TimeSpan.TicksPerMinute);
                    break;
                case ModflowTimeUnit.Hours:
                    timeLength = Convert.ToDouble(timeSpan.Ticks) / Convert.ToDouble(TimeSpan.TicksPerHour);
                    break;
                case ModflowTimeUnit.Days:
                    timeLength = Convert.ToDouble(timeSpan.Ticks) / Convert.ToDouble(TimeSpan.TicksPerDay);
                    break;
                case ModflowTimeUnit.Years:
                    timeLength = Convert.ToDouble(timeSpan.Ticks) / Convert.ToDouble(ticksPerYear);
                    break;
                default:
                    break;
            }
            return timeLength;
        }

        public static double GetTimeLength(long ticks, ModflowTimeUnit modflowTimeUnit)
        {
            long ticksPerYear = TimeSpan.TicksPerDay * 365 + TimeSpan.TicksPerHour * 6;
            double timeLength = 0.0d;
            switch (modflowTimeUnit)
            {
                case ModflowTimeUnit.Seconds:
                    timeLength = Convert.ToDouble(ticks) / Convert.ToDouble(TimeSpan.TicksPerSecond);
                    break;
                case ModflowTimeUnit.Minutes:
                    timeLength = Convert.ToDouble(ticks) / Convert.ToDouble(TimeSpan.TicksPerMinute);
                    break;
                case ModflowTimeUnit.Hours:
                    timeLength = Convert.ToDouble(ticks) / Convert.ToDouble(TimeSpan.TicksPerHour);
                    break;
                case ModflowTimeUnit.Days:
                    timeLength = Convert.ToDouble(ticks) / Convert.ToDouble(TimeSpan.TicksPerDay);
                    break;
                case ModflowTimeUnit.Years:
                    timeLength = Convert.ToDouble(ticks) / Convert.ToDouble(ticksPerYear);
                    break;
                default:
                    break;
            }
            return timeLength;
        }

        public static ModflowTimeUnit GetModflowTimeUnit(int itmuni)
        {
            switch (itmuni)
            {
                case 0:
                    return ModflowTimeUnit.Undefined;
                case 1:
                    return ModflowTimeUnit.Seconds;
                case 2:
                    return ModflowTimeUnit.Minutes;
                case 3:
                    return ModflowTimeUnit.Hours;
                case 4:
                    return ModflowTimeUnit.Days;
                case 5:
                    return ModflowTimeUnit.Years;
                default:
                    return ModflowTimeUnit.Undefined;
            }
        }

        /// <summary>
        /// Interpolate a value at a time of interest in a time interval.
        /// If either intervalStartTime or intervalEndTime equals default DateTime(),
        /// the value at the other interval end is returned.  If intervalStartTime
        /// equals intervalEndTime, the returned value is the average of the two
        /// input values.
        /// </summary>
        /// <param name="intervalStartTime"></param>
        /// <param name="intervalEndTime"></param>
        /// <param name="valueTime">Time of interest</param>
        /// <param name="startValue">Value at intervalStartTime</param>
        /// <param name="endValue">Value at intervalEndTime</param>
        /// <returns></returns>
        public static double Interpolate(DateTime intervalStartTime, DateTime intervalEndTime, 
                                         DateTime valueTime, double startValue, double endValue)
        {
            DateTime defaultTime = new DateTime();
            double value = 0.0d;
            if (intervalStartTime == intervalEndTime)
            {
                value = (startValue + endValue) / 2.0d;
            }
            else if (intervalStartTime == defaultTime)
            {
                value = endValue;
            }
            else if (intervalEndTime == defaultTime)
            {
                value = startValue;
            }
            else
            {
                double valueTimeTicks = Convert.ToDouble(valueTime.Ticks);
                double intervalStartTimeTicks = Convert.ToDouble(intervalStartTime.Ticks);
                double intervalEndTimeTicks = Convert.ToDouble(intervalEndTime.Ticks);
                double timeRatio = ((valueTimeTicks - intervalStartTimeTicks) /
                                   (intervalEndTimeTicks - intervalStartTimeTicks));
                value = startValue + timeRatio * (endValue - startValue);
            }
            return value;
        }
        public static DateTime DefaultDateTime()
        {
            return new DateTime(2000, 1, 1, 0, 0, 0);
        }
    }
}
