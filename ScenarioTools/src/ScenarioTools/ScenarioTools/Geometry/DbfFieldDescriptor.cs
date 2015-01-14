using System;
using System.Collections.Generic;
using System.Text;

using MapTools;

namespace ScenarioTools.Geometry
{
    public class DbfFieldDescriptor
    {
        private string name;
        private ShapeLib.DBFFieldType fieldType;
        private int length;

        public string Name
        {
            get
            {
                return name;
            }
        }
        public ShapeLib.DBFFieldType FieldType
        {
            get
            {
                return fieldType;
            }
        }
        public int Length
        {
            get
            {
                return length;
            }
        }

        public DbfFieldDescriptor(string name, ShapeLib.DBFFieldType fieldType, int length)
        {
            this.name = name;
            this.fieldType = fieldType;
            this.length = length;
        }
        public static DbfFieldDescriptor[] GetFieldDescriptors(Shape2D[] shapes)
        {
            // Make lists for the names, the names in lower case, the types, and the lengths.
            List<string> names = new List<string>();
            List<string> namesLower = new List<string>();
            List<int> lengths = new List<int>();
            List<ShapeLib.DBFFieldType> fieldTypes = new List<ShapeLib.DBFFieldType>();
            
            // Add all unique field desciptors.
            foreach (Shape2D shape in shapes) {
                for (int i = 0; i < shape.NumAttributes; i++)
                {
                    // Get the attribute from the shape.
                    ShapeAttribute attribute = shape.GetAttribute(i);

                    // Determine the value type.
                    ShapeLib.DBFFieldType fieldType;
                    if (attribute.Value is DateTime)
                    {
                        fieldType = ShapeLib.DBFFieldType.FTDate;
                    }
                    else if (attribute.Value is Single || attribute.Value is Double)
                    {
                        fieldType = ShapeLib.DBFFieldType.FTDouble;
                    }
                    else if (attribute.Value is Int16 || attribute.Value is Int32)
                    {
                        fieldType = ShapeLib.DBFFieldType.FTInteger;
                    }
                    else if (attribute.Value is Boolean)
                    {
                        fieldType = ShapeLib.DBFFieldType.FTLogical;
                    }
                    else
                    {
                        fieldType = ShapeLib.DBFFieldType.FTString;
                    }

                    // Determine the length of the string representation of the value.
                    int length = attribute.Value.ToString().Length;

                    // Check if the attribute name is already contained in the list.
                    int indexOfAttribute = (namesLower.IndexOf(attribute.Name.ToLower()));

                    // If the name is not yet contained, add it to the list.
                    if (indexOfAttribute < 0)
                    {
                        names.Add(attribute.Name);
                        namesLower.Add(attribute.Name.ToLower());
                        lengths.Add(length);
                        fieldTypes.Add(fieldType);
                    }

                    // Otherwise, reconcile the attribute with the list.
                    else
                    {
                        // The length is the length of the string representation. We want to know the maximum length for all fields.
                        lengths[indexOfAttribute] = Math.Max(lengths[indexOfAttribute], length);

                        // Determine what datatypes are present. We need to use the least generic datatype that is generic enough for all field values.

                        // If either type is a string, the field has to be a string.
                        if (fieldTypes[indexOfAttribute] == ShapeLib.DBFFieldType.FTString || fieldType == ShapeLib.DBFFieldType.FTString) {
                            fieldTypes[indexOfAttribute] = ShapeLib.DBFFieldType.FTString;
                        }

                        // At this point, we know that the only datatypes are: Date, Double, Integer, Logical.

                        // Otherwise, if both are dates, the field has to be a date (no assignment necessary -- leave value).
                        else if (fieldTypes[indexOfAttribute] == ShapeLib.DBFFieldType.FTDate && fieldType == ShapeLib.DBFFieldType.FTDate) {
                        }

                        // Otherwise, if either is a date, the field must be a string (a date and a non-date can only be reconciled as a string).
                        else if (fieldTypes[indexOfAttribute] == ShapeLib.DBFFieldType.FTDate || fieldType == ShapeLib.DBFFieldType.FTDate) {
                            fieldTypes[indexOfAttribute] = ShapeLib.DBFFieldType.FTString;
                        }

                        // At this point, we know that the only datatypes are: Double, Integer, Logical.

                        // Otherwise, if either is a floating-point number, the field must be a floating-point number.
                        else if (fieldTypes[indexOfAttribute] == ShapeLib.DBFFieldType.FTDouble || fieldType == ShapeLib.DBFFieldType.FTDouble) {
                            fieldTypes[indexOfAttribute] = ShapeLib.DBFFieldType.FTDouble;
                        }

                        // Otherwise, if either field is an integer, the field must be an integer.
                        else if (fieldTypes[indexOfAttribute] == ShapeLib.DBFFieldType.FTInteger || fieldType == ShapeLib.DBFFieldType.FTInteger) {
                            fieldTypes[indexOfAttribute] = ShapeLib.DBFFieldType.FTInteger;
                        }

                        // Otherwise, the field is logical. Leave it as it is.
                    }

                }
            }

            // Make the descriptor array.
            DbfFieldDescriptor[] descriptors = new DbfFieldDescriptor[names.Count];
            for (int i = 0; i < descriptors.Length; i++)
            {
                descriptors[i] = new DbfFieldDescriptor(names[i], fieldTypes[i], lengths[i]);
            }

            // Return the desciptors.
            return descriptors;
        }

        public void AddToDbf(IntPtr dbfFile)
        {
            // Determine the length and the number of decimals.
            int fieldWidth, numDecimals;
            if (this.FieldType == ShapeLib.DBFFieldType.FTInteger)
            {
                fieldWidth = 16;
                numDecimals = 0;
            }
            else if (this.FieldType == ShapeLib.DBFFieldType.FTString)
            {
                fieldWidth = this.Length;
                numDecimals = 0;
            }
            else if (this.FieldType == ShapeLib.DBFFieldType.FTLogical)
            {
                fieldWidth = 5;
                numDecimals = 0;
            }
            else if (this.FieldType == ShapeLib.DBFFieldType.FTDouble)
            {
                fieldWidth = 32;
                numDecimals = 10;
            }
            else // if (this.FieldType == ShapeLib.DBFFieldType.FTDate)
            {
                fieldWidth = 8;
                numDecimals = 0;
            }

            // If the field type is date, flip to integer.
            ShapeLib.DBFFieldType typeToWrite = this.FieldType == ShapeLib.DBFFieldType.FTDate ? ShapeLib.DBFFieldType.FTInteger : this.FieldType;
            
            // Write the field to the DBF.
            ShapeLib.DBFAddField(dbfFile, this.Name, this.FieldType, fieldWidth, numDecimals);
        }

        public void WriteAttributeValue(IntPtr dbfFile, int shape, int ordinal, object value)
        {
            if (this.FieldType == ShapeLib.DBFFieldType.FTDate)
            {
                DateTime attValue = new DateTime(0);
                try
                {
                    attValue = (DateTime)value;
                }
                catch {}
                ShapeLib.DBFWriteDateAttribute(dbfFile, shape, ordinal, attValue);
            }
            else if (this.FieldType == ShapeLib.DBFFieldType.FTDouble)
            {
                double attValue = 0.0;
                try
                {
                    attValue = Double.Parse(value + "");
                }
                catch { }
                ShapeLib.DBFWriteDoubleAttribute(dbfFile, shape, ordinal, attValue);
            }
            else if (this.FieldType == ShapeLib.DBFFieldType.FTInteger)
            {
                int attValue = 0;
                try
                {
                    attValue = (int)value;
                }
                catch { }
                ShapeLib.DBFWriteIntegerAttribute(dbfFile, shape, ordinal, attValue);
            }
            else if (this.FieldType == ShapeLib.DBFFieldType.FTLogical)
            {
                bool attValue = false;
                try
                {
                    attValue = (bool)value;
                }
                catch { }
                ShapeLib.DBFWriteLogicalAttribute(dbfFile, shape, ordinal, attValue);
            }
            else
            {
                string attValue = value + "";
                if (attValue.Length > this.Length)
                {
                    attValue = attValue.Substring(0, this.Length);
                }
                ShapeLib.DBFWriteStringAttribute(dbfFile, shape, ordinal, attValue);
            }
        }
    }
}
