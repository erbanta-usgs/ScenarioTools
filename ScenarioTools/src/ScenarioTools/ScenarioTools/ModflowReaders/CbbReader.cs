using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Drawing;

using ScenarioTools.DataClasses;

namespace ScenarioTools.ModflowReaders
{
    public class CbbReader
    {
        const int ICODE_0_NOT_SPECIFIED = 0;
        const int ICODE_1_GRID_3D = 1;
        const int ICODE_2_SIMPLE_LIST = 2;
        const int ICODE_3_GRID_2D_PAIR = 3;
        const int ICODE_4_GRID_2D_SINGLE = 4;
        const int ICODE_5_LIST_WITH_AUX = 5;

        public static string[][] GetCbbAvailableRecords(string file)
        {
            // Make the list for the layer information.
            List<string[]> layerDescriptions = new List<string[]>();

            // If the specified file doesn't exist, add a single entry to the list that indicates such and return.
            if (!File.Exists(file))
            {
                layerDescriptions.Add(new string[] { "No Layers -- Specified File Does Not Exist", "", "", "" });
                return layerDescriptions.ToArray();
            }

            // Define the precision model
            bool isCompact;
            int precision = PrecisionHelpers.BudgetFilePrecision(file, out isCompact);
            if (precision < 1 || precision > 2)
            {
                layerDescriptions.Add(new string[] { "The precision model of file ", file, " could not be determined." });
                return layerDescriptions.ToArray();
            }
            if (!isCompact)
            {
                layerDescriptions.Add(new string[] { "File ", file, " is not a compact cell-by-cell budget file." });
                return layerDescriptions.ToArray();
            }
            int realSize = precision * 4;
            int intSize = 4;

            BinaryReader br = null;
            try
            {
                // Open the file.
                br = new BinaryReader(File.Open(file, FileMode.Open));

                // Read the entire stream.
                long filePosition = 0;
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    // Read and show the first two values.
                    int timestep = br.ReadInt32();
                    int stressPeriod = br.ReadInt32();

                    // Read the 16-character identifier.
                    string identifier = new string(br.ReadChars(16)).Trim();

                    // The next values are the number of columns, rows, and number of layers.
                    int nCols = br.ReadInt32();
                    int nRows = br.ReadInt32();
                    int nLayersSigned = br.ReadInt32();
                    int nLayers = Math.Abs(nLayersSigned);

                    // Check the file position.
                    filePosition += 16 + 5 * intSize;
                    Debug.Assert(filePosition == br.BaseStream.Position);

                    // Determine the value of the write method (ICODE). If NLAY is positive, then IMETH will not be specified. Otherwise, it will
                    // be the next value.
                    int writeMethod = ICODE_0_NOT_SPECIFIED;

                    // If ICODE was specified, read the DELT, PERTIM, and TOTIM variables.
                    if (nLayersSigned < 0)
                    {
                        writeMethod = br.ReadInt32();
                        switch (precision)
                        {
                            case 1:
                                float delt = br.ReadSingle();
                                float pertim = br.ReadSingle();
                                float totim = br.ReadSingle();
                                break;
                            case 2:
                                double deltd = br.ReadDouble();
                                double pertimd = br.ReadDouble();
                                double totimd = br.ReadDouble();
                                break;
                        }
                        filePosition += intSize + 3 * realSize;
                    }

                    // Build a string describing the layer and add it to the list.
                    string[] layerDescription = { identifier, stressPeriod + "", timestep + "" };
                    layerDescriptions.Add(layerDescription);

                    // This is the case for a single 3D grid dataset (ICODE = 0 or 1).
                    if (writeMethod == ICODE_0_NOT_SPECIFIED || writeMethod == ICODE_1_GRID_3D)
                    {
                        // Skip the 3-dimensional grid.
                        br.BaseStream.Position += nLayers * nCols * nRows * realSize;

                        // Check the file position.
                        filePosition += nLayers * nCols * nRows * realSize;
                        Debug.Assert(filePosition == br.BaseStream.Position);
                    }

                    // This is the case for a simple list (no aux variables).
                    else if (writeMethod == ICODE_2_SIMPLE_LIST)
                    {
                        // Read an integer to determine the data size.
                        int numToRead = br.ReadInt32();

                        // Skip over the data (1 int and 1 float per record).
                        br.BaseStream.Position += numToRead * (intSize + realSize);

                        // Update file position
                        filePosition += 4 + numToRead * (intSize + realSize);
                    }
                    else if (writeMethod == ICODE_3_GRID_2D_PAIR)
                    {
                        // Skip over both grids (one integer array and one real array).
                        br.BaseStream.Position += nCols * nRows * (intSize + realSize);

                        // Update file position
                        filePosition += nCols * nRows * (intSize + realSize);
                    }
                    else if (writeMethod == ICODE_4_GRID_2D_SINGLE)
                    {
                        // Skip over the grid.
                        br.BaseStream.Position += nCols * nRows * realSize;

                        // Update file position
                        filePosition += nCols * nRows * realSize;
                    }
                    else if (writeMethod == ICODE_5_LIST_WITH_AUX)
                    {
                        // Determine the number of auxiliary variables.
                        int numAuxVariables = br.ReadInt32() - 1;
                        filePosition += 4;

                        // Read the auxiliary variable identifiers.
                        for (int i = 0; i < numAuxVariables; i++)
                        {
                            string auxVariableName = new string(br.ReadChars(16));
                            filePosition += 16;
                        }

                        // Determine the number of records.
                        int numToRead = br.ReadInt32();
                        filePosition += 4;

                        // Skip over the data (1 int and (1 + numAux) floats per record).
                        br.BaseStream.Position += numToRead * (intSize + (1 + numAuxVariables) * realSize);
                        filePosition += numToRead * (intSize + (1 + numAuxVariables) * realSize);
                    }
                }
            }
            catch { }

            if (br != null)
            {
                try
                {
                    // Close the file.
                    br.Close();
                }
                catch { }
            }

            // Return the layer descriptions as an array.
            return layerDescriptions.ToArray();
        }

        private static Dictionary<string, string[]> cbbHeaders = new Dictionary<string, string[]>();

        public static string[] GetCbbHeaders(string file, int precision, out int numLayers)
        {
            numLayers = 0;

            // Make the list for the headers.
            List<string> headers = new List<string>();

            int realSize = precision * 4;
            int intSize = 4;

            BinaryReader br = null;
            if (File.Exists(file) && !(precision < 1 || precision > 2))
            {
                try
                {
                    // Open the file.
                    br = new BinaryReader(File.Open(file, FileMode.Open));
                    long filePosition = 0;

                    // Read the entire stream.
                    int startStressPeriod = -1;
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        // Read and show the first two values.
                        int timestep = br.ReadInt32();
                        int stressPeriod = br.ReadInt32();

                        // Read the 16-character identifier.
                        string identifier = new string(br.ReadChars(16)).Trim();

                        // If the identifier is not contained in the list, add it.
                        if (!headers.Contains(identifier))
                        {
                            headers.Add(identifier);
                            //Console.WriteLine("adding " + identifier);
                        }

                        // The next values are the number of columns, rows, and number of layers.
                        int nCols = br.ReadInt32();
                        int nRows = br.ReadInt32();
                        int nLayersSigned = br.ReadInt32();
                        int nLayers = Math.Abs(nLayersSigned);
                        numLayers = nLayers;
                        filePosition += 3 * intSize;

                        // Determine the value of the write method (ICODE). If NLAY is positive, 
                        // then ICODE will not be specified. Otherwise, it will be the next value.
                        int writeMethod;
                        if (nLayersSigned > 0)
                        {
                            writeMethod = ICODE_0_NOT_SPECIFIED;
                        }
                        else
                        {
                            writeMethod = br.ReadInt32();
                            filePosition += 4;
                        }
                        
                        //Debug.Assert(filePosition == br.BaseStream.Position);

                        // If ICODE was specified, read the DELT, PERTIM, and TOTIM variables.
                        if (writeMethod != ICODE_0_NOT_SPECIFIED)
                        {
                            switch (precision)
                            {
                                case 1:
                                    float delt = br.ReadSingle();
                                    float pertim = br.ReadSingle();
                                    float totim = br.ReadSingle();
                                    break;
                                case 2:
                                    double deltd = br.ReadDouble();
                                    double pertimd = br.ReadDouble();
                                    double totimd = br.ReadDouble();
                                    break;
                            }
                            filePosition += 3 * realSize;
                        }
                        // This is the case for a single 3D grid dataset (ICODE = 0 or 1).
                        if (writeMethod == ICODE_0_NOT_SPECIFIED || writeMethod == ICODE_1_GRID_3D)
                        {
                            // Skip the 3-dimensional grid.
                            br.BaseStream.Position += nLayers * nCols * nRows * realSize;
                            filePosition += nLayers * nCols * nRows * realSize;
                        }

                        // This is the case for a simple list (no aux variables).
                        else if (writeMethod == ICODE_2_SIMPLE_LIST)
                        {
                            // Read an integer to determine the data size.
                            int numToRead = br.ReadInt32();

                            // Skip over the data (1 int and 1 float per record).
                            br.BaseStream.Position += numToRead * (intSize + realSize);

                            filePosition += intSize + numToRead * (intSize + realSize);
                        }
                        else if (writeMethod == ICODE_3_GRID_2D_PAIR)
                        {
                            // Skip over both grids (one integer array and one real array).
                            br.BaseStream.Position += nCols * nRows * (intSize + realSize);
                            filePosition += nCols * nRows * (intSize + realSize);
                        }
                        else if (writeMethod == ICODE_4_GRID_2D_SINGLE)
                        {
                            // Skip over the grid.
                            br.BaseStream.Position += nCols * nRows * realSize;
                            filePosition += nCols * nRows * realSize;
                        }
                        else if (writeMethod == ICODE_5_LIST_WITH_AUX)
                        {
                            // Determine the number of auxiliary variables.
                            int numAuxVariables = br.ReadInt32() - 1;
                            filePosition += intSize;

                            // Read the auxiliary variable identifiers.
                            for (int i = 0; i < numAuxVariables; i++)
                            {
                                string auxVariableName = new string(br.ReadChars(16));
                            }
                            filePosition += numAuxVariables * 16;

                            // Determine the number of records.
                            int numToRead = br.ReadInt32();
                            filePosition += intSize;

                            // Skip over the data (1 int and (1 + numAux) floats per record).
                            br.BaseStream.Position += numToRead * (intSize + (1 + numAuxVariables) * realSize);
                            filePosition += numToRead * (intSize + (1 + numAuxVariables) * realSize);
                        }

                        startStressPeriod = stressPeriod;
                    }

                    // Set these as the cache in the headers dictionary.
                    cbbHeaders.Add(file.ToLower(), headers.ToArray());
                }
                catch
                {
                    // If there was a problem, check if there is a cache in the headers dictionary.
                    if (cbbHeaders.ContainsKey(file.ToLower()))
                    {
                        headers = new List<string>(cbbHeaders[file.ToLower()]);
                    }
                }

                if (br != null)
                {
                    try
                    {
                        // Close the file.
                        br.Close();
                    }
                    catch { }
                }
            }

            // Return the headers as an array.
            return headers.ToArray();
        }
        
        public static GeoMap GetMapFromCbb(string file, string identifier, int stressPeriodOneBased, 
            int timestepOneBased, int startLayerOneBased, int endLayerOneBased, 
            out string[] resultMessage, int precision, bool convertFlowToFlux)
        {
            // If the file does not exist, state so in the result message and return null.
            if (!File.Exists(file))
            {
                resultMessage = new string[] { "The file path,", file, " is not valid." };
                return null;
            }

            // Define the precision model
            if (precision < 1 || precision > 2)
            {
                resultMessage = new string[] { "The precision model of file ", file, " could not be determined." };
                return null;
            }
            int realSize = precision * 4;
            int intSize = 4;

            // These are the flags for traking which input parameters are valid.
            bool identifierValid = false;
            bool stressPeriodValid = false;
            bool timestepValid = false;
            bool layerValid = false;

            // Trim and convert the identifier to lower case.
            identifier = identifier.Trim().ToLower();

            // Open the file and perform an initial comparison on the stream location.
            BinaryReader br = null;

            // Read the file.
            float[,] result = null;
            bool pastSpecifiedStressPeriod = false;
            try
            {
                // This index is used in assertions to ensure that the proper location is maintained on the file.
                long filePosition = 0;

                // Open the file.
                br = new BinaryReader(File.Open(file, FileMode.Open));
                Debug.Assert(filePosition == br.BaseStream.Position);

                // Read the stream until the indicated dataset is found.
                while (br.BaseStream.Position < br.BaseStream.Length && result == null && !pastSpecifiedStressPeriod)
                {
                    // The first two values are the timestep and stress period.
                    int currentTimestep = br.ReadInt32();
                    int currentStressPeriod = br.ReadInt32();

                    // Next is the 16-character identifier.
                    string currentIdentifier = new string(br.ReadChars(16)).Trim().ToLower();

                    // The next values are the number of columns, rows, and layers.
                    int nCols = br.ReadInt32();
                    int nRows = br.ReadInt32();
                    int nLayersSigned = br.ReadInt32();
                    int nLayers = Math.Abs(nLayersSigned);

                    // Advance the file position tracker to account for the basic header and check.
                    filePosition += 16 + 5 * intSize;
                    Debug.Assert(filePosition == br.BaseStream.Position);

                    // Update the data flags.
                    if (currentIdentifier.Equals(identifier))
                    {
                        identifierValid = true;
                    }
                    if (currentStressPeriod == stressPeriodOneBased)
                    {
                        stressPeriodValid = true;
                    }
                    if (currentTimestep == timestepOneBased)
                    {
                        timestepValid = true;
                    }
                    if (startLayerOneBased > 0 && startLayerOneBased <= nLayers)
                    {
                        if (endLayerOneBased > 0 && endLayerOneBased <= nLayers)
                        {
                            if (startLayerOneBased <= endLayerOneBased)
                            {
                                layerValid = true;
                            }
                        }
                    }

                    // Determine whether the stress period is past the specified stress period. If so, flag so we can return early.
                    if (currentStressPeriod > stressPeriodOneBased)
                    {
                        pastSpecifiedStressPeriod = true;
                    }

                    // Determine the value of the write method (ICODE). If NLAY is positive, 
                    // then ICODE will not be specified. Otherwise, it will be the next value.
                    int writeMethod;
                    if (nLayersSigned > 0)
                    {
                        writeMethod = ICODE_0_NOT_SPECIFIED;
                    }
                    else
                    {
                        writeMethod = br.ReadInt32();
                        filePosition += intSize;
                    }
                    // Check the file position.
                    Debug.Assert(filePosition == br.BaseStream.Position);

                    // Determine whether this is the dataset we want to return 
                    // (iff the identifier, timestep, and stress period all match).
                    bool selectedDataset =
                        currentIdentifier.Trim().ToLower().Equals(identifier) &&
                        (currentStressPeriod == stressPeriodOneBased) &&
                        (currentTimestep == timestepOneBased) //&&
                        //(startLayerOneBased > 0 && startLayerOneBased <= nLayers)
                        ;

                    // If ICODE was specified, read the DELT, PERTIM, and TOTIM variables.
                    if (writeMethod != ICODE_0_NOT_SPECIFIED)
                    {
                        switch (precision)
                        {
                            case 1:
                                float delt = br.ReadSingle();
                                float pertim = br.ReadSingle();
                                float totim = br.ReadSingle();
                                break;
                            case 2:
                                double deltd = br.ReadDouble();
                                double pertimd = br.ReadDouble();
                                double totimd = br.ReadDouble();
                                break;
                        }

                        // Check the file position.
                        filePosition += 3 * realSize;
                        Debug.Assert(filePosition == br.BaseStream.Position);
                    }

                    // This is the case for a single 3D grid dataset (ICODE = 0 or 1).
                    if (writeMethod == ICODE_0_NOT_SPECIFIED || writeMethod == ICODE_1_GRID_3D)
                    {
                        if (selectedDataset)
                        {
                            // If this is the selected dataset, read it.
                            result = readGrid(br, nCols, nRows, nLayers, startLayerOneBased, endLayerOneBased, precision);
                        }
                        else
                        {
                            // Otherwise, skip over the data.
                            br.BaseStream.Position += nLayers * nCols * nRows * realSize;
                        }

                        // Check the file position.
                        filePosition += nLayers * nCols * nRows * realSize;
                        Debug.Assert(filePosition == br.BaseStream.Position);
                    }

                    // This is the case for a simple list (no aux variables).
                    else if (writeMethod == ICODE_2_SIMPLE_LIST)
                    {
                        // Read an integer to determine the data size.
                        int numToRead = br.ReadInt32();
                        filePosition += intSize;

                        // If this is the selected dataset, read it.
                        if (selectedDataset)
                        {
                            // If this is the selected dataset, read it.
                            result = readListIntoGrid(br, nCols, nRows, nLayers, startLayerOneBased, 
                                                      endLayerOneBased, numToRead, 0, precision);
                        }
                        else
                        {
                            // Otherwise, skip over the data (1 int and 1 float per record).
                            br.BaseStream.Position += numToRead * (intSize + realSize);
                        }

                        // Check the file position.
                        filePosition += numToRead * (intSize + realSize);
                        Debug.Assert(filePosition == br.BaseStream.Position);
                    }
                    else if (writeMethod == ICODE_3_GRID_2D_PAIR)
                    {
                        // If this is the selected dataset, read it.
                        if (selectedDataset)
                        {
                            // Skip the first grid (integer array).
                            br.BaseStream.Position += nCols * nRows * intSize;

                            // Read the second grid.
                            result = readGrid(br, nCols, nRows, 1, 1, precision);
                        }
                        else
                        {
                            // Otherwise, skip over both grids.
                            br.BaseStream.Position += nCols * nRows * (intSize + realSize);
                        }

                        // Check the file position.
                        filePosition += nCols * nRows * (intSize + realSize);
                        Debug.Assert(filePosition == br.BaseStream.Position);
                    }
                    else if (writeMethod == ICODE_4_GRID_2D_SINGLE)
                    {
                        // This method imposes the additional condition that the layer must be 1.
                        if (selectedDataset && startLayerOneBased == 1)
                        {
                            // Read the grid.
                            result = readGrid(br, nCols, nRows, 1, 1, precision);
                        }
                        else
                        {
                            // Otherwise, skip over the grid.
                            br.BaseStream.Position += nCols * nRows * realSize;
                        }

                        // Check the file position.
                        filePosition += nCols * nRows * realSize;
                        Debug.Assert(filePosition == br.BaseStream.Position);
                    }
                    else if (writeMethod == ICODE_5_LIST_WITH_AUX)
                    {
                        // Read the number of auxiliary variables (NAUX+1 in MODFLOW).
                        int numAuxVariables = br.ReadInt32() - 1;
                        filePosition += intSize;

                        // Read the auxiliary variable identifiers (AUXTXT array in MODFLOW). 
                        for (int i = 0; i < numAuxVariables; i++)
                        {
                            string auxVariableName = new string(br.ReadChars(16));
                        }
                        filePosition += numAuxVariables * 16;

                        // Determine the number of records (NLIST in MODFLOW)
                        int numToRead = br.ReadInt32();
                        filePosition += intSize;

                        if (selectedDataset)
                        {
                            // Read the data list and populate array elements.
                            result = readListIntoGrid(br, nCols, nRows, nLayers, startLayerOneBased, 
                                                      endLayerOneBased, numToRead, numAuxVariables, precision);
                        }
                        else
                        {
                            // Skip over the data (1 int and (1 + numAuxVariables) floats per record).
                            br.BaseStream.Position += numToRead * (intSize + (1 + numAuxVariables) * realSize);
                        }
                        filePosition += numToRead * (intSize + (1 + numAuxVariables) * realSize);

                        // Check the file position.
                        Debug.Assert(filePosition == br.BaseStream.Position);
                    }
                }
            }
            catch { }

            if (br != null)
            {
                try
                {
                    // Close the file.
                    br.Close();
                }
                catch { }
            }

            // Make the result message.
            // Ned TODO: revise to work with endLayerOneBased
            resultMessage = new string[] {
                "The specified identifier (" + identifier + ") is " + (identifierValid ? "" : "not ") + "valid.",
                "The specified stress period (" + stressPeriodOneBased + ") is " + (stressPeriodValid ? "" : "not ") + "valid.",
                "The specified timestep (" + timestepOneBased + ") is " + (timestepValid ? "" : "not ") + "valid.",
                "The specified layer (" + startLayerOneBased + ") is " + (layerValid ? "" : "not ") + "valid."
            };

            // Blank inactive areas
            if (GlobalStaticVariables.BlankingMode != MapEnums.BlankingMode.None)
            {
            }

            // Return the result as a GeoMap.
            if (ScenarioTools.Spatial.StaticObjects.Grid != null)
            {
                return GeoMap.GridMap(result, ScenarioTools.Spatial.StaticObjects.Grid, convertFlowToFlux);
            }
            else
            {
                return GeoMap.FixedGridMap(result, ScenarioTools.WorkspaceUtil.GetModelGridExtent(), convertFlowToFlux);
            }
        }
        private static float[,] readListIntoGrid(BinaryReader br, int nCols, int nRows, 
            int nLayers, int layerOneBased, int numToRead, int numAuxVariables, int precision)
        {
            // Make the result grid and populate with NaNs.
            float[,] values = new float[nRows, nCols];
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    values[i, j] = float.NaN;
                }
            }

            // Define the precision model
            if (precision < 1 || precision > 2)
            {
                return values;
            }
            int realSize = precision * 4;

            // Read the values into the array.
            for (int i = 0; i < numToRead; i++)
            {
                int cell = br.ReadInt32();
                float value = float.NaN;
                switch (precision)
                {
                    case 1:
                        value = br.ReadSingle();
                        break;
                    case 2:
                        value = Convert.ToSingle(br.ReadDouble());
                        break;
                }                
                int columnOneBased = cell % nCols;
                cell = (cell - columnOneBased) / nCols;   // This gives us (K - 1) * NR + (I - 1)
                int rowZeroBased = cell % nRows;
                int layerZeroBased = (cell - rowZeroBased) / nRows;
                if ((layerZeroBased + 1) == layerOneBased)
                {
                    values[rowZeroBased, columnOneBased - 1] = value;
                }

                // Skip over any auxiliary variable values
                br.BaseStream.Position += numAuxVariables * realSize;
            }

            // Return the result.
            return values;
        }
        private static float[,] readListIntoGrid(BinaryReader br, int nCols, int nRows,
            int nLayers, int startLayerOneBased, int endLayerOneBased, int numToRead, 
            int numAuxVariables, int precision)
        {
            // Make the result grid and populate with NaNs.
            float[,] values = new float[nRows, nCols];
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    values[i, j] = float.NaN;
                }
            }

            // Define the precision model
            if (precision < 1 || precision > 2)
            {
                return values;
            }
            int realSize = precision * 4;

            // Read the values into the array.
            for (int i = 0; i < numToRead; i++)
            {
                int cell = br.ReadInt32();
                float value = float.NaN;
                switch (precision)
                {
                    case 1:
                        value = br.ReadSingle();
                        break;
                    case 2:
                        value = Convert.ToSingle(br.ReadDouble());
                        break;
                }
                int columnOneBased = cell % nCols;
                cell = (cell - columnOneBased) / nCols;   // This gives us (K - 1) * NR + (I - 1)
                int rowZeroBased = cell % nRows;
                int layerZeroBased = (cell - rowZeroBased) / nRows;
                int layerOneBased = layerZeroBased + 1;
                if (layerOneBased >= startLayerOneBased & layerOneBased <= endLayerOneBased)
                {
                    if (float.IsNaN(values[rowZeroBased, columnOneBased - 1]))
                    {
                        values[rowZeroBased, columnOneBased - 1] = 0.0f;
                    }
                    values[rowZeroBased, columnOneBased - 1] += value;
                }

                // Skip over any auxiliary variable values
                br.BaseStream.Position += numAuxVariables * realSize;
            }

            // Return the result.
            return values;
        }
        private static float[,] readGrid(BinaryReader br, int nCols, int nRows, 
            int nLayers, int layer, int precision)
        {
            int realSize = precision * 4;

            // If the layer is invalid, return null.
            if (layer < 1 | layer > nLayers)
            {
                return null;
            }

            // Skip over the appropriate number of layers to get to the specified layer.
            for (int i = 1; i < layer; i++)
            {
                br.BaseStream.Position += nCols * nRows * realSize;
            }

            // Make the array and populate it from the binary reader.
            float[,] values = new float[nRows, nCols];
            double temp;
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    switch (precision)
                    {
                        case 1:
                            values[i, j] = br.ReadSingle();
                            break;
                        case 2:
                            temp = br.ReadDouble();
                            values[i, j] = Convert.ToSingle(temp);
                            break;
                    }
                }
            }

            // Skip over the appropriate number of layers past the specified layer.
            for (int i = layer; i < nLayers; i++)
            {
                br.BaseStream.Position += nCols * nRows * realSize;
            }

            // Return the result.
            return values;
        }
        private static float[,] readGrid(BinaryReader br, int nCols, int nRows,
            int nLayers, int layerStart, int layerEnd, int precision)
        {
            int realSize = precision * 4;

            // If start or end layer is invalid, return null.
            if (layerStart < 1 | layerStart > nLayers)
            {
                return null;
            }
            if (layerEnd < 1 || layerEnd > nLayers)
            {
                return null;
            }
            if (layerStart > layerEnd)
            {
                return null;
            }

            // Skip over the appropriate number of layers to get to the first layer of interest.
            for (int i = 1; i < layerStart; i++)
            {
                br.BaseStream.Position += nCols * nRows * realSize;
            }

            // Make the array and populate it from the binary reader.
            float[,] values = new float[nRows, nCols];
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    values[i, j] = 0.0f;
                }
            }
            double temp;

            // Accumulate values by row and column for all layers of interest
            for (int k = layerStart; k <= layerEnd; k++)
            {
                for (int i = 0; i < nRows; i++)
                {
                    for (int j = 0; j < nCols; j++)
                    {
                        switch (precision)
                        {
                            case 1:
                                values[i, j] = values[i, j] + br.ReadSingle();
                                break;
                            case 2:
                                temp = br.ReadDouble();
                                values[i, j] = values[i, j] + Convert.ToSingle(temp);
                                break;
                        }
                    }
                }
            }

            // Skip over the appropriate number of layers past the specified layer.
            for (int i = layerEnd; i < nLayers; i++)
            {
                br.BaseStream.Position += nCols * nRows * realSize;
            }

            // Return the result.
            return values;
        }
        public void getCellColumnRowLayerFromIndex(int cellIndex, int nColumns, int nRows, out int columnOneBased, out int rowOneBased, 
            out int layerOneBased)
        {
            columnOneBased = cellIndex % nColumns;
            cellIndex = (cellIndex - columnOneBased) / nColumns;   // This gives us (K - 1) * NR + (I - 1)
            int rowZeroBased = cellIndex % nRows;
            int layerZeroBased = (cellIndex - rowZeroBased) / nRows;

            rowOneBased = rowZeroBased + 1;
            layerOneBased = layerZeroBased + 1;
        }

        public static TimeSeries GetPointTimeSeriesFromCbb(string file, string identifier, 
            int columnOneBased, int rowOneBased, int layerOneBased, out string[] resultMessage, 
            TemporalReference temporalReference)
        {
            // If the temporal reference is null, use the default temporal reference.
            if (temporalReference == null)
            {
                throw new Exception("temporalReference cannot be null in CbbReader.GetPointTimeSeriesFromCbb");
            }

            // If the file does not exist, state so in the result message and return null.
            if (!File.Exists(file))
            {
                resultMessage = new string[] { "The file path " + file + " is not valid." };
                return null;
            }

            // Define the precision model
            bool isCompact;
            int precision = PrecisionHelpers.BudgetFilePrecision(file, out isCompact);
            if (precision < 1 || precision > 2)
            {
                resultMessage = new string[] { "The precision model of file " + file + " could not be determined." };
                return null;
            }
            if (!isCompact)
            {
                resultMessage = new string[] { "File " + file + " is not a compact cell-by-cell budget file." };
                return null;
            }
            int realSize = precision * 4;
            int intSize = 4;

            // These are the flags for tracking which input parameters are valid.
            bool identifierValid = false;
            bool layerValid = false;

            // Trim and convert the identifier to lower case.
            identifier = identifier.Trim().ToLower();

            // Make the list for the values.
            List<TimeSeriesSample> values = new List<TimeSeriesSample>();

            // Frequently used variables
            double totimd = 0.0;

            // Read the file.
            BinaryReader br = null;
            try
            {
                // Open the file.
                br = new BinaryReader(File.Open(file, FileMode.Open));

                // This index is used in assertions to ensure that the proper location is maintained on the file.
                long filePosition = 0;
                Debug.Assert(filePosition == br.BaseStream.Position);

                // Read the stream until the specified stress period range is passed.
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    // The first two values are the timestep and stress period.
                    int currentTimestep = br.ReadInt32();
                    int currentStressPeriod = br.ReadInt32();

                    // Next is the 16-character identifier.
                    string currentIdentifier = new string(br.ReadChars(16)).Trim().ToLower();
                    //Console.WriteLine("id: " + currentIdentifier);

                    // The next values are the number of columns, rows, and layers.
                    int nCols = br.ReadInt32();
                    int nRows = br.ReadInt32();
                    int nLayersSigned = br.ReadInt32();
                    int nLayers = Math.Abs(nLayersSigned);

                    // Check the file position.
                    filePosition += 16 + 5 * intSize;
                    Debug.Assert(filePosition == br.BaseStream.Position);

                    // Determine the value of the write method (ICODE). If NLAY is positive, 
                    // then ICODE will not be specified. Otherwise, it will be the next value.
                    int writeMethod;
                    if (nLayersSigned > 0)
                    {
                        writeMethod = ICODE_0_NOT_SPECIFIED;
                    }
                    else
                    {
                        writeMethod = br.ReadInt32();
                        filePosition += intSize;
                    }
                    
                    // Check the file position.
                    Debug.Assert(filePosition == br.BaseStream.Position);

                    // Determine whether this is a valid dataset. 
                    // This is checking the identifier and the layer.
                    bool selectedDataset = currentIdentifier.Trim().ToLower().Equals(identifier) 
                        && (layerOneBased > 0 && layerOneBased <= nLayers);

                    // If ICODE was specified, read the DELT, PERTIM, and TOTIM variables.
                    if (writeMethod != ICODE_0_NOT_SPECIFIED)
                    {
                        switch (precision)
                        {
                            case 1:
                                float delt = br.ReadSingle();
                                float pertim = br.ReadSingle();
                                float totim = br.ReadSingle();
                                totimd = Convert.ToDouble(totim);
                                break;
                            case 2:
                                double deltd = br.ReadDouble();
                                double pertimd = br.ReadDouble();
                                totimd = br.ReadDouble();
                                break;
                        }

                        // Check the file position.
                        filePosition += 3 * realSize;
                        Debug.Assert(filePosition == br.BaseStream.Position);
                    }

                    // This is the case for a single 3D grid dataset (ICODE = 0 or 1).
                    if (writeMethod == ICODE_0_NOT_SPECIFIED || writeMethod == ICODE_1_GRID_3D)
                    {
                        if (selectedDataset)
                        {
                            // Get the value and add it to the list.
                            float value = readValueFromGrid(br, nCols, nRows, nLayers, 
                                columnOneBased, rowOneBased, layerOneBased, precision);
                            values.Add(new TimeSeriesSample(temporalReference.GetDate(totimd), value));
                        }
                        else
                        {
                            // Otherwise, skip over the data.
                            br.BaseStream.Position += nLayers * nCols * nRows * realSize;
                        }

                        // Check the file position.
                        filePosition += nLayers * nCols * nRows * realSize;
                        Debug.Assert(filePosition == br.BaseStream.Position);
                    }

                    // This is the case for a simple list (no aux variables).
                    else if (writeMethod == ICODE_2_SIMPLE_LIST)
                    {   
                        // Read an integer to determine the data size.
                        int numToRead = br.ReadInt32();

                        // If this is the selected dataset, read it.
                        if (selectedDataset)
                        {
                            // Read the value from the list.
                            float value = sumValuesForCellFromList(br, numToRead, nCols, nRows, 
                                layerOneBased, rowOneBased, columnOneBased, 0, 0, precision);

                            // Add the value to the list.
                            values.Add(new TimeSeriesSample(temporalReference.GetDate(totimd), value));
                        }

                        else
                        {
                            // Otherwise, skip over the data (1 int and 1 float per record).
                            br.BaseStream.Position += numToRead * (intSize + realSize);
                        }

                        // Check the file position.
                        filePosition += intSize + numToRead * (intSize + realSize);
                        Debug.Assert(filePosition == br.BaseStream.Position);
                    }
                    else if (writeMethod == ICODE_3_GRID_2D_PAIR)
                    {
                        // If this is the selected dataset, read it.
                        if (selectedDataset)
                        {
                            // Skip over the first grid (an integer array)
                            br.BaseStream.Position += nCols * nRows * intSize;

                            // Get value from the second grid.
                            float value = readValueFromGrid(br, nCols, nRows, 1, columnOneBased, rowOneBased, 1, precision);

                            // Add the value to the list.
                            values.Add(new TimeSeriesSample(temporalReference.GetDate(totimd), value));
                        }
                        else
                        {
                            // Otherwise, skip over both grids.
                            br.BaseStream.Position += nCols * nRows * (intSize + realSize);
                        }

                        // Check the file position.
                        filePosition += nCols * nRows * (intSize + realSize);
                        Debug.Assert(filePosition == br.BaseStream.Position);
                    }
                    else if (writeMethod == ICODE_4_GRID_2D_SINGLE)
                    {
                        // This write method imposes the additional condition that the layer must be 1.
                        if (selectedDataset && layerOneBased == 1)
                        {
                            // Read the value from the grid.
                            float value = readValueFromGrid(br, nCols, nRows, 1, columnOneBased, rowOneBased, 1, precision);

                            // Add the value to the list.
                            values.Add(new TimeSeriesSample(temporalReference.GetDate(totimd), value));
                        }
                        else
                        {
                            // Otherwise, skip over the grid.
                            br.BaseStream.Position += nCols * nRows * realSize;
                        }

                        // Check the file position.
                        filePosition += nCols * nRows * realSize;
                        Debug.Assert(filePosition == br.BaseStream.Position);
                    }
                    else if (writeMethod == ICODE_5_LIST_WITH_AUX)
                    {
                        // Read the number of auxiliary variables. 
                        int numAuxVariables = br.ReadInt32() - 1;

                        // Skip the auxiliary variable identifiers.
                        br.BaseStream.Position += numAuxVariables * 16;

                        // Read the number of records.
                        int numToRead = br.ReadInt32();

                        filePosition += 2 * intSize + numAuxVariables * 16;

                        // This is the special case for when this dataset has already been determined 
                        // to be the desired dataset. This is the case when the main (not auxiliary) 
                        // dataset has been specified.
                        if (selectedDataset)
                        {
                            // Read the value from the file.
                            float value = sumValuesForCellFromList(br, numToRead, nCols, nRows, layerOneBased, 
                                rowOneBased, columnOneBased, numAuxVariables, 0, precision);

                            // Add the value to the result list.
                            values.Add(new TimeSeriesSample(temporalReference.GetDate(totimd), value));
                        }
                        else
                        {
                            // Skip over the data (1 int and (1 + numAuxVariables) floats per record).
                            br.BaseStream.Position += numToRead * (intSize + (1 + numAuxVariables) * realSize);
                        }

                        // Check the file position.
                        filePosition += numToRead * (intSize + (1 + numAuxVariables) * realSize);
                        Debug.Assert(filePosition == br.BaseStream.Position);
                    }
                }
            }
            catch { }

            if (br != null)
            {
                try
                {
                    // Close the file.
                    br.Close();
                }
                catch { }
            }

            // Make the result message.
            resultMessage = new string[] {
                "The specified identifier (" + identifier + ") is " + (identifierValid ? "" : "not ") + "valid.",
                "The specified layer (" + layerOneBased + ") is " + (layerValid ? "" : "not ") + "valid."
            };

            // Return the result as an array.
            return new TimeSeries(values.ToArray());
        }

        public static TimeSeries GetTimeSeriesGroupFromCbb(string file, string identifier, 
            Point[] cells, int layerStartOneBased, int layerEndOneBased, out string[] resultMessage, 
            TemporalReference temporalReference, bool getAllCells)
        {
            // If the temporal reference is null, use the default temporal reference.
            if (temporalReference == null)
            {
                throw new Exception("temporalReference cannot be null in CbbReader.GetTimeSeriesGroupFromCbb");
            }

            // If the file does not exist, state so in the result message and return null.
            if (!File.Exists(file))
            {
                resultMessage = new string[] { "The file path,", file, ", is not valid." };
                return null;
            }

            // Define the precision model
            bool isCompact;
            int precision = PrecisionHelpers.BudgetFilePrecision(file, out isCompact);
            if (precision < 1 || precision > 2)
            {
                resultMessage = new string[] { "The precision model of file " + file + " could not be determined." };
                return null;
            }
            if (!isCompact)
            {
                resultMessage = new string[] { "File " + file + " is not a compact cell-by-cell budget file." };
                return null;
            }
            int realSize = precision * 4;
            int intSize = 4;

            // Trim and convert the identifier to lower case.
            identifier = identifier.Trim().ToLower();

            // Make the list for the time series samples obtained from the file.
            List<TimeSeriesSample> values = new List<TimeSeriesSample>();

            // Get an array of the cells in the order they will be encountered in the file.
            Point[] orderedCells = CellGroupProvider.InFileOrder(cells);

            // Make an array of the compact cell indices in the order they will be encountered in the file.
            int numLayers = layerEndOneBased - layerStartOneBased + 1;
            int[] orderedCellIndices = null;

            // Frequently used variables
            double totimd = 0.0;
            int currentTimestep = 0;
            int currentStressPeriod = 0;

            // Read the file.
            BinaryReader br = null;
            if (orderedCells.Length > 0 || getAllCells)
            {
                try
                {
                    // Open the file.
                    br = new BinaryReader(File.Open(file, FileMode.Open));

                    // This index is used in assertions to ensure that the proper location is maintained on the file.
                    long filePosition = 0;
                    Debug.Assert(filePosition == br.BaseStream.Position);

                    // Read to the end of the file.
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        // The first two values are the timestep and stress period.
                        currentTimestep = br.ReadInt32();
                        currentStressPeriod = br.ReadInt32();

                        // Next is the 16-character identifier.
                        string currentIdentifier = new string(br.ReadChars(16)).Trim().ToLower();

                        // The next values are the number of columns, rows, and layers.
                        int nCols = br.ReadInt32();
                        int nRows = br.ReadInt32();
                        int nLayersSigned = br.ReadInt32();
                        int nLayers = Math.Abs(nLayersSigned);
                        if (layerEndOneBased > nLayers)
                        {
                            resultMessage = new string[] { "Error: End Layer (" + layerEndOneBased.ToString() +
                                            ") exceeds the number of layers (" + nLayers.ToString() + ")" };
                            return null;
                        }
                        filePosition += 16 + 5 * intSize;

                        // If not yet made, make the array for the compact cell indices.
                        if (orderedCellIndices == null)
                        {
                            orderedCellIndices = new int[orderedCells.Length * numLayers];
                            for (int j = 0; j < numLayers; j++)
                            {
                                for (int i = 0; i < orderedCells.Length; i++)
                                {
                                    orderedCellIndices[i + j * orderedCells.Length] = computeCompactCellIndex(j + 
                                        layerStartOneBased, orderedCells[i].Y, orderedCells[i].X, nCols, nRows);
                                }
                            }
                        }

                        // Check the file position.
                        Debug.Assert(filePosition == br.BaseStream.Position);

                        // Determine the value of the write method (ICODE). If NLAY is positive, 
                        // then ICODE will not be specified. Otherwise, it will be the next value.
                        int writeMethod;
                        if (nLayersSigned > 0)
                        {
                            writeMethod = ICODE_0_NOT_SPECIFIED;
                        }
                        else
                        {
                            writeMethod = br.ReadInt32();
                            filePosition += intSize;
                        }

                        // Check the file position.
                        Debug.Assert(filePosition == br.BaseStream.Position);

                        // Determine whether this is a valid dataset. This is checking the identifier and the layer.
                        bool selectedDataset = currentIdentifier.Trim().ToLower().Equals(identifier);

                        // If ICODE was specified, read the DELT, PERTIM, and TOTIM variables.
                        if (writeMethod != ICODE_0_NOT_SPECIFIED)
                        {
                            switch (precision)
                            {
                                case 1:
                                    float delt = br.ReadSingle();
                                    float pertim = br.ReadSingle();
                                    float totim = br.ReadSingle();
                                    totimd = Convert.ToDouble(totim);
                                    break;
                                case 2:
                                    double deltd = br.ReadDouble();
                                    double pertimd = br.ReadDouble();
                                    totimd = br.ReadDouble();
                                    break;
                            }

                            // Check the file position.
                            filePosition += 3 * realSize;
                            Debug.Assert(filePosition == br.BaseStream.Position);
                        }

                        // This is the case for a single 3D grid dataset (ICODE = 0 or 1).
                        if (writeMethod == ICODE_0_NOT_SPECIFIED || writeMethod == ICODE_1_GRID_3D)
                        {
                            if (selectedDataset)
                            {
                                // Get the value from the grid and add it to the list.
                                float value;
                                if (getAllCells)
                                {
                                    value = readAndSumAllValuesFrom3DGrid(br, nCols, nRows, 
                                        nLayers, layerStartOneBased, layerEndOneBased, precision);
                                }
                                else
                                {
                                    value = readGroupValueFrom3DGrid(br, nCols, nRows, nLayers,
                                        layerStartOneBased, layerEndOneBased, orderedCells, precision);
                                }

                                // Assign totimd if it has not been read from binary file
                                if (writeMethod == ICODE_0_NOT_SPECIFIED)
                                {
                                    //totimd = ???
                                }

                                values.Add(new TimeSeriesSample(temporalReference.GetDate(totimd), value));
                            }
                            else
                            {
                                // Otherwise, skip over the data.
                                br.BaseStream.Position += nLayers * nCols * nRows * realSize;
                            }

                            // Check the file position.
                            filePosition += nLayers * nCols * nRows * realSize;
                            Debug.Assert(filePosition == br.BaseStream.Position);
                        }

                        // This is the case for a simple list (no aux variables).
                        else if (writeMethod == ICODE_2_SIMPLE_LIST)
                        {
                            // Read an integer to determine the data size.
                            int numToRead = br.ReadInt32();
                            filePosition += intSize;

                            // If this is the selected dataset, read it.
                            if (selectedDataset)
                            {
                                // Read the value from the list.
                                float value;
                                if (getAllCells)
                                {
                                    value = sumValuesForAllCellsFromList(br, nCols, nRows, numToRead, 
                                        0, layerStartOneBased, layerEndOneBased, precision);
                                }
                                else
                                {
                                    value = sumValuesForGroupFromList(br, numToRead, orderedCellIndices, 0, precision);
                                }

                                // Add the value to the list.
                                values.Add(new TimeSeriesSample(temporalReference.GetDate(totimd), value));
                            }

                            else
                            {
                                // Otherwise, skip over the data (1 int and 1 float per record).
                                br.BaseStream.Position += numToRead * (intSize + realSize);
                            }

                            // Check the file position.
                            filePosition += numToRead * (intSize + realSize);
                            Debug.Assert(filePosition == br.BaseStream.Position);
                        }
                        else if (writeMethod == ICODE_3_GRID_2D_PAIR)
                        {
                            // If this is the selected dataset, read it.
                            if (selectedDataset)
                            {
                                // Skip over the first grid.
                                br.BaseStream.Position += nCols * nRows * intSize;

                                // Get sum of values from the second grid.
                                float value;
                                if (getAllCells)
                                {
                                    value = readAndSumAllValuesFrom2DGrid(br, nCols, nRows, precision);
                                }
                                else
                                {
                                    value = readGroupValueFrom2DGrid(br, nCols, nRows, orderedCells, precision);
                                }

                                // Add the value to the list.
                                values.Add(new TimeSeriesSample(temporalReference.GetDate(totimd), value));
                            }
                            else
                            {
                                // Otherwise, skip over both grids.
                                br.BaseStream.Position += nCols * nRows * (intSize + realSize);
                            }

                            // Check the file position.
                            filePosition += nCols * nRows * (intSize + realSize);
                            Debug.Assert(filePosition == br.BaseStream.Position);
                        }
                        else if (writeMethod == ICODE_4_GRID_2D_SINGLE)
                        {
                            // If this is the selected dataset, read it.
                            if (selectedDataset)
                            {
                                // Get sum of values from the grid.
                                float value;
                                if (getAllCells)
                                {
                                    value = readAndSumAllValuesFrom2DGrid(br, nCols, nRows, precision);
                                }
                                else
                                {
                                    value = readGroupValueFrom2DGrid(br, nCols, nRows, orderedCells, precision);
                                }

                                // Add the value to the list.
                                values.Add(new TimeSeriesSample(temporalReference.GetDate(totimd), value));
                            }
                            else
                            {
                                // Otherwise, skip over the grid.
                                br.BaseStream.Position += nCols * nRows * realSize;
                            }

                            // Check the file position.
                            filePosition += nCols * nRows * realSize;
                            Debug.Assert(filePosition == br.BaseStream.Position);
                        }
                        else if (writeMethod == ICODE_5_LIST_WITH_AUX)
                        {
                            // Read the number of auxiliary variables. 
                            int numAuxVariables = br.ReadInt32() - 1;

                            // Skip the auxiliary variable identifiers.
                            br.BaseStream.Position += numAuxVariables * 16;

                            // Read the number of records.
                            int numToRead = br.ReadInt32();
                            filePosition += 2 * intSize + numAuxVariables * 16;

                            // This is the special case for when this dataset has already been determined to be the
                            // desired dataset. This is the case when the main (not auxiliary) dataset has been specified.
                            if (selectedDataset)
                            {

                                // Read the value from the file.
                                float value; 
                                if (getAllCells)
                                {
                                    value = 0.0f;
                                    value = sumValuesForAllCellsFromList(br, nCols, nRows, numToRead, 
                                        numAuxVariables, layerStartOneBased, layerEndOneBased, precision);
                                }
                                else
                                {
                                    value = sumValuesForGroupFromList(br, numToRead, orderedCellIndices, numAuxVariables, precision);
                                }

                                // Add the value to the result list.
                                values.Add(new TimeSeriesSample(temporalReference.GetDate(totimd), value));
                            }
                            else
                            {
                                // Skip over the data (1 int and (1 + numAuxVariables) floats per record).
                                br.BaseStream.Position += numToRead * (intSize + (1 + numAuxVariables) * realSize);
                            }
                            filePosition += numToRead * (intSize + (1 + numAuxVariables) * realSize);

                            // Check the file position.
                            Debug.Assert(filePosition == br.BaseStream.Position);
                        }
                    }
                }
                catch { }
            }

            if (br != null)
            {
                try
                {
                    // Close the file.
                    br.Close();
                }
                catch { }
            }

            // Make the result message.
            resultMessage = new string[] {
                //"The specified identifier (" + identifier + ") is " + (identifierValid ? "" : "not ") + "valid.",
                //"The specified stress period (" + stressPeriodOneBased + ") is " + (stressPeriodValid ? "" : "not ") + "valid.",
                //"The specified timestep (" + timestepOneBased + ") is " + (timestepValid ? "" : "not ") + "valid.",
                //"The specified layer (" + layerOneBased + ") is " + (layerValid ? "" : "not ") + "valid."
            };

            // Return the result as an array.
            return new TimeSeries(values.ToArray());
        }

        private static float readGroupValueFrom3DGrid(BinaryReader br, int nCols, int nRows, int nLayers, int startLayerOneBased,
            int endLayerOneBased, Point[] orderedCells, int precision)
        {
            int realSize = precision * 4;
            // Initialize the value for the grid.
            float value = 0.0f;

            // Skip the layers before the start layer.
            int numLayersToSkipBeginning = startLayerOneBased - 1;
            br.BaseStream.Position += numLayersToSkipBeginning * nRows * nCols * realSize;

            // Read from the start layer to the end layer.
            for (int i = startLayerOneBased; i <= endLayerOneBased; i++)
            {
                value += readGroupValueFrom2DGrid(br, nCols, nRows, orderedCells, precision);
            }

            // Skip the layers after the end layer.
            int numLayersToSkipEnd = nLayers - endLayerOneBased;
            br.BaseStream.Position += numLayersToSkipEnd * nRows * nCols * realSize;

            // Return the result.
            return value;
        }
        private static float readAndSumAllValuesFrom3DGrid(BinaryReader br, int nCols, int nRows, int nLayers, int startLayerOneBased,
            int endLayerOneBased, int precision)
        {
            int realSize = precision * 4;
            // Initialize the value for the grid.
            float value = 0.0f;

            // Skip the layers before the start layer.
            int numLayersToSkipBeginning = startLayerOneBased - 1;
            br.BaseStream.Position += numLayersToSkipBeginning * nRows * nCols * realSize;

            // Read from the start layer to the end layer.
            for (int i = startLayerOneBased; i <= endLayerOneBased; i++)
            {
                value += readAndSumAllValuesFrom2DGrid(br, nCols, nRows, precision);
            }

            // Skip the layers after the end layer.
            int numLayersToSkipEnd = nLayers - endLayerOneBased;
            br.BaseStream.Position += numLayersToSkipEnd * nRows * nCols * realSize;

            // Return the result.
            return value;
        }
        private static float readGroupValueFrom2DGrid(BinaryReader br, int nCols, int nRows, 
            Point[] orderedCells, int precision)
        {
            int realSize = precision * 4;
            // Initialize the sum value to 0.0.
            float sumValue = 0.0f;

            // Skip to the first cell.
            int numCellsToSkip = (orderedCells[0].Y - 1) * nCols + (orderedCells[0].X - 1);
            br.BaseStream.Position += numCellsToSkip * realSize;

            // Read the value and add it to the sum.
            float firstValue = float.NaN;
            switch (precision)
            {
                case 1:
                    firstValue = br.ReadSingle();
                    break;
                case 2:
                    firstValue = Convert.ToSingle(br.ReadDouble());
                    break;
            }

            sumValue += firstValue;

            // Read subsequent cells.
            for (int i = 1; i < orderedCells.Length; i++)
            {
                // Skip to the cell.
                numCellsToSkip = (orderedCells[i].Y - orderedCells[i - 1].Y) * nCols + (orderedCells[i].X - orderedCells[i - 1].X) - 1;
                br.BaseStream.Position += numCellsToSkip * realSize;

                // Read the value and add it to the sum.
                float value = float.NaN;
                switch (precision)
                {
                    case 1:
                        value = br.ReadSingle();
                        break;
                    case 2:
                        value = Convert.ToSingle(br.ReadDouble());
                        break;
                }
                sumValue += value;
            }

            // Skip to the end of this grid.
            Point lastCell = orderedCells[orderedCells.Length - 1];
            br.BaseStream.Position += (nCols * nRows - ((lastCell.Y - 1) * nCols + lastCell.X)) * realSize;

            // Return the result.
            return sumValue;
        }
        private static float readAndSumAllValuesFrom2DGrid(BinaryReader br, int nCols, int nRows, int precision)
        {
            int realSize = precision * 4;
            // Initialize the sum value to 0.0.
            float sumValue = 0.0f;
            float value = 0.0f;

            // Find number of cells in one model layer
            int numToRead = nCols * nRows;

            // Read value for each cell and add to sum.
            for (int i = 0; i < numToRead; i++)
            {
                // Read the value and add it to the sum.
                switch (precision)
                {
                    case 1:
                        value = br.ReadSingle();
                        break;
                    case 2:
                        value = Convert.ToSingle(br.ReadDouble());
                        break;
                }
                sumValue += value;
            }

            // Return the result.
            return sumValue;
        }
        public static TimeSeries GetTimeSeriesSumLayersFromCbb(string file, string identifier, 
            int layerStartOneBased, int layerEndOneBased, out string[] resultMessage, 
            TemporalReference temporalReference)
        {
            // If the temporal reference is null, use the default temporal reference.
            if (temporalReference == null)
            {
                throw new Exception("temporalReference cannot be null in CbbReader.GetTimeSeriesSumLayersFromCbb");
            }

            // If the file does not exist, state so in the result message and return null.
            if (!File.Exists(file))
            {
                resultMessage = new string[] { "The file path,", file, " is not valid." };
                return null;
            }

            // Define the precision model
            bool isCompact;
            int precision = PrecisionHelpers.BudgetFilePrecision(file, out isCompact);
            if (precision < 1 || precision > 2)
            {
                resultMessage = new string[] { "The precision model of file ", file, " could not be determined." };
                return null;
            }
            if (!isCompact)
            {
                resultMessage = new string[] { "File ", file, " is not a compact cell-by-cell budget file." };
                return null;
            }
            int realSize = precision * 4;
            int intSize = 4;

            // These are the flags for tracking which input parameters are valid.
            bool identifierValid = false;
            bool stressPeriodValid = false;
            bool timestepValid = false;
            bool columnValid = false;
            bool rowValid = false;
            bool layerValid = false;

            // Trim and convert the identifier to lower case.
            identifier = identifier.Trim().ToLower();

            // Make the list for the values.
            List<TimeSeriesSample> values = new List<TimeSeriesSample>();

            // Frequently used variables
            double totimd = 0.0;

            // Read the file.
            BinaryReader br = null;
            bool pastSpecifiedStressPeriods = false;
            try
            {
                // Open the file.
                br = new BinaryReader(File.Open(file, FileMode.Open));

                // This index is used in assertions to ensure that the proper location is maintained on the file.
                long filePosition = 0;
                Debug.Assert(filePosition == br.BaseStream.Position);

                // Read the stream until the specified stress period range is passed.
                while (br.BaseStream.Position < br.BaseStream.Length && !pastSpecifiedStressPeriods)
                {
                    // The first two values are the timestep and stress period.
                    int currentTimestep = br.ReadInt32();
                    int currentStressPeriod = br.ReadInt32();

                    // Next is the 16-character identifier.
                    string currentIdentifier = new string(br.ReadChars(16)).Trim().ToLower();
                    Console.WriteLine("id (" + currentStressPeriod + "): " + currentIdentifier);

                    // The next values are the number of columns, rows, and layers.
                    int nCols = br.ReadInt32();
                    int nRows = br.ReadInt32();
                    int nLayersSigned = br.ReadInt32();
                    int nLayers = Math.Abs(nLayersSigned);

                    // Check the file position.
                    filePosition += 16 + 5 * intSize;
                    Debug.Assert(filePosition == br.BaseStream.Position);

                    // Update the data flags.
                    /*if (currentIdentifier.Equals(identifier))
                    {
                        identifierValid = true;
                    }
                    if (currentStressPeriod == stressPeriodOneBased)
                    {
                        stressPeriodValid = true;
                    }
                    if (currentTimestep == timestepOneBased)
                    {
                        timestepValid = true;
                    }
                    if (layerOneBased > 0 && layerOneBased <= nLayers)
                    {
                        layerValid = true;
                    }*/

                    // Determine the value of the write method (ICODE). If NLAY is positive,
                    // then ICODE will not be specified. Otherwise, it will be the next value.
                    int writeMethod;
                    if (nLayersSigned > 0)
                    {
                        writeMethod = ICODE_0_NOT_SPECIFIED;
                    }
                    else
                    {
                        writeMethod = br.ReadInt32();
                        filePosition += intSize;
                    }

                    // Check the file position.
                    Debug.Assert(filePosition == br.BaseStream.Position);

                    // Determine whether this is a valid dataset. This is checking the identifier, 
                    // the time, and the layer.
                    bool selectedDataset = currentIdentifier.Trim().ToLower().Equals(identifier);

                    // If ICODE was specified, read the DELT, PERTIM, and TOTIM variables.
                    if (writeMethod != ICODE_0_NOT_SPECIFIED)
                    {
                        switch (precision)
                        {
                            case 1:
                                float delt = br.ReadSingle();
                                float pertim = br.ReadSingle();
                                float totim = br.ReadSingle();
                                totimd = Convert.ToDouble(totim);
                                break;
                            case 2:
                                double deltd = br.ReadDouble();
                                double pertimd = br.ReadDouble();
                                totimd = br.ReadDouble();
                                break;
                        }

                        // Check the file position.
                        filePosition += 3 * realSize;
                        Debug.Assert(filePosition == br.BaseStream.Position);
                    }

                    // This is the case for a single 3D grid dataset (ICODE = 0 or 1).
                    if (writeMethod == ICODE_0_NOT_SPECIFIED || writeMethod == ICODE_1_GRID_3D)
                    {
                        if (selectedDataset)
                        {
                            // Get the value and add it to the list.
                            float value = readSumOfGrids(br, nCols, nRows, nLayers, 
                                layerStartOneBased, layerEndOneBased, precision);
                            values.Add(new TimeSeriesSample(temporalReference.GetDate(totimd), value));
                        }
                        else
                        {
                            // Otherwise, skip over the data.
                            br.BaseStream.Position += nLayers * nCols * nRows * realSize;
                        }

                        // Check the file position.
                        filePosition += nLayers * nCols * nRows * realSize;
                        Debug.Assert(filePosition == br.BaseStream.Position);
                    }

                    // This is the case for a simple list (no aux variables).
                    else if (writeMethod == ICODE_2_SIMPLE_LIST)
                    {
                        // Read an integer to determine the data size.
                        int numToRead = br.ReadInt32();
                        filePosition += intSize;

                        // If this is the selected dataset, read it.
                        if (selectedDataset)
                        {
                            // Read the value from the list.
                            float value = readLayersSumFromList(br, numToRead, nCols, nRows,
                                layerStartOneBased, layerEndOneBased, 0, 0, precision);

                            // Add the value to the list.
                            values.Add(new TimeSeriesSample(temporalReference.GetDate(totimd), value));
                        }

                        else
                        {
                            // Otherwise, skip over the data (1 int and 1 float per record).
                            br.BaseStream.Position += numToRead * (intSize + realSize);
                        }

                        // Check the file position.
                        filePosition += numToRead * (intSize + realSize);
                        Debug.Assert(filePosition == br.BaseStream.Position);
                    }
                    else if (writeMethod == ICODE_3_GRID_2D_PAIR)
                    {
                        // If this is the selected dataset, read it.
                        if (selectedDataset)
                        {
                            // Skip over the first grid
                            br.BaseStream.Position += nCols * nRows * intSize;

                            // Get the value
                            float value = readSumOfGrids(br, nCols, nRows, 1, 1, 1, precision);

                            // Add the value to the list.
                            values.Add(new TimeSeriesSample(temporalReference.GetDate(totimd), value));
                        }
                        else
                        {
                            // Otherwise, skip over both grids.
                            br.BaseStream.Position += nCols * nRows * (intSize + realSize);
                        }

                        // Check the file position.
                        filePosition += nCols * nRows * (intSize + realSize);
                        Debug.Assert(filePosition == br.BaseStream.Position);
                    }
                    else if (writeMethod == ICODE_4_GRID_2D_SINGLE)
                    {
                        // This write method imposes the additional condition that the layer must be 1.
                        if (selectedDataset && layerStartOneBased <= 1)
                        {
                            // Get the value from the grid
                            float value = readSumOfGrids(br, nCols, nRows, 1, 1, 1, precision);

                            // Add the value to the list
                            values.Add(new TimeSeriesSample(temporalReference.GetDate(totimd), value));
                        }
                        else
                        {
                            // Otherwise, skip over the grid.
                            br.BaseStream.Position += nCols * nRows * realSize;
                        }

                        // Check the file positiion.
                        filePosition += nCols * nRows * realSize;
                        Debug.Assert(filePosition == br.BaseStream.Position);
                    }
                    else if (writeMethod == ICODE_5_LIST_WITH_AUX)
                    {
                        // Read the number of auxiliary variables. 
                        int numAuxVariables = br.ReadInt32() - 1;

                        // Skip the auxiliary variable identifiers.
                        br.BaseStream.Position += numAuxVariables * 16;

                        // Read the number of records.
                        int numToRead = br.ReadInt32();

                        filePosition += 2 * intSize + numAuxVariables * 16;

                        // This is the special case for when this dataset has already been 
                        // determined to be the desired dataset. This is the case when
                        // the main (not auxiliary) dataset has been specified.
                        if (selectedDataset)
                        {
                            // Read the value from the file.
                            float value = readLayersSumFromList(br, numToRead, nCols, nRows, layerStartOneBased, 
                                layerEndOneBased, numAuxVariables, 0, precision);

                            // Add the value to the result list.
                            values.Add(new TimeSeriesSample(temporalReference.GetDate(totimd), value));
                        }
                        else
                        {
                            // Skip over the data (1 int and (1 + numAuxVariables) floats per record).
                            br.BaseStream.Position += numToRead * (intSize + (1 + numAuxVariables) * realSize);
                        }

                        // Check the file position.
                        filePosition += numToRead * (intSize + (1 + numAuxVariables) * realSize);
                        Debug.Assert(filePosition == br.BaseStream.Position);
                    }
                }
            }
            catch { }

            if (br != null)
            {
                try
                {
                    // Close the file.
                    br.Close();
                }
                catch { }
            }

            // Make the result message.
            resultMessage = new string[] {
                "The specified identifier (" + identifier + ") is " + (identifierValid ? "" : "not ") + "valid.",
                //"The specified stress period (" + stressPeriodOneBased + ") is " + (stressPeriodValid ? "" : "not ") + "valid.",
                //"The specified timestep (" + timestepOneBased + ") is " + (timestepValid ? "" : "not ") + "valid.",
                //"The specified layer range (" + layerOneBased + ") is " + (layerValid ? "" : "not ") + "valid."
            };

            // Return the result as an array.
            return new TimeSeries(values.ToArray());
        }
        private static float readLayersSumFromList(BinaryReader br, int numToRead, int nCols, 
            int nRows, int layerStartOneBased, int layerEndOneBased, int numAuxVariables,
            int variableIndexZeroBased, int precision)
        {
            int realSize = precision * 4;

            // Calculate the desired cell minimum and maximum indices.
            int desiredCellIndexStart = (layerStartOneBased - 1) * nCols * nRows + 1;
            int desiredCellIndexEnd = (layerEndOneBased) * nCols * nRows;

            float value = 0.0f;

            // Read the records.
            for (int i = 0; i < numToRead; i++)
            {
                // Get the cell index.
                int cellIndex = br.ReadInt32();

                // If the cell index is the desired cell, read the value and flag that the value has been found.
                if (cellIndex >= desiredCellIndexStart && cellIndex <= desiredCellIndexEnd)
                {
                    switch (precision)
                    {
                        case 1:
                            value += br.ReadSingle();
                            break;
                        case 2:
                            value += Convert.ToSingle(br.ReadDouble());
                            break;
                    }
                }

                // Otherwise, skip over the value.
                else
                {
                    br.BaseStream.Position += realSize;
                }

                // Skip over any auxiliary variable values
                br.BaseStream.Position += numAuxVariables * realSize;
            }

            // Return the result.
            return value;
        }
        private static float sumValuesForCellFromList(BinaryReader br, int numToRead, int nCols, int nRows, int layerOneBased, 
            int rowOneBased, int columnOneBased, int numAuxVariables, int variableIndexZeroBased, int precision)
        {
            int realSize = precision * 4;

            // Calculate the desired cell index.
            int desiredCellIndex = computeCompactCellIndex(layerOneBased, rowOneBased, columnOneBased, nCols, nRows);

            float value = 0.0f;

            // Read the records in the list. When the cell of interest is found, add value to sum.
            int numRead = 0;
            while (numRead < numToRead)
            {
                // Get the cell index.
                int cellIndex = br.ReadInt32();

                // If the cell index is the desired cell, read the value and flag that the value has been found.
                if (cellIndex == desiredCellIndex)
                {
                    switch (precision)
                    {
                        case 1:
                            value += br.ReadSingle();
                            break;
                        case 2:
                            value += Convert.ToSingle(br.ReadDouble());
                            break;
                    }
                }

                // Otherwise, skip over the value.
                else
                {
                    br.BaseStream.Position += realSize;
                }

                // Skip over any auxiliary variable values
                br.BaseStream.Position += numAuxVariables * realSize;

                // Increment the counter of the number of values read.
                numRead++;
            }

            //// Skip the remaining unread values.
            //br.BaseStream.Position += (intSize + (1 + numAuxVariables) * realSize) * (numToRead - numRead);

            // Return the result.
            return value;
        }
        private static float sumValuesForGroupFromList(BinaryReader br, int numToRead, 
            int[] orderedCellIndices, int numAuxVariables, int precision)
        {
            int realSize = precision * 4;

            // Make a list of the ordered cell indices.
            List<int> orderedCellIndicesList = new List<int>(orderedCellIndices);

            // Start with zero for the value. All values from the group will be added to this group.
            float value = 0.0f;

            // Iterate through the list of cells.
            int numRead = 0;
            while (numRead < numToRead)
            {
                // Get the cell index for the next cell.
                int cellIndex = br.ReadInt32();

                // If the cell index is in the group, read the value.
                if (orderedCellIndicesList.Contains(cellIndex))
                {
                    switch (precision)
                    {
                        case 1:
                            value += br.ReadSingle();
                            break;
                        case 2:
                            value += Convert.ToSingle(br.ReadDouble());
                            break;
                    }
                }

                // Otherwise, skip over the value.
                else
                {
                    br.BaseStream.Position += realSize;
                }

                // Skip over any auxiliary variable values
                br.BaseStream.Position += numAuxVariables * realSize;

                // Increment the counter of the number of values read.
                numRead++;
            }

            // Return the result.
            return value;
        }
        private static float sumValuesForAllCellsFromList(BinaryReader br, int nCols, int nRows, int numToRead, 
            int numAuxVariables, int startLayerOneBased, int endLayerOneBased, int precision)
        {
            int realSize = precision * 4;
            int layer;

            // Start with zero for the value. All values from the group will be added to this value.
            float value = 0.0f;

            // Iterate through the list of cells.
            int numRead = 0;
            while (numRead < numToRead)
            {
                // Get the cell index for the next cell.
                int cellIndex = br.ReadInt32();

                // Find the layer number for the cell, based on the cell index
                layer = getLayerFromCellIndex(cellIndex, nCols, nRows);

                // If the cell index is in the indicated layer range, read the value.
                if (layer >= startLayerOneBased && layer <= endLayerOneBased)
                {
                    switch (precision)
                    {
                        case 1:
                            value += br.ReadSingle();
                            break;
                        case 2:
                            value += Convert.ToSingle(br.ReadDouble());
                            break;
                    }
                }

                // Otherwise, skip over the value.
                else
                {
                    br.BaseStream.Position += realSize;
                }

                // Skip over any auxiliary variable values
                br.BaseStream.Position += numAuxVariables * realSize;

                // Increment the counter of the number of values read.
                numRead++;
            }

            // Return the result.
            return value;
        }
        private static int computeCompactCellIndex(int layerOneBased, int rowOneBased, int columnOneBased, int nCols, int nRows)
        {
            return (layerOneBased - 1) * nCols * nRows + (rowOneBased - 1) * nCols + columnOneBased;
        }

        private static void getColumnRowLayerFromCellIndex(int cellIndex, int nCols, int nRows, out int columnOneBased, out int rowOneBased, 
            out int layerOneBased)
        {
            columnOneBased = cellIndex % nCols;
            cellIndex = (cellIndex - columnOneBased) / nCols;   // This gives us (K - 1) * NR + (I - 1)
            int rowZeroBased = cellIndex % nRows;
            int layerZeroBased = (cellIndex - rowZeroBased) / nRows;

            rowOneBased = rowZeroBased + 1;
            layerOneBased = layerZeroBased + 1;
        }
        private static int getLayerFromCellIndex(int cellIndex, int nCols, int nRows)
        {
            int columnOneBased = cellIndex % nCols;
            cellIndex = (cellIndex - columnOneBased) / nCols;   // This gives us (K - 1) * NR + (I - 1)
            int rowZeroBased = cellIndex % nRows;
            int layerZeroBased = (cellIndex - rowZeroBased) / nRows;
            return layerZeroBased + 1;
        }

        private static float readValueFromGrid(BinaryReader br, int nColumns, int nRows, int nLayers, int columnOneBased, int rowOneBased, 
            int layerOneBased, int precision)
        {
            int realSize = precision * 4;
            // If the column, row, or layer is invalid, return NaN.
            if (columnOneBased < 1 || columnOneBased > nColumns || rowOneBased < 1 || rowOneBased > nRows || layerOneBased < 1 || layerOneBased > nLayers)
            {
                // Skip over the grid.
                br.BaseStream.Position += nColumns * nRows * nLayers * realSize;

                // Return NaN.
                return float.NaN;
            }
            // Otherwise, return the appropriate value.
            else
            {
                // Skip over the appropriate number of cells.
                int numToSkip = ((columnOneBased - 1) + (rowOneBased - 1) * nColumns + (layerOneBased - 1) * nColumns * nRows) * realSize;
                br.BaseStream.Position += numToSkip;

                // Read the value.
                float value = float.NaN;
                switch (precision)
                {
                    case 1:
                        value = br.ReadSingle();
                        break;
                    case 2:
                        value = Convert.ToSingle(br.ReadDouble());
                        break;
                }
                

                // Skip over the remaining cells in the grid.
                br.BaseStream.Position += nColumns * nRows * nLayers * realSize - numToSkip - realSize;

                // Return the value.
                return value;
            }
        }
        private static float readSumOfGrids(BinaryReader br, int nColumns, int nRows, int nLayers, 
            int layerStartOneBased, int layerEndOneBased, int precision)
        {
            int realSize = precision * 4;

            // If the start layer is greater than the last layer or the end layer is less than the first layer, skip over the grid and return 0.
            if (layerStartOneBased > nLayers || layerEndOneBased < 1) 
            {
                br.BaseStream.Position += nColumns * nRows * nLayers * realSize;
                return 0.0f;
            }

            // If the end layer is greater than the last layer, set it to the last layer.
            if (layerEndOneBased > nLayers) 
            {
                layerEndOneBased = nLayers;
            }

            // If the start layer is greater than the end layer, set it to the end layer.
            if (layerStartOneBased > layerEndOneBased) 
            {
                layerStartOneBased = layerEndOneBased;
            }

            // Skip over the layers before the start layer.
            br.BaseStream.Position += nColumns * nRows * (layerStartOneBased - 1) * realSize;

            // Sum the values of the specified layers.
            float sum = 0.0f;
            int numToRead = nColumns * nRows * (layerEndOneBased - layerStartOneBased + 1);
            switch (precision)
            {
                case 1:
                    for (int i = 0; i < numToRead; i++)
                    {
                        sum += br.ReadSingle();
                    }
                    break;
                case 2:
                    for (int i = 0; i < numToRead; i++)
                    {
                        sum += Convert.ToSingle(br.ReadDouble());
                    }
                    break;
            }

            // Skip over the layers after the end layer.
            br.BaseStream.Position += nColumns * nRows * (nLayers - layerEndOneBased) * realSize;
            
            // Return the result.
            return sum;
        }
    }
}
