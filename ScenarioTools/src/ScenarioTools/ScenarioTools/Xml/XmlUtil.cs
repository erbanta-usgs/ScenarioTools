using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml;
using System.IO;

namespace ScenarioTools.Xml
{
    public class XmlUtil
    {
        public static void FrugalSetAttribute(XmlElement element, string key, string value, string defaultValue)
        {
            // Only set the attribute if the value is non-null and not equal to the default value.
            if (value != null)
            {
                if (!value.Equals(defaultValue))
                {
                    element.SetAttribute(key, value);
                }
            }
        }
        public static void FrugalSetAttribute(XmlElement element, string key, bool value, bool defaultValue)
        {
            if (value != defaultValue)
            {
                FrugalSetAttribute(element, key, value.ToString(), defaultValue.ToString());
            }
        }

        public static bool SafeGetBoolAttribute(XmlElement element, string key, bool defaultValue)
        {
            string aDefault = defaultValue.ToString();
            string aResult = SafeGetStringAttribute(element, key, aDefault);
            return ScenarioTools.Util.StringUtil.StringToBool(aResult);
        }
        public static string SafeGetStringAttribute(XmlElement element, string key, string defaultValue)
        {
            // Get the attribute value for the specified key.
            string value = element.GetAttribute(key);

            // If the value is null or blank, return default
            if (value == null || value == "")
            {
                return defaultValue;
            }

            // Otherwise, return value
            return value;
        }
        public static int SafeGetIntAttribute(XmlElement element, string key, int defaultValue)
        {
            // Get the attribute value for the specified key.
            string value = element.GetAttribute(key);

            // If the value is non-null, try to convert it to an integer and return it.
            if (value != null)
            {
                int parsedValue;
                if (int.TryParse(value, out parsedValue))
                {
                    return parsedValue;
                }
            }

            // Otherwise, return the default value.
            return defaultValue;
        }
        public static double SafeGetDoubleAttribute(XmlElement element, string key, double defaultValue)
        {
            // Get the attribute value for the specified key.
            string value = element.GetAttribute(key);

            // If the value is non-null, try to convert it to an integer and return it.
            if (value != null)
            {
                double parsedValue;
                if (double.TryParse(value, out parsedValue))
                {
                    return parsedValue;
                }
            }

            // Otherwise, return the default value.
            return defaultValue;
        }
        public static float SafeGetFloatAttribute(XmlElement element, string key, float defaultValue)
        {
            // Get the attribute value for the specified key.
            string value = element.GetAttribute(key);

            // If the value is non-null, try to convert it to an integer and return it.
            if (value != null)
            {
                float parsedValue;
                if (float.TryParse(value, out parsedValue))
                {
                    return parsedValue;
                }
            }

            // Otherwise, return the default value.
            return defaultValue;
        }
        public static long SafeGetLongAttribute(XmlElement element, string key, long defaultValue)
        {
            // Get the attribute value for the specified key.
            string value = element.GetAttribute(key);

            // If the value is non-null, try to convert it to a long and return it.
            if (value != null)
            {
                long parsedValue;
                if (long.TryParse(value, out parsedValue))
                {
                    return parsedValue;
                }
            }

            // Otherwise, return the default value.
            return defaultValue;
        }        
        public static Color SafeGetColorAttribute(XmlElement element, string key, Color defaultValue)
        {
            // Get the attribute value for the specified key.
            string value = element.GetAttribute(key);

            // If the value is non-null, try to convert it to a color and return it.
            if (value != null)
            {
                return XmlStringToColor(value, defaultValue);
            }

            // Otherwise, return the default value.
            else
            {
                return defaultValue;
            }
        }

        public static XmlElement GetRootOfFile(string file)
        {
            // Load the XML file.
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(file);

            // Return the document element.
            return xmlDocument.DocumentElement;
        }

        public static string ColorToXmlString(Color color)
        {
            return color.R + "," + color.G + "," + color.B + "," + color.A;
        }
        public static Color XmlStringToColor(string colorString, Color defaultValue)
        {
            try
            {
                string[] split = colorString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                int r = int.Parse(split[0]);
                int g = int.Parse(split[1]);
                int b = int.Parse(split[2]);
                int a = int.Parse(split[3]);
                return Color.FromArgb(a, r, g, b);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static XmlDocument CreateXmlStreamWriter(Stream stream, string rootElementName)
        {
            XmlTextWriter xmlWriter = new XmlTextWriter(stream, System.Text.Encoding.UTF8);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.WriteProcessingInstruction("xml", "version='1.0'");
            xmlWriter.WriteStartElement(rootElementName);

            XmlDocument document = new XmlDocument();
            document.WriteContentTo(xmlWriter);

            return document;
        }
        public static XmlDocument CreateXmlFile(string fileName, string rootElementName)
        {
            // Create the XML writer.
            XmlTextWriter xmlWriter = new XmlTextWriter(fileName, System.Text.Encoding.UTF8);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.WriteProcessingInstruction("xml", "version='1.0'");
            xmlWriter.WriteStartElement(rootElementName);
            xmlWriter.Close();

            // Create the XML document, load the file, and return.
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(fileName);
            return xmlDocument;
        }

        public static string DateToXmlString(DateTime date)
        {
            // A date can be:
            // 1: CCYY/MM/DD
            // 2: CCYY/MM/DD:HH:MM:SS
            // 3: CCYY/MM/DD:HH:MM:SS.ddd (three digits of precision here, 
            //        an arbitrary number of digits on the read side)

            // Make the string builder and assemble the result.
            StringBuilder s = new StringBuilder();
            s.Append(date.Year.ToString().PadLeft(4, '0') + "/" 
                + date.Month.ToString().PadLeft(2, '0') + "/" 
                + date.Day.ToString().PadLeft(2, '0'));

            // Return the result.
            return s.ToString();
        }
        public static DateTime DateFromXmlString(string dateString)
        {
            return DateFromXmlString(dateString, new DateTime());
        }
        public static DateTime DateFromXmlString(string dateString, DateTime defaultDateTime)
        {
            try
            {
                // Split the string on forward slashes.
                string[] split = dateString.Split(new char[] { '/' });

                // Get the year, month, and day.
                int year = int.Parse(split[0]);
                int month = int.Parse(split[1]);
                int day = int.Parse(split[2]);

                // Return the result.
                return new DateTime(year, month, day);
            }
            catch
            {
                return defaultDateTime;
            }
        }
        public static void PrintNode(XmlNode node)
        {
            printNode(node, 0);
        }
        private static void printNode(XmlNode node, int level)
        {
            // Print the level dashes.
            for (int i = 0; i < level; i++)
            {
                Console.Write("-");
            }

            // Print the name.
            Console.WriteLine(node.Name);

            // Print the child nodes.
            foreach (XmlNode child in node.ChildNodes)
            {
                printNode(child, level + 1);
            }
        }

        public static XmlElement GetRootOfFile(Stream stream)
        {
            // Load the XML file.
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(stream);

            // Return the document element.
            return xmlDocument.DocumentElement;
        }
    }
}
