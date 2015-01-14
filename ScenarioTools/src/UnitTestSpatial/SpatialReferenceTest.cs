using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using OSGeo.GDAL; // supports raster formats
using OSGeo.OSR;  // Spatial reference and coordinate transformation.

namespace UnitTestSpatial
{
    [TestFixture]
    public class SpatialReferenceTest
    {
        [Test]
        public void CreateSpatialReference()
        {
            string wktString = "PROJCS[\"NAD_1983_UTM_Zone_17N\",GEOGCS[\"GCS_North_American_1983\",DATUM[\"D_North_American_1983\",SPHEROID[\"GRS_1980\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",-81.0],PARAMETER[\"Scale_Factor\",0.9996],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";
            ScenarioTools.Spatial.SpatialReference newSR = new ScenarioTools.Spatial.SpatialReference(wktString, this);

            string wktTiff = "PROJCS[\"NAD83 / UTM zone 17N\",GEOGCS[\"NAD83\",DATUM[\"North_American_Datum_1983\",SPHEROID[\"GRS 1980\",6378137,298.2572221010002,AUTHORITY[\"EPSG\",\"7019\"]],AUTHORITY[\"EPSG\",\"6269\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4269\"]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",-81],PARAMETER[\"scale_factor\",0.9996],PARAMETER[\"false_easting\",500000],PARAMETER[\"false_northing\",0],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],AUTHORITY[\"EPSG\",\"26917\"]]";
            OSGeo.OSR.SpatialReference spRef = new OSGeo.OSR.SpatialReference(wktTiff);
            spRef.MorphToESRI();
            string newWkt;
            spRef.ExportToWkt(out newWkt);



            string wktString2 = "PROJCS[\"NAD_1983_UTM_Zone_10N\",GEOGCS[\"GCS_North_American_1983\",DATUM[\"D_North_American_1983\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",-123.0],PARAMETER[\"Scale_Factor\",0.9996],PARAMETER[\"Latitude_of_Origin\",0.0],UNIT[\"Meter\",1.0]]";
            ScenarioTools.Spatial.SpatialReference newSR2 = new ScenarioTools.Spatial.SpatialReference(wktString2, this);
            string errMsg = "";
            bool same = newSR.Matches(newSR2, ref errMsg);
            Assert.IsFalse(same);
            string wktString3 = "PROJCS[\"NAD_1983_UTM_Zone_17N\",GEOGCS[\"GCS_North_American_1983\",DATUM[\"D_North_American_1983\",SPHEROID[\"GRS_1980\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",-81.0],PARAMETER[\"Scale_Factor\",0.9996],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";
            ScenarioTools.Spatial.SpatialReference newSR3 = new ScenarioTools.Spatial.SpatialReference(wktString3, this);
            errMsg = "";
            same = newSR.Matches(newSR3, ref errMsg); // Should be identical
            Assert.IsTrue(same);
            // Make insignificant change to wktString3 (change parameter order)
            wktString3 = "PROJCS[\"NAD_1983_UTM_Zone_17N\",GEOGCS[\"GCS_North_American_1983\",DATUM[\"D_North_American_1983\",SPHEROID[\"GRS_1980\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"False_Easting\",500000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",-81.0],PARAMETER[\"Latitude_Of_Origin\",0.0],PARAMETER[\"Scale_Factor\",0.9996],UNIT[\"Meter\",1.0]]";
            errMsg = "";
            same = newSR.Matches(newSR3, ref errMsg);
            Assert.IsTrue(same);
        }

        [Test]
        public void UseOsrSpatialReference()
        {
            Gdal.AllRegister();
            IntPtr ptr = new IntPtr();
            bool memoryOwn = true;
            string testStr = "PROJCS[\"NAD83 / UTM zone 17N\",GEOGCS[\"NAD83\",DATUM[\"North_American_Datum_1983\",SPHEROID[\"GRS 1980\",6378137,298.2572221010002,AUTHORITY[\"EPSG\",\"7019\"]],AUTHORITY[\"EPSG\",\"6269\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4269\"]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",-81],PARAMETER[\"scale_factor\",0.9996],PARAMETER[\"false_easting\",500000],PARAMETER[\"false_northing\",0],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],AUTHORITY[\"EPSG\",\"26917\"]]";
            string wktString = testStr;
            OSGeo.OSR.SpatialReference spRef = new OSGeo.OSR.SpatialReference(wktString);

            OSGeo.OSR.Osr osr = new Osr();

            int epsg = spRef.AutoIdentifyEPSG(); // useless
            int code;
            int zone;
            int datum;
            //int a = spRef.ExportToUSGS(out code, out zone, out datum); // useless
            double[] argin = new double[15];
            //spRef.ImportFromUSGS(code, zone, argin, datum);

            string MIarg;
            spRef.ExportToMICoordSys(out MIarg);

            string str2 = "";
            spRef.MorphToESRI();
            int m = spRef.ExportToWkt(out str2);
            //string str3 = "";
            //m = spRef.ExportToPrettyWkt(out str3, 0);
            //string str4 = "";
            //m = spRef.ExportToPrettyWkt(out str4, 1);
            m = spRef.ImportFromWkt(ref wktString);
            m = spRef.ExportToWkt(out str2);

            //OSGeo.OSR.SpatialReference spRef2 = new OSGeo.OSR.SpatialReference(ptr, memoryOwn, this);
            OSGeo.OSR.SpatialReference spRef2 = new OSGeo.OSR.SpatialReference("");
            
            m = spRef2.ImportFromMICoordSys(MIarg);

            //spRef2.ImportFromESRI
            wktString = testStr;
            
            //m = spRef2.ImportFromWkt(ref wktString);
            string str3 = "";
            spRef2.MorphToESRI();
            spRef2.ExportToWkt(out str3);

            m = spRef.Fixup();
            m = spRef.ExportToWkt(out str3);
        }
    }
}
