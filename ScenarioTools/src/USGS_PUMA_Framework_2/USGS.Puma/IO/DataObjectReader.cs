using System;
using System.Collections.Generic;
using System.Text;
using USGS.Puma.Core;
using USGS.Puma.Utilities;
using USGS.Puma.NTS.IO.GML2;
using USGS.Puma.NTS.Geometries;
using GeoAPI.Geometries;
using System.Xml;


namespace USGS.Puma.IO
{
    public class DataObjectReader
    {
        #region Fields
        private GMLReader gmlReader = null;
        private DataObjectUtility doUtility = null;
        private DataObjectFactory doFactory = null;
        #endregion

        #region Constructors
        public DataObjectReader()
        {
            gmlReader = new GMLReader();
            doUtility = new DataObjectUtility();
            doFactory = new DataObjectFactory();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public IDataObject Read(XmlTextReader reader)
        {
            // Add code
            if (doFactory.IsDataObject(reader))
            {
                return null;
            }
            else
            { return null; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void Write(XmlTextWriter writer)
        {
            // Add code
        }

        public bool IsDataObject(XmlTextReader reader)
        {
            return doFactory.IsDataObject(reader);
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
