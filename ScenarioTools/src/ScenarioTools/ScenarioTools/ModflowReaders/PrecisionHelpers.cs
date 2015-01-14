using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScenarioTools.ModflowReaders
{
    public class PrecisionHelpers
    {
        // Constants used for ICODE in cell-by-cell budget file
        const int ICODE_0_NOT_SPECIFIED = 0;
        const int ICODE_1_GRID_3D = 1;
        const int ICODE_2_SIMPLE_LIST = 2;
        const int ICODE_3_GRID_2D_PAIR = 3;
        const int ICODE_4_GRID_2D_SINGLE = 4;
        const int ICODE_5_LIST_WITH_AUX = 5;

        #region Public methods
        /// <summary>
        /// Find precision model of a MODFLOW binary cell-by-cell budget file.
        /// </summary>
        /// <param name="budgetFilename"></param>
        /// <returns>0 for unrecognized, 1 for single, 2 for double</returns>
        public static int BudgetFilePrecision(string budgetFilename, out bool isCompact)
        {
            // Default is unrecognized file
            int iPrec = 0;
            isCompact = false;
            BinaryReader br = null;
            if (File.Exists(budgetFilename))
            {
                for (int precision = 1; precision <= 2; precision++)
                {
                    if (iPrec == 0)
                    {
                        int realSize = precision * 4;
                        try
                        {
                            // Open the file.
                            br = new BinaryReader(File.Open(budgetFilename, FileMode.Open));

                            // Read from the stream.
                            while (br.BaseStream.Position < br.BaseStream.Length)
                            {
                                // Read the first two values.
                                int timestep = br.ReadInt32();
                                int stressPeriod = br.ReadInt32();

                                // Read the 16-character identifier.
                                string identifier = new string(br.ReadChars(16)).Trim();

                                // The next values are the number of columns, rows, and number of layers.
                                int nCols = br.ReadInt32();
                                int nRows = br.ReadInt32();
                                int nLayersSigned = br.ReadInt32();
                                int nLayers = Math.Abs(nLayersSigned);

                                if (nCols < 1 || nRows < 1)
                                {
                                    throw new Exception();
                                }
                                if (nCols > 100000000 || nRows > 100000000 | nLayers > 100000000)
                                {
                                    throw new Exception();
                                }
                                if (nCols * nRows > 100000000 || nCols * nLayers > 100000000
                                    || nRows * nLayers > 100000000)
                                {
                                    throw new Exception();
                                }

                                // Determine the value of the write method (ICODE).
                                // If NLAY is positive, then ICODE will not be specified. 
                                // Otherwise, it will be the next value.
                                int iCode = nLayersSigned > 0 ? ICODE_0_NOT_SPECIFIED : br.ReadInt32();

                                // If ICODE was specified, read the DELT, PERTIM, and TOTIM variables.
                                if (iCode != ICODE_0_NOT_SPECIFIED)
                                {
                                    isCompact = true;
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
                                }
                                // This is the case for a single 3D grid dataset (ICODE = 0 or 1).
                                if (iCode == ICODE_0_NOT_SPECIFIED || iCode == ICODE_1_GRID_3D)
                                {
                                    // Skip the 3-dimensional grid.
                                    br.BaseStream.Position += nLayers * nCols * nRows * realSize;
                                }

                                // This is the case for a simple list (no aux variables).
                                else if (iCode == ICODE_2_SIMPLE_LIST)
                                {
                                    // Read an integer to determine the data size.
                                    int numToRead = br.ReadInt32();

                                    // Skip over the data (1 int and 1 float per record).
                                    br.BaseStream.Position += numToRead * (4 + realSize);
                                }
                                else if (iCode == ICODE_3_GRID_2D_PAIR)
                                {
                                    // Skip over both grids (one integer array and one real array).
                                    br.BaseStream.Position += nCols * nRows * (4 + realSize);
                                }
                                else if (iCode == ICODE_4_GRID_2D_SINGLE)
                                {
                                    // Skip over the grid.
                                    br.BaseStream.Position += nCols * nRows * realSize;
                                }
                                else if (iCode == ICODE_5_LIST_WITH_AUX)
                                {
                                    // Determine the number of auxiliary variables.
                                    int numAuxVariables = br.ReadInt32() - 1;

                                    // Read the auxiliary variable identifiers.
                                    for (int i = 0; i < numAuxVariables; i++)
                                    {
                                        string auxVariableName = new string(br.ReadChars(16));
                                    }

                                    // Determine the number of records.
                                    int numToRead = br.ReadInt32();

                                    // Skip over the data (1 int and (1 + numAux) floats per record).
                                    br.BaseStream.Position += numToRead * (4 + realSize * (1 + numAuxVariables));
                                }
                                else
                                {
                                    throw new Exception();
                                }
                            }
                            // The reading of data was successful
                            iPrec = precision;
                        }
                        catch
                        {
                        }
                        finally
                        {
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
                    }
                }
            }

            return iPrec;
        }
        /// <summary>
        /// Find precision model of a MODFLOW binary head or drawdown file
        /// or a SEAWATER or MT3D concentration file
        /// </summary>
        /// <param name="headFilename"></param>
        /// <returns>0 for unrecognized, 1 for single, 2 for double</returns>
        public static int BinaryDataFilePrecision(string filename, out string identifier)
        {
            // Default is unrecognized
            int iPrec = 0;

            // Declare variables
            int kstp, kper, ncol, nrow, ilay;
            identifier = "";
            float pertim, totim;
            double pertimd, totimd;
            BinaryReader br = null;

            if (File.Exists(filename))
            {
                for (int precision = 1; precision <= 2; precision++)
                {
                    if (iPrec == 0)
                    {
                        try
                        {
                            int realSize = precision * 4;
                            int countLoops = 0;

                            // Open the binary file.
                            br = new BinaryReader(File.Open(filename, FileMode.Open));

                            // Read from the stream.
                            while (br.BaseStream.Position < br.BaseStream.Length)
                            {
                                // Read and validate identifying information preceding array
                                kstp = br.ReadInt32(); // UCN contains NTRANS here; OK to check
                                if (!validDimension(kstp)) throw new Exception();
                                kper = br.ReadInt32(); // UCN contains KSTP here; OK to check
                                if (!validDimension(kper)) throw new Exception();
                                switch (precision)
                                {
                                    case 1:
                                        pertim = br.ReadSingle(); // UCN contains KPER here; do not check
                                        //if (pertim < 0.0f) throw new Exception();
                                        totim = br.ReadSingle(); // UCN contains TIME2 here; OK to check
                                        if (totim < 0.0f) throw new Exception();
                                        break;
                                    case 2:
                                        // UCN is always single precision, so these statements
                                        // should never be executed for a UCN file.
                                        pertimd = br.ReadDouble(); 
                                        if (pertimd < 0.0) throw new Exception();
                                        totimd = br.ReadDouble();
                                        if (totimd < 0.0) throw new Exception();
                                        break;
                                }

                                // Read and validate identifier
                                identifier = new string(br.ReadChars(16)).Trim();
                                if (!validDatasetIdentifier(identifier)) throw new Exception();

                                ncol = br.ReadInt32();
                                if (!validDimension(ncol)) throw new Exception();
                                nrow = br.ReadInt32();
                                if (!validDimension(nrow)) throw new Exception();
                                ilay = br.ReadInt32();
                                // Note: ilay would be negative if array is for a cross-section model
                                if (ilay > 100000000 || ilay < -100000000 || ilay == 0) throw new Exception();

                                // Skip over 2-D array
                                br.BaseStream.Position += ncol * nrow * realSize;

                                // Increment loop counter and determine if precision has been validated
                                countLoops++;
                                if (countLoops > 1 || br.BaseStream.Position == br.BaseStream.Length)
                                {
                                    iPrec = precision;
                                    break;
                                }
                            }
                        }
                        catch
                        {
                        }
                        finally
                        {
                            if (br != null)
                            {
                                try
                                {
                                    br.Close();
                                }
                                catch
                                {
                                }
                                br = null;
                            }
                        }
                    }
                }
            }

            return iPrec;
        }
        #endregion Public methods

        #region Private methods
        private static bool validDatasetIdentifier(string identifier)
        {
            string temp = identifier.ToLower().Trim();
            return (temp == "head" || temp == "drawdown" || temp == "concentration");
        }
        private static bool validDimension(int dim)
        {
            if (dim < 1) return false;
            if (dim > 100000000) return false;
            return true;
        }
        #endregion Private methods
    }
}
