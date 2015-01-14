using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using GeoAPI.Geometries;

using ScenarioTools.DataClasses;
using ScenarioTools.Geometry;
using ScenarioTools.Graphics;
using ScenarioTools.Spatial;

using USGS.ModflowTrainingTools;
using USGS.Puma.Core;
using USGS.Puma.FiniteDifference;
using USGS.Puma.NTS.Features;
using USGS.Puma.NTS.Geometries;
using USGS.Puma.UI.MapViewer;

namespace ScenarioTools.Reporting
{
    public class STMap : Map
    {

        #region Fields
        private GeoImage _backgroundImage = null;
        private List<STMapLayer> _stMapLayerList;
        private MapControl _mapControl;
        private STMapLegend _stMapLegend;
        private Viewport _Viewport = null;
        private RendererHelper _RH = null;
        private Extent _extent;
        private bool _displayingMessage = false;
        private bool _showBackgroundImage = true;
        private string _desiredExtentName;
        private Image _image;
        double _scaleFactor = 0.0;
        private ImageInfo _imageInfo;
        #endregion Fields

        #region Constructors
        public STMap() : base()
        {
            Name = "";
            _desiredExtentName = "";
            _extent = new Extent();
            _imageInfo = null;
            BackgroundImageBrightness = 0;
            MapDesignerHeight = 600;
            MapDesignerWidth = 800;
            ExplanationWidthRatio = 0.15;
            ScaleHeightRatio = 0.1;
            ContourInterval = 0.0f;
            LabelContours = true;
            ShowBackgroundImage = false;
            EventHandlersAdded = false;
            _stMapLayerList = new List<STMapLayer>();
            _RH = new RendererHelper();
            MapBackgroundColor = System.Drawing.Color.White;
            _Viewport = new Viewport(new System.Drawing.Size(1000, 1000), new USGS.Puma.NTS.Geometries.Envelope(0, 1, 0, 1));
            NeatLineColor = Color.Aqua;

            // Construct the MapControl
            _mapControl = new MapControl(this);
            _mapControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            _mapControl.TabIndex = 0;
            _mapControl.Dock = DockStyle.Fill;
            _mapControl.Size = _Viewport.ViewportSize;

            // Construct the STMapLegend
            _stMapLegend = new STMapLegend();
            _stMapLegend.BorderStyle = BorderStyle.FixedSingle;
            _stMapLegend.TabIndex = 0;
            _stMapLegend.Dock = DockStyle.Fill;
            _stMapLegend.AutoSize = true;

            // Connect STMapLegend events to event handlers
            _stMapLegend.LayerVisibilityChanged += new EventHandler<EventArgs>(STMapLegend_LayerVisibilityChanged);
        }
        public STMap(string name) : this()
        {
            Name = name;
        }
        public STMap(STMap sourceSTMap)
        {
            Name = sourceSTMap.Name;
            _desiredExtentName = sourceSTMap._desiredExtentName;
            if (sourceSTMap._extent != null)
            {
                _extent = new Extent(sourceSTMap._extent);
            }
            else
            {
                _extent = new Extent();
            }
            if (sourceSTMap._imageInfo != null)
            {
                this._imageInfo = new ImageInfo(sourceSTMap._imageInfo.ImagefileAbsolutePath);
            }
            BackgroundImageBrightness = sourceSTMap.BackgroundImageBrightness;
            MapDesignerHeight = sourceSTMap.MapDesignerHeight;
            MapDesignerWidth = sourceSTMap.MapDesignerWidth;
            ExplanationWidthRatio = sourceSTMap.ExplanationWidthRatio;
            ScaleHeightRatio = sourceSTMap.ScaleHeightRatio;
            ContourInterval = sourceSTMap.ContourInterval;
            LabelContours = sourceSTMap.LabelContours;
            ShowBackgroundImage = sourceSTMap.ShowBackgroundImage;
            EventHandlersAdded = false;
            _stMapLayerList = new List<STMapLayer>();
            for (int i = 0; i < sourceSTMap._stMapLayerList.Count; i++)
            {
                _stMapLayerList.Add(sourceSTMap._stMapLayerList[i]);
            }
            _RH = new RendererHelper();
            MapBackgroundColor = sourceSTMap.MapBackgroundColor;
            _Viewport = new Viewport(sourceSTMap._Viewport.ViewportSize, sourceSTMap._Viewport.TargetWorldExtent);
            NeatLineColor = sourceSTMap.NeatLineColor;

            // Construct the MapControl
            _mapControl = new MapControl(this);
            _mapControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            _mapControl.TabIndex = 0;
            _mapControl.Dock = DockStyle.Fill;
            _mapControl.ViewportSize = sourceSTMap._mapControl.ViewportSize;
            for (int i = 0; i < sourceSTMap._mapControl.LayerCount; i++)
            {
                _mapControl.AddLayer(sourceSTMap._mapControl.GetLayer(i));
            }
            if (sourceSTMap._mapControl.Image != null)
            {
                _mapControl.Image = new Bitmap(sourceSTMap._mapControl.Image);
            }
            if (sourceSTMap._mapControl.MapExtent != null)
            {
                _mapControl.MapExtent = new Extent(sourceSTMap._mapControl.MapExtent);
            }
            else
            {
                _mapControl.SizeToFullExtent();
            }

            // Construct the STMapLegend
            _stMapLegend = new STMapLegend();
            _stMapLegend.BorderStyle = BorderStyle.FixedSingle;
            _stMapLegend.TabIndex = 0;
            _stMapLegend.Dock = DockStyle.Fill;
            _stMapLegend.AutoSize = true;
            _stMapLegend.LegendTitle = sourceSTMap._stMapLegend.LegendTitle;
            for (int i = 0; i < sourceSTMap._stMapLegend.Items.Count; i++)
            {
                _stMapLegend.Items.Add(sourceSTMap._stMapLegend.Items[i]);
            }

            // Connect STMapLegend events to event handlers
            _stMapLegend.LayerVisibilityChanged += new EventHandler<EventArgs>(STMapLegend_LayerVisibilityChanged);
        }
        #endregion Constructors

        public Extent GenerateAutomaticExtent()
        {
            List<IEnvelope> envelopes = new List<IEnvelope>();
            for (int i = 0; i < this._stMapLayerList.Count; i++)
            {
                try
                {
                    // Only include non-null extents
                    if (!_stMapLayerList[i].Extent.IsNull)
                    {
                        envelopes.Add(_stMapLayerList[i].Extent);
                    }
                }
                catch
                {
                }
            }
            return Extent.GetEnclosingExtent(envelopes);
        }
        public Extent GenerateAutomaticExtent(List<Extent> extents)
        {
            // If the extents list only contains one extent, just copy that one and return it
            if (extents.Count == 1)
            {
                return new Extent(extents[0]);
            }

            List<IEnvelope> envelopes = new List<IEnvelope>();
            for (int i = 0; i < extents.Count; i++)
            {
                // Only include non-null extents
                if (!Extent.ExtentIsNull(extents[i]))
                {
                    envelopes.Add(extents[i]);
                }
            }
            return Extent.GetEnclosingExtent(envelopes);
        }

        #region Properties
        public GeoImage BackgroundImage
        {
            get
            {
                return _backgroundImage;
            }
            private set
            {
                _backgroundImage = value;
            }
        }
        public int BackgroundImageBrightness { get; set; }
        public int MapDesignerHeight { get; set; }
        public int MapDesignerWidth { get; set; }
        public int STMapLayerCount
        {
            get
            {
                return _stMapLayerList.Count;
            }
        }
        public string Name { get; set; }
        public string BackgroundImageFile 
        {
            get
            {
                return GlobalStaticVariables.BackgroundImageFile;
            }
            set
            {
                _imageInfo = new ImageInfo(value);
                GlobalStaticVariables.BackgroundImageFile = value;
            }
        }
        public string ExtentName
        {
            get
            {
                if (_extent != null)
                {
                    return _extent.Name;
                }
                return "";
            }
            set
            {
                if (_extent != null)
                {
                    if (_extent.Name != value)
                    {
                        ClearImage();
                        _extent.Name = value;
                    }
                }
            }
        }
        public string DesiredExtentName
        {
            get
            {
                return _desiredExtentName;
            }
            set
            {
                if (value != _desiredExtentName)
                {                    
                    ClearImage();
                    _desiredExtentName = value;
                }
            }
        }
        public float ContourInterval { get; set; }
        public double ExplanationWidthRatio { get; set; } // Proportion of image width  
        public double ScaleHeightRatio { get; set; } // Proportion of image height
        public bool LabelContours { get; set; }
        public bool ShowBackgroundImage 
        {
            get
            {
                return _showBackgroundImage;
            }
            set
            {
                _showBackgroundImage = value;
            }
        }
        public bool EventHandlersAdded { get; set; }
        public List<DataSeries> DataSeriesList
        {
            get
            {
                List<DataSeries> dsList = new List<DataSeries>();
                for (int i=0;i<_stMapLayerList.Count;i++)
                {
                    dsList.Add(_stMapLayerList[i].DataSeries);
                }
                return dsList;
            }
        }
        public MapControl MapControl
        {
            get
            {
                return _mapControl;
            }
            set
            {
                _mapControl = value;
            }
        }
        public STMapLegend STMapLegend
        {
            get
            {
                return _stMapLegend;
            }
            set
            {
                _stMapLegend = value;
            }
        }
        public Extent Extent
        {
            get
            {
                return _extent;
            }
            set
            {
                if (value == null)
                {
                    ClearImage();
                    _extent = null;
                }
                else
                {
                    if (_extent == null)
                    {
                        ClearImage();
                        _extent = value;
                    }
                    else
                    {
                        if (!_extent.Equals(value))
                        {
                            ClearImage();
                            _extent = value;
                        }
                    }
                    _mapControl.MapExtent = _extent;
                    _mapControl.SetExtent(_extent.MinX, _extent.MaxX, _extent.MinY, _extent.MaxY);
                    SetExtent(_extent.MinX, _extent.MaxX, _extent.MinY, _extent.MaxY);
                }
            }
        }
        public Color NeatLineColor { get; set; }
        public Image Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
            }
        }
        public Viewport Viewport
        {
            get
            {
                return _Viewport;
            }
        }
        #endregion Properties

        #region IReportElement members
        public int DataSeriesCount()
        {
            return _stMapLayerList.Count;
        }
        public void AddDataSeries(DataSeries dataSeries)
        {
            ClearImage();
            STMapLayer newLayer = new STMapLayer(dataSeries);
            _stMapLayerList.Add(newLayer);
        }
        public void RemoveDataSeries(DataSeries dataSeries)
        {
            ClearImage();
            for (int i = 0; i < _stMapLayerList.Count; i++)
            {
                if (_stMapLayerList[i].DataSeries == dataSeries)
                {
                    _stMapLayerList.Remove(_stMapLayerList[i]);
                    break;
                }
            }
        }
        public DataSeries GetDataSeries(int index)
        {
            return _stMapLayerList[index].DataSeries;
        }
        public object GetDataSeries(string name)
        {
            for (int i = 0; i < _stMapLayerList.Count; i++)
            {
                if (name.Equals(_stMapLayerList[i].DataSeries.Name))
                {
                    return (GeoMap)_stMapLayerList[i].DataSeries.GetDataSynchronous();
                }
            }
            // If none was found, return null.
            return null;
        }
        public void ClearDataSeries()
        {
            ClearImage();
            _stMapLayerList.Clear();
        }
        #endregion IReportElement members

        #region STMapLegend Event Handlers
        /// <summary>
        /// Handles the LayerVisibilityChanged event of the STMapLegend control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        void STMapLegend_LayerVisibilityChanged(object sender, EventArgs e)
        {
            BuildMapLayers(false, null, null, _showBackgroundImage, false);
            _mapControl.Refresh();
        }
        #endregion

        #region IMap members
        public new System.Drawing.Image RenderAsImage()
        {
            #region Draw map image
            populateLayers();
            Image image = _mapControl.RenderAsImage(this._Viewport.ViewportSize);

            Extent tempExtent;
            if (Extent.ExtentIsNull(_extent))
            {
                tempExtent = GenerateAutomaticExtent();
            }
            else
            {
                tempExtent = _extent;
            }

            if (!Extent.ExtentIsNull(tempExtent))
            {
                // Calculate scale factor
                int imageWidth = _Viewport.ViewportSize.Width;
                int imageHeight = _Viewport.ViewportSize.Height;
                double extentXMin = tempExtent.MinX;
                double extentXMax = tempExtent.MaxX;
                double extentYMin = tempExtent.MinY;
                double extentYMax = tempExtent.MaxY;
                double extentWidth = Math.Abs(extentXMax - extentXMin);
                double extentHeight = Math.Abs(extentYMax - extentYMin);
                double imageAspectRatio = Convert.ToDouble(imageWidth) / Convert.ToDouble(imageHeight);
                double extentAspectRatio = extentWidth / extentHeight;
                if (imageAspectRatio > extentAspectRatio)
                {
                    _scaleFactor = Convert.ToDouble(imageHeight) / extentHeight;
                }
                else
                {
                    _scaleFactor = Convert.ToDouble(imageWidth) / extentWidth;
                }

                if (LabelContours)
                {
                    drawContourLabels(image);
                }
            #endregion Draw map image

                #region Crop image
                // Erase any parts of features outside extent
                int clipXMin = 0;
                int clipYMin = 0;
                int clipXMax = Convert.ToInt32(extentWidth * _scaleFactor);
                int clipYMax = Convert.ToInt32(extentHeight * _scaleFactor);
                int width = Math.Abs(clipXMax - clipXMin);
                int height = Math.Abs(clipYMax - clipYMin);

                // Ensure that crop rectangle dimensions do not extend beyond image dimensions
                if (width > image.Width)
                {
                    width = image.Width;
                }
                if (height > image.Height)
                {
                    height = image.Height;
                }
                int originX = (imageWidth - clipXMax) / 2;
                if (originX < 0) originX = 0;
                if (originX + width > imageWidth)
                {
                    width = imageWidth - originX;
                }
                int originY = (imageHeight - clipYMax) / 2;
                if (originY < 0) originY = 0;
                if (originY + height > imageHeight)
                {
                    height = imageHeight - originY;
                }
                Rectangle clipRectangle = new Rectangle(originX, originY, width, height);
                image = GraphicsHelpers.CropImage(image, clipRectangle);
                //    }
                //}
                #endregion Crop image

                #region Draw scale bar
                // Draw scale bar and stack under map image
                Image scaleBarImage = null;
                double geoExtentMin = ((IEnvelope)tempExtent).MinX;
                double geoExtentMax = ((IEnvelope)tempExtent).MaxX;
                double geoY = ((IEnvelope)tempExtent).MinY;
                _Viewport.SetTargetWorldExtent(tempExtent);
                PointF p0 = _Viewport.ToDevicePoint(geoExtentMin, geoY);
                PointF p1 = _Viewport.ToDevicePoint(geoExtentMax, geoY);
                double imageExtentWidth = p1.X - p0.X;
                int scaleBarHeight = 60;
                string lengthUnitAsString = ScenarioTools.DataClasses.LengthReference.ConvertLengthUnitToString(GlobalStaticVariables.GlobalLengthUnit);
                scaleBarImage = GraphicsHelpers.MakeScaleBarImage(_Viewport.TargetWorldExtent, imageExtentWidth,
                    image.Width, scaleBarHeight, lengthUnitAsString);

                if (scaleBarImage != null)
                {
                    // Append scale bar image to bottom of map image
                    image = GraphicsHelpers.StackImages(image, scaleBarImage);
                }
                #endregion Draw scale bar

                #region Draw legend
                // Draw explanation
                int legendWidth = 400;
                Image legendImage = renderLegend(legendWidth, image.Height);

                if (legendImage != null)
                {
                    // Append explanation image to right of map/scale bar image
                    image = GraphicsHelpers.AppendImages(image, legendImage);
                }
                #endregion Draw legend
            }

            Image = image; // Save image in object instance
            return image;
        }
        public new System.Drawing.Image RenderAsImage(GeoAPI.Geometries.IEnvelope extent)
        {
            throw new NotImplementedException();
        }
        public new System.Drawing.Image RenderAsImage(System.Drawing.Size size)
        {
            populateLayers();
            return _mapControl.RenderAsImage(size);
        }
        public new System.Drawing.Image RenderAsImage(System.Drawing.Size size, GeoAPI.Geometries.IEnvelope extent)
        {
            populateLayers();
            return _mapControl.RenderAsImage(size, extent);
        }
        #endregion IMap members

        public void AddLayer(STMapLayer stMapLayer)
        {
            _stMapLayerList.Add(stMapLayer);
            AddLayer(stMapLayer.FeatureLayer);
        }
        private void drawBackground(GeoImage targetImage)
        {
            // Load the background image, if not already loaded.
            if (_backgroundImage == null)
            {
                try
                {
                    // Load the georeferenced image.
                    if (File.Exists(BackgroundImageFile))
                    {
                        _backgroundImage = new GeoImage(BackgroundImageFile);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception loading background image: " + e.Message);
                    return;
                }
            }

            // Draw the background to the provided image.
            if (_backgroundImage != null)
            {
                _backgroundImage.DrawTo(targetImage);
            }
        }

        /// <summary>
        /// Builds the contour FeatureLayer from the contour line list obtained from the generateContours method.
        /// </summary>
        /// <param name="contourList"></param>
        /// <returns></returns>
        private FeatureLayer buildContourLayer(ContourLineList contourList, ContourEngineData contourEngineData, string name)
        {
            if (contourList == null)
                throw new ArgumentNullException("The specified contour list does not exist.");

            FeatureLayer contourLayer = new FeatureLayer(LayerGeometryType.Line);
            ILineSymbol symbol = ((ISingleSymbolRenderer)(contourLayer.Renderer)).Symbol as ILineSymbol;
            symbol.Color = contourEngineData.ContourColor;
            symbol.Width = Convert.ToSingle(contourEngineData.ContourLineWidth);
            for (int i = 0; i < contourList.Count; i++)
            {
                IAttributesTable attributes = new AttributesTable();
                attributes.AddAttribute("value", contourList[i].ContourLevel);
                contourLayer.AddFeature(contourList[i].Contour as IGeometry, attributes);
            }
            if (name != "")
            {
                contourLayer.LayerName = name;
            }
            else
            {
                contourLayer.LayerName = "Current Data Contours";
            }
            return contourLayer;
        }
        private FeatureLayer buildColorFillLayer(List<USGS.Puma.NTS.Features.Feature> featureList, ColorRamp colorRamp, string name)
        {
            FeatureLayer colorFillLayer = new FeatureLayer(LayerGeometryType.Polygon);
            float minValue = 1.0E+30f;
            float maxValue = -1.0E+30f;
            float value;
            float position;
            for (int i = 0; i < featureList.Count; i++)
            {
                if (featureList[i].Attributes.Exists("value"))
                {
                    value = (float)featureList[i].Attributes["value"];
                    if (value < minValue)
                    {
                        minValue = value;
                    }
                    if (value > maxValue)
                    {
                        maxValue = value;
                    }
                }
            }
            for (int i = 0; i < featureList.Count; i++)
            {
                if (featureList[i].Attributes.Exists("value"))
                {
                    value = (float)featureList[i].Attributes["value"];
                    position = value / (maxValue - minValue);
                }
                else
                {
                    position = 0.0f;
                }
                colorFillLayer.AddFeature(featureList[i]);
            }
            ColorRampRenderer colorRampRenderer = new ColorRampRenderer(SymbolType.FillSymbol, colorRamp);
            colorRampRenderer.MaximumValue = maxValue;
            colorRampRenderer.MinimumValue = minValue;
            SolidFillSymbol solidFillSymbol = new SolidFillSymbol();
            solidFillSymbol.Color = System.Drawing.Color.Beige;
            colorRampRenderer.BaseSymbol = solidFillSymbol;
            colorRampRenderer.RenderField = "value";
            colorFillLayer.Renderer = colorRampRenderer;
            if (name != "")
            {
                colorFillLayer.LayerName = name;
            }
            else
            {
                colorFillLayer.LayerName = "Current Data Color Fill";
            }
            return colorFillLayer;
        }
        private bool ensureBackgroundImageLayerIsBuilt(bool forceRebuild)
        {
            if (forceRebuild)
            {
                GlobalStaticVariables.BackgroundImageLayer = null;
            }
            if (GlobalStaticVariables.BackgroundImageLayer == null)
            {
                if (File.Exists(GlobalStaticVariables.BackgroundImageFile))
                {
                    try
                    {
                        // Ned TODO: use background worker; see GdalHelper.ToBitmap and ScenarioManager.MainForm ~line 1700.
                        GlobalStaticVariables.BackgroundImageLayer = new ImageLayer(GlobalStaticVariables.BackgroundImageFile);
                        if (GlobalStaticVariables.BackgroundImageLayer != null)
                        {
                            if (GlobalStaticVariables.BackgroundImageLayer.Extent is Extent)
                            {
                                ((Extent)GlobalStaticVariables.BackgroundImageLayer.Extent).Name = WorkspaceUtil.BACKGROUND_IMAGE_EXTENT_NAME;
                            }
                        }
                        GlobalStaticVariables.BackgroundImageLayer.Brightness = BackgroundImageBrightness;
                        _imageInfo = new ImageInfo(GlobalStaticVariables.BackgroundImageFile);
                        GlobalSpatialStaticVariables.SpatialReference = _imageInfo.SpatialReference;
                        _backgroundImage = GlobalStaticVariables.BackgroundImageLayer.GetGeoImage();
                    }
                    catch
                    {
                    }
                }
            }
            if (GlobalStaticVariables.BackgroundImageLayer != null)
            {
                if (GlobalStaticVariables.BackgroundImageLayer.Brightness != BackgroundImageBrightness)
                {
                    GlobalStaticVariables.BackgroundImageLayer.Brightness = BackgroundImageBrightness;
                    _backgroundImage = GlobalStaticVariables.BackgroundImageLayer.GetGeoImage();
                }
            }

            return GlobalStaticVariables.BackgroundImageLayer != null;
        }

        /// <summary>
        /// Builds the map.
        /// Previous content is cleared. The current copies of the grid and contour map layers are added to the map,
        /// and the map is refreshed.
        /// </summary>
        /// <param name="fullExtent"></param>
        public void BuildMapLayers(bool fullExtent, List<STMapLayer> mapLayerList, Extent specifiedExtent, bool showBackgroundImage, bool forceBackgroundRebuild)
        {
            ScenarioTools.Geometry.Extent localExtent;
            if (specifiedExtent != null)
            {
                localExtent = specifiedExtent;
            }
            else
            {
                localExtent = _extent;
            }
            bool forceFullExtent = fullExtent;

            _mapControl.ClearLayers();

            // Start with background image
            if (showBackgroundImage)
            {
                if (ensureBackgroundImageLayerIsBuilt(forceBackgroundRebuild))
                {
                    _mapControl.AddLayer(GlobalStaticVariables.BackgroundImageLayer);
                }
            }

            // Add all map layers.
            // MapControl.AddLayer inserts each iteam at position 0,
            // so iterate backwards through list.
            if (mapLayerList != null)
            {
                // Get layers from mapLayerList argument
                for (int i = mapLayerList.Count - 1; i >= 0; i--)
                {
                    if (mapLayerList[i].Visible)
                    {
                        _mapControl.AddLayer(mapLayerList[i].FeatureLayer);
                    }
                }
            }
            else
            {
                // Get layers from _stMapLayerList member
                for (int i = _stMapLayerList.Count - 1; i >= 0; i--)
                {
                    if (_stMapLayerList[i].Visible)
                    {
                        _mapControl.AddLayer(_stMapLayerList[i].FeatureLayer);
                    }
                }
            }

            // Draw neatline showing extent
            if (localExtent != null)
            {
                // Create LineString feature enclosing extent (neatline)
                LineString extentLineString = localExtent.GetLineString();
                AttributesTable attributesTable = new AttributesTable();
                USGS.Puma.NTS.Features.Feature extentFeature = new USGS.Puma.NTS.Features.Feature((IGeometry)extentLineString, attributesTable);

                // Create a map layer and add the extent LineString to it
                STMapLayer neatlineMapLayer = new STMapLayer(LayerGeometryType.Line);
                neatlineMapLayer.LayerName = "neatline layer";
                neatlineMapLayer.AddFeature(extentFeature);

                // Define extent line properties
                ILineSymbol symbol = ((ISingleSymbolRenderer)(neatlineMapLayer.FeatureLayer.Renderer)).Symbol as ILineSymbol;
                symbol.Color = NeatLineColor;
                symbol.Width = 3.0f;

                // Add the map layer to the map control
                _mapControl.AddLayer(neatlineMapLayer.FeatureLayer);
            }

            // Assign map extent, using desired map extent if defined
            if (_mapControl.LayerCount > 0)
            {
                if (forceFullExtent || localExtent == null)
                {
                    _mapControl.SizeToFullExtent();
                }
                else
                {
                    _mapControl.MapExtent = localExtent;
                }
            }
            BuildMapLegend(mapLayerList);
        }

        /// <summary>
        /// Builds the map legend
        /// </summary>
        public void BuildMapLegend(List<STMapLayer> mapLayerList)
        {
            clearMapLegend();
            Collection<GraphicLayer> graphicLayers = new Collection<GraphicLayer>();
            MapLegendItemCollection mapLegendItems = new MapLegendItemCollection();

            // Add legend items for all map layers
            if (mapLayerList != null)
            {
                for (int i = 0; i < mapLayerList.Count; i++)
                {
                    MapLegendItem newMapLegendItem = new MapLegendItem(mapLayerList[i].FeatureLayer);
                    newMapLegendItem.LayerVisible = mapLayerList[i].Visible;
                    mapLegendItems.Add(newMapLegendItem);
                }
            }
            else
            {
                for (int i = 0; i < _stMapLayerList.Count; i++)
                {
                    MapLegendItem newMapLegendItem = new MapLegendItem(_stMapLayerList[i].FeatureLayer);
                    newMapLegendItem.LayerVisible = _stMapLayerList[i].Visible;
                    mapLegendItems.Add(newMapLegendItem);
                }
            }

            _stMapLegend.LegendTitle = "Map layers";
            _stMapLegend.AddAndLayoutItems(mapLegendItems);
        }

        public void ClearImage()
        {
            _image = null;
        }

        public void ClearMapLayerList()
        {
            _stMapLayerList.Clear();
            ClearLayers(); // Clears the (ancestor) GraphicLayersList
        }

        /// <summary>
        /// Clears the map legend.
        /// </summary>
        /// <remarks></remarks>
        private void clearMapLegend()
        {
           _stMapLegend.Clear();
           _stMapLegend.LegendTitle = "";
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
                    USGS.Puma.NTS.Features.Feature f = featureList[0];
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
        /// Use _stMapLayerList and _viewport to draw labels on contours
        /// </summary>
        /// <param name="image"></param>
        private void drawContourLabels(Image image)
        {
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image))
            {
                // Determine if extent is available; if not, do not draw labels
                double xcMin, xcMax, ycMin, ycMax;
                if (_extent != null)
                {
                    xcMin = _extent.West;
                    xcMax = _extent.East;
                    ycMin = _extent.South;
                    ycMax = _extent.North;
                    _Viewport.SetTargetWorldExtent((IEnvelope)_extent);
                    
                }
                else
                {
                    Extent tempExtent = GetFullExtent();
                    if (tempExtent != null)
                    {
                        xcMin = tempExtent.West;
                        xcMax = tempExtent.East;
                        ycMin = tempExtent.South;
                        ycMax = tempExtent.North;
                        _Viewport.SetTargetWorldExtent((IEnvelope)tempExtent);
                    }
                    else
                    {
                        return;
                    }
                }

                // Define variables based on extent
                double extentWidth = Math.Abs(xcMax - xcMin);
                double extentHeight = Math.Abs(ycMax - ycMin);
                double maxExtent = Math.Max(extentWidth, extentHeight);
                double geoBuffer = maxExtent * 0.02;
                double geoLabelBuffer = maxExtent * 0.04;
                Extent bufferedExtent = new Extent(xcMin + geoBuffer, ycMin + geoBuffer, xcMax - geoBuffer, ycMax - geoBuffer);

                int iContour, iSeg, n;
                int imageWidth = image.Width;
                int imageHeight = image.Height;
                int numCoordinates;
                float geoX0;
                float geoY0;
                float geoX1;
                float geoY1;
                float geoXMid;
                float geoYMid;
                float roundValue;
                GeoAPI.Geometries.ICoordinate[] coordinates;
                double geoRectX0;
                double geoRectY0;
                double geoRectX1;
                double geoRectY1;
                double minGeoDistance;
                double geoDistance;
                double geoX; 
                double geoY;
                double geoLabelWidth;
                double labelPointGeoDistance;
                double dblValue;
                bool isVisible;
                string label;
                STMapLayer stMapLayer;
                PointF labelPointF;

                // Define font
                float fontEmSize = 12.0f;
                Font labelFont = new Font(FontFamily.GenericSansSerif, fontEmSize, FontStyle.Regular);

                // Set up mask-rectangle properties
                Pen maskPen = new Pen(Color.Black);
                Brush maskBrush = Brushes.White;
                maskPen.Width = 2.0f;
                double rectangleRatio = 0.014 * (Convert.ToDouble(fontEmSize)/8.25);
                double gRectangleHeight = rectangleRatio * maxExtent;

                // Minor adjustment to y position to center label text in mask area,
                // based on font that gets used, which can depend on user settings.
                double yAdjust = 0.029 * labelFont.Height - 0.1;

                // Assign minimum contour line length to label
                double minLength = Math.Max(extentWidth, extentHeight) * 0.05;

                // Iterate through map layers
                for (int iLayer = 0; iLayer < _stMapLayerList.Count; iLayer++)
                {
                    stMapLayer = _stMapLayerList[iLayer];
                    if (stMapLayer.Visible)
                    {
                        // Make a list for the image points at which labels will be drawn
                        List<Point2D> geoLabelPoints = new List<Point2D>();
                        List<string> labelStrings = new List<string>();
                        int conIntervalDigits = 0;
                        string conIntervalString = "";
                        string formatString = "";
                        formatString = generateFormatString(stMapLayer.DataSeries.ContourInterval);
                        conIntervalString = stMapLayer.DataSeries.ContourInterval.ToString(formatString);
                        if (conIntervalString.Contains("."))
                        {
                            int conIntervalLength = conIntervalString.Length;
                            int pos = conIntervalString.IndexOf(".");
                            conIntervalDigits = conIntervalString.Length - pos - 1;
                        }

                        // Get feature layer in current map layer
                        FeatureLayer featureLayer = stMapLayer.FeatureLayer;

                        // Iterate through all contours (features) in feature layer
                        for (iContour = 0; iContour < featureLayer.FeatureCount; iContour++)
                        {
                            USGS.Puma.NTS.Features.Feature feature = featureLayer.GetFeature(iContour);
                            if (feature.Geometry is MultiLineString)
                            {
                                MultiLineString multiLineString = (MultiLineString)feature.Geometry;
                                if (multiLineString.Length > minLength)
                                {
                                    coordinates = multiLineString.Coordinates;
                                    numCoordinates = coordinates.GetLength(0);
                                    if (numCoordinates > 1)
                                    {
                                        isVisible = false;
                                        Point2D geoLabelPoint = new Point2D(-1000, -1000);
                                        labelPointGeoDistance = double.MinValue;

                                        // Iterate through all segments of current contour
                                        for (iSeg = 1; iSeg < numCoordinates; iSeg++)
                                        {
                                            // Store geographic cooordinates of the segment ends
                                            geoX0 = (float)coordinates[iSeg - 1].X;
                                            geoY0 = (float)coordinates[iSeg - 1].Y;
                                            geoX1 = (float)coordinates[iSeg].X;
                                            geoY1 = (float)coordinates[iSeg].Y;

                                            // Find the midpoint (in geographic coordinates)
                                            geoXMid = (geoX0 + geoX1) / 2.0f;
                                            geoYMid = (geoY0 + geoY1) / 2.0f;

                                            // Determine if a contour label will be placed on this segment.
                                            // If the midpoint is contained in the buffered extent, 
                                            // it is a candidate for the label point
                                            if (bufferedExtent.Contains(geoXMid, geoYMid))
                                            {
                                                isVisible = true;

                                                // If the point list is empty, this midpoint is a potential label point
                                                Point2D geoMidpoint = new Point2D(geoXMid, geoYMid);
                                                if (geoLabelPoints.Count == 0)
                                                {
                                                    labelPointGeoDistance = double.MaxValue;
                                                    geoLabelPoint = geoMidpoint;
                                                    break;
                                                }

                                                // Otherwise, find the minimum distance to all other points in the list.
                                                else
                                                {
                                                    minGeoDistance = geoMidpoint.distance(geoLabelPoints[0]);
                                                    for (n = 1; n < geoLabelPoints.Count && minGeoDistance > labelPointGeoDistance; n++)
                                                    {
                                                        geoDistance = geoMidpoint.distance(geoLabelPoints[n]);
                                                        if (geoDistance < minGeoDistance)
                                                        {
                                                            minGeoDistance = geoDistance;
                                                        }
                                                    }
                                                    if (minGeoDistance > labelPointGeoDistance)
                                                    {
                                                        geoLabelPoint = geoMidpoint;
                                                        labelPointGeoDistance = minGeoDistance;
                                                    }
                                                }
                                            }
                                        }
                                        // If the label would be visible, assign label and label point for current contour
                                        if (isVisible && labelPointGeoDistance > geoLabelBuffer)
                                        {
                                            object value = ((AttributesTable)feature.Attributes)["value"];
                                            if (value is float)
                                            {
                                                // Limit number of decimal digits in contour value, then create and save label
                                                dblValue = Convert.ToDouble(value);
                                                roundValue = Convert.ToSingle(Math.Round(dblValue, conIntervalDigits));
                                                label = roundValue.ToString();
                                                labelStrings.Add(label);
                                                geoLabelPoints.Add(geoLabelPoint);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        // Draw contour labels for current feature layer
                        Brush brush = new SolidBrush(stMapLayer.ContourColor);
                        for (iContour = 0; iContour < geoLabelPoints.Count; iContour++)
                        {
                            // Estimate label width, in geographic coordinates
                            float fScaleFactor = 1.0f;
                            if (_scaleFactor != 0.0)
                            {
                                fScaleFactor = Convert.ToSingle(_scaleFactor);
                            }
                            geoLabelWidth = Convert.ToDouble(g.MeasureString(labelStrings[iContour], labelFont).Width) / fScaleFactor;

                            // Draw mask rectangle
                            USGS.Puma.NTS.Geometries.Point p = new USGS.Puma.NTS.Geometries.Point(geoLabelPoints[iContour].X, geoLabelPoints[iContour].Y);
                            geoRectX0 = geoLabelPoints[iContour].X - geoLabelWidth / 2.0;
                            geoRectY0 = geoLabelPoints[iContour].Y - gRectangleHeight / 2.0;
                            geoRectX1 = geoLabelPoints[iContour].X + geoLabelWidth / 2.0;
                            geoRectY1 = geoLabelPoints[iContour].Y + gRectangleHeight / 2.0;
                            Coordinate[] geoCoords = new Coordinate[5];
                            geoCoords[0] = new Coordinate(geoRectX0, geoRectY0);
                            geoCoords[1] = new Coordinate(geoRectX0, geoRectY1);
                            geoCoords[2] = new Coordinate(geoRectX1, geoRectY1);
                            geoCoords[3] = new Coordinate(geoRectX1, geoRectY0);
                            geoCoords[4] = new Coordinate(geoRectX0, geoRectY0);
                            LinearRing geoRing = new LinearRing(geoCoords);
                            Polygon geoMaskPolygon = new Polygon(geoRing);
                            _Viewport.DrawPolygon(geoMaskPolygon, g, null, maskBrush);

                            // Draw label
                            geoX = geoLabelPoints[iContour].X - geoLabelWidth / 2.2;
                            geoY = geoLabelPoints[iContour].Y + gRectangleHeight * yAdjust;
                            labelPointF = _Viewport.ToDevicePoint(geoX, geoY);
                            g.DrawString(labelStrings[iContour], labelFont, brush, labelPointF);
                        }
                    }
                }
            }
            return;
        }

        private string generateFormatString(float contourInterval)
        {
            float absConIntFloat = Math.Abs(contourInterval);
            decimal absConIntDec = Convert.ToDecimal(absConIntFloat);
            decimal greatestIntDec = Math.Truncate(absConIntDec);
            decimal decimalPart = absConIntDec - greatestIntDec;
            const decimal decimalZero = 0.0M;
            if (decimalPart == decimalZero)
            {
                return "F0";
            }
            if (decimalPart < 0.00001M)
            {
                return "F6";
            }
            string decimalPartString = decimalPart.ToString("F5");
            string last4Digits = decimalPartString.Substring(3);
            if (last4Digits == "0000")
            {
                return "F1";
            }
            string last3Digits = decimalPartString.Substring(4);
            if (last3Digits == "000")
            {
                return "F2";
            }
            string last2Digits = decimalPartString.Substring(5);
            if (last2Digits == "00")
            {
                return "F3";
            }
            string lastDigit = decimalPartString.Substring(6);
            if (lastDigit == "0")
            {
                return "F4";
            }
            return "F5";
        }

        public Extent GetFullExtent()
        {
            Extent tempExtent = null;
            for (int i = 0; i < _stMapLayerList.Count; i++)
            {
                if (_stMapLayerList[i].Extent != null)
                {
                    if (tempExtent == null)
                    {
                        tempExtent = GenerateAutomaticExtent();
                    }
                    else
                    {
                        tempExtent.ExpandToInclude(_stMapLayerList[i].Extent);
                    }
                }
            }
            return tempExtent;
        }

        private FeatureLayer generateAndBuildContourLayer(Array2d<float> buffer, CellCenteredArealGrid modelGrid, 
                                                          ContourEngineData contourEngineData, string name, ref float contourInterval)
        {
            ContourLineList contourList = generateContours(buffer, modelGrid, contourEngineData, ref contourInterval);
            if (contourList != null)
            {
                return buildContourLayer(contourList, contourEngineData, name);
            }
            else
            {
                return null;
            }
        }

        private FeatureLayer generateAndBuildColorFillLayer(Array2d<float> buffer, CellCenteredArealGrid modelGrid, ColorRamp colorRamp, string name)
        {
            List<USGS.Puma.NTS.Features.Feature> featureList = generateValuePolygonFeatureList(buffer, modelGrid);
            return buildColorFillLayer(featureList, colorRamp, name);
        }

        private USGS.Puma.NTS.Features.Feature generateValuePolygon(GridCell cell, CellCenteredArealGrid modelGrid, float value)
        {
            AttributesTable table = new AttributesTable();
            table.AddAttribute("value", value);
            if (modelGrid != null)
            {
                Polygon polygon = (Polygon)modelGrid.GetPolygon(cell);
                USGS.Puma.NTS.Features.Feature feature = new USGS.Puma.NTS.Features.Feature(polygon, table);
                return feature;
            }
            return null;
        }

        private List<USGS.Puma.NTS.Features.Feature> generateValuePolygonFeatureList(Array2d<float> buffer, CellCenteredArealGrid modelGrid)
        {
            List<USGS.Puma.NTS.Features.Feature> featureList = new List<USGS.Puma.NTS.Features.Feature>();
            for (int j = 0; j < buffer.RowCount; j++)
            {
                for (int i = 0; i < buffer.ColumnCount; i++)
                {
                    if (!Single.IsNaN(buffer[j+1,i+1]))
                    {
                        GridCell cell = new GridCell(j+1, i+1);
                        USGS.Puma.NTS.Features.Feature feature = generateValuePolygon(cell, modelGrid, buffer[j + 1, i + 1]);
                        featureList.Add(feature);
                    }
                }
            }
            return featureList;
        }

        /// <summary>
        /// Generates contour features for the specified layer array buffer.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="modelGrid"></param>
        /// <returns></returns>
        private ContourLineList generateContours(Array2d<float> buffer, CellCenteredArealGrid modelGrid, ContourEngineData contourEngineData, ref float contourInterval)
        {
            if (buffer == null)
                throw new ArgumentNullException();
            if ((buffer.RowCount != modelGrid.RowCount) || (buffer.ColumnCount != modelGrid.ColumnCount))
                throw new ArgumentException("Array does not match model grid dimensions.");

            ContourEngine ce = new ContourEngine(modelGrid);

            ce.UseDefaultNoDataRange = false;
            foreach (float excludedValue in contourEngineData.ExcludedValues)
            {
                ce.ExcludedValues.Add(excludedValue);
            }
            ce.LayerArray = buffer;
            float refContour = contourEngineData.ReferenceContour;

            contourInterval = 0.0f;

            switch (contourEngineData.ContourIntervalOption)
            {
                case ContourIntervalOption.AutomaticConstantInterval:
                    contourInterval = ce.ComputeContourInterval();
                    ce.ContourLevels = ce.GenerateConstantIntervals(contourInterval, refContour);
                    break;
                case ContourIntervalOption.SpecifiedConstantInterval:
                    contourInterval = contourEngineData.ConstantContourInterval;
                    ce.ContourLevels = ce.GenerateConstantIntervals(contourInterval, refContour);
                    break;
                case ContourIntervalOption.SpecifiedContourLevels:
                    ce.ContourLevels = contourEngineData.ContourLevels;
                    break;
                default:
                    break;
            }

            try
            {
                ContourLineList conLineList = ce.CreateContours();
                return conLineList;
            }
            catch (Exception e)
            {
                string errmsg = e.Message;
                MessageBox.Show(errmsg);
                return null;
            }

        }

        public STMapLayer GetSTMapLayer(int index)
        {
            return _stMapLayerList[index];
        }
        public STMapLayer GetSTMapLayer(string mapLayerName)
        {
            for (int i = 0; i < _stMapLayerList.Count; i++)
            {
                if (_stMapLayerList[i].LayerName == mapLayerName)
                {
                    return _stMapLayerList[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Use DataSeries included in each STMapLayer to generate remaining
        /// members of the STMapLayer
        /// </summary>
        private void populateLayers()
        {
            bool OK = true;
            string msg = "";
            float conInterval;
            ClearLayers();
            DataSeries ds;
            ScenarioTools.Data_Providers.DataSeriesTypeEnum dsType;
            CellCenteredArealGrid modelGrid = ScenarioTools.Spatial.StaticObjects.Grid;
            if (modelGrid != null)
            {
                for (int n = 0; n < _stMapLayerList.Count; n++)
                {
                    int dsStatus;
                    ds = _stMapLayerList[n].DataSeries;
                    _stMapLayerList[n].LayerName = ds.Name;
                    _stMapLayerList[n].ConvertFlowToFlux = ds.ConvertFlowToFlux;
                    _stMapLayerList[n].Visible = ds.Visible;
                    if (ds.Visible)
                    {
                        dsType = ds.DataSeriesType;
                        GeoMap geoMap = (GeoMap)ds.GetData(out dsStatus);
                        if (geoMap != null)
                        {
                            float[,] vals = geoMap.GetValueArray();
                            int nrow = geoMap.NRows;
                            int ncol = geoMap.NCols;
                            Array2d<float> array2dVals;
                            // Ensure that array/grid dimensions match
                            if (modelGrid.RowCount != nrow || modelGrid.ColumnCount != ncol)
                            {
                                OK = false;
                                msg = "Error: Grid dimension mismatch";
                            }
                            if (OK)
                            {
                                // Convert 2D (row-major) array (as stored in GeoMap and 
                                // provided by GeoMap.GetValueArray) to a 1D array
                                // as it would represent a 2D row-major array (as required 
                                // by ContourEngine)
                                float[] vals1d = new float[nrow * ncol];
                                int k = 0;
                                if (GlobalStaticVariables.BlankingMode == MapEnums.BlankingMode.None)
                                {
                                    // For efficiency, if Blanking is None, just assign vals1d
                                    for (int i = 0; i < nrow; i++)
                                    {
                                        for (int j = 0; j < ncol; j++)
                                        {
                                            vals1d[k] = vals[i, j];
                                            k++;
                                        }
                                    }
                                }
                                else
                                {
                                    // Blanking is being used, so need to test each element
                                    for (int i = 0; i < nrow; i++)
                                    {
                                        for (int j = 0; j < ncol; j++)
                                        {
                                            if (GlobalStaticVariables.Blanking[i, j])
                                            {
                                                vals1d[k] = float.NaN;
                                            }
                                            else
                                            {
                                                vals1d[k] = vals[i, j];
                                            }
                                            k++;
                                        }
                                    }
                                }

                                array2dVals = new Array2d<float>(nrow, ncol);
                                array2dVals.SetValues(vals1d);
                                switch (dsType)
                                {
                                    case (ScenarioTools.Data_Providers.DataSeriesTypeEnum.ContourMapSeries):
                                        // Ned TODO: Optionally use user-specified contour interval
                                        _stMapLayerList[n].ContourColor = ds.LineSeriesColor;
                                        switch (ds.ContourSpecificationType)
                                        {
                                            case (DataSeries.ContourSpecificationMethod.Automatic):
                                                {
                                                    _stMapLayerList[n].ContourEngineData.ContourIntervalOption = ContourIntervalOption.AutomaticConstantInterval;
                                                    break;
                                                }
                                            case (DataSeries.ContourSpecificationMethod.EqualInterval):
                                                {
                                                    _stMapLayerList[n].ContourEngineData.ContourIntervalOption = ContourIntervalOption.SpecifiedConstantInterval;
                                                    _stMapLayerList[n].ContourEngineData.ConstantContourInterval = Convert.ToSingle(ds.ContourInterval);
                                                    break;
                                                }
                                            case (DataSeries.ContourSpecificationMethod.ValueList):
                                                {
                                                    _stMapLayerList[n].ContourEngineData.ContourIntervalOption = ContourIntervalOption.SpecifiedContourLevels;
                                                    _stMapLayerList[n].ContourEngineData.ContourLevels.Clear();
                                                    for (int i = 0; i < ds.ContourValues.Length; i++)
                                                    {
                                                        _stMapLayerList[n].ContourEngineData.ContourLevels.Add(Convert.ToSingle(ds.ContourValues[i]));
                                                    }
                                                    break;
                                                }
                                        }
                                        conInterval = 0.0f;
                                        _stMapLayerList[n].FeatureLayer = generateAndBuildContourLayer(array2dVals, modelGrid, _stMapLayerList[n].ContourEngineData, ds.Name, ref conInterval);
                                        ds.ContourInterval = conInterval;
                                        _stMapLayerList[n].Visible = ds.Visible;
                                        _stMapLayerList[n].ConvertFlowToFlux = ds.ConvertFlowToFlux;
                                        break;
                                    case ScenarioTools.Data_Providers.DataSeriesTypeEnum.ColorFillMapSeries:
                                        _stMapLayerList[n].FeatureLayer = generateAndBuildColorFillLayer(array2dVals, modelGrid, _stMapLayerList[n].ColorRamp, ds.Name);
                                        _stMapLayerList[n].Visible = ds.Visible;
                                        _stMapLayerList[n].ConvertFlowToFlux = ds.ConvertFlowToFlux;
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        msg = "Warning: Data series does not return GeoMap";
                    }
                }
            }
            else
            {
                OK = false;
                if (_stMapLayerList.Count == 0)
                {
                    msg = "Warning: ";
                }
                else
                {
                    msg = "Error: ";
                }
                msg = msg + "Model grid has not been defined.  Use menu item: 'Project|Settings' to define model grid.";
            }
            if (!OK && !_displayingMessage)
            {
                _displayingMessage = true;
                DialogResult dr = MessageBox.Show(msg);
                _displayingMessage = false;
            }
            BuildMapLayers(false, null, null, _showBackgroundImage, false);
        }

        private Image renderLegend(int width, int height)
        {
            // Define positions and sizes
            float textSpacing = 10.0f; // Horizontal spacing between symbol and label
            float symbolWidth = 30.0f; // Width of the symbol in legend
            float xLeft = 20;          // Left edge of symbols
            int leading = 2;           // Vertical space between lines of text
            int itemSpace = 10;        // Vertical space between legend items
            int symbolHeight;          // Height of the symbol in legend
            int itemHeight;            // Overall height of a legend item
            int numLines;              // Number of lines required for label
            int textHeight;            // Includes all lines of text plus leading
            float x;
            float x0;
            float y = 0.0f;
            float y0;
            float labelX = 0.0f;
            float labelY = 0.0f;
            float xText;

            string text = "";
            SolidBrush brush = new SolidBrush(Color.Black);

            // Define fonts
            float fontHeightLarge = 18.0f;
            float fontHeightMedium = 16.0f;
            float fontHeightSmall = 14.0f;
            Font fontLarge = new Font(FontFamily.GenericSansSerif, fontHeightLarge, FontStyle.Regular);
            Font fontMedium = new Font(FontFamily.GenericSansSerif, fontHeightMedium, FontStyle.Regular);
            Font fontSmall = new Font(FontFamily.GenericSansSerif, fontHeightSmall, FontStyle.Regular);

            Bitmap legendBitmap = new Bitmap(width, height);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(legendBitmap))
            {
                g.Clear(Color.White);
                // Draw "EXPLANATION" at top
                string explanationText = "EXPLANATION";
                SizeF explanationSize = new SizeF();
                explanationSize = g.MeasureString(explanationText, fontLarge);
                x = (Convert.ToSingle(width) - explanationSize.Width) / 2.0f;
                PointF pt0 = new PointF(x, y);
                g.DrawString(explanationText, fontLarge, brush, pt0);

                // Iterate through legend items
                y = y + fontLarge.Height + itemSpace + itemSpace / 2;
                for (int i = 0; i < _stMapLegend.Items.Count; i++)
                {
                    MapLegendItem item = _stMapLegend.Items[i];
                    GraphicLayer graphicLayer = item.MapLayer;
                    if (graphicLayer != null)
                    {
                        DataSeries ds = null;
                        bool localVisible = graphicLayer.Visible;
                        try
                        {
                            // Visibility of data series takes priority (?)
                            ds = DataSeriesList[i];
                            localVisible = ds.Visible;
                        }
                        catch
                        {
                        }
                        if (graphicLayer.LayerType == GraphicLayerType.VectorLayer && localVisible)
                        {
                            if (graphicLayer is FeatureLayer)
                            {
                                x0 = xLeft;
                                y0 = y;

                                // Draw symbol for current legend item
                                if (item.MapLayer is FeatureLayer)
                                {
                                    FeatureLayer featureLayer = (FeatureLayer)item.MapLayer;
                                    if (featureLayer.Renderer is ColorRampRenderer)
                                    {
                                        ColorRampRenderer rampRenderer = (ColorRampRenderer)featureLayer.Renderer;
                                        Color[] colors = new Color[] { ds.RampColor0, ds.RampColor1 };
                                        rampRenderer.ColorRamp.Colors = colors;
                                    }
                                }
                                float labelYTop = 0.0f;
                                float labelYBottom = 0.0f;
                                float labelYCenter = 0.0f;
                                symbolHeight = GraphicsHelpers.DrawFeatureLayerSymbol(item, legendBitmap, g, x0, y0,
                                                                                      symbolWidth, fontSmall, ref labelX,
                                                                                      ref labelY, ref labelYCenter,
                                                                                      ref labelYTop, ref labelYBottom);

                                // Draw text identifying current legend item
                                xText = x0 + symbolWidth + textSpacing;
                                text = graphicLayer.LayerName;
                                // First, assume text will fit on one line
                                numLines = 1;

                                // Parse item description into lines that fit available width
                                int availableWidth = width - Convert.ToInt32(Math.Floor(labelX));
                                string[] lines = GraphicsHelpers.GetLines(text, availableWidth, fontMedium);
                                numLines = lines.Length;
                                if (numLines == 1)
                                {
                                    labelY = labelYCenter - fontMedium.Height / 2.0f;
                                    // Ned TODO: Add support for multiple lines of text
                                    g.DrawString(text, fontMedium, brush, labelX, labelY);
                                    textHeight = numLines * fontMedium.Height + (numLines - 1) * leading;
                                    itemHeight = Math.Max(symbolHeight, textHeight);
                                }
                                else
                                {
                                    float availableHeight = labelYBottom - labelYTop;
                                    labelY = labelYCenter - fontMedium.Height - leading * 0.5f;
                                    g.DrawString(lines[0], fontMedium, brush, labelX, labelY);
                                    labelY = labelY + fontMedium.Height + leading;
                                    g.DrawString(lines[1], fontMedium, brush, labelX, labelY);
                                    textHeight = numLines * fontMedium.Height + (numLines - 1) * leading;
                                    itemHeight = Math.Max(symbolHeight, textHeight);
                                }
                                y = y + Convert.ToSingle(itemHeight) + itemSpace;
                            }
                        }
                    }
                }

                // Write extent name
                if (ExtentName != "")
                {
                    y = y + fontHeightMedium * 2.0f;
                    text = "Extent: " + ExtentName;
                    g.DrawString(text, fontMedium, brush, xLeft, y);
                }
            }

            return (Image)legendBitmap;
        }

        new public ICoordinate ToMapPoint(int x, int y)
        {
            return ToMapPoint((float)x, (float)y);
        }
        public ICoordinate ToMapPoint(float x, float y)
        {
            double xx = Convert.ToDouble(x);
            double yy = Convert.ToDouble(y);

            // Determine panel center and offset of pointer location from panel center
            double panelWidth = Convert.ToDouble(MapControl.Width - 1);
            double panelCenterX = (panelWidth / 2.0);
            double panelHeight = Convert.ToDouble(MapControl.Height - 1);
            double panelCenterY = (panelHeight / 2.0);
            double panelOffsetX = xx - panelCenterX;
            double panelOffsetY = yy - panelCenterY;

            // Determine extent center, which should coincide with panel center
            double extentCenterX = (_extent.MinX + _extent.MaxX) / 2.0;
            double extentCenterY = (_extent.MinY + _extent.MaxY) / 2.0;

            // Calculate scale factor
            double scaleFactor;
            double imageWidth = Convert.ToDouble(MapControl.Width - 1);
            double imageHeight = Convert.ToDouble(MapControl.Height - 1);
            double extentXMin = _extent.MinX;
            double extentXMax = _extent.MaxX;
            double extentYMin = _extent.MinY;
            double extentYMax = _extent.MaxY;
            double extentWidth = Math.Abs(extentXMax - extentXMin);
            double extentHeight = Math.Abs(extentYMax - extentYMin);
            double imageAspectRatio = imageWidth / imageHeight;
            double extentAspectRatio = extentWidth / extentHeight;
            if (imageAspectRatio > extentAspectRatio)
            {
                scaleFactor = extentHeight / Convert.ToDouble(imageHeight);
            }
            else
            {
                scaleFactor = extentWidth / Convert.ToDouble(imageWidth);
            }

            ICoordinate pt = new Coordinate();
            pt.X = extentCenterX + panelOffsetX * scaleFactor;
            pt.Y = extentCenterY - panelOffsetY * scaleFactor;

            return pt;
        }
    }
}
