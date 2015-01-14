using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

using OSGeo.GDAL;

using ScenarioTools;
using ScenarioTools.Data_Providers;
using ScenarioTools.DataClasses;
using ScenarioTools.Dialogs;
using ScenarioTools.Gui;
using ScenarioTools.ImageProvider;
using ScenarioTools.LogAndErrorProcessing;
using ScenarioTools.PdfWriting;
using ScenarioTools.Reporting;
using ScenarioTools.Spatial;
using ScenarioTools.TreeElementFactories;
using ScenarioTools.UndoItems;
using ScenarioTools.Xml;

using SoftwareProductions.Utilities.Undo;

namespace ScenarioAnalyzer
{
    public sealed partial class SAMainForm : Form, ILogger
    {
        private static readonly Image DEFAULT_IMAGE = Properties.Resources.usgs_logo_48;
        private const string APPLICATION_NAME = "USGS Scenario Analyzer";

        #region Fields
        private bool _generatingBackgroundImage = false;
        private bool _usingDialog = false;
        private bool _readingFile = false;
        private bool _calculatedLayoutValues = false;
        private bool _mainFormIsAlive;
        private bool _okToClose = true;
        private bool _needRefreshOfSelectedNode;
        private bool _refreshingImage = false;
        private bool _refreshingTreeIcons = false;
        private bool _autoRefresh = true;
        private int _maxUndoCount = 0;
        private int _treeViewMarginL;
        private int _treeViewMarginR;
        private int _treeViewMarginT;
        private int _treeViewMarginB;
        private int _pictureBoxMarginR;
        private int _pictureBoxMarginB;
        private int _treeRefreshIconIndex = 0;
        readonly Cursor _defaultCursor;
        private readonly DocumentHolder _documentHolder = new DocumentHolder();
        private SADocument Document
        {
            get
            {
                return _documentHolder.SADocument;
            }
            set
            {
                _documentHolder.SADocument = value;
            }
        }
        private SADocument _oldDocument;
        private TreeNode _treeViewSelectedNode;
        private UndoChain _undoChain = new UndoChain();
        private TreeNode _lastSelectedNode;
        #endregion Fields

        #region Constructor
        public SAMainForm(SADocument document)
        {
            InitializeComponent();

            _okToClose = true;
            var defaultCursor = this.Cursor;
            if (defaultCursor != null) _defaultCursor = defaultCursor;

            // Set the image list of the tree view.
            treeView.ImageList = IconHelper.GetIconList();

            // Register this form as a logger. The updates will be displayed in the information bar.
            Logging.RegisterLogger(this);

            // If the provided document is not null, set it as the current document.
            if (document != null)
            {
                Document = document;
                document.AddExtentOfModelGrid();
            }
            // Otherwise, make a new, empty document.
            else
            {
                Document = new SADocument();
                getUserSettings();
            }

            // Provide global access to list of extents
            GlobalStaticVariables.Extents = Document.Extents;

            // Assign GlobalLengthReference
            ScenarioTools.GlobalStaticVariables.GlobalLengthUnit = Document.ModflowLengthUnit;

            // Set the tree view context menu.
            treeView.ContextMenu = makeTreeContextMenu();

            // Add the event handler to refresh the right pane when an element in the tree is selected.
            treeView.AfterSelect += new TreeViewEventHandler(treeView_AfterSelect);
            treeView.MouseClick += new MouseEventHandler(treeView_MouseClick);

            // Perform an initial refresh of the tree.
            refreshTree();

            //Setup the undo chain events so we can  select the Control 
            //containing the detail that was modified by an undo/redo operation,
            //or update the contacts list if we need to.
            // I think these two lines never do anything
            _undoChain.Undone += new UndoActionEventHandler(UndoChain_Acted);
            _undoChain.Redone += new UndoActionEventHandler(UndoChain_Acted);

            // Save the original document
            saveOldDocument();

            //// Start a timer to refresh the tree.
            System.Windows.Forms.Timer interfaceRefreshTimer = new System.Windows.Forms.Timer();
            interfaceRefreshTimer.Interval = 1000; // Ned TODO: Reset to original value: 1000 msec
            //interfaceRefreshTimer.Interval = 10000; // 10000 msec = 10 sec
            //interfaceRefreshTimer.Interval = 30000; // 30000 msec = 30 sec
            //interfaceRefreshTimer.Interval = 300000; // 300000 msec = 5 min
            //interfaceRefreshTimer.Interval = 3000000; // 3000000 msec = 50 min
            interfaceRefreshTimer.Tick += new EventHandler(interfaceRefreshTimer_Tick);
            interfaceRefreshTimer.Start();

            // Start the thread that refreshes the datasets.
            _mainFormIsAlive = true;
            Thread datasetRefreshThread = new Thread(new ThreadStart(refreshDatasets));
            datasetRefreshThread.Start();
        }
        #endregion Constructor

        private void refreshDatasets()
        {
            // Loop as long as the program is running.
            while (_mainFormIsAlive)
            {
                //Console.WriteLine(DateTime.Now + ": refreshing datasets");

                // First, check if the currently selected node does not have a valid dataset. If this is the case, load its dataset (or datasets).
                // When each dataset is loaded, flag that the image needs to be refreshed.
                TreeNode selectedNode = _treeViewSelectedNode;
                object selectedElementOrDataSeries = null;
                if (selectedNode != null)
                {
                    selectedElementOrDataSeries = selectedNode.Tag;
                    if (selectedElementOrDataSeries != null)
                    {
                        if (selectedElementOrDataSeries is IReportElement)
                        {
                            // Request the data series from each data provider.
                            IReportElement element = (IReportElement)selectedElementOrDataSeries;
                            for (int i = 0; i < element.NumDataSeries; i++)
                            {
                                // Determine the current data status.
                                int dataStatus;
                                DataSeries dataSeries = element.GetDataSeries(i);
                                dataSeries.GetData(out dataStatus);

                                // If the status is anything other than good, refresh the dataset.
                                if (dataStatus != DataStatus.DATA_AVAILABLE_CACHE_PRESENT)
                                {
                                    element.GetDataSeries(i).GetDataSynchronous();
                                    _needRefreshOfSelectedNode = true;
                                }
                            }
                        }
                        else if (selectedElementOrDataSeries is DataSeries)
                        {
                            // Request the data from the data provider.
                            DataSeries dataSeries = (DataSeries)selectedElementOrDataSeries;
                            int dataStatus;
                            dataSeries.GetData(out dataStatus);

                            // If the status is anything other than good, refresh the dataset.
                            if (dataStatus != DataStatus.DATA_AVAILABLE_CACHE_PRESENT)
                            {
                                dataSeries.GetDataSynchronous();
                                _needRefreshOfSelectedNode = true;
                            }
                        }
                    }
                }

                // Now, look for any one other dataset to refresh.
                bool refreshedOneDataset = false;
                    // Iterate through all data series in all elements. Skip the selected element or data series.
                for (int j = 0; j < Document.NumElements && !refreshedOneDataset; j++)
                {
                    // Get the element from the document.
                    IReportElement element = Document.GetElement(j);

                    // Only continue if this is not the selected element.
                    if (element != selectedElementOrDataSeries)
                    {
                        for (int i = 0; i < element.NumDataSeries && !refreshedOneDataset; i++)
                        {
                            DataSeries dataSeries = element.GetDataSeries(i);
                            if (dataSeries != selectedElementOrDataSeries)
                            {
                                // Check the data status.
                                int dataStatus;
                                dataSeries.GetData(out dataStatus);

                                // If the data status is anything other than "good," refresh the data.
                                if (dataStatus != DataStatus.DATA_AVAILABLE_CACHE_PRESENT)
                                {
                                    // Force loading of the data.
                                    dataSeries.GetDataSynchronous();

                                    // Check the data series again. If it is now good, mark that we've refreshed a dataset.
                                    dataSeries.GetData(out dataStatus);
                                    if (dataStatus == DataStatus.DATA_AVAILABLE_CACHE_PRESENT)
                                    {
                                        refreshedOneDataset = true;
                                        Console.WriteLine("Successfully loaded a data series named: " + dataSeries.Name);
                                    }
                                }
                            }
                        }
                    }
                }

                // Sleep for a little while (1 second).
                Thread.Sleep(1000);
            }
        }

        void interfaceRefreshTimer_Tick(object sender, EventArgs e)
        {
            if (!_generatingBackgroundImage && !_readingFile 
                && !_usingDialog && !_refreshingTreeIcons)
            {
                if (_autoRefresh)
                {
                    // Refresh the tree icons.
                    refreshTreeIcons();

                    // Refresh the selected image.
                    refreshImageOfSelectedNode();
                }
            }
        }

        private void validateDataProviderKeys()
        {
            // This method ensures that no two data providers in the workspace have 
            // the same unique identifier. The situation where two data providers do
            // have the same unique identifier will occur quite freuqently -- this 
            // will happen when a data provider is loaded from an XML file and the
            // data provider that was used to create that XML file is still present 
            // in the workspace.

            // This is the list that holds the unique identifiers. It is passed to 
            // all data providers. If a data provider finds that its identifier is
            // already contained in the list, the data provider assigns itself a new 
            // identifier. Each data provider adds its unique identifier to the 
            // list before returning. 
            List<long> uniqueIdentifiers = new List<long>();

            for (int i = 0; i < Document.NumElements; i++)
            {
                Document.GetElement(i).ValidateDataProviderKeys(uniqueIdentifiers);
            }
        }
        private void refreshTree()
        {
            // Validate the data provider keys.
            validateDataProviderKeys();

            // Suspend the form layout while manipulating the tree.
            SuspendLayout();

            // Get the expansion state of the tree.
            TreeSignature treeSignature = TreeSignature.GetTreeSignature(treeView);

            // Clear the tree.
            treeView.Nodes.Clear();

            // Make a node from each element and add it to the tree.
            for (int i = 0; i < Document.NumElements; i++)
            {
                TreeNode elementNode = makeElementNode(Document.GetElement(i));
                treeView.Nodes.Add(elementNode);
            }

            // Impose the signature on the tree.
            treeSignature.Impose(treeView);

            // Update the tree icons.
            refreshTreeIcons();

            // This is a good place to update the title bar. 
            // It may not be necessary, but it's a cheap operation.
            updateTitleBar();

            // Tree manipulations have been completed; resume the form layout.
            this.ResumeLayout();

            Console.WriteLine();
        }

        ContextMenu makeTreeContextMenu()
        {
            // Make the menu items.
            MenuItem menuItemNewSTMap = new MenuItem("Add New Map");
            MenuItem menuItemNewChart = new MenuItem("Add New Chart");
            MenuItem menuItemNewTable = new MenuItem("Add New Table");
            MenuItem menuItemExportPdfReport = new MenuItem("Export to PDF");
            MenuItem menuItemProperties = new MenuItem("Project Settings");

            // Add the event handlers.
            menuItemNewSTMap.Click += new EventHandler(menuItemNewSTMap_Click);
            menuItemNewChart.Click += new EventHandler(menuItemNewChart_Click);
            menuItemNewTable.Click += new EventHandler(menuItemNewTable_Click);
            menuItemExportPdfReport.Click += new EventHandler(menuItemExportPdfReport_Click);
            menuItemProperties.Click += new EventHandler(menuItemProjectSettings_Click);

            // Make the context menu.
            ContextMenu contextMenu = new ContextMenu(new MenuItem[] { menuItemNewSTMap, 
                menuItemNewChart, menuItemNewTable, new MenuItem("-"), 
                menuItemExportPdfReport, new MenuItem("-"), menuItemProperties });

            // Return the result.
            return contextMenu;
        }

        void menuItemSaveXmlReport_Click(object sender, EventArgs e)
        {
            // Get the report from the menu item.
            Report report = (Report)((MenuItem)sender).Tag;

            // Show a dialog for saving the XML file. If accepted, save the file.
            SaveFileDialog dialog = DialogHelper.GetXmlSaveFileDialog();
            _usingDialog = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                report.SaveXmlFile(dialog.FileName);

                // Show a confirmation message.
                MessageBox.Show("Wrote XML file to " + dialog.FileName);
            }
            _usingDialog = false;
        }
        void menuItemExportPdfReport_Click(object sender, EventArgs e)
        {
            // Show a dialog for saving the PDF.
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Portable Document Format | *.pdf";

            // Show the dialog. If the user accepts, save the PDF.
            _usingDialog = true;
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string fileName = saveDialog.FileName;
                    PdfWriter.ExportPDF(Document, fileName);

                    // Display a success message.
                    MessageBox.Show("Saved report to " + fileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to save report: " + ex.Message);
                }
            }
            _usingDialog = false;
        }

        void menuItemExportCsvFile_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode.Tag is ReportElementTable)
            {
                ReportElementTable table = (ReportElementTable)treeView.SelectedNode.Tag;

                // Show a dialog for saving the PDF.
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Comma-Separated Value File | *.csv";

                // Show the dialog. If the user accepts, save the PDF.
                _usingDialog = true;
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string fileName = saveDialog.FileName;

                        table.ExportCsvFile(fileName);

                        // Display a success message.
                        MessageBox.Show("Saved table data in CSV format to " + fileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to save table data in CSV format: " + ex.Message);
                    }
                }
                _usingDialog = false;
            }
        }
        
        TreeNode makeElementNode(IReportElement element)
        {
            // Make the node.
            TreeNode node = new TreeNode(element.ToString());
            node.ImageIndex = IconHelper.BLANK_INDEX;
            node.SelectedImageIndex = node.ImageIndex;

            // Set the element as the tag of the node.
            node.Tag = element;

            // For any type of element, create Duplicate and Delete menu items
            MenuItem menuItemDuplicateElement = new MenuItem("Duplicate element");
            menuItemDuplicateElement.Tag = element;
            menuItemDuplicateElement.Click += new EventHandler(menuItemDuplicateElement_Click);
            MenuItem menuItemDeleteElement = new MenuItem("Delete element");
            menuItemDeleteElement.Tag = element;
            menuItemDeleteElement.Click += new EventHandler(menuItemDeleteElement_Click);

            // For any type of element, create "Reread/Recompute data" menu item (invoke InvalidateDataset)
            MenuItem menuItemInvalidateData = new MenuItem("Reread/Recompute data");
            menuItemInvalidateData.Tag = element;
            menuItemInvalidateData.Click += new EventHandler(menuItemInvalidateData_Click);

            // This is the case of a map element.
            if (element is ReportElementSTMap)
            #region Report Element ST Map
            {
                // Set the icon of the node.
                node.ImageIndex = IconHelper.PUMA_MAP_INDEX;
                node.SelectedImageIndex = node.ImageIndex;

                // Make the menu items.
                MenuItem menuItemAddContourLayer = new MenuItem("Add New Contour Layer");
                MenuItem menuItemAddColorFillLayer = new MenuItem("Add New Color-Fill Layer");
                MenuItem menuItemAddXmlLayer = new MenuItem("Add Layer From XML File");
                MenuItem menuItemSaveXmlMap = new MenuItem("Save Map to XML File");
                MenuItem menuItemEditElementProperties = new MenuItem("Properties");

                // Ned TODO: Work on XML export/import later.  
                // For now, make these menu items invisible.
                menuItemAddXmlLayer.Visible = false;
                menuItemSaveXmlMap.Visible = false;

                // Set the report element as the tag of the menu items.
                menuItemAddContourLayer.Tag = element;
                menuItemAddColorFillLayer.Tag = element;
                menuItemAddXmlLayer.Tag = element;
                menuItemSaveXmlMap.Tag = element;
                menuItemEditElementProperties.Tag = element;

                // Add the event handlers.
                menuItemAddContourLayer.Click += new EventHandler(menuItemAddContourLayer_Click);
                menuItemAddColorFillLayer.Click += new EventHandler(menuItemAddColorFillLayer_Click);
                menuItemAddXmlLayer.Click += new EventHandler(menuItemAddXmlLayer_Click);
                menuItemSaveXmlMap.Click += new EventHandler(menuItemSaveXmlMap_Click);
                menuItemEditElementProperties.Click += new EventHandler(menuItemEditElementProperties_Click);

                // Make the context menu.
                node.ContextMenu = new ContextMenu(new MenuItem[] { menuItemAddContourLayer, 
                    menuItemAddColorFillLayer, menuItemAddXmlLayer, new MenuItem("-"), 
                    //menuItemSaveXmlMap, new MenuItem("-"), 
                    menuItemEditElementProperties, 
                    menuItemDuplicateElement, menuItemInvalidateData, menuItemDeleteElement });
            }
            #endregion Report Element ST Map

            // This is the case of a chart or table element.
            else if (element is ReportElementChart)
            #region Report Element Chart
            {
                // Set the icon of the node.
                node.ImageIndex = IconHelper.GRAPH_INDEX;
                node.SelectedImageIndex = node.ImageIndex;

                // Make the menu items.
                MenuItem menuItemAddNewGraphDataSeries = new MenuItem("Add New Data Series");
                MenuItem menuItemAddXmlGraphDataSeries = new MenuItem("Add Data Series From XML File");
                MenuItem menuItemSaveXmlGraph = new MenuItem("Save Chart to XML File");
                MenuItem menuItemGraphProperties = new MenuItem("Properties");

                // Ned TODO: Work on XML export/import later.  
                // For now, make these menu items invisible.
                menuItemAddXmlGraphDataSeries.Visible = false;
                menuItemSaveXmlGraph.Visible = false;

                // Set the element as the tag of the menu items.
                menuItemAddNewGraphDataSeries.Tag = element;
                menuItemAddXmlGraphDataSeries.Tag = element;
                menuItemSaveXmlGraph.Tag = element;
                menuItemGraphProperties.Tag = element;

                // Add the event handlers for the menu items.
                menuItemAddNewGraphDataSeries.Click += new EventHandler(menuItemAddNewChartDataSeries_Click);
                menuItemAddXmlGraphDataSeries.Click += new EventHandler(menuItemAddXmlLayer_Click);
                menuItemSaveXmlGraph.Click += new EventHandler(menuItemSaveXmlMap_Click);
                menuItemGraphProperties.Click += new EventHandler(menuItemEditElementProperties_Click);

                // Make the context menu.
                node.ContextMenu = new ContextMenu(new MenuItem[] { menuItemAddNewGraphDataSeries, 
                    //menuItemAddXmlGraphDataSeries, new MenuItem("-"), menuItemSaveXmlGraph, 
                    new MenuItem("-"), menuItemGraphProperties, menuItemDuplicateElement,
                    menuItemInvalidateData, menuItemDeleteElement });
            }
            #endregion Report Element Chart

            else
            #region ReportElementTable
            {
                // Set the icon of the node.
                node.ImageIndex = IconHelper.TABLE_INDEX;
                node.SelectedImageIndex = node.ImageIndex;

                // Make the menu items.
                MenuItem menuItemAddNewTableDataSeries = new MenuItem("Add New Data Series");
                MenuItem menuItemAddXmlTableDataSeries = new MenuItem("Add Data Series From XML File");
                MenuItem menuItemSaveXmlTable = new MenuItem("Save Table to XML File");
                MenuItem menuItemExportCsvFile = new MenuItem("Export CSV File");
                menuItemExportCsvFile.Name = "menuItemExportCsvFile";
                MenuItem menuItemGraphProperties = new MenuItem("Properties");

                // Ned TODO: Work on XML export/import later.  
                // For now, make these menu items invisible.
                menuItemAddXmlTableDataSeries.Visible = false;
                menuItemSaveXmlTable.Visible = false;

                // Set the element as the tag of the menu items.
                menuItemAddNewTableDataSeries.Tag = element;
                menuItemAddXmlTableDataSeries.Tag = element;
                menuItemSaveXmlTable.Tag = element;
                menuItemExportCsvFile.Tag = element;
                menuItemGraphProperties.Tag = element;

                // Add the event handlers for the menu items.
                menuItemAddNewTableDataSeries.Click += new EventHandler(menuItemAddNewTableDataSeries_Click);
                menuItemAddXmlTableDataSeries.Click += new EventHandler(menuItemAddXmlLayer_Click);
                menuItemSaveXmlTable.Click += new EventHandler(menuItemSaveXmlMap_Click);
                menuItemExportCsvFile.Click += new EventHandler(menuItemExportCsvFile_Click);
                menuItemGraphProperties.Click += new EventHandler(menuItemEditElementProperties_Click);

                // Make the context menu.
                node.ContextMenu = new ContextMenu(new MenuItem[] { menuItemAddNewTableDataSeries, 
                    menuItemAddXmlTableDataSeries, new MenuItem("-"), menuItemSaveXmlTable, 
                    menuItemExportCsvFile, new MenuItem("-"), menuItemGraphProperties, 
                    menuItemDuplicateElement, menuItemInvalidateData, menuItemDeleteElement });
            }
            #endregion ReportElementTable

            // Add nodes for each data series.
            for (int i = 0; i < element.NumDataSeries; i++)
            {
                TreeNode dataSeriesNode;
                DataSeries dataSeries = element.GetDataSeries(i);
                if (dataSeries.DataSeriesType == DataSeriesTypeEnum.ContourMapSeries)
                {
                    dataSeriesNode = makeContourLayerNode(dataSeries);
                }
                else
                {
                    dataSeriesNode = makeDataSeriesNode(dataSeries);
                }
                node.Nodes.Add(dataSeriesNode);
            }

            // Return the result.
            return node;
        }
        void menuItemEditElementProperties_Click(object sender, EventArgs e)
        {
            // Get the report element from the menu item.
            IReportElement element = (IReportElement)((MenuItem)sender).Tag;

            editElementProperties(element);
        }

        private void editElementProperties(IReportElement element)
        {
            // Show a dialog for the report element.
            DialogResult result = DialogResult.No;
            if (element is ReportElementSTMap)
            {
                STMapDesigner stMapDesigner = new STMapDesigner(((ReportElementSTMap)element).STMap, Document.Extents);
                _usingDialog = true;
                result = stMapDesigner.ShowDialog();
                //stMapDesigner.Dispose();
                stMapDesigner = null;
                if (result == DialogResult.OK)
                {
                    ((ReportElementSTMap)element).ClearImage();
                    ((ReportElementSTMap)element).DesiredExtentName = ((ReportElementSTMap)element).STMap.DesiredExtentName;
                    ((ReportElementSTMap)element).BackgroundImageBrightness = ((ReportElementSTMap)element).STMap.BackgroundImageBrightness;
                    _needRefreshOfSelectedNode = true;
                }
                _usingDialog = false;
            }
            else if (element is ReportElementTable)
            {
                _usingDialog = true;
                result = new ReportElementTableMenu((ReportElementTable)element).ShowDialog();
                _usingDialog = false;
            }
            else if (element is ReportElementChart)
            {
                _usingDialog = true;
                result = new ReportElementChartMenu((ReportElementChart)element).ShowDialog();
                _usingDialog = false;
            }

            // If the dialog was accepted, refresh the tree and add an Undo item.
            if (result == DialogResult.OK)
            {
                refreshTree();
                addSADocumentChangeUndoItem();
            }
        }
        private TreeNode makeContourLayerNode(DataSeries dataSeries)
        {
            try
            {
                // Make the node.
                TreeNode node = new TreeNode(dataSeries.ToString());

                // Set the element as the tag of the node.
                node.Tag = dataSeries;

                // Make the menu items.
                MenuItem menuItemSaveDataSeriesToXmlFile = new MenuItem("Save Layer to XML File");
                MenuItem menuItemExportContourShapefile = new MenuItem("Export Shapefile of Contours");
                MenuItem menuItemEditDataSeries = new MenuItem("Properties");
                MenuItem menuItemDeleteLayer = new MenuItem("Delete Layer");

                // Ned TODO: Work on XML export/import later.  
                // For now, make these menu items invisible.
                menuItemSaveDataSeriesToXmlFile.Visible = false;
                menuItemExportContourShapefile.Visible = false;

                // Create "Reread/Recompute data" menu item (invoke InvalidateDataset)
                MenuItem menuItemInvalidateData = new MenuItem("Reread/Recompute data");
                menuItemInvalidateData.Tag = dataSeries;
                menuItemInvalidateData.Click += new EventHandler(menuItemInvalidateData_Click);

                // Set the data series as the tag of the menu items.
                menuItemSaveDataSeriesToXmlFile.Tag = dataSeries;
                menuItemExportContourShapefile.Tag = dataSeries;
                menuItemEditDataSeries.Tag = dataSeries;
                menuItemDeleteLayer.Tag = dataSeries;

                // Add the event handlers.
                menuItemSaveDataSeriesToXmlFile.Click += new EventHandler(menuItemSaveDataSeriesToXmlFile_Click);
                menuItemExportContourShapefile.Click += new EventHandler(menuItemExportContourShapefile_Click);
                menuItemEditDataSeries.Click += new EventHandler(menuItemEditDataSeries_Click);
                menuItemDeleteLayer.Click += new EventHandler(menuItemDeleteLayer_Click);

                // Make the context menu.
                node.ContextMenu = new ContextMenu(new MenuItem[] { 
                    //menuItemSaveDataSeriesToXmlFile, 
                    //menuItemExportContourShapefile, new MenuItem("-"), 
                    menuItemEditDataSeries, 
                    menuItemInvalidateData, menuItemDeleteLayer });

                // Return the result.
                return node;
            }
            catch
            {
                return new TreeNode("ERROR");
            }
        }
        private TreeNode makeDataSeriesNode(DataSeries dataSeries)
        {
            try
            {
                // Make the node.
                TreeNode node = new TreeNode(dataSeries.ToString());

                // Set the element as the tag of the node.
                node.Tag = dataSeries;

                // Make the menu items.
                MenuItem menuItemSaveDataSeriesToXmlFile = new MenuItem("Save Data Series to XML File");
                MenuItem menuItemExportAsciiFile = new MenuItem("Export CSV of Time Series");
                MenuItem menuItemEditDataSeries = new MenuItem("Properties");
                MenuItem menuItemDeleteDataSeries = new MenuItem("Delete Data Series");

                // Ned TODO: Work on XML export/import later.  
                // For now, make these menu items invisible.
                menuItemSaveDataSeriesToXmlFile.Visible = false;

                if (dataSeries.DataSeriesType != DataSeriesTypeEnum.ChartSeries && 
                    dataSeries.DataSeriesType != DataSeriesTypeEnum.TableSeries)
                {
                    menuItemExportAsciiFile.Visible = false;
                }

                // Create "Reread/Recompute data" menu item (invoke InvalidateDataset)
                MenuItem menuItemInvalidateData = new MenuItem("Reread/Recompute data");
                menuItemInvalidateData.Tag = dataSeries;
                menuItemInvalidateData.Click += new EventHandler(menuItemInvalidateData_Click);

                // Set the data series as the tag of the menu items.
                menuItemSaveDataSeriesToXmlFile.Tag = dataSeries;
                menuItemExportAsciiFile.Tag = dataSeries;
                menuItemEditDataSeries.Tag = dataSeries;
                menuItemDeleteDataSeries.Tag = dataSeries;

                // Add the event handlers.
                menuItemSaveDataSeriesToXmlFile.Click += new EventHandler(menuItemSaveDataSeriesToXmlFile_Click);
                menuItemExportAsciiFile.Click += new EventHandler(menuItemExportAsciiFile_Click);
                menuItemEditDataSeries.Click += new EventHandler(menuItemEditDataSeries_Click);
                menuItemDeleteDataSeries.Click += new EventHandler(menuItemDeleteDataSeries_Click);

                // Make the context menu.
                if (menuItemExportAsciiFile.Visible || menuItemSaveDataSeriesToXmlFile.Visible)
                {
                    node.ContextMenu = new ContextMenu(new MenuItem[] { menuItemSaveDataSeriesToXmlFile, 
                        menuItemExportAsciiFile, new MenuItem("-"), menuItemEditDataSeries,
                        menuItemInvalidateData, menuItemDeleteDataSeries });
                }
                else
                {
                    node.ContextMenu = new ContextMenu(new MenuItem[] { menuItemEditDataSeries,
                        menuItemInvalidateData, menuItemDeleteDataSeries });
                }

                // Return the result.
                return node;
            }
            catch
            {
                return new TreeNode("ERROR");
            }
        }

        void menuItemSaveDataSeriesToXmlFile_Click(object sender, EventArgs e)
        {
            // Get the data series from the sender.
            DataSeries dataSeries = (DataSeries)((MenuItem)sender).Tag;

            // Show a save-XML dialog. Only continue if accepted.
            SaveFileDialog dialog = DialogHelper.GetXmlSaveFileDialog();
            _usingDialog = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Save the file.
                dataSeries.SaveXmlFile(dialog.FileName);

                // Show a confirmation message.
                MessageBox.Show("Successfully wrote file: " + dialog.FileName);
            }
            _usingDialog = false;
        }

        void menuItemExportContourShapefile_Click(object sender, EventArgs e)
        {
            // Get the data series from the sender.
            DataSeries dataSeries = (DataSeries)((MenuItem)sender).Tag;

            // Show a save-shapefile dialog. Only continue if accepted.
            SaveFileDialog dialog = DialogHelper.GetShapefileSaveDialog();
            _usingDialog = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Save the file.
                dataSeries.SaveContourShapefile(dialog.FileName);

                // Show a confirmation message.
                MessageBox.Show("Successfully wrote file: " + dialog.FileName);
            }
            _usingDialog = false;
        }

        void menuItemExportAsciiFile_Click(object sender, EventArgs e)
        {
            // Get the data series from the sender.
            DataSeries dataSeries = (DataSeries)((MenuItem)sender).Tag;

            // Show a save-CSV dialog. Only continue if accepted.
            SaveFileDialog dialog = DialogHelper.GetCsvSaveFileDialog();
            _usingDialog = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Get the time series from the data series.
                    TimeSeries timeSeries = (TimeSeries)dataSeries.GetDataSynchronous();

                    WorkspaceUtil.WriteTimeSeriesToCsv(timeSeries, dialog.FileName);

                    // Show a confirmation message.
                    MessageBox.Show("Successfully wrote file: " + dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error exporting data series to CSV: " + ex.Message, "Error Exporting Data Series", MessageBoxButtons.OK, 
                        MessageBoxIcon.Error);
                }
            }
            _usingDialog = false;
        }

        void menuItemEditDataSeries_Click(object sender, EventArgs e)
        {
            // Get the data series from the sender.
            DataSeries dataSeries = (DataSeries)((MenuItem)sender).Tag;

            editDataSeries(dataSeries);
        }

        private void editDataSeries(DataSeries dataSeries)
        {
            // Show the dialog for the data series. Only continue if the dialog is accepted.
            Form dialog = new DataSeriesMenu(dataSeries);
            _usingDialog = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                dataSeries.ClearImage();
                // Refresh the tree.
                refreshTree();
                addSADocumentChangeUndoItem();
            }
            _usingDialog = false;
        }

        void menuItemDeleteLayer_Click(object sender, EventArgs e)
        {
            // Get the data series from the sender.
            DataSeries dataSeries = (DataSeries)((MenuItem)sender).Tag;
            IReportElement element = (IReportElement)dataSeries.ParentElement;
            element.RemoveDataSeries(dataSeries);
            refreshTree();
            addSADocumentChangeUndoItem();
        }

        void menuItemDeleteDataSeries_Click(object sender, EventArgs e)
        {
            // Get the data series from the sender.
            DataSeries dataSeries = (DataSeries)((MenuItem)sender).Tag;

            deleteDataSeries(dataSeries);
        }

        private void deleteDataSeries(DataSeries dataSeries)
        {
            if (dataSeries.IsReferenced())
            {
                string errMsg =
                    "Data series '" + dataSeries.Name + "' is referenced in a calculated data set and may not be removed.";
                MessageBox.Show(errMsg);
            }
            else
            {
                // Delete the data series
                dataSeries.ParentElement.RemoveDataSeries(dataSeries);

                // Refresh the tree.
                refreshTree();

                // Refresh the preview panel.
                refreshImageOfSelectedNode();
                addSADocumentChangeUndoItem();
            }
        }

        void menuItemAddContourLayer_Click(object sender, EventArgs e)
        {
            addDataSeries(sender, DataSeriesTypeEnum.ContourMapSeries);
        }
        void menuItemAddColorFillLayer_Click(object sender, EventArgs e)
        {
            addDataSeries(sender, DataSeriesTypeEnum.ColorFillMapSeries);
        }

        #region Handler methods for chart nodes

        void menuItemAddNewChartDataSeries_Click(object sender, EventArgs e)
        {
            addDataSeries(sender, DataSeriesTypeEnum.ChartSeries);
        }
        void menuItemAddNewTableDataSeries_Click(object sender, EventArgs e)
        {
            addDataSeries(sender, DataSeriesTypeEnum.TableSeries);
        }
        void menuItemDuplicateElement_Click(object sender, EventArgs e)
        {
            if (((MenuItem)sender).Tag is IReportElement)
            {
                IReportElement newReportElement = ((IReportElement)((MenuItem)sender).Tag).DeepClone();
                Document.AddElement(newReportElement);
                refreshTree();
                addSADocumentChangeUndoItem();
            }
        }
        void menuItemInvalidateData_Click(object sender, EventArgs e)
        {
            rereadRecompute(((MenuItem)sender).Tag);
        }
        void rereadRecompute(object nodeTag)
        {
            _needRefreshOfSelectedNode = true;
            int count = 0;
            if (nodeTag is IReportElement)
            {
                IReportElement element = (IReportElement)nodeTag;
                for (int i = 0; i < element.NumDataSeries; i++)
                {
                    element.ClearImage();
                    element.GetDataSeries(i).InvalidateDataset();
                    count++;
                }
            }
            else if (nodeTag is DataSeries)
            {
                DataSeries ds = (DataSeries)nodeTag;
                ds.ClearImage();
                ds.InvalidateDataset();
                count++;
            }
            if (count > 0)
            {
                string msg = "Recomputing " + count.ToString() + " items";
                statusLabelPrimary.Text = msg;
            }
        }
        void rereadRecomputeAllElements()
        {
            for (int i = 0; i < this.Document.NumElements; i++)
            {
                IReportElement element = Document.GetElement(i);
                statusLabelPrimary.Text = "Recomputing " + element.ToString();
                rereadRecompute(element);
            }
        }
        void rereadRecomputeAllChartElements()
        {
            for (int i = 0; i < this.Document.NumElements; i++)
            {
                IReportElement element = Document.GetElement(i);
                if (element is ReportElementChart)
                {
                    statusLabelPrimary.Text = "Recomputing " + element.ToString();
                    rereadRecompute(element);
                }
            }
        }
        void rereadRecomputeAllMapElements()
        {
            for (int i = 0; i < this.Document.NumElements; i++)
            {
                IReportElement element = Document.GetElement(i);
                if (element is ReportElementSTMap)
                {
                    statusLabelPrimary.Text = "Recomputing " + element.ToString();
                    rereadRecompute(element);
                }
            }
        }
        void rereadRecomputeAllTableElements()
        {
            for (int i = 0; i < this.Document.NumElements; i++)
            {
                IReportElement element = Document.GetElement(i);
                if (element is ReportElementTable)
                {
                    statusLabelPrimary.Text = "Recomputing " + element.ToString();
                    rereadRecompute(element);
                }
            }
        }
        void rereadRecomputeChartAndTableElements()
        {
            int count = 0;
            for (int i = 0; i < this.Document.NumElements; i++)
            {
                IReportElement element = Document.GetElement(i);
                if (element.NumDataSeries>0)
                {
                    DataSeries ds = element.GetDataSeries(0);
                    if (ds.DataSeriesType == DataSeriesTypeEnum.ChartSeries 
                        || ds.DataSeriesType == DataSeriesTypeEnum.TableSeries)
                    {
                        rereadRecompute(element);
                        count++;
                    }
                }
            }
            if (count > 0)
            {
                string msg = "Recomputing " + count.ToString() + " items";
                statusLabelPrimary.Text = msg;
            }
        }
        void menuItemDeleteElement_Click(object sender, EventArgs e)
        {
            if (((MenuItem)sender).Tag is IReportElement)
            {
                Document.RemoveElement((IReportElement)((MenuItem)sender).Tag);
                refreshTree();
                addSADocumentChangeUndoItem();
            }
        }

        void menuItemAddXmlLayer_Click(object sender, EventArgs e)
        {
            // Get the report element from the sender.
            IReportElement reportElement = (IReportElement)((MenuItem)sender).Tag;

            // Show an open-file dialog for an XML file. Only continue if accepted.
            OpenFileDialog dialog = DialogHelper.GetXmlOpenFileDialog();
            _usingDialog = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Make a data series from the file.
                XmlElement rootElement = XmlUtil.GetRootOfFile(dialog.FileName);
                DataSeries dataSeries = new DataSeries(DataSeriesTypeEnum.None);
                dataSeries.InitFromXml(reportElement, rootElement, dialog.FileName);

                // Add the data series to the report and refresh the tree.
                reportElement.AddDataSeries(dataSeries);
                refreshTree();
            }
            _usingDialog = false;
        }
        void menuItemSaveXmlMap_Click(object sender, EventArgs e)
        {
            // Get the report element from the sender.
            IReportElement reportElement = (IReportElement)((MenuItem)sender).Tag;

            // Show a save-file dialog for an XML file. Only continue if accepted.
            SaveFileDialog dialog = DialogHelper.GetXmlSaveFileDialog();
            _usingDialog = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Save the file.
                reportElement.SaveXMLFile(dialog.FileName);

                // Show a confirmation message.
                MessageBox.Show("Successfully wrote file: " + dialog.FileName);
            }
            _usingDialog = false;
        }

        #endregion Handler methods for chart nodes

        void addDataSeries(object sender, DataSeriesTypeEnum dataSeriesType)
        {
            // Get the report element from the sender.
            IReportElement element = (IReportElement)((MenuItem)sender).Tag;

            addDataSeries(dataSeriesType, element);
        }

        private void addDataSeries(DataSeriesTypeEnum dataSeriesType, IReportElement element)
        {
            // Make a data series and show a dialog for it. Only continue if the dialog is accepted.
            DataSeries dataSeries = new DataSeries(dataSeriesType);
            Form dialog = new DataSeriesMenu(dataSeries);
            dataSeries.ParentElement = element;
            _usingDialog = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                TreeNode savedElementNode = null;
                TreeNode addedDataSeriesNode = null;

                // Add the data series to the element.
                element.AddDataSeries(dataSeries);

                // Refresh the tree.
                refreshTree();

                // Find and save the node containing the specified element.
                for (int i = 0; i < Document.NumElements; i++)
                {
                    if (Document.GetElement(i).Equals(element))
                    {
                        savedElementNode = treeView.Nodes[i];
                        break;
                    }
                }

                // The added DataSeries node needs to be selected before refreshTree is called (again)
                if (savedElementNode != null)
                {
                    if (savedElementNode.Tag is IReportElement)
                    {
                        addedDataSeriesNode = selectLastDataSeriesTreeNode(((IReportElement)savedElementNode.Tag));
                        if (addedDataSeriesNode != null)
                        {
                            if (addedDataSeriesNode.Tag is DataSeries)
                            {
                                ((DataSeries)addedDataSeriesNode.Tag).ClearImage();
                            }
                        }
                    }
                    _needRefreshOfSelectedNode = true;
                    refreshTree();
                }
                addSADocumentChangeUndoItem();
            }
            _usingDialog = false;
        }

        private TreeNode selectLastDataSeriesTreeNode(IReportElement element)
        {
            // Select the last DataSeries node under the specified element node.
            for (int i = 0; i < Document.NumElements; i++)
            {
                if (Document.GetElement(i).Equals(element))
                {
                    treeView.SelectedNode = treeView.Nodes[i].LastNode;
                    return treeView.SelectedNode;
                }
            }
            return null;
        }

        void menuItemNewSTMap_Click(object sender, EventArgs e)
        {
            makeNewStMapElement();
        }

        private void makeNewStMapElement()
        {
            // Make a new map element and show a dialog for it. Only continue if the dialog is accepted.
            ReportElementSTMap element = new ReportElementSTMap();
            Form dialog = new STMapDesigner(element.STMap, Document.Extents);
            _usingDialog = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Add the map to the report.
                Document.AddElement(element);

                // Refresh the tree.
                refreshTree();

                // Select the new element (the last element in the tree).
                treeView.SelectedNode = treeView.Nodes[treeView.Nodes.Count - 1];
                addSADocumentChangeUndoItem();
            }
            _usingDialog = false;
        }
        
        void menuItemNewChart_Click(object sender, EventArgs e)
        {
            makeNewChartElement();
        }

        private void makeNewChartElement()
        {
            // Make a new MSChart element and show a dialog for it. 
            // Only continue if the dialog is accepted.
            ReportElementChart element = new ReportElementChart();
            Form dialog = new ReportElementChartMenu(element);
            ((ReportElementChartMenu)dialog).ReportElement = element;
            _usingDialog = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Add the chart to the report.
                Document.AddElement(element);

                // Refresh the tree.
                refreshTree();

                // Select the new element (the last element in the tree).
                treeView.SelectedNode = treeView.Nodes[treeView.Nodes.Count - 1];
                addSADocumentChangeUndoItem();
            }
            _usingDialog = false;
        }
        void menuItemNewTable_Click(object sender, EventArgs e)
        {
            makeNewTableElement();
        }

        private void makeNewTableElement()
        {
            // Make a new table element and show a dialog for it. Only continue if the dialog is accepted.
            ReportElementTable element = new ReportElementTable();
            Form dialog = new ReportElementTableMenu(element);
            _usingDialog = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Add the table to the report.
                Document.AddElement(element);

                // Refresh the tree.
                refreshTree();

                // Select the new element (the last element in the tree).
                treeView.SelectedNode = treeView.Nodes[treeView.Nodes.Count - 1];
                addSADocumentChangeUndoItem();
            }
            _usingDialog = false;
        }
        
        void menuItemAddXmlElement_Click(object sender, EventArgs e)
        {
            // Show an open-file dialog for an XML file. Only continue if accepted.
            OpenFileDialog openDialog = DialogHelper.GetXmlOpenFileDialog();
            _usingDialog = true;
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                // Get the XML node from the specified file.
                XmlElement xmlElement = XmlUtil.GetRootOfFile(openDialog.FileName);

                // Make a report element from the XML element.
                IReportElement reportElement = ReportElementFactory.FromXml(xmlElement, openDialog.FileName);

                // Add the report element and refresh the tree.
                Document.AddElement(reportElement);
                refreshTree();

                // Select the new element (the last element in the tree).
                treeView.SelectedNode = treeView.Nodes[treeView.Nodes.Count - 1];
                addSADocumentChangeUndoItem();
            }
            _usingDialog = false;
        }
        void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            clearStatus();
            // Store the selected node.
            _treeViewSelectedNode = treeView.SelectedNode;

            // Invoke the method that displays the dataset. This automatically refreshes according to a timer, but it should also instantly refresh
            // when a node is selected to prevent any illusion of latency.
            refreshImageOfSelectedNode();
        }
        private void refreshPanelImageResizingMode()
        {
            if (pictureBoxPreview.BackgroundImage != null)
            {
                Image image = pictureBoxPreview.BackgroundImage;
                Size panelSize = pictureBoxPreview.ClientSize;
                if (image.Width >= panelSize.Width || image.Height >= panelSize.Height)
                {
                    pictureBoxPreview.BackgroundImageLayout = ImageLayout.Zoom;
                }
                else
                {
                    pictureBoxPreview.BackgroundImageLayout = ImageLayout.Center;
                }
            }
        }
        private void layoutForm()
        {
            // Only continue if the layout values have been calculated.
            if (_calculatedLayoutValues)
            {
                this.SuspendLayout();

                // Refresh the image resizing mode of the panel.
                refreshPanelImageResizingMode();

                this.ResumeLayout();
            }
        }
        private void splitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            layoutForm();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Initialize fields

            // Clear the status label text.
            statusLabelPrimary.Text = "";

            // Add event handlers for resizing and for when the application is going to exit.
            this.ResizeEnd += new EventHandler(MainForm_ResizeEnd);
            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);

            // Store the margins of the tree view.
            _treeViewMarginL = treeView.Left;
            _treeViewMarginR = splitContainer.Panel1.Width - treeView.Right;
            _treeViewMarginT = treeView.Top;
            _treeViewMarginB = splitContainer.Panel1.Height - treeView.Bottom;

            // Store the margins of the picture box.
            _pictureBoxMarginR = splitContainer.Panel2.Width - pictureBoxPreview.Right;
            _pictureBoxMarginB = splitContainer.Panel2.Height - pictureBoxPreview.Bottom;

            // Indicate that the layout values have been calculated.
            _calculatedLayoutValues = true;

            refreshUi();
        }
        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If the current document has unsaved changes, save it.
            if (!_okToClose)
            {
                exit();
                CloseReason cr = e.CloseReason;
                if (cr == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                }
            }

            // Flag that the main form is no longer active.
            _mainFormIsAlive = false;
        }
        void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            layoutForm();
        }

        private void setUpImageMakerBackgroundWorker(BackgroundWorker backgroundWorker)
        {
            if (backgroundWorker != null)
            {
                backgroundWorker.WorkerReportsProgress = true;
                backgroundWorker.WorkerSupportsCancellation = true;
                backgroundWorker.DoWork += new DoWorkEventHandler(imageMakerBackgroundWorker_DoWork);
                backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(imageMakerBackgroundWorker_ProgressChanged);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(imageMakerBackgroundWorker_RunWorkerCompleted);
                GlobalStaticVariables.GlobalBackgroundWorker = backgroundWorker;
                GlobalStaticVariables.MyDelegate a = new GlobalStaticVariables.MyDelegate(Application.DoEvents);
                GlobalStaticVariables.DoEvents = a;
            }
        }
        private void setUpFileReaderBackgroundWorker(BackgroundWorker backgroundWorker)
        {
            if (backgroundWorker != null)
            {
                backgroundWorker.WorkerReportsProgress = true;
                backgroundWorker.WorkerSupportsCancellation = true;
                backgroundWorker.DoWork += new DoWorkEventHandler(FileReaderBackgroundWorker_DoWork);
                backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(FileReaderBackgroundWorker_ProgressChanged);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(FileReaderBackgroundWorker_RunWorkerCompleted);
                GlobalStaticVariables.GlobalBackgroundWorker = backgroundWorker;
                GlobalStaticVariables.MyDelegate a = new GlobalStaticVariables.MyDelegate(Application.DoEvents);
                GlobalStaticVariables.DoEvents = a;
            }
        }

        private void enableControls(bool enable)
        {
            this.menuStrip.Enabled = enable;
            this.treeView.Enabled = enable;
            waitCursor(!enable);
        }

        private void clearStatus()
        {
            statusLabelPrimary.Text = "";
            toolStripProgressBar.Visible = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exit();
        }
        private void fileMenuOpen_Click(object sender, EventArgs e)
        {
            openDocumentFile();
        }

        private void openDocumentFile()
        {
            // If necessary, present a dialog to ask if the user wishes 
            // to save the changes to the current document.
            bool mayContinue = true;
            if (Document.HasUnsavedChanges)
            {
                // The return value of this method tells us whether the user 
                // followed a course that allows us to continue with opening
                // a new document.
                mayContinue = presentSaveChangesDialog();
            }

            // Present an open-SA dialog box. Only continue if the user accepts.
            if (mayContinue)
            {
                OpenFileDialog dialog = DialogHelper.GetOpenSADialog();
                _usingDialog = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // Create a new document and initialize it to the contents of the file.
                    Document = new SADocument();
                    Document.StableFileName = dialog.FileName;

                    // Provide global access to list of extents
                    GlobalStaticVariables.Extents = Document.Extents;

                    prepareToReadFile();
                    if (GlobalStaticVariables.GlobalBackgroundWorker != null)
                    {
                        GlobalStaticVariables.GlobalBackgroundWorker.RunWorkerAsync(Document);
                        while (GlobalStaticVariables.GlobalBackgroundWorker.IsBusy)
                        {
                            Thread.Sleep(1);
                            GlobalStaticVariables.DoEvents();
                        }
                    }
                    else
                    {
                        Document.InitFromSAFile(dialog.FileName, null);
                    }
                    saveOldDocument();
                    clearUndoChain();
                    statusLabelPrimary.Text = "";
                    _readingFile = false;
                    //UseWaitCursor = false;
                    enableControls(true);

                    // Refresh the user interface.
                    refreshUi();

                    // Perform an update indicating that the document was loaded.
                    Logging.Update("Opened document: " + dialog.FileName, Logging.ERROR_90_INFORMATION_ONLY);
                    toolStripProgressBar.Visible = false;
                }
                _usingDialog = false;
            }
        }
        private bool presentSaveChangesDialog()
        {
            // Present a dialog asking if the user wants to save changes.
            DialogResult dialogResult = MessageBox.Show("Do you want to save the changes to " + Document.ShortFileName + "?", APPLICATION_NAME,
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

            // The easy case is when the user doesn't want to save changes. Return true to indicate that the process that initiated this dialog may 
            // continue.
            if (dialogResult == DialogResult.No)
            {
                return true;
            }

            return true;
        }
        private void fileMenuSave_Click(object sender, EventArgs e)
        {
            enableControls(false);
            // First, check if the document has a permanent path. If so, try to save to that path.
            bool savedDocument = false;
            if (Document.StableFileName != "")
            {
                try
                {
                    // Try to save the document. The return value tells us 
                    // whether saving was successful.
                    savedDocument = saveCurrentDocument(Document.StableFileName);
                }
                catch
                {
                    savedDocument = false;
                }
            }

            // If we haven't yet saved the document, invoke the no-argument save-document 
            // method, which will interact with the user to obtain a file name.
            if (!savedDocument)
            {
                savedDocument = saveCurrentDocument();
            }

            // Update the title bar.
            this.updateTitleBar();

            // Before leaving, ensure that the process indicator is set to 0 
            // and the wait cursor is deactivated.
            toolStripProgressBar.Value = 0;
            enableControls(true);
        }
        private void fileMenuSaveAs_Click(object sender, EventArgs e)
        {
            // Save the current document.
            if (saveCurrentDocument())
            {
                statusLabelPrimary.Text = "Save of current project succeeded.";
            }
            else
            {
                statusLabelPrimary.Text = "Save of current project failed.";
            }


            // Update the title bar.
            this.updateTitleBar();

            // Before leaving, ensure that the process indicator is set to 0 and the wait cursor is deactivated.
            toolStripProgressBar.Value = 0;
            //this.UseWaitCursor = false;
        }
        private void updateTitleBar()
        {
            // Set the title bar text.
            this.Text = Document.ShortFileNameWithExtension + " - " + APPLICATION_NAME;
            if (Document.StableFileName == "")
            {
                this.Text = APPLICATION_NAME;
            }
            else
            {
                this.Text = APPLICATION_NAME + ": [" + Document.StableFileName + "]";
            }
        }

        private TreeNode getTreeNode(int elementIndex, int dataSeriesIndex)
        {
            try
            {
                TreeNode elementNode = treeView.Nodes[elementIndex];
                return elementNode.Nodes[dataSeriesIndex];
            }
            catch
            {
                return null;
            }
        }

        private void refreshTreeIcons()
        {
            try
            {
                if (! _refreshingTreeIcons)
                {
                    _refreshingTreeIcons = true;
                    // Refresh all of the elements in the report.
                    for (int j = 0; j < Document.NumElements; j++)
                    {
                        IReportElement element = Document.GetElement(j);
                        for (int i = 0; i < element.NumDataSeries; i++)
                        {
                            // Get the dataset status from the data series.
                            DataSeries dataSeries = element.GetDataSeries(i);
                            int datasetStatus;
                            dataSeries.GetData(out datasetStatus);

                            // Get the tree node that corresponds to this data series.
                            TreeNode treeNode = getTreeNode(j, i);

                            // Set the icon on the tree node.
                            if (treeNode != null)
                            {
                                if (datasetStatus == DataStatus.DATA_AVAILABLE_CACHE_PRESENT)
                                {
                                    treeNode.ImageIndex = IconHelper.NORMAL_INDEX;
                                }
                                else if (datasetStatus == DataStatus.DATASET_NEEDS_REFRESH)
                                {
                                    treeNode.ImageIndex = IconHelper.REFRESH_INDEX_0 + _treeRefreshIconIndex;
                                }
                                else if (datasetStatus == DataStatus.DATA_UNAVAILABLE_CACHE_PRESENT)
                                {
                                    treeNode.ImageIndex = IconHelper.WARNING_INDEX;
                                }
                                else
                                {
                                    treeNode.ImageIndex = IconHelper.ERROR_INDEX;
                                }
                                treeNode.SelectedImageIndex = treeNode.ImageIndex;
                            }
                        }
                    }
                    _refreshingTreeIcons = false;
                }
            }
            catch { }


            // Increment the refresh icon index.
            _treeRefreshIconIndex = (_treeRefreshIconIndex + 1) % 4;
        }

        #region ILogger Members

        public void Update(string message, int priority)
        {
            statusLabelPrimary.Text = message;
        }

        #endregion

        private void fileMenuNew_Click(object sender, EventArgs e)
        {
            // This flag tells us whether we may continue creating a new document.
            bool mayContinue = true;

            // If the current document has unsaved changes, present a dialog asking if the changes should be saved.
            if (_undoChain.Dirty)
            {
                // Show a dialog to ask if the changes should be saved.
                DialogResult dialogResult = MessageBox.Show("Do you want to save the changes to " + Document.ShortFileName + "?", APPLICATION_NAME,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                // If the user responds affirmatively, initiate the file-saving process. The return value will tell us whether the file has been 
                // saved successfully, allowing us to continue.
                if (dialogResult == DialogResult.Yes)
                {
                    mayContinue = saveCurrentDocument();
                }

                // If the user responds by cancelling, we may not continue.
                else if (dialogResult == DialogResult.Cancel)
                {
                    mayContinue = false;

                    Logging.Update("Creation of new document cancelled.", Logging.ERROR_90_INFORMATION_ONLY);
                }

                // Otherwise, the result is no -- in this case, we may continue.
            }

            // If we are permitted to continue, create a new document.
            if (mayContinue)
            {
                // Make the document.
                Document = new SADocument();
                getUserSettings();
                // Provide global access to list of extents
                GlobalStaticVariables.Extents = Document.Extents;

                saveOldDocument();

                // Refresh the tree.
                refreshTree();
            }

        }
        private bool saveCurrentDocument()
        {
            // Present a dialog for the user to save the file.
            SaveFileDialog dialog = DialogHelper.getSaveSADialog();

            // If the user accepts, try to save the document to the specified path. 
            // Return the value indicating whether the document was saved.
            _usingDialog = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _usingDialog = false;
                return saveCurrentDocument(dialog.FileName);
            }

            // Otherwise, return false (indicating that the document was not saved.
            else
            {
                _usingDialog = false;
                return false;
            }
        }
        private bool saveCurrentDocument(string file)
        {
            bool savedDocument = false;

            // Try to save the document to the specified path. 
            // If this completes, flag saving as successful.
            try
            {
                // Update the progress indicator to 20% and switch to the wait cursor.
                toolStripProgressBar.Value = 20;
                waitCursor(true);

                // Save the document and log the action.
                if (!Document.Save(file))
                {
                    throw new Exception("Error encountered during file save");
                }
                Logging.Update("Saved document: " + file, Logging.ERROR_90_INFORMATION_ONLY);

                // Update the stable path of the document so a dialog is not necessary 
                // next time the user tries to save the document.
                Document.StableFileName = file;

                // Update the progress indicator to 100%.
                toolStripProgressBar.Value = 100;

                // Flag saving as successful.
                savedDocument = true;

                // Save user settings
                saveUserSettings();

                // Clear Undo chain
                clearUndoChain();
                refreshUi();
            }
            catch (Exception e)
            {
                Logging.Update("Error saving document: " + file + " -- " + e.Message, Logging.ERROR_50_SERIOUS_ERROR_NO_IMMEDIATE_DATA_LOSS);
                savedDocument = false;
            }
            finally
            {
                waitCursor(false);
            }

            return savedDocument;
        }

        private void refreshImageOfSelectedNode()
        {
            // NOTE: Because this method modifies the form controls, 
            // it must run on the user-interface thread.

            // Get the node that is currently selected in the tree view.
            TreeNode selectedNode = treeView.SelectedNode;

            if (!_refreshingImage)
            {
                _refreshingImage = true;
                // This is a flag that determines whether we want to refresh the image. 
                // If we do want to refresh the image, we will determine later what
                // that image should be.
                bool shouldRefreshImage = false;

                // If the need-refresh flag is set, we want to refresh the image.
                if (_needRefreshOfSelectedNode)
                {
                    shouldRefreshImage = true;
                    _needRefreshOfSelectedNode = false;
                }

                // If either node is null, we want to refresh.
                if (_lastSelectedNode == null || selectedNode == null)
                {
                    shouldRefreshImage = true;
                }

                // If neither node is null, want want to refresh if the node has changed 
                // or if the status of the selected node has changed.
                else
                {
                    if (_lastSelectedNode != selectedNode)
                    {
                        shouldRefreshImage = true;
                    }
                }

                // If we want to refresh the image, we'll display a good image if we can. 
                // Otherwise, we'll display the filler image.
                if (shouldRefreshImage)
                {
                    bool displayedImage = false;

                    if (selectedNode != null)
                    {
                        enableControls(false);
                        if (selectedNode.Tag != null)
                        {
                            if (selectedNode.Tag is IImageProvider)
                            {
                                //if (!_refreshingImage)
                                {
                                    try
                                    {
                                        //_refreshingImage = true;
                                        IImageProvider imageProvider = (IImageProvider)selectedNode.Tag;
                                        if (imageProvider is ReportElementSTMap)
                                        {
                                            ((ReportElementSTMap)imageProvider).AssignDesiredExtent(Document.Extents);
                                            if (File.Exists(GlobalStaticVariables.BackgroundImageFile)
                                                && GlobalStaticVariables.BackgroundImageLayer == null
                                                && ((ReportElementSTMap)imageProvider).ShowBasemap)
                                            {
                                                prepareToGenerateBackgroundImage();
                                            }
                                        }
                                        Image image = imageProvider.GetImage();
                                        if (imageProvider is ReportElementSTMap)
                                        {
                                            Document.AddExtentOfBackgroundImage();
                                            Application.DoEvents();
                                            _generatingBackgroundImage = false;
                                        }

                                        if (image != null)
                                        {
                                            pictureBoxPreview.BackgroundImage = image;
                                            displayedImage = true;
                                        }
                                    }
                                    finally
                                    {
                                        _refreshingImage = false;
                                    }
                                }
                            }
                        }
                        enableControls(true);
                    }

                    if (!displayedImage)
                    {
                        pictureBoxPreview.BackgroundImage = DEFAULT_IMAGE;
                    }

                    // Set the scaling according to the relative size of the background image and the client area.
                    Size imageSize = pictureBoxPreview.BackgroundImage.Size;
                    Size containerSize = pictureBoxPreview.ClientSize;
                    if (imageSize.Width > containerSize.Width || imageSize.Height > containerSize.Height)
                    {
                        pictureBoxPreview.BackgroundImageLayout = ImageLayout.Zoom;
                    }
                    else
                    {
                        pictureBoxPreview.BackgroundImageLayout = ImageLayout.Center;
                    }
                }

                // Store the previously selected node.
                _lastSelectedNode = selectedNode;
                //Application.DoEvents();
                _refreshingImage = false;
            }
        }

        private void prepareToGenerateBackgroundImage()
        {
            if (genericBackgroundWorker != null)
            {
                genericBackgroundWorker.Dispose();
            }
            genericBackgroundWorker = new BackgroundWorker();
            setUpImageMakerBackgroundWorker(genericBackgroundWorker);
            //statusLabelPrimary.Text = "Generating image...";
            _generatingBackgroundImage = true;
            //UseWaitCursor = true;
            enableControls(false);
            Application.DoEvents();
        }
        private void prepareToReadFile()
        {
            CreateCellCenteredArealGrid.ProgressBar = null;
            CreateCellCenteredArealGrid.doEvents = null;
            if (genericBackgroundWorker != null)
            {
                genericBackgroundWorker.Dispose();
            }
            genericBackgroundWorker = new BackgroundWorker();
            setUpFileReaderBackgroundWorker(genericBackgroundWorker);
            statusLabelPrimary.Text = "Reading file...";
            _readingFile = true;
            //UseWaitCursor = true;
            enableControls(false);
            //Application.DoEvents();
        }

        private void fileMenuExportPDF_Click(object sender, EventArgs e)
        {
            // Pass to the export-PDF handler.
            menuItemExportPdfReport_Click(sender, e);
        }

        private void treeView_KeyDown(object sender, KeyEventArgs e)
        {
            clearStatus();
            if (e.Control)
            {
                // KeyDown is for a <CTRL>-key combination
            }
            else
            {
                if (e.KeyCode == Keys.Delete)
                {
                    TreeNode node = treeView.SelectedNode;
                    if (node.Tag is DataSeries)
                    {
                        DataSeries ds = (DataSeries)node.Tag;
                        deleteDataSeries(ds);
                    }
                    else if (node.Tag is IReportElement)
                    {
                        IReportElement element = (IReportElement)node.Tag;
                        Document.RemoveElement(element);
                        node.Remove();
                        addSADocumentChangeUndoItem();
                    }
                    Refresh();
                }
            }
        }

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            editMenuElementProperties.Enabled = false;
            editMenuDataSeriesProperties.Enabled = false;
            editMenuMapLayerProperties.Enabled = false;
            menuEditAddNewContourLayer.Enabled = false;
            menuEditAddNewColorFillLayer.Enabled = false;
            menuEditAddNewDataSeries.Enabled = false;
            if (treeView.SelectedNode != null)
            {
                TreeNode node = treeView.SelectedNode;
                if (node.Tag is IReportElement)
                {
                    editMenuElementProperties.Enabled = true;
                    IReportElement element = (IReportElement)node.Tag;
                    if (element is ReportElementChart)
                    {
                        menuEditAddNewDataSeries.Enabled = true;
                    }
                    else if (element is ReportElementTable)
                    {
                        menuEditAddNewDataSeries.Enabled = true;
                    }
                    else if (element is ReportElementSTMap)
                    {
                        menuEditAddNewColorFillLayer.Enabled = true;
                        menuEditAddNewContourLayer.Enabled = true;
                    }
                }
                else if (node.Tag is DataSeries)
                {
                    DataSeries ds = (DataSeries)node.Tag;
                    if (ds.DataConsumerType == DataConsumerTypeEnum.Map || ds.DataConsumerType == DataConsumerTypeEnum.STMap)
                    {
                        editMenuMapLayerProperties.Enabled = true;
                    }
                    else
                    {
                        editMenuDataSeriesProperties.Enabled = true;
                    }
                }
            }
        }

        private void editMenuElementProperties_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView.SelectedNode;
            if (node.Tag is IReportElement)
            {
                IReportElement element = (IReportElement)node.Tag;
                editElementProperties(element);
            }
        }

        private void editMenuMapLayerProperties_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView.SelectedNode;
            if (node.Tag is DataSeries)
            {
                DataSeries ds = (DataSeries)node.Tag;
                // Show the dialog for the data series. Only continue if the dialog is accepted.
                Form dialog = new DataSeriesMenu(ds);
                _usingDialog = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // Refresh the tree.
                    refreshTree();
                    addSADocumentChangeUndoItem();
                }
                _usingDialog = false;
            }
        }

        private void editMenuDataSeriesProperties_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView.SelectedNode;
            if (node.Tag is DataSeries)
            {
                DataSeries ds = (DataSeries)node.Tag;
                // Show the dialog for the data series. Only continue if the dialog is accepted.
                Form dialog = new DataSeriesMenu(ds);
                _usingDialog = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // Refresh the tree.
                    refreshTree();
                    addSADocumentChangeUndoItem();
                }
                _usingDialog = false;
            }
        }

        private void menuEditAddNewChart_Click(object sender, EventArgs e)
        {
            makeNewChartElement();
        }

        private void menuEditAddNewTable_Click(object sender, EventArgs e)
        {
            makeNewTableElement();
        }

        private void menuEditAddNewContourLayer_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                IReportElement element = (IReportElement)treeView.SelectedNode.Tag;
                addDataSeries(DataSeriesTypeEnum.ContourMapSeries, element);
            }
        }

        private void menuEditAddNewColorFillLayer_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                IReportElement element = (IReportElement)treeView.SelectedNode.Tag;
                addDataSeries(DataSeriesTypeEnum.ColorFillMapSeries, element);
            }
        }

        private void menuEditAddNewDataSeries_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                IReportElement element = (IReportElement)treeView.SelectedNode.Tag;
                if (element is ReportElementChart)
                {
                    addDataSeries(DataSeriesTypeEnum.ChartSeries, element);
                }
                else if (element is ReportElementTable)
                {
                    addDataSeries(DataSeriesTypeEnum.TableSeries, element);
                }
            }
        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            clearStatus();
        }

        private void treeView_Click(object sender, EventArgs e)
        {
            clearStatus();
        }

        private void statusLabelPrimary_Click(object sender, EventArgs e)
        {
            clearStatus();
        }

        private void treeView_MouseClick(object sender, MouseEventArgs e)
        {
            clearStatus();
            if (sender is TreeView)
            {
                TreeNode node = ((TreeView)sender).GetNodeAt(e.X, e.Y);
                treeView.SelectedNode = node;
            }
        }

        private void imageMakerBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            Dataset ds = (Dataset)e.Argument;
            e.Result = GdalHelper.GetBitmapBuffered(ds, 0, worker);
        }
        private void imageMakerBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage < toolStripProgressBar.Minimum)
            {
                toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                statusLabelPrimary.Text = "Generating image (progress < min)...";
            }
            else if (e.ProgressPercentage > toolStripProgressBar.Maximum)
            {
                toolStripProgressBar.Value = toolStripProgressBar.Maximum;
                statusLabelPrimary.Text = "Generating image (progress > max)...";
            }
            else
            {
                toolStripProgressBar.Value = e.ProgressPercentage;
                if (e.ProgressPercentage < toolStripProgressBar.Maximum)
                {
                    enableControls(false);
                    //UseWaitCursor = true;
                    toolStripProgressBar.Visible = true;
                    statusLabelPrimary.Text = "Generating image...";
                }
            }
        }
        private void imageMakerBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripProgressBar.Visible = false;
            enableControls(true);
            //UseWaitCursor = false;
            this.statusLabelPrimary.Text = "";
            if (e.Result is Bitmap)
            {
                GlobalStaticVariables.GlobalBitmap = (Bitmap)e.Result;
            }
        }

        private void FileReaderBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            if (e.Argument is SADocument)
            {
                SADocument document = (SADocument)e.Argument;
                document.InitFromSAFile(document.StableFileName, worker);
            }
        }
        private void FileReaderBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (e.ProgressPercentage < toolStripProgressBar.Minimum)
                {
                    toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                    statusLabelPrimary.Text = "Reading file (progress < min)...";
                }
                else if (e.ProgressPercentage > toolStripProgressBar.Maximum)
                {
                    toolStripProgressBar.Value = toolStripProgressBar.Maximum;
                    statusLabelPrimary.Text = "Reading file (progress > max)...";
                }
                else
                {
                    toolStripProgressBar.Value = e.ProgressPercentage;
                    if (e.ProgressPercentage < toolStripProgressBar.Maximum)
                    {
                        enableControls(false);
                        //UseWaitCursor = true;
                        toolStripProgressBar.Visible = true;
                        statusLabelPrimary.Text = "Reading file...";
                    }
                }
            }
            catch
            {
            }
        }
        private void FileReaderBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripProgressBar.Visible = false;
            enableControls(true);
            //UseWaitCursor = false;
            this.statusLabelPrimary.Text = "";
        }

        private void treeView_MouseUp(object sender, MouseEventArgs e)
        {
            clearStatus();
        }

        private void pictureBoxPreview_Click(object sender, EventArgs e)
        {
            clearStatus();
        }

        private void addNewSTMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            makeNewStMapElement();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            AboutBox aboutBox = new AboutBox(assembly);
            _usingDialog = true;
            aboutBox.ShowDialog();
            _usingDialog = false;
        }

        //After an undo or redo is performed, we want to move the focus to the 
        //control whose value just changed, so the user can see what the change was. 
        //The value in the control will automatically be updated by the data binding.
        private void UndoChain_Acted(object sender, UndoActionEventArgs e)
        {
        }
        private void saveOldDocument()
        {
            _oldDocument = new SADocument(Document);
        }
        private void addSADocumentChangeUndoItem()
        {
            addSADocumentChangeUndoItem(_oldDocument, Document);
        }
        private void addSADocumentChangeUndoItem(SADocument saDocumentOld, SADocument saDocumentNew)
        {
            _undoChain.AddUndoItem(new SADocumentChangeUndoItem(_documentHolder, saDocumentOld, saDocumentNew));
            refreshUndoMenu();
            saveOldDocument();
        }

        private void addGlobalTemporalReferenceChangeUndoItem(TemporalReference oldTemporalReference, TemporalReference newTemporalReference)
        {
            TemporalReference localOldTempRef = new TemporalReference(oldTemporalReference);
            TemporalReference localNewTempRef = new TemporalReference(newTemporalReference);
            _undoChain.AddUndoItem(new GlobalTemporalReferenceChangeUndoItem(localOldTempRef, localNewTempRef));
            refreshUndoMenu();
        }
        private void addGlobalBackgroundImageFileChangeUndoItem(string oldBackgroundImageFile,
            string newBackgroundImageFile)
        {
            _undoChain.AddUndoItem(new GlobalBackgroundImageFileChangeUndoItem(oldBackgroundImageFile,newBackgroundImageFile));
            refreshUndoMenu();
        }
        private void addGlobalGridShapefilePathChangeUndoItem(string oldGridShapefilePath, string newGridShapefilePath)
        {
            _undoChain.AddUndoItem(new GlobalGridShapefilePathChangeUndoItem(oldGridShapefilePath, newGridShapefilePath));
            refreshUndoMenu();
        }

        private void refreshUndoMenu()
        {
            if (_undoChain.Count > _maxUndoCount)
            {
                _maxUndoCount = _undoChain.Count;
            }
            string itemName0 = editToolStripMenuItem.Name;
            string itemName1 = undoToolStripMenuItem.Name;
            if (_undoChain.Count == 0)
            {
                ((ToolStripMenuItem)menuStrip.Items[itemName0]).DropDownItems[itemName1].Enabled = false;
            }
            else
            {
                ((ToolStripMenuItem)menuStrip.Items[itemName0]).DropDownItems[itemName1].Enabled = true;
            }
            refreshSaveMenuItem();
        }
        private void refreshRedoMenu()
        {
            if (_undoChain.Count > _maxUndoCount)
            {
                _maxUndoCount = _undoChain.Count;
            }
            string itemName0 = editToolStripMenuItem.Name;
            string itemName1 = redoToolStripMenuItem.Name;
            if (_undoChain.Count == _maxUndoCount)
            {
                ((ToolStripMenuItem)menuStrip.Items[itemName0]).DropDownItems[itemName1].Enabled = false;
            }
            else
            {
                ((ToolStripMenuItem)menuStrip.Items[itemName0]).DropDownItems[itemName1].Enabled = true;
            }
            refreshSaveMenuItem();
        }
        private void refreshUndoRedoMenus()
        {
            refreshUndoMenu();
            refreshRedoMenu();
        }
        private void refreshSaveMenuItem()
        {
            if (_undoChain.Dirty)
            {
                fileMenuSave.Enabled = true;
                _okToClose = false;
            }
            else
            {
                fileMenuSave.Enabled = false;
                _okToClose = true;
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UndoItemBase undoItemCurrent = (UndoItemBase)_undoChain.Current;
            _undoChain.Undo();
            redoToolStripMenuItem.Enabled = true;
            if (_undoChain.Count == 0)
            {
                undoToolStripMenuItem.Enabled = false;
            }
            else
            {
                undoToolStripMenuItem.Enabled = true;
            }
            refreshUi();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _undoChain.Redo();
            undoToolStripMenuItem.Enabled = true;
            if (_undoChain.Count == _maxUndoCount)
            {
                redoToolStripMenuItem.Enabled = false;
            }
            else
            {
                redoToolStripMenuItem.Enabled = true;
            }
            refreshUi();
        }
        private void refreshUi()
        {
            refreshUi(true);
        }
        private void refreshUi(bool clearStatus)
        {
            refreshTree();
            updateTitleBar();
            refreshUndoRedoMenus();
            refreshSaveMenuItem();
            Refresh();
        }
        private void clearUndoChain()
        {
            _undoChain.Clear();
            _maxUndoCount = 0;
        }
        private void exit()
        {
            if (_undoChain.Dirty)
            {
                string msg = "Project has unsaved changes.  Save now?";
                DialogResult dr = MessageBox.Show(msg, "Scenario Analyzer",
                                                  MessageBoxButtons.YesNoCancel,
                                                  MessageBoxIcon.Question);
                switch (dr)
                {
                    case DialogResult.Cancel:
                        break;
                    case DialogResult.No:
                        _okToClose = true;
                        Close();
                        break;
                    case DialogResult.Yes:
                        saveCurrentDocument();
                        exit();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                _okToClose = true;
                Close();
            }
        }

        private void treeView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            clearStatus();
            if (sender is TreeView)
            {
                TreeNode node = ((TreeView)sender).GetNodeAt(e.X, e.Y);
                treeView.SelectedNode = node;
                if (node.Tag is DataSeries)
                {
                    editDataSeries((DataSeries)node.Tag);
                }
                else if (node.Tag is IReportElement)
                {
                    editElementProperties((IReportElement)node.Tag);
                }
            }
        }
        private void waitCursor(bool showWaitCursor)
        {
            if (showWaitCursor)
            {
                this.Cursor = Cursors.WaitCursor;
            }
            else
            {
                this.Cursor = _defaultCursor;
            }
            Application.DoEvents();
        }
        private void menuItemProjectSettings_Click(object sender, EventArgs e)
        {
            // Pass to the edit-report handler.
            editProjectSettings();
        }
        private void editProjectSettings()
        {
            statusLabelPrimary.Text = "";
            StaticObjects.Status = "";
            CreateCellCenteredArealGrid.ProgressBar = toolStripProgressBar;
            CreateCellCenteredArealGrid.MyDelegate a = new CreateCellCenteredArealGrid.MyDelegate(Application.DoEvents);
            CreateCellCenteredArealGrid.doEvents = a;

            // Save current simulation start time
            DateTime oldSimStartTime = new DateTime(GlobalStaticVariables.GlobalTemporalReference.SimulationStartTime.Ticks);

            // Show a dialog for editing the document properties. If accepted, refresh the tree.
            _usingDialog = true;
            Document.ChartsNeedRecompute = false;
            Document.MapsNeedRecompute = false;
            Document.TablesNeedRecompute = false;
            DocumentMenu documentMenu = new DocumentMenu(Document);
            if (documentMenu.ShowDialog() == DialogResult.OK)
            {
                statusLabelPrimary.Text = "Applying project settings...";
                // Refresh the tree.
                refreshTree();
                addSADocumentChangeUndoItem();
                StaticObjects.Status = "";
                if (documentMenu.BackgroundImageFile != GlobalStaticVariables.BackgroundImageFile)
                {
                    // Generate an Undo item for the change
                    addGlobalBackgroundImageFileChangeUndoItem(GlobalStaticVariables.BackgroundImageFile, 
                        documentMenu.BackgroundImageFile);
                    GlobalStaticVariables.BackgroundImageFile = documentMenu.BackgroundImageFile;
                }
                if (documentMenu.GridShapefilePath != StaticObjects.GridShapefilePath)
                {
                    // Generate an Undo item for the change
                    addGlobalGridShapefilePathChangeUndoItem(StaticObjects.GridShapefilePath, 
                        documentMenu.GridShapefilePath);
                    ScenarioTools.Spatial.StaticObjects.GridShapefilePath = documentMenu.GridShapefilePath;
                    Document.AddExtentOfModelGrid();
                }
                if (!documentMenu.TemporalRef.Equals(GlobalStaticVariables.GlobalTemporalReference))
                {
                    // Create a GlobalTemporalReferenceChangeUndoItem
                    addGlobalTemporalReferenceChangeUndoItem(GlobalStaticVariables.GlobalTemporalReference,
                        documentMenu.TemporalRef);
                    GlobalStaticVariables.GlobalTemporalReference.ModelTimeUnit = documentMenu.TemporalRef.ModelTimeUnit;
                    GlobalStaticVariables.GlobalTemporalReference.SimulationStartTime =
                        documentMenu.TemporalRef.SimulationStartTime;
                }
                GlobalStaticVariables.GlobalLengthUnit = Document.ModflowLengthUnit;

                // Recompute elements as needed
                if (Document.ChartsNeedRecompute) rereadRecomputeAllChartElements();
                if (Document.MapsNeedRecompute) rereadRecomputeAllMapElements();
                if (Document.TablesNeedRecompute) rereadRecomputeAllTableElements();
            }
            _usingDialog = false;
        }

        private void rereadRecomputeAllElementsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rereadRecomputeAllElements();
        }

        private void menuItemProject_DropDownOpening(object sender, EventArgs e)
        {
            int count = this.treeView.GetNodeCount(false);
            rereadRecomputeAllDisplayElementsToolStripMenuItem.Enabled = count > 0;
        }
        private void getUserSettings()
        {
            Properties.Settings.Default.Reload();
            // If this is the first time getting settings since new installation of app, 
            // copy settings from previous installation
            if (Properties.Settings.Default.UpgradeNeeded)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeNeeded = false;
            }
            Document.Author = Properties.Settings.Default.Author;
        }
        private void saveUserSettings()
        {
            Properties.Settings.Default.Author = Document.Author;
            Properties.Settings.Default.Save();
        }

        private void SAMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveUserSettings();
        }

        private void toggleAutoRefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _autoRefresh = !_autoRefresh;
        }

        private void rereadRecomputeSelectedElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView.SelectedNode;
            rereadRecompute(selectedNode.Tag);
        }

        private void scenarioAnalyzerHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string exePath = Application.ExecutablePath;
            string exeDir = Path.GetDirectoryName(exePath);
            string helpFilename = "ScenarioToolsHelp.chm";
            string helpPath = exeDir + Path.DirectorySeparatorChar + helpFilename;
            string helpUrl = "file://" + helpPath;
            HelpNavigator helpNavKeyIndx = HelpNavigator.KeywordIndex;
            Help.ShowHelp(this, helpUrl, helpNavKeyIndx, "Scenario Analyzer");
        }
    }
}
