using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using GeoAPI.Geometries;

using ScenarioTools.DataClasses;
using ScenarioTools.ModflowReaders;
using ScenarioTools.Numerical;
using ScenarioTools.Util;

using USGS.Puma.FiniteDifference;
using USGS.Puma.NTS.Features;

namespace ScenarioTools.Scene
{
    public class RchPackage : Package
    {
        #region Fields
        const string spaces50 = "                                                  ";
        private int _nprch = 0;
        private bool _areaFeaturesNeedRepopulating = true;
        #endregion Fields

        /// <summary>
        /// Constructor for class RchPackage
        /// </summary>
        /// <param name="parent"></param>
        public RchPackage(ITaggable parent)
            : base(PackageType.RchType, parent)
        {
            _nprch = 0;
        }

        #region Properties
        public int Nprch
        {
            get
            {
                return _nprch;
            }
            private set
            {
                _nprch = value;
            }
        }
        public bool AreaFeaturesNeedRepopulating
        {
            get
            {
                return _areaFeaturesNeedRepopulating;
            }
            set
            {
                _areaFeaturesNeedRepopulating = value;
            }
        }
        #endregion Properties

        #region Methods

        public override void AssignFrom(ITaggable rchPackageSource)
        {
            if (rchPackageSource is RchPackage)
            {
                RchPackage rchPkg = (RchPackage)rchPackageSource;
                Nprch = rchPkg.Nprch;
                CbcFlag = rchPkg.CbcFlag;
                IndexMasterFeatureSet = rchPkg.IndexMasterFeatureSet;
                // Assign list of FeatureSet objects
                Items.Clear();
                for (int i = 0; i < rchPkg.Items.Count; i++)
                {
                    FeatureSet fs = new FeatureSet(PackageType.RchType, this);
                    fs.AssignFrom(rchPkg.Items[i]);
                    Items.Add(fs);
                }
                _areaFeaturesNeedRepopulating = true;
            }
        }

        public override object Clone()
        {
            RchPackage rchPackage = new RchPackage(this.Parent);
            rchPackage.Nprch = this._nprch;
            rchPackage.CbcFlag = this.CbcFlag;
            rchPackage.IndexMasterFeatureSet = this.IndexMasterFeatureSet;
            rchPackage.Tag = this.Tag;
            rchPackage.Link = (TagLink)_link.Clone();
            rchPackage.Link.ScenarioElement = rchPackage;
            rchPackage.Link.TreeNode = this.Link.TreeNode;
            rchPackage.Name = this._name;
            rchPackage.Parent = this.Parent;
            rchPackage.Type = this.Type;
            rchPackage.Description = this._description;
            // Clone list of FeatureSet objects
            foreach (FeatureSet fs in this.Items)
            {
                FeatureSet newFeatureSet = (FeatureSet)fs.Clone();
                rchPackage.Items.Add(newFeatureSet);
            }
            _areaFeaturesNeedRepopulating = true;
            return rchPackage;
        }

        public override bool Export(CellCenteredArealGrid modelGrid, 
                                    ToolStripItem toolStripItem,
                                    BackgroundWorker bgWorker,
                                    bool freeFormat)
        {
            // Set up reporting of progress
            int progressMin = 0;
            int progressMax = 100;
            int bgwProgress = progressMin;
            bgWorker.ReportProgress(bgwProgress);
            toolStripItem.Text = "Exporting RCH Package...";
            
            // Declare some variables
            double rate;
            double[,] rechargeRates, tempRates;
            bool OK = true;
            int progMax = progressMax;
            int progFsInc = progMax - progressMin;
            int numFS = Items.Count;
            if (numFS > 0)
            {
                progFsInc = (progMax - progressMin) / numFS;
            }
            int nrchop = ((FeatureSet)Items[IndexMasterFeatureSet]).PackageOption;
            int irchcb = this.CbcFlag;
            int unitNumber = NameFileEntry.Unit;
            List<string> lines;
            FeatureSet fs;
            AreaFeature af;
            
            // Initialize the recharge rates arrays
            int nRow = modelGrid.RowCount;
            int nCol = modelGrid.ColumnCount;
            rechargeRates = new double[nRow, nCol]; // all elements set to 0.0d
            tempRates = new double[nRow, nCol];     // all elements set to 0.0d
            
            // Ensure that the list of area features for the package has been populated.
            Populate(modelGrid);
            
            // Open a StreamWriter
            string filePath = this.NameFileDirectory + Path.DirectorySeparatorChar
                                  + this.GetScenarioID() + Path.DirectorySeparatorChar
                                  + FileName();
            using (StreamWriter sw = File.CreateText(filePath))
            {
                // Write comments at top of RCH input file (Item 0)
                string str;
                str = "# RCH file generated by exporting package: \"" + Name + "\"";
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
                //
                // Current version assumes zero RCH parameters.
                // When a RCH file can be imported, will need to 
                // add code to handle parameters. -- No, I think it would
                // make more sense not to support RCH parameters.  So just
                // assume Export will replace any existing RCH input file.
                //
                // Define temporal discretization of well stresses, 
                // including start time of first stress period.
                DateTime perStartDateTime;
                DateTime perEndDateTime = new DateTime();
                TimeSpan simTime = new TimeSpan(0);
                StressPeriod currentStressPeriod;
                perStartDateTime = SimulationStartTime;
                int nper = DiscretizationFile.getNper();
                int perCounter = 0;
                int perInc = nper / progFsInc;

                // Write item 2 (Omit item 1 because parameters are not supported)
                if (freeFormat)
                {
                    str = "  " + nrchop.ToString() + "  " + irchcb.ToString() + "    Item 2: NRCHOP IRCHCB";
                }
                else
                {
                    str = ModflowHelpers.ItoS(nrchop, 10) + ModflowHelpers.ItoS(irchcb, 10) + "    Item 2: NRCHOP IRCHCB";
                }
                sw.WriteLine(str);

                // Set flag indicating array of layer numbers is needed
                int inirch = 0;
                if (nrchop == 2)
                {
                    inirch = 1;
                }
                // Loop through stress periods
                for (int iper = 0; iper < nper; iper++)
                {
                    // Set contents of recharge rates array to zero
                    ArrayOps.SetArrayElements(ref rechargeRates, 0.0d);
                    
                    // Define stress-period end time
                    currentStressPeriod = DiscretizationFile.getStressPeriod(iper);
                    perEndDateTime = perStartDateTime + currentStressPeriod.getTimeSpan();
                    
                    // Iterate through all AreaFeature objects in package, and sum contributions from them
                    for (int ifs = 0; ifs < Items.Count; ifs++)
                    {
                        fs = (FeatureSet)Items[ifs];
                        for (int iArea = 0; iArea < Items[ifs].Items.Count; iArea++)
                        {
                            af = (AreaFeature)Items[ifs].Items[iArea];
                            // Assign recharge rates associated with current AreaFeature
                            rate = StressPeriodHelper.GetTimeAveragedValueForStressPeriod(currentStressPeriod, perStartDateTime,
                                af.SmpSeries, DiscretizationFile.ModflowTimeUnit, fs.InterpretationMethod);
                            if (!double.IsNaN(rate))
                            {
                                ArrayOps.MultiplyArrayByScalar(ref tempRates, af.PolygonGridder.MultiplierMatrix, rate);

                                // Add these recharge rates to the main recharge rates array
                                ArrayOps.AddArrays(ref rechargeRates, tempRates);
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }

                    // Write item 5 for current stress period (INRECH INIRCH)
                    // INRECH is always 1 because the RECH array is provided every stress period.
                    if (freeFormat)
                    {
                        str = "  1  " + inirch.ToString() + "    Item 5: INRECH INIRCH, Stress period " + (iper + 1).ToString();
                    }
                    else
                    {
                        str = "         1" + ModflowHelpers.ItoS(inirch, 10) + "    Item 5: INRECH INIRCH, Stress period " + (iper + 1).ToString();
                    }
                    sw.WriteLine(str);

                    // Write item 6 for current stress period (RECH array of recharge rates)
                    lines = ModflowHelpers.CreateInputForU2drel(rechargeRates, freeFormat, unitNumber);
                    for (int i = 0; i < lines.Count; i++)
                    {
                        sw.WriteLine(lines[i]);
                    }

                    // If this is stress period 1 and NRCHOP is 2, write item 8 (IRCH array containing layer numbers)
                    if (iper == 0 && nrchop == 2)
                    {
                        // Initialize the IRCH (layer indicator) array
                        int[,] irch = GetIndexArray();

                        // Write RCH item 8 (IRCH array containing layer numbers)
                        lines = ModflowHelpers.CreateInputForU2dint(irch, freeFormat, unitNumber);
                        for (int i = 0; i < lines.Count; i++)
                        {
                            sw.WriteLine(lines[i]);
                        }

                        // Turn off flag that causes IRCH to be read
                        inirch = -1;
                    }

                    // Report progress
                    perCounter++;
                    if (perCounter == perInc)
                    {
                        bgwProgress++;
                        bgWorker.ReportProgress(bgwProgress);
                        perCounter = 0;
                    }

                    // Advance time for next stress period
                    perStartDateTime = perEndDateTime;
                    sw.Flush();
                }
                sw.Dispose();
            }
            return OK;
        }

        /// <summary>
        /// Populate an int array of layer indices that determine the layer 
        /// where recharge is simulated at each (row, column) location.
        /// </summary>
        private int[,] GetIndexArray()
        {
            // Populate an int array, assigned on the basis of the LayerAttribute value for the 
            // layer-number (weighted by polygon area) that occupies the largest part of each cell.

            // Declare some variables
            int nLay = DiscretizationFile.getNlay();
            int nRow = DiscretizationFile.getNrow();
            int nCol = DiscretizationFile.getNcol();
            int i, j, k, m, n;
            int[,] irch = new int[nRow, nCol];
            double[,,] weights = new double[nLay, nRow, nCol];
            double[,] maxWeights = new double[nRow, nCol];
            string layerAttribute = "";
            FeatureSet fs;
            AreaFeature af;

            // Assign default layer = 1
            ArrayOps.SetArrayElements(ref irch, 1);

            // Iterate through all the AreaFeatures in all the
            // FeatureSets, and populate the weights array.
            for (m = 0; m < Items.Count; m++)
            {
                if (Items[m] is FeatureSet)
                {
                    fs = (FeatureSet)Items[m];
                    layerAttribute = fs.LayerAttribute;
                }
                for (n = 0; n < Items[m].Items.Count; n++)
                {
                    if (Items[m].Items[n] is AreaFeature)
                    {
                        af = (AreaFeature)Items[m].Items[n];

                        // Get layer number for this AreaFeature
                        // Here, k is one-based
                        k = Convert.ToInt32(af.Attributes[layerAttribute]);
                        if (k > 0 && k <= nLay)
                        {
                            for (i = 0; i < nRow; i++)
                            {
                                for (j = 0; j < nCol; j++)
                                {
                                    weights[k-1, i, j] = weights[k-1, i, j] + af.PolygonGridder.MultiplierMatrix[i, j];
                                }
                            }
                        }
                    }
                }
            }

            // Iterate through the weights array, populate the maxWeights array, and assign irch
            for (i = 0; i < nRow; i++)
            {
                for (j = 0; j < nCol; j++)
                {
                    // Here, k is one-based
                    for (k = 1; k <= nLay; k++)
                    {
                        if (weights[k - 1, i, j] > maxWeights[i, j])
                        {
                            maxWeights[i, j] = weights[k - 1, i, j];
                            irch[i, j] = k;
                        }
                    }
                }
            }
            return irch;
        }

        public override string FileName()
        {
            return GetScenarioID() + ".rch";
        }

        public override string FileType()
        {
            return "RCH";
        }

        public override string GetDefaultFeatureSetNodeText()
        {
            return "New Recharge Set";
        }

        /// <summary>
        /// Populate all FeatureSet.Items with AreaFeatures from shapefile and
        /// assign SmpSeries filtered by ID from time-series file.
        /// </summary>
        /// <param name="modelGrid"></param>
        public override void Populate(CellCenteredArealGrid modelGrid)
        {
            if (_areaFeaturesNeedRepopulating)
            {
                FeatureSet fs;
                string keyField;
                string id;
                if (Items.Count > 0)
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        fs = (FeatureSet)Items[i];
                        ClearAndPopulateOneFeatureSet(fs, modelGrid);
                        keyField = fs.KeyField;
                        // Populate fs.Items with AreaFeatures from shapefile
                        // Populate the SmpSeries for each AreaFeature in the FeatureSet
                        for (int j = 0; j < fs.Items.Count; j++)
                        {
                            if (fs.Items[j] is AreaFeature)
                            {
                                AreaFeature af = (AreaFeature)fs.Items[j];
                                id = af.Attributes[keyField].ToString();
                                af.SmpSeries = fs.SmpSeries.FilterById(id);
                            }
                        }
                    }
                    _areaFeaturesNeedRepopulating = false;
                }
            }
        }

        protected bool ClearAndPopulateOneFeatureSet(FeatureSet featureSet, CellCenteredArealGrid modelGrid)
        {
            bool OK = false;
            featureSet.Clear();
            if (File.Exists(featureSet.TimeSeriesAbsolutePath))
            {
                featureSet.SmpSeries = new SmpSeries(featureSet.TimeSeriesAbsolutePath);
                FeatureCollection featureList;
                if (File.Exists(featureSet.ShapefileAbsolutePath))
                {
                    featureList = USGS.Puma.IO.EsriShapefileIO.Import(featureSet.ShapefileAbsolutePath);
                    if ((modelGrid != null) && (featureList != null))
                    {
                        OK = PopulateFeatureSet(featureSet, featureList, modelGrid);
                    }
                }
            }
            return OK;
        }

        protected bool PopulateFeatureSet(FeatureSet featureSet, FeatureCollection featureList,
                                        CellCenteredArealGrid modelGrid)
        {
            // Populate a FeatureSet belonging to a RchPackage with polygons from the shapefile.
            // A FeatureSet can contain multiple polygons.
            // Each RCH feature is defined by one polygon and can be considered to have an associated 
            // multiplier matrix, which is constant through a simulation.  
            bool OK = false;
            string keyField = featureSet.KeyField;
            if (featureList != null && modelGrid != null)
            {
                USGS.Puma.NTS.Geometries.Geometry geometry;
                IPolygon polygon;
                IAttributesTable attributesTable;

                // RCH needs layer to which recharge is to be applied; for NRCHOP = 2, an integer
                // array is needed to provide layer to which recharge is to be applied in each 
                // stack of cells.  The integer array can be generated from the set of polygons in
                // the shapefile associated with the FeatureSet...the layer number would need to be
                // stored in an integer or float attribute.  The FeatureSetForm for RCH will need
                // a drop-down list to set NRCHOP, and when NRCHOP = 2, another drop-down list 
                // would be enabled, where the user would select the attribute containing the 
                // layer index for the feature.  Where a cell contains parts of multiple polygons,
                // the area-weighted multiplier matrices would be used to find the polygon with
                // the largest weight to determine the layer...the recharge rate in this case would
                // still be the area-weighted sum of contributions from all polygons in the cell.

                for (int m = 0; m < featureList.Count; m++)
                {
                    geometry = (USGS.Puma.NTS.Geometries.Geometry)featureList[m].Geometry;
                    attributesTable = (IAttributesTable)featureList[m].Attributes;
                    if (geometry is IPolygon)
                    {
                        polygon = (IPolygon)geometry;
                        // The following constructor causes multiplier matrix to be populated
                        AreaFeature newAreaFeature = new AreaFeature(PackageType.RchType, 
                            polygon, attributesTable, modelGrid); 
                        string id = attributesTable[keyField].ToString();
                        newAreaFeature.SmpSeries = featureSet.SmpSeries.FilterById(id);
                        newAreaFeature.Parent = featureSet;
                        featureSet.Items.Add(newAreaFeature);
                    }
                }
            }
            return OK;
        }
        #endregion Methods
    }
}
