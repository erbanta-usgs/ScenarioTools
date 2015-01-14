using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ScenarioTools.LogAndErrorProcessing;

namespace ScenarioTools.Geometry
{
    public abstract class Shape2D
    {
        List<ShapeAttribute> attributes;

        public Shape2D()
        {
            // Make the attribute list.
            attributes = new List<ShapeAttribute>();
        }
        public void SetAttribute(string name, object value)
        {
            // If the attribute exists, remove it.
            ShapeAttribute existingAttribute = attributes.Find(
                delegate(ShapeAttribute attribute)
                {
                    return attribute.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
                });
            if (existingAttribute != null)
            {
                attributes.Remove(existingAttribute);
            }

            // Add the new attribute.
            attributes.Add(new ShapeAttribute(name, value));
        }
        public object GetAttributeValue(string name, object defaultValue)
        {
            // Get the attribute.
            ShapeAttribute attribute = this.GetAttribute(name);

            // If the attribute is null, return the default value.
            if (attribute == null)
            {
                return defaultValue;
            }

            // Otherwise, return the attribute value.
            else
            {
                return attribute.Value;
            }
        }
        public object GetAttributeValue(string name)
        {
            return GetAttributeValue(name, null);
        }
        public ShapeAttribute GetAttribute(string name)
        {
            ShapeAttribute existingAttribute = attributes.Find(
                delegate(ShapeAttribute attribute)
                {
                    return attribute.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
                });

            return existingAttribute;
        }
        public ShapeAttribute GetAttribute(int index)
        {
            return attributes[index];
        }

        public int NumAttributes
        {
            get
            {
                return attributes.Count;
            }
        }

        protected static void CleanForShapefile(string baseFilename)
        {
            string[] extensions = { "shp", "shx", "sbn", "sbx", "dbf", "prj" };

            for (int i = 0; i < extensions.Length; i++)
            {
                string filename = baseFilename + "." + extensions[i];
                if (File.Exists(filename))
                {
                    try
                    {
                        File.Delete(filename);
                    }
                    catch (Exception e)
                    {
                        Logging.Update("Error in cleaning up a shapefile. Unable to delete " + filename + ". ERROR: " + e.Message + ".", 
                            Logging.ERROR_50_SERIOUS_ERROR_NO_IMMEDIATE_DATA_LOSS);
                    }
                }
            }
        }
        public abstract int NumVertices
        {
            get;
        }
        public abstract Range2D Extent
        {
            get;
        }
        public abstract Point2D GetVertex(int index);

        public static Polyline2D[] AsPolylineArray(Shape2D[] shapes)
        {
            Polyline2D[] polylines = new Polyline2D[shapes.Length];
            for (int i = 0; i < polylines.Length; i++)
            {
                polylines[i] = (Polyline2D)shapes[i];
            }

            return polylines;
        }
    }
}
