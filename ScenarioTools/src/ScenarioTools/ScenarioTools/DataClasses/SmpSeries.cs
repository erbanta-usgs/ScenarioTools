using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ScenarioTools.Util;

namespace ScenarioTools.DataClasses
{
    public enum TimeSeriesInterpretationMethod
    // Designates method used to determine value at a time intermediate between 
    // times of adjacent SmpRecord members of _records list of an SmpSeries.
    {
        Piecewise = 0, // Intermediate values are interpolated linearly from previous value to next value.
        Stepwise = 1   // Intermediate values are assumed to equal previous value.
        // Note: Stepwise interpretation does not make sense for CHD Package because CHD implements 
        // linear interpolation between beginning and end of stress period anyway.
    }

    /// <summary>
    /// Represents a time series of data of type SmpRecord
    /// </summary>
    public class SmpSeries
    {
        #region Fields
        private List<SmpRecord> _records;
        #endregion Fields

        #region Constructors
        public SmpSeries()
        {
            _records = new List<SmpRecord>();
        }
        public SmpSeries(string path) : this()
        {
            if (File.Exists(path))
            {
                string id;
                DateTime dateTime;
                double value;
                bool lacksIntegrity;
                string[] contents = FileUtil.GetFileContents(path);
                for (int i = 0; i < contents.Count(); i++)
                {
                    SmpRecord smpRecord = new SmpRecord();
                    if (smpRecord.Define(contents[i]))
                    {
                        _records.Add(smpRecord);
                    }
                }
            }
        }
        #endregion Constructors

        #region Properties
        public List<SmpRecord> Records
        {
            get
            {
                return _records;
            }
            set
            {
                _records = value;
            }
        }
        #endregion Properties

        #region Methods
        public void AssignFrom(SmpSeries seriesSource)
        {
            Records.Clear();
            for (int i = 0; i < seriesSource.Records.Count; i++ )
            {
                SmpRecord record = new SmpRecord();
                record.AssignFrom(seriesSource.Records[i]);
                Records.Add(record);
            }
        }
        public SmpSeries FilterById(string id)
        {
            SmpSeries filteredSmpSeries = new SmpSeries();
            for (int i = 0; i < _records.Count; i++)
            {
                if (_records[i].ID == id)
                {
                    filteredSmpSeries.Records.Add(_records[i]);
                }
            }
            return filteredSmpSeries;
        }
        /// <summary>
        /// True if SmpSeries contains a positive value in interval from timeStart to timeEnd, including timeStart but excluding timeEnd.
        /// If SmpSeries does not contain a record in the interval, but last preceding value is positive, method returns true.
        /// If SmpSeries does not contain a record in the interval and there are no preceding records, method returns false.
        /// </summary>
        /// <param name="timeStart">Beginning of time interval (inclusive)</param>
        /// <param name="timeEnd">End of time interval (exclusive)</param>
        /// <returns></returns>
        public bool ValuePositiveDuringTimeInterval(DateTime timeStart, DateTime timeEnd)
        {
            bool foundPositivePrecedingInterval = false;
            bool foundPositiveInInterval = false;
            bool foundPrecedingRecord = false;
            bool foundRecordInInterval = false;
            double currentValue;
            for (int i = 0; i < _records.Count; i++)
            {
                if (_records[i].DateTime < timeStart)
                {
                    foundPrecedingRecord = true;
                    currentValue = _records[i].Value;
                    foundPositivePrecedingInterval = (currentValue > 0.0);
                }
                else if (_records[i].DateTime < timeEnd)
                {
                    foundRecordInInterval = true;
                    currentValue = _records[i].Value;
                    if (currentValue > 0.0)
                    {
                        foundPositiveInInterval = true;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            if (foundRecordInInterval)
            {
                return foundPositiveInInterval;
            }
            else if (foundPrecedingRecord)
            {
                return foundPositivePrecedingInterval;
            }
            else
            {
                return false;
            }
        }
        #endregion Methods
    }
}
