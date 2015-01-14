using System.Collections.Generic;

using ScenarioTools.DataClasses;
using ScenarioTools.Util;

namespace ScenarioTools.Scene
{
    public class SerializableScenarioElement
    {
        #region Fields
        private int _parentTag;
        private List<SerializableScenarioElement> _items = new List<SerializableScenarioElement>();
        private PackageType _type = PackageType.NoType;
        private List<GeoValue> _geoValueList = new List<GeoValue>();
        #endregion Fields

        #region Constructors

        public SerializableScenarioElement()
        {
            Tag = -1;
            CbcFlag = 0;
            ParentTag = -1;
            ElemType = ElementType.NoType;
            Description = "";
            Name = "";
            ShapefileRelativePath = "";
            TimeSeriesRelativePath = "";
            TimeSeriesSecondaryRelativePath = "";
            KeyField = "";
            LayerAttribute = "";
            PackageOption = 1;
            TopElevationAttribute = "";
            BottomElevationAttribute = "";
            LabelFeatures = FeatureLabel.None;
        }

        public SerializableScenarioElement(SerializableScenarioElement element)
        {
            Tag = element.Tag;
            CbcFlag = element.CbcFlag;
            ParentTag = element.ParentTag;
            ElemType = element.ElemType;
            Description = element.Description;
            Name = element.Name;
            ShapefileRelativePath = element.ShapefileRelativePath;
            TimeSeriesRelativePath = element.TimeSeriesRelativePath;
            TimeSeriesSecondaryRelativePath = element.TimeSeriesSecondaryRelativePath;
            KeyField = element.KeyField;
            LayerAttribute = element.LayerAttribute;
            PackageOption = element.PackageOption;
            TopElevationAttribute = element.TopElevationAttribute;
            BottomElevationAttribute = element.BottomElevationAttribute;
            LabelFeatures = element.LabelFeatures;
            _type = element.Type;
            foreach (SerializableScenarioElement e in element.Items)
            {
                _items.Add(e);
            }
            foreach (GeoValue geoValue in element._geoValueList)
            {
                _geoValueList.Add(geoValue);
            }
        }

        public SerializableScenarioElement(ITaggable tagElement, string projectFileDirectory) : this()
        {
            Tag = tagElement.Tag;
            if (tagElement.Parent != null)
            {
                ParentTag = tagElement.Parent.Tag;
            }
            ElemType = tagElement.ElemType;
            Description = tagElement.Description;
            Name = tagElement.Name;
            if (tagElement.Items != null)
            {
                foreach (ITaggable item in tagElement.Items)
                {
                    // Project file probably does not need
                    // to include individual features
                    if (item.ElemType != ElementType.Feature)
                    {
                        _items.Add(new SerializableScenarioElement(item, projectFileDirectory));
                    }
                }
            }

            // Assign type-specific properties
            if (tagElement is Scenario)
            {
            }
            else if (tagElement is Package)
            {
                this.Type = ((Package)tagElement).Type;
                this.CbcFlag = ((Package)tagElement).CbcFlag;
            }
            else if (tagElement is FeatureSet)
            {
                this.ShapefileRelativePath = FileUtil.Absolute2Relative(((FeatureSet)tagElement).ShapefileAbsolutePath, projectFileDirectory);
                this.TimeSeriesRelativePath = FileUtil.Absolute2Relative(((FeatureSet)tagElement).TimeSeriesAbsolutePath, projectFileDirectory);
                this.TimeSeriesSecondaryRelativePath = FileUtil.Absolute2Relative(((FeatureSet)tagElement).TimeSeriesSecondaryAbsolutePath, projectFileDirectory);
                this.KeyField = ((FeatureSet)tagElement).KeyField;
                this.DefaultLayer = ((FeatureSet)tagElement).DefaultLayer;
                this.LayerAttribute = ((FeatureSet)tagElement).LayerAttribute;
                foreach (GeoValue geoValue in ((FeatureSet)tagElement).GeoValueList)
                {
                    this._geoValueList.Add(new GeoValue(geoValue));
                }
                this.PackageOption = ((FeatureSet)tagElement).PackageOption;
                this.CbcFlag = ((FeatureSet)tagElement).CbcFlag;
                this.TopElevationAttribute = ((FeatureSet)tagElement).TopElevationAttribute;
                this.BottomElevationAttribute = ((FeatureSet)tagElement).BottomElevationAttribute;
                this.LayMethod = ((FeatureSet)tagElement).LayMethod;
                this.InterpretationMethod = ((FeatureSet)tagElement).InterpretationMethod;
                this.Type = ((FeatureSet)tagElement).Type;
                this.LabelFeatures = ((FeatureSet)tagElement).LabelFeatures;
            }
            else if (tagElement is ScenarioFeature)
            {
                this.Type = ((ScenarioFeature)tagElement).Type;
            }
        }

        #endregion Constructors

        #region Properties
        public int DefaultLayer { get; set; }
        public int Tag { get; set; }
        public int PackageOption { get; set; }
        public int CbcFlag { get; set; }
        public int ParentTag
        {
            get
            {
                return _parentTag;
            }
            set
            {
                _parentTag = value;
            }
        }
        public string BottomElevationAttribute { get; set; }
        public string Description { get; set; }
        public string KeyField { get; set; }
        public string LayerAttribute { get; set; }
        public string Name { get; set; }
        public string ShapefileRelativePath { get; set; }
        public string TimeSeriesRelativePath { get; set; }
        public string TimeSeriesSecondaryRelativePath { get; set; }
        public string TopElevationAttribute { get; set; }
        public ElementType ElemType { get; set; }
        public FeatureLabel LabelFeatures { get; set; }
        public LayerMethod LayMethod { get; set; }
        public PackageType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }
        public TimeSeriesInterpretationMethod InterpretationMethod { get; set; }
        public List<SerializableScenarioElement> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items.Clear();
                foreach (SerializableScenarioElement element in value)
                {
                    _items.Add(new SerializableScenarioElement(element));
                }
            }
        }
        public List<GeoValue> GeoValueList 
        {
            get
            {
                return _geoValueList;
            }
        }
        #endregion Properties
    }
}
