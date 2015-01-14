using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml;

using ScenarioTools.DataClasses;
using ScenarioTools.Geometry;
using ScenarioTools.ModflowReaders;
using ScenarioTools.Xml;
using ScenarioTools.Util;

namespace ScenarioTools.Data_Providers
{
    public class DataProviderCbbAtPoint : IDataProvider
    {
        #region Constants
        private const string XML_KEY_OBJECT_KEY = "key";
        private const string XML_KEY_FILE = "file";
        private const string XML_KEY_IDENTIFIER = "identifier";
        private const string XML_KEY_COLUMN = "column";
        private const string XML_KEY_ROW = "row";
        private const string XML_KEY_LAYER= "layer";
        private const string XML_KEY_STRESS_PERIOD_START = "stressPeriodStart";
        private const string XML_KEY_STRESS_PERIOD_END = "stressPeriodEnd";
        private const string XML_KEY_TIMESTEP_START = "timestepStart";
        private const string XML_KEY_TIMESTEP_END = "timestepEnd";
        #endregion Constants

        #region Fields
        private string _cbbFile;
        private string _dataIdentifier;
        private int _column;
        private int _row;
        private int _layer;
        private long _key;
        private TimeSeries _dataset;
        private bool _datasetNeedsRefresh;
        private bool _currentDatasetObtainable;
        private string[] _datasetResultMessage;
        private DateTime _sourceFileModificationTime;
        #endregion Fields

        #region Constructor
        public DataProviderCbbAtPoint()
        {
            _dataset = null;
            _datasetNeedsRefresh = true;
            _currentDatasetObtainable = true;

            _key = WorkspaceUtil.GetUniqueIdentifier();

            DataModificationTime = DateTime.MinValue;
        }
        #endregion Constructor

        #region Properties
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
            }
        }
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

        #region IDataProvider Members
        public void InitFromXml(XmlElement element, string sourceFileName)
        {
            // Get the key.
            _key = XmlUtil.SafeGetLongAttribute(element, XML_KEY_OBJECT_KEY, WorkspaceUtil.GetUniqueIdentifier());

            // Get the attributes.
            string relPath = XmlUtil.SafeGetStringAttribute(element, XML_KEY_FILE, "");
            string dir = Path.GetDirectoryName(sourceFileName);
            _cbbFile = FileUtil.Relative2Absolute(relPath, dir);
            _dataIdentifier = XmlUtil.SafeGetStringAttribute(element, XML_KEY_IDENTIFIER, "");
            _column = XmlUtil.SafeGetIntAttribute(element, XML_KEY_COLUMN, 0);
            _row = XmlUtil.SafeGetIntAttribute(element, XML_KEY_ROW, 0);
            _layer = XmlUtil.SafeGetIntAttribute(element, XML_KEY_LAYER, 0);
            _sourceFileModificationTime = File.GetLastWriteTime(_cbbFile);
        }
        public XmlNode GetXmlNode(XmlDocument document, string targetFileName)
        {
            // Make the element that represents this object.
            XmlElement element = document.CreateElement("DataProviderCbbAtPoint");

            // Set the key.
            XmlUtil.FrugalSetAttribute(element, XML_KEY_OBJECT_KEY, _key + "", "");

            // Set the attributes.
            string dir = Path.GetDirectoryName(targetFileName);
            string relPath = FileUtil.Absolute2Relative(CbbFile, dir);
            XmlUtil.FrugalSetAttribute(element, XML_KEY_FILE, relPath, "");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_IDENTIFIER, DataIdentifier, "");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_COLUMN, Column + "", "0");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_ROW, Row + "", "0");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_LAYER, Layer + "", "0");

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
                // Get the time series from the MODFLOW reader class. If there is an issue reading the time series, this function will return null.
                TimeSeries newDataset = CbbReader.GetPointTimeSeriesFromCbb(_cbbFile, _dataIdentifier, _column, _row, _layer, 
                    out _datasetResultMessage, ScenarioTools.GlobalStaticVariables.GlobalTemporalReference);

                // If the new dataset is null, mark that the current dataset is unobtainable. Do not overwrite the current cache.
                if (newDataset == null)
                {
                    MessageBox.Show(_datasetResultMessage[0]);
                    _currentDatasetObtainable = false;
                }

                // Otherwise, store the dataset and mark that the current dataset is obtainable (marking this is likely not necessary, but it's 
                // certainly true).
                else
                {
                    _dataset = newDataset;
                    _currentDatasetObtainable = true;
                    DataModificationTime = File.GetLastWriteTime(_cbbFile);
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
            // If the message is null, set it to a generic failure message.
            if (_datasetResultMessage == null)
            {
                _datasetResultMessage = new string[] { "Invalid data set." };
            }

            // Return the result message.
            return _datasetResultMessage;
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

                metadata.Add("Data Series Column: " + this.Column);
                metadata.Add("Data Series Row: " + this.Row);
                metadata.Add("Data Series Layer: " + this.Layer);
            }
            catch { }

            return metadata.ToArray();
        }
        #endregion IHasUniqueIdentifier Members

        #region IDeepCloneable Members
        public object DeepClone()
        {
            DataProviderCbbAtPoint dataProvCbbPointCopy = new DataProviderCbbAtPoint();
            dataProvCbbPointCopy._cbbFile = this._cbbFile;
            dataProvCbbPointCopy._column = this._column;
            dataProvCbbPointCopy._dataIdentifier = this._dataIdentifier;
            dataProvCbbPointCopy._layer = this._layer;
            dataProvCbbPointCopy._row = this._row;
            dataProvCbbPointCopy._sourceFileModificationTime = new DateTime(this._sourceFileModificationTime.Ticks);
            dataProvCbbPointCopy.ConvertFlowToFlux = this.ConvertFlowToFlux;
            dataProvCbbPointCopy.DataModificationTime = new DateTime(this.DataModificationTime.Ticks);
            return dataProvCbbPointCopy;
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
            return "Cell-By-Cell Budget at Point";
        }
        #endregion Methods
    }
}
