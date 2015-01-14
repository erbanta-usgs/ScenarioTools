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
    public class GhbPackage : Package
    {
        const int GEO_VALUE_INDEX_LEAKANCE = 0;

        #region Fields
        private int _mxactb;
        private int _npghb = 0;
        private bool _noPrint;
        const string spaces50 = "                                                  ";
        #endregion Fields

        /// <summary>
        /// Constructor for class GhbPackage
        /// </summary>
        /// <param name="parent"></param>
        public GhbPackage(ITaggable parent)
            : base(PackageType.GhbType, parent)
        {
            _mxactb = 0;
            _npghb = 0;
            _noPrint = false;
        }

        #region Properties
        public int Mxactb
        {
            get
            {
                return _mxactb;
            }
            set
            {
                _mxactb = value;
            }
        }
        public int Npghb
        {
            get
            {
                return _npghb;
            }
            set
            {
                _npghb = value;
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
            // Define GeoValueList for GHB package
            // Leakance
            GeoValue geoValue = new GeoValue(0.0);
            geoValue.Descriptor = "leakance";
            geoValueList.Add(geoValue);
        }
        public override void AssignFrom(ITaggable ghbPackageSource)
        {
            if (ghbPackageSource is GhbPackage)
            {
                GhbPackage ghbPkg = (GhbPackage)ghbPackageSource;
                Mxactb = ghbPkg.Mxactb;
                CbcFlag = ghbPkg.CbcFlag;
                Npghb = ghbPkg.Npghb;                
                NoPrint = ghbPkg.NoPrint;
                Items.Clear();
                for (int i = 0; i < ghbPkg.Items.Count; i++)
                {
                    FeatureSet fs = new FeatureSet(PackageType.ChdType, this);
                    fs.AssignFrom(ghbPkg.Items[i]);
                    Items.Add(fs);
                }
            }
        }
        /// <summary>
        /// Requires that all feature sets in package have already been populated with features
        /// </summary>
        /// <param name="progressMin"></param>
        /// <param name="progressMax"></param>
        /// <param name="bgWorker"></param>
        private void buildActiveMatrix(int progressMin, int progressMax,
                                           BackgroundWorker bgWorker)
        {
            int nRow = DiscretizationFile.getNrow();
            int nCol = DiscretizationFile.getNcol();
            int nPer = DiscretizationFile.getNper();
            FeatureSet featureSet;
            GhbCell ghbCell;

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
                stressPeriodEndTime = stressPeriodEndTimes[isp+1];

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
                        // Iterate through GHB cells in current feature set
                        for (int i = 0; i < featureSet.Items.Count; i++)
                        {
                            ghbCell = (GhbCell)featureSet.Items[i];

                            // Determine if secondary time series indicates GHB cell should be active in current stress period
                            indx++;
                            _active[isp, indx] = ghbCell.SecondarySmpSeries.ValuePositiveDuringTimeInterval(stressPeriodStartTime, stressPeriodEndTime);
                        }
                    }
                }
            }
        }

        public override object Clone()
        {
            GhbPackage ghbPackage = new GhbPackage(this.Parent);
            ghbPackage.Mxactb = this._mxactb;
            ghbPackage.CbcFlag = this.CbcFlag;
            ghbPackage.Npghb = this._npghb;
            ghbPackage.NoPrint = this._noPrint;
            ghbPackage.Tag = this.Tag;
            ghbPackage.Link = (TagLink)_link.Clone();
            ghbPackage.Link.ScenarioElement = ghbPackage;
            ghbPackage.Link.TreeNode = this.Link.TreeNode;
            ghbPackage.Name = this._name;
            ghbPackage.Parent = this.Parent;
            ghbPackage.Type = this.Type;
            ghbPackage.Description = this._description;
            foreach (FeatureSet fs in this.Items)
            {
                FeatureSet newFeatureSet = (FeatureSet)fs.Clone();
                ghbPackage.Items.Add(newFeatureSet);
            }
            return ghbPackage;
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
            toolStripItem.Text = "Exporting GHB Package...";

            // Allocate and initialize _active matrix
            buildActiveMatrix(progressMin, progressMax, bgWorker);

            int progMax = progressMax;
            int progFsInc = progMax - progressMin;
            int numFS = Items.Count;
            if (numFS > 0)
            {
                progFsInc = (progMax - progressMin) / numFS;
            }
            double bHead;
            FeatureSet fs;

            // Open a StreamWriter
            string filePath = this.NameFileDirectory + Path.DirectorySeparatorChar
                                  + this.GetScenarioID() + Path.DirectorySeparatorChar
                                  + FileName();
            using (StreamWriter sw = File.CreateText(filePath))
            {
                // Write comments at top of GHB input file (Item 0)
                string str;
                str = "# GHB file generated by exporting package: \"" + Name + "\"";
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
                _mxactb = CountActive();
                if (freeFormat)
                {
                    str = _mxactb.ToString() + "  " + CbcFlag.ToString();
                }
                else
                {
                    str = ModflowHelpers.ItoS(_mxactb, 10) + ModflowHelpers.ItoS(CbcFlag, 10);
                }
                if (_noPrint)
                {
                    str = str + "  NOPRINT ";
                }
                int posComment = 25;
                str = StringUtil.PadRightToLength(str, posComment);
                str = str + "# Item 2: MXACTB IGHBCB";
                sw.WriteLine(str);
                
                // First, iterate through stress periods
                int itmp;
                DateTime perStartDateTime;
                DateTime perEndDateTime = new DateTime();
                TimeSpan simTime = new TimeSpan(0);
                StressPeriod currentStressPeriod;
                GhbCell ghbCell;
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
                        str = itmp.ToString() + "  " + _npghb.ToString();
                    }
                    else
                    {
                        str = ModflowHelpers.ItoS(itmp, 10) + ModflowHelpers.ItoS(_npghb, 10);
                    }
                    str = StringUtil.PadRightToLength(str, posComment);
                    str = str + "# Item 5: ITMP NP, Stress period " + (iper + 1).ToString();
                    sw.WriteLine(str);

                    // Iterate through all FeatureSet items in package
                    for (int ifs = 0; ifs < Items.Count; ifs++)
                    {
                        fs = (FeatureSet)Items[ifs];

                        // Iterate through all GHB cells in FeatureSet
                        for (int icel = 0; icel < fs.Items.Count; icel++)
                        {
                            ghbCell = (GhbCell)fs.Items[icel];
                            if (_active[iper, ghbCell.Index])
                            {
                                // Find GHB head for current stress period for this feature
                                bHead = ghbCell.GhbHead(currentStressPeriod, perStartDateTime, DiscretizationFile.ModflowTimeUnit);
                                if (freeFormat)
                                {
                                    str = ghbCell.Layer.ToString() + "  " + ghbCell.Row.ToString() + "  " +
                                          ghbCell.Column.ToString() + "  " + bHead.ToString() + "  " + ghbCell.Conductance;
                                    str = StringUtil.PadRightToLength(str, 44);
                                }
                                else
                                {
                                    str = ModflowHelpers.ItoS(ghbCell.Layer, 10) + ModflowHelpers.ItoS(ghbCell.Row, 10) + ModflowHelpers.ItoS(ghbCell.Column, 10)
                                          + ModflowHelpers.DtoS(bHead, 10) + ModflowHelpers.DtoS(ghbCell.Conductance, 10);
                                }
                                if (fs.LabelFeatures == FeatureLabel.FeatureSetOnly || fs.LabelFeatures == FeatureLabel.FeatureSetAndName)
                                {
                                    str = str + "    Feature set: " + fs.Name;
                                }
                                if (fs.LabelFeatures == FeatureLabel.FeatureNameOnly || fs.LabelFeatures == FeatureLabel.FeatureSetAndName)
                                {
                                    str = str + "    GHB feature name: " + ghbCell.Name;
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
            return GetScenarioID() + ".ghb";
        }

        public override string FileType()
        {
            return "GHB";
        }

        public override string GetDefaultFeatureSetNodeText()
        {
            return "New GHB Set";
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
                if (featureList != null && modelGrid != null)
                {
                    OK = PopulateFeatureSet(featureSet, featureList, modelGrid);
                }
            }
            return OK;
        }

        /// <summary>
        /// Populate a FeatureSet belonging to a GhbPackage by intersecting 
        /// polygons in the shapefile with cells in the model grid.
        /// </summary>
        /// <param name="featureSet">FeatureSet to be populated</param>
        /// <param name="featureList">List of polygon features from shapefile</param>
        /// <param name="modelGrid">Model grid</param>
        /// <returns></returns>
        protected bool PopulateFeatureSet(FeatureSet featureSet, FeatureCollection featureList,
                                        CellCenteredArealGrid modelGrid)
        {
            // For the GHB Package one feature is one GhbCell
            bool OK = true;
            try
            {
                if (featureList != null && modelGrid != null)
                {
                    featureSet.Clear();
                    int nRow = DiscretizationFile.getNrow();
                    int nCol = DiscretizationFile.getNcol();
                    int layer, rowOneBased, columnOneBased;
                    double openTopElev;
                    double openBottomElev;
                    double cellArea;
                    double cellTopElev;
                    double cellBottomElev;
                    double cellHeight;
                    double leakance = double.NaN;
                    double maxOpenHeightInCell;
                    double openHeightInCell;
                    double openCellTop;
                    double openCellBottom;
                    string keyField = featureSet.KeyField;
                    bool leakanceIsUniform = featureSet.GeoValueList[GEO_VALUE_INDEX_LEAKANCE].GeoValueType == GeoValueType.Uniform;
                    if (leakanceIsUniform)
                    {
                        leakance = featureSet.GeoValueList[GEO_VALUE_INDEX_LEAKANCE].UniformValue;
                    }
                    USGS.Puma.NTS.Geometries.Geometry geometry;
                    GridCell gridCell;
                    Coordinate location = new Coordinate();
                    IPolygon polygon;
                    AttributesTable attributesTable;
                    SmpSeries smpSeriesForFeature;
                    SmpSeries secondarySmpSeriesForFeature = null;
                    featureSet.SmpSeries = new SmpSeries(featureSet.TimeSeriesAbsolutePath);
                    if (File.Exists(featureSet.TimeSeriesSecondaryAbsolutePath))
                    {
                        featureSet.SecondarySmpSeries = new SmpSeries(featureSet.TimeSeriesSecondaryAbsolutePath);
                    }

                    // Iterate through GHB polygons
                    for (int m = 0; m < featureList.Count; m++)
                    {
                        geometry = (USGS.Puma.NTS.Geometries.Geometry)featureList[m].Geometry;
                        attributesTable = (AttributesTable)featureList[m].Attributes;
                        if (!leakanceIsUniform)
                        {
                            leakance = Convert.ToDouble(attributesTable[featureSet.GeoValueList[GEO_VALUE_INDEX_LEAKANCE].Attribute]);
                        }
                        if (geometry is IPolygon)
                        {
                            polygon = (IPolygon)geometry;

                            // The following constructor causes multiplier matrix to be populated
                            AreaWeightedPolygonGridder gridder = new AreaWeightedPolygonGridder(modelGrid, polygon);
                            string id = attributesTable[keyField].ToString();
                            smpSeriesForFeature = featureSet.SmpSeries.FilterById(id);
                            if (featureSet.SecondarySmpSeries != null)
                            {
                                secondarySmpSeriesForFeature = featureSet.SecondarySmpSeries.FilterById(id);
                            }

                            // Iterate through model cells
                            int indx = -1;
                            for (int i = 0; i < nRow; i++)
                            {
                                rowOneBased = i + 1;
                                for (int j = 0; j < nCol; j++)
                                {
                                    columnOneBased = j + 1;

                                    // Does GHB polygon intersect this cell?
                                    if (gridder.MultiplierMatrix[i, j] > 0.0)
                                    {
                                        gridCell = new GridCell(rowOneBased, columnOneBased);

                                        #region Assign layer
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

                                        // Default layer = 1
                                        gridCell.Layer = 1;
                                        if (layer > 0 && layer <= DiscretizationFile.getNlay())
                                        {
                                            gridCell.Layer = layer;
                                        }
                                        #endregion Assign layer

                                        // Construct a new GHB cell
                                        GhbCell ghbCell = new GhbCell(gridCell);

                                        // Assign the (filtered) SmpSeries to the GHB cell
                                        ghbCell.SmpSeries = smpSeriesForFeature;
                                        ghbCell.SecondarySmpSeries = secondarySmpSeriesForFeature;

                                        // Assign AreaWeightedPolygonGridder and AttributesTable to the GHB cell
                                        ghbCell.PolygonGridder = gridder;
                                        ghbCell.Attributes = attributesTable;

                                        // Calculate GHB-cell conductance
                                        cellArea = DiscretizationFile.Delr[j] * DiscretizationFile.Delc[i];
                                        ghbCell.Conductance = leakance * gridder.MultiplierMatrix[i, j] * cellArea;

                                        // Assign properties from feature set
                                        ghbCell.InterpretationMethod = featureSet.InterpretationMethod;

                                        // Assign sequential index of this GHB cell
                                        indx++;
                                        ghbCell.Index = indx;

                                        // Assign the parent, but as which object? Ned TODO: decide this
                                        // Should the parent be the feature set or the package? 
                                        // Not sure it matters at the feature level.
                                        //ghbCell.Parent = featureSet;
                                        ghbCell.Parent = this;

                                        // The GHB cell name is assigned the name of the GHB polygon
                                        ghbCell.Name = (string)featureList[m].Attributes[featureSet.KeyField];

                                        // Add the new GHB cell to the feature set
                                        featureSet.Add(ghbCell);
                                    }
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
