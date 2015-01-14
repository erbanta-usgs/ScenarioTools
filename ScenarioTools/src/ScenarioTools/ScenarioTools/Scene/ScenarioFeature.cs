using System.Collections.Generic;
using System.Windows.Forms;

using GeoAPI.Geometries;

using ScenarioTools.DataClasses;

using USGS.Puma.FiniteDifference;
using USGS.Puma.NTS.Features;

namespace ScenarioTools.Scene
{
    /// <summary>
    /// A ScenarioFeature represents one model boundary.
    /// For the Well Package, a ScenarioFeature represents one well cell.
    /// For the River Package, a ScenarioFeature represents one river reach.
    /// For the CHD Package, ScenarioFeature represents one CHD cell.
    /// </summary>
    public abstract class ScenarioFeature : Feature, ITaggable
    {
        #region Fields
        // Fields required to implement ITaggable
        protected PackageType _type;
        protected int _tag;
        protected TagLink _link;
        protected string _name;
        protected string _description;
        protected ITaggable _parent;
        //
        protected int _lay;
        protected int _row;
        protected int _col;
        protected SmpSeries _smpSeries;
        protected SmpSeries _secondarySmpSeries;
        protected TimeSeriesInterpretationMethod _interpretationMethod;
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Constructor for abstract class Feature
        /// </summary>
        public ScenarioFeature()
        {
            _type = PackageType.NoType;
            _smpSeries = new SmpSeries();
            _interpretationMethod = TimeSeriesInterpretationMethod.Piecewise;
        }
        /// <summary>
        /// Constructor for abstract class Feature
        /// </summary>
        /// <param name="type"></param>
        public ScenarioFeature(PackageType type)
            : this()
        {
            _type = type;
        }
        public ScenarioFeature(PackageType type, IGeometry geometry, IAttributesTable attributesTable)
            : base(geometry, attributesTable)
        {
            _type = type;
            _smpSeries = new SmpSeries();
            _interpretationMethod = TimeSeriesInterpretationMethod.Piecewise;
        }
        public ScenarioFeature(PackageType type, IGeometry geometry, IAttributesTable attributesTable,
            CellCenteredArealGrid modelGrid) : base(geometry, attributesTable)
        {
            _type = type;
            _smpSeries = new SmpSeries();
            _interpretationMethod = TimeSeriesInterpretationMethod.Piecewise;
        }
        public ScenarioFeature(PackageType type, IAttributesTable attributesTable)
            : base()
        {
            _type = type;
            _smpSeries = new SmpSeries();
            _interpretationMethod = TimeSeriesInterpretationMethod.Piecewise;
            
        }
        public ScenarioFeature(ScenarioFeature scenarioFeature)
            : base(scenarioFeature.Geometry, scenarioFeature.Attributes)
        {
        }
        #endregion Constructors

        #region Properties
        public virtual int Layer
        {
            get
            {
                return _lay;
            }
            set
            {
                _lay = value;
            }
        }
        /// <summary>
        /// 1-based Modflow row index
        /// </summary>
        public virtual int Row
        {
            get
            {
                return _row;
            }
            set
            {
                _row = value;
            }
        }
        /// <summary>
        /// 1-based Modflow column index
        /// </summary>
        public virtual int Column
        {
            get
            {
                return _col;
            }
            set
            {
                _col = value;
            }
        }
        public int Index { get; set; } // index in Package.Active matrix
        public SmpSeries SmpSeries
        {
            get
            {
                return _smpSeries;
            }
            set
            {
                _smpSeries = value;
            }
        }
        public SmpSeries SecondarySmpSeries
        {
            get
            {
                return _secondarySmpSeries;
            }
            set
            {
                _secondarySmpSeries = value;
            }
        }
        public TimeSeriesInterpretationMethod InterpretationMethod
        {
            get
            {
                return _interpretationMethod;
            }
            set
            {
                _interpretationMethod = value;
            }
        }
        public PackageType Type
        {
            get
            {
                return _type;
            }
            protected set
            {
                _type = value;
            }
        }
        #endregion Properties

        #region ITaggable Members
        public virtual void AssignFrom(ITaggable featureSource)
        {
            if (featureSource is ScenarioFeature)
            {
                ScenarioFeature ftr = (ScenarioFeature)featureSource;
                Layer = ftr.Layer;
                Row = ftr.Row;
                Column = ftr.Column;
                Index = ftr.Index;
                SmpSeries.AssignFrom(ftr.SmpSeries);
                InterpretationMethod = ftr.InterpretationMethod;
                Type = ftr.Type;
                Tag = ftr.Tag;
                Link.AssignFrom(ftr.Link);
                Name = ftr.Name;
                Description = ftr.Description;
                Parent = ftr.Parent;
                SmpSeries.AssignFrom(ftr.SmpSeries);
                _interpretationMethod = ftr._interpretationMethod;
            }
        }
        public int GetNewTag()
        {
            _tag = _link.Owner.GetNextTag();
            return _tag;
        }
        public int Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }
        public virtual string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public TagLink Link
        {
            get
            {
                return _link;
            }
            set
            {
                _link = value;
            }
        }
        public ElementType ElemType
        {
            get
            {
                return ElementType.Feature;
            }
        }
        public List<ITaggable> Items { get; set; }
        public ITaggable Parent
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
        public void ReTag()
        {
            _link.Tag = _link.Owner.GetNextTag();
        }
        public void ConnectList(List<ITaggable> owner)
        {
            TagUtilities.ConnectList(this, owner);
        }
        public void ConvertRelativePaths(string oldDirectoryPath, string newDirectoryPath)
        {
        }
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }
        public abstract object Clone();
        public void SelectTreeNode(TreeView treeView)
        {
            // A ScenarioFeature is not represented by a TreeNode, so do nothing
        }
        #endregion ITaggable Members

        #region Other Methods
        public abstract void Draw();
        #endregion Other Methods
    }
}
