using System;
using ScenarioTools.DataClasses;
using ScenarioTools.ModflowReaders;
using USGS.Puma.FiniteDifference;

namespace ScenarioTools.Scene
{
    public class GhbCell : AreaFeature
    {
        #region Fields
        double _conductance;
        #endregion Fields

        #region Constructors
        public GhbCell()
        {
            _type = PackageType.GhbType;
            _lay = 0;
            _row = 0;
            _col = 0;
            _conductance = 0.0;
        }
        public GhbCell(int row, int col) : this()
        {
            _row = row;
            _col = col;
        }
        public GhbCell(int lay, int row, int col) : this()
        {
            _lay = lay;
            _row = row;
            _col = col;
        }
        public GhbCell(GridCell gridCell)
            : this()
        {
            _lay = gridCell.Layer;
            _row = gridCell.Row;
            _col = gridCell.Column;
        }
        #endregion Constructors

        #region Properties
        public double Conductance
        {
            get
            {
                return _conductance;
            }
            set
            {
                if (value < 0.0f)
                {
                    _conductance = 0.0f;
                }
                else
                {
                    _conductance = value;
                }
            }
        }
        #endregion Properties

        #region Methods
        public override object Clone()
        {
            GhbCell ghbCell = new GhbCell();
            ghbCell._lay = _lay;
            ghbCell._col = _col;
            ghbCell._row = _row;
            ghbCell._name = _name;
            ghbCell._conductance = _conductance;
            return ghbCell;
        }
        public override void Draw()
        {
        }

        /// <summary>
        /// Returns average GHB boundary head over specified stress period, 
        /// taking into account the time-series interpretation method.
        /// </summary>
        /// <param name="stressPeriod"></param>
        /// <param name="periodStartDateTime"></param>
        /// <returns></returns>
        public double GhbHead(StressPeriod stressPeriod, DateTime perStartDateTime,
                               ModflowTimeUnit modflowTimeUnit)
        {
            // This method is based on WellCell.PumpRate
            double bHead = 0.0d;
            double cumHeadTime = 0.0d; // cumulative sum of bHead * (time increment)
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

                //Find head (and associated time) preceding stress-period start
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
                        cumHeadTime = cumHeadTime + ((valueStart + valueEnd) / 2.0d) * timeLen;
                        break;
                    case TimeSeriesInterpretationMethod.Stepwise:
                        cumHeadTime = cumHeadTime + valueStart * timeLen;
                        break;
                }
                
                for (int i = indxFirstRecInStressPeriod; i < indxLastRecInStressPeriod; i++)
                {
                    valueStart = SmpSeries.Records[i].Value;
                    valueEnd = SmpSeries.Records[i + 1].Value;
                    ticks = SmpSeries.Records[i + 1].DateTime.Ticks
                                - SmpSeries.Records[i].DateTime.Ticks;
                    timeLen = TimeHelper.GetTimeLength(ticks, modflowTimeUnit);
                    switch (InterpretationMethod)
                    {
                        case TimeSeriesInterpretationMethod.Piecewise:
                            cumHeadTime = cumHeadTime + ((valueStart + valueEnd) / 2.0d) * timeLen;
                            break;
                        case TimeSeriesInterpretationMethod.Stepwise:
                            cumHeadTime = cumHeadTime + valueStart * timeLen;
                            break;
                    }
                }
                
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
                timeLen = TimeHelper.GetTimeLength(ticks, modflowTimeUnit);
                switch (InterpretationMethod)
                {
                    case TimeSeriesInterpretationMethod.Piecewise:
                        cumHeadTime = cumHeadTime + ((valueStart + valueEnd) / 2.0d) * timeLen;
                        break;
                    case TimeSeriesInterpretationMethod.Stepwise:
                        cumHeadTime = cumHeadTime + valueStart * timeLen;
                        break;
                }
                
                bHead = cumHeadTime / stressPeriod.getPerlen();
            }
            catch
            {
            }
            return bHead;
        }

        #endregion Methods
    }
}
