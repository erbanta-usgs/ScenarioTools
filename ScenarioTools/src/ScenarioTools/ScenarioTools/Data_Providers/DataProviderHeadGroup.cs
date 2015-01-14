using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Drawing;

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
    public class DataProviderHeadGroup : IDataProvider
    {
        private const string XML_KEY_FILE = "file";
        private const string XML_KEY_DATA_DESCRIPTOR = "datadescriptor";
        private const string XML_KEY_OBJECT_KEY = "key";

        #region Fields
        private int _startLayer;
        private int _endLayer;
        private CellGroupProvider[] _cellGroups;
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
        public string HeadsFile { get; set; }
        public string DataDescriptor { get; set; }
        public CellGroupProvider[] CellGroups
        {
            get
            {
                if (_cellGroups == null)
                {
                    _cellGroups = new CellGroup[0];
                }
                return _cellGroups;
            }
            set
            {
                _cellGroups = value;
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
        public DataProviderHeadGroup()
        {
            _dataset = null;
            _datasetNeedsRefresh = true;
            _currentDatasetObtainable = true;

            _key = WorkspaceUtil.GetUniqueIdentifier();
            DataDescriptor = "";
            DataModificationTime = DateTime.MinValue;
        }
        #endregion Constructor

        #region IDataProvider Members
        public XmlNode GetXmlNode(XmlDocument document, string targetFileName)
        {
            // Make the element that represents this object.
            XmlElement element = document.CreateElement("DataProviderHeadGroup");

            // Set the attributes.
            XmlUtil.FrugalSetAttribute(element, XML_KEY_OBJECT_KEY, _key + "", "");
            string dir = Path.GetDirectoryName(targetFileName);
            string relPath = FileUtil.Absolute2Relative(HeadsFile, dir);
            XmlUtil.FrugalSetAttribute(element, XML_KEY_FILE, relPath, "");
            XmlUtil.FrugalSetAttribute(element, XML_KEY_DATA_DESCRIPTOR, DataDescriptor, "");

            // Add the cell group nodes.
            for (int i = 0; i < CellGroups.Length; i++)
            {
                element.AppendChild(CellGroups[i].GetXmlNode(document, targetFileName));
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
            HeadsFile = FileUtil.Relative2Absolute(relPath, dir);
            _sourceFileModificationTime = File.GetLastWriteTime(HeadsFile);
            DataDescriptor = XmlUtil.SafeGetStringAttribute(element, XML_KEY_DATA_DESCRIPTOR, "");

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
            _cellGroups = cellGroupList.ToArray();

            // Register the cell groups with the workspace.
            GroupHelper.RegisterGroups(_cellGroups);

            Console.WriteLine("Got " + _cellGroups.Length + " groups from XML file");

            // Set the heads array to null.
            _dataset = null;
        }
        public void InvalidateDataset()
        {
            // Set the in-memory dataset to null.
            _dataset = null;

            // Clear the cache.
            WorkspaceUtil.ClearCache(_key);
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
                // Get the time series from the MODFLOW reader class. 
                // If there is an issue reading the time series, this function will return null.
                Point[] combinedCells = CellGroupProvider.GetCombinedCells(CellGroups);
                TimeSeries newDataset = ModflowReader.GetArrayDataAtCells(HeadsFile, DataDescriptor, combinedCells, 
                    StartLayer, EndLayer, ScenarioTools.GlobalStaticVariables.GlobalTemporalReference);

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
                FileInfo fileInfo = new FileInfo(this.HeadsFile);
                string fileSize = FileUtil.GetFileLengthString(fileInfo.Length);

                return new string[] {
                "Data Category: " + this.ToString(),
                "Source File: " + this.HeadsFile,
                "Creation Date: " + fileInfo.CreationTime,
                "Modification Date: " + fileInfo.LastWriteTime,
                "File Size: " + fileSize,
                "Data Identifier: " + this.DataDescriptor
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
            DataProviderHeadGroup dataProvHeadGroupCopy = new DataProviderHeadGroup();
            int n = this._cellGroups.GetLength(0);
            dataProvHeadGroupCopy._cellGroups = new CellGroupProvider[n];
            for (int i = 0; i < n; i++)
            {
                dataProvHeadGroupCopy._cellGroups[i] = (CellGroupProvider)this._cellGroups[i].DeepClone();
            }
            dataProvHeadGroupCopy._endLayer = this._endLayer;
            dataProvHeadGroupCopy._sourceFileModificationTime = new DateTime(this._sourceFileModificationTime.Ticks);
            dataProvHeadGroupCopy._startLayer = this._startLayer;
            dataProvHeadGroupCopy.ConvertFlowToFlux = this.ConvertFlowToFlux;
            dataProvHeadGroupCopy.DataDescriptor = this.DataDescriptor;
            dataProvHeadGroupCopy.DataModificationTime = new DateTime(this.DataModificationTime.Ticks);
            dataProvHeadGroupCopy.HeadsFile = this.HeadsFile;
            return dataProvHeadGroupCopy;
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
            return "Head, Drawdown, or Concentration by Cell Group";
        }
        #endregion Methods
    }
}
