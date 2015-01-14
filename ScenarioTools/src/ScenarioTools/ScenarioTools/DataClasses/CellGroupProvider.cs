using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Drawing;

using ScenarioTools.Util;
using ScenarioTools.Xml;

namespace ScenarioTools.DataClasses
{
    public abstract class CellGroupProvider : IDeepCloneable
    {
        protected const string XML_KEY_FILE = "file";
        protected const string XML_KEY_IDENTIFIER = "name";

        public abstract XmlNode GetXmlNode(XmlDocument document, string targetFileName);

        public static CellGroupProvider FromXmlElement(XmlElement element)
        {
            // Get the name of the element. Only continue if it equals "Group."
            if (element.Name.Equals("Group", StringComparison.OrdinalIgnoreCase))
            {
                // Get the file attribute. Only continue if it is non-empty.
                string file = XmlUtil.SafeGetStringAttribute(element, "file", "");
                if (!file.Equals(""))
                {
                    // Get the name. If it is empty, this is a cell group file. Otherwise, it is a cell group.
                    string name = XmlUtil.SafeGetStringAttribute(element, "name", "");
                    if (name.Equals(""))
                    {
                        return CellGroupFile.FromFile(file);
                    }
                    else
                    {
                        CellGroupFile parentFile = CellGroupFile.FromFile(file);
                        CellGroup group = parentFile.GetGroup(name);
                        return group;
                    }
                }
            }
            return null;
        }
        public abstract Point[] GetCells();

        public static Point[] GetCombinedCells(CellGroupProvider[] cellGroupProviders)
        {
            // Make a list for the cells.
            List<Point> combinedCells = new List<Point>();

            // Add the cells from all providers in the array.
            foreach (CellGroupProvider provider in cellGroupProviders)
            {
                combinedCells.AddRange(provider.GetCells());
            }

            // Ned TODO: if cellGroupProviders is empty, get an array of all cell locations in domain
            // Convention: a Point's (X,Y) coordinates are specified as (column, row).
            if (cellGroupProviders.GetLength(0) == 0)
            {
                if (ScenarioTools.Spatial.StaticObjects.Grid != null)
                {
                    USGS.Puma.FiniteDifference.CellCenteredArealGrid grid = ScenarioTools.Spatial.StaticObjects.Grid;
                    int nRows = grid.RowCount;
                    int nCols = grid.ColumnCount;
                    // Add Points in MODFLOW's cell order (across all columns in row 1, then across all columns in row 2, ...)
                    for (int i = 0; i < nRows; i++)
                    {
                        for (int j = 0; j < nCols; j++)
                        {
                            Point point = new Point(j + 1, i + 1);
                            combinedCells.Add(point);
                        }
                    }
                }
            }

            // Return the result as an array.
            return combinedCells.ToArray();
        }

        public static Point[] InFileOrder(Point[] cells)
        {
            // Determine the maximum column value in the list of cells.
            int maxColumn = 1;
            foreach (Point cell in cells) 
            {
                maxColumn = Math.Max(maxColumn, cell.X);
            }

            // Copy and sort the cell array.
            int[] cellKeys = new int[cells.Length];
            for (int i = 0; i < cells.Length; i++)
            {
                cellKeys[i] = cells[i].Y * maxColumn + cells[i].X;
            }
            Point[] orderedCells = (Point[])cells.Clone();
            Array.Sort(cellKeys, orderedCells);

            // Return the result.
            return orderedCells;
        }

        public static bool ArrayEquals(CellGroupProvider[] group1, CellGroupProvider[] group2)
        {
            // This method assumes that neither array is null. If either is, throw an argument exception.
            if (group1 == null || group2 == null)
            {
                throw new ArgumentException("Neither group array may be null.");
            }

            // Make a list of the second array.
            List<CellGroupProvider> group2List = new List<CellGroupProvider>(group2);

            // If the length is different, return false.
            if (group1.Length != group2.Length)
            {
                return false;
            }

            // If any element of the first array is not contained in the second array, return false.
            foreach (CellGroupProvider group in group1)
            {
                if (!group2List.Contains(group))
                {
                    return false;
                }
            }

            // All conditions have been satisfied, so return true.
            return true;
        }

        #region IDeepCloneable Members
        public abstract object DeepClone();
        public abstract void AssignParent(object parent);
        #endregion IDeepCloneable Members
    }
}
