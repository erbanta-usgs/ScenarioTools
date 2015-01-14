using System;
using System.Collections.Generic;
using System.Text;
using USGS.Puma.Core;
using USGS.Puma.FiniteDifference;
using USGS.Puma.Utilities;
using USGS.Puma.NTS.IO.GML2;
using USGS.Puma.NTS.Geometries;
using GeoAPI.Geometries;
using System.Xml;
using System.IO;

namespace USGS.Puma.IO
{
    public class DataObjectWriter
    {
        #region Static Methods

        #endregion

        #region Fields
        private DataObjectFactory doFactory = null;
        private DataObjectUtility doUtility = null;
        private GMLWriter gmlWriter = null;
        #endregion

        #region Constructors
        public DataObjectWriter()
        {
            doFactory = new DataObjectFactory();
            doUtility = new DataObjectUtility();
            gmlWriter = new GMLWriter();
        }
        #endregion

        #region Public Properties

        #endregion

        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public XmlTextWriter CreateWriter(string filename)
        {
            XmlTextWriter writer = new XmlTextWriter(filename, null);
            writer.Namespaces = true;
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            return writer;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public XmlTextWriter CreateWriter(FileStream fileStream)
        {
            XmlTextWriter writer = new XmlTextWriter(fileStream, null);
            writer.Namespaces = true;
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            return writer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="writer"></param>
        public void Write(IDataObject dataObject, XmlTextWriter writer)
        {
            Write(dataObject, writer, dataObject.DefaultName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="writer"></param>
        /// <param name="elementName"></param>
        public void Write(IDataObject dataObject, XmlTextWriter writer, string elementName)
        {
            switch (dataObject.PumaType)
            {
                case "ContourLine":
                    WriteContourLine(dataObject as ContourLine, writer, elementName);
                    break;
                case "ContourLineList":
                    WriteContourLineList(dataObject as ContourLineList, writer, elementName);
                    break;
                case "CellCenteredArealGrid":
                    WriteCellCenteredArealGrid(dataObject as CellCenteredArealGrid, writer, elementName);
                    break;
                case "Array1dSingle":
                    WriteArray1dSingle(dataObject as Array1d<float>, writer, elementName);
                    break;
                case "Array1dDouble":
                    WriteArray1dDouble(dataObject as Array1d<double>, writer, elementName);
                    break;
                case "Array1dInt32":
                    WriteArray1dInt32(dataObject as Array1d<Int32>, writer, elementName);
                    break;
                case "Array2dSingle":
                    WriteArray2dSingle(dataObject as Array2d<float>, writer, elementName);
                    break;
                case "Array2dDouble":
                    WriteArray2dDouble(dataObject as Array2d<double>, writer, elementName);
                    break;
                case "Array2dInt32":
                    WriteArray2dInt32(dataObject as Array2d<Int32>, writer, elementName);
                    break;
                default:
                    throw new ArgumentException("XML output for this DataObject type is not yet supported.");
            }
        }

        #endregion

        #region DataObject Write Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contour"></param>
        /// <param name="writer"></param>
        /// <param name="elementName"></param>
        protected void WriteContourLine(ContourLine contour, XmlTextWriter writer, string elementName)
        {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("PumaType", contour.PumaType);
            writer.WriteStartElement("ContourLevel");
            writer.WriteString(contour.ContourLevel.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("Contour");
            gmlWriter.Write(writer, contour.Contour as IGeometry);
            writer.WriteEndElement();
            writer.WriteEndElement();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contourList"></param>
        /// <param name="writer"></param>
        /// <param name="elementName"></param>
        protected void WriteContourLineList(ContourLineList contourList, XmlTextWriter writer, string elementName)
        {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("PumaType", contourList.PumaType);
            for (int i = 0; i < contourList.Count; i++)
            {
                WriteContourLine(contourList[i], writer, contourList[i].DefaultName);
            }
            writer.WriteEndElement();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="writer"></param>
        /// <param name="elementName"></param>
        protected void WriteCellCenteredArealGrid(CellCenteredArealGrid grid, XmlTextWriter writer, string elementName)
        {
            string s = null;

            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("PumaType", grid.PumaType);
            writer.WriteElementString("RowCount", grid.RowCount.ToString());
            writer.WriteElementString("ColumnCount", grid.ColumnCount.ToString());
            writer.WriteElementString("Angle", grid.Angle.ToString());

            // Process and write the row spacing
            if (grid.RowSpacingIsConstant())
            { s = grid.GetRowSpacing(1).ToString(); }
            else
            {
                Array1d<double> spacing = grid.GetRowSpacing();
                s = NumberArrayIO<double>.ArrayToString(spacing.GetValues(), true);
            }
            writer.WriteElementString("RowSpacing", s);

            // Process and write the column spacing
            if (grid.ColumnSpacingIsConstant())
            { s = grid.GetColumnSpacing(1).ToString(); }
            else
            {
                Array1d<double> spacing = grid.GetColumnSpacing();
                s = NumberArrayIO<double>.ArrayToString(spacing.GetValues(), true);
            }
            writer.WriteElementString("ColumnSpacing", s);

            writer.WriteElementString("Projection", grid.Projection);

            // Close the element
            writer.WriteEndElement();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataArray"></param>
        /// <param name="writer"></param>
        /// <param name="elementName"></param>
        protected void WriteArray1dSingle(Array1d<float> dataArray, XmlTextWriter writer, string elementName)
        {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("PumaType", dataArray.PumaType);
            writer.WriteAttributeString("ElementCount", dataArray.ElementCount.ToString());
            if (dataArray.IsConstant)
            {
                writer.WriteElementString("ConstantValue", dataArray[1].ToString());
            }
            else
            {
                string s = NumberArrayIO<float>.ArrayToString(dataArray.ToArray(), false);
                writer.WriteElementString("Elements", s);
            }
            writer.WriteEndElement();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataArray"></param>
        /// <param name="writer"></param>
        /// <param name="elementName"></param>
        protected void WriteArray1dDouble(Array1d<double> dataArray, XmlTextWriter writer, string elementName)
        {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("PumaType", dataArray.PumaType);
            writer.WriteAttributeString("ElementCount", dataArray.ElementCount.ToString());
            if (dataArray.IsConstant)
            {
                writer.WriteElementString("ConstantValue", dataArray[1].ToString());
            }
            else
            {
                string s = NumberArrayIO<double>.ArrayToString(dataArray.ToArray(), false);
                writer.WriteElementString("Elements", s);
            }
            writer.WriteEndElement();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataArray"></param>
        /// <param name="writer"></param>
        /// <param name="elementName"></param>
        protected void WriteArray1dInt32(Array1d<int> dataArray, XmlTextWriter writer, string elementName)
        {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("PumaType", dataArray.PumaType);
            writer.WriteAttributeString("ElementCount", dataArray.ElementCount.ToString());
            if (dataArray.IsConstant)
            {
                writer.WriteElementString("ConstantValue", dataArray[1].ToString());
            }
            else
            {
                string s = NumberArrayIO<Int32>.ArrayToString(dataArray.ToArray(), false);
                writer.WriteElementString("Elements", s);
            }
            writer.WriteEndElement();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataArray"></param>
        /// <param name="writer"></param>
        /// <param name="elementName"></param>
        protected void WriteArray2dSingle(Array2d<float> dataArray, XmlTextWriter writer, string elementName)
        {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("PumaType", dataArray.PumaType);
            writer.WriteAttributeString("RowCount", dataArray.RowCount.ToString());
            writer.WriteAttributeString("ColumnCount", dataArray.ColumnCount.ToString());
            if (dataArray.IsConstant)
            {
                writer.WriteElementString("ConstantValue", dataArray[1, 1].ToString());
            }
            else
            {
                string s = null;
                for (int row = 1; row <= dataArray.RowCount; row++)
                {
                    writer.WriteStartElement("Row");
                    writer.WriteAttributeString("i", row.ToString());
                    s = NumberArrayIO<float>.ArrayToString(dataArray.ToRowArray(row), false);
                    writer.WriteString(s);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataArray"></param>
        /// <param name="writer"></param>
        /// <param name="elementName"></param>
        protected void WriteArray2dDouble(Array2d<double> dataArray, XmlTextWriter writer, string elementName)
        {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("PumaType", dataArray.PumaType);
            writer.WriteAttributeString("RowCount", dataArray.RowCount.ToString());
            writer.WriteAttributeString("ColumnCount", dataArray.ColumnCount.ToString());
            if (dataArray.IsConstant)
            {
                writer.WriteElementString("ConstantValue", dataArray[1, 1].ToString());
            }
            else
            {
                string s = null;
                for (int row = 1; row <= dataArray.RowCount; row++)
                {
                    writer.WriteStartElement("Row");
                    writer.WriteAttributeString("i", row.ToString());
                    s = NumberArrayIO<double>.ArrayToString(dataArray.ToRowArray(row), false);
                    writer.WriteString(s);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataArray"></param>
        /// <param name="writer"></param>
        /// <param name="elementName"></param>
        protected void WriteArray2dInt32(Array2d<int> dataArray, XmlTextWriter writer, string elementName)
        {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("PumaType", dataArray.PumaType);
            writer.WriteAttributeString("RowCount", dataArray.RowCount.ToString());
            writer.WriteAttributeString("ColumnCount", dataArray.ColumnCount.ToString());
            if (dataArray.IsConstant)
            {
                writer.WriteElementString("ConstantValue", dataArray[1, 1].ToString());
            }
            else
            {
                string s = null;
                for (int row = 1; row <= dataArray.RowCount; row++)
                {
                    writer.WriteStartElement("Row");
                    writer.WriteAttributeString("i", row.ToString());
                    s = NumberArrayIO<Int32>.ArrayToString(dataArray.ToRowArray(row), false);
                    writer.WriteString(s);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }


        #endregion

        #region Private Methods

        #endregion

    }
}
