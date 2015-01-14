using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;

using ScenarioTools.Data_Providers;
using ScenarioTools.Geometry;
using ScenarioTools.Util;
using ScenarioTools.Xml;

namespace ScenarioTools.Reporting
{
    public class ReportElementSTMap : IReportElement, IReportMap
    {
        #region Constants
        private const long INVALID_TICKS = -1;
        private const string XML_NODE_NAME = "ReportElementSTMap";
        private const string XML_KEY_NAME = "name";
        private const string XML_EXTENT_NAME = "DesiredExtentName";
        private const string XML_KEY_SHOW_BACKGROUND_IMAGE = "ShowBackgroundImage";
        private const string XML_BACKGROUND_IMAGE_BRIGHTNESS = "BackgroundImageBrightness";
        #endregion Constants

        #region Fields
        private STMap _stMap;
        private string _desiredExtentName;
        private string _backgroundImageFile = "";
        private int _backgroundImageBrightness;
        #endregion Fields

        #region Constructor
        public ReportElementSTMap()
        {
            _stMap = new STMap();
            _stMap.ClearImage();
            _backgroundImageBrightness = 0;
            _desiredExtentName = "";
        }
        #endregion Constructor

        #region IReportElement Members
        public string Name 
        {
            get
            {
                return _stMap.Name;
            }
            set
            {
                _stMap.Name = value;
            }
        }
        public int NumDataSeries
        {
            get
            {
                return _stMap.DataSeriesCount();
            }
        }
        public void AddDataSeries(DataSeries dataSeries)
        {
            // Add the data series to the list of data series.
            _stMap.AddDataSeries(dataSeries);

            // Set this as the parent of the data series.
            dataSeries.ParentElement = this;
        }
        public void RemoveDataSeries(DataSeries dataSeries)
        {
            _stMap.RemoveDataSeries(dataSeries);
        }
        public DataSeries GetDataSeries(int index)
        {
            return _stMap.GetDataSeries(index);
        }
        public object GetDataSeries(string name)
        {
            return _stMap.GetDataSeries(name);
        }
        public void ClearDataSeries()
        {
            _stMap.ClearDataSeries();
        }
        public XmlNode GetXmlNode(XmlDocument document, string targetFileName)
        {
            // Make the XML element that represents this object.
            XmlElement element = document.CreateElement(XML_NODE_NAME);
            element.SetAttribute("NodeType", WorkspaceUtil.XML_NODE_TYPE_REPORT_ELEMENT);

            // Prepare the XML element.
            prepareXmlElement(document, element, targetFileName);

            // Return the result.
            return element;
        }
        public void SaveXMLFile(string file)
        {
            // Make the XML file.
            XmlDocument document = XmlUtil.CreateXmlFile(file, XML_NODE_NAME);

            // Prepare the root element.
            prepareXmlElement(document, document.DocumentElement, file);

            // Write the file.
            document.Save(file);
        }
        public void InitFromXML(XmlElement xmlElement, string sourceFileName)
        {
            _stMap.ClearImage();

            // Get the name and extent name attributes from the XML element.
            Name = XmlUtil.SafeGetStringAttribute(xmlElement, XML_KEY_NAME, "");
            DesiredExtentName = XmlUtil.SafeGetStringAttribute(xmlElement, XML_EXTENT_NAME, "");
            BackgroundImageBrightness = XmlUtil.SafeGetIntAttribute(xmlElement, XML_BACKGROUND_IMAGE_BRIGHTNESS, 0);
            ShowBasemap = ScenarioTools.Util.StringUtil.StringToBool(XmlUtil.SafeGetStringAttribute(xmlElement, XML_KEY_SHOW_BACKGROUND_IMAGE, "True"));

            // Get all of the data series.
            foreach (XmlElement child in xmlElement.ChildNodes)
            {
                // Get the map type of the child.
                DataSeriesTypeEnum mapType = DataSeriesTypeEnum.ColorFillMapSeries;
                string mapTypeString = child.GetAttribute("mapType");
                if (mapTypeString == "contour")
                {
                    mapType = DataSeriesTypeEnum.ContourMapSeries;
                }

                // Make the data series and initialize it with the XML node.
                DataSeries dataSeries = new DataSeries(mapType);
                dataSeries.InitFromXml(this, child, sourceFileName);

                // Add the data series to the list.
                _stMap.AddDataSeries(dataSeries);
            }
        }
        public void ValidateDataProviderKeys(List<long> uniqueIdentifiers)
        {
            // Validate the data provider key of every data series.
            for (int i = 0; i < _stMap.DataSeriesCount(); i++)
            {
                _stMap.DataSeriesList[i].ValidateDataProviderKey(uniqueIdentifiers);
            }
        }
        public void UpdateCaches()
        {

        }
        public bool IsUniqueName(string name, DataSeries dataSeries)
        {
            // If any other data series in the element match the specified name, return false.
            for (int i = 0; i < _stMap.DataSeriesCount(); i++)
            {
                if (_stMap.DataSeriesList[i] != dataSeries)
                {
                    if (_stMap.DataSeriesList[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
            }

            // None matched, so return false.
            return true;
        }
        #endregion IReportElement Members

        #region IImageProvider members
        public Image GetImage()
        {
            _stMap.NeatLineColor = Color.Black;
            _stMap.RenderAsImage();
            return _stMap.Image;
        }
        public Image GetImage(float minValue, float maxValue, long minTicks,
                              long maxTicks, double xcMin, double xcMax, double ycMin,
                              double ycMax, bool isStandalone)
        {
            // Ned TODO: Code ReportElementSTMap.GetImage.
            return null;
        }
        #endregion IImageProviders members

        public void ClearImage()
        {
            _stMap.ClearImage();
        }

        #region Properties
        public bool ShowBasemap
        {
            get
            {
                return _stMap.ShowBackgroundImage;
            }
            set
            {
                _stMap.ShowBackgroundImage = value;
            }
        }
        public string ExtentName
        {
            get
            {
                if (_stMap != null)
                {
                    return _stMap.ExtentName;
                }
                return "";
            }
            private set
            {
                if (_stMap != null)
                {
                    _stMap.ExtentName = value;
                }
            }
        }
        public string DesiredExtentName
        {
            get
            {
                if (_stMap != null)
                {
                    _desiredExtentName = _stMap.DesiredExtentName;
                    return _desiredExtentName;
                }
                return _desiredExtentName;
            }
            set
            {
                if (_desiredExtentName != value)
                {
                    _desiredExtentName = value;
                }
                if (_stMap != null)
                {
                    _stMap.DesiredExtentName = value;
                }
            }
        }
        public int BackgroundImageBrightness
        {
            get
            {
                if (_stMap != null)
                {
                    _backgroundImageBrightness = _stMap.BackgroundImageBrightness;
                }
                return _backgroundImageBrightness;
            }
            set
            {
                _backgroundImageBrightness = value;
                if (_stMap != null)
                {
                    _stMap.BackgroundImageBrightness = value;
                }
            }
        }
        public STMap STMap
        {
            get
            {
                return _stMap;
            }
        }
        public Image Image
        {
            get
            {
                return _stMap.Image;
            }
            //private set
            //{
            //    STMap.Image = value;
            //}
        }
        #endregion Properties

        #region IDeepCloneable Members
        public IReportElement DeepClone()
        {
            return (IReportElement)this.Copy();
        }
        object IDeepCloneable.DeepClone()
        {
            return this.DeepClone();
        }
        private ReportElementSTMap Copy()
        {
            ReportElementSTMap mapCopy = new ReportElementSTMap();
            mapCopy.Name = this.Name;
            mapCopy.DesiredExtentName = this._desiredExtentName;
            mapCopy.BackgroundImageBrightness = this._backgroundImageBrightness;
            mapCopy.BackgroundImageBrightness = this._backgroundImageBrightness;
            mapCopy._stMap.ShowBackgroundImage = this._stMap.ShowBackgroundImage;
            mapCopy._stMap.BackgroundImageFile = this._stMap.BackgroundImageFile;
            if (this.NumDataSeries > 0)
            {
                for (int i = 0; i < this.NumDataSeries; i++)
                {
                    DataSeries newDataSeries = new DataSeries(this.GetDataSeries(i));
                    newDataSeries.AssignParent(mapCopy);
                    mapCopy.AddDataSeries(newDataSeries);
                }
            }
            return mapCopy;
        }
        public void AssignParent(object parent)
        {
            // No parent required
        }
        #endregion IDeepCloneable Members

        public override string ToString()
        {
            string unnamed = "Unnamed Map";
            return "Map: " + (Name == null ? unnamed : (Name.Equals("") ? unnamed : Name));
        }

        private void prepareXmlElement(XmlDocument document, XmlElement element, string targetFileName)
        {
            // Set the name attribute.
            XmlUtil.FrugalSetAttribute(element, XML_KEY_NAME, Name, "");

            // Set the extent name attribute.
            XmlUtil.FrugalSetAttribute(element, XML_EXTENT_NAME, this.DesiredExtentName, "");

            // Set the ShowBasemap attribute.
            string  aShowBasemap = ShowBasemap.ToString();
            XmlUtil.FrugalSetAttribute(element, XML_KEY_SHOW_BACKGROUND_IMAGE, aShowBasemap, "True");

            // Set the BackgroundImageBrightness
            string bkgImBright = this._backgroundImageBrightness.ToString();
            XmlUtil.FrugalSetAttribute(element, XML_BACKGROUND_IMAGE_BRIGHTNESS, bkgImBright, "0");

            // Add all of the data series as children.
            for (int i = 0; i < _stMap.DataSeriesCount(); i++)
            {
                element.AppendChild(_stMap.DataSeriesList[i].GetXmlNode(document, targetFileName));
            }
        }
        public void AssignDesiredExtent(List<Extent> extents)
        {
            if (_stMap != null)
            {
                // Check names against ReportElementSTMap._stMap.DesiredExtentName
                // and return if match found
                if (_stMap.DesiredExtentName != "")
                {
                    for (int i = 0; i < extents.Count; i++)
                    {
                        if (extents[i].Name == _stMap.DesiredExtentName)
                        {
                            if (_stMap.Extent != extents[i])
                            {
                                _stMap.Extent = extents[i];
                            }
                            // Ensure that two copies of desired extent name match
                            _desiredExtentName = _stMap.DesiredExtentName;
                            return;
                        }
                    }
                }

                // Check extent names against ReportElementSTMap._desiredExtentName
                // and return if match found
                if (_desiredExtentName != "")
                {
                    for (int i = 0; i < extents.Count; i++)
                    {
                        if (extents[i].Name == _desiredExtentName)
                        {
                            _stMap.Extent = extents[i];
                            // Ensure that two copies of desired extent name match
                            _stMap.DesiredExtentName = _desiredExtentName;
                            return;
                        }
                    }
                }

                // If no match found, generate extent automatically
                _stMap.Extent = _stMap.GenerateAutomaticExtent();
                if (Extent.ExtentIsNull(_stMap.Extent))
                {
                    _stMap.Extent = _stMap.GenerateAutomaticExtent(extents);
                }
            }
        }
    }
}
