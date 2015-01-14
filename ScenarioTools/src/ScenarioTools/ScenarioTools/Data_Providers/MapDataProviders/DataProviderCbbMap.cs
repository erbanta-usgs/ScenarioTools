using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

using ScenarioTools.DataClasses;
using ScenarioTools.Geometry;
using ScenarioTools.ModflowReaders;
using ScenarioTools.Util;
using ScenarioTools.Xml;

namespace ScenarioTools.Data_Providers
{
    public class DataProviderCbbMap : IDataProvider
    {
        #region Constants
        private const string XML_KEY_OBJECT_KEY = "key";
        private const string XML_KEY_FILE = "file";
        private const string XML_KEY_IDENTIFIER = "identifier";
        private const string XML_KEY_START_LAYER = "startLayer";
        private const string XML_KEY_END_LAYER = "endLayer";
        private const string XML_KEY_STRESS_PERIOD = "stressPeriod";
        private const string XML_KEY_TIMESTEP = "timestep";
        #endregion Constants

        #region Fields
        private string _cbbFile;
        private string _dataIdentifier;
        private int _stressPeriod;
        private int _timestep;
        private int _startLayer;
        private int _endLayer;
        private GeoMap _dataset;
        private bool _datasetNeedsRefresh;
        private bool _currentDatasetObtainable;
        private string[] _datasetResultMessage;
        private DateTime _sourceFileModificationTime;
        #endregion Fields

        #region Constructor
        public DataProviderCbbMap()
        {
            key = WorkspaceUtil.GetUniqueIdentifier();

            _dataset = null;
            _datasetNeedsRefresh = true;
            _currentDatasetObtainable = true;

            ConvertFlowToFlux = false;
            DataModificationTime = DateTime.MinValue;
        }
        #endregion Constructor

        #region Properties
        public long Key
        {
            get
            {
                return key;
            }
        }
        public DateTime DataModificationTime { get; set; }
        public string CbbFile
        {
            get
            {
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
                // Depending on identifier, convert map values from flows to fluxes
                string idLower = _dataIdentifier.ToLower();
            }
        }
        public int StressPeriod
        {
            get
            {
                if (_stressPeriod < 1)
                {
                    _stressPeriod = 1;
                }
                return _stressPeriod;
            }
            set
            {
                _stressPeriod = value;
            }
        }
        public int Timestep
        {
            get
            {
                if (_timestep < 1)
                {
                    _timestep = 1;
                }
                return _timestep;
            }
            set
            {
                _timestep = value;
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
                if (_endLayer < 1)
                {
                    _endLayer = 1;
                }
                return _endLayer;
            }
            set
            {
                _endLayer = value;
            }
        }
        public bool ConvertFlowToFlux { get; set; }
        #endregion Properties

        #region IDataProvider Members
        public void InitFromXml(XmlElement element, string sourceFileName)
        {
            // Get the key.
            key = XmlUtil.SafeGetLongAttribute(element, XML_KEY_OBJECT_KEY, WorkspaceUtil.GetUniqueIdentifier());

            // Get the attributes.
            string relPath = XmlUtil.SafeGetStringAttribute(element, XML_KEY_FILE, "");
            string dir = Path.GetDirectoryName(sourceFileName);
            _cbbFile = FileUtil.Relative2Absolute(relPath, dir);
            _dataIdentifier = XmlUtil.SafeGetStringAttribute(element, XML_KEY_IDENTIFIER, "");
            _stressPeriod = XmlUtil.SafeGetIntAttribute(element, XML_KEY_STRESS_PERIOD, 0);
            _timestep = XmlUtil.SafeGetIntAttribute(element, XML_KEY_TIMESTEP, 0);
            _startLayer = XmlUtil.SafeGetIntAttribute(element, XML_KEY_START_LAYER, 0);
            _endLayer = XmlUtil.SafeGetIntAttribute(element, XML_KEY_END_LAYER, 0);
            _sourceFileModificationTime = File.GetLastWriteTime(_cbbFile);
        }
        public XmlNode GetXmlNode(XmlDocument document, string targetFileName)
        {
            // Make the element that represents this object.
            XmlElement element = document.CreateElement("DataProviderCbbMap");

            // Set the attributes.
            XmlUtil.FrugalSetAttribute(element, XML_KEY_OBJECT_KEY, key + "", "");
            string dir = Path.GetDirectoryName(targetFileName);
            string relPath = FileUtil.Absolute2Relative(CbbFile, dir);
            XmlUtil.FrugalSetAttribute(element, XML_KEY_FILE, relPath, "");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_IDENTIFIER, DataIdentifier, "");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_STRESS_PERIOD, StressPeriod + "", "0");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_TIMESTEP, Timestep + "", "0");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_START_LAYER, StartLayer + "", "0");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_END_LAYER, EndLayer + "", "0");

            // Return the result.
            return element;
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
                // Get the time series from the MODFLOW reader class. 
                // If there is an issue reading the time series, this function will return null.
                bool isCompact;
                int precision = PrecisionHelpers.BudgetFilePrecision(CbbFile, out isCompact);
                GeoMap newDataset = CbbReader.GetMapFromCbb(CbbFile, DataIdentifier, 
                    StressPeriod, Timestep, StartLayer, EndLayer, out _datasetResultMessage, 
                    precision, ConvertFlowToFlux);

                // If the new dataset is null, mark that the current dataset is unobtainable. 
                // Do not overwrite the current cache.
                if (newDataset == null)
                {
                    _currentDatasetObtainable = false;
                }

                // Otherwise, store the dataset and mark that the current dataset is obtainable 
                // (marking this is likely not necessary, but it's certainly true).
                else
                {
                    _dataset = newDataset;
                    _currentDatasetObtainable = true;
                    DataModificationTime = File.GetLastWriteTime(CbbFile);
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
            _dataset = WorkspaceUtil.GetCachedGeoMap(stream);

            // If the dataset is no longer null, switch off the update flag.
            if (_dataset != null)
            {
                _datasetNeedsRefresh = false;
                DataModificationTime = streamDateTime;
            }
        }
        public bool SupportsDataConsumerType(DataConsumerTypeEnum dataConsumerType)
        {
            if (dataConsumerType == DataConsumerTypeEnum.Map || dataConsumerType == DataConsumerTypeEnum.STMap)
            {
                return true;
            }
            return false;
        }
        public string[] GetResultMessage()
        {
            return _datasetResultMessage;
        }
        #endregion IDataProvider Members

        #region IHasUniqueIdentifier Members
        private long key;
        public long GetUniqueIdentifier()
        {
            return key;
        }
        public void UpdateUniqueIdentifier()
        {
            key = WorkspaceUtil.GetUniqueIdentifier();
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
            List<string> metadata = new List<string>();
            try
            {
                metadata.Add("Data Category: " + this.ToString());
                metadata.Add("Source File: " + this.CbbFile);

                try
                {
                    FileInfo fileInfo = new FileInfo(this.CbbFile);
                    string fileSize = FileUtil.GetFileLengthString(fileInfo.Length);
                    metadata.Add("Creation Date: " + fileInfo.CreationTime);
                    metadata.Add("Modification Date: " + fileInfo.LastWriteTime);
                    metadata.Add("File Size: " + fileSize);
                }
                catch { }

                metadata.Add("Data Series Stress Period: " + this.StressPeriod);
                metadata.Add("Data Series Timestep: " + this.Timestep);
                metadata.Add("Data Series Start Layer: " + this.StartLayer);
                metadata.Add("Data Series End Layer: " + this.EndLayer);
                if (this.ConvertFlowToFlux)
                {
                    metadata.Add("Flow values have been converted to flux values");
                    metadata.Add("for display.");
                }
            }
            catch { }

            return metadata.ToArray();
        }
        #endregion IHasUniqueIdentifier Members

        #region IDeepCloneable Members
        public object DeepClone()
        {
            DataProviderCbbMap dataProvCbbMapCopy = new DataProviderCbbMap();
            dataProvCbbMapCopy._cbbFile = this._cbbFile;
            dataProvCbbMapCopy._dataIdentifier = this._dataIdentifier;
            dataProvCbbMapCopy._endLayer = this._endLayer;
            dataProvCbbMapCopy._sourceFileModificationTime = new DateTime(this._sourceFileModificationTime.Ticks);
            dataProvCbbMapCopy._startLayer = this._startLayer;
            dataProvCbbMapCopy._stressPeriod = this._stressPeriod;
            dataProvCbbMapCopy._timestep = this._timestep;
            dataProvCbbMapCopy.ConvertFlowToFlux = this.ConvertFlowToFlux;
            dataProvCbbMapCopy.DataModificationTime = new DateTime(this.DataModificationTime.Ticks);
            return dataProvCbbMapCopy;
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
            return "Cell-By-Cell Budget";
        }
        #endregion Methods
    }
}
