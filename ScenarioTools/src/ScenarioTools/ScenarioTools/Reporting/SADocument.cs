using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml;

using GeoAPI.Geometries;

using ICSharpCode.SharpZipLib.Zip;

using ScenarioTools.Geometry;
using ScenarioTools.ModflowReaders;
using ScenarioTools.TreeElementFactories;
using ScenarioTools.Xml;

namespace ScenarioTools.Reporting
{
    /// <summary>
    /// An SADocument is a representation of a Scenario Analyzer report.
    /// </summary>
    public class SADocument : IStaticDataSource
    {
        #region Static variables
        private static int documentIndex = 1;
        private static byte[] getByteArray()
        {
            byte[] array = new byte[100];
            for (int i = 0; i < array.Length - 3; i += 4)
            {
                array[i] = (byte)'a';
                array[i + 1] = (byte)'b';
                array[i + 2] = (byte)'c';
                array[i + 3] = (byte)'d';
            }
            return array;
        }
        #endregion Static variables

        #region Constants
        private const string XML_KEY_REPORT_NAME = "reportName";
        private const string XML_KEY_AUTHOR = "author";
        private const string XML_KEY_BACKGROUND_IMAGE_FILE = "imageFile";
        private const string XML_KEY_GRID_SHAPEFILE = "gridShapefile";
        private const string XML_KEY_HNOFLO_TEXT = "hnofloText";
        private const string XML_KEY_HDRY_TEXT = "hdryText";
        private const string XML_KEY_CINACT_TEXT = "cinactText";
        private const string XML_KEY_SIMULATION_START_TIME = "simulationStartTime";
        private const string XML_KEY_MODFLOW_TIME_UNIT = "modflowTimeUnit";
        private const string XML_KEY_MODFLOW_LENGTH_UNIT = "modflowLengthUnit";
        private const string XML_KEY_NAME_FILE = "disFile";
        private const string XML_KEY_BLANKING_MODE = "blankingMode";
        private const string XML_KEY_BLANKING_LAYER = "blankingLayer";
        private const string DEFAULT_REPORT_NAME = "Unnamed Report";
        private const string DEFAULT_AUTHOR = "";
        private const string DEFAULT_BACKGROUND_IMAGE_FILE = "";
        private const string DEFAULT_GRID_SHAPEFILE = "";
        private const string DEFAULT_HNOFLO_TEXT = "99999.0";
        private const string DEFAULT_HDRY_TEXT = "-99999.0";
        private const string DEFAULT_CINACT_TEXT = "-1.0";
        private const string DEFAULT_SIMULATION_START_TIME = "";
        public  const string DEFAULT_MODFLOW_TIME_UNIT = "1";   // Default: Seconds
        public  const string DEFAULT_MODFLOW_LENGTH_UNIT = "2"; // Default: Meters
        private const string DEFAULT_NAME_FILE = "";
        private const string DEFAULT_BLANKING_MODE = "0";
        private const string DEFAULT_BLANKING_LAYER = "1";
        #endregion Constants

        #region Fields
        private bool _hasUnsavedChanges;
        private bool _modflowBasicDataDefined;
        private string _temporaryFileName;
        private string _stableFileName;
        private string _reportName;
        private string _author;
        private string _hnofloText; // Assign only through property HnofloText, to ensure that ModflowReader.HnofloText is assigned the same
        private string _hdryText;   // Assign only through property HdryText, to ensure that ModflowReader.HdryText is assigned the same
        private string _cinactText; // Assign only through property CinactText, to ensure that ModflowReader.CinactText is assigned the same
        private string _nameFile;
        private List<IReportElement> _elements;
        private List<Extent> _extents;
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Makes a new, empty Scenario Analyzer document.
        /// </summary>
        public SADocument()
        {
            // By convention, a new, empty document does not have unsaved changes.
            _hasUnsavedChanges = false;
            _modflowBasicDataDefined = false;

            // Make a default temporary file name and set the stable name to null.
            _temporaryFileName = "Document " + documentIndex++;
            _stableFileName = "";

            // Make the list of elements.
            _elements = new List<IReportElement>();

            // Make the list of extents.
            _extents = new List<Extent>();

            HnofloText = DEFAULT_HNOFLO_TEXT;
            HdryText = DEFAULT_HDRY_TEXT; 
            CinactText = DEFAULT_CINACT_TEXT;
            NameFile = DEFAULT_NAME_FILE;
            BlankingLayer = Convert.ToInt32(DEFAULT_BLANKING_LAYER);
            BlankingMode = (ScenarioTools.DataClasses.MapEnums.BlankingMode)(Convert.ToInt32(DEFAULT_BLANKING_MODE));
            //ModelTimeUnit = ScenarioTools.DataClasses.TemporalReference.ModflowTimeUnit.seconds;
            ModflowLengthUnit = ScenarioTools.DataClasses.LengthReference.ModflowLengthUnit.meters;
        }
        /// <summary>
        /// Makes a copy of an existing Scenario Analyzer document.
        /// </summary>
        /// <param name="saDocument"></param>
        public SADocument(SADocument saDocument) : this()
        {
            _hasUnsavedChanges = saDocument._hasUnsavedChanges;
            _temporaryFileName = saDocument._temporaryFileName;
            _stableFileName = saDocument._stableFileName;
            _reportName = saDocument._reportName;
            _author = saDocument._author;
            HnofloText = saDocument.HnofloText;
            HdryText = saDocument.HdryText;
            CinactText = saDocument.CinactText;
            NameFile = saDocument.NameFile;
            BlankingMode = saDocument.BlankingMode;
            BlankingLayer = saDocument.BlankingLayer;
            //BackgroundImageFile = saDocument.BackgroundImageFile;
            //ModelTimeUnit = saDocument.ModelTimeUnit;
            ModflowLengthUnit = saDocument.ModflowLengthUnit;

            // Copy the list of elements
            for (int i = 0; i < saDocument._elements.Count; i++)
            {
                _elements.Add(saDocument._elements[i].DeepClone());
            }

            // Copy the list of extents.
            for (int i = 0; i < saDocument._extents.Count; i++)
            {
                _extents.Add((Extent)saDocument._extents[i].Clone());
            }
        }
        #endregion Constructors

        #region Properties
        public int NumElements
        {
            get
            {
                return _elements.Count;
            }
        }
        public int NumExtents
        {
            get
            {
                return _extents.Count;
            }
        }
        public int BlankingLayer { get; set; }
        public bool HasUnsavedChanges
        {
            get
            {
                return _hasUnsavedChanges;
            }
            set
            {
                _hasUnsavedChanges = value;
            }
        }
        public bool ModflowBasicDataDefined
        {
            get
            {
                return _modflowBasicDataDefined;
            }
        }
        public bool ChartsNeedRecompute { get; set; }
        public bool MapsNeedRecompute { get; set; }
        public bool TablesNeedRecompute { get; set; }
        public string ReportName
        {
            get
            {
                if (_reportName == null)
                {
                    _reportName = "";
                }
                if (_reportName.Equals(""))
                {
                    _reportName = DEFAULT_REPORT_NAME;
                }
                return _reportName;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                    if (!value.Equals(""))
                    {
                        _reportName = value;
                    }
                }
            }
        }
        public string Author
        {
            get
            {
                return _author;
            }
            set
            {
                _author = value;
            }
        }
        public string ShortFileName
        {
            get
            {
                // If there's a stable file name, return a short version of it.
                if (_stableFileName != "")
                {
                    return Path.GetFileNameWithoutExtension(_stableFileName);
                }

                // Otherwise, return the temporary file name.
                else
                {
                    return _temporaryFileName;
                }
            }
        }
        public string ShortFileNameWithExtension
        {
            get
            {
                // If there's a stable file name, return a short version of it with the extension.
                if (_stableFileName != "")
                {
                    return Path.GetFileName(_stableFileName);
                }

                // Otherwise, return the temporary file name.
                else
                {
                    return _temporaryFileName;
                }
            }
        }
        public string StableFileName
        {
            get
            {
                return _stableFileName;
            }
            set
            {
                _stableFileName = value;
            }
        }
        public string HnofloText
        {
            get
            {
                return _hnofloText;
            }
            set
            {
                _hnofloText = value;
                ModflowReader.HnofloText = value;
            }
        }
        public string HdryText
        {
            get
            {
                return _hdryText;
            }
            set
            {
                _hdryText = value;
                ModflowReader.HdryText = value;
            }
        }
        public string CinactText
        {
            get
            {
                return _cinactText;
            }
            set
            {
                _cinactText = value;
                ModflowReader.CinactText = value;
            }
        }
        public string NameFile 
        {
            get
            {
                return _nameFile;
            }
            set
            {
                string origNameFile = _nameFile;
                _nameFile = value;
                if (value != origNameFile)
                {
                    if (value == "")
                    {
                        _modflowBasicDataDefined = false;
                    }
                    else
                    {
                        _modflowBasicDataDefined = readModflowNameDisBas();
                    }
                }
            }
        }
        public List<Extent> Extents
        {
            get
            {
                return _extents;
            }            
        }
        public Extent ModelGridExtent { get; private set; }
        public DateTime FileModificationDate 
        {
            get
            {
                if (File.Exists(_stableFileName))
                {
                    return File.GetLastWriteTime(_stableFileName);
                }
                else
                {
                    return System.DateTime.MinValue;
                }
            }
        }
        public DataClasses.TemporalReference.ModflowTimeUnit ModelTimeUnit
        {
            get
            {
                if (GlobalStaticVariables.GlobalTemporalReference == null)
                {
                    GlobalStaticVariables.GlobalTemporalReference = new DataClasses.TemporalReference();
                }
                return GlobalStaticVariables.GlobalTemporalReference.ModelTimeUnit;
            }
            set
            {
                if (GlobalStaticVariables.GlobalTemporalReference == null)
                {
                    GlobalStaticVariables.GlobalTemporalReference = new DataClasses.TemporalReference(value);
                }
                else
                {
                    GlobalStaticVariables.GlobalTemporalReference.ModelTimeUnit = value;
                }
            }
        }
        public DataClasses.LengthReference.ModflowLengthUnit ModflowLengthUnit { get; set; }
        public DataClasses.MapEnums.BlankingMode BlankingMode { get; set; }
        #endregion Properties

        #region IStaticDataSource Members
        public Stream GetSource()
        {
            return new SADocumentXmlStream(this);
        }
        #endregion IStaticDataSource Members

        #region Public methods
        public void AddElement(IReportElement element)
        {
            // Add the element to the list.
            _elements.Add(element);

            // Flag that the document has unsaved changes.
            this.HasUnsavedChanges = true;
        }
        public void AddExtent(Extent extent)
        {
            if (!extent.IsNull)
            {
                _extents.Add(extent);
            }
        }
        public void RemoveElement(IReportElement element)
        {
            _elements.Remove(element);
        }
        public void RemoveExtent(string name)
        {
            for (int i = 0; i < _extents.Count; i++)
            {
                if (name == _extents[i].Name)
                {
                    _extents.RemoveAt(i);
                    break;
                }
            }
        }
        public void RemoveExtent(Extent extent)
        {
            _extents.Remove(extent);
        }
        public void RemoveExtent(int index)
        {
            _extents.RemoveAt(index);
        }
        public IReportElement GetElement(int index)
        {
            return _elements[index];
        }
        public Extent GetExtent(int index)
        {
            return _extents[index];
        }
        public void InitFromSAFile(string fileName, BackgroundWorker worker)
        {
            ZipFile file = null;
            int progress = 1;
            if (worker != null)
            {
                worker.ReportProgress(progress);
            }

            try
            {
                // Store the file path as the stable file path.
                _stableFileName = fileName;

                // Open the zip file.
                file = new ZipFile(fileName);
                GlobalStaticVariables.CurrentDirectory = Path.GetDirectoryName(fileName);
                Directory.SetCurrentDirectory(GlobalStaticVariables.CurrentDirectory);

                // Get the XML entry.
                Stream xmlEntry = file.GetInputStream(file.GetEntry("document.xml"));

                // Load the elements from the XML file.
                XmlElement root = XmlUtil.GetRootOfFile(xmlEntry);
                int progressIncrement = 29;
                if (root.ChildNodes.Count > 1)
                {
                    progressIncrement = 29 / root.ChildNodes.Count;
                }

                // Get the report name and author and file names.
                this.ReportName = XmlUtil.SafeGetStringAttribute(root, XML_KEY_REPORT_NAME, DEFAULT_REPORT_NAME);
                this.Author = XmlUtil.SafeGetStringAttribute(root, XML_KEY_AUTHOR, DEFAULT_AUTHOR);
                GlobalStaticVariables.BackgroundImageFile = XmlUtil.SafeGetStringAttribute(root, XML_KEY_BACKGROUND_IMAGE_FILE, DEFAULT_BACKGROUND_IMAGE_FILE);

                // Get other global data, which may be needed for processing element data
                ScenarioTools.Spatial.StaticObjects.GridShapefilePath = XmlUtil.SafeGetStringAttribute(root, XML_KEY_GRID_SHAPEFILE, DEFAULT_GRID_SHAPEFILE);
                this.AddExtentOfModelGrid();
                this.HnofloText = XmlUtil.SafeGetStringAttribute(root, XML_KEY_HNOFLO_TEXT, DEFAULT_HNOFLO_TEXT);
                this.HdryText = XmlUtil.SafeGetStringAttribute(root, XML_KEY_HDRY_TEXT, DEFAULT_HDRY_TEXT);
                this.CinactText = XmlUtil.SafeGetStringAttribute(root, XML_KEY_CINACT_TEXT, DEFAULT_CINACT_TEXT);
                string simStartTime = XmlUtil.SafeGetStringAttribute(root, XML_KEY_SIMULATION_START_TIME, DEFAULT_SIMULATION_START_TIME);
                GlobalStaticVariables.GlobalTemporalReference.SimulationStartTime = XmlUtil.DateFromXmlString(simStartTime, defaultSimulationStartTime());
                string timeUnit = XmlUtil.SafeGetStringAttribute(root, XML_KEY_MODFLOW_TIME_UNIT, DEFAULT_MODFLOW_TIME_UNIT);
                GlobalStaticVariables.GlobalTemporalReference.ModelTimeUnit = (DataClasses.TemporalReference.ModflowTimeUnit)Convert.ToInt32(timeUnit);
                string lengthUnit = XmlUtil.SafeGetStringAttribute(root, XML_KEY_MODFLOW_LENGTH_UNIT, DEFAULT_MODFLOW_LENGTH_UNIT);
                this.ModflowLengthUnit = (DataClasses.LengthReference.ModflowLengthUnit)Convert.ToInt32(lengthUnit);

                // Get data related to blanking of inactive areas
                this.NameFile = XmlUtil.SafeGetStringAttribute(root, XML_KEY_NAME_FILE, DEFAULT_NAME_FILE);
                this.BlankingLayer = XmlUtil.SafeGetIntAttribute(root, XML_KEY_BLANKING_LAYER, 1);
                int blankingMode = XmlUtil.SafeGetIntAttribute(root, XML_KEY_BLANKING_MODE, 0);
                this.BlankingMode = (ScenarioTools.DataClasses.MapEnums.BlankingMode)blankingMode;
                if (File.Exists(this.NameFile))
                {
                    GlobalStaticVariables.DefineBlanking(BlankingMode, BlankingLayer);
                }

                // Make a new list for the report elements.
                _elements = new List<IReportElement>();

                // Clear existing extents
                _extents.Clear();

                // Make a report element or extent from each child node.
                foreach (XmlElement child in root.ChildNodes)
                {
                    if (child.HasAttribute("NodeType"))
                    {
                        string nodeType = child.GetAttribute("NodeType");
                        if (nodeType == WorkspaceUtil.XML_NODE_TYPE_REPORT_ELEMENT)
                        {
                            // Make a report element from the XML node.
                            IReportElement element = ReportElementFactory.FromXml(child, fileName);

                            // Add the element to the list.
                            _elements.Add(element);
                        }
                        else if (nodeType == Extent.XML_NODE_TYPE_EXTENT)
                        {
                            // Make an extent from the XML node
                            Extent extent = Extent.FromXml(child);

                            // Add the extent to the list
                            _extents.Add(extent);
                        }
                    }
                    if (worker != null)
                    {
                        progress += progressIncrement;
                        progress = Math.Min(progress, 100);
                        worker.ReportProgress(progress);
                    }
                }

                if (_elements.Count > 0)
                {
                    progressIncrement = 29 / _elements.Count;
                }
                else
                {
                    progressIncrement = 29;
                }
                // Load the data caches from the zip file.
                foreach (IReportElement element in _elements)
                {
                    for (int i = 0; i < element.NumDataSeries; i++)
                    {
                        DataSeries dataSeries = element.GetDataSeries(i);

                        ZipEntry cacheEntry = file.GetEntry(dataSeries.DataProvider.Key + ".bin");
                        if (cacheEntry != null)
                        {
                            dataSeries.DataProvider.LoadCacheFromStream(file.GetInputStream(cacheEntry), FileModificationDate);
                        }
                    }
                    if (worker != null)
                    {
                        progress += progressIncrement;
                        progress = Math.Min(progress, 100);
                        worker.ReportProgress(progress);
                    }
                }

                if (_extents.Count > 0)
                {
                    progressIncrement = 29 / _extents.Count;

                    // Get the Extents data caches from the zip file
                    foreach (Extent extent in _extents)
                    {
                        ZipEntry cacheEntry = file.GetEntry(extent.Key + ".bin");
                        if (cacheEntry != null)
                        {
                            extent.LoadCacheFromStream(file.GetInputStream(cacheEntry));
                        }
                        if (worker != null)
                        {
                            progress += progressIncrement;
                            progress = Math.Min(progress, 100);
                            worker.ReportProgress(progress);
                        }
                    }
                }

                // For each ReportElementSTMap element, assign desired extent
                foreach (IReportElement element in _elements)
                {
                    if (element is ReportElementSTMap)
                    {
                        ReportElementSTMap elementSTMap = (ReportElementSTMap)element;
                        elementSTMap.AssignDesiredExtent(_extents);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("There was an error opening the file " + fileName + " -- " + e.Message + ".", "Invalid Scenario Analyzer File", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Close the zip file.
            if (file != null)
            {
                try
                {
                    file.Close();
                }
                catch { }
            }

            // Report progress
            if (worker != null)
            {
                progress = 100;
                worker.ReportProgress(progress);
            }
        }
        public bool IsInUse(Extent extent)
        {
            string extentName = extent.Name;
            for (int i = 0; i < _elements.Count; i++)
            {
                if (_elements[i] is ReportElementSTMap)
                {
                    ReportElementSTMap reportElementSTMap = (ReportElementSTMap)_elements[i];
                    if (reportElementSTMap.ExtentName == extentName)
                    {
                        return true;
                    }

                    // Don't allow user to delete the "model grid" extent!
                    if (extent.Name == WorkspaceUtil.MODEL_GRID_EXTENT_NAME)
                    {
                        return true;
                    }

                    // Don't allow user to delete the "background image" extent!
                    if (extent.Name == WorkspaceUtil.BACKGROUND_IMAGE_EXTENT_NAME)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void AddExtentOfModelGrid()
        {
            if (ScenarioTools.Spatial.StaticObjects.Grid != null)
            {
                // Add extent for model grid
                USGS.Puma.FiniteDifference.CellCenteredArealGrid modelGrid = ScenarioTools.Spatial.StaticObjects.Grid;
                IPolygon polygon = modelGrid.GetPolygon();
                IEnvelope envelope = polygon.EnvelopeInternal;
                Extent gridExtent = new Extent(envelope);
                gridExtent.Name = WorkspaceUtil.MODEL_GRID_EXTENT_NAME;
                this.RemoveExtent(gridExtent.Name);
                this.AddExtent(gridExtent);
                WorkspaceUtil.SetDefaultExtent(gridExtent);
                ModelGridExtent = gridExtent;
            }
        }
        public void AddExtentOfBackgroundImage()
        {
            // Add extent for background image
            if (GlobalStaticVariables.BackgroundImageLayer != null)
            {
                Extent backgroundImageExtent = new Extent(GlobalStaticVariables.BackgroundImageLayer.Extent);
                backgroundImageExtent.Name = WorkspaceUtil.BACKGROUND_IMAGE_EXTENT_NAME;
                this.RemoveExtent(backgroundImageExtent.Name);
                this.AddExtent(backgroundImageExtent);
            }
        }
        public void ClearElements()
        {
            _elements.Clear();
        }
        public void ClearExtents()
        {
            _extents.Clear();
        }
        public bool Save(string fileName)
        {
            // If the file doesn't end with the ".sa" extension, append it.
            if (!fileName.ToLower().EndsWith(".sa"))
            {
                fileName += ".sa";
            }

            // Delete the file if it already exists
            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
            catch
            {
                string msg = "Error: File (" + fileName + ") cannot be overwritten.  Save failed.";
                MessageBox.Show(msg);
                return false;
            }

            // Open the file for writing.
            FileStream fileStream = File.OpenWrite(fileName);

            // Make a zip file from the stream.
            ZipOutputStream zipFile = new ZipOutputStream(fileStream);

            // Write the XML file to the zip.
            ZipEntry xmlEntry = new ZipEntry("document.xml");
            zipFile.PutNextEntry(xmlEntry);

            MemoryStream ms = new MemoryStream();
            using (XmlWriter writer = XmlWriter.Create(ms))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("ScenarioAnalyzerDocument");
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }

            // Create a document object from the memory stream.
            ms.Position = 0;
            XmlDocument document = new XmlDocument();
            document.Load(ms);

            // Add the author and title and file names as document attributes.
            XmlUtil.FrugalSetAttribute(document.DocumentElement, XML_KEY_REPORT_NAME, this.ReportName, DEFAULT_REPORT_NAME);
            XmlUtil.FrugalSetAttribute(document.DocumentElement, XML_KEY_AUTHOR, this.Author, DEFAULT_AUTHOR);
            XmlUtil.FrugalSetAttribute(document.DocumentElement, XML_KEY_BACKGROUND_IMAGE_FILE, GlobalStaticVariables.BackgroundImageFile, DEFAULT_BACKGROUND_IMAGE_FILE);
            XmlUtil.FrugalSetAttribute(document.DocumentElement, XML_KEY_GRID_SHAPEFILE, ScenarioTools.Spatial.StaticObjects.GridShapefilePath, DEFAULT_GRID_SHAPEFILE);
            XmlUtil.FrugalSetAttribute(document.DocumentElement, XML_KEY_HNOFLO_TEXT, this.HnofloText, DEFAULT_HNOFLO_TEXT);
            XmlUtil.FrugalSetAttribute(document.DocumentElement, XML_KEY_HDRY_TEXT, this.HdryText, DEFAULT_HDRY_TEXT);
            XmlUtil.FrugalSetAttribute(document.DocumentElement, XML_KEY_CINACT_TEXT, this.CinactText, DEFAULT_CINACT_TEXT);
            string simStartDate = XmlUtil.DateToXmlString(GlobalStaticVariables.GlobalTemporalReference.SimulationStartTime);
            XmlUtil.FrugalSetAttribute(document.DocumentElement, XML_KEY_SIMULATION_START_TIME, simStartDate, DEFAULT_SIMULATION_START_TIME);
            string timeUnit = ((int)GlobalStaticVariables.GlobalTemporalReference.ModelTimeUnit).ToString();
            XmlUtil.FrugalSetAttribute(document.DocumentElement, XML_KEY_MODFLOW_TIME_UNIT, timeUnit, DEFAULT_MODFLOW_TIME_UNIT);
            string lengthUnit = ((int)this.ModflowLengthUnit).ToString();
            XmlUtil.FrugalSetAttribute(document.DocumentElement, XML_KEY_MODFLOW_LENGTH_UNIT, lengthUnit, DEFAULT_MODFLOW_LENGTH_UNIT);

            // Add settings related to blanking of inactive areas
            XmlUtil.FrugalSetAttribute(document.DocumentElement, XML_KEY_NAME_FILE, this.NameFile, DEFAULT_NAME_FILE);
            string blankingLayer = this.BlankingLayer.ToString();
            XmlUtil.FrugalSetAttribute(document.DocumentElement, XML_KEY_BLANKING_LAYER, blankingLayer, DEFAULT_BLANKING_LAYER);
            string blankingMode = ((int)this.BlankingMode).ToString();
            XmlUtil.FrugalSetAttribute(document.DocumentElement, XML_KEY_BLANKING_MODE, blankingMode, DEFAULT_BLANKING_MODE);

            // Add the elements to the document.
            for (int i = 0; i < _elements.Count; i++)
            {
                document.DocumentElement.AppendChild(_elements[i].GetXmlNode(document, fileName));
            }

            // Add the extents to the XML document.
            for (int i = 0; i < _extents.Count; i++)
            {
                document.DocumentElement.AppendChild(_extents[i].GetXmlNode(document, fileName));
            }

            // Make a new memory stream and save the report to it.
            MemoryStream ms2 = new MemoryStream();
            document.Save(ms2);

            // Write the memory stream to the zip file.
            byte[] buffer = new byte[1024];
            ms2.Position = 0;
            int bytesToWrite = ms2.Read(buffer, 0, 1024);
            while (bytesToWrite > 0)
            {
                zipFile.Write(buffer, 0, bytesToWrite);
                bytesToWrite = ms2.Read(buffer, 0, 1024);
            }

            // Write the data caches to the zip file.
            foreach (IReportElement element in _elements)
            {
                for (int i = 0; i < element.NumDataSeries; i++)
                {
                    // Get the dataset through a synchronous call.
                    DataSeries dataSeries = element.GetDataSeries(i);
                    object dataset = dataSeries.GetDataSynchronous();

                    // If the dataset is not null, add it to the file.
                    if (dataset != null)
                    {
                        // Convert the dataset to an array of bytes.
                        byte[] datasetByteArray = WorkspaceUtil.ConvertCacheToByteArray(dataset);

                        // If the byte array is good, write it to the file.
                        if (datasetByteArray != null)
                        {
                            if (datasetByteArray.Length > 0)
                            {
                                // Make the entry for the cache.
                                ZipEntry cacheEntry = new ZipEntry(dataSeries.DataProvider.Key + ".bin");
                                zipFile.PutNextEntry(cacheEntry);
                                zipFile.Write(datasetByteArray, 0, datasetByteArray.Length);
                                zipFile.CloseEntry();
                            }
                        }
                    }
                }
            }

            // Write the Extent coordinates and bool values to the zip file.
            foreach (Extent extent in _extents)
            {
                byte[] extentByteArray = WorkspaceUtil.ConvertCacheToByteArray(extent);
                // If the byte array is good, write it to the file.
                if (extentByteArray != null)
                {
                    if (extentByteArray.Length > 0)
                    {
                        // Make the entry for the cache.
                        ZipEntry cacheEntry = new ZipEntry(extent.Key + ".bin");
                        zipFile.PutNextEntry(cacheEntry);
                        zipFile.Write(extentByteArray, 0, extentByteArray.Length);
                        zipFile.CloseEntry();
                    }
                }
            }

            // Flush and close the zip file.
            zipFile.Flush();
            zipFile.Close();

            // Flag that the document does not have unsaved changes.
            this._hasUnsavedChanges = false;
            return true;
        }
        #endregion Public methods

        #region Private methods
        private static DateTime defaultSimulationStartTime()
        {
            return new DateTime(1900, 1, 1);
        }
        private static bool gridDimensionMismatch()
        {
            bool ok = false;
            if (ScenarioTools.Spatial.StaticObjects.Grid != null && GlobalStaticVariables.DisFileData != null)
            {
                int nR = GlobalStaticVariables.DisFileData.RowCount;
                int nC = GlobalStaticVariables.DisFileData.ColumnCount;
                if (ScenarioTools.Spatial.StaticObjects.Grid.ColumnCount != nC ||
                    ScenarioTools.Spatial.StaticObjects.Grid.RowCount != nR)
                {
                    string errMsg =
                        "Error: Grid dimension mismatch between MODFLOW Discretization file and grid shapefile.";
                    MessageBox.Show(errMsg);
                    ok = true;
                }
            }
            return ok;
        }
        private bool readModflowNameDisBas()
        {
            bool Ok = true;
            string currentAction = "";
            try
            {
                currentAction = "reading MODFLOW Name file: " + NameFile;
                GlobalStaticVariables.ModflowNameData = USGS.Puma.Modflow.ModflowNameFileReader.Read(NameFile);
                if (GlobalStaticVariables.ModflowNameData == null)
                {
                    throw new Exception();
                }
                USGS.Puma.Modflow.NameFileItem disFileItem = GlobalStaticVariables.ModflowNameData.GetItemsAsList("DIS")[0];
                string disFile = disFileItem.FileName;
                int disUnit = disFileItem.FileUnit;
                currentAction = "reading MODFLOW Discretization file: " + disFile;
                USGS.Puma.Modflow.DisDataReader disDataReader = new USGS.Puma.Modflow.DisDataReader();
                GlobalStaticVariables.DisFileData = disDataReader.Read(GlobalStaticVariables.ModflowNameData);
                if (GlobalStaticVariables.DisFileData == null)
                {
                    throw new Exception();
                }
                int nLay = GlobalStaticVariables.DisFileData.LayerCount;
                int nRow = GlobalStaticVariables.DisFileData.RowCount;
                int nCol = GlobalStaticVariables.DisFileData.ColumnCount;
                string basFile = GlobalStaticVariables.ModflowNameData.GetItemsAsList("BAS6")[0].FileName;
                currentAction = "reading MODFLOW Basic Package file: " + basFile;
                USGS.Puma.Modflow.BasDataReader basDataReader = new USGS.Puma.Modflow.BasDataReader();
                GlobalStaticVariables.BasFileData = basDataReader.Read(GlobalStaticVariables.ModflowNameData, nLay, nRow, nCol);
                if (GlobalStaticVariables.BasFileData == null)
                {
                    throw new Exception();
                }
                if (gridDimensionMismatch())
                {
                    throw new Exception();
                }
            }
            catch
            {
                string errMsg = "Error encountered while " + currentAction;
                MessageBox.Show(errMsg);
                Ok = false;
            }
            return Ok;
        }
        #endregion Private methods
    }
}
