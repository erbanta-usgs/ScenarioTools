using System;
using System.Collections.Generic;

using System.Text;

namespace ScenarioTools.DataClasses
{
    public struct TimeSeriesSample
    {
        DateTime date;
        float value;

        public TimeSeriesSample(DateTime date, float value)
        {
            this.date = date;
            this.value = value;
        }
        public DateTime Date
        {
            get
            {
                return date;
            }
        }
        public float Value
        {
            get
            {
                return value;
            }
        }

    }
}
