using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;

using ScenarioTools.Util;
using ScenarioTools.Xml;

namespace ScenarioTools.DataClasses
{
    public class CellGroup : CellGroupProvider
    {
        #region Fields
        private CellGroupFile _parent;
        private string _name;
        private Point[] _cells;
        #endregion Fields

        #region Constructors
        private CellGroup()
        {
            this._parent = null;
            this._name = "";
            this._cells = null;
        }
        public CellGroup(string name, Point[] cells) : this()
        {
            this._name = name;
            this._cells = cells;
        }
        #endregion Constructors

        #region Properties
        public CellGroupFile Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
        }
        #endregion Properties

        #region Methods
        public override XmlNode GetXmlNode(XmlDocument document, string targetFileName)
        {
            // Make the element that represents this object.
            XmlElement element = document.CreateElement("Group");

            // Set the attributes.
            string dir = Path.GetDirectoryName(targetFileName);
            string relPath = FileUtil.Absolute2Relative(_parent.Name, dir);
            XmlUtil.FrugalSetAttribute(element, XML_KEY_FILE, relPath, "");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_IDENTIFIER, _name, "");

            // Return the result.
            return element;
        }
        public override bool Equals(object obj)
        {
            if (obj is CellGroup)
            {
                CellGroup compare = (CellGroup)obj;
                return compare.Parent.Equals(this.Parent) && compare.Name.Equals(this.Name);
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return this.Parent.GetHashCode() ^ this.Name.GetHashCode();
        }
        public override Point[] GetCells()
        {
            // Return the cells array.
            return _cells;
        }
        #endregion Methods

        #region IDeepCloneable Members
        public override object DeepClone()
        {
            CellGroup cellGroupCopy = new CellGroup();
            cellGroupCopy._name = this._name;
            if (_cells != null)
            {
                int n = _cells.GetLength(0);
                cellGroupCopy._cells = new Point[n];
                for (int i = 0; i < n; i++)
                {
                    cellGroupCopy._cells[i] = new Point(_cells[i].X, _cells[i].Y);
                }
            }
            return cellGroupCopy;
        }
        public override void AssignParent(object parent)
        {
            if (parent is CellGroupFile)
            {
                this._parent = (CellGroupFile)parent;
            }
        }
        #endregion IDeepCloneable Members
    }
}
