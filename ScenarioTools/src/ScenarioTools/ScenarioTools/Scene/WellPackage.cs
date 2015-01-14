using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using ScenarioTools.DataClasses;
using ScenarioTools.ModflowReaders;
using ScenarioTools.Util;

using USGS.Puma.FiniteDifference;
using USGS.Puma.NTS.Features;
using USGS.Puma.NTS.Geometries;

namespace ScenarioTools.Scene
{
    public class WellPackage : Package
    {
        #region Fields

        int _npwel = 0; // Number of well parameters. NPWEL of Modflow input
        int _mxactw;    // Max. number of wells in simulation. MXACTW of Modflow input
        bool _noPrint;
        const string spaces50 = "                                                  ";

        #endregion Fields

        /// <summary>
        /// Constructor for WellPackage class
        /// </summary>
        public WellPackage(ITaggable parent) : base(PackageType.WellType, parent)
        {
            _npwel = 0;
            _mxactw = 0;
            _noPrint = false;
        }

        #region Properties

        public int Mxactw
        {
            get
            {
                return _mxactw;
            }
            set
            {
                _mxactw = value;
            }
        }

        public int Npwel
        {
            get
            {
                return _npwel;
            }
            set
            {
                _npwel = value;
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

        public override void AssignFrom(ITaggable wellPackageSource)
        {
            if (wellPackageSource is WellPackage)
            {
                WellPackage wps = (WellPackage)wellPackageSource;
                Npwel = wps.Npwel;
                Mxactw = wps.Mxactw;
                CbcFlag = wps.CbcFlag;
                NoPrint = wps.NoPrint;
                Items.Clear();
                for (int i = 0; i < wps.Items.Count; i++)
                {
                    FeatureSet fs = new FeatureSet(PackageType.WellType, this);
                    fs.AssignFrom(wps.Items[i]);
                    Items.Add(fs);
                }
            }
        }

        private bool BuildActiveWellMatrix(int progressMin, int progressMax, 
                                           BackgroundWorker bgWorker)
        {
            bool OK = false;
            bool noneActive = true;
            int bgwProgress = progressMin;
            bgWorker.ReportProgress(bgwProgress);
            int nftr = CountAllFeatures();
            DiscretizationFile disFile = DiscretizationFile;
            int nper = disFile.getNper();
            ScenarioFeature feature;
            ScenarioFeature f;
            int periodNum = -1;
            try
            {
                // Allocate _active matrix
                _active = new bool[nper, nftr];
                for (int j = 0; j < nftr; j++)
                {
                    for (int i = 0; i < nper; i++)
                    {
                        _active[i, j] = false;
                    }
                }
                // Assign index in _active to each feature in package
                int indx = 0;
                for (int ifs = 0; ifs < Items.Count; ifs++)
                {
                    for (int iftr = 0; iftr < Items[ifs].Items.Count; iftr++)
                    {
                        feature = (ScenarioFeature)Items[ifs].Items[iftr];
                        feature.Index = indx;
                        indx++;
                    }
                }
                DateTime smpTime;
                DateTime[] stressPeriodStartTimes = new DateTime[nper];
                DateTime[] stressPeriodEndTimes = new DateTime[nper];
                stressPeriodStartTimes[0] = SimulationStartTime;
                for (int iper=0;iper<nper;iper++)
                {
                    stressPeriodEndTimes[iper] = stressPeriodStartTimes[iper] + 
                                                     disFile.getStressPeriod(iper).getTimeSpan();
                    if (iper < (nper - 1))
                    {
                        stressPeriodStartTimes[iper + 1] = stressPeriodEndTimes[iper];
                    }
                }
                DateTime stressPeriodEndTime;
                SmpRecord smpRecord;
                FeatureSet fs;
                bool activeSet;
                int ftrCounter;
                int ftrCount;
                int ftrInc;
                int progFsInc = progressMax - progressMin;
                int numFS = Items.Count;
                if (numFS > 0)
                {
                    progFsInc = (progressMax - progressMin) / numFS;
                }
                if (progFsInc == 0) progFsInc = 1;
                for (int ifs = 0; ifs < Items.Count; ifs++)
                {
                    fs = (FeatureSet)Items[ifs];
                    ftrCount = fs.Items.Count;
                    ftrInc = ftrCount / progFsInc;
                    ftrCounter = 0;
                    for (int iftr = 0; iftr < fs.Items.Count; iftr++)
                    {
                        f = (ScenarioFeature)fs.Items[iftr];
                        activeSet = false;
                        stressPeriodEndTime = stressPeriodEndTimes[0];
                        for (int irec = 0; irec < f.SmpSeries.Records.Count; irec++)
                        {
                            smpRecord = f.SmpSeries.Records[irec];
                            smpTime = smpRecord.DateTime;
                            if (smpTime >= stressPeriodEndTime)
                            {
                                activeSet = false;
                            }
                            if (!activeSet)
                            {
                                for (int i = 0; i < nper; i++)
                                {
                                    if (smpTime >= stressPeriodStartTimes[i] && smpTime <= stressPeriodEndTimes[i])
                                    {
                                        periodNum = i;
                                        stressPeriodEndTime = stressPeriodEndTimes[i];
                                        break;
                                    }
                                }
                                if (smpRecord.Value != 0.0d)
                                {
                                    if (periodNum >= 0)
                                    {
                                        _active[periodNum, f.Index] = true;
                                        activeSet = true;
                                        noneActive = false;
                                    }
                                }
                            }
                        }
                        ftrCounter++;
                        if (ftrCounter == ftrInc)
                        {
                            bgwProgress++;
                            bgWorker.ReportProgress(bgwProgress);
                            ftrCounter = 0;
                        }
                    }
                }
                OK = true;
            }
            catch (Exception ex)
            {
                OK = false;
            }
            if (OK && noneActive)
            {
                string msg = "Well Package has no active wells.  Is simulation start time correctly assigned?";
                MessageBox.Show(msg);
            }
            return OK;
        }

        public override object Clone()
        {
            WellPackage wellPackage = new WellPackage(this.Parent);
            wellPackage.Npwel = this._npwel;
            wellPackage.Mxactw = this._mxactw;
            wellPackage.CbcFlag = this.CbcFlag;
            wellPackage.NoPrint = this.NoPrint;
            wellPackage.Tag = this.Tag;
            wellPackage.Link = (TagLink)_link.Clone();
            wellPackage.Link.ScenarioElement = wellPackage;
            wellPackage.Link.TreeNode = this.Link.TreeNode;
            wellPackage.Name = this._name;
            wellPackage.Parent = this.Parent;
            wellPackage.Type = this.Type;
            wellPackage.Description = this._description;
            foreach (FeatureSet fs in this.Items)
            {
                FeatureSet newFeatureSet = (FeatureSet)fs.Clone();
                //newFeatureSet.Parent = this;
                wellPackage.Items.Add(newFeatureSet);
            }
            return wellPackage;
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
            toolStripItem.Text = "Exporting Well Package...";
            int nper = DiscretizationFile.getNper();
            int progMax = progressMin + ((progressMax - progressMin) / 4);
            int smpInc;
            int smpCounter = 0;
            int progFsInc = progMax - progressMin;
            int numFS = Items.Count;
            if (numFS > 0)
            {
                progFsInc = (progMax - progressMin) / numFS;
            }
            if (OK)
            {
                // Intersect well points with model-grid polygons 
                // to define row and column of each well.  Iterate
                // through all feature sets included in package.
                // Also populate the SmpSeries belonging to each well.
                string wellID;
                FeatureSet fs;
                List<ScenarioFeature> features = new List<ScenarioFeature>();
                SmpRecord smpRecord;
                int smpCount;
                for (int i = 0; i < Items.Count; i++) // iterate through FeatureSets
                {
                    fs = (FeatureSet)Items[i];
                    PopulateOneFeatureSet(fs, modelGrid, true);
                    fs.SmpSeries = new SmpSeries(fs.TimeSeriesAbsolutePath);
                    smpCount = fs.SmpSeries.Records.Count;
                    smpInc = smpCount / progFsInc;
                    // iterate through SMP records of current FeatureSet
                    for (int j = 0; j < smpCount; j++) 
                    {
                        smpRecord = fs.SmpSeries.Records[j];
                        wellID = smpRecord.ID;
                        features.Clear();
                        // Populate features list with feature(s) in the FeatureSet that have matching wellID
                        fs.AddFeaturesByName(features, wellID); 
                        // Iterate through features and add SMP record to each corresponding feature
                        for (int k = 0; k < features.Count; k++)
                        {
                            features[k].SmpSeries.Records.Add(smpRecord);
                        }
                        smpCounter++;
                        if (smpCounter == smpInc)
                        {
                            bgwProgress++;
                            bgWorker.ReportProgress(bgwProgress);
                            smpCounter = 0;
                        }
                    }
                }
            }
            int progMin = bgwProgress;
            int buildMatrixProgressMax = progMin + Convert.ToInt32(0.25 * (progressMax - progMin));
            if (!BuildActiveWellMatrix(progMin, buildMatrixProgressMax, bgWorker))
            {
                return false;
            }
            progMin = buildMatrixProgressMax;
            bgwProgress = progMin;
            int writeProgressMax = progMin + Convert.ToInt32(0.9 * (progressMax - progMin));
            int progSpInc = nper / (writeProgressMax - progMin);
            int spCounter = 0;
            // Open a StreamWriter
            string filePath = this.NameFileDirectory + Path.DirectorySeparatorChar
                                  + this.GetScenarioID() + Path.DirectorySeparatorChar 
                                  + FileName();
            using (StreamWriter sw = File.CreateText(filePath))
            {
                // Write comments at top of Well input file (Item 0)
                string str;
                str = "# Well file generated by exporting package: \"" + Name + "\"";
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
                        FeatureSet fs = (FeatureSet)item;
                        str = "# Feature Set: \"" + fs.Name + "\"";
                        sw.WriteLine(str);
                        StringUtil.InsertAsModflowCommentLines(sw, fs.Description);
                        sw.WriteLine("# ");
                    }
                }

                // Write Item 2
                _mxactw = CountActive();
                if (freeFormat)
                {
                    str = _mxactw.ToString() + "  " + CbcFlag.ToString();
                }
                else
                {
                    str = ModflowHelpers.ItoS(_mxactw, 10) + ModflowHelpers.ItoS(CbcFlag, 10);
                }
                if (_noPrint)
                {
                    str = str + "  NOPRINT ";
                }
                int posComment = 30;
                str = StringUtil.PadRightToLength(str, posComment);
                str = str + "# Item 2: MXACTW IWELCB";
                sw.WriteLine(str);

                // Define temporal discretization of well stresses
                int itmp;
                DateTime perStartDateTime;
                DateTime perEndDateTime = new DateTime();
                TimeSpan simTime = new TimeSpan(0);
                StressPeriod currentStressPeriod;
                WellCell wellCell;
                double pumpRate;
                perStartDateTime = SimulationStartTime;
                int progInc = (progressMax - buildMatrixProgressMax) / nper;
                // Iterate through stress periods
                for (int iper = 0; iper < nper; iper++)
                {
                    currentStressPeriod = DiscretizationFile.getStressPeriod(iper);
                    perEndDateTime = perStartDateTime + currentStressPeriod.getTimeSpan();
                    itmp = CountActive(iper);
                    if (freeFormat)
                    {
                        str = itmp.ToString() + "  " + _npwel.ToString();
                    }
                    else
                    {
                        str = ModflowHelpers.ItoS(itmp, 10) + ModflowHelpers.ItoS(_npwel, 10);
                    }
                    str = StringUtil.PadRightToLength(str,posComment);
                    str = str + "# Item 5: ITMP NP, Stress period " + (iper+1).ToString();
                    sw.WriteLine(str);
                    //
                    // Iterate through all FeatureSet items in package
                    for (int ifs = 0; ifs < Items.Count; ifs++)
                    {
                        FeatureSet fs = (FeatureSet)Items[ifs];
                        // Iterate through all well cells in FeatureSet
                        for (int iWellCell = 0; iWellCell < fs.Items.Count; iWellCell++)
                        {
                            wellCell = (WellCell)fs.Items[iWellCell];
                            if (Active[iper, wellCell.Index])
                            {
                                pumpRate = wellCell.PumpRate(currentStressPeriod, perStartDateTime, 
                                                         DiscretizationFile.ModflowTimeUnit);
                                if (freeFormat)
                                {
                                    str = wellCell.Layer.ToString() + "  " + wellCell.Row.ToString() + "  " +
                                          wellCell.Column.ToString() + "  " + pumpRate.ToString();
                                    str = StringUtil.PadRightToLength(str, 38);
                                }
                                else
                                {
                                    str = ModflowHelpers.ItoS(wellCell.Layer, 10) + ModflowHelpers.ItoS(wellCell.Row, 10) + ModflowHelpers.ItoS(wellCell.Column, 10)
                                          + ModflowHelpers.DtoS(pumpRate, 10);
                                }
                                if (fs.LabelFeatures == FeatureLabel.FeatureSetOnly || fs.LabelFeatures == FeatureLabel.FeatureSetAndName)
                                {
                                    str = str + "    Feature set: " + fs.Name;
                                }
                                if (fs.LabelFeatures == FeatureLabel.FeatureNameOnly || fs.LabelFeatures == FeatureLabel.FeatureSetAndName)
                                {
                                    str = str + "    Well name: " + wellCell.Name;
                                }
                                sw.WriteLine(str);
                            }
                        }                    
                    }
                    sw.Flush();
                    perStartDateTime = perEndDateTime;
                    spCounter++;
                    if (spCounter == progSpInc)
                    {
                        bgwProgress++;
                        bgWorker.ReportProgress(bgwProgress);
                        spCounter = 0;
                    }
                }
                sw.Dispose();
                toolStripItem.Text = "";
            }
            return OK;
        }

        public override void DescribeToPdf(HPdf.HPdfDoc pdf, ref HPdf.HPdfPoint currentTextPos, 
                                           HPdf.HPdfFont font, HPdf.HPdfFont fontBold, 
                                           float fontHeight, float indent)
        {
            base.DescribeToPdf(pdf, ref currentTextPos, font, fontBold, fontHeight, indent);
        }

        public override string FileName()
        {
            return GetScenarioID() + ".wel";
        }

        public override string FileType()
        {
            return "WEL";
        }

        public override string GetDefaultFeatureSetNodeText()
        {
            return "New Well Set";
        }

        public override bool Import(string filename)
        {
            // Ned TODO: Code WellPackage.Import 
            return base.Import(filename);
        }

        public override void Populate(CellCenteredArealGrid modelGrid)
        {
            FeatureSet featureSet;
            for (int j = 0; j < Items.Count; j++)
            {
                featureSet = (FeatureSet)Items[j];
                PopulateOneFeatureSet(featureSet, modelGrid, false);
            }        
        }

        protected bool PopulateFeatureSet(FeatureSet featureSet, FeatureCollection wellList, 
                                        CellCenteredArealGrid modelGrid, bool showError)
        {
            // Populate a FeatureSet belonging to a WellPackage with well features
            bool OK = false;
            bool topLessThanBottom = false;
            string errorWellName = "";
            if (wellList != null && modelGrid != null)
            {
                featureSet.Clear();
                USGS.Puma.NTS.Geometries.Geometry geometry;
                GridCell gridCell;
                Coordinate location = new Coordinate();
                for (int i = 0; i < wellList.Count; i++)
                {
                    // Ned TODO: Implement test to see if this feature should be 
                    // included in FeatureSet.
                    geometry = (USGS.Puma.NTS.Geometries.Geometry)wellList[i].Geometry;
                    location.X = geometry.Coordinate.X;
                    location.Y = geometry.Coordinate.Y;
                    gridCell = modelGrid.FindRowColumn(location);
                    if (gridCell != null)
                    {
                        switch (featureSet.LayMethod)
                        {
                            case LayerMethod.Uniform:
                                WellCell uWellCell = new WellCell(gridCell);
                                uWellCell.Layer = featureSet.DefaultLayer;
                                uWellCell.Name = (string)wellList[i].Attributes[featureSet.KeyField];
                                uWellCell.InterpretationMethod = featureSet.InterpretationMethod;
                                featureSet.Add(uWellCell);
                                break;
                            case LayerMethod.ByAttribute:
                                WellCell aWellCell = new WellCell(gridCell);
                                aWellCell.Layer = Convert.ToInt32(wellList[i].Attributes[featureSet.LayerAttribute]);
                                aWellCell.Name = (string)wellList[i].Attributes[featureSet.KeyField];
                                aWellCell.InterpretationMethod = featureSet.InterpretationMethod;
                                featureSet.Add(aWellCell);
                                break;
                            case LayerMethod.ByCellTops:
                                // Need method to return number of WellCells connected to Well.
                                // Iterate through cells connected to well, and add a WellCell Feature for each.
                                double openTopElev = Convert.ToDouble(wellList[i].Attributes[featureSet.TopElevationAttribute]);
                                double openBottomElev = Convert.ToDouble(wellList[i].Attributes[featureSet.BottomElevationAttribute]);
                                Well well = new Well(gridCell.Row, gridCell.Column, openTopElev, openBottomElev);
                                int numCells = well.CountCells(DiscretizationFile); // not needed
                                // Ned TODO: Add code to assign layer(s) for LayerMethod.ByCellTops
                                for (int lay = 1; lay <= DiscretizationFile.getNlay(); lay++)
                                {
                                    if (well.CellOpenLength(DiscretizationFile, lay) > 0.0)
                                    {
                                        WellCell newWellCell = new WellCell(lay, gridCell.Row, gridCell.Column);
                                        newWellCell.Name = (string)wellList[i].Attributes[featureSet.KeyField];
                                        newWellCell.FractionOfOpenLength = well.FractionOfOpenLength(DiscretizationFile, lay);
                                        newWellCell.InterpretationMethod = featureSet.InterpretationMethod;
                                        featureSet.Add(newWellCell);
                                    }
                                    else if (openTopElev < openBottomElev)
                                    {
                                        topLessThanBottom = true;
                                        errorWellName = (string)wellList[i].Attributes[featureSet.KeyField];
                                    }
                                }
                                break;
                        }
                        OK = true;
                    }
                }
            }
            if (topLessThanBottom && showError)
            {
                string errmsg = "For feature set '" + featureSet.Name + "', at least one well has top open elevation below bottom open elevation. Example: well '" + errorWellName + "'";
                MessageBox.Show(errmsg);
            }
            return OK;
        }

        protected bool PopulateOneFeatureSet(FeatureSet featureSet, CellCenteredArealGrid modelGrid, bool showError)
        {
            bool OK = false;
            FeatureCollection wellList;
            if (File.Exists(featureSet.ShapefileAbsolutePath))
            {
                wellList = USGS.Puma.IO.EsriShapefileIO.Import(featureSet.ShapefileAbsolutePath);
                if ((modelGrid != null) && (wellList != null))
                {
                    OK = PopulateFeatureSet(featureSet, wellList, modelGrid, showError);
                }
            }
            return OK;
        }

        #endregion Methods
    }
}
