// supports raster formats
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using OSGeo.GDAL;
using ScenarioTools.Geometry;
using ScenarioTools.Graphics;

namespace ScenarioTools.Spatial
{
    public class GeoImage
    {
        #region Fields
        private Image _image;
        private Image _originalImage;
        private Extent _extent;
        private int _brightness;
        private ScenarioTools.Spatial.SpatialReference _spatialReference;
        private string _filepath;
        #endregion Fields

        #region Constructors

        public GeoImage()
        {
            this._image = null;
            this._originalImage = null;
            this._extent = null;
            _brightness = 0;
            _spatialReference = null;
            _filepath = "";
        }

        public GeoImage(Image image, Extent extent)
            : this()
        {
            // Store references to the image and the extent.
            _brightness = 0;
            this._originalImage = image;
            this._image = new Bitmap(image);
            if (extent == null)
            {
                this._extent = new Extent(0.0, 0.0, 0.0, 0.0);
            }
            else
            {
                this._extent = extent;
            }
        }

        public GeoImage(Image image, USGS.Puma.NTS.Geometries.Envelope envelope)
            : this()
        {
            _brightness = 0;
            this._originalImage = image;
            this._image = new Bitmap(image);
            DefineExtent(envelope);
        }

        public GeoImage(string geoTiffPath)
            : this()
        {
            _filepath = geoTiffPath;
            try
            {
                Gdal.AllRegister();
                IntPtr ptr = new IntPtr();
                bool memoryOwn = true;
                OSGeo.GDAL.Dataset dataset = new Dataset(ptr, memoryOwn, this);
                Access access = Access.GA_ReadOnly;

                // Ned TODO: Check capability of Gdal.Open.  I think it supports many file formats,
                // so argument name could be general.  The file open dialog should have its
                // filter defined to show all supported file types.
                dataset = Gdal.Open(geoTiffPath, access);
                string geoTiffWkt = dataset.GetProjection();
                OSGeo.OSR.SpatialReference spRef = new OSGeo.OSR.SpatialReference(geoTiffWkt);
                spRef.MorphToESRI();
                string esriWkt = "";
                int m = spRef.ExportToWkt(out esriWkt);
                this._spatialReference = new ScenarioTools.Spatial.SpatialReference(esriWkt, this);
                GlobalSpatialStaticVariables.SpatialReference = _spatialReference;
                if (GlobalStaticVariables.GlobalBackgroundWorker != null)
                {
                    GlobalStaticVariables.GlobalBackgroundWorker.Dispose();
                    GlobalStaticVariables.GlobalBackgroundWorker = null;
                }
                Bitmap bitmap = ScenarioTools.GdalHelper.ToBitmap(dataset, 0);
                if (bitmap == null)
                {
                    while (GlobalStaticVariables.GlobalBitmap == null)
                    {
                        Thread.Sleep(1);

                    }
                    bitmap = GlobalStaticVariables.GlobalBitmap;
                }
                if (bitmap != null)
                {
                    this._originalImage = (Image)bitmap;
                    this._image = new Bitmap(bitmap);

                    // Find geographic extent of image and store it
                    double xMax = Convert.ToDouble(dataset.RasterXSize);
                    double yMax = Convert.ToDouble(dataset.RasterYSize);
                    double geoXMin = Double.NaN, geoXMax = Double.NaN, geoYMin = Double.NaN, geoYMax = Double.NaN;

                    // Get coordinates of lower left corner of image
                    GdalHelper.GDALInfoGetPosition(dataset, 0.0, yMax, ref geoXMin, ref geoYMin);

                    // Get coordinates of upper right corner of image
                    GdalHelper.GDALInfoGetPosition(dataset, xMax, 0.0, ref geoXMax, ref geoYMax);

                    // Define geographic extent of image
                    this._extent = new Extent(geoXMin, geoYMin, geoXMax, geoYMax);
                    //this.DefineEnvelope();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion Constructors

        #region Properties
        public int Brightness
        {
            get
            {
                return _brightness;
            }
            set
            {
                bool needImageRedo = false;
                if (value < -255)
                {
                    needImageRedo = _brightness != -255;
                    _brightness = -255;
                }
                else if (value > 255)
                {
                    needImageRedo = _brightness != 255;
                    _brightness = 255;
                }
                else
                {
                    needImageRedo = _brightness != value;
                    _brightness = value;
                }
                if (needImageRedo)
                {
                    if (_originalImage != null)
                    {
                        _image = GraphicsHelpers.ApplyBrightness((Bitmap)_originalImage, _brightness);
                    }
                }
            }
        }
        #endregion Properties

        // Ned TODO: GeoImage.FromFile is not called from anywhere...delete it?
        //public static GeoImage FromFile(string imagePath, string extentPath)
        //{
        //    Image image = Image.FromFile(imagePath);
        //    Extent extent = Extent.FromFile(extentPath);

        //    return new GeoImage(image, extent);
        //}

        #region Methods

        public void DrawTo(GeoImage targetImage)
        {
            // Find the relative coordinates on the target image.
            float xLeftTarget = (float)((this._extent.West - targetImage._extent.West) / targetImage._extent.Width * targetImage._image.Width);
            float xRightTarget = (float)((this._extent.East - targetImage._extent.West) / targetImage._extent.Width * targetImage._image.Width);
            float yBottomTarget = (float)((targetImage._extent.North - this._extent.South) / targetImage._extent.Height * targetImage._image.Height);
            float yTopTarget = (float)((targetImage._extent.North - this._extent.North) / targetImage._extent.Height * targetImage._image.Height);

            // If the coordinates need to be flipped, flip them.
            if (xLeftTarget > xRightTarget) 
            {
                float temp = xLeftTarget;
                xLeftTarget = xRightTarget;
                xRightTarget = temp;
            }
            if (yTopTarget > yBottomTarget) 
            {
                float temp = yTopTarget;
                yTopTarget = yBottomTarget;
                yBottomTarget = temp;
            }

            // Calculate the width and height.
            float width = xRightTarget - xLeftTarget;
            float height = yBottomTarget - yTopTarget;

            // Draw the image to the target.
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(targetImage._image);
            try
            {
                if (this._image != null)
                {
                    g.DrawImage(this._image, xLeftTarget, yTopTarget, width, height);
                }
            }
            finally
            {
                g.Dispose();
            }
        }

        public Image GetImage()
        {
            return GetImage(Brightness);
        }

        public Image GetImage(int brightness)
        {
            if (brightness == 0)
            {
                return _originalImage;
            }
            else
            {
                if (brightness == Brightness && _image != null)
                {
                    // No need to reapply brightness
                    return _image;
                }
                if (_originalImage != null)
                {
                    Bitmap newBitmap = GraphicsHelpers.ApplyBrightness((Bitmap)_originalImage, brightness);
                    return newBitmap;
                }
                else
                {
                    return null;
                }
            }
        }

        private void DefineExtent(USGS.Puma.NTS.Geometries.Envelope envelope)
        {
            double west = envelope.MaxX;
            double east = envelope.MinX;
            double south = envelope.MinY;
            double north = envelope.MaxY;
            _extent = new Extent(west, south, east, north);
        }

        public GeoAPI.Geometries.IEnvelope Envelope
        {
            get
            {
                return (GeoAPI.Geometries.IEnvelope)_extent;
            }
        }

        public override string ToString()
        {
            return "Georeferenced image: " + _filepath;
        }

        #endregion Methods
    }
}
