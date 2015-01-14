using System;
using System.Drawing;
using System.IO;

using GeoAPI.Geometries;
using ScenarioTools.Spatial;
using USGS.Puma.UI.MapViewer;

namespace ScenarioTools.Reporting
{
    /// <summary>
    /// A GraphicLayer containing a georeferenced image
    /// </summary>
    public class ImageLayer : GraphicLayer, IImageLayer
    {
        #region Fields

        protected string _geoImagePath;
        protected GeoImage _geoImage;
        protected GeoAPI.Geometries.IEnvelope _envelope;
        protected int _brightness;
        #endregion Fields

        #region Properties

        public int Brightness
        {
            get
            {
                if (_geoImage != null)
                {
                    return _geoImage.Brightness;
                }
                else
                {
                    return _brightness;
                }
            }
            set
            {
                if (_geoImage != null)
                {
                    _geoImage.Brightness = value;
                }
                _brightness = value;
            }
        }

        /// <summary>
        /// Identifies an ImageLayer as having GraphicLayerType ImageLayer
        /// </summary>
        public override GraphicLayerType LayerType
        {
            get { return GraphicLayerType.ImageLayer; }
        }

        /// <summary>
        /// Gets the Envelope for the image
        /// </summary>
        public override IEnvelope Extent
        {
            get
            {
                if (_envelope == null)
                    Update();
                return _envelope;
            }
        }

        /// <summary>
        /// Gets or sets the SRID (Spatial Reference System Identifier )
        /// </summary>
        public override int SRID
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// A GraphicLayer suitable for displaying a georeferenced image
        /// </summary>
        public ImageLayer() 
            : base()
        {
            Visible = true;
            Brightness = 0;
            MinVisible = 0.0;
            MaxVisible = double.MaxValue;
            LayerName = "";
            _geoImage = null;
            _geoImagePath = "";
            _envelope = new USGS.Puma.NTS.Geometries.Envelope(0.0,0.0,0.0,0.0);
        }

        /// <summary>
        /// Constructs an ImageLayer from a file containing a georeferenced image
        /// </summary>
        /// <param name="geoTiffPath"></param>
        public ImageLayer(string geoTiffPath)
            : this()
        {
            try
            {
                if (File.Exists(geoTiffPath))
                {
                    // Ned TODO: use background worker; see GdalHelper.ToBitmap and ScenarioManager.MainForm ~line 1700.
                    _geoImage = new GeoImage(geoTiffPath);
                    _geoImagePath = geoTiffPath;
                    _envelope = _geoImage.Envelope;
                }
            }
            catch
            {
            }
        }

        #endregion Constructors

        #region IImageLayer Methods

        public void RenderImage(System.Drawing.Graphics g, Viewport vp)
        {
            // Draw image to Graphics g
            if (this._envelope != null)
            {
                RectangleF rectF = vp.ToDeviceRectangle(this._envelope);
                g.DrawImage(this._geoImage.GetImage(), rectF);
            }
        }

        #endregion IImageLayer Methods

        #region Methods

        /// <summary>
        /// Update the Envelope for the georeferenced image
        /// </summary>
        public void Update()
        {
            try
            {
                if (_geoImage == null && File.Exists(_geoImagePath))
                {
                    _geoImage = new GeoImage(_geoImagePath);
                }
                if (_geoImage != null)
                {
                    _envelope = _geoImage.Envelope;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Replace existing 
        /// </summary>
        /// <param name="geoTiffPath"></param>
        public void Replace(string geoTiffPath)
        {
            _geoImage = null;
            _geoImagePath = geoTiffPath;
            Update();
        }

        public GeoImage GetGeoImage()
        {
            if (_geoImage != null)
            {
                return _geoImage;
            }
            else
            {
                return null;
            }
        }

        public Image GetImage()
        {
            if (_geoImage != null)
            {
                return _geoImage.GetImage(Brightness);
            }
            else
            {
                return null;
            }
        }

        public Image GetImage(int brightness)
        {
            if (_geoImage != null)
            {
                return _geoImage.GetImage(brightness);
            }
            else
            {
                return null;
            }
        }

        #endregion Methods
    }
}
