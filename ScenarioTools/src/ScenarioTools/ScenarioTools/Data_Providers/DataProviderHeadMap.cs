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
    /// <summary>
    /// This class can provide a head, drawdown, or concentration map
    /// </summary>
    public class DataProviderHeadMap : IDataProvider
    {
        #region Constants
        private const string XML_KEY_OBJECT_KEY = "key";
        private const string XML_KEY_FILE = "file";
        private const string XML_KEY_DATA_DESCRIPTOR = "datadescriptor";
        private const string XML_KEY_LAYER = "layer";
        private const string XML_KEY_STRESS_PERIOD = "stressPeriod";
        private const string XML_KEY_TIMESTEP = "timestep";
        #endregion Constants

        #region Fields
        private int _layer;
        private int _stressPeriod;
        private int _timestep;
        private long _key;
        private GeoMap _dataset;
        private bool _datasetNeedsRefresh;
        private bool _currentDatasetObtainable;
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
        public string HeadsFile { get; set; }
        public string DataDescriptor { get; set; }
        public int Layer
        {
            get
            {
                if (_layer < 1)
                {
                    _layer = 1;
                }
                return _layer;
            }
            set
            {
                _layer = value;
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
        #endregion Properties

        #region Constructor
        public DataProviderHeadMap()
        {
            // Make the key.
            _key = WorkspaceUtil.GetUniqueIdentifier();

            // Set the dataset to null and flag that the dataset needs to be refreshed.
            _dataset = null;
            _datasetNeedsRefresh = true;
            _currentDatasetObtainable = true;
            DataDescriptor = "";
            DataModificationTime = DateTime.MinValue;
        }
        #endregion Constructor

        #region IDataProvider Members
        public void InitFromXml(XmlElement element, string sourceFileName)
        {
            // Get the attributes.
            _key = XmlUtil.SafeGetLongAttribute(element, XML_KEY_OBJECT_KEY, WorkspaceUtil.GetUniqueIdentifier());

            string relPath = XmlUtil.SafeGetStringAttribute(element, XML_KEY_FILE, "");
            string dir = Path.GetDirectoryName(sourceFileName);
            HeadsFile = FileUtil.Relative2Absolute(relPath, dir);
            DataDescriptor = XmlUtil.SafeGetStringAttribute(element, XML_KEY_DATA_DESCRIPTOR, "");
            _sourceFileModificationTime = File.GetLastWriteTime(HeadsFile);
            _layer = XmlUtil.SafeGetIntAttribute(element, XML_KEY_LAYER, 0);
            _stressPeriod = XmlUtil.SafeGetIntAttribute(element, XML_KEY_STRESS_PERIOD, 0);
            _timestep = XmlUtil.SafeGetIntAttribute(element, XML_KEY_TIMESTEP, 0);
        }
        public XmlNode GetXmlNode(XmlDocument document, string targetFileName)
        {
            // Make the element that represents this object.
            XmlElement element = document.CreateElement("DataProviderHeadMap");

            // Set the attributes.
            XmlUtil.FrugalSetAttribute(element, XML_KEY_OBJECT_KEY, _key + "", "");
            string dir = Path.GetDirectoryName(targetFileName);
            string relPath = FileUtil.Absolute2Relative(HeadsFile, dir);
            XmlUtil.FrugalSetAttribute(element, XML_KEY_FILE, relPath, "");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_DATA_DESCRIPTOR, DataDescriptor, "");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_LAYER, Layer + "", "0");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_STRESS_PERIOD, StressPeriod + "", "0");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_TIMESTEP, Timestep + "", "0");

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
            // Do a quick check of the heads file. If it does not exist, mark the dataset as unobtainable.
            if (_currentDatasetObtainable || _datasetNeedsRefresh)
            {
                if (!File.Exists(this.HeadsFile))
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
                GeoMap newDataset = ModflowReader.GetArrayDataMapAtTimestep(HeadsFile, DataDescriptor, Layer, StressPeriod, Timestep);

                // If the new dataset is null, mark that the current dataset is unobtainable. Do not overwrite the current cache.
                if (newDataset == null)
                {
                    _currentDatasetObtainable = false;
                }

                // Otherwise, store the dataset and mark that the current dataset is obtainable (marking this is likely not necessary, but it's 
                // certainly true).
                else
                {
                    _dataset = newDataset;
                    _currentDatasetObtainable = true;
                    DataModificationTime = File.GetLastWriteTime(HeadsFile);
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
            if (_dataset == null)
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
            List<string> metadata = new List<string>();
            FileInfo fileInfo = null;

            try
            {
                fileInfo = new FileInfo(this.HeadsFile);
            }
            catch { }

            string fileSize = "FILE NOT FOUND";
            string creationDate = "FILE NOT FOUND";
            string modificationDate = "FILE NOT FOUND";
            if (fileInfo.Exists)
            {
                fileSize = FileUtil.GetFileLengthString(fileInfo.Length);
                creationDate = fileInfo.CreationTime + "";
                modificationDate = fileInfo.LastWriteTime + "";
            }

            metadata.Add("Data Category: " + this.ToString());
            metadata.Add("Source File: " + this.HeadsFile);
            metadata.Add("Creation Date: " + creationDate);
            metadata.Add("Modification Date: " + modificationDate);
            metadata.Add("File Size: " + fileSize);
            metadata.Add("Data Identifier: " + this.DataDescriptor);
            metadata.Add("Layer: " + this.Layer);
            metadata.Add("Stress Period: " + this.StressPeriod);
            metadata.Add("Timestep: " + this.Timestep);

            return metadata.ToArray();
        }
        #endregion IHasUniqueIdentifier Members

        #region Methods
        public Extent GetExtent()
        {
            return WorkspaceUtil.GetDefaultExtent();
        }
        public override string ToString()
        {
            return "Head, Drawdown, or Concentration";
        }
        #endregion Methods

        #region IDeepCloneable Members
        public object DeepClone()
        {
            DataProviderHeadMap dataProvHeadMapCopy = new DataProviderHeadMap();
            dataProvHeadMapCopy._layer = this._layer;
            dataProvHeadMapCopy._sourceFileModificationTime = new DateTime(this._sourceFileModificationTime.Ticks);
            dataProvHeadMapCopy._stressPeriod = this._stressPeriod;
            dataProvHeadMapCopy._timestep = this._timestep;
            dataProvHeadMapCopy.ConvertFlowToFlux = this.ConvertFlowToFlux;
            dataProvHeadMapCopy.DataDescriptor = this.DataDescriptor;
            dataProvHeadMapCopy.DataModificationTime = new DateTime(this.DataModificationTime.Ticks);
            dataProvHeadMapCopy.HeadsFile = this.HeadsFile;
            return dataProvHeadMapCopy;
        }
        public void AssignParent(object parent)
        {
            // No parent required
        }
        #endregion IDeepCloneable Members
    }
}
