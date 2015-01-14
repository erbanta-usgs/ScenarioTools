using System;
using System.Collections.Generic;
using System.Text;

using OSGeo.GDAL; // supports raster formats

namespace ScenarioTools.Spatial
{
    public class SpatialReferenceHelpers
    {
        private static Dictionary<string, string> synonyms = new Dictionary<string, string>()
        {
            // This method may not be needed, because OSGeo.OSR.SpatialReference.MorphToESRI()
            // seems able to convert WKT from a GeoTiff to the ESRI version of WKT.  However,
            // keep it around, just in case...

            // Enter synonyms as all lowercase
            //{"grs 1980", "grs_1980"},
            //{"kilometer", "kilometre"},
            //{"meter", "metre"},
            //{"nad83", "gcs_north_american_1983"},
            //{"nad83 / utm zone 17n", "nad_1983_utm_zone_17n"},
            //{"north_american_datum_1983", "d_north_american_1983"}
        };

        /// <summary>
        /// Get Well Known Text from a GeoTiff file and
        /// convert it to ESRI's Well Known Text format
        /// </summary>
        /// <param name="geoTiffPath"></param>
        /// <returns>Well Known Text in ESRI format</returns>
        public static string GetEsriWktFromGeoTiff(string geoTiffPath)
        {
            // Get Well Known Text from a GeoTiff file and 
            // convert it to ESRI's Well Known Text format
            string esriWkt = "";

            // Register GDAL and initialize required objects
            Gdal.AllRegister();
            IntPtr ptr = new IntPtr();
            bool memoryOwn = true;
            object obj = new object();

            // Create a GDAL dataset
            OSGeo.GDAL.Dataset dataset = new Dataset(ptr, memoryOwn, obj);
            Access access = Access.GA_ReadOnly;
            dataset = Gdal.Open(geoTiffPath, access);

            // Get the projection Well Known Text from the dataset, 
            // and create an OSGeo.OSR.SpatialReference from it
            string geoTiffWkt = dataset.GetProjection();
            OSGeo.OSR.SpatialReference spRef = new OSGeo.OSR.SpatialReference(geoTiffWkt);

            // Convert the well known text in the SpatialReference to ESRI format
            spRef.MorphToESRI();
            
            // Return the ESRI-format well known text
            esriWkt = "";
            int m = spRef.ExportToWkt(out esriWkt);
            return esriWkt;
        }

        /// <summary>
        /// Does a case-insensitive comparison of two strings for equality or synonymy
        /// </summary>
        /// <param name="string0"></param>
        /// <param name="string1"></param>
        /// <returns></returns>
        public static bool Synonymous(string string0, string string1)
        {
            // Convert both strings to lowercase
            string str0 = string0.ToLower();
            string str1 = string1.ToLower();

            // If strings are the same, return true
            if (str0 == str1)
            {
                return true;
            }

            string synonym = "";
            bool found;

            // Look up synonyms, first for string0 ...
            found = SpatialReferenceHelpers.synonyms.TryGetValue(str0, out synonym);
            if (found)
            {
                if (str1 == synonym)
                {
                    return true;
                }
            }
            // ... then for string1
            found = SpatialReferenceHelpers.synonyms.TryGetValue(str1, out synonym);
            if (found)
            {
                if (str0 == synonym)
                {
                    return true;
                }
            }

            // If execution reaches here, the strings are not the same, 
            // and the Synonyms dictionary does not contain an entry indicating 
            // the two strings are synonymous.
            return false;
        }
    }
}
