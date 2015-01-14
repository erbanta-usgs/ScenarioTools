using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

using OSGeo.GDAL; // supports raster formats

namespace ScenarioTools.Spatial
{
    public class ImageInfo
    {
        #region Fields
        private string _imagefileAbsolutePath;
        private SpatialReference _spatialReference;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Assigning ImagefileAbsolutePath generates a SpatialReference from the geoTiff file
        /// </summary>
        public string ImagefileAbsolutePath 
        {
            get
            {
                return _imagefileAbsolutePath;
            }
            set
            {
                _imagefileAbsolutePath = value;
                generateSpatialReference();
            }
        }
        public SpatialReference SpatialReference
        {
            get
            {
                return _spatialReference;
            }
        }
        #endregion Properties

        #region Constructors
        public ImageInfo()
        {
            _imagefileAbsolutePath = "";
            _spatialReference = null;
        }
        public ImageInfo(string imagefileAbsolutePath)
            : this()
        {
            _imagefileAbsolutePath = imagefileAbsolutePath;
            generateSpatialReference();
        }
        #endregion Constructors

        #region Public Methods
        public override string ToString()
        {
            return "Image file: " + ImagefileAbsolutePath;
        }
        #endregion Public Methods

        #region Private methods
        private void generateSpatialReference()
        {
            if (File.Exists(_imagefileAbsolutePath))
            {
                try
                {
                    Gdal.AllRegister();
                    const Access access = Access.GA_ReadOnly;
                    Dataset dataset = Gdal.Open(_imagefileAbsolutePath, access);
                    string geoTiffWkt = dataset.GetProjection();
                    OSGeo.OSR.SpatialReference spRef = new OSGeo.OSR.SpatialReference(geoTiffWkt);
                    spRef.MorphToESRI();
                    string esriWkt;
                    int m = spRef.ExportToWkt(out esriWkt);
                    this._spatialReference = new ScenarioTools.Spatial.SpatialReference(esriWkt, this);
                    GlobalSpatialStaticVariables.SpatialReference = _spatialReference;
                }
                catch (Exception e)
                {
                    string errmsg = "Error encountered in generating spatial reference from file '" + _imagefileAbsolutePath + "': ";
                    string err2 = e.Message;
                    errmsg = errmsg + err2;
                    MessageBox.Show(errmsg);
                    _spatialReference = null;
                }
            }
            else
            {
                _spatialReference = null;
            }
        }
        #endregion Private methods
    }
}
