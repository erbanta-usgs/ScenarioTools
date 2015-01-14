using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using ScenarioTools.DataClasses;
using ScenarioTools.Geometry;
using ScenarioTools.ModflowReaders;
using ScenarioTools.Util;
using ScenarioTools.Xml;

namespace ScenarioTools.Data_Providers
{
    public class DataProviderCbbGroup : IDataProvider
    {
        #region Constants
        private const string XML_KEY_FILE = "file";
        private const string XML_KEY_OBJECT_KEY = "key";
        private const string XML_KEY_DATA_IDENTIFIER = "identifier";
        private const string XML_KEY_START_LAYER = "layerStart";
        private const string XML_KEY_END_LAYER = "layerEnd";
        #endregion Constants

        #region Fields
        private string _cbbFile;
        private string _dataIdentifier;
        private int _startLayer;
        private int _endLayer;
        private CellGroupProvider[] _cellGroupProviders;
        private string[] _datasetResultMessage;
        private TimeSeries _dataset;
        private bool _datasetNeedsRefresh;
        private bool _currentDatasetObtainable;
        private long _key;
        private DateTime _sourceFileModificationTime;
        #endregion Fields

        #region Properties
        public long Key
        {
            get
            {
                return _key;
            }
        }
        public DateTime DataModificationTime { get; set; }
        public bool ConvertFlowToFlux { get; set; }
        public string CbbFile
        {
            get
            {
                if (_cbbFile == null)
                {
                    _cbbFile = "";
                }
                return _cbbFile;
            }
            set
            {
                _cbbFile = value;
            }
        }
        public string DataIdentifier
        {
            get
            {
                return _dataIdentifier;
            }
            set
            {
                _dataIdentifier = value;
            }
        }
        public CellGroupProvider[] CellGroupProviders
        {
            get
            {
                if (_cellGroupProviders == null)
                {
                    _cellGroupProviders = new CellGroupProvider[0];
                }
                return _cellGroupProviders;
            }
            set
            {
                _cellGroupProviders = value;
            }
        }
        public int StartLayer
        {
            get
            {
                if (_startLayer < 1)
                {
                    _startLayer = 1;
                }
                return _startLayer;
            }
            set
            {
                _startLayer = value;
            }
        }
        public int EndLayer
        {
            get
            {
                if (_endLayer < StartLayer)
                {
                    _endLayer = StartLayer;
                }
                return _endLayer;
            }
            set
            {
                _endLayer = value;
            }
        }
        #endregion Properties

        #region Constructor
        public DataProviderCbbGroup()
        {
            _dataset = null;
            _datasetNeedsRefresh = true;
            _currentDatasetObtainable = true;
            _key = WorkspaceUtil.GetUniqueIdentifier();

            DataModificationTime = DateTime.MinValue;
        }
        #endregion Constructor

        #region IDataProvider Members
        public XmlNode GetXmlNode(XmlDocument document, string targetFileName)
        {
            // Make the element that represents this object.
            XmlElement element = document.CreateElement("DataProviderCbbGroup");

            // Set the attributes.
            XmlUtil.FrugalSetAttribute(element, XML_KEY_OBJECT_KEY, _key + "", "");
            string dir = Path.GetDirectoryName(targetFileName);
            string relPath = FileUtil.Absolute2Relative(CbbFile, dir);
            XmlUtil.FrugalSetAttribute(element, XML_KEY_FILE, relPath, "");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_DATA_IDENTIFIER, _dataIdentifier + "", "");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_START_LAYER, _startLayer + "", "");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_END_LAYER, _endLayer + "", "");

            // Add the cell group nodes.
            for (int i = 0; i < CellGroupProviders.Length; i++)
            {
                element.AppendChild(CellGroupProviders[i].GetXmlNode(document,targetFileName));
            }

            // Return the result.
            return element;
        }
        public void InitFromXml(XmlElement element, string sourceFileName)
        {
            // Get the attributes from the node.
            _key = XmlUtil.SafeGetLongAttribute(element, XML_KEY_OBJECT_KEY, WorkspaceUtil.GetUniqueIdentifier());
            string relPath = XmlUtil.SafeGetStringAttribute(element, XML_KEY_FILE, "");
            string dir = Path.GetDirectoryName(sourceFileName);
            _cbbFile = FileUtil.Relative2Absolute(relPath, dir);
            _dataIdentifier = XmlUtil.SafeGetStringAttribute(element, XML_KEY_DATA_IDENTIFIER, "");
            _startLayer = XmlUtil.SafeGetIntAttribute(element, XML_KEY_START_LAYER, 1);
            _endLayer = XmlUtil.SafeGetIntAttribute(element, XML_KEY_END_LAYER, 1);
            _sourceFileModificationTime = File.GetLastWriteTime(_cbbFile);

            // Get the groups under the node.
            List<CellGroupProvider> cellGroupList = new List<CellGroupProvider>();
            foreach (XmlElement child in element.ChildNodes)
            {
                CellGroupProvider group = CellGroupProvider.FromXmlElement(child);
                if (group != null)
                {
                    cellGroupList.Add(group);
                }
            }
            _cellGroupProviders = cellGroupList.ToArray();

            // Register the cell groups with the workspace.
            GroupHelper.RegisterGroups(_cellGroupProviders);

            //Console.WriteLine("Got " + cellGroups.Length + " groups from XML file");

            // Set the heads array to null.
            _dataset = null;
        }
        public void InvalidateDataset()
        {
            // Mark that the dataset needs to be refreshed.
            _datasetNeedsRefresh = true;

            // Mark that the current dataset is obtainable. Until we know otherwise, we will assume that it is.
            _currentDatasetObtainable = true;
        }
        public object GetData(out int datasetStatus)
        {
            // Do a quick check of the file. If it does not exist, mark the dataset as unobtainable.
            if (_currentDatasetObtainable || _datasetNeedsRefresh)
            {
                if (!File.Exists(this.CbbFile))
                {
                    _currentDatasetObtainable = false;
                    _datasetNeedsRefresh = false;
                }
            }

            // If the source file for the dataset is newer than the dataset
            // modification time, the dataset needs to be refreshed.
            if (_sourceFileModificationTime > DataModificationTime)
            {
                _datasetNeedsRefresh = true;
            }

            // Set the data status.
            datasetStatus = DataStatus.GetDataStatus(_currentDatasetObtainable, _dataset != null, _datasetNeedsRefresh);

            // Return the dataset.
            return _dataset;
        }
        public int GetDataStatus()
        {
            int datasetStatus;
            GetData(out datasetStatus);
            if (_sourceFileModificationTime <= DataModificationTime)
            {
                return datasetStatus;
            }
            else
            {
                return DataStatus.DATASET_NEEDS_REFRESH;
            }
        }
        public object GetDataSynchronous()
        {
            // If the dataset needs to be refreshed, try to read the dataset.
            if (_datasetNeedsRefresh)
            {
                // Get the combined group of cells.
                Point[] combinedCells = CellGroupProvider.GetCombinedCells(CellGroupProviders);

                // If no cell groups have been defined, get data for all cells in specified layers
                bool getAllCells = (CellGroupProviders.Length == 0);

                // Read the dataset from the specified file.
                TimeSeries newDataset = CbbReader.GetTimeSeriesGroupFromCbb(CbbFile, DataIdentifier, combinedCells, StartLayer, EndLayer, 
                    out _datasetResultMessage, ScenarioTools.GlobalStaticVariables.GlobalTemporalReference, getAllCells);

                // If the new dataset is null, mark that the current dataset is unobtainable. Do not overwrite the current cache.
                if (newDataset == null)
                {
                    _currentDatasetObtainable = false;
                    MessageBox.Show(_datasetResultMessage[0]);
                }

                // Otherwise, store the dataset and mark that the current dataset is obtainable.
                else
                {
                    _dataset = newDataset;
                    DataModificationTime = File.GetLastWriteTime(CbbFile);
                    _currentDatasetObtainable = true;
                }

                // Mark that the dataset does not need to be refreshed.
                _datasetNeedsRefresh = false;
            }

            // Return the dataset.
            return _dataset;
        }
        public void LoadCacheFromStream(Stream stream, DateTime streamDateTime)
        {
            // Load the cache.
            _dataset = WorkspaceUtil.GetCachedTimeSeries(stream);

            // If the dataset is no longer null, switch off the update flag.
            if (_dataset != null)
            {
                _datasetNeedsRefresh = false;
                DataModificationTime = streamDateTime;
            }
        }
        public bool SupportsDataConsumerType(DataConsumerTypeEnum dataConsumerType)
        {
            if (dataConsumerType == DataConsumerTypeEnum.Chart)
            {
                return true;
            }
            return false;
        }
        public string[] GetResultMessage()
        {
            // Get the dataset status.
            int datasetStatus;
            GetData(out datasetStatus);

            if (datasetStatus == DataStatus.DATASET_NEEDS_REFRESH)
            {
                return new string[] { "Data set refreshing" };
            }
            else if (datasetStatus == DataStatus.DATA_UNAVAILABLE_CACHE_MISSING)
            {
                return new string[] { "Invalid data set" };
            }
            else
            {
                return new string[] { "Valid data set" };
            }
        }
        #endregion IDataProvider Members

        #region IHasUniqueIdentifier Members
        public long GetUniqueIdentifier()
        {
            return _key;
        }
        public void UpdateUniqueIdentifier()
        {
            _key = WorkspaceUtil.GetUniqueIdentifier();
        }
        public void ValidateUniqueIdentifier(List<long> uniqueIdentifiers)
        {
            // If this object's identifier is already in the list, update the identifier.
            if (uniqueIdentifiers.Contains(GetUniqueIdentifier()))
            {
                UpdateUniqueIdentifier();
            }

            // Add this object's identifier to the list.
            uniqueIdentifiers.Add(GetUniqueIdentifier());
        }
        public string[] GetMetadata()
        {
            try
            {
                FileInfo fileInfo = new FileInfo(this.CbbFile);
                string fileSize = FileUtil.GetFileLengthString(fileInfo.Length);

                return new string[] {
                "Data Category: " + this.ToString(),
                "Source File: " + this.CbbFile,
                "Creation Date: " + fileInfo.CreationTime,
                "Modification Date: " + fileInfo.LastWriteTime,
                "File Size: " + fileSize
            };
            }
            catch
            {
                return new string[0];
            }
        }
        #endregion IHasUniqueIdentifier Members

        #region IDeepCloneable Members
        public object DeepClone()
        {
            DataProviderCbbGroup dataProvCbbGroupCopy = new DataProviderCbbGroup();
            dataProvCbbGroupCopy._cbbFile = this._cbbFile;
            dataProvCbbGroupCopy._dataIdentifier = this._dataIdentifier;
            dataProvCbbGroupCopy._endLayer = this._endLayer;
            dataProvCbbGroupCopy._sourceFileModificationTime = new DateTime(_sourceFileModificationTime.Ticks);
            dataProvCbbGroupCopy._startLayer = this._startLayer;
            dataProvCbbGroupCopy.ConvertFlowToFlux = this.ConvertFlowToFlux;
            dataProvCbbGroupCopy.DataModificationTime = new DateTime(this.DataModificationTime.Ticks);
            int n = this._cellGroupProviders.GetLength(0);
            dataProvCbbGroupCopy._cellGroupProviders = new CellGroupProvider[n];
            for (int i = 0; i < n; i++)
            {
                dataProvCbbGroupCopy._cellGroupProviders[i] = (CellGroupProvider)_cellGroupProviders[i].DeepClone();
            }
            return dataProvCbbGroupCopy;
        }
        public void AssignParent(object parent)
        {
            // No parent required
        }
        #endregion IDeepCloneable Members

        #region Methods
        public Extent GetExtent()
        {
            return null;
        }
        public override string ToString()
        {
            return "Cell-By-Cell Budget by Cell Group";
        }
        #endregion Methods
    }
}
