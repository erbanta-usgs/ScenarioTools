using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScenarioTools.Spatial
{
    public class GridInfo
    {
        #region Fields
        private string _shapefileAbsolutePath;
        private SpatialReference _spatialReference;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Assigning ShapefileAbsolutePath generates a SpatialReference from the .prj file
        /// </summary>
        public string ShapefileAbsolutePath 
        {
            get
            {
                return _shapefileAbsolutePath;
            }
            set
            {
                _shapefileAbsolutePath = value;
                generateSpatialReference();
            }
        }
        public SpatialReference SpatialReference
        {
            get
            {
                return _spatialReference;
            }
        }
        #endregion Properties

        #region Constructors
        public GridInfo()
        {
            _shapefileAbsolutePath = "";
            _spatialReference = null;
        }
        public GridInfo(string shapefileAbsolutePath)
            : this()
        {
            _shapefileAbsolutePath = shapefileAbsolutePath;
            generateSpatialReference();
        }
        #endregion Constructors

        #region Methods
        public override string ToString()
        {
            return "Grid shapefile: " + ShapefileAbsolutePath;
        }
        private void generateSpatialReference()
        {
            string prjFile = Path.ChangeExtension(_shapefileAbsolutePath, "prj");
            if (File.Exists(prjFile))
            {
                using (StreamReader sr = File.OpenText(prjFile))
                {
                    string wktString = sr.ReadLine();
                    _spatialReference = new ScenarioTools.Spatial.SpatialReference(wktString, this);
                    GlobalSpatialStaticVariables.SpatialReference = _spatialReference;
                }
            }
            else
            {
                _spatialReference = null;
            }
        }
        #endregion Methods
    }
}
