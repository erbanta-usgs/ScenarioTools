using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;

using ScenarioTools.ImageProvider;
using ScenarioTools.TreeElementFactories;
using ScenarioTools.Xml;

namespace ScenarioTools.Reporting
{
    public class Report : IImageProvider
    {
        private const string XML_NODE_NAME = "Report";
        private const string XML_NODE_NAME_WORKSPACE = "Workspace";

        private const string XML_KEY_NAME = "name";
        private const string XML_KEY_AUTHOR = "author";

        private string name;
        private string author;
        List<IReportElement> elements;

        public Report()
        {
            // Make the list for the elements.
            elements = new List<IReportElement>();
        }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string Author
        {
            get
            {
                return author;
            }
            set
            {
                author = value;
            }
        }
        public int NumElements
        {
            get
            {
                return elements.Count;
            }
        }

        public void AddElement(IReportElement element)
        {
            // Add the element to the list.
            elements.Add(element);
        }

        public override string ToString()
        {
            string unnamedReport = "Unnamed Report";
            return "Report: " + (name == null ? unnamedReport : (name.Equals("") ? unnamedReport : name));
        }

        #region IImageProvider Members

        public Image GetImage()
        {
            int width = 200;
            int height = 100;

            // Make the image and create the graphics.
            Image image = new Bitmap(width, height);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);

            // Fill the background.
            g.FillRectangle(Brushes.White, 0.0f, 0.0f, width, height);

            // Draw the report name.
            Font font = new Font(FontFamily.GenericSansSerif, 15.0f);
            StringFormat stringFormat = new StringFormat(StringFormat.GenericDefault);
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            g.DrawString(this.ToString(), font, Brushes.Black, new RectangleF(0.0f, 0.0f, width, height), stringFormat);

            // Dispose the graphics object to reclaim resources.
            g.Dispose();

            return image;
        }
        public Image GetImage(float minValue, float maxValue, long minTicks, long maxTicks,
                              double xcMin, double xcMax, double ycMin, double ycMax, bool isStandalone)
        {
            return GetImage();
        }

        #endregion
        public void ClearImage()
        {
        }

        public IReportElement GetElement(int index)
        {
            return elements[index];
        }

        public XmlNode GetXmlNode(XmlDocument document, string targetFileName)
        {
            // Make the XML element that represents this object.
            XmlElement element = document.CreateElement(XML_NODE_NAME);

            // Prepare the XML element.
            PrepareXmlElement(document, element, targetFileName);

            // Return the result.
            return element;
        }
        public void SaveXmlFile(string file)
        {
            // Make the XML file.
            XmlDocument document = XmlUtil.CreateXmlFile(file, XML_NODE_NAME);

            // Prepare the root element.
            PrepareXmlElement(document, document.DocumentElement, file);

            // Write the file.
            document.Save(file);
        }
        public void PrepareXmlElement(XmlDocument document, XmlElement element, string targetFileName)
        {
            // Set the attributes.
            XmlUtil.FrugalSetAttribute(element, XML_KEY_NAME, Name, "");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_AUTHOR, Author, "");

            // Append all of the report elements as children.
            for (int i = 0; i < elements.Count; i++)
            {
                // Get the report element XML node.
                XmlNode reportElementNode = elements[i].GetXmlNode(document, targetFileName);

                // Append the report element node to the root.
                element.AppendChild(reportElementNode);
            }
        }

        public void InitFromXmlFile(string file, string sourceFileName)
        {
            // Get the root from the file.
            XmlElement root = XmlUtil.GetRootOfFile(file);

            // Initialize from the root.
            InitFromXmlElement(root, sourceFileName);
        }
        public void InitFromXmlElement(XmlElement element, string sourceFileName)
        {
            // Get the name and unique identifier.
            name = XmlUtil.SafeGetStringAttribute(element, XML_KEY_NAME, "");
            author = XmlUtil.SafeGetStringAttribute(element, XML_KEY_AUTHOR, "");

            // Get the report elements and add them to the list.
            foreach (XmlElement child in element.ChildNodes)
            {
                IReportElement reportElement = ReportElementFactory.FromXml(child, sourceFileName);
                if (reportElement != null)
                {
                    elements.Add(reportElement);
                }
            }
        }

        public void ClearElements()
        {
            this.elements.Clear();
        }

        public static void SaveXmlFile(Report[] reports, string file)
        {
            // Make the XML file.
            XmlDocument document = XmlUtil.CreateXmlFile(file, XML_NODE_NAME_WORKSPACE);

            // Prepare the root element.
            prepareXmlElement(reports, document, document.DocumentElement, file);

            // Show the root.
            XmlUtil.PrintNode(document.DocumentElement);

            // If the file exists, delete it.
            if (File.Exists(file))
            {
                File.Delete(file);
                Console.WriteLine("file: " + Path.GetFullPath(file));
                Console.WriteLine("Deleted file");
            }

            // Write the file.
            document.Save(file);
        }
        private static void prepareXmlElement(Report[] reports, XmlDocument document, XmlElement element, string targetFileName)
        {
            // Append all of the reports as children.
            for (int i = 0; i < reports.Length; i++)
            {
                // Get the report XML node.
                XmlNode reportNode = reports[i].GetXmlNode(document,targetFileName);

                // Append the report node to this node.
                element.AppendChild(reportNode);
            }
        }
        public static Report[] WorkspaceFromXmlFile(string file)
        {
            // Get the root of the specified file.
            XmlElement root = XmlUtil.GetRootOfFile(file);

            // Make a list for the reports.
            List<Report> reports = new List<Report>();

            // Make a report from each child node.
            foreach (XmlElement child in root.ChildNodes)
            {
                Report report = new Report();
                report.InitFromXmlElement(child, file);
                reports.Add(report);
            }

            // Return the reports.
            return reports.ToArray();
        }

        public void ValidateDataProviderKeys(List<long> uniqueIdentifiers)
        {
            // Validate the data provider keys of all report elements.
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].ValidateDataProviderKeys(uniqueIdentifiers);
            }
        }

        public void UpdateCaches()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].UpdateCaches();
            }
        }
    }
}
