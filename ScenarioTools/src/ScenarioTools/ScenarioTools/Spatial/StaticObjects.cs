using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using USGS.Puma.FiniteDifference;

namespace ScenarioTools.Spatial
{
    public static class StaticObjects
    {
        #region Static fields
        private static string _gridShapefilePath = "";
        private static CellCenteredArealGrid _grid = null;
        #endregion Static fields

        #region Static Properties
        public static string GridShapefilePath 
        {
            get
            {
                return _gridShapefilePath;
            }
            set
            {
                Status = "";
                if (File.Exists(value))
                {
                    // If pathname has changed or grid is null, (re)create grid
                    if (value != _gridShapefilePath || _grid == null)
                    {
                        Status = "Importing model grid shapefile...";
                        _grid = CreateCellCenteredArealGrid.CreateGrid(value, ModelGridInfo);
                        _gridShapefilePath = value;
                        if (_grid != null)
                        {
                            Status = "Model grid shapefile successfully imported";
                        }
                        else
                        {
                            Status = "Import of model grid shapefile \"" + value + "\" failed";
                        }
                    }
                }
                else
                {
                    _grid = null;
                    ModelGridInfo.ShapefileAbsolutePath = "";
                    _gridShapefilePath = value;
                    //Status = "Warning: Model grid shapefile \"" + value + "\" does not exist";
                }
            }
        }
        public static CellCenteredArealGrid Grid
        {
            get
            {
                return _grid;
            }
            private set
            {
                _grid = value;
            }
        }
        public static GridInfo ModelGridInfo { get; set; }
        public static string Status { get; set; }
        #endregion Static Properties

        #region Static methods

        public static void Initialize()
        {
            if (ModelGridInfo == null)
            {
                ModelGridInfo = new GridInfo();
            }
            GridShapefilePath = "";
        }
        #endregion Static methods
    }
}
