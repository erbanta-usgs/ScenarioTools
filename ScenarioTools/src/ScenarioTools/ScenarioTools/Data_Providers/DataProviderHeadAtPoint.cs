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
    /// This class can provide head, drawdown, or concentration data
    /// </summary>
    public class DataProviderHeadAtPoint : IDataProvider
    {
        #region Constants
        private const string XML_KEY_FILE = "file";
        private const string XML_KEY_DATA_DESCRIPTOR = "datadescriptor";
        private const string XML_KEY_COLUMN = "column";
        private const string XML_KEY_ROW = "row";
        private const string XML_KEY_LAYER = "layer";
        private const string XML_KEY_OBJECT_KEY = "key";
        #endregion Constants

        #region Fields
        private int _column;
        private int _row;
        private int _layer;
        private long _key;

        // These are the dataset and the flags that track the state of the dataset. 
        // The dataset is only null when the data provider is first created.
        // After the dataset is set to a non-null value, it can only be replaced 
        // by another non-null value. The needs-refresh flag notes when 
        // parameters have changed that require the dataset to be refreshed. 
        // The current-dataset-obtainable notes whether the last refresh of the 
        // dataset was successful.
        private TimeSeries _dataset;
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
        public int Column
        {
            get
            {
                if (_column < 1)
                {
                    _column = 1;
                }
                return _column;
            }
            set
            {
                _column = value;
            }
        }
        public int Row
        {
            get
            {
                if (_row < 1)
                {
                    _row = 1;
                }
                return _row;
            }
            set
            {
                _row = value;
            }
        }
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
        #endregion Properties

        #region Constructor
        public DataProviderHeadAtPoint()
        {
            // Set the dataset to null and mark that the dataset needs to be refreshed.
            _dataset = null;
            _datasetNeedsRefresh = true;
            _currentDatasetObtainable = true;
            _key = WorkspaceUtil.GetUniqueIdentifier();

            DataModificationTime = DateTime.MinValue;
            DataDescriptor = "";
        }
        #endregion Constructor

        #region IDataProvider Members
        public XmlNode GetXmlNode(XmlDocument document, string targetFileName)
        {
            // Make the element that represents this object.
            XmlElement element = document.CreateElement("DataProviderHeadAtPoint");

            // Set the key.
            XmlUtil.FrugalSetAttribute(element, XML_KEY_OBJECT_KEY, _key + "", "");

            // Set the attributes.
            string dir = Path.GetDirectoryName(targetFileName);
            string relPath = FileUtil.Absolute2Relative(HeadsFile, dir);
            XmlUtil.FrugalSetAttribute(element, XML_KEY_FILE, relPath, "");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_DATA_DESCRIPTOR, DataDescriptor, "");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_COLUMN, Column + "", "0");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_ROW, Row + "", "0");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_LAYER, Layer + "", "0");

            // Return the result.
            return element;
        }
        public void InitFromXml(XmlElement element, string sourceFileName)
        {
            // Get the key.
            _key = XmlUtil.SafeGetLongAttribute(element, XML_KEY_OBJECT_KEY, WorkspaceUtil.GetUniqueIdentifier());

            // Get the attributes from the node.
            string relPath = XmlUtil.SafeGetStringAttribute(element, XML_KEY_FILE, "");
            string dir = Path.GetDirectoryName(sourceFileName);
            HeadsFile = FileUtil.Relative2Absolute(relPath, dir);
            DataDescriptor = XmlUtil.SafeGetStringAttribute(element, XML_KEY_DATA_DESCRIPTOR, "");
            _column = XmlUtil.SafeGetIntAttribute(element, XML_KEY_COLUMN, 0);
            _row = XmlUtil.SafeGetIntAttribute(element, XML_KEY_ROW, 0);
            _layer = XmlUtil.SafeGetIntAttribute(element, XML_KEY_LAYER, 0);
            _sourceFileModificationTime = File.GetLastWriteTime(HeadsFile);

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
                // Get the time series from the MODFLOW reader class.  If there is
                // an issue reading the time series, this function will return null.
                TimeSeries newDataset = ModflowReader.GetArrayDataAtCell(HeadsFile, DataDescriptor, Column, 
                    Row, Layer, ScenarioTools.GlobalStaticVariables.GlobalTemporalReference);

                // If the new dataset is null, mark that the current dataset is unobtainable. 
                // Do not overwrite the current cache.
                if (newDataset == null)
                {
                    _currentDatasetObtainable = false;
                }

                // Otherwise, store the dataset and mark that the current dataset is 
                // obtainable (marking this is likely not necessary, but it's 
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
            if (_dataset == null)
            {
                return new string[] { "Invalid data set" };
            }
            else
            {
                return new string[] { "Valid data set" };
            }
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
            try
            {
                metadata.Add("Data Category: " + this.ToString());
                metadata.Add("Source File: " + this.HeadsFile);

                try
                {
                    FileInfo fileInfo = new FileInfo(this.HeadsFile);
                    string fileSize = FileUtil.GetFileLengthString(fileInfo.Length);
                    metadata.Add("Creation Date: " + fileInfo.CreationTime);
                    metadata.Add("Modification Date: " + fileInfo.LastWriteTime);
                    metadata.Add("File Size: " + fileSize);
                }
                catch { }

                metadata.Add("Data Identifier: " + this.DataDescriptor);
                metadata.Add("Data Series Layer: " + this.Layer);
                metadata.Add("Data Series Row: " + this.Row);
                metadata.Add("Data Series Column: " + this.Column);
            }
            catch { }

            return metadata.ToArray();
        }
        #endregion

        #region IDeepCloneable Members
        public object DeepClone()
        {
            DataProviderHeadAtPoint dataProvHeadPointCopy = new DataProviderHeadAtPoint();
            dataProvHeadPointCopy._column = this._column;
            dataProvHeadPointCopy._layer = this._layer;
            dataProvHeadPointCopy._row = this._row;
            dataProvHeadPointCopy._sourceFileModificationTime = new DateTime(this._sourceFileModificationTime.Ticks);
            dataProvHeadPointCopy.ConvertFlowToFlux = this.ConvertFlowToFlux;
            dataProvHeadPointCopy.DataDescriptor = this.DataDescriptor;
            dataProvHeadPointCopy.DataModificationTime = new DateTime(this.DataModificationTime.Ticks);
            dataProvHeadPointCopy.HeadsFile = this.HeadsFile;
            return dataProvHeadPointCopy;
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
            return "Head, Drawdown, or Concentration at Point";
        }
        #endregion Methods
    }
}
