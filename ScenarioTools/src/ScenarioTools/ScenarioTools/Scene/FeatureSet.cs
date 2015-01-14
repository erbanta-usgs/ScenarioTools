using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HPdf;
using ScenarioTools.DataClasses;
using ScenarioTools.PdfWriting;
using ScenarioTools.Spatial;

namespace ScenarioTools.Scene
{
    public enum FeatureLabel
    {
        None = 0,
        FeatureSetOnly = 1,
        FeatureNameOnly = 2,
        FeatureSetAndName = 3
    }
    
    public class FeatureSet : ITaggable
    {
        #region Fields

        // Fields required to implement ITaggable
        protected int _number;
        protected int _tag;
        protected string _description;
        protected string _name;
        protected PackageType _type;
        protected List<ITaggable> _features;
        protected TagLink _link;
        protected ITaggable _parent;

        // Other fields
        protected string _baseDirectory;
        protected int _defaultLayer;
        // _keyField is name of shapefile field that links to time series
        protected string _keyField;
        protected string _shapefileAbsolutePath;
        protected string _timeSeriesAbsolutePath;
        protected string _timeSeriesSecondaryAbsolutePath;
        protected bool _needRowAndCol;
        protected LayerMethod _layerMethod;
        private TimeSeriesInterpretationMethod _interpretationMethod;
        private ScenarioTools.Spatial.SpatialReference _spatialReference;

        // Field(s) that are not saved
        protected SmpSeries _smpSeries;
        protected SmpSeries _secondarySmpSeries;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Default constructor for class FeatureSet
        /// </summary>
        private FeatureSet()
        {
            _type = PackageType.NoType;
            _tag = 0;
            _description = "";
            _name = "";
            _shapefileAbsolutePath = "";
            _timeSeriesAbsolutePath = "";
            _timeSeriesSecondaryAbsolutePath = "";
            _keyField = "";
            _layerMethod = LayerMethod.Uniform;
            _interpretationMethod = TimeSeriesInterpretationMethod.Piecewise;
            _defaultLayer = 1;
            _features = new List<ITaggable>();
            _link = new TagLink();
            _parent = null;
            _number = -1;
            _needRowAndCol = true;
            _spatialReference = null;
            LayerAttribute = "";
            GeoValueList = new List<GeoValue>();
            PackageOption = 1;
            CbcFlag = 0;
            TopElevationAttribute = "";
            BottomElevationAttribute = "";
            LabelFeatures = FeatureLabel.None;
        }

        /// <summary>
        /// Constructor for class FeatureSet
        /// </summary>
        /// <param name="type"></param>
        public FeatureSet(PackageType type, ITaggable parent) : this()
        {
            _type = type;
            _parent = parent;
            if (parent is Package)
            {
                ((Package)parent).InitializeGeoValueList(GeoValueList);
            }
        }

        #endregion Constructors

        #region Properties

        public int Count
        {
            get
            {
                return _features.Count;
            }
        }
        public int DefaultLayer
        {
            get
            {
                return _defaultLayer;
            }
            set
            {
                _defaultLayer = value;
            }
        }
        // Ned TODO: both Number and _number can probably be eliminated
        public int Number
        {
            get
            {
                return _number;
            }
            protected set
            {
                _number = value;
            }
        }
        /// <summary>
        /// MODFLOW-package option, e.g. NRCHOP or NEVTOP
        /// </summary>
        public int PackageOption { get; set; }
        public int CbcFlag { get; set; }
        public string BottomElevationAttribute { get; set; }
        public string KeyField
        {
            get
            {
                return _keyField;
            }
            set
            {
                _keyField = value;
            }
        }
        public string LayerAttribute { get; set; }
        public List<GeoValue> GeoValueList { get; set; }
        public string ShapefileAbsolutePath
        {
            get
            {
                return _shapefileAbsolutePath;
            }
            set
            {
                _shapefileAbsolutePath = value;
            }
        }
        public ScenarioTools.Spatial.SpatialReference SpatialReference
        {
            get
            {
                return _spatialReference;
            }
            set
            {
                _spatialReference = value;
            }
        }
        public string TimeSeriesAbsolutePath
        {
            get
            {
                return _timeSeriesAbsolutePath;
            }
            set
            {
                _timeSeriesAbsolutePath = value;
            }
        }
        public string TimeSeriesSecondaryAbsolutePath
        {
            get
            {
                return _timeSeriesSecondaryAbsolutePath;
            }
            set
            {
                _timeSeriesSecondaryAbsolutePath = value;
            }
        }
        public string TopElevationAttribute { get; set; }
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
        public FeatureLabel LabelFeatures { get; set; }
        public LayerMethod LayMethod
        {
            get
            {
                return _layerMethod;
            }
            set
            {
                _layerMethod = value;
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

        #endregion Properties

        #region ITaggable members

        public void AssignFrom(ITaggable fsSource)
        {
            if (fsSource is FeatureSet)
            {
                FeatureSet fs = (FeatureSet)fsSource;
                Name = fs.Name;
                Type = fs.Type;
                ShapefileAbsolutePath = fs.ShapefileAbsolutePath;
                TimeSeriesAbsolutePath = fs.TimeSeriesAbsolutePath;
                TimeSeriesSecondaryAbsolutePath = fs.TimeSeriesSecondaryAbsolutePath;
                KeyField = fs.KeyField;
                LayerAttribute = fs.LayerAttribute;
                GeoValueList.Clear();
                for (int i = 0; i < fs.GeoValueList.Count; i++)
                {
                    GeoValueList.Add(fs.GeoValueList[i]);
                }
                PackageOption = fs.PackageOption;
                CbcFlag = fs.CbcFlag;
                TopElevationAttribute = fs.TopElevationAttribute;
                BottomElevationAttribute = fs.BottomElevationAttribute;
                LabelFeatures = fs.LabelFeatures;
                DefaultLayer = fs.DefaultLayer;
                LayMethod = fs.LayMethod;
                InterpretationMethod = fs.InterpretationMethod;
                if (fs.SmpSeries != null)
                {
                    SmpSeries.AssignFrom(fs.SmpSeries);
                }
                // Following code added 7/27/12 -- is it OK?
                Number = -1;
                _features.Clear();
                for (int i = 0; i < fs.Count; i++)
                {
                    _features.Add(fs.Items[i]);
                    Number = _features.Count;
                }
            }
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

        public ElementType ElemType
        {
            get
            {
                return ElementType.FeatureSet;
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

        public virtual List<ITaggable> Items
        {
            get
            {
                return _features;
            }
            set
            {
                _features = value;
            }
        }

        public string Name
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

        public int GetNewTag()
        {
            _tag = _link.Owner.GetNextTag();
            return _tag;
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

        public virtual object Clone()
        {
            FeatureSet featureSet = new FeatureSet();
            featureSet.Tag = this._tag;
            featureSet.Link = (TagLink)this._link.Clone();
            featureSet.Link.ScenarioElement = featureSet;
            featureSet.Link.TreeNode = this.Link.TreeNode;
            featureSet.Name = this._name;
            featureSet.Type = this.Type;
            featureSet.Description = this._description;
            featureSet._shapefileAbsolutePath = this._shapefileAbsolutePath;
            if (this._spatialReference != null)
            {
                featureSet._spatialReference = new SpatialReference(this.SpatialReference.GetWktString(),featureSet);
            }
            else
            {
                featureSet._spatialReference = null;
            }
            featureSet._timeSeriesAbsolutePath = this._timeSeriesAbsolutePath;
            featureSet._timeSeriesSecondaryAbsolutePath = this._timeSeriesSecondaryAbsolutePath;
            featureSet._keyField = this._keyField;
            featureSet._defaultLayer = this._defaultLayer;
            featureSet.LayerAttribute = this.LayerAttribute;
            featureSet.GeoValueList.Clear();
            for (int i = 0; i < GeoValueList.Count; i++)
            {
                featureSet.GeoValueList.Add(this.GeoValueList[i]);
            }
            featureSet.PackageOption = this.PackageOption;
            featureSet.CbcFlag = this.CbcFlag;
            featureSet.TopElevationAttribute = this.TopElevationAttribute;
            featureSet.BottomElevationAttribute = this.BottomElevationAttribute;
            featureSet.LabelFeatures = this.LabelFeatures;
            featureSet._layerMethod = this._layerMethod;
            featureSet._interpretationMethod = this._interpretationMethod;
            featureSet._needRowAndCol = this._needRowAndCol;
            featureSet.Number = -1;
            featureSet.Parent = this.Parent;
            foreach (ScenarioFeature feature in this._features)
            {
                ScenarioFeature newScenarioFeature = (ScenarioFeature)feature.Clone();
                featureSet.Add(newScenarioFeature);
                featureSet.Number = featureSet.Count;
            }
            if (featureSet._smpSeries != null)
            {
                featureSet._smpSeries.Records.Clear();
                for (int i = 0; i < this._smpSeries.Records.Count; i++)
                {
                    featureSet._smpSeries.Records.Add(this._smpSeries.Records[i]);
                }
            }
            if (featureSet._secondarySmpSeries != null)
            {
                featureSet._secondarySmpSeries.Records.Clear();
                for (int i = 0; i < this._secondarySmpSeries.Records.Count; i++)
                {
                    featureSet._secondarySmpSeries.Records.Add(this._secondarySmpSeries.Records[i]);
                }
            }
            return featureSet;
        }

        public void SelectTreeNode(TreeView treeView)
        {
            int nodeTag = -2;
            if (treeView != null)
            {
                for (int i = 0; i < treeView.Nodes.Count; i++)
                {
                    for (int j = 0; j < treeView.Nodes[i].Nodes.Count; j++)
                    {
                        for (int k = 0; k < treeView.Nodes[i].Nodes[j].Nodes.Count; k++)
                        {
                            nodeTag = (int)treeView.Nodes[i].Nodes[j].Nodes[k].Tag;
                            if (nodeTag == this.Tag)
                            {
                                treeView.SelectedNode = treeView.Nodes[i].Nodes[j].Nodes[k];
                                return;
                            }
                        }
                    }
                }
            }
        }

        #endregion ITaggable members

        #region Methods

        public void Add(ScenarioFeature feature)
        {
            _features.Add(feature);
            Number = _features.Count;
        }

        /// <summary>
        /// Add features that match specified name to List of features
        /// </summary>
        /// <param name="features">List of features to which matching features are to be added</param>
        /// <param name="name">Name to be matched</param>
        public void AddFeaturesByName(List<ScenarioFeature> features, string name)
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (((ScenarioFeature)Items[i]).Name == name)
                {
                    features.Add((ScenarioFeature)Items[i]);
                    Number = _features.Count;
                }
            }
            return;
        }

        public void Clear()
        {
            _features.Clear();
            Number = 0;
            if (_smpSeries != null)
            {
                if (_smpSeries.Records != null)
                {
                    _smpSeries.Records.Clear();
                }
            }
            if (_secondarySmpSeries != null)
            {
                if (_secondarySmpSeries.Records != null)
                {
                    _secondarySmpSeries.Records.Clear();
                }
            }
        }

        /// <summary>
        /// Return description of FeatureSet
        /// </summary>
        /// <returns></returns>
        public List<string> Describe()
        {
            List<string> descriptionList = new List<string>();
            descriptionList.Add("Feature set name: " + _name);
            descriptionList.Add("Feature set type: " + _type.ToString());
            descriptionList.Add("Description: " + _description);
            descriptionList.Add("Shapefile: " + _shapefileAbsolutePath);
            if (Number < 0)
            {
                descriptionList.Add("Number of features: Unknown (Export to define number of features)");
            }
            else
            {
                descriptionList.Add("Number of features: " + _features.Count.ToString());
            }
            descriptionList.Add("Data-series file: " + _timeSeriesAbsolutePath);
            if (_timeSeriesSecondaryAbsolutePath != "")
            {
                descriptionList.Add("Secondary data-series file: " + _timeSeriesSecondaryAbsolutePath);
            }
            string[] interpretationMethods = new string[2];
            interpretationMethods[0] = "Piecewise";
            interpretationMethods[1] = "Stepwise";
            descriptionList.Add("Method used for interpreting time-series data: " + interpretationMethods[(int)InterpretationMethod]);
            descriptionList.Add("Shapefile field linking features to time-series data: " + _keyField);
            descriptionList.Add("Method used for assigning model layer to features: " + _layerMethod.ToString());

            switch (_layerMethod)
            {
                case LayerMethod.ByAttribute:
                    descriptionList.Add("Attribute containing layer number: " + LayerAttribute);
                    break;
                case LayerMethod.ByCellTops:
                    descriptionList.Add("Attribute containing feature top elevation: " + TopElevationAttribute);
                    descriptionList.Add("Attribute containing feature bottom elevation: " + BottomElevationAttribute);
                    break;
                case LayerMethod.Uniform:
                    descriptionList.Add("Model layer for all features: Layer " + _defaultLayer.ToString());
                    break;
            }

            if (GeoValueList.Count > 0)
            {
                for (int i = 0; i < GeoValueList.Count; i++)
                {
                    GeoValue geoValue = GeoValueList[i];
                    switch (geoValue.GeoValueType)
                    {
                        case GeoValueType.Uniform:
                            descriptionList.Add("Uniform " + geoValue.Descriptor + ": " + geoValue.UniformValue.ToString());
                            break;
                        case GeoValueType.Attribute:
                            if (geoValue.Attribute != "")
                            {
                                descriptionList.Add("Attribute used for " + geoValue.Descriptor + ": " + geoValue.Attribute);
                            }
                            else
                            {
                                descriptionList.Add("Attribute for " + geoValue.Descriptor + " has not been defined");
                            }
                            break;
                    }
                }
            }
            // Ned TODO: Revise code above to use GeoValueList, and add code to handle other packages'
            // use of GeoValueList -- use Descriptor property.  Make it non-package-specific.
            
            return descriptionList;
        }

        /// <summary>
        /// Add FeatureSet description to a PDF
        /// </summary>
        /// <param name="pdf"></param>
        public void DescribeToPdf(HPdfDoc pdf, ref HPdfPoint currentTextPos,
                                          HPdfFont font, HPdfFont fontBold, 
                                          float fontHeight, float indent)
        {
            // Write FeatureSet description to the PDF
            float leadingFactor = 0.5f;
            float indentLocal = 2 * indent;
            float paragraphSpacing = fontHeight;
            float headingSpacing = 1.5f * fontHeight;
            float spaceAbove = fontHeight / 2;
            float noSpaceAbove = 0;
            string line = "Feature set name: \"" + Name + "\"";
            PdfHelper.WriteLines(pdf, headingSpacing, leadingFactor, indentLocal, line,
                                 ref currentTextPos, fontBold, fontHeight);

            // Write feature set type
            line = "Feature set type: " + _type.ToString();
            PdfHelper.WriteLines(pdf, spaceAbove, leadingFactor, indentLocal, line,
                                 ref currentTextPos, font, fontHeight);

            // Write the FeatureSet.Description
            currentTextPos.x = PdfHelper.MARGIN_LEFT;
            string description = "Description: " + Description;
            PdfHelper.WriteLines(pdf, spaceAbove, leadingFactor, indentLocal, description,
                                 ref currentTextPos, font, fontHeight);

            // Write name of the shapefile
            if (_shapefileAbsolutePath == "")
            {
                line = "Shapefile: *** not assigned *** ";
            }
            else
            {
                line = "Shapefile: " + _shapefileAbsolutePath;
            }
            PdfHelper.WriteLines(pdf, spaceAbove, leadingFactor, indentLocal, line,
                                 ref currentTextPos, font, fontHeight);

            // Write name of the SMP file
            if (_timeSeriesAbsolutePath == "")
            {
                line = "Time-series data file: *** not assigned *** ";
            }
            else
            {
                line = "Time-series data file: " + _timeSeriesAbsolutePath;
            }
            PdfHelper.WriteLines(pdf, spaceAbove, leadingFactor, indentLocal, line,
                                 ref currentTextPos, font, fontHeight);

            // Write name of the secondary SMP file
            if (_timeSeriesSecondaryAbsolutePath != "")
            {
                line = "Secondary time-series data file: " + _timeSeriesSecondaryAbsolutePath;
                PdfHelper.WriteLines(pdf, spaceAbove, leadingFactor, indentLocal, line,
                                     ref currentTextPos, font, fontHeight);
            }

            // Write interpretation method for time-series data
            if (_timeSeriesAbsolutePath != "")
            {
                string[] interpretationMethods = new string[2];
                interpretationMethods[0] = "Piecewise";
                interpretationMethods[1] = "Stepwise";
                line = "Method used for interpreting time-series data: " + interpretationMethods[(int)InterpretationMethod];
                PdfHelper.WriteLines(pdf, spaceAbove, leadingFactor, indentLocal, line,
                                     ref currentTextPos, font, fontHeight);
            }

            // Write name shapefile attribute that provides link to time-series data
            if (_keyField == "")
            {
                line = "Shapefile attribute linking features to time-series data: *** not assigned ***";
            }
            else
            {
                line = "Shapefile attribute linking features to time-series data: " + _keyField;
            }
            PdfHelper.WriteLines(pdf, spaceAbove, leadingFactor, indentLocal, line,
                                 ref currentTextPos, font, fontHeight);

            // Describe method for assigning layer
            switch (_layerMethod)
            {
                case LayerMethod.ByAttribute:
                    line = "Attribute containing layer number: " + LayerAttribute;
                    PdfHelper.WriteLines(pdf, noSpaceAbove, leadingFactor, indentLocal, line,
                                         ref currentTextPos, font, fontHeight);
                    break;
                case LayerMethod.ByCellTops:
                    line = "Attribute containing feature top elevation: " + TopElevationAttribute;
                    PdfHelper.WriteLines(pdf, noSpaceAbove, leadingFactor, indentLocal, line,
                                         ref currentTextPos, font, fontHeight);
                    line = "Attribute containing feature bottom elevation: " + BottomElevationAttribute;
                    PdfHelper.WriteLines(pdf, noSpaceAbove, leadingFactor, indentLocal, line,
                                         ref currentTextPos, font, fontHeight);
                    break;
                case LayerMethod.Uniform:
                    line = "Model layer for all features: Layer " + _defaultLayer.ToString();
                    PdfHelper.WriteLines(pdf, noSpaceAbove, leadingFactor, indentLocal, line,
                                         ref currentTextPos, font, fontHeight);
                    break;
            }

            if (GeoValueList.Count > 0)
            {
                for (int i = 0; i < GeoValueList.Count; i++)
                {
                    GeoValue geoValue = GeoValueList[i];
                    switch (geoValue.GeoValueType)
                    {
                        case GeoValueType.Uniform:
                            line = "Uniform " + geoValue.Descriptor + ": " + geoValue.UniformValue.ToString();
                            break;
                        case GeoValueType.Attribute:
                            if (geoValue.Attribute != "")
                            {
                                line = "Attribute used for " + geoValue.Descriptor + ": " + geoValue.Attribute;
                            }
                            else
                            {
                                line = "Attribute for " + geoValue.Descriptor + " has not been defined";
                            }
                            break;
                    }
                    PdfHelper.WriteLines(pdf, noSpaceAbove, leadingFactor, indentLocal, line, ref currentTextPos, font, fontHeight);
                }
            }

            // Write summary (number of features)
            string summary = "";
            if (Items.Count == 1)
            {
                summary = "Feature set \"" + Name + "\" contains 1 feature.";
            }
            else
            {
                summary = "Feature set \"" + Name + "\" contains " + Items.Count.ToString() + " features.";
            }
            switch (this.Type)
            {
                case PackageType.ChdType:
                    summary = summary + "  For CHD feature sets, one feature is one CHD cell.";
                    break;
                case PackageType.RiverType:
                    summary = summary + "  For River feature sets, one feature is one river cell.";
                    break;
                case PackageType.WellType:
                    summary = summary + "  For Well feature sets, one feature is one well cell.";
                    if (_layerMethod == LayerMethod.ByCellTops)
                    {
                        summary = summary + "  When layer assignment is determined by comparing " +
                                  "well open interval to cell tops and bottoms, " +
                                  "a well may be represented by one or more well cells.";
                    }
                    break;
                case PackageType.RchType:
                    summary = summary + "  For Recharge feature sets, one feature includes all cells intersected by one recharge polygon'";
                    break;
                case PackageType.GhbType:
                    summary = summary + "  For GHB feature sets, one feature is one GHB cell.";
                    break;
            }
            PdfHelper.WriteLines(pdf, spaceAbove, leadingFactor, indentLocal, summary,
                                 ref currentTextPos, font, fontHeight);

        }

        public virtual void Draw()
        {
            throw new NotImplementedException();
        }

        public ScenarioFeature GetFeatureByName(string name)
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (((ScenarioFeature)Items[i]).Name == name)
                {
                    return (ScenarioFeature)Items[i];
                }
            }
            return null;
        }

        public GeoValue GetGeoValueByDescriptor(string descriptor)
        {
            for (int i = 0; i < GeoValueList.Count; i++)
            {
                if (GeoValueList[i].Descriptor == descriptor)
                {
                    return GeoValueList[i];
                }
            }
            return null;
        }

        public virtual string GetScenarioID()
        {
            return ((Package)Parent).GetScenarioID();
        }

        public bool NeedRowAndCol
        {
            get
            {
                return _needRowAndCol;
            }
            set
            {
                _needRowAndCol = value;
            }
        }

        public override string ToString()
        {
            return "Shapefile: " + _shapefileAbsolutePath;
        }

        #endregion Methods
    }
}
