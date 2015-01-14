using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace USGS.Puma.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class DataObjectHeader
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <remarks></remarks>
        public DataObjectHeader()
        {
            PumaType = "";
            Version = 1;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DataObjectHeader"/> class.
        /// </summary>
        /// <param name="xmlNode">The XML node.</param>
        /// <remarks></remarks>
        public DataObjectHeader(XmlNode xmlNode)
            : this()
        {
            XmlAttribute pumaType = xmlNode.Attributes["PumaType"];
            if (pumaType == null)
                return;
            PumaType = pumaType.Value;

            XmlAttribute version = xmlNode.Attributes["Version"];
            if (version != null)
                Version = int.Parse(version.Value);

        }
        #endregion


        /// <summary>
        /// 
        /// </summary>
        private string _PumaType;
        /// <summary>
        /// Gets or sets the type of the puma.
        /// </summary>
        /// <value>The type of the puma.</value>
        /// <remarks></remarks>
        public string PumaType
        {
            get { return _PumaType; }
            set { _PumaType = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        private int _Version;
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        /// <remarks></remarks>
        public int Version
        {
            get { return _Version; }
            set { _Version = value; }
        }
    }
}
