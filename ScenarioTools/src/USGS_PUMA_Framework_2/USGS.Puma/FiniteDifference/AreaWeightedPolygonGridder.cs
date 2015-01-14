using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GeoAPI.Geometries;
using USGS.Puma.NTS.Geometries;

namespace USGS.Puma.FiniteDifference
{
    public class AreaWeightedPolygonGridder
    {
        #region Constructors
        public AreaWeightedPolygonGridder()
        {
            _grid = null;
            _polygon = null;
        }
        public AreaWeightedPolygonGridder(CellCenteredArealGrid grid)
        {
            _grid = grid;
            _polygon = null;
        }
        public AreaWeightedPolygonGridder(IPolygon polygon)
        {
            _grid = null;
            _polygon = polygon;
        }
        public AreaWeightedPolygonGridder(CellCenteredArealGrid grid, IPolygon polygon)
        {
            _grid = grid;
            Polygon = polygon; // Triggers construction of multiplier matrix
        }
        public AreaWeightedPolygonGridder(AreaWeightedPolygonGridder gridder) 
            : this(gridder._grid, gridder._polygon)
        {
        }
        #endregion Constructors

        #region Fields
        private CellCenteredArealGrid _grid;
        private IPolygon _polygon;
        private double[,] _multiplierMatrix;
        #endregion Fields

        #region Properties
        public CellCenteredArealGrid Grid
        {
            get
            {
                return _grid;
            }
            set
            {
                // Recalculate multiplier matrix only if necessary
                bool needToDefineMatrix = false;
                if (value != null)
                {
                    if (_grid == null)
                    {
                        needToDefineMatrix = true;
                        _grid = value;
                    }
                    else
                    {
                        if (!_grid.Equals(value))
                        {
                            needToDefineMatrix = true;
                            _grid = value;
                        }
                    }
                }
                if (needToDefineMatrix)
                {
                    if (_polygon != null)
                    {
                        PopulateMultiplierMatrix();
                    }
                }
            }
        }
        public IPolygon Polygon
        {
            get
            {
                return _polygon;
            }
            set
            {
                // Recalculate multiplier matrix only if necessary
                bool needToDefineMatrix = false;
                if (value != null)
                {
                    if (_polygon == null)
                    {
                        needToDefineMatrix = true;
                        _polygon = value;
                    }
                    else
                    {
                        if (!_polygon.Equals(value))
                        {
                            needToDefineMatrix = true;
                            _polygon = value;
                        }
                    }
                }
                if (needToDefineMatrix)
                {
                    if (_grid != null)
                    {
                        PopulateMultiplierMatrix();
                    }
                }
            }
        }
        /// <summary>
        /// Return area-weighted multiplier [nRow, nCol] zero-based matrix
        /// </summary>
        public double[,] MultiplierMatrix
        {
            get
            {
                return _multiplierMatrix;
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Calculate area-weighted multiplier matrix, based on model grid and polygon
        /// </summary>
        private void PopulateMultiplierMatrix()
        {
            if (_grid != null && _polygon != null)
            {
                int nCol = _grid.ColumnCount;
                int nRow = _grid.RowCount;
                _multiplierMatrix = new double[nRow, nCol];
                GridCell gridCell = new GridCell();
                IPolygon cellPolygon;
                IGeometry intersectionGeometry;
                for (int i = 0; i < nRow; i++)
                {
                    gridCell.Row = i + 1;
                    for (int j = 0; j < nCol; j++)
                    {
                        gridCell.Column = j + 1;
                        cellPolygon = (IPolygon)_grid.GetPolygon(gridCell);
                        intersectionGeometry = _polygon.Intersection(cellPolygon);
                        if (intersectionGeometry is IPolygon)
                        {
                            _multiplierMatrix[i, j] = ((IPolygon)intersectionGeometry).Area / cellPolygon.Area;
                        }
                    }
                }
            }
            else
            {
                _multiplierMatrix = null;
            }
        }
        #endregion Methods
    }
}
