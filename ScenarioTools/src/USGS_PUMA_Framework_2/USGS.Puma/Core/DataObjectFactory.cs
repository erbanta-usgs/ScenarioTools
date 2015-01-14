using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace USGS.Puma.Core
{
    /// <summary>
    /// Create Puma DataObjects from XML data.
    /// </summary>
    /// <remarks></remarks>
    public class DataObjectFactory
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, string> dataObjectNames = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <remarks></remarks>
        public DataObjectFactory()
        {
            dataObjectNames = new Dictionary<string, string>();
            dataObjectNames.Add("ContourLine", "ContourLine");
            dataObjectNames.Add("ContourLineList", "ContourLineList");
            dataObjectNames.Add("CellCenteredArealGrid", "CellCenteredArealGrid");
            dataObjectNames.Add("Array1dInt32", "Array1dInt32");
            dataObjectNames.Add("Array1dSingle", "Array1dSingle");
            dataObjectNames.Add("Array1dDouble", "Array1dDouble");
            dataObjectNames.Add("Array2dInt32", "Array2dInt32");
            dataObjectNames.Add("Array2dSingle", "Array2dSingle");
            dataObjectNames.Add("Array2dDouble", "Array2dDouble");
            dataObjectNames.Add("Array2dListInt32", "Array2dListInt32");
            dataObjectNames.Add("Array2dListSingle", "Array2dListSingle");
            dataObjectNames.Add("Array2dListDouble", "Array2dListDouble");
            dataObjectNames.Add("GridCell", "GridCell");
            dataObjectNames.Add("GridCellList", "GridCellList");
            dataObjectNames.Add("GridCellRegion", "GridCellRegion");
            dataObjectNames.Add("GridCellRegionValue", "GridCellRegionValue");
            dataObjectNames.Add("GridCellValue", "GridCellValue");
            dataObjectNames.Add("VerticalGrid", "VerticalGrid");
        }
        #endregion

        #region Public Members
        /// <summary>
        /// Gets the data object names.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<string> GetDataObjectNames()
        {
            List<string> list = new List<string>();
            foreach (KeyValuePair<string,string> item in dataObjectNames)
            {
                list.Add(item.Key);
            }
            return list;
        }

        /// <summary>
        /// Gets the data object header.
        /// </summary>
        /// <param name="xmlString">The XML string.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public DataObjectHeader GetDataObjectHeader(string xmlString)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            XmlNode root = doc.DocumentElement as XmlNode;
            return GetDataObjectHeader(root);
        }

        /// <summary>
        /// Gets the data object header.
        /// </summary>
        /// <param name="xmlNode">The XML node.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public DataObjectHeader GetDataObjectHeader(XmlNode xmlNode)
        {
            DataObjectHeader header = new DataObjectHeader(xmlNode);
            if (string.IsNullOrEmpty(header.PumaType))
            {
                return null;
            }
            else
            {
                return header;
            }
        }

        /// <summary>
        /// Determines whether [is data object] [the specified XML node].
        /// </summary>
        /// <param name="xmlNode">The XML node.</param>
        /// <returns><c>true</c> if [is data object] [the specified XML node]; otherwise, <c>false</c>.</returns>
        /// <remarks></remarks>
        public bool IsDataObject(XmlNode xmlNode)
        {
            DataObjectHeader header = GetDataObjectHeader(xmlNode);
            if (header != null)
            {
                return IsDataObject(header.PumaType);
            }
            else
            { return false; }
        }
        /// <summary>
        /// Determines whether [is data object] [the specified reader].
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns><c>true</c> if [is data object] [the specified reader]; otherwise, <c>false</c>.</returns>
        /// <remarks></remarks>
        public bool IsDataObject(XmlTextReader reader)
        {
            try
            {
                string pumaType = reader.GetAttribute("PumaType");
                return IsDataObject(pumaType);
            }
            catch (Exception)
            {
                return false;
            }

        }
        /// <summary>
        /// Determines whether [is data object] [the specified puma type].
        /// </summary>
        /// <param name="pumaType">Type of the puma.</param>
        /// <returns><c>true</c> if [is data object] [the specified puma type]; otherwise, <c>false</c>.</returns>
        /// <remarks></remarks>
        public bool IsDataObject(string pumaType)
        {
            string shortName = GetShortTypeName(pumaType);
            return dataObjectNames.ContainsKey(shortName);
        }

        /// <summary>
        /// Determines whether this instance can create the specified XML string.
        /// </summary>
        /// <param name="xmlString">The XML string.</param>
        /// <returns><c>true</c> if this instance can create the specified XML string; otherwise, <c>false</c>.</returns>
        /// <remarks></remarks>
        public bool CanCreate(string xmlString)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Determines whether this instance can create the specified data object header.
        /// </summary>
        /// <param name="dataObjectHeader">The data object header.</param>
        /// <returns><c>true</c> if this instance can create the specified data object header; otherwise, <c>false</c>.</returns>
        /// <remarks></remarks>
        public bool CanCreate(DataObjectHeader dataObjectHeader)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Creates the specified XML string.
        /// </summary>
        /// <param name="xmlString">The XML string.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IDataObject Create(string xmlString)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the short name of the type.
        /// </summary>
        /// <param name="pumaType">Type of the puma.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private string GetShortTypeName(string pumaType)
        {
            string[] parts = pumaType.Split(new char[1] { '.' });
            string shortName = "";
            if (parts.Length > 0)
                shortName = parts[parts.Length - 1];
            return shortName;
        }

        #endregion

    }
}
