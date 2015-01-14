using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ScenarioTools.DataClasses;

namespace ScenarioTools.FileReaders
{
    public class BoreholeSampleFileReader
    {
        public static string[] GetAvailableSeries(string file)
        {
            StreamReader sr = null;
            List<string> availableSeries = new List<string>();

            char[] delim = { ' ', '\t' };
            try
            {
                // Open the file.
                sr = File.OpenText(file);

                // Add all unique identifiers.
                while (!sr.EndOfStream)
                {
                    // Get and split the line.
                    string[] line = sr.ReadLine().Split(delim, StringSplitOptions.RemoveEmptyEntries);

                    // If the first element is valid and unique, add it to the list.
                    if (line.Length > 0)
                    {
                        line[0] = line[0].Trim();
                        if (!line[0].Equals(""))
                        {
                            if (!availableSeries.Contains(line[0]))
                            {
                                availableSeries.Add(line[0]);
                            }
                        }
                    }
                }
            }
            catch { }

            // Close the file.
            if (sr != null)
            {
                try
                {
                    sr.Close();
                }
                catch { }
            }

            // Return the result.
            return availableSeries.ToArray();
        }
        public static TimeSeries GetValues(string file, string identifier)
        {
            // Trim the identifier and convert to lower case.
            identifier = identifier.Trim().ToLower();

            // Make the list for the values and declare the reader.
            List<TimeSeriesSample> values = new List<TimeSeriesSample>();
            StreamReader sr = null;

            char[] delim = { ' ', '\t' };
            char[] delimDate = { '/' };
            char[] delimTime = { ':' };

            try
            {
                // Open the file.
                sr = File.OpenText(file);

                // Add all entries from the specified series.
                while (!sr.EndOfStream)
                {
                    // Read the line and split it.
                    string[] line = sr.ReadLine().Split(delim, StringSplitOptions.RemoveEmptyEntries);

                    // If the line is long enough, try to get the value (the fourth entry).
                    if (line.Length >= 4)
                    {
                        // Only continue if this is the specified series.
                        if (identifier.Equals(line[0].Trim().ToLower()))
                        {

                            try
                            {
                                int year, month, day, hour, minute, second;

                                // The year, month, and day are obtained from the second entry, delimited by '/'.
                                string[] dateSplit = line[1].Split(delimDate, StringSplitOptions.RemoveEmptyEntries);
                                if (int.TryParse(dateSplit[0], out month) && int.TryParse(dateSplit[1], out day) && int.TryParse(dateSplit[2], out year))
                                {
                                    // The hour, minute, and second are obtained from the third entry.
                                    string[] timeSplit = line[2].Split(delimTime, StringSplitOptions.RemoveEmptyEntries);
                                    if (int.TryParse(timeSplit[0], out hour) && int.TryParse(timeSplit[1], out minute) && int.TryParse(timeSplit[2], out second))
                                    {
                                        // Make the date.
                                        DateTime date = new DateTime(year, month, day, hour, minute, second);

                                        // The value is obtained from the fourth entry.
                                        float value;
                                        if (float.TryParse(line[3], out value))
                                        {
                                            values.Add(new TimeSeriesSample(date, value));
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }

            // Close the reader.
            if (sr != null)
            {
                try
                {
                    sr.Close();
                }
                catch { }
            }

            // Return the result.
            return new TimeSeries(values.ToArray());
        }
    }
}
