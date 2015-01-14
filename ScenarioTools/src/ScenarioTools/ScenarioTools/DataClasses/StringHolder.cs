using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScenarioTools.DataClasses
{
    // Ned TODO: Instead of StringHolder, the same could 
    // be done with boxing and unboxing
    public class StringHolder
    {
        private string _string;

        #region Constructors
        public StringHolder()
        {
            _string = "";
        }

        public StringHolder(string str)
        {
            _string = str;
        }
        #endregion

        #region Properties
        public string String
        {
            get
            {
                return _string;
            }
            set
            {
                _string = value;
            }
        }
        #endregion
    }
}
