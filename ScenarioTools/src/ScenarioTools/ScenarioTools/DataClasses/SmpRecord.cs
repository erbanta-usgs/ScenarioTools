using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScenarioTools.DataClasses
{
    /// <summary>
    /// Represents one record of a "Bore Sample File"
    /// as documented in: Groundwater Data Utilities Part A: Overview
    /// (Watermark Numerical Computing, 2003), p. 11.
    /// </summary>
    public struct SmpRecord
    {
        #region Fields

        private string _id;           // Identifier
        private DateTime _dateTime;
        private double _value;        // Measurement value
        private bool _lacksIntegrity;

        #endregion Fields

        public SmpRecord(string id, DateTime dateTime, double value, bool lacksIntegrity)
        {
            _id = id;
            _dateTime = dateTime;
            _value = value;
            _lacksIntegrity = lacksIntegrity;
        }

        #region Properties

        public string ID
        {
            get
            {
                return _id;
            }
            private set
            {
                _id = value;
            }
        }

        public DateTime DateTime
        {
            get
            {
                return _dateTime;
            }
            private set
            {
                _dateTime = value;
            }
        }

        public double Value
        {
            get
            {
                return _value;
            }
            private set
            {
                _value = value;
            }
        }

        public bool LacksIntegrity
        {
            get
            {
                return _lacksIntegrity;
            }
            private set
            {
                _lacksIntegrity = value;
            }
        }

        #endregion Properties

        #region Methods

        public void AssignFrom(SmpRecord recordSource)
        {
            ID = recordSource.ID;
            DateTime = recordSource.DateTime;
            Value = recordSource.Value;
            LacksIntegrity = recordSource.LacksIntegrity;
        }

        /// <summary>
        /// Assign SmpRecord fields by reading a line of text
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool Define(string line)
        {
            bool OK = true;
            try
            {
                string[] words = line.Split();
                //
                // Assign ID
                _id = words[0];
                //
                // Assign DateTime
                char[] slash = new char[] { '/' };
                string[] dates = words[1].Split(slash, 3);
                int month = Convert.ToInt32(dates[0]);
                int day = Convert.ToInt32(dates[1]);
                int year = Convert.ToInt32(dates[2]);
                char[] colon = new char[] { ':' };
                string[] times = words[2].Split(colon, 3);
                int hour = Convert.ToInt32(times[0]);
                int minute = Convert.ToInt32(times[1]);
                int second = Convert.ToInt32(times[2]);
                _dateTime = new DateTime(year, month, day, hour, minute, second);
                //
                // Assign Value
                _value = Convert.ToDouble(words[3]);
                //
                // Assign LacksIntegrity
                _lacksIntegrity = false;
                if (words.Count() > 4)
                {
                    int compare = String.Compare(words[4], "x", true);
                    _lacksIntegrity = (compare == 0);
                }
            }
            catch
            {
                OK = false;
            }
            return OK;
        }

        #endregion Methods
    }
}
