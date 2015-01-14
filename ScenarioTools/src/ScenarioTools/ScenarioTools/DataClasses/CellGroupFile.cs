using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Xml;

using ScenarioTools.Util;
using ScenarioTools.Xml;

namespace ScenarioTools.DataClasses
{
    public class CellGroupFile : CellGroupProvider
    {
        #region Fields
        string _name;
        CellGroup[] _cellGroups;
        #endregion Fields

        #region Constructors
        private CellGroupFile()
        {
            this._name = "";
            this._cellGroups = null;
        }
        private CellGroupFile(string name, CellGroup[] cellGroups) : this()
        {
            // Store references to the name and cell groups.
            this._name = name;
            this._cellGroups = cellGroups;

            // Set this as the parent of all the cell groups.
            foreach (CellGroup group in cellGroups)
            {
                group.Parent = this;
            }
        }
        #endregion Constructors

        #region Properties
        public string Name
        {
            get
            {
                return _name;
            }
        }
        public int NumGroups
        {
            get
            {
                if (_cellGroups != null)
                {
                    return _cellGroups.Length;
                }
                else
                {
                    return 0;
                }
            }
        }
        public CellGroup this[int index]
        {
            get
            {
                return _cellGroups[index];
            }
        }
        #endregion Properties

        #region Methods
        public override bool Equals(object obj)
        {
            if (obj is CellGroupFile)
            {
                return this.Name.Equals(((CellGroupFile)obj).Name, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return false;
            }
        }
        public static CellGroupFile FromFile(string fileName)
        {
            // Get the contents of the file.
            string[] fileContents = FileUtil.GetFileContents(fileName);

            // Make a list for the cell groups.
            List<CellGroup> cellGroups = new List<CellGroup>();

            // These are the components of a cell group.
            string name = null;
            List<Point> cells = null;
            for (int i = 0; i < fileContents.Length; i++)
            {
                // Try to get a cell from the current line. If we can recognize it 
                // as a cell, add it to the list of cells. Otherwise, treat it as
                // an identifier (INTERCAL :).
                Point cell;
                if (tryParse(fileContents[i], out cell))
                {
                    cells.Add(cell);
                }
                else
                {
                    // This is an identifier. If there is an existing group, 
                    // package it and add it to the list of groups.
                    if (name != null && cells != null)
                    {
                        if (cells.Count > 0)
                        {
                            cellGroups.Add(new CellGroup(name, cells.ToArray()));
                        }
                    }

                    // Store the new identifier and make a new list for cells.
                    name = fileContents[i];
                    cells = new List<Point>();
                }
            }

            // If valid, add the last group of cells to the list of groups.
            if (name != null && cells != null)
            {
                if (cells.Count > 0)
                {
                    cellGroups.Add(new CellGroup(name, cells.ToArray()));
                }
            }

            // Return the groups as an array.
            return new CellGroupFile(Path.GetFullPath(fileName), cellGroups.ToArray());
        }
        public override XmlNode GetXmlNode(XmlDocument document, string targetFileName)
        {
            // Make the element that represents this object.
            XmlElement element = document.CreateElement("Group");

            // Set the attributes.
            string dir = Path.GetDirectoryName(targetFileName);
            string relPath = FileUtil.Absolute2Relative(Name, dir);
            XmlUtil.FrugalSetAttribute(element, XML_KEY_FILE, relPath, "");

            // Return the result.
            return element;
        }
        private static bool tryParse(string s, out Point cell)
        {
            // We are going to try to make a point from this line.
            // Conventions: 
            //   An entry in a cell group file has form: row, column
            //   Column is the X coordinate.
            //   Row is the Y coordinate.
            //   A cell defined by a Point is constructed as: new Point(column, row)
            // Note: The applicable Point constructor is: Point(int x, int y)
            char[] delim = { ',' };
            try
            {
                string[] split = s.Split(delim);
                if (split.Length >= 2)
                {
                    int x, y;
                    if (int.TryParse(split[0], out y) && int.TryParse(split[1], out x))
                    {
                        cell = new Point(x, y);
                        return true;
                    }
                }
            }
            catch
            { }

            cell = new Point();
            return false;
        }
        public CellGroup GetGroup(string name)
        {
            // Return the matching group.
            foreach (CellGroup group in _cellGroups)
            {
                if (group.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return group;
                }
            }

            // A matching group was not found. Return null.
            return null;
        }
        public override Point[] GetCells()
        {
            // Make a list for the cells.
            List<Point> cells = new List<Point>();

            // Add the cells from all groups to the list.
            foreach (CellGroup group in _cellGroups)
            {
                cells.AddRange(group.GetCells());
            }

            // Return the result as an array.
            return cells.ToArray();
        }
        #endregion Methods

        #region IDeepCloneable Members
        public override object DeepClone()
        {
            CellGroupFile cellGroupFileCopy = new CellGroupFile();
            cellGroupFileCopy._name = this._name;
            int n = this.NumGroups;
            if (n > 0)
            {
                cellGroupFileCopy._cellGroups = new CellGroup[n];
                for (int i = 0; i < n; i++)
                {
                    CellGroup cellGroupCopy = (CellGroup)this._cellGroups[i].DeepClone();
                    cellGroupCopy.AssignParent(cellGroupFileCopy);
                    cellGroupFileCopy._cellGroups[i] = cellGroupCopy;
                }
            }
            return cellGroupFileCopy;
        }
        public override void AssignParent(object parent)
        {
            // No parent required
        }
        #endregion IDeepCloneable Members
    }
}
