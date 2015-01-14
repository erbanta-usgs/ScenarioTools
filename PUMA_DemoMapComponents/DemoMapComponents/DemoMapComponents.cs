using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using USGS.Puma.Core;
using USGS.Puma.FiniteDifference;
using USGS.Puma.IO;
using USGS.Puma.Modflow;
using USGS.Puma.Modpath;
using USGS.Puma.NTS;
using USGS.Puma.NTS.Features;
using USGS.Puma.Utilities;
using USGS.Puma.UI.MapViewer;
using USGS.ModflowTrainingTools;
using GeoAPI.Geometries;


namespace DemoMapComponents
{
    /// <summary>
    /// The DemoMapComponents application demonstrates how to use a number of the classes and features available
    /// in the PUMA Framework. (PUMA stands for Programming Utility for Modeling Applications).
    /// </summary>
    /// <remarks></remarks>
    public partial class DemoMapComponents : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Forms.Form"/> class.
        /// </summary>
        /// <remarks></remarks>
        public DemoMapComponents()
        {
            InitializeComponent();

            #region Create and initialize map component controls
            // The mapControl, mapLegend, and indexMap controls are defined programmatically in the following
            // blocks of code rather than through the visual form designer. This is a little more work up front,
            // but it is safer to do it this way if the classes for those three controls are undergoing development
            // and changing frequently.

            // mapControl: create, initialize, and add control to panelMapControl container
            mapControl = new MapControl();
            mapControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            mapControl.TabIndex = 0;
            mapControl.Dock = DockStyle.Fill;
            panelMapControl.Controls.Add(mapControl);
            // Connect mapControl events to event handlers
            mapControl.MouseDoubleClick += new MouseEventHandler(mapControl_MouseDoubleClick);
            mapControl.MouseClick += new MouseEventHandler(mapControl_MouseClick);
            mapControl.MouseMove += new MouseEventHandler(mapControl_MouseMove);

            // mapLegend: create, initialize, and add control to panelMapLegend container
            mapLegend = new MapLegend();
            mapLegend.BorderStyle = BorderStyle.FixedSingle;
            mapLegend.TabIndex = 0;
            mapLegend.Dock = DockStyle.Fill;
            panelMapLegend.Controls.Add(mapLegend);
            // Connect mapLegend events to event handlers
            mapLegend.LayerVisibilityChanged += new EventHandler<EventArgs>(mapLegend_LayerVisibilityChanged);

            // indexMap: create, initialize, and add control to panelIndexMap container
            indexMap = new IndexMapControl();
            indexMap.SourceMap = mapControl; // this associates the mapControl with the indexMap control
            indexMap.SuppressMapImageUpdate = false;
            indexMap.BorderStyle = BorderStyle.FixedSingle;
            indexMap.TabIndex = 0;
            indexMap.Dock = DockStyle.Fill;
            panelIndexMap.Controls.Add(indexMap);

            #endregion

            // Create a ContourEngineData object to store contour settings. This provides the data used by the edit contour dialog.
            _ContourEngineData = new ContourEngineData();

        }

        #region Private Fields
        // NOTE: I use a leading underscore for private variables at the class level to allow them to be
        // distinguished easily from local private varialbles within methods. The private variables that hold
        // the map control, index map, and map legend are exceptions. They do not have a leading underscore.

        /// <summary>
        /// A control that displays the contents of the map as a series of map layers.
        /// </summary>
        private USGS.Puma.UI.MapViewer.MapControl mapControl = null;
        /// <summary>
        /// A control that displays an index map based on the mapControl as its source map
        /// </summary>
        private USGS.Puma.UI.MapViewer.IndexMapControl indexMap = null;
        /// <summary>
        /// A control that displays a legend for the mapControl.
        /// Map layers are added manually to the legend control.
        /// </summary>
        private USGS.Puma.UI.MapViewer.MapLegend mapLegend = null;
        /// <summary>
        /// A ContourEngineData object that stores the contour data.
        /// This variable provides the data used by the edit contour dialog.
        /// It also provides access to a BinaryLayerReader object conneected to
        /// the current binary MODFLOW head file.
        /// </summary>
        private USGS.ModflowTrainingTools.ContourEngineData _ContourEngineData = null;
        /// <summary>
        /// A CellCenteredArealGrid object that defines a model grid corresponding to
        /// the dimensions of the current head file.
        /// </summary>
        private USGS.Puma.FiniteDifference.CellCenteredArealGrid _ModelGrid = null;
        /// <summary>
        /// A FeatureLayer containing the interior model grid lines.
        /// </summary>
        private USGS.Puma.UI.MapViewer.FeatureLayer _GridlinesMapLayer = null;
        /// <summary>
        /// A FeatureLayer containing the grid outline.
        /// </summary>
        private USGS.Puma.UI.MapViewer.FeatureLayer _GridOutlineMapLayer = null;
        /// <summary>
        /// A FeatureLayer containing the contours for the selected head data layer.
        /// </summary>
        private USGS.Puma.UI.MapViewer.FeatureLayer _CurrentContourMapLayer = null;
        /// <summary>
        /// A FeatureLayer containing features from a user-selected shapefile
        /// </summary>
        private USGS.Puma.UI.MapViewer.FeatureLayer _ImportedShapefileMapLayer = null;


        #endregion

        #region General Event Handlers
        /// <summary>
        /// Handles the Click event of the btnZoomFullExtent control.
        /// Resets the map to full extent.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void btnZoomFullExtent_Click(object sender, EventArgs e)
        {
            // Reset the mapControl to show the full extent of the current map layers.
            // Skip this if the map is empty.
            if (mapControl.LayerCount > 0)
            {
                mapControl.SizeToFullExtent();
            }
        }
        /// <summary>
        /// Handles the Click event of the btnEditContourData control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void btnEditContourData_Click(object sender, EventArgs e)
        {
            EditContourLayer();
        }
        /// <summary>
        /// Handles the Click event of the btnBrowseHeadFiles control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void btnBrowseHeadFiles_Click(object sender, EventArgs e)
        {
            OpenHeadFile();
        }
        /// <summary>
        /// Handles the Click event of the btnZoomToGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void btnZoomToGrid_Click(object sender, EventArgs e)
        {
            ZoomToGrid();
        }
        /// <summary>
        /// Handles the Click event of the btnExportContours control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void btnExportContours_Click(object sender, EventArgs e)
        {
            ExportContoursToShapefile("Contours");
        }
        /// <summary>
        /// Handles the Click event of the btnImportShapefile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void btnImportShapefile_Click(object sender, EventArgs e)
        {
            ImportShapefile();
        }

        #endregion

        #region mapControl Event Handlers
        /// <summary>
        /// Handles the MouseMove event of the mapControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        void mapControl_MouseMove(object sender, MouseEventArgs e)
        {
            // add code here   
        }
        /// <summary>
        /// Handles the MouseClick event of the mapControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        void mapControl_MouseClick(object sender, MouseEventArgs e)
        {
            // add code here
        }
        /// <summary>
        /// Handles the MouseDoubleClick event of the mapControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        void mapControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // add code here
        }
        #endregion

        #region mapLegend Event Handlers
        /// <summary>
        /// Handles the LayerVisibilityChanged event of the mapLegend control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        void mapLegend_LayerVisibilityChanged(object sender, EventArgs e)
        {
            mapControl.Refresh();
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Exports the contours to a shapefile.
        /// </summary>
        /// <param name="baseShapefileName">Name of the base shapefile.</param>
        /// <remarks></remarks>
        private void ExportContoursToShapefile(string baseShapefileName)
        {
            if (_ContourEngineData.ContourSourceFile != null)
            {
                if (_CurrentContourMapLayer != null)
                {
                    try
                    {
                        string directory = System.IO.Path.GetDirectoryName(_ContourEngineData.ContourSourceFile.Filename);
                        FeatureCollection featureList = _CurrentContourMapLayer.GetFeatures();
                        USGS.Puma.IO.EsriShapefileIO.Export(featureList, directory, baseShapefileName);
                        MessageBox.Show("Features were exported to shapefile:");
                    }
                    catch
                    {
                        MessageBox.Show("Error exporting features as shapefile.");
                    }
                }
                else
                {
                    MessageBox.Show("No head data layer is currently selected.");
                }
            }
            else
            {
                MessageBox.Show("No MODFLOW head file output is loaded.");
            }

        }
        /// <summary>
        /// Reads an ESRI shapefile and imports the features as a FeatureCollection
        /// The shapefile name is the full pathname, including the .shp extension.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private void ImportShapefile()
        {
            if(_ContourEngineData==null)
            { return;}
            if(_ContourEngineData.ContourSourceFile==null)
            {return;}

            FeatureCollection featureList = null;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = System.IO.Path.GetDirectoryName(_ContourEngineData.ContourSourceFile.Filename);
            dialog.Filter = "*.shp (ESRI shapefiles)|*.shp|*.* (All files)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Read ESRI shapefile and import features to a FeatureCollection
                    featureList = USGS.Puma.IO.EsriShapefileIO.Import(dialog.FileName);
                    _ImportedShapefileMapLayer = CreateMapLayerFromFeatureCollection(featureList, System.Drawing.Color.Blue, 1.0f, 1);
                    string layerName = System.IO.Path.GetFileNameWithoutExtension(dialog.FileName);
                    _ImportedShapefileMapLayer.LayerName = layerName;
                    BuildMapLayers(true);
                }
                catch
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Creates the map layer from a collection of features.
        /// Sets up the layer to use a single-symbol renderer with the specified color, size, and symbol style.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="color">The color.</param>
        /// <param name="size">The size.</param>
        /// <param name="symbolStyle">The symbol style.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private FeatureLayer CreateMapLayerFromFeatureCollection(FeatureCollection featureList, System.Drawing.Color color, float size, int symbolStyle)
        {

            FeatureLayer layer = null;

            if (featureList != null)
            {
                if (featureList.Count > 0)
                {
                    Feature f = featureList[0];
                    if (f.Geometry is IMultiLineString || f.Geometry is ILineString)
                    {
                        layer = new FeatureLayer(LayerGeometryType.Line);
                        ILineSymbol symbol = ((ISingleSymbolRenderer)(layer.Renderer)).Symbol as ILineSymbol;
                        symbol.Color = color;
                        symbol.Width = size;
                    }
                    else if (f.Geometry is IPolygon)
                    {
                        layer = new FeatureLayer(LayerGeometryType.Polygon);
                        ISolidFillSymbol symbol = ((ISingleSymbolRenderer)(layer.Renderer)).Symbol as ISolidFillSymbol;
                        symbol.Color = color;
                    }
                    else if (f.Geometry is IPoint)
                    {
                        layer = new FeatureLayer(LayerGeometryType.Point);
                        SimplePointSymbol symbol = (((layer.Renderer as SingleSymbolRenderer).Symbol) as SimplePointSymbol);
                        symbol.Color = color;
                        symbol.Size = size;
                        if (symbolStyle == 1)
                        {
                            symbol.SymbolType = PointSymbolTypes.Circle;
                        }
                        else if (symbolStyle == 2)
                        {
                            symbol.SymbolType = PointSymbolTypes.Square;
                        }
                        else
                        {
                            symbol.SymbolType = PointSymbolTypes.Circle;
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Cannot create layer for the specified feature type.");
                    }

                    for (int i = 0; i < featureList.Count; i++)
                    {
                        layer.AddFeature(featureList[i]);
                    }

                    layer.Visible = true;

                }
            }

            return layer;

        }
        /// <summary>
        /// Creates a FeatureLayer representing the interior model gridlines
        /// </summary>
        /// <param name="modelGrid"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private FeatureLayer CreateModelGridlinesLayer(CellCenteredArealGrid modelGrid, Color color)
        {
            IMultiLineString outline = modelGrid.GetOutline();
            IMultiLineString[] gridlines = modelGrid.GetGridLines();

            FeatureLayer layer = new FeatureLayer(LayerGeometryType.Line);
            ILineSymbol symbol = ((ISingleSymbolRenderer)(layer.Renderer)).Symbol as ILineSymbol;
            symbol.Color = color;
            symbol.Width = 1.0f;

            IAttributesTable attributes = null;
            int value = 1;
            for (int i = 0; i < gridlines.Length; i++)
            {
                attributes = new AttributesTable();
                attributes.AddAttribute("Value", value);
                layer.AddFeature(gridlines[i] as IGeometry, attributes);
            }

            attributes = new AttributesTable();
            value = 0;
            attributes.AddAttribute("Value", value);
            layer.AddFeature(outline as IGeometry, attributes);
            layer.Visible = false;

            layer.LayerName = "Model gridlines";
            layer.Visible = true;
            return layer;

        }
        /// <summary>
        /// Creates a FeatureLayer representing the outline of the model grid
        /// </summary>
        /// <param name="modelGrid"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private FeatureLayer CreateModelGridOutlineLayer(CellCenteredArealGrid modelGrid, Color color)
        {
            IMultiLineString outline = modelGrid.GetOutline();
            if (outline == null)
                throw new Exception("The model grid outline was not created.");

            FeatureLayer layer = new FeatureLayer(LayerGeometryType.Line);
            ILineSymbol symbol = ((ISingleSymbolRenderer)(layer.Renderer)).Symbol as ILineSymbol;
            symbol.Color = color;
            symbol.Width = 1.0f;

            IAttributesTable attributes = new AttributesTable();
            int value = 0;
            attributes.AddAttribute("Value", value);
            layer.AddFeature(outline as IGeometry, attributes);

            layer.LayerName = "Model grid outline";
            layer.Visible = true;
            return layer;

        }
        /// <summary>
        /// Zooms the viewport of the map to the extent of the model grid.
        /// </summary>
        private void ZoomToGrid()
        {
            if (_GridOutlineMapLayer != null)
            {
                IEnvelope rect = _GridOutlineMapLayer.Extent;
                mapControl.SetExtent(rect.MinX, rect.MaxX, rect.MinY, rect.MaxY);
            }
        }
        /// <summary>
        /// Clears the map legend.
        /// </summary>
        /// <remarks></remarks>
        private void ClearMapLegend()
        {
            mapLegend.Clear();
            mapLegend.LegendTitle = "";
        }
        /// <summary>
        /// Builds the map legend
        /// </summary>
        private void BuildMapLegend()
        {
            ClearMapLegend();
            Collection<GraphicLayer> legendItems = new Collection<GraphicLayer>();

            // Add model grid outline
            if (_GridOutlineMapLayer != null)
            {
                legendItems.Add(_GridOutlineMapLayer);
            }

            // Add model grid interior lines
            if (_GridlinesMapLayer != null)
            {
                legendItems.Add(_GridlinesMapLayer);
            }

            // Add head contour layer
            if (_CurrentContourMapLayer != null)
            {
                legendItems.Add(_CurrentContourMapLayer);
            }

            // Add imported shapefile layer
            if (_ImportedShapefileMapLayer != null)
            {
                legendItems.Add(_ImportedShapefileMapLayer);
            }

            mapLegend.LegendTitle = "Map layers";
            mapLegend.AddItems(legendItems);
        }
        /// <summary>
        /// Builds the map.
        /// Previous content is cleared. The current copies of the grid and contour map layers are added to the map,
        /// and the map is refreshed.
        /// </summary>
        /// <param name="fullExtent"></param>
        private void BuildMapLayers(bool fullExtent)
        {
            bool forceFullExtent = fullExtent;
            if (mapControl.LayerCount == 0)
                forceFullExtent = true;

            mapControl.ClearLayers();

            // Interior grid lines
            if (_GridlinesMapLayer != null)
            { mapControl.AddLayer(_GridlinesMapLayer); }

            // Grid outline
            if (_GridOutlineMapLayer != null)
            { mapControl.AddLayer(_GridOutlineMapLayer); }

            // Head contours
            if (_CurrentContourMapLayer != null)
            {
                mapControl.AddLayer(_CurrentContourMapLayer);
            }

            // Imported shapefile
            if (_ImportedShapefileMapLayer != null)
            {
                mapControl.AddLayer(_ImportedShapefileMapLayer);
            }

            // Prepare and display the map
            if (mapControl.LayerCount > 0)
            {
                if (forceFullExtent)
                {
                    mapControl.SizeToFullExtent();
                }
            }
            BuildMapLegend();
            mapControl.Refresh();
            indexMap.UpdateMapImage();

        }
        /// <summary>
        /// Manages creating the contour features and building the contour FeatureLayer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="modelGrid"></param>
        private void GenerateAndBuildContourLayer(Array2d<float> buffer, CellCenteredArealGrid modelGrid)
        {
            ContourLineList contourList = GenerateContours(buffer, modelGrid);
            FeatureLayer contourLayer = BuildContourLayer(contourList);
            bool layerVisible = true;
            if (_CurrentContourMapLayer != null)
            {
                layerVisible = _CurrentContourMapLayer.Visible;
            }
            contourLayer.LayerName = "Head, Layer " + _ContourEngineData.SelectedDataLayer.Layer.ToString() + " (Period " + _ContourEngineData.SelectedDataLayer.StressPeriod.ToString() + ", Step " + _ContourEngineData.SelectedDataLayer.TimeStep.ToString() + ")";
            _CurrentContourMapLayer = contourLayer;
            _CurrentContourMapLayer.Visible = layerVisible;
        }
        /// <summary>
        /// Generates contour features for the specified layer array buffer.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="modelGrid"></param>
        /// <returns></returns>
        private ContourLineList GenerateContours(Array2d<float> buffer, CellCenteredArealGrid modelGrid)
        {
            if (buffer == null)
                throw new ArgumentNullException();
            if ((buffer.RowCount != modelGrid.RowCount) || (buffer.ColumnCount != modelGrid.ColumnCount))
                throw new ArgumentException("Array does not match model grid dimensions.");

            ContourEngine ce = new ContourEngine(modelGrid);

            ce.UseDefaultNoDataRange = false;
            foreach (float excludedValue in _ContourEngineData.ExcludedValues)
            {
                ce.ExcludedValues.Add(excludedValue);
            }
            ce.LayerArray = buffer;
            float refContour = _ContourEngineData.ReferenceContour;

            float conInterval;

            switch (_ContourEngineData.ContourIntervalOption)
            {
                case ContourIntervalOption.AutomaticConstantInterval:
                    conInterval = ce.ComputeContourInterval();
                    ce.ContourLevels = ce.GenerateConstantIntervals(conInterval, refContour);
                    break;
                case ContourIntervalOption.SpecifiedConstantInterval:
                    conInterval = _ContourEngineData.ConstantContourInterval;
                    ce.ContourLevels = ce.GenerateConstantIntervals(conInterval, refContour);
                    break;
                case ContourIntervalOption.SpecifiedContourLevels:
                    ce.ContourLevels = _ContourEngineData.ContourLevels;
                    break;
                default:
                    break;
            }

            ContourLineList conLineList = ce.CreateContours();
            return conLineList;

        }
        /// <summary>
        /// Builds the contour FeatureLayer from the contour line list obtained from the GenerateContours method.
        /// </summary>
        /// <param name="contourList"></param>
        /// <returns></returns>
        private FeatureLayer BuildContourLayer(ContourLineList contourList)
        {
            if (contourList == null)
                throw new ArgumentNullException("The specified contour list does not exist.");

            FeatureLayer contourLayer = new FeatureLayer(LayerGeometryType.Line);
            ILineSymbol symbol = ((ISingleSymbolRenderer)(contourLayer.Renderer)).Symbol as ILineSymbol;
            symbol.Color = _ContourEngineData.ContourColor;
            symbol.Width = Convert.ToSingle(_ContourEngineData.ContourLineWidth);
            for (int i = 0; i < contourList.Count; i++)
            {
                IAttributesTable attributes = new AttributesTable();
                attributes.AddAttribute("Value", contourList[i].ContourLevel);
                contourLayer.AddFeature(contourList[i].Contour as IGeometry, attributes);
            }

            contourLayer.LayerName = "Current Data Contours";
            return contourLayer;
        }
        /// <summary>
        /// Provides access to a dialog that allows contour data to be edited.
        /// </summary>
        /// <remarks></remarks>
        private void EditContourLayer()
        {
            if (_ContourEngineData != null)
            {
                ModflowOutputContoursEditDialog dialog = new ModflowOutputContoursEditDialog();
                dialog.ContourData = _ContourEngineData;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // If a head file is loaded, generate contours, build contour map layer, and rebuild map and legend.
                    if (_ContourEngineData.ContourSourceFile != null)
                    {
                        // Retrieve the data layer record for the selected head data layer.
                        LayerDataRecord<float> headLayerRecord = _ContourEngineData.ContourSourceFile.GetRecordAsSingle(_ContourEngineData.SelectedDataLayer);
                        if (headLayerRecord != null)
                        {
                            // Generate contours and create a new contours FeatureLayer based on the currently selected layer data and the model grid.
                            GenerateAndBuildContourLayer(headLayerRecord.DataArray, _ModelGrid);
                        }
                        else
                        {
                            // Set the current contour FeatureLayer to null
                            _CurrentContourMapLayer = null;
                        }
                        // Rebuild the map, then rebuild the map legend.
                        BuildMapLayers(false);
                        BuildMapLegend();
                    }
                }
            }
        }
        /// <summary>
        /// Provides access to an OpenFileDialog to select a MODFLOW head file.
        /// If a head file is selected, a BinaryLayerReader is connected to the 
        /// selected head file.
        /// </summary>
        /// <remarks></remarks>
        private void OpenHeadFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "*.hed (MODFLOW head files)|*.hed|*.* (All files)|*.*";

            // If a valid file was selected, open the newly selected head file and process the data
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Create a BinaryLayerReader object connected to the selected head file.
                BinaryLayerReader headReader = new BinaryLayerReader(dialog.FileName);

                // Check to see if a valid MODFLOW head file was successfully connected. If so, process the data.
                // Otherwise, show an error message.
                if (headReader.Valid)
                {
                    // Set the ContourSourceFile property equal to the headReader object that was just created.
                    _ContourEngineData.ContourSourceFile = headReader;

                    // Set the SelectedDataLayer property equal to the first header record.
                    _ContourEngineData.SelectedDataLayer = headReader.ReadRecordHeader(0);

                    // Create a new model grid object with with the number of rows and columns that correspond
                    // to the current head file. No grid spacing data is available, so assume a square grid with grid spacing equal to 
                    // 1 in both the row and column direction. 
                    _ModelGrid = new CellCenteredArealGrid(headReader.RowCount, headReader.ColumnCount, 1.0, 0.0, 0.0, 0.0);

                    // Create the model grid outline and model grid interior gridlines FeatureLayers
                    _GridOutlineMapLayer = CreateModelGridOutlineLayer(_ModelGrid, System.Drawing.Color.Black);
                    _GridlinesMapLayer = CreateModelGridlinesLayer(_ModelGrid, System.Drawing.Color.LightGray);

                    // Retrieve a LayerDataRecord object corresponding to the selected data layer.
                    LayerDataRecord<float> layerRecord = _ContourEngineData.ContourSourceFile.GetRecordAsSingle(0);

                    // Create contours and build contours FeatureLayer
                    GenerateAndBuildContourLayer(layerRecord.DataArray, _ModelGrid);

                    // Rebuild mapControl display
                    BuildMapLayers(false);

                    // Rebuild mapLegend display
                    BuildMapLegend();

                    // Update selected head file name
                    txtHeadFile.Text = headReader.Filename;
                }
                else
                {
                    MessageBox.Show("An error occurred processing the selected file." + Environment.NewLine + "The file may not be a valid MODFLOW head file.");
                    return;
                }
            }

        }

        #endregion



    }

}
