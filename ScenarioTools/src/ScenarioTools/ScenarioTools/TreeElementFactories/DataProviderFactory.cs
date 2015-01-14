using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using ScenarioTools.Data_Providers;
using ScenarioTools.DatasetCalculator;
using ScenarioTools.Reporting;

namespace ScenarioTools.TreeElementFactories
{
    public class DataProviderFactory
    {
        public static IDataProvider GraphDataProviderFromXml(XmlElement xmlElement, IReportElement parentElement, string sourceFileName)
        {
            // Declare the data provider and get the XML element name.
            IDataProvider dataProvider = null;
            string name = xmlElement.Name.Trim().ToLower();

            // Make the appropriate type of data provider.
            if (name.Equals("dataprovidercalculatedseries")) dataProvider = new DataProviderCalculatedSeries(parentElement);

            else if (name.Equals("dataprovidercbbatpoint")) dataProvider = new DataProviderCbbAtPoint();
            else if (name.Equals("dataprovidercbbgroup")) dataProvider = new DataProviderCbbGroup();

            else if (name.Equals("dataproviderheadatpoint")) dataProvider = new DataProviderHeadAtPoint();
            else if (name.Equals("dataproviderheadgroup")) dataProvider = new DataProviderHeadGroup();

            else if (name.Equals("dataproviderobservedseries")) dataProvider = new DataProviderObservedSeries();            

            // Initialize the report element with the XML element.
            if (dataProvider != null)
            {
                dataProvider.InitFromXml(xmlElement, sourceFileName);
            }

            return dataProvider;
        }

        public static IDataProvider ChartDataProviderFromXml(XmlElement xmlElement, IReportElement parentElement, string sourceFileName)
        {
            // Declare the data provider and get the XML element name.
            IDataProvider dataProvider = null;
            string name = xmlElement.Name.Trim().ToLower();

            // Make the appropriate type of data provider.
            if (name.Equals("dataprovidercalculatedseries")) dataProvider = new DataProviderCalculatedSeries(parentElement);

            else if (name.Equals("dataprovidercbbatpoint")) dataProvider = new DataProviderCbbAtPoint();
            else if (name.Equals("dataprovidercbbgroup")) dataProvider = new DataProviderCbbGroup();

            else if (name.Equals("dataproviderheadatpoint")) dataProvider = new DataProviderHeadAtPoint();
            else if (name.Equals("dataproviderheadgroup")) dataProvider = new DataProviderHeadGroup();

            else if (name.Equals("dataproviderobservedseries")) dataProvider = new DataProviderObservedSeries();

            // Initialize the report element with the XML element.
            if (dataProvider != null)
            {
                dataProvider.InitFromXml(xmlElement, sourceFileName);
            }

            return dataProvider;
        }

        public static IDataProvider STMapDataProviderFromXml(XmlElement xmlElement, ReportElementSTMap parentMap, string sourceFileName)
        {
            // Declare the data provider and get the XML element name.
            IDataProvider dataProvider = null;
            string name = xmlElement.Name.Trim().ToLower();

            // Make the appropriate type of data provider.
            if (name.Equals("dataprovidercalculatedmap")) dataProvider = new DataProviderCalculatedMap(parentMap);
            else if (name.Equals("dataprovidercbbmap")) dataProvider = new DataProviderCbbMap();
            else if (name.Equals("dataproviderheadmap")) dataProvider = new DataProviderHeadMap();

            // Initialize the report element with the XML element.
            if (dataProvider != null)
            {
                dataProvider.InitFromXml(xmlElement, sourceFileName);
            }

            return dataProvider;
        }

        public static IDataProvider TableDataProviderFromXml(XmlElement xmlElement)
        {
            throw new Exception("Should not be creating a table data provider.");
        }

    }
}
