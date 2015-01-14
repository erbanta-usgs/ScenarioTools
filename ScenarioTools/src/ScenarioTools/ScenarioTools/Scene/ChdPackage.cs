using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using ScenarioTools.DataClasses;
using ScenarioTools.ModflowReaders;
using ScenarioTools.Util;
using USGS.Puma.FiniteDifference;
using USGS.Puma.NTS.Algorithm;
using USGS.Puma.NTS.Features;
using USGS.Puma.NTS.Geometries;

namespace ScenarioTools.Scene
{
    public class ChdPackage : Package
    {
        #region Fields

        private int _mxactc;
        private int _npchd = 0;
        private bool _noPrint;
        const string spaces50 = "                                                  ";

        #endregion Fields

        /// <summary>
        /// Constructor for class ChdPackage
        /// </summary>
        /// <param name="parent"></param>
        public ChdPackage(ITaggable parent)
            : base(PackageType.ChdType, parent)
        {
            _mxactc = 0;
            _npchd = 0;
            _noPrint = false;
        }

        #region Properties

        public int Mxactc
        {
            get
            {
                return _mxactc;
            }
            set
            {
                _mxactc = value;
            }
        }

        public int Npchd
        {
            get
            {
                return _npchd;
            }
            set
            {
                _npchd = value;
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

        private bool AllocateChdMatrix(CellCenteredArealGrid modelGrid)
        {
            bool OK = true;
            // For CHD, active matrix is dimensioned [nrow,ncol] and all active locations 
            // are assumed to be constant-head cells in all stress periods.
            int nRow = DiscretizationFile.getNrow();
            int nCol = DiscretizationFile.getNcol();
            int indx = -1;
            bool needAllocate = true;
            if (_active != null)
            {
                int dim0 = _active.GetLength(0);
                int dim1 = _active.GetLength(1);
                if ((dim0 == 1) && dim1 == (nRow * nCol))
                {
                    needAllocate = false;
                }
            }
            if (needAllocate)
            {
                _active = new bool[1, nRow * nCol];
            }
            for (int i = 0; i < nRow; i++)
            {
                for (int j = 0; j < nCol; j++)
                {
                    indx++;
                    _active[0, indx] = false;
                }
            }
            return OK;
        }

        public override void AssignFrom(ITaggable chdPackageSource)
        {
            if (chdPackageSource is ChdPackage)
            {
                ChdPackage chdPkg = (ChdPackage)chdPackageSource;
                Mxactc = chdPkg.Mxactc;
                Npchd = chdPkg.Npchd;
                NoPrint = chdPkg.NoPrint;
                Items.Clear();
                for (int i = 0; i < chdPkg.Items.Count; i++)
                {
                    FeatureSet fs = new FeatureSet(PackageType.ChdType, this);
                    fs.AssignFrom(chdPkg.Items[i]);
                    Items.Add(fs);
                }
            }
        }

        public override object Clone()
        {
            ChdPackage chdPackage = new ChdPackage(this.Parent);
            chdPackage.Mxactc = this._mxactc;
            chdPackage.Npchd = this._npchd;
            chdPackage.NoPrint = this._noPrint;
            chdPackage.Tag = this.Tag;
            chdPackage.Link = (TagLink)_link.Clone();
            chdPackage.Link.ScenarioElement = chdPackage;
            chdPackage.Link.TreeNode = this.Link.TreeNode;
            chdPackage.Name = this._name;
            chdPackage.Parent = this.Parent;
            chdPackage.Type = this.Type;
            chdPackage.Description = this._description;
            foreach (FeatureSet fs in this.Items)
            {
                FeatureSet newFeatureSet = (FeatureSet)fs.Clone();
                //newFeatureSet.Parent = this;
                chdPackage.Items.Add(newFeatureSet);
            }
            return chdPackage;
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
            toolStripItem.Text = "Exporting CHD Package...";
            // Allocate and initialize _active matrix
            AllocateChdMatrix(modelGrid);
            int progMax = progressMax;
            int progFsInc = progMax - progressMin;
            int numFS = Items.Count;
            if (numFS > 0)
            {
                progFsInc = (progMax - progressMin) / numFS;
            }
            FeatureSet fs;
            // Intersect model-cell centroids with shapefile polygon(s)
            // to find CHD cells.  Iterate through all feature sets included 
            // in package.  
            // Populate the SmpSeries.
            for (int i = 0; i < Items.Count; i++)
            {
                fs = (FeatureSet)Items[i];
                PopulateOneFeatureSet(fs, modelGrid);
                fs.SmpSeries = new SmpSeries(fs.TimeSeriesAbsolutePath);
            }
            // Open a StreamWriter
            string filePath = this.NameFileDirectory + Path.DirectorySeparatorChar
                                  + this.GetScenarioID() + Path.DirectorySeparatorChar
                                  + FileName();
            using (StreamWriter sw = File.CreateText(filePath))
            {
                // Write comments at top of CHD input file (Item 0)
                string str;
                str = "# CHD file generated by exporting package: \"" + Name + "\"";
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
                _mxactc = CountActive();
                if (freeFormat)
                {
                    str = _mxactc.ToString();
                }
                else
                {
                    str = ModflowHelpers.ItoS(_mxactc, 10);
                }
                if (_noPrint)
                {
                    str = str + "  NOPRINT ";
                }
                int posComment = 25;
                str = StringUtil.PadRightToLength(str, posComment);
                str = str + "# Item 2: MXACTC";
                sw.WriteLine(str);
                // Define temporal discretization of stresses
                // First, iterate through stress periods
                int itmp;
                DateTime perStartDateTime;
                DateTime perEndDateTime = new DateTime();
                TimeSpan simTime = new TimeSpan(0);
                StressPeriod currentStressPeriod;
                ChdCell chdCell;
                double startHead = 0.0d, endHead = 0.0d;
                perStartDateTime = SimulationStartTime;
                int nper = DiscretizationFile.getNper();
                int perCounter = 0;
                int perInc = nper / progFsInc;
                for (int iper = 0; iper < nper; iper++)
                {
                    currentStressPeriod = DiscretizationFile.getStressPeriod(iper);
                    perEndDateTime = perStartDateTime + currentStressPeriod.getTimeSpan();
                    itmp = CountActive(0);
                    if (freeFormat)
                    {
                        str = itmp.ToString() + "  " + _npchd.ToString();
                    }
                    else
                    {
                        str = ModflowHelpers.ItoS(itmp, 10) + ModflowHelpers.ItoS(_npchd, 10);
                    }
                    str = StringUtil.PadRightToLength(str, posComment);
                    str = str + "# Item 5: ITMP NP, Stress period " + (iper + 1).ToString();
                    sw.WriteLine(str);
                    // Iterate through all FeatureSet items in package
                    for (int ifs = 0; ifs < Items.Count; ifs++)
                    {
                        fs = (FeatureSet)Items[ifs];
                        StartEndHeads(fs, currentStressPeriod, perStartDateTime, 
                                      DiscretizationFile.ModflowTimeUnit, ref startHead, ref endHead);
                        // Iterate through all CHD cells in FeatureSet
                        for (int icel = 0; icel < fs.Items.Count; icel++)
                        {
                            chdCell = (ChdCell)fs.Items[icel];
                            if (_active[0, chdCell.Index])
                            {
                                if (freeFormat)
                                {
                                    str = chdCell.Layer.ToString() + "  " + chdCell.Row.ToString() + "  " +
                                          chdCell.Column.ToString() + "  " + startHead.ToString() + "  " + endHead.ToString();
                                    str = StringUtil.PadRightToLength(str, 44);
                                }
                                else
                                {
                                    str = ModflowHelpers.ItoS(chdCell.Layer, 10) + ModflowHelpers.ItoS(chdCell.Row, 10) + ModflowHelpers.ItoS(chdCell.Column, 10)
                                          + ModflowHelpers.DtoS(startHead, 10) + ModflowHelpers.DtoS(endHead, 10);
                                }
                                if (fs.LabelFeatures == FeatureLabel.FeatureSetOnly || fs.LabelFeatures == FeatureLabel.FeatureSetAndName)
                                {
                                    str = str + "    Feature set: " + fs.Name;
                                }
                                if (fs.LabelFeatures == FeatureLabel.FeatureNameOnly || fs.LabelFeatures == FeatureLabel.FeatureSetAndName)
                                {
                                    str = str + "    CHD feature name: " + chdCell.Name;
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
            return GetScenarioID() + ".chd";
        }

        public override string FileType()
        {
            return "CHD";
        }

        public override string GetDefaultFeatureSetNodeText()
        {
            return "New CHD Set";
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
            // Populate a FeatureSet belonging to a ChdPackage with polygons
            bool OK = false;
            if (featureList != null && modelGrid != null)
            {
                AllocateChdMatrix(modelGrid);
                featureSet.Clear();
                USGS.Puma.NTS.Geometries.Geometry geometry;
                GridCell gridCell;
                Coordinate location = new Coordinate();
                int nRow = DiscretizationFile.getNrow();
                int nCol = DiscretizationFile.getNcol();

                int layer;
                double openTopElev;
                double openBottomElev;
                double cellTopElev;
                double cellBottomElev;
                double cellHeight;
                double maxOpenHeightInCell;
                double openHeightInCell;
                double openCellTop;
                double openCellBottom;

                for (int m = 0; m < featureList.Count; m++)
                {
                    geometry = (USGS.Puma.NTS.Geometries.Geometry)featureList[m].Geometry;
                    // Iterate through model cells
                    int indx = -1;
                    for (int i = 0; i < nRow; i++)
                    {
                        for (int j = 0; j < nCol; j++)
                        {
                            indx++;
                            gridCell = new GridCell(i + 1, j + 1);
                            location = (Coordinate)modelGrid.GetNodePoint(gridCell);
                            if (SimplePointInAreaLocator.ContainsPointInPolygon(location, (Polygon)geometry))
                            {
                                if (!_active[0, indx])
                                {
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
                                    _active[0, indx] = true;
                                    ChdCell chdCell = new ChdCell(gridCell);
                                    // Default layer = 1
                                    chdCell.Layer = 1;
                                    if (layer > 0 && layer <= DiscretizationFile.getNlay())
                                    {
                                        chdCell.Layer = layer;
                                    }
                                    chdCell.Index = indx;
                                    chdCell.Parent = this;
                                    chdCell.Name = (string)featureList[m].Attributes[featureSet.KeyField];
                                    featureSet.Add(chdCell);
                                }
                            }
                        }
                    }
                }
            }
            return OK;
        }
        /// <summary>
        /// Returns start and end head for specified feature set and stress period
        /// </summary>
        /// <param name="stressPeriod"></param>
        /// <param name="periodStartDateTime"></param>
        public void StartEndHeads(FeatureSet fs, StressPeriod stressPeriod, DateTime perStartDateTime,
                                  ModflowTimeUnit modflowTimeUnit, 
                                  ref double startHead, ref double endHead)
        {
            startHead = -9999.0d;
            endHead = -9999.0d;
            try
            {
                int indxRecLastPreceding = -1;
                int indxFirstRecInStressPeriod = -1;
                int indxLastRecInStressPeriod = -1;
                int indxFirstRecAfterStressPeriod = -1;
                int countRecsInStressPeriod = 0;
                DateTime perEndDateTime = perStartDateTime + stressPeriod.getTimeSpan();
                DateTime currentDateTime;
                // Find index of last SMP record preceding stress period start, 
                // index of first record in stress period, and index of last
                // record in stress period.
                // 
                if (fs.SmpSeries.Records.Count > 0)
                {
                    for (int i = 0; i < fs.SmpSeries.Records.Count; i++)
                    {
                        currentDateTime = fs.SmpSeries.Records[i].DateTime;
                        if (currentDateTime < perStartDateTime)
                        {
                            indxRecLastPreceding = i;
                        }
                        if (indxFirstRecInStressPeriod == -1)
                        {
                            if (currentDateTime >= perStartDateTime)
                            {
                                indxFirstRecInStressPeriod = i;
                            }
                        }
                        if (currentDateTime <= perEndDateTime)
                        {
                            indxLastRecInStressPeriod = i;
                            if (currentDateTime >= perStartDateTime)
                            {
                                countRecsInStressPeriod++;
                            }
                        }
                        if (currentDateTime > perEndDateTime)
                        {
                            indxFirstRecAfterStressPeriod = i;
                            break;
                        }
                    }
                }

                // Note: Stepwise interpretation does not make sense for CHD Package because CHD implements 
                // linear interpolation between beginning and end of stress period anyway.
                // So do not implement code for TimeSeriesInterpretationMethod.Stepwise.  The following
                // code implements TimeSeriesInterpretationMethod.Piecewise.
                double valueStart, valueEnd;
                DateTime dtPreceding, dtFollowing;
                double valuePreceding, valueFollowing;
                //
                if (indxRecLastPreceding > -1)
                {
                    dtPreceding = fs.SmpSeries.Records[indxRecLastPreceding].DateTime;
                    valuePreceding = fs.SmpSeries.Records[indxRecLastPreceding].Value;
                }
                else
                {
                    dtPreceding = new DateTime();
                    valuePreceding = 0.0d;
                }
                valueEnd = fs.SmpSeries.Records[indxFirstRecInStressPeriod].Value;
                valueStart = TimeHelper.Interpolate(dtPreceding,
                    fs.SmpSeries.Records[indxFirstRecInStressPeriod].DateTime, perStartDateTime,
                    valuePreceding, valueEnd);
                startHead = valueStart;
                //
                if (indxFirstRecAfterStressPeriod > indxLastRecInStressPeriod)
                {
                    dtFollowing = fs.SmpSeries.Records[indxFirstRecAfterStressPeriod].DateTime;
                    valueFollowing = fs.SmpSeries.Records[indxFirstRecAfterStressPeriod].Value;
                }
                else
                {
                    dtFollowing = new DateTime();
                    valueFollowing = 0.0d;
                }
                valueStart = fs.SmpSeries.Records[indxLastRecInStressPeriod].Value;
                valueEnd = TimeHelper.Interpolate(fs.SmpSeries.Records[indxLastRecInStressPeriod].DateTime,
                    dtFollowing, perEndDateTime, valueStart, valueFollowing);
                endHead = valueEnd;
            }
            catch
            {
            }
        }

        #endregion Methods

    }
}
