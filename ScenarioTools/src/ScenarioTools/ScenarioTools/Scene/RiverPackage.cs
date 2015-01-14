using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using GeoAPI.Geometries;
using ScenarioTools.DataClasses;
using ScenarioTools.ModflowReaders;
using ScenarioTools.Util;
using USGS.Puma.FiniteDifference;
using USGS.Puma.NTS.Features;
using USGS.Puma.NTS.Geometries;

namespace ScenarioTools.Scene
{
    public class RiverPackage : Package
    {
        const int GEO_VALUE_INDEX_RIVERBED_BOTTOM_ELEVATION = 0;
        const int GEO_VALUE_INDEX_HYDRAULIC_CONDUCTIVITY = 1;
        const int GEO_VALUE_INDEX_WIDTH = 2;
        const int GEO_VALUE_INDEX_RIVERBED_THICKNESS = 3;
        
        #region Fields
        int _npriv = 0; // Number of river parameters.  NPRIV of Modflow input
        int _mxactr;    // Max. number of river cells in simulation. MXACTR of Modflow input
        bool _noPrint;
        const string spaces50 = "                                                  ";
        #endregion Fields

        /// <summary>
        /// Constructor for class RiverPackage
        /// </summary>
        public RiverPackage(ITaggable parent) 
            : base(PackageType.RiverType, parent)
        {
            _npriv = 0;
            _mxactr = 0;
            _noPrint = false;
        }

        #region Properties
        public int Mxactr
        {
            get
            {
                return _mxactr;
            }
            set
            {
                _mxactr = value;
            }
        }
        public int Npriv
        {
            get
            {
                return _npriv;
            }
            set
            {
                _npriv = value;
            }
        }
        public bool NoPrint
        {
            get
            {
                return _noPrint;
            }
            set
            {
                _noPrint = value;
            }
        }
        #endregion Properties

        #region Methods
        public override void InitializeGeoValueList(List<GeoValue> geoValueList)
        {
            if (geoValueList == null)
            {
                geoValueList = new List<GeoValue>();
            }
            else
            {
                geoValueList.Clear();
            }

            // Define GeoValueList for River package
            // Riverbed bottom elevation
            // Allow only GeoValueType.Attribute for riverbed bottom elevation
            GeoValue geoValue = new GeoValue(0.0);
            geoValue.Descriptor = "riverbed bottom elevation";
            geoValue.GeoValueType = GeoValueType.Attribute;
            geoValueList.Add(geoValue);

            // Hydraulic conductivity of the riverbed
            geoValue = new GeoValue(0.0);
            geoValue.Descriptor = "hydraulic conductivity";
            geoValueList.Add(geoValue);

            // River width
            geoValue = new GeoValue(0.0);
            geoValue.Descriptor = "width";
            geoValueList.Add(geoValue);

            // Riverbed thickness
            geoValue = new GeoValue(0.0);
            geoValue.Descriptor = "riverbed thickness";
            geoValueList.Add(geoValue);
        }

        public override void AssignFrom(ITaggable riverPackageSource)
        {
            if (riverPackageSource is RiverPackage)
            {
                RiverPackage rps = (RiverPackage)riverPackageSource;
                CbcFlag = rps.CbcFlag;
                Npriv = rps.Npriv;
                Mxactr = rps.Mxactr;
                NoPrint = rps.NoPrint;
                Items.Clear();
                for (int i = 0; i < rps.Items.Count; i++)
                {
                    FeatureSet fs = new FeatureSet(PackageType.RiverType, this);
                    fs.AssignFrom(rps.Items[i]);
                    Items.Add(fs);
                }
            }
        }

        private void buildActiveMatrix(int progressMin, int progressMax,
                                           BackgroundWorker bgWorker)
        {
            int nRow = DiscretizationFile.getNrow();
            int nCol = DiscretizationFile.getNcol();
            int nPer = DiscretizationFile.getNper();
            FeatureSet featureSet;
            RiverCell rivCell;

            int numFeatureSets = Items.Count;
            int indx;

            // First, get number of features in all feature sets.
            int numFeatures = FeatureCount();

            // Allocate _active array and set all elements to true
            _active = new bool[nPer, numFeatures];
            for (int i = 0; i < nPer; i++)
            {
                for (int j = 0; j < numFeatures; j++)
                {
                    _active[i, j] = true;
                }
            }

            // Get an array of DateTime containing stress period end times.  Element [0] contains
            // the simulation start time.
            DateTime[] stressPeriodEndTimes = DiscretizationFile.GetStressPeriodEndTimes(SimulationStartTime);
            DateTime stressPeriodStartTime;
            DateTime stressPeriodEndTime;

            // Iterate through all stress periods
            for (int isp = 0; isp < nPer; isp++)
            {
                stressPeriodStartTime = stressPeriodEndTimes[isp];
                stressPeriodEndTime = stressPeriodEndTimes[isp + 1];

                // Reset index that points to all features in package
                indx = -1;

                // Iterate through all feature sets
                for (int ifs = 0; ifs < Items.Count; ifs++)
                {
                    featureSet = (FeatureSet)Items[ifs];
                    // Determine if this features in this feature set should be active during current stress period.

                    // If there is no secondary time series, all features are active in all stress periods, and 
                    // there is no need to change default (true) value stored in _active.

                    // If there is a secondary time series, a feature is active if any value in the 
                    // secondary time series for the stress period is > zero, or if most recent secondary
                    // time-series value preceding beginning of stress period is > zero.

                    if (featureSet.SecondarySmpSeries == null)
                    {
                        // Leave _active elements as is (true) for all features in this feature set.
                        indx += featureSet.Items.Count;
                    }
                    else
                    {
                        // Iterate through cells in current feature set
                        for (int i = 0; i < featureSet.Items.Count; i++)
                        {
                            rivCell = (RiverCell)featureSet.Items[i];

                            // Determine if secondary time series indicates river cell should be active in current stress period
                            indx++;
                            _active[isp, indx] = rivCell.SecondarySmpSeries.ValuePositiveDuringTimeInterval(stressPeriodStartTime, stressPeriodEndTime);
                        }
                    }
                }
            }
        }

        public override object Clone()
        {
            RiverPackage riverPackage = new RiverPackage(this.Parent);
            riverPackage.Npriv = this._npriv;
            riverPackage.Mxactr = this._mxactr;
            riverPackage.CbcFlag = this.CbcFlag;
            riverPackage.NoPrint = this._noPrint;
            riverPackage.Tag = this.Tag;
            riverPackage.Link = (TagLink)_link.Clone();
            riverPackage.Link.ScenarioElement = riverPackage;
            riverPackage.Link.TreeNode = this.Link.TreeNode;
            riverPackage.Name = this._name;
            riverPackage.Parent = this.Parent;
            riverPackage.Type = this.Type;
            riverPackage.Description = this._description;
            foreach (FeatureSet fs in this.Items)
            {
                FeatureSet newFeatureSet = (FeatureSet)fs.Clone();
                riverPackage.Items.Add(newFeatureSet);
            }
            return riverPackage;
        }

        public override bool Export(CellCenteredArealGrid modelGrid, 
                                    ToolStripItem toolStripItem,
                                    BackgroundWorker bgWorker,
                                    bool freeFormat)
        {
            bool OK = true;
            int progressMin = 0;
            int progressMax = 100;
            int bgwProgress = progressMin;
            bgWorker.ReportProgress(bgwProgress);
            toolStripItem.Text = "Exporting River Package...";

            // Allocate and initialize _active matrix
            buildActiveMatrix(progressMin, progressMax, bgWorker);

            int progMax = progressMax;
            int progFsInc = progMax - progressMin;
            int numFS = Items.Count;
            if (numFS > 0)
            {
                progFsInc = (progMax - progressMin) / numFS;
            }
            double stage;
            FeatureSet fs;

            // Open a StreamWriter
            string filePath = this.NameFileDirectory + Path.DirectorySeparatorChar
                                  + this.GetScenarioID() + Path.DirectorySeparatorChar
                                  + FileName();
            using (StreamWriter sw = File.CreateText(filePath))
            {
                // Write comments at top of RIV input file (Item 0)
                string str;
                str = "# RIV file generated by exporting package: \"" + Name + "\"";
                sw.WriteLine(str);
                sw.WriteLine("# ");
                StringUtil.InsertAsModflowCommentLines(sw, Description);
                sw.WriteLine("# ");
                str = "# Package includes the following feature set(s):";
                sw.WriteLine(str);
                sw.WriteLine("# ");
                foreach (ITaggable item in Items)
                {
                    if (item is FeatureSet)
                    {
                        fs = (FeatureSet)item;
                        str = "# Feature Set: \"" + fs.Name + "\"";
                        sw.WriteLine(str);
                        StringUtil.InsertAsModflowCommentLines(sw, fs.Description);
                        sw.WriteLine("# ");
                    }
                }
                _mxactr = CountActive();
                if (freeFormat)
                {
                    str = _mxactr.ToString() + "  " + CbcFlag.ToString();
                }
                else
                {
                    str = ModflowHelpers.ItoS(_mxactr, 10) + ModflowHelpers.ItoS(CbcFlag, 10);
                }
                if (_noPrint)
                {
                    str = str + "  NOPRINT ";
                }
                int posComment = 25;
                str = StringUtil.PadRightToLength(str, posComment);
                str = str + "# Item 2: MXACTR IRIVCB";
                sw.WriteLine(str);

                // First, iterate through stress periods
                int itmp;
                DateTime perStartDateTime;
                DateTime perEndDateTime = new DateTime();
                TimeSpan simTime = new TimeSpan(0);
                StressPeriod currentStressPeriod;
                RiverCell rivCell;
                perStartDateTime = SimulationStartTime;
                int nper = DiscretizationFile.getNper();
                int perCounter = 0;
                int perInc = nper / progFsInc;
                for (int iper = 0; iper < nper; iper++)
                {
                    currentStressPeriod = DiscretizationFile.getStressPeriod(iper);
                    perEndDateTime = perStartDateTime + currentStressPeriod.getTimeSpan();
                    itmp = CountActive(iper);
                    if (freeFormat)
                    {
                        str = itmp.ToString() + "  " + _npriv.ToString();
                    }
                    else
                    {
                        str = ModflowHelpers.ItoS(itmp, 10) + ModflowHelpers.ItoS(_npriv, 10);
                    }
                    str = StringUtil.PadRightToLength(str, posComment);
                    str = str + "# Item 5: ITMP NP, Stress period " + (iper + 1).ToString();
                    sw.WriteLine(str);

                    // Iterate through all FeatureSet items in package
                    for (int ifs = 0; ifs < Items.Count; ifs++)
                    {
                        fs = (FeatureSet)Items[ifs];

                        // Iterate through all river cells in FeatureSet
                        for (int icel = 0; icel < fs.Items.Count; icel++)
                        {
                            rivCell = (RiverCell)fs.Items[icel];
                            if (_active[iper, rivCell.Index])
                            {
                                // Find stage for current stress period for this feature
                                stage = rivCell.RiverStage(currentStressPeriod, perStartDateTime, DiscretizationFile.ModflowTimeUnit);
                                if (freeFormat)
                                {
                                    str = rivCell.Layer.ToString() + "  " + rivCell.Row.ToString() + "  " +
                                          rivCell.Column.ToString() + "  " + stage.ToString() + "  " + rivCell.Conductance +
                                          "  " + rivCell.Rbot;
                                    str = StringUtil.PadRightToLength(str, 44);
                                }
                                else
                                {
                                    str = ModflowHelpers.ItoS(rivCell.Layer, 10) + ModflowHelpers.ItoS(rivCell.Row, 10) 
                                          + ModflowHelpers.ItoS(rivCell.Column, 10) + ModflowHelpers.DtoS(stage, 10)
                                          + ModflowHelpers.DtoS(rivCell.Conductance, 10) + ModflowHelpers.DtoS(rivCell.Rbot, 10); 
                                }
                                if (fs.LabelFeatures == FeatureLabel.FeatureSetOnly || fs.LabelFeatures == FeatureLabel.FeatureSetAndName)
                                {
                                    str = str + "    Feature set: " + fs.Name;
                                }
                                if (fs.LabelFeatures == FeatureLabel.FeatureNameOnly || fs.LabelFeatures == FeatureLabel.FeatureSetAndName)
                                {
                                    str = str + "    River feature name: " + rivCell.Name;
                                }
                                sw.WriteLine(str);
                            }
                        }
                    }
                    perCounter++;
                    if (perCounter == perInc)
                    {
                        bgwProgress++;
                        bgWorker.ReportProgress(bgwProgress);
                        perCounter = 0;
                    }
                    perStartDateTime = perEndDateTime;
                    sw.Flush();
                }
                sw.Dispose();
            }
            return OK;
        }

        public override string FileName()
        {
            return GetScenarioID() + ".riv";
        }

        public override string FileType()
        {
            return "RIV";
        }

        public override string GetDefaultFeatureSetNodeText()
        {
            return "New River Set";
        }

        public override void Populate(CellCenteredArealGrid modelGrid)
        {
            FeatureSet featureSet;
            for (int j = 0; j < Items.Count; j++)
            {
                featureSet = (FeatureSet)Items[j];
                PopulateOneFeatureSet(featureSet, modelGrid);
            }
        }

        protected bool PopulateOneFeatureSet(FeatureSet featureSet, CellCenteredArealGrid modelGrid)
        {
            bool OK = false;
            FeatureCollection featureList;
            if (File.Exists(featureSet.ShapefileAbsolutePath))
            {
                featureList = USGS.Puma.IO.EsriShapefileIO.Import(featureSet.ShapefileAbsolutePath);
                if ((modelGrid != null) && (featureList != null))
                {
                    OK = PopulateFeatureSet(featureSet, featureList, modelGrid);
                }
            }
            return OK;
        }

        protected bool PopulateFeatureSet(FeatureSet featureSet, FeatureCollection featureList, 
                                        CellCenteredArealGrid modelGrid)
        {
            // Populate a FeatureSet belonging to a RiverPackage with polylines
            bool OK = true;
            try
            {
                if (featureList != null && modelGrid != null)
                {
                    featureSet.Clear();
                    int nRow = DiscretizationFile.getNrow();
                    int nCol = DiscretizationFile.getNcol();
                    int layer;
                    int indx = -1;
                    double openTopElev;
                    double openBottomElev;
                    double cellTopElev;
                    double cellBottomElev;
                    double cellHeight;
                    double hydCond = double.NaN;
                    double width = double.NaN;
                    double thickness = double.NaN;
                    double riverbedBottomElevation = double.NaN;
                    double maxOpenHeightInCell;
                    double openHeightInCell;
                    double openCellTop;
                    double openCellBottom;
                    string keyField = featureSet.KeyField;
                    bool hydCondIsUniform = featureSet.GeoValueList[GEO_VALUE_INDEX_HYDRAULIC_CONDUCTIVITY].GeoValueType == GeoValueType.Uniform;
                    bool widthIsUniform = featureSet.GeoValueList[GEO_VALUE_INDEX_WIDTH].GeoValueType == GeoValueType.Uniform;
                    bool thicknessIsUniform = featureSet.GeoValueList[GEO_VALUE_INDEX_RIVERBED_THICKNESS].GeoValueType == GeoValueType.Uniform;
                    if (hydCondIsUniform)
                    {
                        hydCond = featureSet.GeoValueList[GEO_VALUE_INDEX_HYDRAULIC_CONDUCTIVITY].UniformValue;
                    }
                    if (widthIsUniform)
                    {
                        width = featureSet.GeoValueList[GEO_VALUE_INDEX_WIDTH].UniformValue;
                    }
                    if (thicknessIsUniform)
                    {
                        thickness = featureSet.GeoValueList[GEO_VALUE_INDEX_RIVERBED_THICKNESS].UniformValue;
                    }
                    LineGridder lineGridder = new LineGridder(modelGrid);
                    USGS.Puma.NTS.Geometries.Geometry geometry;
                    GridCell gridCell;
                    RiverCell riverCell;
                    IMultiLineString multiLineString;
                    AttributesTable attributesTable;
                    SmpSeries smpSeriesForFeature;
                    SmpSeries secondarySmpSeriesForFeature = null;
                    featureSet.SmpSeries = new SmpSeries(featureSet.TimeSeriesAbsolutePath);
                    if (File.Exists(featureSet.TimeSeriesSecondaryAbsolutePath))
                    {
                        featureSet.SecondarySmpSeries = new SmpSeries(featureSet.TimeSeriesSecondaryAbsolutePath);
                    }

                    // Iterate through RIV multi-line strings
                    for (int m = 0; m < featureList.Count; m++)
                    {
                        geometry = (USGS.Puma.NTS.Geometries.Geometry)featureList[m].Geometry;
                        attributesTable = (AttributesTable)featureList[m].Attributes;
                        if (!hydCondIsUniform)
                        {
                            hydCond = Convert.ToDouble(attributesTable[featureSet.GeoValueList[GEO_VALUE_INDEX_HYDRAULIC_CONDUCTIVITY].Attribute]);
                        }
                        if (!widthIsUniform)
                        {
                            width = Convert.ToDouble(attributesTable[featureSet.GeoValueList[GEO_VALUE_INDEX_WIDTH].Attribute]);
                        }
                        if (!thicknessIsUniform)
                        {
                            thickness = Convert.ToDouble(attributesTable[featureSet.GeoValueList[GEO_VALUE_INDEX_RIVERBED_THICKNESS].Attribute]);
                        }
                        riverbedBottomElevation = Convert.ToDouble(attributesTable[featureSet.GeoValueList[GEO_VALUE_INDEX_RIVERBED_BOTTOM_ELEVATION].Attribute]);

                        if (geometry is MultiLineString)
                        {
                            multiLineString = (MultiLineString)geometry;
                            Feature line = new Feature(multiLineString, attributesTable);
                            string id = attributesTable[keyField].ToString();
                            smpSeriesForFeature = featureSet.SmpSeries.FilterById(id);
                            if (featureSet.SecondarySmpSeries != null)
                            {
                                secondarySmpSeriesForFeature = featureSet.SecondarySmpSeries.FilterById(id);
                            }

                            double tolerance = 0.001;
                            GridCellReachList gridCellReachList = lineGridder.CreateReachListFromFeature(line, tolerance, 0, 1);

                            if (gridCellReachList.Count > 0)
                            {
                                // Iterate through grid cell reaches
                                for (int i = 0; i < gridCellReachList.Count; i++)
                                {
                                    GridCellReach gridCellReach = gridCellReachList[i];
                                    double length = gridCellReach.ReachLength;
                                    gridCell = new GridCell(gridCellReach.Row, gridCellReach.Column);

                                    // Assign layer
                                    layer = 0;
                                    switch (featureSet.LayMethod)
                                    {
                                        case LayerMethod.Uniform:
                                            layer = featureSet.DefaultLayer;
                                            break;
                                        case LayerMethod.ByAttribute:
                                            layer = Convert.ToInt32(featureList[m].Attributes[featureSet.LayerAttribute]);
                                            break;
                                        case LayerMethod.ByCellTops:
                                            openTopElev = Convert.ToDouble(featureList[m].Attributes[featureSet.TopElevationAttribute]);
                                            openBottomElev = Convert.ToDouble(featureList[m].Attributes[featureSet.BottomElevationAttribute]);
                                            maxOpenHeightInCell = 0.0;
                                            // Select first (closest to layer 1) cell that contains largest part of open interval
                                            for (int k = 1; k <= DiscretizationFile.getNlay(); k++)
                                            {
                                                cellTopElev = DiscretizationFile.GetCellTopElevation(k, gridCell.Row, gridCell.Column);
                                                cellBottomElev = DiscretizationFile.GetCellBottomElevation(k, gridCell.Row, gridCell.Column);
                                                cellHeight = cellTopElev - cellBottomElev;
                                                if (cellHeight > 0.0 && openTopElev > cellBottomElev && openBottomElev < cellTopElev)
                                                {
                                                    openCellTop = Math.Min(cellTopElev, openTopElev);
                                                    openCellBottom = Math.Max(cellBottomElev, openBottomElev);
                                                    openHeightInCell = openCellTop - openCellBottom;
                                                    if (openHeightInCell > maxOpenHeightInCell)
                                                    {
                                                        layer = k;
                                                        maxOpenHeightInCell = openHeightInCell;
                                                    }
                                                }
                                            }
                                            break;
                                    }

                                    // Create a RiverCell from the GridCell and the GridCellReach
                                    riverCell = new RiverCell(gridCell);
                                    riverCell.ReachLength = gridCellReach.ReachLength;

                                    // Default layer = 1
                                    riverCell.Layer = 1;
                                    if (layer > 0 && layer <= DiscretizationFile.getNlay())
                                    {
                                        riverCell.Layer = layer;
                                    }

                                    // Assign the (filtered )SmpSeries to the river cell
                                    riverCell.SmpSeries = smpSeriesForFeature;
                                    riverCell.SecondarySmpSeries = secondarySmpSeriesForFeature;

                                    // Assign the attributes table to the river cell
                                    riverCell.Attributes = attributesTable;

                                    // Calculate riverbed conductance
                                    riverCell.Conductance = hydCond * riverCell.ReachLength * width / thickness;

                                    // Assign riverbed bottom elevation
                                    riverCell.Rbot = riverbedBottomElevation;

                                    // Assign properties from feature set
                                    riverCell.InterpretationMethod = featureSet.InterpretationMethod;

                                    // Assign sequential index of this river cell
                                    indx++;
                                    riverCell.Index = indx;

                                    // Assign the parent
                                    riverCell.Parent = this;

                                    // The river cell name is assigned the name of the multiline string
                                    riverCell.Name = (string)featureList[m].Attributes[featureSet.KeyField];

                                    // Add the new river cell to the feature set
                                    featureSet.Add(riverCell);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                OK = false;
            }
            return OK;
        }

        #endregion Methods
    }
}
