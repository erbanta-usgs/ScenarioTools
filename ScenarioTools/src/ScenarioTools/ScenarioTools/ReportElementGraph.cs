using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;

using ScenarioTools.Data_Providers;
using ScenarioTools.DataClasses;
using ScenarioTools.Numerical;
using ScenarioTools.Xml;

namespace ScenarioTools
{
    //public class ReportElementGraph : IReportElement
    //{
    //    private const long INVALID_TICKS = -1;

    //    private const string XML_NODE_NAME = "ReportElementGraph";

    //    // These are the state variables. Each variable is followed 
    //    // by its property and, if it is written to the XML file, 
    //    // its XML key and, possibly, a default value.
    //    private string name;
    //    public string Name
    //    {
    //        get
    //        {
    //            return name;
    //        }
    //        set
    //        {
    //            name = value;
    //        }
    //    }
    //    private const string XML_NAME_KEY = "name";
    //    private const string XML_NAME_DEFAULT_VALUE = "";

    //    private bool dateRangeIsAutomatic;
    //    public bool DateRangeIsAutomatic
    //    {
    //        get
    //        {
    //            return dateRangeIsAutomatic;
    //        }
    //        set
    //        {
    //            dateRangeIsAutomatic = value;
    //        }
    //    }

    //    private DateTime dateRangeStart;
    //    public DateTime DateRangeStart
    //    {
    //        get
    //        {
    //            if (DateRangeIsAutomatic)
    //            {
    //                return this.GetMinTimeOfDataSeries();
    //            }
    //            else
    //            {
    //                return dateRangeStart;
    //            }
    //        }
    //        set
    //        {
    //            dateRangeStart = value;
    //        }
    //    }
    //    private const string XML_DATE_RANGE_START_KEY = "dateRangeStart";

    //    private DateTime dateRangeEnd;
    //    public DateTime DateRangeEnd
    //    {
    //        get
    //        {
    //            if (DateRangeIsAutomatic)
    //            {
    //                return this.GetMaxTimeOfDataSeries();
    //            }
    //            else
    //            {
    //                return dateRangeEnd;
    //            }
    //        }
    //        set
    //        {
    //            dateRangeEnd = value;
    //        }
    //    }
    //    private const string XML_DATE_RANGE_END_KEY = "dateRangeEnd";

    //    private bool valueRangeIsAutomatic;
    //    public bool ValueRangeIsAutomatic
    //    {
    //        get
    //        {
    //            return valueRangeIsAutomatic;
    //        }
    //        set
    //        {
    //            valueRangeIsAutomatic = value;
    //        }
    //    }

    //    private float valueRangeMin;
    //    public float ValueRangeMin
    //    {
    //        get
    //        {
    //            if (ValueRangeIsAutomatic)
    //            {
    //                return this.GetMinValueOfDataSeries();
    //            }
    //            else
    //            {
    //                return valueRangeMin;
    //            }
    //        }
    //        set
    //        {
    //            valueRangeMin = value;
    //        }
    //    }

    //    public bool IsUniqueName(string name, DataSeries dataSeries)
    //    {
    //        // If any other data series in the element match the specified name, return false.
    //        for (int i = 0; i < dataSeriesList.Count; i++)
    //        {
    //            if (dataSeriesList[i] != dataSeries)
    //            {
    //                if (dataSeriesList[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
    //                {
    //                    return false;
    //                }
    //            }
    //        }

    //        // None matched, so return true.
    //        return true;
    //    }

    //    private const string XML_VALUE_RANGE_MIN_KEY = "valueRangeMin";

    //    private float valueRangeMax;
    //    public float ValueRangeMax {
    //        get {
    //            if (ValueRangeIsAutomatic)
    //            {
    //                return this.GetMaxValueOfDataSeries();
    //            }
    //            else 
    //            {
    //                return valueRangeMax;
    //            }
    //        }
    //        set
    //        {
    //            valueRangeMax = value;
    //        }
    //    }

    //    private const string XML_VALUE_RANGE_MAX_KEY = "valueRangeMax";

    //    // This is the data series list. 
    //    // The elements of this list are written as child XML nodes.
    //    List<DataSeries> dataSeriesList;

    //    public ReportElementGraph()
    //    {
    //        // Make the list for the data series.
    //        dataSeriesList = new List<DataSeries>();

    //        // Set the date and value ranges to automatic.
    //        this.DateRangeIsAutomatic = true;
    //        this.ValueRangeIsAutomatic = true;
    //    }

    //    public XmlNode GetXmlNode(XmlDocument document, string targetFileName)
    //    {
    //        // Make the XML element that represents this object.
    //        XmlElement element = document.CreateElement(XML_NODE_NAME);

    //        // Prepare the XML element.
    //        prepareXmlElement(document, element,targetFileName);

    //        // Return the result.
    //        return element;
    //    }
    //    public void SaveXMLFile(string file)
    //    {
    //        // Make the XML file.
    //        XmlDocument document = XmlUtil.CreateXmlFile(file, XML_NODE_NAME);

    //        // Prepare the root element.
    //        prepareXmlElement(document, document.DocumentElement, file);

    //        // Write the file.
    //        document.Save(file);
    //    }
    //    private void prepareXmlElement(XmlDocument document, XmlElement element, string targetFileName)
    //    {
    //        // Set the name attribute.
    //        XmlUtil.FrugalSetAttribute(element, XML_NAME_KEY, Name, XML_NAME_DEFAULT_VALUE);

    //        // If the date range is not automatic, write the start and end dates. Lack of dates in the XML file signals an automatic date range.
    //        if (!DateRangeIsAutomatic)
    //        {
    //            XmlUtil.FrugalSetAttribute(element, XML_DATE_RANGE_START_KEY, XmlUtil.DateToXmlString(DateRangeStart), "");
    //            XmlUtil.FrugalSetAttribute(element, XML_DATE_RANGE_END_KEY, XmlUtil.DateToXmlString(DateRangeEnd), "");
    //        }

    //        // If the value range is not automatic, write the min and max values. Lack of these values in the XML file signals an automatic value range.
    //        if (!ValueRangeIsAutomatic)
    //        {
    //            XmlUtil.FrugalSetAttribute(element, XML_VALUE_RANGE_MIN_KEY, ValueRangeMin + "", "");
    //            XmlUtil.FrugalSetAttribute(element, XML_VALUE_RANGE_MAX_KEY, ValueRangeMax + "", "");
    //        }

    //        // Add all of the data series as children.
    //        for (int i = 0; i < dataSeriesList.Count; i++)
    //        {
    //            element.AppendChild(dataSeriesList[i].GetXmlNode(document, targetFileName));
    //        }
    //    }
    //    public void InitFromXML(XmlElement xmlElement, string sourceFileName)
    //    {
    //        // Get the name attribute from the XML element.
    //        name = XmlUtil.SafeGetStringAttribute(xmlElement, XML_NAME_KEY, "");

    //        // If the start and end date range exist, read them and set automatic range to false.
    //        if (xmlElement.HasAttribute(XML_DATE_RANGE_START_KEY) && xmlElement.HasAttribute(XML_DATE_RANGE_END_KEY))
    //        {
    //            DateRangeIsAutomatic = false;
    //            DateRangeStart = XmlUtil.DateFromXmlString(XmlUtil.SafeGetStringAttribute(xmlElement, XML_DATE_RANGE_START_KEY, ""));
    //            DateRangeEnd = XmlUtil.DateFromXmlString(XmlUtil.SafeGetStringAttribute(xmlElement, XML_DATE_RANGE_END_KEY, ""));
    //        }

    //        // If the min and max values exist, read them and set automatic value range to false.
    //        if (xmlElement.HasAttribute(XML_VALUE_RANGE_MIN_KEY) && xmlElement.HasAttribute(XML_VALUE_RANGE_MAX_KEY))
    //        {
    //            float valueRangeMinFromXml, valueRangeMaxFromXml;
    //            if (float.TryParse(XmlUtil.SafeGetStringAttribute(xmlElement, XML_VALUE_RANGE_MIN_KEY, ""), out valueRangeMinFromXml) &&
    //                float.TryParse(XmlUtil.SafeGetStringAttribute(xmlElement, XML_VALUE_RANGE_MAX_KEY, ""), out valueRangeMaxFromXml))
    //            {
    //                ValueRangeIsAutomatic = false;
    //                ValueRangeMin = valueRangeMinFromXml;
    //                ValueRangeMax = valueRangeMaxFromXml;
    //            }
    //        }

    //        // Get all of the data series.
    //        foreach (XmlElement child in xmlElement.ChildNodes)
    //        {
    //            // Make the data series and initialize it with the XML node.
    //            DataSeries dataSeries = new DataSeries(DataSeriesTypeEnum.Graph);
    //            dataSeries.InitFromXml(this, child, sourceFileName);

    //            // Add the data series to the list.
    //            dataSeriesList.Add(dataSeries);
    //        }
    //    }


    //    #region IReportElement Members


    //    public int NumDataSeries
    //    {
    //        get
    //        {
    //            return dataSeriesList.Count;
    //        }
    //    }

    //    public Form GetForm()
    //    {
    //        return new ReportElementGraphMenu(this);
    //    }

    //    public Image GetImage()
    //    {
    //        // This returns the image with the specified min and max for domain and range. 
    //        // If automatic ranging is specified for either of these,
    //        // the values will span the full dataset extent.
    //        return GetImage(ValueRangeMin, ValueRangeMax, DateRangeStart.Ticks, DateRangeEnd.Ticks, 
    //                        float.NaN, float.NaN, float.NaN, float.NaN, true);
    //    }
    //    public Image GetImage(float minValue, float maxValue, long minTicks, long maxTicks,
    //                          double xcMin, double xcMax, double ycMin, double ycMax, bool isStandalone)
    //    {
    //        // Make the image.
    //        int width = 800;
    //        int height = 400;
    //        Bitmap image = new Bitmap(width, height);

    //        // Fill the background.
    //        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
    //        g.FillRectangle(Brushes.White, 0, 0, width, height);

    //        // Draw the axes.
    //        int leftBuffer = 20;
    //        int rightBuffer = 20;
    //        int bottomBuffer = 20;
    //        int topBuffer = 20;
    //        g.DrawLine(Pens.Black, leftBuffer, height - bottomBuffer, width - rightBuffer, height - bottomBuffer); // x axis
    //        g.DrawLine(Pens.Black, leftBuffer, height - bottomBuffer, leftBuffer, topBuffer);
            
    //        // Only continue if there are data series.
    //        if (dataSeriesList.Count > 0)
    //        {
    //            // If the min or max are NaN, compute them.
    //            if (float.IsNaN(minValue) || float.IsNaN(maxValue))
    //            {
    //                // Determine the minimum and maximum values of all data series.
    //                minValue = float.NaN;
    //                maxValue = float.NaN;
    //                for (int i = 0; i < dataSeriesList.Count; i++)
    //                {
    //                    minValue = MathUtil.NanAwareMin(minValue, dataSeriesList[i].GetMinValueOfGraphDataSeries());
    //                    maxValue = MathUtil.NanAwareMax(maxValue, dataSeriesList[i].GetMaxValueOfGraphDataSeries());
    //                }
    //            }

    //            // If the min or max date are invalid, compute them.
    //            if (minTicks == INVALID_TICKS || maxTicks == INVALID_TICKS)
    //            {
    //                // Determine the minimum and maximum values of all data series.
    //                minTicks = this.GetMinTimeOfDataSeries().Ticks;
    //                maxTicks = this.GetMaxTimeOfDataSeries().Ticks;
    //            }
                
    //            // If the range is zero, make it one.
    //            if (maxValue - minValue == 0.0f)
    //            {
    //                maxValue += 1.0f;
    //            }

    //            // Compute the ticks (date) range. If it is zero, make it one.
    //            double ticksRange = maxTicks - minTicks;
    //            if (ticksRange == 0.0)
    //            {
    //                maxTicks += 1;
    //            }

    //            // Draw the contributions from all of the data series.
    //            for (int i = dataSeriesList.Count - 1; i >= 0; i--)
    //            {
    //                dataSeriesList[i].DrawContent(g, width, height, minValue, maxValue, minTicks, maxTicks, float.NaN, float.NaN, float.NaN, float.NaN,
    //                    true);
    //            }
    //        }

    //        // Dispose of the graphics object to reclaim resources.
    //        g.Dispose();

    //        return image;
    //    }
    //    public DateTime GetMinTimeOfDataSeries()
    //    {
    //        // Determine the minimum tick value of all data series.
    //        long minTicks = long.MaxValue;
    //        for (int i = 0; i < dataSeriesList.Count; i++)
    //        {
    //            minTicks = Math.Min(minTicks, dataSeriesList[i].GetMinTicksOfGraphDataSeries());
    //        }

    //        // Return the result as a date-time object.
    //        if (minTicks == long.MaxValue)
    //        {
    //            return new DateTime();
    //        }
    //        else
    //        {
    //            return new DateTime(minTicks);
    //        }
    //    }
    //    public DateTime GetMaxTimeOfDataSeries()
    //    {
    //        // Determine the minimum tick value of all data series.
    //        long maxTicks = long.MinValue;
    //        for (int i = 0; i < dataSeriesList.Count; i++)
    //        {
    //            maxTicks = Math.Max(maxTicks, dataSeriesList[i].GetMaxTicksOfGraphDataSeries());
    //        }

    //        // Return the result as a date-time object.
    //        if (maxTicks == long.MinValue)
    //        {
    //            return new DateTime();
    //        }
    //        else
    //        {
    //            return new DateTime(maxTicks);
    //        }
    //    }
    //    public float GetMinValueOfDataSeries()
    //    {
    //        // Determine the minimum value of all data series.
    //        float minValue = float.NaN;
    //        for (int i = 0; i < dataSeriesList.Count; i++)
    //        {
    //            minValue = MathUtil.NanAwareMin(minValue, dataSeriesList[i].GetMinValueOfGraphDataSeries());
    //        }

    //        // Return the result.
    //        return minValue;
    //    }
    //    public float GetMaxValueOfDataSeries()
    //    {
    //        // Determine the maximum value of all data series.
    //        float maxValue = float.NaN;
    //        for (int i = 0; i < dataSeriesList.Count; i++)
    //        {
    //            maxValue = MathUtil.NanAwareMax(maxValue, dataSeriesList[i].GetMaxValueOfGraphDataSeries());
    //        }

    //        // Return the result.
    //        return maxValue;
    //    }

    //    public DataSeries GetDataSeries(int index)
    //    {
    //        return dataSeriesList[index];
    //    }

    //    public void ValidateDataProviderKeys(List<long> uniqueIdentifiers)
    //    {
    //        // Validate the data provider key of every data series.
    //        for (int i = 0; i < dataSeriesList.Count; i++)
    //        {
    //            dataSeriesList[i].ValidateDataProviderKey(uniqueIdentifiers);
    //        }
    //    }
    //    public void UpdateCaches()
    //    {
    //    }

    //    #endregion

    //    public override string ToString()
    //    {
    //        string unnamed = "Unnamed Graph";
    //        return "Graph: " + (name == null ? unnamed : (name.Equals("") ? unnamed : name));
    //    }

    //    public void AddDataSeries(DataSeries dataSeries)
    //    {
    //        // Add the data series to the list of data series.
    //        dataSeriesList.Add(dataSeries);

    //        // Set this as the parent of the data series.
    //        dataSeries.ParentElement = this;
    //    }

    //    public void RemoveDataSeries(DataSeries dataSeries)
    //    {
    //        dataSeriesList.Remove(dataSeries);
    //    }

    //    public void ClearDataSeries()
    //    {
    //        this.dataSeriesList.Clear();
    //    }

    //    public object GetDataSeries(string name)
    //    {
    //        for (int i = 0; i < dataSeriesList.Count; i++)
    //        {
    //            if (name.Equals(dataSeriesList[i].Name))
    //            {
    //                return (TimeSeries)dataSeriesList[i].GetDataSynchronous();
    //            }
    //        }

    //        // If none was found, return null.
    //        return null;
    //    }
    //}
}
