using System;
using System.Windows.Forms;

using ScenarioTools.Util;

namespace ScenarioTools.ModflowReaders
{
    public enum InOutAccess
    {
        Unknown = 0,
        Input = 1,
        Output = 2,
        ScenarioInput = 3
    }

    /// <summary>
    /// Represents one entry in a MODFLOW name file
    /// </summary>
    public class NameFileEntry
    {
        public static string[] InputTypes = 
            { "BAS6", "BCF6", "WEL", "DRN", "RIV", "EVT", "GHB", "RCH", 
              "SIP", "DE4", "OC", "PCG", "LMG", "GWT", "FHB", "RES", 
              "STR", "IBS", "CHD", "HFB6", "LAK", "LPF", "DIS", "PVAL", 
              "HOB", "ZONE", "MULT", "DROB", "RVOB", "GBOB", "STOB", 
              "HUF2", "CHOB", "ETS", "DRT", "GMG", "HYD", "SFR", 
              "GAGE", "LVDA", "LMT6", "MNW1", "KDEP", "SUB", "UZF", 
              "GWM", "SWT", "CFP", "NRS", "SWR" };

        public static string[] OutputTypes = { "LIST", "GLOBAL" };

        #region Fields

        private string _type;
        private int _unit;
        private string _filename;
        private string _status;
        private InOutAccess _access;
        // Add a StreamReader and a StreamWriter?

        #endregion Fields

        #region Constructors

        public NameFileEntry()
        {
            _type = "";
            _unit = 0;
            _filename = "";
            _status = "";
            _access = InOutAccess.Unknown;
        }

        /// <summary>
        /// Construct a NameFileEntry by parsing a line read from a MODFLOW name file
        /// </summary>
        /// <param name="line">Line read from a MODFLOW name file</param>
        public NameFileEntry(string line) : this()
        {
            string[] words = StringUtil.ParseLine(line);
            if (words[0].StartsWith("#"))
            {
                return;
            }
            try
            {
                _type = words[0].ToUpper();
                _unit = Convert.ToInt32(words[1]);
                _filename = words[2];
                if (words.Length > 3)
                {
                    if (words[3].ToUpper() == "OLD")
                    {
                        _status = "OLD";
                    }
                    else if (words[3].ToUpper() == "REPLACE")
                    {
                        _status = "REPLACE";
                    }
                }

                if (_status != "")
                {
                    if (_status == "OLD")
                    {
                        _access = InOutAccess.Input;
                    }
                    else if (_status == "REPLACE")
                    {
                        _access = InOutAccess.Output;
                    }
                }
                else
                {
                    for (int i = 0; i < InputTypes.Length; i++)
                    {
                        if (_type == InputTypes[i])
                        {
                            _access = InOutAccess.Input;
                            break;
                        }
                    }
                    for (int i = 0; i < OutputTypes.Length; i++)
                    {
                        if (_type == OutputTypes[i])
                        {
                            _access = InOutAccess.Output;
                            break;
                        }
                    }
                }
            }
            catch
            {
                return;
            }
        }

        public NameFileEntry(NameFileEntry nameFileEntry)
            : this()
        {
            _type = nameFileEntry.Type;
            _unit = nameFileEntry.Unit;
            _filename = nameFileEntry.Filename;
            _status = nameFileEntry.Status;
            _access = nameFileEntry.Access;
        }
        #endregion Constructors

        #region Properties

        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public int Unit
        {
            get
            {
                return _unit;
            }
            set
            {
                _unit = value;
            }
        }

        public string Filename
        {
            get
            {
                return _filename;
            }
            set
            {
                _filename = value;
            }
        }

        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        public InOutAccess Access
        {
            get
            {
                return _access;
            }
            set
            {
                _access = value;
            }
        }

        #endregion Properties

        #region Methods

        public string MakeNamefileLine(string scenarioId)
        {
            string line = "";
            string type = StringUtil.PadRightToLength(_type, 14);
            string unit = _unit.ToString();            
            unit = unit.PadLeft(6);
            switch (_access)
            {
                case InOutAccess.ScenarioInput:
                    // fall through
                case InOutAccess.Input:
                    line = type + unit + "   " + StringUtil.SingleQuoteIfNeeded(_filename);
                    line = line + "   OLD";
                    break;
                case InOutAccess.Output:
                    line = type + unit + "   " + StringUtil.SingleQuoteIfNeeded(_filename);
                    line = line + "   REPLACE";
                    break;
                default:
                    MessageBox.Show("In MakeNamefileLine, access unknown: " + _access.ToString());
                    break;
            }
            return line;
        }

        #endregion Methods
    }
}
