using System;

using GeoAPI.Geometries;

using USGS.Puma.FiniteDifference;
using USGS.Puma.NTS.Features;

namespace ScenarioTools.Scene
{
    /// <summary>
    /// A ScenarioFeature with a LineGridder
    /// Not currently being used, so this class could be eliminated
    /// </summary>
    public class LineFeature : ScenarioFeature
    {
        #region Fields
        private LineGridder _lineGridder;
        #endregion Fields

        #region Constructors
        public LineFeature() : base()
        {
            _lineGridder = new LineGridder();
        }
        public LineFeature(PackageType type) : base(type)
        {
            _lineGridder = new LineGridder();
        }
        public LineFeature(PackageType type, IGeometry geometry, IAttributesTable attributesTable)
            : base(type, geometry, attributesTable)
        {
            _lineGridder = new LineGridder();
        }
        public LineFeature(PackageType type, IGeometry geometry, IAttributesTable attributesTable, CellCenteredArealGrid modelGrid)
            : base(type, geometry, attributesTable, modelGrid)
        {
            _lineGridder = new LineGridder();
        }
        public LineFeature(PackageType type, IAttributesTable attributesTable, LineGridder lineGridder)
            : base(type, attributesTable)
        {
            _lineGridder = lineGridder;
        }
        public LineFeature(LineFeature lineFeature)
            : base(lineFeature)
        {
        }
        #endregion Constructors

        #region Properties
        public LineGridder LineGridder
        {
            get
            {
                return _lineGridder;
            }
            set
            {
                _lineGridder = value;
            }
        }
        #endregion Properties

        #region ScenarioFeature methods
        public override void AssignFrom(ITaggable featureSource)
        {
            base.AssignFrom(featureSource);
            if (featureSource is LineFeature)
            {
                LineGridder = new LineGridder(((LineFeature)featureSource).LineGridder.Grid);
            }
        }
        public override object Clone()
        {
            throw new NotImplementedException();
        }
        public override void Draw()
        {
            throw new NotImplementedException();
        }
        #endregion ScenarioFeature methods
    }
}
