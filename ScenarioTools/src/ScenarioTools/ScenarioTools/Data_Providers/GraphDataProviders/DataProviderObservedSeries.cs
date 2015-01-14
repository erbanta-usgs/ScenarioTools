using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

using ScenarioTools.DataClasses;
using ScenarioTools.FileReaders;
using ScenarioTools.Geometry;
using ScenarioTools.Util;
using ScenarioTools.Xml;

namespace ScenarioTools.Data_Providers
{
    public class DataProviderObservedSeries : IDataProvider
    {
        #region Constants
        private const string XML_KEY_FILE = "file";
        private const string XML_KEY_IDENTIFIER = "identifier";
        private const string XML_KEY_OBJECT_KEY = "key";
        #endregion Constants

        #region Fields
        private string _filePath;
        private string _seriesName;
        private bool _datasetNeedsRefresh;
        private bool _currentDatasetObtainable;
        private long _key;
        private DateTime _sourceFileModificationTime;
        private TimeSeries _dataset;
        #endregion Fields

        #region Properties
        public string FilePath
        {
            get
            {
                if (_filePath == null)
                {
                    _filePath = "";
                }
                return _filePath;
            }
            set
            {
                _filePath = value;
            }
        }
        public string SeriesName
        {
            get
            {
                if (_seriesName == null)
                {
                    _seriesName = "";
                }
                return _seriesName;
            }
            set
            {
                _seriesName = value;
            }
        }
        public long Key
        {
            get
            {
                return _key;
            }
        }
        public DateTime DataModificationTime { get; set; }
        public bool ConvertFlowToFlux { get; set; }
        #endregion Properties

        #region Constructor
        public DataProviderObservedSeries()
        {
            // Generate a unique key. This may be overridden if the object is initialized from an XML node. This is not a problem. However, we want to
            // avoid the situation where the key has a non-unique default value (0).
            _key = WorkspaceUtil.GetUniqueIdentifier();

            _dataset = null;
            _datasetNeedsRefresh = true;
            _currentDatasetObtainable = true;

            DataModificationTime = DateTime.MinValue;
        }
        #endregion Constructor

        #region IDataProvider Members
        public object GetData(out int datasetStatus)
        {
            // Do a quick check of the file. If it does not exist, mark the dataset as unobtainable.
            if (_currentDatasetObtainable || _datasetNeedsRefresh)
            {
                if (!File.Exists(this.FilePath))
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
                // Get the time series from the MODFLOW reader class. If there is an issue reading the time series, this function will return null.
                TimeSeries newDataset = BoreholeSampleFileReader.GetValues(FilePath, SeriesName);

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
                    DataModificationTime = File.GetLastWriteTime(FilePath);
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
        public void InvalidateDataset()
        {
            // Mark that the dataset needs to be refreshed.
            _datasetNeedsRefresh = true;

            // Mark that the current dataset is obtainable. Until we know otherwise, we will assume that it is.
            _currentDatasetObtainable = true;
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
            if (_dataset == null)
            {
                return new string[] { "Invalid data set." };
            }
            else
            {
                return new string[] { "Valid data set." };
            }
        }
        public XmlNode GetXmlNode(XmlDocument document, string targetFileName)
        {
            // Make the element that represents this object.
            XmlElement element = document.CreateElement("DataProviderObservedSeries");

            // Set the attributes.
            XmlUtil.FrugalSetAttribute(element, XML_KEY_OBJECT_KEY, _key + "", "");
            string dir = Path.GetDirectoryName(targetFileName);
            string relPath = FileUtil.Absolute2Relative(FilePath, dir);
            XmlUtil.FrugalSetAttribute(element, XML_KEY_FILE, relPath, "");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_IDENTIFIER, SeriesName, "");

            // Return the result.
            return element;
        }
        public void InitFromXml(XmlElement element, string sourceFileName)
        {
            // Get the attributes.
            _key = XmlUtil.SafeGetLongAttribute(element, XML_KEY_OBJECT_KEY, WorkspaceUtil.GetUniqueIdentifier());
            string relPath = XmlUtil.SafeGetStringAttribute(element, XML_KEY_FILE, "");
            string dir = Path.GetDirectoryName(sourceFileName);
            FilePath = FileUtil.Relative2Absolute(relPath, dir);
            SeriesName = XmlUtil.SafeGetStringAttribute(element, XML_KEY_IDENTIFIER, "");
            _sourceFileModificationTime = File.GetLastWriteTime(FilePath);
        }
        #endregion

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
                fileInfo = new FileInfo(this.FilePath);
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
            metadata.Add("Source File: " + this.FilePath);
            metadata.Add("Series Name: " + this.SeriesName);
            metadata.Add("Creation Date: " + creationDate);
            metadata.Add("Modification Date: " + modificationDate);
            metadata.Add("File Size: " + fileSize);

            return metadata.ToArray();
        }
        #endregion IHasUniqueIdentifier Members

        #region IDeepCloneable Members
        public object DeepClone()
        {
            DataProviderObservedSeries dataProvObsSeriesCopy = new DataProviderObservedSeries();
            dataProvObsSeriesCopy._filePath = this._filePath;
            dataProvObsSeriesCopy._seriesName = this._seriesName;
            dataProvObsSeriesCopy._sourceFileModificationTime = new DateTime(this._sourceFileModificationTime.Ticks);
            dataProvObsSeriesCopy.ConvertFlowToFlux = this.ConvertFlowToFlux;
            dataProvObsSeriesCopy.DataModificationTime = new DateTime(this.DataModificationTime.Ticks);
            return dataProvObsSeriesCopy;
        }
        public void AssignParent(object parent)
        {
            // No parent required
        }
        #endregion IDeepCloneable Members

        #region Methods
        public override string ToString()
        {
            return "Observed Series (SMP file)";
        }
        public Extent GetExtent()
        {
            return null;
        }
        #endregion Methods
    }
}
