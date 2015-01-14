using System;

using ScenarioTools.DataClasses;

namespace ScenarioTools.ModflowReaders
{
    public class StressPeriodHelper
    {
        /// <summary>
        /// Compute time-averaged value from a single-identifier time series for specified stress period
        /// </summary>
        /// <param name="stressPeriod"></param>
        /// <param name="perStartDateTime"></param>
        /// <param name="smpSeries">Times series containing values for a single identifier</param>
        /// <param name="modflowTimeUnit"></param>
        /// <param name="timeSeriesInterpretationMethod"></param>
        /// <returns></returns>
        public static double GetTimeAveragedValueForStressPeriod(StressPeriod stressPeriod, DateTime perStartDateTime,
            SmpSeries smpSeries, ModflowTimeUnit modflowTimeUnit, 
            TimeSeriesInterpretationMethod timeSeriesInterpretationMethod)
        {
            // This code is largely copied from WellCell.PumpRate.
            double volume = 0.0d; // cumulative volume pumped
            double rate = 0.0d;
            try
            {
                long ticks;
                double timeLen;
                int indxRecLastPreceding = -1;
                int indxFirstRecInStressPeriod = -1;
                int indxLastRecInStressPeriod = -1;
                int indxFirstRecAfterStressPeriod = -1;
                int countRecsInStressPeriod = 0;
                DateTime perEndDateTime = perStartDateTime + stressPeriod.getTimeSpan();
                DateTime currentDateTime;
                // Find index of last SMP record preceding stress period start, 
                // index of first record in stress period, index of last record
                // in stress period, and index of first record after stress period.
                // 
                if (smpSeries.Records.Count > 0)
                {
                    for (int i = 0; i < smpSeries.Records.Count; i++)
                    {
                        currentDateTime = smpSeries.Records[i].DateTime;
                        if (currentDateTime < perStartDateTime)
                        {
                            indxRecLastPreceding = i;
                        }
                        if (indxFirstRecInStressPeriod == -1)
                        {
                            if (currentDateTime >= perStartDateTime)
                            {
                                indxFirstRecInStressPeriod = i;
                            }
                        }
                        if (currentDateTime <= perEndDateTime)
                        {
                            indxLastRecInStressPeriod = i;
                            if (currentDateTime >= perStartDateTime)
                            {
                                countRecsInStressPeriod++;
                            }
                        }
                        if (currentDateTime > perEndDateTime)
                        {
                            indxFirstRecAfterStressPeriod = i;
                            break;
                        }
                    }
                }
                else
                {
                    return Double.NaN;
                }

                double valueStart, valueEnd;
                DateTime dtPreceding, dtFollowing;
                double valuePreceding, valueFollowing;
                //Find rate (and associated time) preceding stress-period start
                if (indxRecLastPreceding > -1)
                {
                    dtPreceding = smpSeries.Records[indxRecLastPreceding].DateTime;
                    valuePreceding = smpSeries.Records[indxRecLastPreceding].Value;
                }
                else
                {
                    dtPreceding = new DateTime();
                    valuePreceding = 0.0d;
                }
                valueEnd = smpSeries.Records[indxFirstRecInStressPeriod].Value;
                valueStart = TimeHelper.Interpolate(dtPreceding,
                    smpSeries.Records[indxFirstRecInStressPeriod].DateTime, perStartDateTime,
                    valuePreceding, valueEnd);
                ticks = smpSeries.Records[indxFirstRecInStressPeriod].DateTime.Ticks
                            - perStartDateTime.Ticks;
                timeLen = TimeHelper.GetTimeLength(ticks, modflowTimeUnit);
                switch (timeSeriesInterpretationMethod)
                {
                    case TimeSeriesInterpretationMethod.Piecewise:
                        volume = volume + ((valueStart + valueEnd) / 2.0d) * timeLen;
                        break;
                    case TimeSeriesInterpretationMethod.Stepwise:
                        volume = volume + valueStart * timeLen;
                        break;
                }
                //
                for (int i = indxFirstRecInStressPeriod; i < indxLastRecInStressPeriod; i++)
                {
                    valueStart = smpSeries.Records[i].Value;
                    valueEnd = smpSeries.Records[i + 1].Value;
                    ticks = smpSeries.Records[i + 1].DateTime.Ticks
                                - smpSeries.Records[i].DateTime.Ticks;
                    timeLen = TimeHelper.GetTimeLength(ticks, modflowTimeUnit);
                    switch (timeSeriesInterpretationMethod)
                    {
                        case TimeSeriesInterpretationMethod.Piecewise:
                            volume = volume + ((valueStart + valueEnd) / 2.0d) * timeLen;
                            break;
                        case TimeSeriesInterpretationMethod.Stepwise:
                            volume = volume + valueStart * timeLen;
                            break;
                    }
                }
                //
                if (indxFirstRecAfterStressPeriod > indxLastRecInStressPeriod)
                {
                    dtFollowing = smpSeries.Records[indxFirstRecAfterStressPeriod].DateTime;
                    valueFollowing = smpSeries.Records[indxFirstRecAfterStressPeriod].Value;
                }
                else
                {
                    dtFollowing = new DateTime();
                    valueFollowing = 0.0d;
                }
                valueStart = smpSeries.Records[indxLastRecInStressPeriod].Value;
                valueEnd = TimeHelper.Interpolate(
                    smpSeries.Records[indxLastRecInStressPeriod].DateTime,
                    dtFollowing, perEndDateTime, valueStart, valueFollowing);
                ticks = perEndDateTime.Ticks
                            - smpSeries.Records[indxLastRecInStressPeriod].DateTime.Ticks;
                timeLen = TimeHelper.GetTimeLength(ticks, modflowTimeUnit);
                switch (timeSeriesInterpretationMethod)
                {
                    case TimeSeriesInterpretationMethod.Piecewise:
                        volume = volume + ((valueStart + valueEnd) / 2.0d) * timeLen;
                        break;
                    case TimeSeriesInterpretationMethod.Stepwise:
                        volume = volume + valueStart * timeLen;
                        break;
                }
                //
                rate = (volume / stressPeriod.getPerlen());
                return rate;
            }
            catch
            {
                throw new Exception("Error in StressPeriodHelper.GetRateForStressPeriod");
            }
        }
    }

}
