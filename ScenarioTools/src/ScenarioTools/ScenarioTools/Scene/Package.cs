using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using HPdf;

using ScenarioTools.DataClasses;
using ScenarioTools.ModflowReaders;
using ScenarioTools.PdfWriting;

using USGS.Puma.FiniteDifference;

namespace ScenarioTools.Scene
{
    public abstract class Package : ITaggable
    {
        #region Fields
        protected int _indexMasterFeatureSet = 0;
        protected int _tag;
        protected string _description;
        protected string _name;
        protected bool[,] _active;  // For list-type packages.  Dimension: [nper, # boundaries]
        protected PackageType _type;
        protected TagLink _link;
        protected List<ITaggable> _featureSets;
        protected ITaggable _parent;
        #endregion Fields

        #region Constructors

        /// <summary>
        /// Default constructor for abstract class Package
        /// </summary>
        public Package()
        {
            _type = PackageType.NoType;
            _tag = 0;
            _link = new TagLink();
            _description = "";
            _name = "";
            _featureSets = new List<ITaggable>();
            _parent = null;
            CbcFlag = 0;
            NameFileDirectory = "";
            InputFileAbsolutePath = "";
            NameFileEntry = new NameFileEntry();
        }

        /// <summary>
        /// Typed constructor for abstract class Package
        /// </summary>
        /// <param name="type"></param>
        protected Package(PackageType type, ITaggable parent ) : this()
        {
            _type = type;
            _parent = parent;
        }

        #endregion Constructors

        #region Properties
        public int IndexMasterFeatureSet
        {
            get
            {
                return _indexMasterFeatureSet;
            }
            set
            {
                _indexMasterFeatureSet = value;
            }
        }
        public int CbcFlag { get; set; }
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
        public string NameFileDirectory { get; set; }
        public string InputFileAbsolutePath { get; set; }
        public bool[,] Active
        {
            get
            {
                return _active;
            }
        }
        public virtual PackageType Type
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
        public ElementType ElemType
        {
            get
            {
                return ElementType.Package;
            }
        }
        public DiscretizationFile DiscretizationFile { get; set; }        
        public DateTime SimulationStartTime { get; set; }
        public DateTime SimulationEndTime { get; set; }
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
        public NameFileEntry NameFileEntry { get; set; }
        #endregion Properties

        #region Abstract Methods

        public abstract object Clone();

        public abstract bool Export(CellCenteredArealGrid modelGrid, 
                                    ToolStripItem toolStripItem, 
                                    BackgroundWorker bgWorker, 
                                    bool freeFormat);

        public abstract string FileName();

        public abstract string FileType();

        public abstract string GetDefaultFeatureSetNodeText();

        public abstract void Populate(CellCenteredArealGrid modelGrid);

        #endregion Abstract Methods

        #region Virtual Methods

        public virtual void InitializeGeoValueList(List<GeoValue> geoValueList)
        {
            if (geoValueList == null)
            {
                geoValueList = new List<GeoValue>();
            }
            else
            {
                geoValueList.Clear();
            }
            // Packages that require GeoValue elements add them here
        }
        /// <summary>
        /// Return complete description of package
        /// </summary>
        /// <returns></returns>
        public virtual List<string> Describe()
        {
            List<string> descriptionList = new List<string>();

            // Start with package description
            descriptionList.Add("Package name: " + _name);
            descriptionList.Add("Package type: " + _type.ToString());
            descriptionList.Add("Description: " + _description);
            int featureSetCount = 0;
            if (_featureSets != null)
            {
                featureSetCount = _featureSets.Count;
            }
            descriptionList.Add("Number of feature sets in \"" + _name + "\" package: " + featureSetCount.ToString());

            // Add description of each FeatureSet in package
            if (featureSetCount > 0)
            {
                List<string> fsDescriptionList;
                string entry;
                descriptionList.Add("Feature set documentation:");
                descriptionList.Add("");

                // Iterate through feature sets; add description of each
                for (int i = 0; i < _featureSets.Count; i++)
                {
                    fsDescriptionList = ((FeatureSet)_featureSets[i]).Describe();
                    for (int k = 0; k < fsDescriptionList.Count; k++)
                    {
                        entry = "    " + fsDescriptionList[k];
                        descriptionList.Add(entry);
                    }
                    if (i < _featureSets.Count - 1)
                    {
                        descriptionList.Add("");
                    }
                }
            }
            return descriptionList;
        }

        /// <summary>
        /// Add Package description to a PDF
        /// </summary>
        /// <param name="pdf"></param>
        public virtual void DescribeToPdf(HPdfDoc pdf, ref HPdfPoint currentTextPos,
                                          HPdfFont font, HPdfFont fontBold, float fontHeight, 
                                          float indent)
        {
            float leadingFactor = 0.5f;
            float indentLocal = indent;
            float paragraphSpacing = fontHeight;
            float headingSpacing = 1.5f * fontHeight;
            float spaceAbove = fontHeight / 2;
            string line = "Package name: \"" + Name + "\"";
            PdfHelper.WriteLines(pdf, headingSpacing, leadingFactor, indentLocal, line,
                                 ref currentTextPos, fontBold, fontHeight);

            // Write package type
            line = "Package type: " + _type.ToString();
            PdfHelper.WriteLines(pdf, spaceAbove, leadingFactor, indentLocal, line,
                                 ref currentTextPos, font, fontHeight);

            // Write name of package input file
            string scenarioID = ((Scenario)Parent).GetScenarioID();
            string inputFile = ((Scenario)Parent).NameFileDirectory + Path.DirectorySeparatorChar 
                               + scenarioID + Path.DirectorySeparatorChar + scenarioID + "." 
                               + FileType().ToLower();
            line = "Input file for this package: " + inputFile;
            PdfHelper.WriteLines(pdf, spaceAbove, leadingFactor, indentLocal, line,
                                 ref currentTextPos, font, fontHeight);

            // Write CbcFlag
            if (CbcFlag > 0)
            {
                line = "Cell-by-cell flow terms are written to file unit " + CbcFlag.ToString();
                PdfHelper.WriteLines(pdf, spaceAbove, leadingFactor, indentLocal, line,
                                     ref currentTextPos, font, fontHeight);
            }

            // Write the Package.Description
            currentTextPos.x = PdfHelper.MARGIN_LEFT;
            string description = "Description: " + Description;
            PdfHelper.WriteLines(pdf, spaceAbove, leadingFactor, indentLocal, description,
                                 ref currentTextPos, font, fontHeight);

            // Write summary (number of feature sets)
            string summary = "";
            if (Items.Count == 1)
            {
                summary = "This package contains 1 feature set.";
            }
            else
            {
                summary = "This package contains " + Items.Count.ToString() + " feature sets.";
            }
            PdfHelper.WriteLines(pdf, spaceAbove, leadingFactor, indentLocal, summary,
                                 ref currentTextPos, font, fontHeight);


            // Write Package description to the PDF

            // For each FeatureSet in scenario, write FeatureSet description to the PDF
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] is FeatureSet)
                {
                    ((FeatureSet)Items[i]).DescribeToPdf(pdf, ref currentTextPos, font, 
                                                         fontBold, fontHeight, indent);
                }
            }
        }

        public virtual void DrawAllFeatureSets()
        {
            foreach (FeatureSet fs in _featureSets)
            {
                fs.Draw();
            }
        }

        public virtual int FeatureCount()
        {
            int K = 0;
            for (int i = 0; i < Items.Count; i++)
            {
                K = K + Items[i].Items.Count;
            }
            return K;
        }

        public virtual string GetScenarioID()
        {
            return ((Scenario)Parent).GetScenarioID();
        }

        public virtual bool Import(string filename)
        {
            return false;
        }

        #endregion Virtual Methods

        #region Methods

        /// <summary>
        /// Return number of boundaries in package that are active in a specified time interval
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private int CountActive(DateTime beginTime, DateTime endTime)
        {
            // Iterate through all FeatureSet items in package
            int count = 0;
            SmpRecord smpRecord;
            foreach (FeatureSet fs in this._featureSets)
            {
                // Iterate through all Feature items in current FeatureSet
                foreach (ITaggable item in fs.Items)
                {
                    ScenarioFeature f = (ScenarioFeature)item;
                    for (int i = 0; i < f.SmpSeries.Records.Count; i++)
                    {
                        smpRecord = f.SmpSeries.Records[i];
                        // Ensure that smpRecord.DateTime is within time span of arguments
                        if (smpRecord.DateTime >= beginTime && smpRecord.DateTime <= endTime)
                        {
                            // If current Feature has a non-zero value in SmpSeries, increment _mxactw.
                            if (smpRecord.Value != 0.0d)
                            {
                                count++;
                                break;
                            }
                        }
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Return number of boundaries defined in package that are active in any stress period
        /// </summary>
        /// <returns></returns>
        public int CountActive()
        {
            int k = 0;
            int nper = _active.GetLength(0);
            int nbound = _active.GetLength(1);
            bool counted;
            for (int ibd = 0; ibd < nbound; ibd++)
            {
                counted = false;
                for (int iper = 0; iper < nper; iper++)
                {
                    if (_active[iper, ibd])
                    {
                        k++;
                        counted = true;
                        break;
                    }
                    if (counted)
                    {
                        break;
                    }
                }
            }
            return k;
        }

        /// <summary>
        /// Return number of boundaries defined in package that are active in a specified stress period
        /// </summary>
        /// <param name="periodNumZeroBased"></param>
        /// <returns></returns>
        public int CountActive(int periodNumZeroBased)
        {
            int k = 0;
            int nper = _active.GetLength(0);
            int nwel = _active.GetLength(1);
            if ((periodNumZeroBased >= 0) && (periodNumZeroBased < nper))
            {
                for (int iwel = 0; iwel < nwel; iwel++)
                {
                    if (_active[periodNumZeroBased, iwel])
                    {
                        k++;
                    }
                }
            }
            return k;
        }

        /// <summary>
        /// Return number of all features defined in package
        /// </summary>
        /// <returns></returns>
        public int CountAllFeatures()
        {
            int k = 0;
            for (int ifs = 0; ifs < Items.Count; ifs++)
            {
                k = k + Items[ifs].Items.Count;
            }
            return k;
        }

        public ScenarioFeature GetFeatureByName(string name)
        {
            ScenarioFeature feature = null;
            for (int ifs = 0; ifs < Items.Count; ifs++)
            {
                for (int iftr = 0; iftr < Items[ifs].Items.Count; iftr++)
                {
                    feature = (ScenarioFeature)Items[ifs].Items[iftr];
                    if (feature.Name == name)
                    {
                        return feature;
                    }
                }
            }
            return feature;
        }

        #endregion Methods

        #region ITaggable members

        public abstract void AssignFrom(ITaggable packageSource);

        public virtual List<ITaggable> Items
        {
            get
            {
                return _featureSets;
            }
            set
            {
                _featureSets = value;
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
            // Invoke ReTag for all FeatureSets.
            foreach (FeatureSet fs in _featureSets)
            {
                fs.ReTag();
            }
        }

        public void ConnectList(List<ITaggable> owner)
        {
            TagUtilities.ConnectList(this, owner);
        }

        public void ConvertRelativePaths(string oldDirectoryPath, string newDirectoryPath)
        {
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
                        nodeTag = (int)treeView.Nodes[i].Nodes[j].Tag;
                        if (nodeTag == this.Tag)
                        {
                            treeView.SelectedNode = treeView.Nodes[i].Nodes[j];
                            return;
                        }
                    }
                }
            }
        }

        #endregion ITaggable members
    
    }
}
