using System;
using System.Drawing;

using GeoAPI.Geometries;

using USGS.ModflowTrainingTools;
using USGS.Puma.UI.MapViewer;

namespace ScenarioTools.Reporting
{
    public class STMapLayer : IFeatureLayer
    {
        #region Static Fields
        protected const LayerGeometryType DEFAULT_GEOMETRY_TYPE = LayerGeometryType.Line;
        #endregion Static Fields

        #region Fields
        protected DataSeries _dataSeries;
        protected ContourEngineData _contourEngineData;
        protected FeatureLayer _featureLayer;
        protected ColorRamp _randomRamp = null;
        protected Color _contourColor = Color.Black;
        protected float _randomPosition;
        #endregion Fields

        #region Constructors
        public STMapLayer()
        {
            _featureLayer = new FeatureLayer(DEFAULT_GEOMETRY_TYPE);
            _contourEngineData = new ContourEngineData();
            _dataSeries = null;
        }
        public STMapLayer(LayerGeometryType geometryType)
        {
            _featureLayer = new FeatureLayer(geometryType);
            _contourEngineData = new ContourEngineData();
            _dataSeries = null;
        }
        public STMapLayer(LayerGeometryType geometryType, DataSeries dataSeries, ContourEngineData contourEngineData) : this(geometryType)
        {
            _dataSeries = dataSeries;
            _contourEngineData = contourEngineData;
        }
        public STMapLayer(DataSeries dataSeries) : this()
        {
            _dataSeries = dataSeries;
        }
        #endregion Constructors

        #region Properties
        public DataSeries DataSeries
        {
            get
            {
                return _dataSeries;
            }
            set
            {
                _dataSeries = value;
                _contourEngineData.ContourColor = _dataSeries.LineSeriesColor;
            }
        }
        public ContourEngineData ContourEngineData
        {
            get
            {
                return _contourEngineData;
            }
            set
            {
                _contourEngineData = value;
            }
        }
        public FeatureLayer FeatureLayer
        {
            get
            {
                return _featureLayer;
            }
            set
            {
                _featureLayer = value;
            }
        }
        public LayerGeometryType LayerGeometryType
        {
            get
            {
                return GeometryType;
            }
            set
            {
                switch (value)
                {
                    case LayerGeometryType.Mixed:
                        throw new NotImplementedException("Mixed geometry");
                    case LayerGeometryType.Point:
                        Renderer = new SingleSymbolRenderer(new SimplePointSymbol(PointSymbolTypes.Circle, _randomRamp.GetColor(_randomPosition), 1.0f));
                        break;
                    case LayerGeometryType.Line:
                        Renderer = new SingleSymbolRenderer(new LineSymbol(_randomRamp.GetColor(_randomPosition), System.Drawing.Drawing2D.DashStyle.Solid, 1.0f));
                        break;
                    case LayerGeometryType.Polygon:
                        Renderer = new SingleSymbolRenderer(new SolidFillSymbol(_randomRamp.GetColor(_randomPosition)));
                        break;
                    default:
                        break;
                }
            }
        }
        public Color ContourColor 
        {
            get
            {
                return _contourColor;
            }
            set
            {
                _contourColor = value;
                if (_dataSeries != null)
                {
                    _dataSeries.LineSeriesColor = value;
                }
                _contourEngineData.ContourColor = value;
            }
        }
        public ColorRamp ColorRamp
        {
            get
            {
                Color[] colors = new Color[] { DataSeries.RampColor0, DataSeries.RampColor1 };
                ColorRamp colorRamp = new ColorRamp(colors);
                return colorRamp;
            }
        }
        #endregion Properties

        #region IFeatureLayer members
        public LayerGeometryType GeometryType 
        {
            get
            {
                return FeatureLayer.GeometryType;
            }
        }
        public IFeatureRenderer Renderer 
        {
            get
            {
                return FeatureLayer.Renderer;
            }
            set
            {
                FeatureLayer.Renderer = value;
            }
        }
        public int FeatureCount 
        {
            get
            {
                return FeatureLayer.FeatureCount;
            }
        }
        public void AddFeature(GeoAPI.Geometries.IGeometry geometry, USGS.Puma.NTS.Features.IAttributesTable attributes)
        {
            FeatureLayer.AddFeature(geometry, attributes);
        }
        public void AddFeature(USGS.Puma.NTS.Features.Feature feature)
        {
            FeatureLayer.AddFeature(feature);
        }
        public void RemoveFeature(int index)
        {
            FeatureLayer.RemoveFeature(index);
        }
        public void RemoveAll()
        {
            FeatureLayer.RemoveAll();
        }
        public void MoveUp(int fromIndex)
        {
            FeatureLayer.MoveUp(fromIndex);
        }
        public void MoveDown(int fromIndex)
        {
            FeatureLayer.MoveDown(fromIndex);
        }
        public void MoveToTop(int fromIndex)
        {
            FeatureLayer.MoveToTop(fromIndex);
        }
        public void ModeToBottom(int fromIndex)
        {
            FeatureLayer.ModeToBottom(fromIndex);
        }
        public USGS.Puma.NTS.Features.Feature GetFeature(int index)
        {
            return FeatureLayer.GetFeature(index);
        }
        public USGS.Puma.NTS.Features.FeatureCollection GetFeatures()
        {
            return FeatureLayer.GetFeatures();
        }
        public void UpdateFeature(USGS.Puma.NTS.Features.Feature feature, int index)
        {
            FeatureLayer.UpdateFeature(feature, index);
        }
        public void Update()
        {
            FeatureLayer.Update();
        }
        #endregion  IFeatureLayer members

        #region IGraphicLayer members
        public GraphicLayerType LayerType 
        {
            get
            {
                if (FeatureLayer != null)
                {
                    return FeatureLayer.LayerType;
                }
                else
                {
                    return GraphicLayerType.ImageLayer;
                }
            }
        }
        public double MinVisible 
        {
            get
            {
                if (FeatureLayer != null)
                {
                    return FeatureLayer.MinVisible;
                }
                else
                {
                    return double.NaN;
                }
            }
            set
            {
                if (FeatureLayer != null)
                {
                    FeatureLayer.MinVisible = value;
                }
            }
        }
        public double MaxVisible 
        { 
            get
            {
                if (FeatureLayer != null)
                {
                    return FeatureLayer.MaxVisible;
                }
                else
                {
                    return double.NaN;
                }
            }
            set
            {
                if (FeatureLayer != null)
                {
                    FeatureLayer.MaxVisible = value;
                    }
            }
        }
        public bool Visible 
        {
            get
            {
                if (FeatureLayer != null)
                {
                    return FeatureLayer.Visible;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (FeatureLayer != null)
                {
                    FeatureLayer.Visible = value;
                }
            }
        }
        public bool ConvertFlowToFlux { get; set; }
        public string LayerName 
        {
            get
            {
                if (FeatureLayer != null)
                {
                    return FeatureLayer.LayerName;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                if (FeatureLayer != null)
                {
                    FeatureLayer.LayerName = value;
                }
            }
        }
        public IEnvelope Extent 
        {
            get
            {
                return FeatureLayer.Extent;
            }
        }
        public int SRID 
        {
            get
            {
                return FeatureLayer.SRID;
            }
            set
            {
                FeatureLayer.SRID = value;
            }
        }
        #endregion IGraphicLayer members

        public override string ToString()
        {
            return this.LayerName;
        }
    }
}
