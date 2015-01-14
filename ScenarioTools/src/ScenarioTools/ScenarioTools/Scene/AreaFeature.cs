using GeoAPI.Geometries;
using USGS.Puma.FiniteDifference;
using USGS.Puma.NTS.Features;

namespace ScenarioTools.Scene
{
    /// <summary>
    /// A ScenarioFeature with an AreaWeightedPolygonGridder
    /// For the GHB Package, AreaFeature represents an area of the
    ///      model domain enclosed by one GHB polygon
    /// For the RCH Package, AreaFeature represents an area of the
    ///      model domain enclosed by one recharge polygon.
    /// </summary>
    public class AreaFeature : ScenarioFeature
    {
        #region Fields
        private AreaWeightedPolygonGridder _polygonGridder;
        #endregion Fields

        #region Constructors
        public AreaFeature()
            : base()
        {
            _polygonGridder = null;
        }
        public AreaFeature(PackageType type)
            : base(type)
        {
            _polygonGridder = null;
        }
        public AreaFeature(PackageType type, IPolygon polygon, IAttributesTable attributesTable)
            : base(type, polygon, attributesTable)
        {
            _polygonGridder = new AreaWeightedPolygonGridder(polygon);
        }
        public AreaFeature(PackageType type, IPolygon polygon, IAttributesTable attributesTable, 
            CellCenteredArealGrid modelGrid) 
            : base(type, polygon, attributesTable, modelGrid)
        {
            _polygonGridder = new AreaWeightedPolygonGridder(modelGrid, polygon);
        }
        public AreaFeature(PackageType type, IAttributesTable attributesTable, AreaWeightedPolygonGridder polygonGridder)
            : base(type, attributesTable)
        {
            _polygonGridder = polygonGridder;
        }
        public AreaFeature(AreaFeature areaFeature)
            : base(areaFeature)
        {
        }
        #endregion Constructors

        #region Properties
        public AreaWeightedPolygonGridder PolygonGridder
        {
            get
            {
                return _polygonGridder;
            }
            set
            {
                _polygonGridder = value;
            }
        }
        #endregion Properties

        #region ScenarioFeature Methods
        public override void AssignFrom(ITaggable featureSource)
        {
            base.AssignFrom(featureSource);
            if (featureSource is AreaFeature)
            {
                PolygonGridder = new AreaWeightedPolygonGridder(((AreaFeature)featureSource).PolygonGridder);
            }
        }
        public override object Clone()
        {
            AreaFeature areaFeature = new AreaFeature(this);
            return areaFeature;
        }
        public override void Draw()
        {
        }
        #endregion ScenarioFeature Methods
    }
}
