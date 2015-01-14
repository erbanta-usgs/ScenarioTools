using System;

using ScenarioTools.DataClasses;
using ScenarioTools.ModflowReaders;
using USGS.Puma.FiniteDifference;

namespace ScenarioTools.Scene
{
    public class WellCell : ScenarioFeature
    {
        // Ned TODO: Implement (overide abstract) Name 
        // property (string) and field to hold key item 
        // (e.g. WellName) value.

        #region Properties

        public double FractionOfOpenLength { get; set; }

        #endregion Properties

        #region Constructors

        public WellCell()
        {
            _type = PackageType.WellType;
            _lay = 0;
            _row = 0;
            _col = 0;
            FractionOfOpenLength = 1.0;
        }

        public WellCell(int row, int col) : this()
        {
            _row = row;
            _col = col;
        }

        public WellCell(int lay, int row, int col) : this()
        {
            _lay = lay;
            _row = row;
            _col = col;
        }

        public WellCell(GridCell gridCell) : this()
        {
            _lay = gridCell.Layer;
            _row = gridCell.Row;
            _col = gridCell.Column;
        }

        #endregion Constructors

        #region Methods

        public override object Clone()
        {
            WellCell well = new WellCell();
            well._lay = _lay;
            well._col = _col;
            well._row = _row;
            well._name = _name;
            return well;
        }

        public override void Draw()
        {
        }

        /// <summary>
        /// Returns average pump rate over specified stress period
        /// </summary>
        /// <param name="stressPeriod"></param>
        /// <param name="periodStartDateTime"></param>
        /// <returns></returns>
        public double PumpRate(StressPeriod stressPeriod, DateTime perStartDateTime, 
                               ModflowTimeUnit modflowTimeUnit)
        {
            double Q = 0.0d;
            double volume = 0.0d; // cumulative volume pumped
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
                if (SmpSeries.Records.Count > 0)
                {
                    for (int i = 0; i < SmpSeries.Records.Count; i++)
                    {
                        currentDateTime = SmpSeries.Records[i].DateTime;
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

                double valueStart, valueEnd;
                DateTime dtPreceding, dtFollowing;
                double valuePreceding, valueFollowing;
                //Find rate (and associated time) preceding stress-period start
                if (indxRecLastPreceding > -1)
                {
                    dtPreceding = SmpSeries.Records[indxRecLastPreceding].DateTime;
                    valuePreceding = SmpSeries.Records[indxRecLastPreceding].Value;
                }
                else
                {
                    dtPreceding = new DateTime();
                    valuePreceding = 0.0d;
                }
                valueEnd = SmpSeries.Records[indxFirstRecInStressPeriod].Value;
                valueStart = TimeHelper.Interpolate(dtPreceding,
                    SmpSeries.Records[indxFirstRecInStressPeriod].DateTime, perStartDateTime,
                    valuePreceding, valueEnd);
                ticks = SmpSeries.Records[indxFirstRecInStressPeriod].DateTime.Ticks 
                            - perStartDateTime.Ticks;
                timeLen = TimeHelper.GetTimeLength(ticks, modflowTimeUnit);
                switch (InterpretationMethod)
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
                    valueStart = SmpSeries.Records[i].Value;
                    valueEnd = SmpSeries.Records[i + 1].Value;
                    ticks = SmpSeries.Records[i + 1].DateTime.Ticks 
                                - SmpSeries.Records[i].DateTime.Ticks;
                    timeLen = TimeHelper.GetTimeLength(ticks,modflowTimeUnit);
                    switch (InterpretationMethod)
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
                    dtFollowing = SmpSeries.Records[indxFirstRecAfterStressPeriod].DateTime;
                    valueFollowing = SmpSeries.Records[indxFirstRecAfterStressPeriod].Value;
                }
                else
                {
                    dtFollowing = new DateTime();
                    valueFollowing = 0.0d;
                }
                valueStart = SmpSeries.Records[indxLastRecInStressPeriod].Value;
                valueEnd = TimeHelper.Interpolate(
                    SmpSeries.Records[indxLastRecInStressPeriod].DateTime, 
                    dtFollowing, perEndDateTime, valueStart, valueFollowing);
                ticks = perEndDateTime.Ticks 
                            - SmpSeries.Records[indxLastRecInStressPeriod].DateTime.Ticks;
                timeLen = TimeHelper.GetTimeLength(ticks,modflowTimeUnit);
                switch (InterpretationMethod)
                {
                    case TimeSeriesInterpretationMethod.Piecewise:
                        volume = volume + ((valueStart + valueEnd) / 2.0d) * timeLen;
                        break;
                    case TimeSeriesInterpretationMethod.Stepwise:
                        volume = volume + valueStart * timeLen;
                        break;
                }
                //
                Q = FractionOfOpenLength * ( volume / stressPeriod.getPerlen() );
            }
            catch
            {
            }
            return Q;
        }

        #endregion Methods
    }
}
