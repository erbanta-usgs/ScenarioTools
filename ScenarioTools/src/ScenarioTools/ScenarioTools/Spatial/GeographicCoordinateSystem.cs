using System;
using System.Collections.Generic;
using System.Text;

namespace ScenarioTools.Spatial
{
    public class GeographicCoordinateSystem
    {
        #region Fields
        private string _name;
        private string _datum;
        private string _primeMeridian;
        private string _angularUnit;
        private string _spheroid;
        private List<SpatialReferenceParameter> _spatialReferenceParameterList;
        #endregion Fields

        #region Constructors
        public GeographicCoordinateSystem()
        {
            _name = "";
            _datum = "";
            _primeMeridian = "";
            _angularUnit = "";
            _spheroid = "";
            _spatialReferenceParameterList = new List<SpatialReferenceParameter>();
        }
        /// <summary>
        /// Construct a GeographicCoordinateSystem object
        /// </summary>
        /// <param name="wktProjCS">Well Known Text string of projected coordinate system</param>
        public GeographicCoordinateSystem(string wktProjCS)
            : this()
        {
            // Parse well known text of projected coordinate system and
            // extract parameters that define geographic coordinate system
            parse(wktProjCS);
        }
        #endregion Constructors

        #region Properties
        public string Name
        {
            get
            {
                return _name;
            }
        }
        public string Datum
        {
            get
            {
                return _datum;
            }
        }
        public string PrimeMeridian
        {
            get
            {
                return _primeMeridian;
            }
        }
        public string AngularUnit
        {
            get
            {
                return _angularUnit;
            }
        }
        public string Spheroid
        {
            get
            {
                return _spheroid;
            }
        }
        #endregion Properties

        #region Private methods
        private void parse(string wktProjCS)
        {
            // Parse well known text of projected coordinate system and
            // extract string containing parameters that define geographic 
            // coordinate system
            string geogcsArgString = ExtractGeogCsArgs(wktProjCS);

            // Parse name of geographic coordinate system
            int nameStart = geogcsArgString.IndexOf('"') + 1;
            int nameEnd = geogcsArgString.IndexOf('"', nameStart);
            int len = nameEnd - nameStart;
            _name = geogcsArgString.Substring(nameStart, len);

            //
            string remainingString = "";
            SpatialReferenceParameter tempSRP = new SpatialReferenceParameter(geogcsArgString);
            if (tempSRP.ArgString != "")
            {
                bool done = false;
                remainingString = tempSRP.ArgString;
                while (!done)
                {
                    // Ned TODO: Start Here
                }
            }
            geogcsArgString = geogcsArgString.Remove(0, nameEnd + 1);
            //List<string> parameterList = new List<string>();
            //bool done = false;
            //while (!done)
            //{
            //    SpatialReferenceParameter newSRP = new SpatialReferenceParameter(
            //}
        }
        #endregion Private methods

        #region Public methods
        public string ExtractGeogCsArgs(string wktProjCS)
        {
            string result = "";
            int pos0 = -1;
            pos0 = wktProjCS.IndexOf("GEOGCS[");
            if (pos0 < 0)
            {
                pos0 = wktProjCS.IndexOf("GEOGCS(");
            }
            if (pos0 >= 0)
            {
                string sub0 = wktProjCS.Remove(0, pos0);
                pos0 = 7; // Points to 1st character following '[' in "GEOGCS["
                int k = 1; // Nesting level within brackets
                string ch;
                for (int i = pos0; i < sub0.Length; i++)
                {
                    ch = sub0.Substring(i, 1);
                    if (ch == "[" || ch == "(")
                    {
                        k++;
                    }
                    else if (ch == "]" || ch == ")")
                    {
                        k--;
                    }
                    if (k == 0)
                    {
                        // Found ']' that matches initial '['
                        int len = i - 7;
                        // Return string enclosed by outer set of []
                        if (len > 0)
                        {
                            result = sub0.Substring(7, len);
                        }
                        break;
                    }
                }
            }
            return result;
        }
        #endregion Public methods
    }
}
