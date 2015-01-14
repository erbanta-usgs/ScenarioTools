using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;

using ScenarioTools.DataClasses;
using ScenarioTools.DatasetCalculator;
using ScenarioTools.Data_Providers;
using ScenarioTools.Geometry;
using ScenarioTools.Graphics;
using ScenarioTools.ImageProcessing;
using ScenarioTools.ImageProvider;
using ScenarioTools.Numerical;
using ScenarioTools.Spatial;
using ScenarioTools.TreeElementFactories;
using ScenarioTools.Util;
using ScenarioTools.Xml;

namespace ScenarioTools.Reporting
{
    public class DataSeries : IImageProvider, IDeepCloneable
    {
        #region Enumerations
        public enum ContourSpecificationMethod
        {
            Automatic = 0,
            EqualInterval = 1,
            ValueList = 2
        }
        #endregion Enumerations

        #region Constants
        private const long INVALID_TICKS = -1;
        private const string XML_NODE_NAME = "DataSeries";
        private const string XML_KEY_MAP_TYPE = "mapType";
        private const string XML_KEY_NAME = "name";
        private const string XML_KEY_LINE_SERIES_COLOR = "lineColor";
        private const string XML_KEY_POINT_SERIES_COLOR = "pointColor";
        private const string XML_KEY_RAMP_COLOR_0 = "rampColor0";
        private const string XML_KEY_RAMP_COLOR_1 = "rampColor1";
        private const string XML_KEY_PLOT_COLOR = "plotColor";
        private const string XML_KEY_START_CONTOUR = "startContour";
        private const string XML_KEY_CONTOUR_INTERVAL = "contourInterval";
        private const string XML_KEY_CONTOUR_SPECIFICATION_METHOD = "contourSpecificationMethod";
        private const string XML_KEY_VISIBLE = "visible";
        private const string XML_KEY_CONVERT_FLOW_TO_FLUX = "convertFlowToFlux";
        private static readonly DataSeries.ContourSpecificationMethod DEFAULT_CONTOUR_SPECIFICATION_METHOD = ContourSpecificationMethod.Automatic;
        private static readonly Color DEFAULT_LINE_SERIES_COLOR = Color.Black;
        private static readonly Color DEFAULT_POINT_SERIES_COLOR = Color.Transparent;
        private static readonly Color DEFAULT_RAMP_COLOR_0 = Color.Black;
        private static readonly Color DEFAULT_RAMP_COLOR_1 = Color.White;
        private static readonly Color DEFAULT_PLOT_COLOR = Color.FromArgb(0, 0, 0);
        private static readonly bool DEFAULT_VISIBLE = true;
        private static readonly bool DEFAULT_CONVERT_FLOW_TO_FLUX = false;
        private const float ERROR_MESSAGE_FONT_SIZE = 15.0f;
        #endregion Constants

        #region Fields
        private string _name;
        private Color _lineSeriesColor;
        private Color _pointSeriesColor;
        private IReportElement _parentElement;
        private IDataProvider _dataProvider;
        private DataSeriesTypeEnum _dataSeriesType;
        private DataSeries.ContourSpecificationMethod _contourSpecificationType;
        private float _startContour;
        private float _contourInterval;
        private float[] _contourValues;
        private PointList _contourPointList;
        private DateTime _datasetTime;
        #endregion Fields

        #region Constructors
        public DataSeries(DataSeriesTypeEnum dataSeriesType)
        {
            Visible = true;
            ConvertFlowToFlux = false;

            // Store the data series type.
            this._dataSeriesType = dataSeriesType;

            // Set the default symbol colors.
            this.LineSeriesColor = DEFAULT_LINE_SERIES_COLOR;
            this.PointSeriesColor = DEFAULT_POINT_SERIES_COLOR;
            this.RampColor0 = DEFAULT_RAMP_COLOR_0;
            this.RampColor1 = DEFAULT_RAMP_COLOR_1;

            // Set default dataset to be as old as possible
            this._datasetTime = DateTime.MinValue;

            // Set other defaults
            this._contourSpecificationType = ContourSpecificationMethod.Automatic;
        }
        public DataSeries(DataSeries dataSeries) : this(dataSeries.DataSeriesType)
        {
            this._name = dataSeries._name;
            this._lineSeriesColor = dataSeries._lineSeriesColor;
            this._pointSeriesColor = dataSeries._pointSeriesColor;
            this.RampColor0 = dataSeries.RampColor0;
            this.RampColor1 = dataSeries.RampColor1;
            this._parentElement = dataSeries._parentElement;
            this._dataProvider = (IDataProvider)dataSeries._dataProvider.DeepClone();
            this._dataProvider.AssignParent(dataSeries.ParentElement); // this is probably temporary
            this._contourSpecificationType = dataSeries._contourSpecificationType;
            this._startContour = dataSeries._startContour;
            this._contourInterval = dataSeries._contourInterval;
            this.Visible = dataSeries.Visible;
            this.ConvertFlowToFlux = dataSeries.ConvertFlowToFlux;
            if (dataSeries._contourValues != null)
            {
                this._contourValues = new float[dataSeries._contourValues.Length];
                for (int i = 0; i < dataSeries._contourValues.Length; i++)
                {
                    this._contourValues[i] = dataSeries._contourValues[i];
                }
            }
        }
        #endregion Constructors

        #region Properties
        public DataSeries.ContourSpecificationMethod ContourSpecificationType
        {
            get
            {
                return _contourSpecificationType;
            }
            set
            {
                _contourSpecificationType = value;
            }
        }
        public float StartContour
        {
            get
            {
                return _startContour;
            }
            set
            {
                _startContour = value;
            }
        }
        public float ContourInterval
        {
            get
            {
                if (_contourInterval == 0.0f)
                {
                    _contourInterval = 1.0f;
                }
                return _contourInterval;
            }
            set
            {
                _contourInterval = value;
            }
        }
        public float[] ContourValues
        {
            get
            {
                if (_contourValues == null)
                {
                    _contourValues = new float[] { 0.0f, 1.0f };
                }
                return _contourValues;
            }
            set
            {
                _contourValues = value;
            }
        }
        public bool Visible { get; set; }
        public bool ConvertFlowToFlux { get; set; }
        public IReportElement ParentElement
        {
            get
            {
                return _parentElement;
            }
            set
            {
                _parentElement = value;
            }
        }
        public string Name
        {
            get
            {
                if (_name == null)
                {
                    Name = "";
                }
                return _name;
            }
            set
            {
                // This is the new value that was provided.
                string newName = value;

                // First, check if it is valid (not null, not an empty string).
                bool isValid = false;
                if (newName != null)
                {
                    newName = newName.Trim();
                    if (!newName.Equals(""))
                    {
                        isValid = true;
                    }
                }

                // If the new name is not valid, make a name for it.
                if (!isValid)
                {
                    // This is a special case when the data provider is null. In this case, we just want to set an empty string so that a proper name
                    // can be assigned when the data provider is no longer null.
                    if (this.DataProvider == null)
                    {
                        newName = "";
                    }
                    else
                    {
                        newName = this.DataProvider.ToString();
                    }
                }

                // Only continue if the new name is not empty.
                if (!newName.Equals(""))
                {
                    if (_parentElement != null)
                    {
                        // Check the name with the parent report element.
                        if (!_parentElement.IsUniqueName(newName, this))
                        {
                            int index = 2;
                            string modifiedName = newName + " (" + index++ + ")";
                            while (!_parentElement.IsUniqueName(modifiedName, this))
                            {
                                modifiedName = newName + " (" + index++ + ")";
                            }
                            newName = modifiedName;
                        }
                    }
                }

                _name = newName;
            }
        }
        public Color LineSeriesColor
        {
            get
            {
                return _lineSeriesColor;
            }
            set
            {
                _lineSeriesColor = value;
            }
        }
        public Color PointSeriesColor
        {
            get
            {
                return _pointSeriesColor;
            }
            set
            {
                _pointSeriesColor = value;
            }
        }
        public Color RampColor0 { get; set; }
        public Color RampColor1 { get; set; }
        public DataConsumerTypeEnum DataConsumerType {
            get
            {
                if (_parentElement is ReportElementSTMap)
                {
                    return DataConsumerTypeEnum.STMap;
                }
                else if (_parentElement is ReportElementTable)
                {
                    return DataConsumerTypeEnum.Table;
                }
                else if (_parentElement is ReportElementChart)
                {
                    return DataConsumerTypeEnum.Chart;
                }

                return DataConsumerTypeEnum.None;
            }
        }
        public IDataProvider DataProvider
        {
            get
            {
                return _dataProvider;
            }
            set
            {
                _dataProvider = value;
            }
        }
        public DataSeriesTypeEnum DataSeriesType
        {
            get
            {
                return _dataSeriesType;
            }
            set
            {
                _dataSeriesType = value;
            }
        }
        #endregion Properties

        public XmlNode GetXmlNode(XmlDocument document, string targetFileName)
        {
            // Create the element that represents this object.
            XmlElement element = document.CreateElement(XML_NODE_NAME);

            // Prepare the element.
            prepareXmlElement(document, element, targetFileName);

            // Return the result.
            return element;
        }
        public void SaveXmlFile(string file)
        {
            // Make the XML file.
            XmlDocument document = XmlUtil.CreateXmlFile(file, XML_NODE_NAME);

            // Prepare the root element.
            prepareXmlElement(document, document.DocumentElement, file);

            // Write the file.
            document.Save(file);
        }
        private void prepareXmlElement(XmlDocument document, XmlElement element, string targetFileName)
        {
            // If this is a map, set the map-type attribute.
            if (DataSeriesType == DataSeriesTypeEnum.ColorFillMapSeries || DataSeriesType == DataSeriesTypeEnum.ContourMapSeries)
            {
                XmlUtil.FrugalSetAttribute(element, XML_KEY_MAP_TYPE, DataSeriesType == DataSeriesTypeEnum.ColorFillMapSeries ? "fill" : "contour", "");
            }

            // Set the attributes of this object.
            XmlUtil.FrugalSetAttribute(element, XML_KEY_NAME, _name, "");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_LINE_SERIES_COLOR, XmlUtil.ColorToXmlString(_lineSeriesColor), 
                XmlUtil.ColorToXmlString(DEFAULT_LINE_SERIES_COLOR));
            XmlUtil.FrugalSetAttribute(element, XML_KEY_POINT_SERIES_COLOR, XmlUtil.ColorToXmlString(_pointSeriesColor), 
                XmlUtil.ColorToXmlString(DEFAULT_POINT_SERIES_COLOR));
            XmlUtil.FrugalSetAttribute(element, XML_KEY_RAMP_COLOR_0, XmlUtil.ColorToXmlString(RampColor0), XmlUtil.ColorToXmlString(DEFAULT_RAMP_COLOR_0));
            XmlUtil.FrugalSetAttribute(element, XML_KEY_RAMP_COLOR_1, XmlUtil.ColorToXmlString(RampColor1), XmlUtil.ColorToXmlString(DEFAULT_RAMP_COLOR_1));
            XmlUtil.FrugalSetAttribute(element, XML_KEY_START_CONTOUR, _startContour.ToString("0.000000"), "0.00");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_CONTOUR_INTERVAL, _contourInterval.ToString("0.0000000"), "1.00");
            string sdfg = _contourSpecificationType.ToString();
            int asdf = (int)_contourSpecificationType;
            XmlUtil.FrugalSetAttribute(element, XML_KEY_CONTOUR_SPECIFICATION_METHOD, ((int)_contourSpecificationType).ToString(),
                                       ((int)DataSeries.ContourSpecificationMethod.Automatic).ToString());
            XmlUtil.FrugalSetAttribute(element, XML_KEY_VISIBLE, Visible, DEFAULT_VISIBLE);
            XmlUtil.FrugalSetAttribute(element, XML_KEY_CONVERT_FLOW_TO_FLUX, ConvertFlowToFlux, DEFAULT_CONVERT_FLOW_TO_FLUX);

            // Add the data provider as a child node.
            if (_dataProvider != null)
            {
                element.AppendChild(_dataProvider.GetXmlNode(document, targetFileName));
            }
        }
        public void InitFromXml(IReportElement parentElement, XmlElement element, string sourceFileName)
        {
            // Store a reference to the parent element.
            this._parentElement = parentElement;

            // Determine the data series type.
            if (parentElement is ReportElementChart)
            {
                DataSeriesType = DataSeriesTypeEnum.ChartSeries;
            }
            else if (parentElement is ReportElementTable)
            {
                DataSeriesType = DataSeriesTypeEnum.TableSeries;
            }

            // Get the attributes from the XML element.
            Name = XmlUtil.SafeGetStringAttribute(element, XML_KEY_NAME, "");
            LineSeriesColor = XmlUtil.SafeGetColorAttribute(element, XML_KEY_LINE_SERIES_COLOR, DEFAULT_LINE_SERIES_COLOR);
            PointSeriesColor = XmlUtil.SafeGetColorAttribute(element, XML_KEY_POINT_SERIES_COLOR, DEFAULT_POINT_SERIES_COLOR);
            RampColor0 = XmlUtil.SafeGetColorAttribute(element, XML_KEY_RAMP_COLOR_0, DEFAULT_RAMP_COLOR_0);
            RampColor1 = XmlUtil.SafeGetColorAttribute(element, XML_KEY_RAMP_COLOR_1, DEFAULT_RAMP_COLOR_1);

            StartContour = XmlUtil.SafeGetFloatAttribute(element, XML_KEY_START_CONTOUR, 0.0f);
            ContourInterval = XmlUtil.SafeGetFloatAttribute(element, XML_KEY_CONTOUR_INTERVAL, 1.0f);
            _contourSpecificationType = (ContourSpecificationMethod)XmlUtil.SafeGetIntAttribute(element, XML_KEY_CONTOUR_SPECIFICATION_METHOD, (int)ContourSpecificationMethod.Automatic);
            Visible = XmlUtil.SafeGetBoolAttribute(element, XML_KEY_VISIBLE, DEFAULT_VISIBLE);
            ConvertFlowToFlux = XmlUtil.SafeGetBoolAttribute(element, XML_KEY_CONVERT_FLOW_TO_FLUX, DEFAULT_CONVERT_FLOW_TO_FLUX);

            // Get the data provider from the child node.
            if (parentElement is ReportElementSTMap)
            {
                DataProvider = DataProviderFactory.STMapDataProviderFromXml((XmlElement)element.ChildNodes[0], (ReportElementSTMap)parentElement, sourceFileName);
            }
            else if (parentElement is ReportElementTable)
            {
                DataProvider = DataProviderFactory.GraphDataProviderFromXml((XmlElement)element.ChildNodes[0], parentElement, sourceFileName);
            }
            else if (parentElement is ReportElementChart)
            {
                DataProvider = DataProviderFactory.ChartDataProviderFromXml((XmlElement)element.ChildNodes[0], parentElement, sourceFileName);
            }
            //_datasetTime = DataProvider.FileModificationTime;
        }

        public override string ToString()
        {
            string unnamed = "Unnamed Data Series";
            return Name == null ? unnamed : (Name.Equals("") ? unnamed : Name);
        }

        #region IImageProvider Members
        private string[,] getTableData(out bool isTruncated, out int rowsInFullTable)
        {
            // Get the time series from the data provider.
            int datasetStatus;
            object timeSeriesObject = this.GetData(out datasetStatus);
            if (timeSeriesObject == null)
            {
                isTruncated = false;
                rowsInFullTable = 0;
                return new string[,] { { "Data set status: " + datasetStatus } };
            }
            TimeSeries timeSeries = (TimeSeries)timeSeriesObject;

            // Make the string array that will store the data for this table. For the preview, limit to 30 data rows.
            int numColumns = 2;
            int numRows = timeSeries.Length + 1;
            isTruncated = false;
            int maxNumRows = 31;
            if (numRows > maxNumRows)
            {
                numRows = maxNumRows;
                isTruncated = true;
            }
            string[,] tableData = new string[numColumns, numRows];
            tableData[1, 0] = this.Name;
            if (_parentElement is ReportElementTable)
            {
                bool includeTimeInTable = ((ReportElementTable)_parentElement).IncludeTimeInTable;

                // Populate the header rows in the table.
                    if (includeTimeInTable)
                {
                    tableData[0, 0] = "Date and Time";
                }
                else
                {
                    tableData[0, 0] = "Date";
                }

                // Populate the dates in the table.
                for (int i = 0; i < numRows - 1; i++)
                {
                    // Optionally include Time as well as Date
                    if (includeTimeInTable)
                    {
                        tableData[0, i + 1] = timeSeries[i].Date.ToShortDateString() + " " + timeSeries[i].Date.ToShortTimeString();
                    }
                    else
                    {
                        tableData[0, i + 1] = timeSeries[i].Date.ToShortDateString();
                    }
                }
            }

            // Populate the rows for this series.
            for (int i = 0; i < numRows - 1; i++)
            {
                float value = timeSeries[i].Value;
                tableData[1, i + 1] = value.ToString();
            }
            
            // Return the table data.
            rowsInFullTable = timeSeries.Length;
            return tableData;
        }
        public object GetData(out int datasetStatus)
        {
            // Get the data from the data provider.
            object dataset = this.DataProvider.GetData(out datasetStatus);

            // If this is a contour map, modify the dataset status according to the contour conditions.
            if (this.DataSeriesType == DataSeriesTypeEnum.ContourMapSeries)
            {
                if (datasetStatus == DataStatus.DATA_AVAILABLE_CACHE_PRESENT)
                {
                    if (this._contourPointList == null)
                    {
                        datasetStatus = DataStatus.DATASET_NEEDS_REFRESH;
                    }
                }
            }

            // Return the dataset.
            return dataset;
        }
        public int GetDataStatus()
        {
            int datasetStatus;
            GetData(out datasetStatus);
            // If the DataProvider modification time is later (>) than the
            // dataset time, the dataset needs to be refreshed
            //if (DataProvider.FileModificationTime <= _datasetTime)
            //{
                return datasetStatus;
            //}
            //else
            //{
            //    return DataStatus.DATASET_NEEDS_REFRESH;
            //}
        }
        public Image GetImage()
        {
            return GetImage(float.NaN, float.NaN, INVALID_TICKS, INVALID_TICKS, double.NaN, double.NaN, double.NaN, double.NaN, true);
        }
        public Image GetImage(float minValue, float maxValue, long minTicks, long maxTicks, 
                              double xcMin, double xcMax, double ycMin, double ycMax, bool isStandalone)
        {
            // If this is table, get the table data and return an image with it.
            if (this.DataConsumerType == DataConsumerTypeEnum.Table)
            {
                // Get the values from the data provider.
                bool isTruncated;
                int rowsInFullTable;
                string[,] values = getTableData(out isTruncated, out rowsInFullTable);

                // Return the image.
                return TableMaker.MakeTableImage(this.Name, values, isTruncated, rowsInFullTable, ((ReportElementTable)_parentElement).IncludeTimeInTable);
            }

            else if (this.DataConsumerType == DataConsumerTypeEnum.Chart)
            {
                int width = ReportElementChart.DEFAULT_CHART_WIDTH;
                int height = ReportElementChart.DEFAULT_CHART_HEIGHT;
                Bitmap image = new Bitmap(width, height);

                // Create the graphics for the image.
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);

                // Draw the content.
                DrawContent(g, width, height, minValue, maxValue, minTicks, maxTicks, xcMin, xcMax, ycMin, ycMax, isStandalone);

                // Dispose of the graphics object to reclaim resources.
                g.Dispose();

                return image;
            }

            // Otherwise, make the appropriate image.
            else
            {
                int width = 800;
                int height = this.DataConsumerType == DataConsumerTypeEnum.Chart ? 400 : 800;
                Bitmap image = new Bitmap(width, height);

                // Create the graphics for the image.
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);

                // Draw the content.
                DrawContent(g, width, height, minValue, maxValue, minTicks, maxTicks, xcMin, xcMax, ycMin, ycMax, isStandalone);

                // Dispose of the graphics object to reclaim resources.
                g.Dispose();

                return image;
            }
        }
        public void ClearImage()
        {
            if (ParentElement != null)
            {
                ParentElement.ClearImage();
            }
        }
        public void DrawContent(Chart chart, int width, int height, float minValue, float maxValue, long minTicks, long maxTicks,
                                double xcMin, double xcMax, double ycMin, double ycMax)
        {
            #region Draw Chart Content
            chart.Width = width;
            chart.Height = height;
            int markerSize = height / 120;
            int dataStatus;
            int numPoints = GetNumTableRows();
            if (!chart.Series.IsUniqueName(_name))
            {
                chart.Series.Remove(chart.Series[_name]);
            }
            Series series = new Series(_name); // [numPoints];

            // Assign series line and marker properties according to DataSeries properties
            series.Color = this._lineSeriesColor;
            series.MarkerColor = this._pointSeriesColor;
            series.MarkerSize = markerSize;
            if (this.LineSeriesColor == Color.Transparent)
            {
                series.ChartType = SeriesChartType.Point;
            }
            else
            {
                series.ChartType = SeriesChartType.Line;
            }
            if (this._pointSeriesColor == Color.Transparent)
            {
                series.MarkerStyle = MarkerStyle.None;
            }
            else
            {
                series.MarkerStyle = MarkerStyle.Circle;
            }

            // Ned TODO: For charts where X axis is not DateTime, need to add other options here
            series.XValueType = ChartValueType.DateTime;
            object dataset = this.GetData(out dataStatus);
            TimeSeries ts = (TimeSeries)dataset;
            for (int i = 0; i < numPoints; i++)
            {
                System.DateTime x = ts[i].Date;
                series.Points.AddXY(x.ToOADate(), Convert.ToDouble(ts[i].Value));
            }
            chart.Series.Add(series);
            #endregion Draw Chart Content
        }
        public void DrawContent(System.Drawing.Graphics g, int width, int height, float minValue,
            float maxValue, long minTicks, long maxTicks, double xcMin,
            double xcMax, double ycMin, double ycMax, bool isStandalone)
        {
            //// If the data provider type is a graph, draw a graph.
            //if (this.DataConsumerType == DataConsumerTypeEnum.Graph)
            //#region Draw Graph Content
            //{
            //    drawContentGraph(g, width, height, minValue, maxValue, minTicks, maxTicks, isStandalone);
            //}
            //#endregion Draw Graph Content
            //else 
            if (this.DataConsumerType == DataConsumerTypeEnum.Map)
            #region Draw Map Content
            {
                // If the coordinates are NaN, set them to the extent of this dataset.
                if (double.IsNaN(xcMin) || double.IsNaN(xcMax) || double.IsNaN(ycMin) || double.IsNaN(ycMax))
                {
                    int datasetStatus;
                    object mapObject = this.GetData(out datasetStatus);
                    if (mapObject != null)
                    {
                        GeoMap map = (GeoMap)mapObject;
                        Extent extent = map.GetExtent();
                        xcMin = extent.West;
                        xcMax = extent.East;
                        ycMin = extent.South;
                        ycMax = extent.North;

                        // Adjust the extent.
                        double ratioX = (xcMax - xcMin) / width;
                        double ratioY = (ycMax - ycMin) / height;
                        double ratio = Math.Max(ratioX, ratioY);
                        double xAdd = ((ratio * width) - (xcMax - xcMin)) / 2.0f;
                        double yAdd = ((ratio * height) - (ycMax - ycMin)) / 2.0f;
                        xcMin -= xAdd;
                        xcMax += xAdd;
                        ycMin -= yAdd;
                        ycMax += yAdd;
                    }
                }


                if (this.DataSeriesType == DataSeriesTypeEnum.ColorFillMapSeries)
                {
                    drawContentMapColorFill(g, width, height, xcMin, xcMax, ycMin, ycMax);
                }
                else
                {
                    drawContentMapContour(g, width, height, xcMin, xcMax, ycMin, ycMax);
                }
            }
            #endregion Draw Map Content
        }
        private void drawContentGraph(System.Drawing.Graphics g, int width, int height, float minValue, float maxValue, long minTicks, long maxTicks, 
            bool isStandalone)
        {
            try
            {
                int leftBuffer = 20;
                int rightBuffer = 20;
                int bottomBuffer = 20;
                int topBuffer = 20;
                float graphWidth = width - leftBuffer - rightBuffer;
                float graphHeight = height - topBuffer - bottomBuffer;

                // Get the dataset from the data provider. It will be either a time series or a floating-point value.
                int datasetStatus;
                object dataObject = _dataProvider.GetData(out datasetStatus);

                // If the data array is null, draw an error message and return.
                if (dataObject == null)
                {
                    if (isStandalone)
                    {
                        drawErrorMessage(g, width, height);
                        Console.WriteLine("drawing error message " + minValue + " : " + maxValue);
                    }
                    return;
                }

                // Otherwise, draw the data.
                else
                {
                    // If the data object is a floating-point value, show it appropriately.
                    if (dataObject is float)
                    {
                        // Cast the object to a floating-point number.
                        float dataValue = (float)dataObject;

                        // If the min or max is NaN, show the value as a string. It wouldn't make sense to draw the line, because there is nothing 
                        // against  which it could be compared.
                        if (float.IsNaN(minValue) || float.IsNaN(maxValue))
                        {
                            drawMessage(new string[] { "value: " + dataValue }, g, width, height);
                        }

                        // If the min and max are already defined, draw the value in the appropriate position.
                        else
                        {
                            Pen pen = new Pen(new SolidBrush(_lineSeriesColor), 1.0f);
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            float x0 = leftBuffer;
                            float x1 = width - rightBuffer;
                            float y = topBuffer + (maxValue - dataValue) / (maxValue - minValue) * graphHeight;
                            g.DrawLine(pen, x0, y, x1, y);
                        }
                    }

                    // If the data object is a time series, draw it appropriately.
                    else if (dataObject is TimeSeries)
                    {
                        // Cast the data object to a time series.
                        TimeSeries dataSeries = (TimeSeries)dataObject;

                        // Only continue if the data series is longer than zero length.
                        if (dataSeries.Length > 0)
                        {
                            // If the min or max is NaN, compute both of them.
                            if (float.IsNaN(minValue) || float.IsNaN(maxValue))
                            {
                                // Determine the minimum and maximum of all data series.
                                minValue = dataSeries[0].Value;
                                maxValue = dataSeries[0].Value;
                                for (int i = 1; i < dataSeries.Length; i++)
                                {
                                    minValue = MathUtil.NanAwareMin(minValue, dataSeries[i].Value);
                                    maxValue = MathUtil.NanAwareMax(maxValue, dataSeries[i].Value);
                                }
                            }

                            // If the min or max date are invalid, compute them.
                            if (minTicks == INVALID_TICKS || maxTicks == INVALID_TICKS)
                            {
                                // Determine the minimum and maximum values of all data series.
                                minTicks = this.GetMinTicksOfGraphDataSeries();
                                maxTicks = this.GetMaxTicksOfGraphDataSeries();
                            }

                            // Compute the range. If it is zero, make it one.
                            float range = maxValue - minValue;
                            if (range == 0.0f)
                            {
                                maxValue += 1.0f;
                                range = 1.0f;
                            }

                            // Compute the ticks (date) range. If it is zero, make it one.
                            double ticksRange = maxTicks - minTicks;
                            if (ticksRange == 0.0)
                            {
                                maxTicks += 1;
                                ticksRange = 1.0;
                            }

                            Console.WriteLine("min: " + minValue + ", max: " + maxValue);

                            // Draw the line data series in the appropriate line color.
                            Pen pen = new Pen(new SolidBrush(_lineSeriesColor), 1.0f);
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                            float xc0 = 0.0f;
                            float yc0 = 0.0f;
                            for (int i = 0; i < dataSeries.Length; i++)
                            {
                                float xc1 = (float)(leftBuffer + (dataSeries[i].Date.Ticks - minTicks) / ticksRange * graphWidth);
                                float yc1 = topBuffer + (maxValue - dataSeries[i].Value) / range * graphHeight;

                                if (i != 0 && !float.IsNaN(yc0) && !float.IsNaN(yc1))
                                {
                                    if (xc1 < xc0)
                                    {
                                        pen = new Pen(new SolidBrush(RgbPixel.GetRainbowColor().ToColor()));
                                        Console.WriteLine("Problem at index " + i);
                                    }
                                    g.DrawLine(pen, xc0, yc0, xc1, yc1);
                                }
                                xc0 = xc1;
                                yc0 = yc1;
                            }

                            // Draw the point data series in the appropriate line color.
                            pen = new Pen(new SolidBrush(this.PointSeriesColor), 1.0f);
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            for (int i = 0; i < dataSeries.Length; i++)
                            {
                                //float xc1 = leftBuffer + i * graphWidth / (dataSeries.Length - 1.0f);
                                float xc = (float)(leftBuffer + (dataSeries[i].Date.Ticks - minTicks) / ticksRange * graphWidth);
                                float yc = topBuffer + (maxValue - dataSeries[i].Value) / range * graphHeight;

                                g.DrawEllipse(pen, xc - 2, yc - 2, 4, 4);
                            }

                        }
                    }
                }
            }
            catch { }
        }
        private void drawContentMapColorFill3(System.Drawing.Graphics g, int imageWidth, int imageHeight, float xcMin, float xcMax, float ycMin, float ycMax)
        {
            // Get the data from the data provider.
            int datasetStatus;
            GeoMap data = (GeoMap)_dataProvider.GetData(out datasetStatus);

            // Ned TODO: Replace hard-coded coordinate values
            float xDataMin = 543750.0f;
            float xDataMax = xDataMin + 50.0f * 930.0f;
            float yDataMin = 2872250.0f;
            float yDataMax = yDataMin + 50.0f * 1730.0f;

            // If the data array is null, draw an error message and return.
            if (data == null)
            {
                drawErrorMessage(g, imageWidth, imageHeight);
                return;
            }

            // Only continue if the data series is not null.
            if (data != null)
            {
                float minValue = float.NaN;
                float maxValue = float.NaN;

                int dataWidth = data.NCols;
                int dataHeight = data.NRows;

                // If the min or max is NaN, compute both of them.
                if (float.IsNaN(minValue) || float.IsNaN(maxValue))
                {
                    // Determine the minimum and maximum of all data series.
                    for (int j = 0; j < dataHeight; j++)
                    {
                        for (int i = 0; i < dataWidth; i++)
                        {
                            minValue = MathUtil.NanAwareMin(minValue, data.GetValueAtCell(i, j));
                            maxValue = MathUtil.NanAwareMax(maxValue, data.GetValueAtCell(i, j));
                        }
                    }
                }

                // Compute the range. If it is zero, make it one.
                float range = maxValue - minValue;
                if (range == 0.0f)
                {
                    maxValue += 1.0f;
                    range = 1.0f;
                }

                // Compute the ratio for rendering the map.
                float ratioHorizontal = imageWidth / (float)dataWidth;
                float ratioVertical = imageHeight / (float)dataHeight;
                float ratio = Math.Min(ratioHorizontal, ratioVertical);

                float xImageCellSize = (xcMax - xcMin) / imageWidth;
                float yImageCellSize = (ycMax - ycMin) / imageHeight;

                // Compute the image buffers.
                int leftBuffer = (int)((imageWidth - dataWidth * ratio) / 2);
                int topBuffer = (int)((imageHeight - dataHeight * ratio) / 2);

                RgbaPixel[,] imageRgba = new RgbaPixel[imageWidth, imageHeight];
                RgbaPixel.SetSolidColor(imageRgba, RgbaPixel.Clear);

                for (int j = 0; j < imageHeight; j++)
                {
                    int y = (int)((j * yImageCellSize - ycMin + yDataMin) / 50.0f) - 1730 - 20;
                    for (int i = 0; i < imageWidth; i++)
                    {
                        int x = (int)((i * xImageCellSize + xcMin - xDataMin) / 50.0f);
                        if (x >= 0 && y >= 0 && x < dataWidth && y < dataHeight)
                        {
                            if (!float.IsNaN(data.GetValueAtCell(x, y)))
                            {
                                int gray = (int)((data.GetValueAtCell(x, y) - minValue) / range * 255);
                                byte grayB = (byte)gray;
                                imageRgba[i, j] = new RgbaPixel(grayB, grayB, grayB, 255);
                            }
                        }
                    }
                }
                Image image = RgbaPixelArrayToImage.Convert(imageRgba);

                g.DrawImage(image, 0.0f, 0.0f);
            }
        }
        private void drawContentMapColorFill(System.Drawing.Graphics g, int imageWidth, int imageHeight,
                                              double xcMin, double xcMax, double ycMin, double ycMax)
        {
            // Get the data from the data provider.
            int datasetStatus;
            object dataObject = _dataProvider.GetData(out datasetStatus);
            GeoMap data = dataObject == null ? null : (GeoMap)_dataProvider.GetData(out datasetStatus);

            // If the data array is null, draw an error message and return.
            if (data == null)
            {
                drawErrorMessage(g, imageWidth, imageHeight);
                return;
            }

            float minValue = float.NaN;
            float maxValue = float.NaN;

            int nCols = data.NCols;
            int nRows = data.NRows;

            // If the min or max is NaN, compute both of them.
            if (float.IsNaN(minValue) || float.IsNaN(maxValue))
            {
                // Determine the minimum and maximum of all data series.
                for (int j = 0; j < nRows; j++)
                {
                    for (int i = 0; i < nCols; i++)
                    {
                        minValue = MathUtil.NanAwareMin(minValue, data.GetValueAtCell(i, j));
                        maxValue = MathUtil.NanAwareMax(maxValue, data.GetValueAtCell(i, j));
                    }
                }
            }

            // Compute the range. If it is zero, make it one.
            float range = maxValue - minValue;
            if (range == 0.0f)
            {
                maxValue += 1.0f;
                range = 1.0f;
            }

            Extent extent = data.GetExtent();
            double pixelSizeX = (xcMax - xcMin) / imageWidth;
            double pixelSizeY = (ycMax - ycMin) / imageHeight;
            double pixelSize = (pixelSizeX + pixelSizeY) / 2.0f;

            // Make the image and clear the background.
            RgbaPixel[,] imageRgba = new RgbaPixel[imageWidth, imageHeight];
            RgbaPixel.SetSolidColor(imageRgba, RgbaPixel.Clear);

            // Draw the pixels of the image.
            try
            {
                for (int j = 0; j < imageHeight; j++)
                {
                    float y = Convert.ToSingle(ycMin + (imageHeight - j - 0.5f) * pixelSize);
                    for (int i = 0; i < imageWidth; i++)
                    {
                        float x = Convert.ToSingle(xcMin + (i + 0.5f) * pixelSize);
                        float value = data.GetValueAtPoint(new PointF(x, y));
                        if (!float.IsNaN(value))
                        {
                            int gray = (int)((value - minValue) / range * 255);
                            byte grayB = (byte)gray;
                            imageRgba[i, j] = new RgbaPixel(grayB, grayB, grayB, 255);
                        }
                    }
                }
            }
            catch
            {
            }
            Image image = RgbaPixelArrayToImage.Convert(imageRgba);

            g.DrawImage(image, 0.0f, 0.0f);
        }
        private void generateContourPointList()
        {
            // Get the data from the data provider.
            int datasetStatus;
            object datasetObject = this.GetData(out datasetStatus);
            GeoMap data = datasetObject == null ? null : (GeoMap)datasetObject;

            // If the dataset is null, return.
            if (data == null)
            {
                return;
            }

            // Make and configure a contour object.
            ContourMaker contourMaker = new ContourMaker();
            int scaleFactor = 1;
            if (this.ContourSpecificationType == DataSeries.ContourSpecificationMethod.EqualInterval)
            {
                contourMaker.AssignContourValues(this.StartContour, this.ContourInterval);
            }
            else
            {
                contourMaker.AssignContourValues(this.ContourValues);
            }
            contourMaker.SetGrid(convertArray(data.GetValueArray(), scaleFactor));
            contourMaker.SetExtent(data.GetExtent());

            // Get the contour lines from the contour maker.
            PointList pointList = new PointList();
            contourMaker.MakeContours(pointList);

            // Store the contour lines in the class variable.
            this._contourPointList = pointList;
        }
        private void drawContentMapContour(System.Drawing.Graphics g, int imageWidth, int imageHeight,
                                           double xcMin, double xcMax, double ycMin, double ycMax)
        {
            // Get a local reference to the contour point list.
            PointList contourPointListLocal = this._contourPointList;

            // If the contour point list is null, draw an error message and return.
            if (contourPointListLocal == null)
            {
                //drawErrorMessage(g, imageWidth, imageHeight);
                return;
            }            

            // Show the number of contours.
            Console.WriteLine("Num contours: " + contourPointListLocal.GetNumLines());

            // If the number of contours is zero, draw a message.
            if (contourPointListLocal.GetNumLines() == 0)
            {
                string s = "The map is valid, but no contours were generated.";
                Font f = new Font(FontFamily.GenericSansSerif, ERROR_MESSAGE_FONT_SIZE, FontStyle.Bold);
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                g.DrawString(s, f, Brushes.Black, imageWidth / 2.0f, imageHeight / 2.0f, stringFormat);
            }

            // Make a list for the image points at which labels are drawn.
            List<Point2D> labelPoints = new List<Point2D>();
            List<string> labelStrings = new List<string>();
            int borderSize = 15;

            // get the polylines from the point list.
            Polyline2D[] polylines = contourPointListLocal.GetPolylines();
            Pen pen = new Pen(new SolidBrush(this.LineSeriesColor));
            foreach (Polyline2D polyline in polylines)
            {
                // Draw the line segments that comprise the contour.
                bool isVisible = false;
                Point2D labelPoint = new Point2D(-10, -10);
                double labelPointDistance = double.MinValue;

                // Iterate though all segments of this polyline
                for (int j = 1; j < polyline.NumVertices; j++)
                {
                    // Draw current segment
                    float x0 = (float)((polyline.GetVertex(j - 1).X - xcMin) / (xcMax - xcMin) * imageWidth);
                    float y0 = (float)((ycMax - polyline.GetVertex(j - 1).Y) / (ycMax - ycMin) * imageHeight);
                    float x1 = (float)((polyline.GetVertex(j).X - xcMin) / (xcMax - xcMin) * imageWidth);
                    float y1 = (float)((ycMax - polyline.GetVertex(j).Y) / (ycMax - ycMin) * imageHeight);

                    // Compute the midpoint.
                    float xMid = (x0 + x1) / 2.0f;
                    float yMid = (y0 + y1) / 2.0f;

                    // Determine if contour label will be placed next to this segment
                    // If the midpoint is contained in the image and far enough from the edge, 
                    // it is a candidate for the label point.
                    if (xMid >= borderSize && xMid <= imageWidth - borderSize 
                        && yMid >= borderSize && yMid <= imageHeight - borderSize)
                    {
                        isVisible = true;

                        // If the point list is empty, this midpoint as the label point.
                        Point2D midpoint = new Point2D(xMid, yMid);
                        if (labelPoints.Count == 0)
                        {
                            labelPointDistance = double.MaxValue;
                            labelPoint = midpoint;
                        }

                        // Otherwise, find the minimum distance to all other points in the list.
                        else
                        {
                            double minDistance = midpoint.distance(labelPoints[0]);
                            for (int i = 1; i < labelPoints.Count && minDistance > labelPointDistance; i++)
                            {
                                double distance = midpoint.distance(labelPoints[i]);
                                if (distance < minDistance)
                                {
                                    minDistance = distance;
                                }
                            }
                            if (minDistance > labelPointDistance)
                            {
                                labelPoint = midpoint;
                                labelPointDistance = minDistance;
                            }
                        }
                    }
                    g.DrawLine(pen, x0, y0, x1, y1);
                }

                // If the label would be visible, assign label and label point for current polyline.
                if (isVisible && labelPointDistance > 10)
                {
                    labelStrings.Add(polyline.GetAttributeValue("contour") + "");
                    labelPoints.Add(labelPoint);
                }
            }
            // Done drawing all contour polylines

            // Draw all contour labels -- Label contours
            Brush brush = new SolidBrush(this.LineSeriesColor);
            for (int i = 0; i < labelPoints.Count; i++)
            {
                g.DrawString(labelStrings[i], SystemFonts.SmallCaptionFont, brush, 
                    new PointF((float)labelPoints[i].GetX(), (float)labelPoints[i].GetY()));
            }
        }
        private void drawMessage(string[] message, System.Drawing.Graphics g, int imageWidth, int imageHeight)
        {
            // Make the regular and bold fonts.
            float fontHeight = ERROR_MESSAGE_FONT_SIZE;
            float lineSpacing = 12.0f;
            Font fontRegular = new Font(FontFamily.GenericSansSerif, fontHeight);
            Font fontBold = new Font(FontFamily.GenericSansSerif, fontHeight, FontStyle.Bold);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // Determine the location for the first line of text.
            float yText = imageHeight / 2.0f - (message.Length * fontHeight + (message.Length - 1) * lineSpacing) / 2.0f;

            // Determine the top and 
            for (int i = 0; i < message.Length; i++)
            {
                Font font = message[i].ToLower().Contains("not valid") ? fontBold : fontRegular;
                g.DrawString(message[i], font, Brushes.Black, imageWidth / 2, yText, stringFormat);
                yText += fontHeight + lineSpacing;
            }
        }
        private void drawErrorMessage(System.Drawing.Graphics g, int imageWidth, int imageHeight)
        {
            // Make the regular and bold fonts.
            float fontHeight = ERROR_MESSAGE_FONT_SIZE;
            float lineSpacing = 12.0f;
            Font fontRegular = new Font(FontFamily.GenericSansSerif, fontHeight);
            Font fontBold = new Font(FontFamily.GenericSansSerif, fontHeight, FontStyle.Bold);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // Get the result message from the data provider.
            string[] resultMessage = _dataProvider.GetResultMessage();

            // Add a title to the result message.
            string title = this.ToString();
            if (resultMessage == null)
            {
                resultMessage = new string[0];
            }
            string[] newResultMessage = new string[resultMessage.Length + 2];
            newResultMessage[0] = title;
            newResultMessage[1] = "";
            for (int i = 0; i < resultMessage.Length; i++)
            {
                newResultMessage[i + 2] = resultMessage[i];
            }
            resultMessage = newResultMessage;

            // Determine the location for the first line of text.
            float yText = imageHeight / 2.0f - (resultMessage.Length * fontHeight + (resultMessage.Length - 1) * lineSpacing) / 2.0f;

            // Determine the top and 

            for (int i = 0; i < resultMessage.Length; i++)
            {
                Font font = resultMessage[i].ToLower().Contains("not valid") ? fontBold : fontRegular;
                g.DrawString(resultMessage[i], font, Brushes.Black, imageWidth / 2, yText, stringFormat);
                yText += fontHeight + lineSpacing;
            }
        }
        private float[][] convertArray(float[,] array2D, int scaleFactor)
        {
            if (array2D == null)
            {
                return null;
            }

            // Determine the width and height.
            int width = array2D.GetLength(0) / scaleFactor;
            int height = array2D.GetLength(1) / scaleFactor;

            // Make the result array.
            float[][] converted = new float[width][];
            for (int i = 0; i < width; i++)
            {
                converted[i] = new float[height];
            }

            // Populate the result array.
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    converted[i][j] = array2D[i * scaleFactor, j * scaleFactor];
                }
            }

            // Return the result.
            return converted;
        }
        #endregion

        public bool IsReferenced()
        {
            if (_parentElement != null)
            {
                for (int i = 0; i < _parentElement.NumDataSeries; i++)
                {
                    DataSeries dataSeries = _parentElement.GetDataSeries(i);
                    string expression = "";
                    if (dataSeries._dataProvider is DataProviderCalculatedMap)
                    {
                        DataProviderCalculatedMap calcMap = (DataProviderCalculatedMap)dataSeries._dataProvider;
                        expression = calcMap.Expression;
                    }
                    if (dataSeries._dataProvider is DataProviderCalculatedSeries)
                    {
                        DataProviderCalculatedSeries calcSeries = (DataProviderCalculatedSeries) dataSeries._dataProvider;
                        expression = calcSeries.Expression;
                    }
                    string thisExpression = "[" + Name + "]";
                    if (expression.Contains(thisExpression))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public float GetMinValueOfGraphDataSeries()
        {
            if (_dataProvider != null)
            {
                // Get the data object from the data provider.
                int datasetStatus;
                object dataObject = _dataProvider.GetData(out datasetStatus);

                // If the data object is not null, return the appropriate value.
                if (dataObject != null)
                {
                    // If this is a time series, return the minimum series value.
                    if (dataObject is TimeSeries)
                    {
                        TimeSeries dataSeries = (TimeSeries)dataObject;
                        return dataSeries.GetMinValue();
                    }

                    // If this is a floating-point value, return the value.
                    else if (dataObject is float)
                    {
                        return (float)dataObject;
                    }
                }
            }

            // If we have reached this point, it is because there is no value to be returned. Return NaN.
            return float.NaN;
        }
        public float GetMaxValueOfGraphDataSeries()
        {
            if (_dataProvider != null)
            {
                // Get the data object from the data provider.
                int datasetStatus;
                object dataObject = _dataProvider.GetData(out datasetStatus);

                // If the data object is not null, return the appropriate value.
                if (dataObject != null)
                {
                    // If this is a time series, return the maximum series value.
                    if (dataObject is TimeSeries)
                    {
                        TimeSeries dataSeries = (TimeSeries)dataObject;
                        return dataSeries.GetMaxValue();
                    }

                    // If this is a floating-point value, return the value.
                    else if (dataObject is float)
                    {
                        return (float)dataObject;
                    }
                }
            }

            // If we have reached this point, it is because there is no value to be returned. Return NaN.
            return float.NaN;
        }
        public long GetMinTicksOfGraphDataSeries()
        {
            try
            {
                if (_dataProvider != null)
                {
                    // Get the data object from the data provider.
                    int datasetStatus;
                    object dataObject = _dataProvider.GetData(out datasetStatus);

                    // If the data object is not null, return the appropriate value.
                    if (dataObject != null)
                    {
                        // If this is a time series, return the minimum ticks.
                        if (dataObject is TimeSeries)
                        {
                            TimeSeries dataSeries = (TimeSeries)dataObject;
                            return dataSeries[0].Date.Ticks;
                        }
                    }
                }
            }
            catch { }

            // If we have reached this point, it is because there is no value to be returned. Return the maximum long value.
            return long.MaxValue;
        }
        public long GetMaxTicksOfGraphDataSeries()
        {
            try
            {
                if (_dataProvider != null)
                {
                    // Get the data object from the data provider.
                    int datasetStatus;
                    object dataObject = _dataProvider.GetData(out datasetStatus);

                    // If the data object is not null, return the appropriate value.
                    if (dataObject != null)
                    {
                        // If this is a time series, return the maximum ticks.
                        if (dataObject is TimeSeries)
                        {
                            TimeSeries dataSeries = (TimeSeries)dataObject;
                            return dataSeries[dataSeries.Length - 1].Date.Ticks;
                        }
                    }
                }
            }
            catch { }

            // If we have reached this point, it is because there is no value to be returned. Return the minimum long value.
            return long.MinValue;
        }
        public int GetNumTableRows()
        {
            int datasetStatus;
            object dataset = _dataProvider.GetData(out datasetStatus);
            if (datasetStatus == DataStatus.DATA_AVAILABLE_CACHE_PRESENT)
            {
                if (dataset is TimeSeries)
                {
                    TimeSeries ts = (TimeSeries)dataset;
                    return ts.Length;
                }
                else if (dataset is float[])
                {
                    float[] dataValues = (float[])dataset;
                    //return dataValues.Length + 1;
                    return dataValues.Length;
                }
                // Ned TODO: Are there additional types generated by dataProvider that need to be supported?
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        public void ValidateDataProviderKey(List<long> uniqueIdentifiers)
        {
            if (_dataProvider != null)
            {
                _dataProvider.ValidateUniqueIdentifier(uniqueIdentifiers);
            }
        }
        public GeoImage GetGeoImage(int width, int height, Extent imageExtent)
        {
            // Make an image.
            Image image = new Bitmap(width, height);

            // If the map type is a contour map, draw a contour map to the image.
            if (this.DataSeriesType == DataSeriesTypeEnum.ContourMapSeries)
            {
                // Draw the contour map to the image.
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
                drawContentMapContour(g, width, height, (float)imageExtent.West, (float)imageExtent.East, (float)imageExtent.South, (float)imageExtent.North);
                g.Dispose();
            }

            // Otherwise, draw a color fill map to the image.
            else
            {
                // Draw the color-fill map to the image.
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
                drawContentMapColorFill(g, width, height, (float)imageExtent.West, (float)imageExtent.East, (float)imageExtent.South, (float)imageExtent.North);
                g.Dispose();
            }

            // Return the image.
            return new GeoImage(image, imageExtent);
        }
        public void SaveContourShapefile(string filename)
        {
            Console.WriteLine("In SaveContourShapefile() method of the DataSeries class");

            // Trim the extension, if necessary.
            if (filename.ToLower().EndsWith(".shp"))
            {
                filename = filename.Substring(0, filename.Length - 4);
            }

            // Get the contour lines from the data provider.
            // Get the data from the data provider.
            int datasetStatus;
            object dataObject = _dataProvider.GetData(out datasetStatus);
            GeoMap data = dataObject == null ? null : (GeoMap)_dataProvider.GetData(out datasetStatus);

            // If the data array is null, throw an exception.
            if (data == null)
            {
                throw new DataException("Data set for contour shapefile is invalid.");
            }

            // If the contour point list is null, make it.
            if (this._contourPointList == null)
            {
                this.generateContourPointList();
            }         

            // Show the number of contours.
            Console.WriteLine("Num contours for shapefile export: " + _contourPointList.GetNumLines());

            // Convert the contours to polylines.
            Polyline2D[] contourPolylines = _contourPointList.GetPolylines();

            //// Write the polylines to a shapefile.
            //Polyline2D.WriteShapefile(filename, contourPolylines, SpatialReference.Utm17N);
        }
        public object GetDataSynchronous()
        {
            // Set the contour point list to null.
            this._contourPointList = null;

            // Retrieve the dataset synchronously from the data provider.
            this.DataProvider.ConvertFlowToFlux = this.ConvertFlowToFlux;
            object dataset = this.DataProvider.GetDataSynchronous();

            // If this is a contour map, regenerate the contour point list.
            if (this.DataSeriesType == DataSeriesTypeEnum.ContourMapSeries)
            {
                this.generateContourPointList();
            }

            // Return the dataset.
            return dataset;
        }
        public void InvalidateDataset()
        {
            _dataProvider.InvalidateDataset();
        }
        private Polyline2D[] convertPointListToPolylines(PointList pointList)
        {
            // Make the array for the polylines.
            int numLines = pointList.GetNumLines();
            Polyline2D[] polylines = new Polyline2D[numLines];

            // Populate the array.
            for (int i = 0; i < numLines; i++)
            {
                // Make a polyline from the line.
                Line line = pointList.GetLine(i);
                polylines[i] = new Polyline2D(new Point2D[] { new Point2D(line.x0, line.y0), new Point2D(line.x1, line.y1) });
                polylines[i].SetAttribute("ID", i + "");
            }

            // Return the array of polylines.
            return polylines;
        }

        #region IDeepCloneable Members
        public object DeepClone()
        {
            DataSeries dsCopy = new DataSeries(this);
            return dsCopy;
        }
        public void AssignParent(object parent)
        {
            if (parent is IReportElement)
            {
                this.ParentElement = (IReportElement)parent;
                this.DataProvider.AssignParent(parent);
            }
        }
        #endregion IDeepCloneable Members
    }
}
