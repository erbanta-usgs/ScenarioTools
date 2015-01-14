using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using ScenarioTools.Reporting;

namespace ScenarioTools.TreeElementFactories
{
    public class ReportElementFactory
    {
        public static IReportElement FromXml(XmlElement xmlElement, string sourceFileName)
        {
            // Make the appropriate type of report element.
            IReportElement reportElement = null;
            string name = xmlElement.Name.Trim().ToLower();
            if (name.Equals("reportelementstmap"))
            {
                reportElement = new ReportElementSTMap();
            }
            else if (name.Equals("reportelementtable"))
            {
                reportElement = new ReportElementTable();
            }
            else if (name.Equals("reportelementchart"))
            {
                reportElement = new ReportElementChart();
            }

            // Initialize the report element with the XML element.
            if (reportElement != null)
            {
                reportElement.InitFromXML(xmlElement, sourceFileName);
            }

            return reportElement;
        }
    }
}
