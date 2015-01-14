using System;
using System.Collections.Generic;
using System.Text;

namespace ScenarioTools.Spatial
{
    public enum SpatialRefCompareResult
    {
        Same = 0,
        Different = 1
    }

    /// <summary>
    /// Contains definition of a projected coordinate system
    /// </summary>
    public class SpatialReference
    {
        //private const string SPE_WKT_STRING =
        //    "PROJCS[\"NAD_1983_HARN_StatePlane_Florida_East_FIPS_0901_Feet\",GEOGCS[\"GCS_North_American_1983_HARN\",DATUM[\"D_North_American_1983_HARN\",SPHEROID[\"GRS_1980\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",656166.6666666665],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",-81.0],PARAMETER[\"Scale_Factor\",0.9999411764705882],PARAMETER[\"Latitude_Of_Origin\",24.33333333333333],UNIT[\"Foot_US\",0.3048006096012192]]";
        //private const string UTM_WKT_STRING =
        //    "PROJCS[\"NAD_1983_UTM_Zone_17N\",GEOGCS[\"GCS_North_American_1983\",DATUM[\"D_North_American_1983\",SPHEROID[\"GRS_1980\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",-81.0],PARAMETER[\"Scale_Factor\",0.9996],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";
        //private const string GCS_WKT_STRING =
        //    "GEOGCS[\"WGS 84\",DATUM[\"World Geodetic System 1984\",SPHEROID[\"WGS 84\", 6378137.0, 298.257223563, AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\", 0.0, AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\", 0.017453292519943295],AXIS[\"Geodetic latitude\", NORTH],AXIS[\"Geodetic longitude\", EAST],AUTHORITY[\"EPSG\",\"4326\"]]";

        //public static readonly SpatialReference FloridaStatePlaneEast = new SpatialReference(SPE_WKT_STRING);
        //public static readonly SpatialReference Utm17N = new SpatialReference(UTM_WKT_STRING);
        //public static readonly SpatialReference Wgs84 = new SpatialReference(GCS_WKT_STRING);

        #region Fields
        private string _wktString;
        private SpatialReferenceParameter _spatialReferenceParameter;
        private object _georeferencedObject;
        #endregion Fields

        #region Constructors
        public SpatialReference()
        {
            _wktString = "";
            _spatialReferenceParameter = null;
            _georeferencedObject = null;
        }
        public SpatialReference(string wktString, object georeferencedObject)
            : this()
        {
            this._wktString = wktString;
            this._spatialReferenceParameter = new SpatialReferenceParameter(wktString);
            this._georeferencedObject = georeferencedObject;
        }
        #endregion Constructors

        #region Properties
        public object GeoreferencedObject
        {
            get
            {
                return _georeferencedObject;
            }
        }
        public string WktString
        {
            get
            {
                return _wktString;
            }
        }
        #endregion Properties

        #region Public methods
        public string GetWktString()
        {
            return this._wktString;
        }
        public bool Matches(SpatialReference otherSpatialReference, ref string errMsg)
        {
            errMsg = "";
            SpatialRefCompareResult comparison = compare(otherSpatialReference, ref errMsg);
            if (comparison == SpatialRefCompareResult.Same)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion Public methods

        #region Private methods
        private SpatialRefCompareResult compare(SpatialReference otherSpatialReference, ref string errorMessage)
        {
            SpatialRefCompareResult result = this._spatialReferenceParameter.Compare(otherSpatialReference._spatialReferenceParameter);
            if (result == SpatialRefCompareResult.Different)
            {
                _spatialReferenceParameter.AppendErrorMessage(ref errorMessage);
            }
            return result;
        }
        #endregion Private methods
    }
}
