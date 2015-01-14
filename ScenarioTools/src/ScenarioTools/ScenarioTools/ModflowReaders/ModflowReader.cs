using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ScenarioTools.DataClasses;
using ScenarioTools.Numerical;
using ScenarioTools.Util;

namespace ScenarioTools.ModflowReaders
{
    public class ModflowReader
    {
        public static string HnofloText { get; set; }
        public static string HdryText { get; set; }
        public static string CinactText { get; set; }

        public static void GetDiscretizationInfo(string discretizationFile, out int nCols, 
            out int nRows, out int nLayers, out int nStressPeriods, out int nTotalTimesteps, 
            bool freeFormat, NamefileInfo nfi)
        {
            // Make a discretization file.
            DiscretizationFile disFile = DiscretizationFile.fromFile(discretizationFile, freeFormat, nfi);

            nCols = disFile.getNcol();
            nRows = disFile.getNrow();
            nLayers = disFile.getNlay();
            nStressPeriods = disFile.getNper();

            nTotalTimesteps = 0;
            for (int i = 0; i < nStressPeriods; i++)
            {
                nTotalTimesteps += disFile.getStressPeriod(i).Nstp;
            }

            Console.WriteLine("total timesteps: " + nTotalTimesteps);
        }
        public static TimeSeries GetArrayDataAtCell(string headsFile, string dataDescriptor, int colOneBased, int rowOneBased, 
            int layerOneBased, TemporalReference temporalReference)
        {
            Console.WriteLine("*********************READING " + colOneBased + ", " + rowOneBased + " FROM " + headsFile + "********************");

            // If the temporal reference is null, use the default temporal reference.
            if (temporalReference == null)
            {
                throw new Exception("temporalReference cannot be null in ModflowReader.GetArrayDataAtCell");
            }

            // Determine the precision model of the heads-type file
            string identifier;
            int precision = PrecisionHelpers.BinaryDataFilePrecision(headsFile, out identifier);
            if (precision < 1 || precision > 2)
            {
                return null;
            }
            int realSize = precision * 4;

            // Do data values represent concentrations?
            bool dataAreConcentrations = identifier.ToLower() == "concentration";

            // Define values that should be ignored
            float hnoflo = Convert.ToSingle(HnofloText);
            float hdry = Convert.ToSingle(HdryText);
            float cinact = Convert.ToSingle(CinactText);
            double hnoflod = Convert.ToDouble(HnofloText);
            double hdryd = Convert.ToDouble(HdryText);
            double cinactd = Convert.ToDouble(CinactText);

            // Declare variables that are used frequently
            int kstp, kper, ncol, nrow, ilay, ntrans;
            int numCellsToSkip;
            float value = float.NaN;
            double tempDouble;
            double totimd;
            string text;

            // Make the array.
            List<TimeSeriesSample> result = new List<TimeSeriesSample>();

            // Determine the zero-based indices for the row and column.
            int colZeroBased = colOneBased - 1;
            int rowZeroBased = rowOneBased - 1;

            // Read the values from the file.
            BinaryReader br = null;
            try
            {
                // Open the file.
                br = new BinaryReader(new FileStream(headsFile, FileMode.Open, FileAccess.Read, FileShare.Read, 256));

                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    System.GC.Collect();

                    // Read the header for this 2D array.
                    if (dataAreConcentrations)
                    {
                        ModflowReadersHelpers.ReadUcnHeader(precision, br, out kstp, out kper, out ntrans, out totimd);
                    }
                    else
                    {
                        ModflowReadersHelpers.ReadModflowHeader(precision, br, out kstp, out kper, out totimd);
                    }
                    text = new string(br.ReadChars(16)).Trim();
                    ncol = br.ReadInt32();
                    nrow = br.ReadInt32();
                    ilay = br.ReadInt32();

                    // If this is the appropriate layer, read the value for the cell.
                    if (ilay == layerOneBased && text == dataDescriptor)
                    {
                        // Skip ahead the appropriate amount.
                        numCellsToSkip = rowZeroBased * ncol + colZeroBased;
                        br.BaseStream.Position += numCellsToSkip * realSize;

                        // Read the value and add it to the list.
                        switch (precision)
                        {
                            case 1:
                                value = br.ReadSingle();
                                if (dataAreConcentrations)
                                {
                                    if (value == cinact)
                                    {
                                        value = float.NaN;
                                    }
                                }
                                else
                                {
                                    if (value == hnoflo || value == hdry)
                                    {
                                        value = float.NaN;
                                    }
                                }
                                break;
                            case 2:
                                tempDouble = br.ReadDouble();
                                if (dataAreConcentrations)
                                {
                                    if (tempDouble==cinactd)
                                    {
                                        value = float.NaN;
                                    }
                                    else
                                    {
                                        value = Convert.ToSingle(tempDouble);
                                    }
                                }
                                else 
                                {
                                    if (tempDouble == hnoflod || tempDouble == hdryd)
                                    {
                                        value = float.NaN;
                                    }
                                    else
                                    {
                                        value = Convert.ToSingle(tempDouble);
                                    }
                                }
                                break;
                        }
                        result.Add(new TimeSeriesSample(temporalReference.GetDate(totimd), value));

                        // Skip over the rest of the grid.
                        br.BaseStream.Position += (ncol * nrow - numCellsToSkip - 1) * realSize;
                    }
                    else
                    {
                        br.BaseStream.Position += ncol * nrow * realSize;
                    }
                }
            }

            catch (Exception e) {
                Console.WriteLine("Exception:: " + e.Message);
            }

            if (br != null)
            {
                try
                {
                    br.Close();
                }
                catch { }
            }

            // If the result is of zero length, return null.
            if (result.Count == 0)
            {
                return null;
            }

            // Return the result as an array.
            return new TimeSeries(result.ToArray());
        }
        public static TimeSeries GetArrayDataAtCells(string headsFile, string dataDescriptor, Point[] cells, int startLayerOneBased, int endLayerOneBased, 
            TemporalReference temporalReference)
        {
            // If the temporal reference is null, use the default temporal reference.
            if (temporalReference == null)
            {
                throw new Exception("temporalReference cannot be null in ModflowReader.GetArrayDataAtCells");
            }

            // Determine the precision model of the heads file
            string identifier;
            int precision = PrecisionHelpers.BinaryDataFilePrecision(headsFile, out identifier);
            if (precision < 1 || precision > 2)
            {
                return null;
            }
            int realSize = precision * 4;

            // Do data values represent concentrations?
            bool dataAreConcentrations = identifier.ToLower() == "concentration";

            // Define values that should be ignored
            float hnoflo = Convert.ToSingle(HnofloText);
            float hdry = Convert.ToSingle(HdryText);
            float cinact = Convert.ToSingle(CinactText);
            double hnoflod = Convert.ToDouble(HnofloText);
            double hdryd = Convert.ToDouble(HdryText);
            double cinactd = Convert.ToDouble(CinactText);

            // Declare variables that are used frequently
            int kstp, kper, ncol, nrow, ilay, i, ntrans;
            int numCellsToSkip;
            float tempFloat;
            double tempDouble;
            double totimd = 0.0;
            string text;

            // Make the list for the time series samples obtained from the file.
            List<TimeSeriesSample> result = new List<TimeSeriesSample>();

            // Get a list of the cells in the order they will be encountered in the file.
            Point[] orderedCells = CellGroupProvider.InFileOrder(cells);

            // Read the values from the file.
            BinaryReader br = null;
            if (orderedCells.Length > 0)
            {
                try
                {
                    // Open the file.
                    br = new BinaryReader(new FileStream(headsFile, FileMode.Open));

                    // Initialize the value.
                    float sumValue = float.NaN;

                    // Make variables for tracking the stress period and timestep, and descriptor. 
                    // These are used to determine when the stress period or timestep or descriptor have changed.
                    int lastStressPeriod = 0;
                    int lastTimestep = 0;
                    string lastDescriptor = "";

                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        // Read the header for this 2D array.
                        if (dataAreConcentrations)
                        {
                            ModflowReadersHelpers.ReadUcnHeader(precision, br, out kstp, out kper, out ntrans, out totimd);
                        }
                        else
                        {
                            ModflowReadersHelpers.ReadModflowHeader(precision, br, out kstp, out kper, out totimd);
                        }
                        text = new string(br.ReadChars(16)).Trim();
                        ncol = br.ReadInt32();
                        nrow = br.ReadInt32();
                        ilay = br.ReadInt32();

                        // Show the identifier.
                        Console.WriteLine(text);

                        // If this is a new stress period or timestep or descriptor, add the value to the time series and reset the value.
                        if (kper != lastStressPeriod || kstp != lastTimestep || text != lastDescriptor)
                        {
                            // If the value is good, add it to the time series.
                            if (!float.IsNaN(sumValue))
                            {
                                result.Add(new TimeSeriesSample(temporalReference.GetDate(totimd), sumValue));
                            }

                            // Update the stress period and timestep and descriptor.
                            lastStressPeriod = kper;
                            lastTimestep = kstp;
                            lastDescriptor = text;

                            // Reset the value accumulator.
                            sumValue = 0.0f;
                        }

                        // If this is in the specified layer range, read the values.
                        if (ilay >= startLayerOneBased && ilay <= endLayerOneBased && text == dataDescriptor)
                        {
                            // Skip to the first cell.
                            numCellsToSkip = (orderedCells[0].Y - 1) * ncol + (orderedCells[0].X - 1);
                            br.BaseStream.Position += numCellsToSkip * realSize;

                            // Read the first value and add it to the sum.
                            switch (precision)
                            {
                                case 1:
                                    tempFloat = br.ReadSingle();
                                    if (dataAreConcentrations)
                                    {
                                        if (tempFloat != cinact)
                                        {
                                            sumValue += tempFloat;
                                        }
                                    }
                                    else
                                    {
                                        if (tempFloat != hnoflo && tempFloat != hdry)
                                        {
                                            sumValue += tempFloat;
                                        }
                                    }
                                    break;
                                case 2:
                                    tempDouble = br.ReadDouble();
                                    if (dataAreConcentrations)
                                    {
                                        if (tempDouble != cinactd)
                                        {
                                            sumValue += Convert.ToSingle(tempDouble);
                                        }
                                    }
                                    else
                                    {
                                        if (tempDouble != hnoflod && tempDouble != hdryd)
                                        {
                                            sumValue += Convert.ToSingle(tempDouble);
                                        }
                                    }
                                    break;
                            }


                            // Read subsequent cells.
                            for (i = 1; i < orderedCells.Length; i++)
                            {
                                // Skip to the cell.
                                numCellsToSkip = (orderedCells[i].Y - orderedCells[i - 1].Y) * ncol + (orderedCells[i].X - orderedCells[i - 1].X) - 1;
                                br.BaseStream.Position += numCellsToSkip * realSize;

                                // Read the value and add it to the sum.
                                switch (precision)
                                {
                                    case 1:
                                        tempFloat = br.ReadSingle();
                                        if (dataAreConcentrations)
                                        {
                                            if (tempFloat != cinact)
                                            {
                                                sumValue += tempFloat;
                                            }
                                        }
                                        else
                                        {
                                            if (tempFloat != hnoflo && tempFloat != hdry)
                                            {
                                                sumValue += tempFloat;
                                            }
                                        }
                                        break;
                                    case 2:
                                        tempDouble = br.ReadDouble();
                                        if (dataAreConcentrations)
                                        {
                                            if (tempDouble != cinactd)
                                            {
                                                sumValue += Convert.ToSingle(tempDouble);
                                            }
                                        }
                                        else
                                        {
                                            if (tempDouble != hnoflod && tempDouble != hdryd)
                                            {
                                                sumValue += Convert.ToSingle(tempDouble);
                                            }
                                        }
                                        break;
                                }
                            }

                            // Skip to the end of this grid.
                            Point lastCell = orderedCells[orderedCells.Length - 1];
                            br.BaseStream.Position += (ncol * nrow - ((lastCell.Y - 1) * ncol + lastCell.X)) * realSize;
                        }
                        else
                        {
                            br.BaseStream.Position += ncol * nrow * realSize;
                        }
                    }

                    // If the last value is good, add it to the time series.
                    // If the value is good, add it to the time series.
                    if (!float.IsNaN(sumValue))
                    {
                        result.Add(new TimeSeriesSample(temporalReference.GetDate(totimd), sumValue));
                    }
                }
                catch { }
            }

            if (br != null)
            {
                try
                {
                    br.Close();
                }
                catch { }
            }

            // Return the result as an array.
            return new TimeSeries(result.ToArray());
        }

        public static GeoMap GetArrayDataMapAtTimestep(string headsFile, string dataDescriptor, int layerOneBased, 
            int stressPeriodOneBased, int timestepOneBased)
        {
            // Ned TODO: Also, make a version that will take a CellCenteredArealGrid, 
            // to use as source of extent and cell coordinates.
            // Make the array.
            float[,] result = null;

            // Determine the precision model of the heads file
            string identifier;
            int precision = PrecisionHelpers.BinaryDataFilePrecision(headsFile, out identifier);
            if (precision < 1 || precision > 2)
            {
                return null;
            }
            int realSize = precision * 4;

            // Do data values represent concentrations?
            bool dataAreConcentrations = identifier.ToLower() == "concentration";

            // Define values that should be ignored
            float hnoflo = Convert.ToSingle(HnofloText);
            float hdry = Convert.ToSingle(HdryText);
            float cinact = Convert.ToSingle(CinactText);
            double hnoflod = Convert.ToDouble(HnofloText);
            double hdryd = Convert.ToDouble(HdryText);
            double cinactd = Convert.ToDouble(CinactText);

            // Declare variables that are used frequently
            int kstp, kper, ncol, nrow, ilay, i, j, ntrans;
            float tempFloat;
            double tempDouble;
            double totimd = 0.0;
            string text;

            BinaryReader br = null;
            try
            {
                // Open the file.
                br = new BinaryReader(new FileStream(headsFile, FileMode.Open));

                while (result == null)
                {
                    // Read the header.
                    if (dataAreConcentrations)
                    {
                        ModflowReadersHelpers.ReadUcnHeader(precision, br, out kstp, out kper, out ntrans, out totimd);
                    }
                    else
                    {
                        ModflowReadersHelpers.ReadModflowHeader(precision, br, out kstp, out kper, out totimd);
                    }
                    text = new string(br.ReadChars(16)).Trim();
                    ncol = br.ReadInt32();
                    nrow = br.ReadInt32();
                    ilay = br.ReadInt32();

                    if (ilay == layerOneBased && kper == stressPeriodOneBased && kstp == timestepOneBased && text == dataDescriptor)
                    {
                        // Make an array for the current layer.
                        result = new float[nrow, ncol];

                        // Populate the array.
                        switch (precision)
                        {
                            case 1:
                                for (i = 0; i < nrow; i++)
                                {
                                    for (j = 0; j < ncol; j++)
                                    {
                                        tempFloat = br.ReadSingle();
                                        if (dataAreConcentrations)
                                        {
                                            if (tempFloat == cinact)
                                            {
                                                tempFloat = float.NaN;
                                            }
                                        }
                                        else
                                        {
                                            if (tempFloat == hnoflo || tempFloat == hdry)
                                            {
                                                tempFloat = float.NaN;
                                            }
                                        }
                                        result[i, j] = tempFloat;
                                    }
                                }
                                break;
                            case 2:
                                for (i = 0; i < nrow; i++)
                                {
                                    for (j = 0; j < ncol; j++)
                                    {
                                        tempDouble = br.ReadDouble();
                                        if (dataAreConcentrations)
                                        {
                                            if (tempDouble == cinactd)
                                            {
                                                result[i, j] = float.NaN;
                                            }
                                            else
                                            {
                                                result[i, j] = Convert.ToSingle(tempDouble);
                                            }
                                        }
                                        else
                                        {
                                            if (tempDouble == hnoflod || tempDouble == hdryd)
                                            {
                                                result[i, j] = float.NaN;
                                            }
                                            else
                                            {
                                                result[i, j] = Convert.ToSingle(tempDouble);
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        br.BaseStream.Position += ncol * nrow * realSize;
                    }
                }
            }

            catch (Exception ex) {

                MessageBox.Show("Error reading file " + headsFile + ": " + ex.Message);
            }

            if (br != null)
            {
                try
                {
                    br.Close();
                }
                catch { }
            }

            // Return the result as a GeoMap.
            // The convertFlowToFlux argument to either method invoked below is 
            // always false because this method is for getting data from a 
            // head, drawdown, or concentration file, which are never flow values.
            Geometry.Extent modelGridExtent = WorkspaceUtil.GetModelGridExtent();
            if (modelGridExtent != null)
            {
                return GeoMap.FixedGridMap(result, modelGridExtent, false);
            }
            else if (ScenarioTools.Spatial.StaticObjects.Grid != null)
            {
                return GeoMap.GridMap(result, ScenarioTools.Spatial.StaticObjects.Grid, false);
            }
            else
            {
                return null;
            }
        }

        public static float[] GetMinArrayValueOverMapInInterval(string headsFile, string dataDescriptor, 
            int layerOneBased, int stressPeriodStartOneBased, int timestepStartOneBased, 
            int stressPeriodEndOneBased, int timestepEndOneBased)
        {
            // Make the result array.
            List<float> values = new List<float>();

            // Determine the precision model of the heads file
            string identifier;
            int precision = PrecisionHelpers.BinaryDataFilePrecision(headsFile, out identifier);
            if (precision < 1 || precision > 2)
            {
                return null;
            }
            int realSize = precision * 4;

            // Do data values represent concentrations?
            bool dataAreConcentrations = identifier.ToLower() == "concentration";

            // Define values that should be ignored
            float hnoflo = Convert.ToSingle(HnofloText);
            float hdry = Convert.ToSingle(HdryText);
            float cinact = Convert.ToSingle(CinactText);
            double hnoflod = Convert.ToDouble(HnofloText);
            double hdryd = Convert.ToDouble(HdryText);
            double cinactd = Convert.ToDouble(CinactText);

            // Declare variables that are used frequently
            int kstp, kper, ilay, i, j, k, m, ncol, nrow, ntrans;
            float value;
            double tempDouble;
            double totimd = 0.0;
            string text;

            BinaryReader br = null;
            try
            {
                // Open the file.
                br = new BinaryReader(new FileStream(headsFile, FileMode.Open));

                // This flag is for whne the specified time period has been exceeded in the file.
                bool done = false;

                while (!done)
                {
                    // Read the header.
                    if (dataAreConcentrations)
                    {
                        ModflowReadersHelpers.ReadUcnHeader(precision, br, out kstp, out kper, out ntrans, out totimd);
                    }
                    else
                    {
                        ModflowReadersHelpers.ReadModflowHeader(precision, br, out kstp, out kper, out totimd);
                    }
                    text = new string(br.ReadChars(16)).Trim();
                    ncol = br.ReadInt32();
                    nrow = br.ReadInt32();
                    ilay = br.ReadInt32();

                    /*
                    Console.WriteLine("kstp:   " + kstp);
                    Console.WriteLine("kper:   " + kper);
                    Console.WriteLine("pertim: " + pertim);
                    Console.WriteLine("totim:  " + totim);
                    Console.WriteLine("text:   " + text);
                    Console.WriteLine("ncol:   " + ncol);
                    Console.WriteLine("nrow:   " + nrow);
                    Console.WriteLine("ilay:   " + ilay);
                     */

                    // If past the last stress period, or if past the last timstep in the last stress period, flag as done.
                    if (kper > stressPeriodEndOneBased || (kper == stressPeriodEndOneBased && kstp > timestepEndOneBased)) {
                        done = true;
                    }

                    // If in the speicifed range, read the values and find the max.
                    else if (ilay == layerOneBased && kper >= stressPeriodStartOneBased && kstp >= timestepStartOneBased && 
                        kper <= stressPeriodEndOneBased && kstp <= timestepEndOneBased && text == dataDescriptor)
                    {
                        // Initialize the minimum value. We will be using a NaN-aware min, so this will be replaced with the first non-NaN value.
                        float minValue = float.NaN;

                        // Check all values in the current layer.
                        switch (precision)
                        {
                            case 1:
                                for (j = 0; j < nrow; j++)
                                {
                                    for (i = 0; i < ncol; i++)
                                    {
                                        value = br.ReadSingle();
                                        if (dataAreConcentrations)
                                        {
                                            if (value == cinact)
                                            {
                                                value = float.NaN;
                                            }
                                        }
                                        else
                                        {
                                            if (value == hnoflo || value == hdry)
                                            {
                                                value = float.NaN;
                                            }
                                        }
                                        minValue = MathUtil.NanAwareMin(value, minValue);
                                    }
                                }
                                break;
                            case 2:
                                for (j = 0; j < nrow; j++)
                                {
                                    for (i = 0; i < ncol; i++)
                                    {
                                        tempDouble = br.ReadDouble();
                                        value = Convert.ToSingle(tempDouble);
                                        if (dataAreConcentrations)
                                        {
                                            if (tempDouble == cinactd)
                                            {
                                                value = float.NaN;
                                            }
                                        }
                                        else
                                        {
                                            if (tempDouble == hnoflod || tempDouble == hdryd)
                                            {
                                                value = float.NaN;
                                            }
                                        }
                                        minValue = MathUtil.NanAwareMin(value, minValue);
                                    }
                                }
                                break;
                        }

                        // Add the value to the list.
                        values.Add(minValue);
                    }

                    // Otherwise, advance in the stream.
                    else
                    {
                        br.BaseStream.Position += ncol * nrow * 4;
                    }
                }
            }

            catch { }

            // Close the file.
            if (br != null)
            {
                try
                {
                    br.Close();
                }
                catch { }
            }

            // Return the result.
            return values.ToArray();
        }
        public static float[] GetMaxArrayValueOverMapInInterval(string headsFile, string dataDescriptor, 
               int layerOneBased, int stressPeriodStartOneBased, int timestepStartOneBased,
               int stressPeriodEndOneBased, int timestepEndOneBased, out string[] datasetResultMessage)
        {
            // Make the result array.
            List<float> values = new List<float>();

            // Set the dataset result message to null.
            datasetResultMessage = null;

            // Determine the precision model of the heads file
            string identifier;
            int precision = PrecisionHelpers.BinaryDataFilePrecision(headsFile, out identifier);
            if (precision < 1 || precision > 2)
            {
                return null;
            }
            int realSize = precision * 4;

            // Do data values represent concentrations?
            bool dataAreConcentrations = identifier.ToLower() == "concentration";

            // Define values that should be ignored
            float hnoflo = Convert.ToSingle(HnofloText);
            float hdry = Convert.ToSingle(HdryText);
            float cinact = Convert.ToSingle(CinactText);
            double hnoflod = Convert.ToDouble(HnofloText);
            double hdryd = Convert.ToDouble(HdryText);
            double cinactd = Convert.ToDouble(CinactText);

            // Declare variables that are used frequently
            int kstp, kper, ilay, i, j, k, m, ncol, nrow, ntrans;
            float value;
            double tempDouble;
            double totimd = 0.0;
            string text;

            // If the file does not exist, indicate so in the dataset result message and return.
            if (!File.Exists(headsFile))
            {
                datasetResultMessage = new string[] { "The specified file, " + headsFile + " does not exist." };
                return null;
            }

            BinaryReader br = null;
            try
            {
                // Open the file.
                br = new BinaryReader(new FileStream(headsFile, FileMode.Open));

                // This flag is for whne the specified time period has been exceeded in the file.
                bool done = false;

                while (!done)
                {
                    // Read the header.
                    if (dataAreConcentrations)
                    {
                        ModflowReadersHelpers.ReadUcnHeader(precision, br, out kstp, out kper, out ntrans, out totimd);
                    }
                    else
                    {
                        ModflowReadersHelpers.ReadModflowHeader(precision, br, out kstp, out kper, out totimd);
                    }
                    text = new string(br.ReadChars(16)).Trim();
                    ncol = br.ReadInt32();
                    nrow = br.ReadInt32();
                    ilay = br.ReadInt32();

                    /*
                    Console.WriteLine("kstp:   " + kstp);
                    Console.WriteLine("kper:   " + kper);
                    Console.WriteLine("pertim: " + pertim);
                    Console.WriteLine("totim:  " + totim);
                    Console.WriteLine("text:   " + text);
                    Console.WriteLine("ncol:   " + ncol);
                    Console.WriteLine("nrow:   " + nrow);
                    Console.WriteLine("ilay:   " + ilay);
                     */

                    // If past the last stress period, or if past the last timstep in the last stress period, flag as done.
                    if (kper > stressPeriodEndOneBased || (kper == stressPeriodEndOneBased && kstp > timestepEndOneBased))
                    {
                        done = true;
                    }

                    // If in the specified range, read the values and find the max.
                    else if (ilay == layerOneBased && kper >= stressPeriodStartOneBased && kstp >= timestepStartOneBased &&
                        kper <= stressPeriodEndOneBased && kstp <= timestepEndOneBased && text == dataDescriptor)
                    {
                        // Initialize the maximum value. We will be using a NaN-aware max, so this will be replaced with the first non-NaN value.
                        float maxValue = float.NaN;

                        // Check all values in the current layer.
                        switch (precision)
                        {
                            case 1:
                                for (j = 0; j < nrow; j++)
                                {
                                    for (i = 0; i < ncol; i++)
                                    {
                                        value = br.ReadSingle();
                                        if (dataAreConcentrations)
                                        {
                                            if (value == cinact)
                                            {
                                                value = float.NaN;
                                            }
                                        }
                                        else
                                        {
                                            if (value == hnoflo || value == hdry)
                                            {
                                                value = float.NaN;
                                            }
                                        }
                                        maxValue = MathUtil.NanAwareMax(value, maxValue);
                                    }
                                }
                                break;
                            case 2:
                                for (j = 0; j < nrow; j++)
                                {
                                    for (i = 0; i < ncol; i++)
                                    {
                                        tempDouble = br.ReadDouble();
                                        value = Convert.ToSingle(tempDouble);
                                        if (dataAreConcentrations)
                                        {
                                            if (tempDouble == cinactd)
                                            {
                                                value = float.NaN;
                                            }
                                        }
                                        else
                                        {
                                            if (tempDouble == hnoflod || tempDouble == hdryd)
                                            {
                                                value = float.NaN;
                                            }
                                        }
                                        maxValue = MathUtil.NanAwareMax(value, maxValue);
                                    }
                                }
                                break;
                        }

                        // Add the value to the list.
                        values.Add(maxValue);
                    }

                    // Otherwise, advance in the stream.
                    else
                    {
                        br.BaseStream.Position += ncol * nrow * 4;
                    }
                }
            }

            catch { }

            // Close the file.
            if (br != null)
            {
                try
                {
                    br.Close();
                }
                catch { }
            }

            // Return the result.
            return values.ToArray();
        }

        public static List<NameFileEntry> GetNameFileEntries(string nameFile)
        {
            List<NameFileEntry> filesList = new List<NameFileEntry>();
            string[] contents = FileUtil.GetFileContents(nameFile);
            for (int i = 0; i < contents.Length; i++)
            {
                if (!contents[i].StartsWith("#"))
                {
                    filesList.Add(new NameFileEntry(contents[i]));
                }
            }
            return filesList;
        }

    }
}
