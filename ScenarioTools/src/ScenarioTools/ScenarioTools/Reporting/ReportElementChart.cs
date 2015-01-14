using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;

using ScenarioTools.DataClasses;
using ScenarioTools.Data_Providers;
using ScenarioTools.Numerical;
using ScenarioTools.Util;
using ScenarioTools.Xml;

namespace ScenarioTools.Reporting
{
    public class ReportElementChart : IReportElement
    {
        #region Constants
        private const long INVALID_TICKS = -1;
        private const string XML_NODE_NAME = "ReportElementChart";
        private const string XML_NAME_KEY = "name";
        private const string XML_NAME_DEFAULT_VALUE = "";
        private const string XML_DATE_RANGE_START_KEY = "dateRangeStart";
        private const string XML_DATE_RANGE_END_KEY = "dateRangeEnd";
        private const string XML_VALUE_RANGE_MIN_KEY = "valueRangeMin";
        private const string XML_VALUE_RANGE_MAX_KEY = "valueRangeMax";

        public const int DEFAULT_CHART_WIDTH = 1200;
        public const int DEFAULT_CHART_HEIGHT = 900;
        private const float DEFAULT_FONT_HEIGHT = 20.0f;
        #endregion Constants

        #region Fields

        // This is the data series list. The elements of this list are written as child XML nodes.
        List<DataSeries> _dataSeriesList;

        private bool _dateRangeIsAutomatic;
        private DateTime _dateRangeEnd;
        private DateTime _dateRangeStart;
        private bool _valueRangeIsAutomatic;
        private float _valueRangeMin;
        private float _valueRangeMax;

        #endregion Fields

        #region Properties

        public Chart Chart { get; set; }
        public bool DateRangeIsAutomatic
        {
            get
            {
                return _dateRangeIsAutomatic;
            }
            set
            {
                _dateRangeIsAutomatic = value;
            }
        }
        public DateTime DateRangeEnd
        {
            get
            {
                if (DateRangeIsAutomatic)
                {
                    return this.GetMaxTimeOfDataSeries();
                }
                else
                {
                    return _dateRangeEnd;
                }
            }
            set
            {
                _dateRangeEnd = value;
            }
        }
        public DateTime DateRangeStart
        {
            get
            {
                if (DateRangeIsAutomatic)
                {
                    return this.GetMinTimeOfDataSeries();
                }
                else
                {
                    return _dateRangeStart;
                }
            }
            set
            {
                _dateRangeStart = value;
            }
        }
        public bool ValueRangeIsAutomatic
        {
            get
            {
                return _valueRangeIsAutomatic;
            }
            set
            {
                _valueRangeIsAutomatic = value;
            }
        }
        public float ValueRangeMin
        {
            get
            {
                if (ValueRangeIsAutomatic)
                {
                    return this.GetMinValueOfDataSeries();
                }
                else
                {
                    return _valueRangeMin;
                }
            }
            set
            {
                _valueRangeMin = value;
            }
        }
        public float ValueRangeMax
        {
            get
            {
                if (ValueRangeIsAutomatic)
                {
                    return this.GetMaxValueOfDataSeries();
                }
                else
                {
                    return _valueRangeMax;
                }
            }
            set
            {
                _valueRangeMax = value;
            }
        }

        #endregion Properties

        #region IReportElement Members

        public string Name { get; set; }

        public int NumDataSeries
        {
            get
            {
                return _dataSeriesList.Count;
            }
        }

        public void AddDataSeries(DataSeries dataSeries)
        {
            // Add the data series to the list of data series.
            _dataSeriesList.Add(dataSeries);

            // Set this as the parent of the data series.
            dataSeries.ParentElement = this;
        }

        public DataSeries GetDataSeries(int index)
        {
            return _dataSeriesList[index];
        }

        public object GetDataSeries(string name)
        {
            for (int i = 0; i < _dataSeriesList.Count; i++)
            {
                if (name.Equals(_dataSeriesList[i].Name))
                {
                    return (TimeSeries)_dataSeriesList[i].GetDataSynchronous();
                }
            }

            // If none was found, return null.
            return null;
        }

        public void RemoveDataSeries(DataSeries dataSeries)
        {
            //for (int i = 0; i < _dataSeriesList.Count; i++)
            //{
            //    if (_dataSeriesList[i].Equals(dataSeries))
            //    {
            _dataSeriesList.Remove(dataSeries);
            //        break;
            //    }
            //}
        }

        public void ClearDataSeries()
        {
            this._dataSeriesList.Clear();
        }

        public System.Xml.XmlNode GetXmlNode(System.Xml.XmlDocument document, 
                                             string targetFileName)
        {
            // Make the XML element that represents this object.
            XmlElement element = document.CreateElement(XML_NODE_NAME);
            element.SetAttribute("NodeType", WorkspaceUtil.XML_NODE_TYPE_REPORT_ELEMENT);

            // Prepare the XML element.
            prepareXmlElement(document, element,targetFileName);

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

        public void InitFromXML(System.Xml.XmlElement xmlElement, string sourceFileName)
        {
            // Get the name attribute from the XML element.
            Name = XmlUtil.SafeGetStringAttribute(xmlElement, XML_NAME_KEY, "");

            // If the start and end date range exist, read them and set automatic range to false.
            if (xmlElement.HasAttribute(XML_DATE_RANGE_START_KEY) && xmlElement.HasAttribute(XML_DATE_RANGE_END_KEY))
            {
                DateRangeIsAutomatic = false;
                DateRangeStart = XmlUtil.DateFromXmlString(XmlUtil.SafeGetStringAttribute(xmlElement, XML_DATE_RANGE_START_KEY, ""));
                DateRangeEnd = XmlUtil.DateFromXmlString(XmlUtil.SafeGetStringAttribute(xmlElement, XML_DATE_RANGE_END_KEY, ""));
            }

            // If the min and max values exist, read them and set automatic value range to false.
            if (xmlElement.HasAttribute(XML_VALUE_RANGE_MIN_KEY) && xmlElement.HasAttribute(XML_VALUE_RANGE_MAX_KEY))
            {
                float valueRangeMinFromXml, valueRangeMaxFromXml;
                if (float.TryParse(XmlUtil.SafeGetStringAttribute(xmlElement, XML_VALUE_RANGE_MIN_KEY, ""), out valueRangeMinFromXml) &&
                    float.TryParse(XmlUtil.SafeGetStringAttribute(xmlElement, XML_VALUE_RANGE_MAX_KEY, ""), out valueRangeMaxFromXml))
                {
                    ValueRangeIsAutomatic = false;
                    ValueRangeMin = valueRangeMinFromXml;
                    ValueRangeMax = valueRangeMaxFromXml;
                }
            }

            // Get all of the data series.
            foreach (XmlElement child in xmlElement.ChildNodes)
            {
                // Make the data series and initialize it with the XML node.
                DataSeries dataSeries = new DataSeries(DataSeriesTypeEnum.ChartSeries);
                dataSeries.InitFromXml(this, child, sourceFileName);

                // Add the data series to the list.
                _dataSeriesList.Add(dataSeries);
            }
        }

        public void ValidateDataProviderKeys(List<long> uniqueIdentifiers)
        {
            // Validate the data provider key of every data series.
            for (int i = 0; i < _dataSeriesList.Count; i++)
            {
                _dataSeriesList[i].ValidateDataProviderKey(uniqueIdentifiers);
            }
        }

        public void UpdateCaches()
        {
        }

        public bool IsUniqueName(string newName, DataSeries dataSeries)
        {
            // If any other data series in the element match the specified name, return false.
            for (int i = 0; i < _dataSeriesList.Count; i++)
            {
                if (_dataSeriesList[i] != dataSeries)
                {
                    if (_dataSeriesList[i].Name.Equals(Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
            }

            // None matched, so return true.
            return true;
        }

        #endregion

        #region IDeepCloneable Members
        public IReportElement DeepClone()
        {
            return (IReportElement)this.Clone();
        }
        object IDeepCloneable.DeepClone()
        {
            return this.DeepClone();
        }
        public void AssignParent(object parent)
        {
            // No parent required
        }
        #endregion IDeepCloneable Members

        #region IImageProvider Members

        public System.Drawing.Image GetChartImage(ref float minValue, ref float maxValue, ref long minTicks, ref long maxTicks)
        {
            // Make the image.
            // Assign default dimensions
            int width = DEFAULT_CHART_WIDTH;
            int height = DEFAULT_CHART_HEIGHT;

            // Define font
            //float fontHeightMedium = Convert.ToSingle(height)/70.0f;
            float fontHeightMedium = DEFAULT_FONT_HEIGHT;
            Font fontMedium;
            fontMedium = new Font(FontFamily.GenericSansSerif, fontHeightMedium, FontStyle.Regular);

            Image image;

            try
            {
                // Clear existing ChartArea
                if (this.Chart.ChartAreas.Count > 0)
                {
                    this.Chart.ChartAreas.Clear();
                }

                // Add a new chart area
                Chart.ChartAreas.Add(new ChartArea());

                // Assign chart-area properties
                Chart.ChartAreas[0].BorderDashStyle = ChartDashStyle.Solid;
                this.Chart.ChartAreas[0].AxisX.LabelStyle.Font = fontMedium;
                this.Chart.ChartAreas[0].AxisX2.LabelStyle.Font = fontMedium;
                this.Chart.ChartAreas[0].AxisY.LabelStyle.Font = fontMedium;
                this.Chart.ChartAreas[0].AxisY2.LabelStyle.Font = fontMedium; // Needed to make Y2 tick spacing like Y tick spacing

                this.Chart.Series.Clear();

                // Only continue if there are data series.
                if (_dataSeriesList.Count > 0)
                {
                    // If the min or max are NaN, compute them.
                    if (float.IsNaN(minValue) || float.IsNaN(maxValue))
                    {
                        // Determine the minimum and maximum values of all data series.
                        minValue = float.NaN;
                        maxValue = float.NaN;
                        for (int i = 0; i < _dataSeriesList.Count; i++)
                        {
                            if (_dataSeriesList[i].Visible)
                            {
                                minValue = MathUtil.NanAwareMin(minValue, _dataSeriesList[i].GetMinValueOfGraphDataSeries());
                                maxValue = MathUtil.NanAwareMax(maxValue, _dataSeriesList[i].GetMaxValueOfGraphDataSeries());
                            }
                        }
                    }

                    // If the min or max date are invalid, compute them.
                    if (minTicks == INVALID_TICKS || maxTicks == INVALID_TICKS)
                    {
                        // Determine the minimum and maximum values of all data series.
                        minTicks = this.GetMinTimeOfDataSeries().Ticks;
                        maxTicks = this.GetMaxTimeOfDataSeries().Ticks;
                    }

                    // If the range is zero, make it one.
                    if (maxValue - minValue == 0.0f)
                    {
                        maxValue += 1.0f;
                    }

                    // Compute the ticks (date) range. If it is zero, make it one.
                    double ticksRange = maxTicks - minTicks;
                    if (ticksRange == 0.0)
                    {
                        maxTicks += 1;
                    }

                    // Draw the contributions from all of the data series.
                    for (int i = _dataSeriesList.Count - 1; i >= 0; i--)
                    {
                        if (_dataSeriesList[i].Visible)
                        {
                            _dataSeriesList[i].DrawContent(Chart, width, height, minValue, maxValue,
                                minTicks, maxTicks, float.NaN, float.NaN, float.NaN, float.NaN);
                        }
                    }
                }
                if (this.ValueRangeIsAutomatic)
                {
                    this.Chart.ChartAreas[0].AxisY.IsStartedFromZero = true;
                    this.Chart.ChartAreas[0].AxisY.Minimum = float.NaN;
                    this.Chart.ChartAreas[0].AxisY.Maximum = float.NaN;
                    this.Chart.ChartAreas[0].AxisY2.IsStartedFromZero = true;
                    this.Chart.ChartAreas[0].AxisY2.Minimum = float.NaN;
                    this.Chart.ChartAreas[0].AxisY2.Maximum = float.NaN;
                }
                else
                {
                    this.Chart.ChartAreas[0].AxisY.IsStartedFromZero = false;
                    this.Chart.ChartAreas[0].AxisY.Minimum = this._valueRangeMin;
                    this.Chart.ChartAreas[0].AxisY.Maximum = this._valueRangeMax;
                    this.Chart.ChartAreas[0].AxisY2.IsStartedFromZero = false;
                    this.Chart.ChartAreas[0].AxisY2.Minimum = this._valueRangeMin;
                    this.Chart.ChartAreas[0].AxisY2.Maximum = this._valueRangeMax;
                }
                if (this.DateRangeIsAutomatic)
                {
                    this.Chart.ChartAreas[0].AxisX.Minimum = float.NaN;
                    this.Chart.ChartAreas[0].AxisX.Maximum = float.NaN;
                    this.Chart.ChartAreas[0].AxisX2.Minimum = float.NaN;
                    this.Chart.ChartAreas[0].AxisX2.Maximum = float.NaN;
                }
                else
                {
                    this.Chart.ChartAreas[0].AxisX.Minimum = this.DateRangeStart.ToOADate();
                    this.Chart.ChartAreas[0].AxisX.Maximum = this.DateRangeEnd.ToOADate();
                    this.Chart.ChartAreas[0].AxisX2.Minimum = this.DateRangeStart.ToOADate();
                    this.Chart.ChartAreas[0].AxisX2.Maximum = this.DateRangeEnd.ToOADate();
                }
                this.Chart.ChartAreas[0].AxisX.IsStartedFromZero = false;
                this.Chart.ChartAreas[0].AxisX2.IsStartedFromZero = false;
                this.Chart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
                this.Chart.ChartAreas[0].AxisX2.IntervalAutoMode = IntervalAutoMode.VariableCount;
                this.Chart.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
                this.Chart.ChartAreas[0].AxisY2.IntervalAutoMode = IntervalAutoMode.VariableCount;
                using (MemoryStream memStream = new MemoryStream())
                {
                    ImageFormat format = ImageFormat.Bmp;
                    //ChartImageFormat format = ChartImageFormat.Bmp;
                    Chart.SaveImage(memStream, format);
                    image = Image.FromStream(memStream);
                    memStream.Dispose();
                }
            }
            catch (Exception ex)
            {
                minValue = 0.0F;
                maxValue = 1.0F;
                minTicks = 0;
                maxTicks = 2;
                image = GetDefaultImage(ref minValue, ref maxValue, ref minTicks, ref maxTicks);
            }
            return image;
        }

        public Image GetImage()
        {
            // This returns the image with the specified min and max for domain and range. 
            // If automatic ranging is specified for either of these,
            // the values will span the full dataset extent.
            return GetImage(ValueRangeMin, ValueRangeMax, DateRangeStart.Ticks, DateRangeEnd.Ticks,
                            double.NaN, double.NaN, double.NaN, double.NaN, true);
        }

        public Image GetImage(float minValue, float maxValue, long minTicks, long maxTicks,
                              double xcMin, double xcMax, double ycMin, double ycMax, bool isStandalone)
        {
            int dataPointCount = 0;
            for (int i = 0; i < _dataSeriesList.Count; i++)
            {
                dataPointCount = dataPointCount + _dataSeriesList[i].GetNumTableRows();
            }
            if (dataPointCount == 0)
            {
                return GetDefaultImage(ref minValue, ref maxValue, ref minTicks, ref maxTicks);
            }
            else
            {
                return GetChartImage(ref minValue, ref maxValue, ref minTicks, ref maxTicks);
            }
        }

        private Image GetDefaultImage(ref float minValue, ref float maxValue, ref long minTicks, ref long maxTicks)
        {
            // Make the image.
            int width = DEFAULT_CHART_WIDTH;
            int height = DEFAULT_CHART_HEIGHT;
            Bitmap image = new Bitmap(width, height);

            // Fill the background.
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
            g.FillRectangle(Brushes.White, 0, 0, width, height);

            // Draw the axes.
            int leftBuffer = 20;
            int rightBuffer = 20;
            int bottomBuffer = 20;
            int topBuffer = 20;
            //g.DrawLine(Pens.Black, leftBuffer, height - bottomBuffer, width - rightBuffer, height - bottomBuffer); // x axis
            //g.DrawLine(Pens.Black, leftBuffer, height - bottomBuffer, leftBuffer, topBuffer);

            // Only continue if there are data series.
            if (_dataSeriesList.Count > 0)
            {
                // If the min or max are NaN, compute them.
                if (float.IsNaN(minValue) || float.IsNaN(maxValue))
                {
                    // Determine the minimum and maximum values of all data series.
                    minValue = float.NaN;
                    maxValue = float.NaN;
                    for (int i = 0; i < _dataSeriesList.Count; i++)
                    {
                        minValue = MathUtil.NanAwareMin(minValue, _dataSeriesList[i].GetMinValueOfGraphDataSeries());
                        maxValue = MathUtil.NanAwareMax(maxValue, _dataSeriesList[i].GetMaxValueOfGraphDataSeries());
                    }
                }

                // If the min or max date are invalid, compute them.
                if (minTicks == INVALID_TICKS || maxTicks == INVALID_TICKS)
                {
                    // Determine the minimum and maximum values of all data series.
                    minTicks = this.GetMinTimeOfDataSeries().Ticks;
                    maxTicks = this.GetMaxTimeOfDataSeries().Ticks;
                }

                // If the range is zero, make it one.
                if (maxValue - minValue == 0.0f)
                {
                    maxValue += 1.0f;
                }

                // Compute the ticks (date) range. If it is zero, make it one.
                double ticksRange = maxTicks - minTicks;
                if (ticksRange == 0.0)
                {
                    maxTicks += 1;
                }

                // Draw the contributions from all of the data series.
                for (int i = _dataSeriesList.Count - 1; i >= 0; i--)
                {
                    _dataSeriesList[i].DrawContent(Chart, width, height, minValue, maxValue, minTicks, maxTicks, float.NaN, float.NaN, float.NaN, float.NaN);
                }
            }

            // Dispose of the graphics object to reclaim resources.
            g.Dispose();

            return image;
        }

        public void ClearImage()
        {
        }

        #endregion

        #region ICloneable Member

        public object Clone()
        {
            ReportElementChart repElemClone = new ReportElementChart();
            repElemClone.Chart.Size = this.Chart.Size;

            // Define a Chart font
            repElemClone.defineChartFontAndSize();

            repElemClone._dataSeriesList = new List<DataSeries>();
            for (int i = 0; i < this._dataSeriesList.Count; i++)
            {
                DataSeries ds = new DataSeries(_dataSeriesList[i]);                
                ds.AssignParent(repElemClone);
                repElemClone._dataSeriesList.Add(ds);
            }
            return (object)repElemClone;
        }

        #endregion ICloneable Member

        #region Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public ReportElementChart()
        {
            Name = "";
            // Instantiate a Microsoft Chart Controls chart.
            Chart = new Chart();

            // Define a Chart font and size
            defineChartFontAndSize();

            // Add a chart area
            Chart.ChartAreas.Add(new ChartArea());
            Chart.ChartAreas[0].BorderDashStyle = ChartDashStyle.Solid;

            // Make the list for the data series.
            _dataSeriesList = new List<DataSeries>();

            // Set the date and value ranges to automatic.
            this.DateRangeIsAutomatic = true;
            this.ValueRangeIsAutomatic = true;

            // Assign primary axis properties
            Chart.ChartAreas[0].AxisX.MajorTickMark.TickMarkStyle = TickMarkStyle.InsideArea;
            Chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            Chart.ChartAreas[0].AxisY.MajorTickMark.TickMarkStyle = TickMarkStyle.InsideArea;
            Chart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            // Assign secondary axis properties
            Chart.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;
            Chart.ChartAreas[0].AxisY2.MajorTickMark.TickMarkStyle = TickMarkStyle.InsideArea;
            Chart.ChartAreas[0].AxisY2.MajorGrid.Enabled = false;
            Chart.ChartAreas[0].AxisY2.LabelStyle.ForeColor = Color.Transparent;
            //_chart.ChartAreas[0].AxisY2.LabelStyle.Enabled = false;

            Chart.ChartAreas[0].AxisX2.Enabled = AxisEnabled.True;
            Chart.ChartAreas[0].AxisX2.MajorTickMark.TickMarkStyle = TickMarkStyle.InsideArea;
            Chart.ChartAreas[0].AxisX2.MajorGrid.Enabled = false;
            Chart.ChartAreas[0].AxisX2.LabelStyle.Enabled = false;
        }

        public DateTime GetMinTimeOfDataSeries()
        {
            // Determine the minimum tick value of all data series.
            long minTicks = long.MaxValue;
            for (int i = 0; i < _dataSeriesList.Count; i++)
            {
                minTicks = Math.Min(minTicks, _dataSeriesList[i].GetMinTicksOfGraphDataSeries());
            }

            // Return the result as a date-time object.
            if (minTicks == long.MaxValue)
            {
                return new DateTime();
            }
            else
            {
                return new DateTime(minTicks);
            }
        }
        public DateTime GetMaxTimeOfDataSeries()
        {
            // Determine the minimum tick value of all data series.
            long maxTicks = long.MinValue;
            for (int i = 0; i < _dataSeriesList.Count; i++)
            {
                maxTicks = Math.Max(maxTicks, _dataSeriesList[i].GetMaxTicksOfGraphDataSeries());
            }

            // Return the result as a date-time object.
            if (maxTicks == long.MinValue)
            {
                return new DateTime();
            }
            else
            {
                return new DateTime(maxTicks);
            }
        }
        public float GetMinValueOfDataSeries()
        {
            // Determine the minimum value of all data series.
            float minValue = float.NaN;
            for (int i = 0; i < _dataSeriesList.Count; i++)
            {
                minValue = MathUtil.NanAwareMin(minValue, _dataSeriesList[i].GetMinValueOfGraphDataSeries());
            }

            // Return the result.
            return minValue;
        }
        public float GetMaxValueOfDataSeries()
        {
            // Determine the maximum value of all data series.
            float maxValue = float.NaN;
            for (int i = 0; i < _dataSeriesList.Count; i++)
            {
                maxValue = MathUtil.NanAwareMax(maxValue, _dataSeriesList[i].GetMaxValueOfGraphDataSeries());
            }

            // Return the result.
            return maxValue;
        }
        public override string ToString()
        {
            string unnamed = "Unnamed Chart";
            return "Chart: " + (Name == null ? unnamed : (Name.Equals("") ? unnamed : Name));
        }

        private void defineChartFontAndSize()
        {
            // Define Chart font
            float fontHeightMedium = DEFAULT_FONT_HEIGHT;
            System.Drawing.Font fontMedium = new Font(FontFamily.GenericSansSerif, fontHeightMedium, FontStyle.Regular);
            this.Chart.Font = fontMedium;

            // Define Chart size
            this.Chart.Size = new Size(DEFAULT_CHART_WIDTH, DEFAULT_CHART_HEIGHT);
        }
        private void prepareXmlElement(XmlDocument document, XmlElement element, 
                                       string targetFileName)
        {
            // Set the name attribute.
            XmlUtil.FrugalSetAttribute(element, XML_NAME_KEY, Name, XML_NAME_DEFAULT_VALUE);

            // If the date range is not automatic, write the start and end dates. Lack of dates in the XML file signals an automatic date range.
            if (!DateRangeIsAutomatic)
            {
                XmlUtil.FrugalSetAttribute(element, XML_DATE_RANGE_START_KEY, XmlUtil.DateToXmlString(DateRangeStart), "");
                XmlUtil.FrugalSetAttribute(element, XML_DATE_RANGE_END_KEY, XmlUtil.DateToXmlString(DateRangeEnd), "");
            }

            // If the value range is not automatic, write the min and max values. Lack of these values in the XML file signals an automatic value range.
            if (!ValueRangeIsAutomatic)
            {
                XmlUtil.FrugalSetAttribute(element, XML_VALUE_RANGE_MIN_KEY, ValueRangeMin + "", "");
                XmlUtil.FrugalSetAttribute(element, XML_VALUE_RANGE_MAX_KEY, ValueRangeMax + "", "");
            }

            // Add all of the data series as children.
            for (int i = 0; i < _dataSeriesList.Count; i++)
            {
                element.AppendChild(_dataSeriesList[i].GetXmlNode(document, targetFileName));
            }
        }

        #endregion Methods
    }
}
