using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

using ScenarioTools.DataClasses;
using ScenarioTools.Data_Providers;
using ScenarioTools.Dialogs;
using ScenarioTools.Numerical;
using ScenarioTools.Util;
using ScenarioTools.Xml;

namespace ScenarioTools.Reporting
{
    public class ReportElementTable : IReportElement
    {
        #region Constants
        private const long INVALID_TICKS = -1;
        private const string XML_NODE_NAME = "ReportElementTable";
        private const string XML_NAME_KEY = "name";
        private const string XML_NAME_DEFAULT_VALUE = "";
        private const string XML_DATE_RANGE_START_KEY = "dateRangeStart";
        private const string XML_DATE_RANGE_END_KEY = "dateRangeEnd";
        private const string XML_INCLUDETIME_KEY = "includeTimeInTable";
        private const bool XML_INCLUDETIME_DEFAULT_VALUE = false;
        #endregion Constants

        #region Fields
        // These are the state variables. Each variable is associated with a property and, 
        // if it is written to the XML file, an XML key and, possibly, a default value.
        private string _name;
        private bool _dateRangeIsAutomatic;
        private DateTime _dateRangeStart;
        private DateTime _dateRangeEnd;
        private bool _includeTimeInTable;
        // This is the data series list. The elements of this list are written as child XML nodes.
        private List<DataSeries> _dataSeriesList;
        #endregion Fields

        #region Properties
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
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
        public bool IncludeTimeInTable
        {
            get
            {
                return _includeTimeInTable;
            }
            set
            {
                _includeTimeInTable = value;
            }
        }
        #endregion Properties

        #region Constructor
        public ReportElementTable()
        {
            _name = "";
            // Make the list for the data series.
            _dataSeriesList = new List<DataSeries>();

            // Set the date range to automatic.
            this.DateRangeIsAutomatic = true;

            // Assign other defaults
            _includeTimeInTable = XML_INCLUDETIME_DEFAULT_VALUE;
        }
        #endregion Constructor

        #region Private methods
        private void prepareXmlElement(XmlDocument document, XmlElement element, string targetFileName)
        {
            // Set the name attribute.
            XmlUtil.FrugalSetAttribute(element, XML_NAME_KEY, Name, XML_NAME_DEFAULT_VALUE);

            // If the date range is not automatic, write the start and end dates. Lack of dates in the XML file signals an automatic date range.
            if (!DateRangeIsAutomatic)
            {
                XmlUtil.FrugalSetAttribute(element, XML_DATE_RANGE_START_KEY, XmlUtil.DateToXmlString(DateRangeStart), "");
                XmlUtil.FrugalSetAttribute(element, XML_DATE_RANGE_END_KEY, XmlUtil.DateToXmlString(DateRangeEnd), "");
            }

            // Write an entry for IncludeTimeInTable
            XmlUtil.FrugalSetAttribute(element, XML_INCLUDETIME_KEY, IncludeTimeInTable, XML_INCLUDETIME_DEFAULT_VALUE);

            // Add all of the data series as children.
            for (int i = 0; i < _dataSeriesList.Count; i++)
            {
                element.AppendChild(_dataSeriesList[i].GetXmlNode(document, targetFileName));
            }
        }
        #endregion Private methods

        #region IDeepCloneable Members
        public IReportElement DeepClone()
        {
            return (IReportElement)this.Copy();
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

        #region Public methods
        private ReportElementTable Copy()
        {
            ReportElementTable tableCopy = new ReportElementTable();
            tableCopy._name = this._name;
            tableCopy._dateRangeIsAutomatic = this._dateRangeIsAutomatic;
            tableCopy._dateRangeStart = new DateTime(this._dateRangeStart.Ticks);
            tableCopy._dateRangeEnd = new DateTime(this._dateRangeEnd.Ticks);
            tableCopy._includeTimeInTable = this._includeTimeInTable;
            for (int i = 0; i < _dataSeriesList.Count; i++)
            {
                DataSeries dsNew = new DataSeries(this._dataSeriesList[i]);
                dsNew.AssignParent(tableCopy);
                tableCopy._dataSeriesList.Add(dsNew);
            }
            return tableCopy;
        }
        public bool IsUniqueName(string name, DataSeries dataSeries)
        {
            // If any other data series in the element match the specified name, return false.
            for (int i = 0; i < _dataSeriesList.Count; i++)
            {
                if (_dataSeriesList[i] != dataSeries)
                {
                    if (_dataSeriesList[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
            }

            // None matched, so return false.
            return true;
        }
        public XmlNode GetXmlNode(XmlDocument document, string targetFileName)
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
        public void InitFromXML(XmlElement xmlElement, string sourceFileName)
        {
            // Get the name attribute from the XML element.
            _name = XmlUtil.SafeGetStringAttribute(xmlElement, XML_NAME_KEY, "");

            // Get the IncludeTimeInTable attribute from the XML element.
            IncludeTimeInTable = XmlUtil.SafeGetBoolAttribute(xmlElement, XML_INCLUDETIME_KEY, XML_INCLUDETIME_DEFAULT_VALUE);

            // If the start and end date range exist, read them and set automatic range to false.
            if (xmlElement.HasAttribute(XML_DATE_RANGE_START_KEY) && xmlElement.HasAttribute(XML_DATE_RANGE_END_KEY))
            {
                DateRangeIsAutomatic = false;
                DateRangeStart = XmlUtil.DateFromXmlString(XmlUtil.SafeGetStringAttribute(xmlElement, XML_DATE_RANGE_START_KEY, ""));
                DateRangeEnd = XmlUtil.DateFromXmlString(XmlUtil.SafeGetStringAttribute(xmlElement, XML_DATE_RANGE_END_KEY, ""));
            }

            // Get all of the data series.
            foreach (XmlElement child in xmlElement.ChildNodes)
            {
                // Make the data series and initialize it with the XML node.
                DataSeries dataSeries = new DataSeries(DataSeriesTypeEnum.TableSeries);
                dataSeries.InitFromXml(this, child, sourceFileName);

                // Add the data series to the list.
                _dataSeriesList.Add(dataSeries);
            }
        }
        public override string ToString()
        {
            string unnamed = "Unnamed Table";
            return "Table: " + (_name == null ? unnamed : (_name.Equals("") ? unnamed : _name));
            //return name == null ? unnamed : (name.Equals("") ? unnamed : name);
        }
        public void AddDataSeries(DataSeries dataSeries)
        {
            // Add the data series to the list of data series.
            _dataSeriesList.Add(dataSeries);

            // Set this as the parent of the data series.
            dataSeries.ParentElement = this;
        }
        public void RemoveDataSeries(DataSeries dataSeries)
        {
            _dataSeriesList.Remove(dataSeries);
        }
        public void ClearDataSeries()
        {
            this._dataSeriesList.Clear();
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
        public void ExportCsvFile(string fileName)
        {
            // If there are no data series, create an empty file.
            if (this.NumDataSeries == 0)
            {
                string[] fileContents = new string[0];
                FileUtil.WriteFile(fileName, fileContents);
            }
            else
            {
                // Get the table data.
                bool isTruncated;
                int rowsInFullTable;
                string[,] tableData = GetTableData(false, out isTruncated, -1, out rowsInFullTable);

                // Headings are contained in row 0 (elements [j,0])
                // Dates (or Dates & Times) are contained in column 0 (elements [0,i])
                int numTableRows = rowsInFullTable + 1;
                int numTableColumns = tableData.GetLength(0);

                // Make 1-D array of table rows, delimited by commas
                string[] fileContents = new string[numTableRows];
                const string comma = ",";
                int i; // row in table
                for (i = 0; i < numTableRows; i++)
                {
                    string line;
                    int j; // column in table
                    if (i == 0)
                    {
                        // Enclose headings in quotes and build line
                        string heading = tableData[0, 0];
                        heading = heading.Trim();
                        heading = @"""" + heading + @"""";
                        line = heading;
                        for (j = 1; j < numTableColumns; j++)
                        {
                            heading = tableData[j, 0];
                            heading = heading.Trim();
                            heading = @"""" + heading + @"""";
                            line = line + comma + heading;
                        }
                    }
                    else
                    {
                        // Build line without enclosing values in quotes
                        line = tableData[0, i];
                        for (j = 1; j < numTableColumns; j++)
                        {
                            line = line + comma + tableData[j, i];
                        }
                    }
                    fileContents[i] = line;
                }

                // Write table data to file
                FileUtil.WriteFile(fileName, fileContents);
            }
        }
        #endregion Public methods

        #region IReportElement Members
        public int NumDataSeries
        {
            get
            {
                return _dataSeriesList.Count;
            }
        }
        public int NumVisibleDataSeries
        {
            get
            {
                int count = 0;
                for (int i = 0; i < _dataSeriesList.Count; i++)
                {
                    if (_dataSeriesList[i].Visible)
                    {
                        count++;
                    }
                }
                return count;
            }
        }
        public Form GetForm()
        {
            return new ReportElementTableMenu(this);
        }
        private DateTime[] getRowDates()
        {
            // If there are no data series, return an empty array.
            if (this.NumDataSeries == 0)
            {
                return new DateTime[0];
            }

            // If there are no visible data series, return an empty array
            int countVisible = 0;
            DataSeries firstVisibleDataSeries = null;
            for (int i = 0; i < _dataSeriesList.Count; i++)
            {
                if (_dataSeriesList[i].Visible)
                {
                    countVisible++;
                    if (firstVisibleDataSeries == null)
                    {
                        firstVisibleDataSeries = _dataSeriesList[i];
                    }
                }
            }
            if (countVisible == 0)
            {
                return new DateTime[0];
            }

            // Otherwise, return an array of the dates in the first visible data series.
            // Get the time series from the first data provider.
            int datasetStatus;
            TimeSeries timeSeries = (TimeSeries)firstVisibleDataSeries.GetData(out datasetStatus);

            // Return an array of the dates in the time series.
            if (timeSeries == null)
            {
                return new DateTime[0];
            }
            else
            {
                DateTime[] dates = new DateTime[timeSeries.Length];
                for (int i = 0; i < timeSeries.Length; i++)
                {
                    dates[i] = timeSeries[i].Date;
                }
                return dates;
            }
        }
        public string[,] GetTableData(bool truncateForPreview, out bool isTruncated, int dataSeriesIndex, out int rowsInFullTable)
        {
            // Get the row dates of the data series.
            DateTime[] rowDates = getRowDates();
            rowsInFullTable = rowDates.Length;

            // Make an array of the column indices to include in this table.
            int[] columnIndices;
            if (dataSeriesIndex < 0)
            {
                columnIndices = new int[this.NumVisibleDataSeries];
                for (int i = 0; i < columnIndices.Length; i++)
                {
                    columnIndices[i] = i;
                }
            }
            else
            {
                columnIndices = new int[] { dataSeriesIndex };
            }

            // Make the string array that will store the data for this table. For the preview, limit to 30 data rows.
            int numColumns = columnIndices.Length + 1;
            int numRows = rowDates.Length + 1;
            isTruncated = false;
            int maxNumRows = 31;
            if (truncateForPreview && numRows > maxNumRows)
            {
                numRows = maxNumRows;
                isTruncated = true;
            }
            string[,] tableData = new string[numColumns, numRows];

            // Populate the header rows in the table.
            if (IncludeTimeInTable)
            {
                tableData[0, 0] = "Date and Time";
            }
            else
            {
                tableData[0, 0] = "Date";
            }

            // Populate the column headings for the visible data series
            int numDataSeries = this.NumDataSeries;
            DataSeries currentDataSeries = null;
            int col = 0;
            for (int i = 0; i < numDataSeries; i++)
            {
                currentDataSeries = _dataSeriesList[i];
                if (currentDataSeries.Visible)
                {
                    col++;
                    tableData[col, 0] = currentDataSeries.Name;
                }
            }

            // Populate the dates in the table.
            for (int i = 0; i < numRows - 1; i++)
            {
                if (IncludeTimeInTable)
                {
                    tableData[0, i + 1] = rowDates[i].ToShortDateString() + " " + rowDates[i].ToShortTimeString();
                }
                else
                {
                    tableData[0, i + 1] = rowDates[i].ToShortDateString();
                }
            }

            // Populate the cells in the table.
            col = 0;
            for (int i = 0; i < numDataSeries; i++)
            {
                currentDataSeries = _dataSeriesList[i];
                // Get the time series for this column.
                int datasetStatus;
                TimeSeries timeSeries = (TimeSeries)currentDataSeries.GetData(out datasetStatus);

                // Populate the rows for this series.
                if (timeSeries != null)
                {
                    if (currentDataSeries.Visible)
                    {
                        col++;
                        for (int j = 0; j < numRows - 1; j++)
                        {
                            float value = timeSeries.GetValue(rowDates[j]);
                            tableData[col, j + 1] = value.ToString();
                        }
                    }
                }
            }

            // Return the table data.
            return tableData;
        }
        public Image GetImage()
        {
            // If there are no data series, return an "empty table" image.
            if (this.NumDataSeries == 0)
            {
                return TableMaker.MakeTableImage(this.ToString(), new string[,] { { "No Data Series" } }, false, 0, IncludeTimeInTable);
            }

            // Get the table data.
            bool isTruncated;
            int rowsInFullTable;
            string[,] tableData = GetTableData(true, out isTruncated, -1, out rowsInFullTable);

            // Populate the header row (first column) in the table.
            if (IncludeTimeInTable)
            {
                tableData[0, 0] = "Date and Time";
            }
            else
            {
                tableData[0, 0] = "Date";
            }

            // Populate the dates in the table.
            int numRows = tableData.GetLength(1);
            int datasetStatus;

            // Find first visible data series
            DataSeries firstVisibleDataSeries = null;
            for (int i = 0; i < _dataSeriesList.Count; i++)
            {
                if (_dataSeriesList[i].Visible)
                {
                    firstVisibleDataSeries = _dataSeriesList[i];
                    break;
                }
            }

            if (firstVisibleDataSeries != null)
            {
                object obj0 = firstVisibleDataSeries.GetData(out datasetStatus);
                if (obj0 is TimeSeries)
                {
                    TimeSeries ts0 = (TimeSeries)obj0;

                    for (int i = 0; i < (numRows - 1); i++)
                    {
                        // Optionally include Time as well as Date
                        if (IncludeTimeInTable)
                        {
                            tableData[0, i + 1] = ts0[i].Date.ToShortDateString() + " " + ts0[i].Date.ToShortTimeString();
                        }
                        else
                        {
                            tableData[0, i + 1] = ts0[i].Date.ToShortDateString();
                        }
                    }
                }
            }


            // Return an image of the table.
            return TableMaker.MakeTableImage(this.ToString(), tableData, isTruncated, rowsInFullTable, IncludeTimeInTable);
        }
        public Image GetImage(float minValue, float maxValue, long minTicks, long maxTicks,
                              double xcMin, double xcMax, double ycMin, double ycMax, bool isStandalone)
        {
            // Make the image.
            int width = 800;
            int height = 400;
            Bitmap image = new Bitmap(width, height);

            // Fill the background.
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
            g.FillRectangle(Brushes.White, 0, 0, width, height);

            // Draw the top and left borders.
            int leftBuffer = 20;
            int rightBuffer = 20;
            int bottomBuffer = 20;
            int topBuffer = 20;
            // top border
            g.DrawLine(Pens.Black, leftBuffer, height - bottomBuffer, width - rightBuffer, height - bottomBuffer);
            // left border
            g.DrawLine(Pens.Black, leftBuffer, height - bottomBuffer, leftBuffer, topBuffer);

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
                    _dataSeriesList[i].DrawContent(g, width, height, minValue, maxValue, 
                                                  minTicks, maxTicks, float.NaN, float.NaN, 
                                                  float.NaN, float.NaN, true);
                }
            }

            // Dispose of the graphics object to reclaim resources.
            g.Dispose();

            return image;
        }
        public void ClearImage()
        {
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
        public DataSeries GetDataSeries(int index)
        {
            return _dataSeriesList[index];
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
        #endregion IReportElement Members

    }
}
