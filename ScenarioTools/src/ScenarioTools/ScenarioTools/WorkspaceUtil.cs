using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

using ScenarioTools.DataClasses;
using ScenarioTools.Geometry;

namespace ScenarioTools
{
    public class WorkspaceUtil
    {
        public const string XML_NODE_TYPE_REPORT_ELEMENT = "reportelement";
        public const string MODEL_GRID_EXTENT_NAME = "model grid";
        public const string BACKGROUND_IMAGE_EXTENT_NAME = "background image";

        public static string GetWorkspaceXmlFile()
        {
            return GetWorkspaceDirectory() + "\\workspace.xml";
        }
        public static string GetCacheFilePath(long key)
        {
            return GetWorkspaceDirectory() + "\\" + key + ".bin";
        }
        public static string GetApplicationDirectory()
        {
            // Get the local path of the executable.
            string path = new Uri(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;

            // Get the directory name of this path.
            string directory = Path.GetDirectoryName(path);

            // Return the result.
            return directory;
        }
        public static string GetWorkspaceDirectory()
        {
            // Append the workspace directory to the application executable directory.
            string directory = GetApplicationDirectory() + "\\workspace";

            // Return the directory name.
            return directory;
        }

        public static ScenarioTools.Reporting.Report[] GetPersistedWorkspace()
        {
            string workspaceFilePath = GetWorkspaceXmlFile();
            ScenarioTools.Reporting.Report[] persistedWorkspace = null;
            if (File.Exists(workspaceFilePath))
            {
                try
                {
                    // Get the reports from the workspace file.
                    persistedWorkspace = ScenarioTools.Reporting.Report.WorkspaceFromXmlFile(workspaceFilePath);
                }
                catch
                {
                }
            }

            // If the persisted workspace is null, make it an empty array.
            if (persistedWorkspace == null)
            {
                persistedWorkspace = new ScenarioTools.Reporting.Report[0];
            }

            // Return the result.
            return persistedWorkspace;
        }
        public static void PersistWorkspace(ScenarioTools.Reporting.Report[] reports)
        {
            // Get the workspace XML file path.
            string directoryPath = GetWorkspaceDirectory();
            string filePath = GetWorkspaceXmlFile();

            // Create the folder. This has no effect if the directory already exists.
            Directory.CreateDirectory(directoryPath);

            // Write the file.
            Reporting.Report.SaveXmlFile(reports, filePath);
        }

        public static float[] GetCachedFloatArray(long key)
        {
            // Declare the array and reader.
            float[] array = null;
            BinaryReader br = null;

            // This is the section where the file values are read.
            try
            {
                // Open the file as a binary file.
                br = new BinaryReader(File.OpenRead(GetCacheFilePath(key)));

                // Read the values into a list.
                List<float> values = new List<float>();
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    values.Add(br.ReadSingle());
                }

                // Store the result in the array.
                array = values.ToArray();
            }
            catch
            {
                // If there was a problem, set the array to null (the cache is invalid).
                array = null;
            }

            // Close the reader.
            if (br != null)
            {
                try
                {
                    br.Close();
                }
                catch { }
            }

            // Return the result.
            return array;
        }
        public static float[,] GetCachedFloat2DArray(long key)
        {
            // Declare the array and reader.
            float[,] array = null;
            BinaryReader br = null;

            // This is the section where the file values are read.
            try
            {
                // Open the file as a binary file.
                br = new BinaryReader(File.OpenRead(GetCacheFilePath(key)));

                // Read the width and height of the array.
                int nCols = br.ReadInt32();
                int nRows = br.ReadInt32();

                // Make the array and read the values into it.
                array = new float[nRows, nCols];
                for (int i = 0; i < nRows; i++)
                {
                    for (int j = 0; j < nCols; j++)
                    {
                        array[i, j] = br.ReadSingle();
                    }
                }
            }
            catch
            {
            }

            // Close the reader.
            if (br != null)
            {
                try
                {
                    br.Close();
                }
                catch { }
            }

            // Return the result.
            return array;
        }
        //public static Extent GetCachedExtent(Stream stream)
        //{
        //    // Declare the reader and Extent fields
        //    BinaryReader br = null;
        //    //string name;
        //    double west, south, east, north;
        //    bool yIsFlipped;

        //    // Read data from file
        //    try
        //    {
        //        // Make a binary reader on the stream.
        //        br = new BinaryReader(stream);

        //        // Read the name
        //        //name = br.ReadString();

        //        // Read the coordinates
        //        west = br.ReadDouble();
        //        south = br.ReadDouble();
        //        east = br.ReadDouble();
        //        north = br.ReadDouble();

        //        // Read yIsFlipped
        //        yIsFlipped = br.ReadBoolean();
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //    Extent extent = new Extent(west, south, east, north);
        //    extent.YIsFlipped = yIsFlipped;
        //    return extent;
        //}
        public static GeoMap GetCachedGeoMap(Stream stream)
        {
            // Declare the arrays and reader.
            float[,] values = null;
            double[] xC = null;
            double[] yC = null;
            BinaryReader br = null;

            // This is the section where the file values are read.
            try
            {
                // Make a binary reader on the stream.
                br = new BinaryReader(stream);

                // Read the number of columns and the x coordinates.
                int nCols = br.ReadInt32();
                xC = new double[nCols + 1];
                for (int i = 0; i < xC.Length; i++)
                {
                    xC[i] = br.ReadDouble();
                }

                // Read the number of rows and the y coordinates.
                int nRows = br.ReadInt32();
                yC = new double[nRows + 1];
                for (int i = 0; i < yC.Length; i++)
                {
                    yC[i] = br.ReadDouble();
                }

                // Make the values and read the values into it.
                values = new float[nRows, nCols];
                for (int i = 0; i < nRows; i++)
                {
                    for (int j = 0; j < nCols; j++)
                    {
                        values[i, j] = br.ReadSingle();
                    }
                }
            }
            catch
            {
            }

            // If any of the components are null, return null.
            if (values == null || xC == null || yC == null)
            {
                return null;
            }
            else if (double.IsNaN(xC[0]) || double.IsNaN(yC[0]))
            {
                return null;
            }
            // Otherwise, return the result.
            else
            {
                return new GeoMap(values, xC, yC);
            }
        }
        public static TimeSeries GetCachedTimeSeries(Stream stream)
        {
            TimeSeries timeSeries = null;
            try
            {
                // Make a binary reader on the stream.
                BinaryReader br = new BinaryReader(stream);

                // Read the values into a list.
                List<TimeSeriesSample> samples = new List<TimeSeriesSample>();
                bool done = false;
                while (!done) //br.BaseStream.Position < br.BaseStream.Length)
                {
                    try
                    {
                        // Read the values.
                        long ticks = br.ReadInt64();
                        float value = br.ReadSingle();

                        // Add the time series sample to the list.
                        samples.Add(new TimeSeriesSample(new DateTime(ticks), value));
                    }
                    catch
                    {
                        done = true;
                    }
                }

                // Store the result in the time series.
                timeSeries = new TimeSeries(samples.ToArray());
            }

            // If there was a problem, set the time series to null.
            catch
            {
                timeSeries = null;
            }

            // If the time series is not null, but its length is zero, it is invalid, so set it to null.
            if (timeSeries != null)
            {
                if (timeSeries.Length == 0)
                {
                    timeSeries = null;
                }
            }

            // Return the time series.
            return timeSeries;
        }
        public static TimeSeries GetCachedTimeSeries(long key)
        {
            // Declare the time series and reader.
            TimeSeries timeSeries = null;
            Stream stream = null;


            try
            {
                // Open the file.
                stream = File.OpenRead(GetCacheFilePath(key));

                // Get the time series from the file.
                timeSeries = GetCachedTimeSeries(stream);
            }
            catch
            {
                stream = null;
            }

            // Close the file.
            if (stream != null)
            {
                try
                {
                    stream.Close();
                }
                catch { }
            }

            // Return the result.
            return timeSeries;
        }

        public static float[,] GetCachedFloat2DArray(Stream stream)
        {
            float[,] array = null;
            try
            {
                // Make a binary reader on the stream.
                BinaryReader br = new BinaryReader(stream);

                // Read the width and height of the array.
                int nCols = br.ReadInt32();
                int nRows = br.ReadInt32();

                // Make the array and read the values into it.
                array = new float[nRows, nCols];
                for (int i = 0; i < nRows; i++)
                {
                    for (int j = 0; j < nCols; j++)
                    {
                        array[i, j] = br.ReadSingle();
                    }
                }
            }

            // If there was a problem, set the array to null.
            catch
            {
                array = null;
            }

            // Return the array.
            return array;
        }

        private static void createWorkspaceDirectory()
        {
            // Get the path of the workspace directory.
            string workspace = GetWorkspaceDirectory();

            // If the path does not exist, create it.
            if (!Directory.Exists(workspace))
            {
                Directory.CreateDirectory(workspace);
            }
        }
        private static void writeFloatArray(BinaryWriter bw, float[] array)
        {
            // Write the values.
            for (int i = 0; i < array.Length; i++)
            {
                bw.Write(array[i]);
            }
        }
        public static void SetCachedFloatArray(long key, float[] array)
        {
            // Ensure that the workspace directory exists.
            createWorkspaceDirectory();

            // Open the file for writing.
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(GetCacheFilePath(key)));

            // Write the float array.
            writeFloatArray(bw, array);

            // Close the file.
            bw.Close();
        }
        private static void writeFloat2DArray(BinaryWriter bw, float[,] array)
        {
            // Write the number of columns and rows.
            int nCols = array.GetLength(0);
            int nRows = array.GetLength(1);
            bw.Write(nCols);
            bw.Write(nRows);

            // Write the array.
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    bw.Write(array[i, j]);
                }
            }
        }
        private static void writeExtent(BinaryWriter bw, Extent extent)
        {
            // Write the name
            //bw.Write(extent.Name);

            // Write the coordinates
            bw.Write(extent.West);
            bw.Write(extent.South);
            bw.Write(extent.East);
            bw.Write(extent.North);

            // Write yIsFlipped
            bw.Write(extent.YIsFlipped);
        }
        private static void writeGeoMap(BinaryWriter bw, GeoMap geoMap)
        {
            // Write the number of columns.
            int nCols = geoMap.NCols;
            bw.Write(nCols);

            // Write the x coordinates.
            for (int i = 0; i <= nCols; i++)
            {
                bw.Write(geoMap.GetXCellBound(i));
            }

            // Write the number of rows.
            int nRows = geoMap.NRows;
            bw.Write(nRows);

            // Write the y coordinates.
            for (int i = 0; i <= nRows; i++)
            {
                bw.Write(geoMap.GetYCellBound(i));
            }

            // Write the array.
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    bw.Write(geoMap.GetValueAtCell(i, j));
                }
            }
        }
        public static void SetCachedFloat2DArray(long key, float[,] array)
        {
            // Ensure that the workspace directory exists.
            createWorkspaceDirectory();

            // If the array is null, return.
            if (array == null)
            {
                return;
            }

            // Try to write the file.
            BinaryWriter bw = null;
            try
            {
                // Open the file for writing.
                bw = new BinaryWriter(File.OpenWrite(GetCacheFilePath(key)));

                // Write the array.
                writeFloat2DArray(bw, array);
            }
            catch { }

            // Close the file.
            if (bw != null)
            {
                try
                {
                    bw.Close();
                }
                catch { }
            }
        }
        
        private static void writeTimeSeries(BinaryWriter bw, TimeSeries timeSeries) {
            // Write the values.
            for (int i = 0; i < timeSeries.Length; i++)
            {
                bw.Write(timeSeries[i].Date.Ticks);
                bw.Write(timeSeries[i].Value);
            }
        }
        public static void SetCachedTimeSeries(long key, TimeSeries timeSeries)
        {
            // Ensure that the workspace directory exists.
            createWorkspaceDirectory();

            // Open the file for writing.
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(GetCacheFilePath(key)));

            // Write the time series.
            writeTimeSeries(bw, timeSeries);

            // Close the file.
            bw.Close();
        }
        public static void ClearCache(long key)
        {
            // Get the file path corresponding to the key.
            string path = GetCacheFilePath(key);

            // If the file exists, delete it.
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private static RandomNumberGenerator rng = RNGCryptoServiceProvider.Create();
        public static long GetUniqueIdentifier()
        {
            // Remove a time-dependent number of values.
            int cycle = DateTime.Now.Millisecond % 50;
            byte[] discard = new byte[cycle];
            rng.GetBytes(discard);

            // Produce and return a long int value.
            byte[] buffer = new byte[sizeof(long)];
            rng.GetBytes(buffer);
            return Math.Abs(BitConverter.ToInt64(buffer, 0));
        }

        public static byte[] ConvertCacheToByteArray(object dataset)
        {
            // Make a binary writer on a memory stream.
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            // Write the dataset to the stream.
            if (dataset is float[])
            {
                writeFloatArray(bw, (float[])dataset);
            }
            else if (dataset is float[,])
            {
                writeFloat2DArray(bw, (float[,])dataset);
            }
            else if (dataset is TimeSeries)
            {
                writeTimeSeries(bw, (TimeSeries)dataset);
            }
            else if (dataset is Extent)
            {
                writeExtent(bw, (Extent)dataset);
            }
            else if (dataset is GeoMap)
            {
                writeGeoMap(bw, (GeoMap)dataset);
            }
            else if (dataset is float)
            {
                bw.Write(long.MinValue);
                bw.Write((float)dataset);
            }

            // Return a copy of the byte array behind the stream.
            byte[] bytes = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(bytes, 0, (int)ms.Length);
            return bytes;
        }

        // Field and methods to implement default extent
        private static ScenarioTools.Geometry.Extent _defaultExtent = new Extent();
        public static void ClearDefaultExtent()
        {
            _defaultExtent = new Extent();
        }
        public static void SetDefaultExtent(Extent extent)
        {
            if (extent != null)
            {
                if (!extent.IsNull)
                {
                    _defaultExtent = new Extent(extent);
                }
            }
        }
        public static ScenarioTools.Geometry.Extent GetDefaultExtent()
        {
            return _defaultExtent;
        }
        public static ScenarioTools.Geometry.Extent GetModelGridExtent()
        {
            if (_defaultExtent.Name == WorkspaceUtil.MODEL_GRID_EXTENT_NAME)
            {
                return _defaultExtent;
            }
            return null;
        }

        public static object GetCachedTimeSeriesOrFloat(Stream stream)
        {
            TimeSeries timeSeries = null;
            try
            {
                // Make a binary reader on the stream.
                BinaryReader br = new BinaryReader(stream);

                // Read the values into a list.
                List<TimeSeriesSample> samples = new List<TimeSeriesSample>();
                bool done = false;
                while (!done) //br.BaseStream.Position < br.BaseStream.Length)
                {
                    try
                    {
                        // Read the values.
                        long ticks = br.ReadInt64();
                        float value = br.ReadSingle();

                        if (ticks == long.MinValue)
                        {
                            return value;
                        }

                        // Add the time series sample to the list.
                        samples.Add(new TimeSeriesSample(new DateTime(ticks), value));
                    }
                    catch
                    {
                        done = true;
                    }
                }

                // Store the result in the time series.
                timeSeries = new TimeSeries(samples.ToArray());
            }

            // If there was a problem, set the time series to null.
            catch
            {
                timeSeries = null;
            }

            // If the time series is not null, but its length is zero, it is invalid, so set it to null.
            if (timeSeries != null)
            {
                if (timeSeries.Length == 0)
                {
                    timeSeries = null;
                }
            }

            // Return the time series.
            return timeSeries;
        }

        public static void WriteTimeSeriesToCsv(TimeSeries timeSeries, string filename)
        {
            // Open the file for writing.
            StreamWriter sw = File.CreateText(filename);

            // Write the header.
            sw.WriteLine("Date, Value");

            // Write the values.
            for (int i = 0; i < timeSeries.Length; i++)
            {
                sw.WriteLine(timeSeries[i].Date.ToShortDateString() + " " + timeSeries[i].Date.ToShortTimeString() + ", " + timeSeries[i].Value);
            }

            // Flush and close the file.
            sw.Flush();
            sw.Close();
        }
    }
}
