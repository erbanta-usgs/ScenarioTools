using System;
using System.Collections.Generic;
using System.IO;

using ScenarioTools.Numerical;
using ScenarioTools.Util;

namespace ScenarioTools.ModflowReaders
{
    public class ModflowHelpers
    {
        #region Public Methods
        /// <summary>
        /// Get next line from StreamReader that does not start with "#"
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public static string GetDataLine(StreamReader sr)
        {
            string line = "";
            bool OK = false;
            while (!OK)
            {
                line = sr.ReadLine();
                if (!line.StartsWith("#"))
                {
                    OK = true;
                }
            }
            return line;
        }

        public static void ReadArrayControlLine(string line, bool freeFormat, ref string keyword,
                                                 ref int icnstnt, ref string fmtin, ref int nunit,
                                                 ref string fname)
        {
            double cnstnt = 0.0d;
            ReadArrayControlLine(line, ref freeFormat, ref keyword, ref cnstnt, ref fmtin, ref nunit, ref fname);
            icnstnt = Convert.ToInt32(cnstnt);
        }

        public static void ReadArrayControlLine(string line, ref bool freeFormat, ref string keyword, 
                                                 ref double cnstnt, ref string fmtin, ref int nunit, 
                                                 ref string fname)
        {
            freeFormat = true;
            keyword = "";
            cnstnt = 0.0d;
            fmtin = "";
            nunit = 0;
            fname = "";
            string[] contents = StringUtil.ParseLine(line);
            if (String.Equals(contents[0], "constant", StringComparison.CurrentCultureIgnoreCase))
            {
                keyword = "constant";
                cnstnt = Convert.ToInt32(contents[1]);
                return;
            }
            else if (String.Equals(contents[0], "internal", StringComparison.CurrentCultureIgnoreCase))
            {
                keyword = "internal";
                cnstnt = Convert.ToDouble(contents[1]);
                fmtin = contents[2];
                return;
            }
            else if (String.Equals(contents[0], "external", StringComparison.CurrentCultureIgnoreCase))
            {
                keyword = "external";
                nunit = Convert.ToInt32(contents[1]);
                cnstnt = Convert.ToDouble(contents[2]);
                return;
            }
            else if (String.Equals(contents[0], "open/close", StringComparison.CurrentCultureIgnoreCase))
            {
                keyword = "open/close";
                fname = contents[1];
                cnstnt = Convert.ToDouble(contents[2]);
                fmtin = contents[3];
                return;
            }
            else
            {
                // Format is fixed-format
                freeFormat = false;
                string field = line.Substring(0, 10);
                nunit = Convert.ToInt32(field);
                field = line.Substring(10, 10);
                cnstnt = Convert.ToDouble(field);
                fmtin = line.Substring(20, 20);
                return;
            }
        }

        /// <summary>
        /// Implements Modflow's U1DREL array-reading utility.
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="dim"></param>
        /// <param name="freeFormat"></param>
        /// <returns></returns>
        public static double[] U1drel(StreamReader sr, int dim, NamefileInfo nfi)
        {
            bool freeFormat = true;
            string keyword = "";
            double cnstnt = 0.0d;
            string fmtin = "";
            int locat = 0;
            string fname = "";
            double[] values = new double[dim];
            string line;
            //
            try
            {
                // Read control line
                line = sr.ReadLine();
                ReadArrayControlLine(line, ref freeFormat, ref keyword, ref cnstnt, 
                                     ref fmtin, ref locat, ref fname);
                if (freeFormat)
                {
                    switch (keyword)
                    {
                        case ("constant"):
                            for (int i = 0; i < dim; i++)
                            {
                                values[i] = cnstnt;
                            }
                            break;
                        case ("internal"):
                            ReadValues1DFmt(sr, dim, cnstnt, fmtin, ref values);
                            break;
                        case ("external"):
                            // Ned TODO: Note that external option as coded will only support reading one 1-D array from a given file.
                            NameFileEntry nfe = nfi.GetEntry(locat);
                            fname = nfe.Filename;
                            string namefileDir = nfi.GetDirectory();
                            string path = FileUtil.Relative2Absolute(fname, namefileDir);
                            using (StreamReader sr2 = new StreamReader(path))
                            {
                                ReadValues1DFmt(sr2, dim, cnstnt, fmtin, ref values);
                                sr2.Dispose();
                            }
                            break;
                        case ("open/close"):
                            using (StreamReader sr2 = new StreamReader(fname))
                            {
                                ReadValues1DFmt(sr2, dim, cnstnt, fmtin, ref values);
                                sr2.Dispose();
                            }
                            break;
                    }
                }
                else
                {
                    // Array-control record is in fixed format
                    if (locat == 0)
                    {
                        for (int i = 0; i < dim; i++)
                        {
                            values[i] = cnstnt;
                        }
                    }
                    else
                    {
                        ReadValues1DFmt(sr, dim, cnstnt, fmtin, ref values);
                    }
                }
            }
            catch
            {
            }
            return values;
        }

        /// <summary>
        /// Implements Modflow's U2DREL array-reading utility.
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="nrow"></param>
        /// <param name="ncol"></param>
        /// <param name="nfi"></param>
        /// <returns></returns>
        public static double[,] U2drel(StreamReader sr, int nrow, int ncol, NamefileInfo nfi)
        {
            bool freeFormat = true;
            string keyword = "";
            double cnstnt = 0.0d;
            string fmtin = "";
            int locat = 0;
            string fname = "";
            double[,] values = new double[nrow, ncol];
            string line;
            //
            try
            {
                // Read control line
                line = sr.ReadLine();
                ReadArrayControlLine(line, ref freeFormat, ref keyword, ref cnstnt,
                                     ref fmtin, ref locat, ref fname);
                if (freeFormat)
                {
                    switch (keyword)
                    {
                        case ("constant"):
                            for (int i = 0; i < nrow; i++)
                            {
                                for (int j = 0; j < ncol; j++)
                                {
                                    values[i, j] = cnstnt;
                                }
                            }
                            break;
                        case ("internal"):
                            ReadValues2DFmt(sr, nrow, ncol, cnstnt, fmtin, ref values);
                            break;
                        case ("external"):
                            // Ned TODO: Note that external option as coded will only support reading one 2-D array from a given file.
                            NameFileEntry nfe = nfi.GetEntry(locat);
                            fname = nfe.Filename;
                            string namefileDir = nfi.GetDirectory();
                            string path = FileUtil.Relative2Absolute(fname, namefileDir);
                            using (StreamReader sr2 = new StreamReader(path))
                            {
                                ReadValues2DFmt(sr2, nrow, ncol, cnstnt, fmtin, ref values);
                                sr2.Dispose();
                            }
                            break;
                        case ("open/close"):
                            using (StreamReader sr2 = new StreamReader(fname))
                            {
                                ReadValues2DFmt(sr2, nrow, ncol, cnstnt, fmtin, ref values);
                                sr2.Dispose();
                            }
                            break;
                    }
                }
                else
                {
                    // Array-control record is in fixed format
                    if (locat == 0)
                    {
                        // C# array storage order is row-major
                        for (int i = 0; i < nrow; i++)
                        {
                            for (int j = 0; j < ncol; j++)
                            {
                                values[i, j] = cnstnt;
                            }
                        }
                    }
                    else
                    {
                        ReadValues2DFmt(sr, nrow, ncol, cnstnt, fmtin, ref values);
                    }
                }
            }
            catch
            {
            }
            return values;
        }

        /// <summary>
        /// Prepare list of strings to be read by U2DREL from a 2-d array dimensioned [NROW,NCOL]
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static List<string> CreateInputForU2drel(double[,] array, bool freeFormat, int unitNumber)
        {
            // Ned TODO: Make a version of CreateInputForU2drel that accepts a value for print flag IPRN?
            // Ned TODO: Make a version of CreateInputForU2drel that accepts a comment to be written on control line?
            string str;
            int locat = 0;
            int iprn = 0;
            double constantValue = 0.0d;
            List<string> lines = new List<string>();
            if (ArrayOps.IsConstant(array, ref constantValue))
            {
                if (freeFormat)
                {
                    str = "CONSTANT  " + constantValue.ToString();
                }
                else
                {
                    locat = 0;
                    str = ItoS(locat,10) + DtoS(constantValue,10) + "              (FREE)" + ItoS(iprn,10);
                }
                lines.Add(str);
            }
            else
            {
                int nRow = array.GetLength(0);
                int nCol = array.GetLength(1);
                int valsPerLine = 10;
                int kVals;
                if (freeFormat)
                {
                    str = "INTERNAL  1.0  (FREE)  0";
                }
                else
                {
                    locat = unitNumber;
                    str = ItoS(locat, 10) + DtoS(1.0d, 10) + "              (FREE)" + ItoS(iprn, 10);
                }
                lines.Add(str);
                for (int i = 0; i < nRow; i++)
                {
                    str = "";
                    kVals = 0;
                    for (int j = 0; j < nCol; j++)
                    {
                        str = str + array[i, j].ToString() + " ";
                        kVals++;
                        if (kVals == valsPerLine)
                        {
                            lines.Add(str);
                            str = "";
                            kVals = 0;
                        }
                    }
                    if (str != "")
                    {
                        lines.Add(str);
                    }
                }
            }
            return lines;
        }
        /// <summary>
        /// Prepare list of strings to be read by U2DINT from a 2-d array dimensioned [NROW,NCOL]
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static List<string> CreateInputForU2dint(int[,] array, bool freeFormat, int unitNumber)
        {
            // Ned TODO: Make a version of CreateInputForU2dint that accepts a value for print flag IPRN?
            // Ned TODO: Make a version of CreateInputForU2dint that accepts a comment to be written on control line?
            string str;
            int constantValue = 0;
            int locat;
            int iprn = 8;
            List<string> lines = new List<string>();
            if (ArrayOps.IsConstant(array, ref constantValue))
            {
                if (freeFormat)
                {
                    str = "CONSTANT  " + constantValue.ToString();
                }
                else
                {
                    locat = 0;
                    str = ItoS(locat, 10) + ItoS(constantValue, 10) + "              (FREE)" + ItoS(iprn, 10);
                }
                lines.Add(str);
            }
            else
            {
                int nRow = array.GetLength(0);
                int nCol = array.GetLength(1);
                int valsPerLine = 20;
                int kVals;
                if (freeFormat)
                {
                    str = "INTERNAL  1  (FREE)  " + iprn.ToString();
                }
                else
                {
                    locat = unitNumber;
                    str = ItoS(locat, 10) + ItoS(1, 10) + "              (FREE)" + ItoS(iprn, 10);
                }
                lines.Add(str);
                for (int i = 0; i < nRow; i++)
                {
                    str = "";
                    kVals = 0;
                    for (int j = 0; j < nCol; j++)
                    {
                        str = str + array[i, j].ToString() + " ";
                        kVals++;
                        if (kVals == valsPerLine)
                        {
                            lines.Add(str);
                            str = "";
                            kVals = 0;
                        }
                    }
                    if (str != "")
                    {
                        lines.Add(str);
                    }
                }
            }
            return lines;
        }
        /// <summary>
        /// Wrapper for the StringUtil.DoubleToString method
        /// </summary>
        /// <param name="x"></param>
        /// <param name="fieldWidth"></param>
        /// <returns></returns>
        public static string DtoS(double x, int fieldWidth)
        {
            return StringUtil.DoubleToString(x, fieldWidth);
        }
        /// <summary>
        /// Wrapper for the StringUtil.IntToString method
        /// </summary>
        /// <param name="i"></param>
        /// <param name="fieldWidth"></param>
        /// <returns></returns>
        public static string ItoS(int i, int fieldWidth)
        {
            return StringUtil.IntToString(i, fieldWidth);
        }
        #endregion Public Methods

        #region Private methods
        private static void ReadValues1DFmt(StreamReader sr, int dim, double cnstnt, 
                                            string fmtin, ref double[] values)
        {
            string fmtLocal = fmtin;
            string line;
            char[] descriptors = { 'd', 'D', 'e', 'E', 'f', 'F', 'g', 'G', '.' };
            fmtLocal = fmtLocal.Replace('(', ' ');
            fmtLocal = fmtLocal.Replace(')', ' ');
            fmtLocal = fmtLocal.Trim();
            string[] format = fmtLocal.Split(descriptors);
            int numFields = Convert.ToInt32(format[0]);
            int fieldWidth = Convert.ToInt32(format[1]); // Ignore # decimal places--assume decimal point is included
            int valuesNeeded = dim;
            line = sr.ReadLine();
            string strVal;
            int pos = 0;
            int i = 0;
            while (valuesNeeded > 0)
            {
                strVal = line.Substring(0, fieldWidth);
                values[i] = Double.Parse(strVal) * cnstnt;
                i++;
                pos++;
                valuesNeeded--;
                if (valuesNeeded > 0)
                {
                    if (pos == numFields)
                    {
                        line = sr.ReadLine();
                        pos = 0;
                    }
                    else
                    {
                        line = line.Remove(0, fieldWidth);
                    }
                }
            }
        }

        private static void ReadValues2DFmt(StreamReader sr, int nrow, int ncol, double cnstnt, 
                                            string fmtin, ref double[,] values)
        {
            string fmtLocal = fmtin;
            string line;
            char[] descriptors = { 'd', 'D', 'e', 'E', 'f', 'F', 'g', 'G', '.' };
            fmtLocal = fmtLocal.Replace('(', ' ');
            fmtLocal = fmtLocal.Replace(')', ' ');
            fmtLocal = fmtLocal.Trim();
            string[] format = fmtLocal.Split(descriptors);
            int numFields = Convert.ToInt32(format[0]);
            int fieldWidth = Convert.ToInt32(format[1]); // Ignore # decimal places--assume decimal point is included
            string strVal;
            int pos;
            int j;
            int valuesNeeded;
            for (int i = 0; i < nrow; i++)
            {
                valuesNeeded = ncol;
                line = sr.ReadLine();
                j = 0;
                pos = 0;
                while (valuesNeeded > 0)
                {
                    strVal = line.Substring(0, fieldWidth);
                    values[i, j] = Double.Parse(strVal) * cnstnt;
                    j++;
                    pos++;
                    valuesNeeded--;
                    if (valuesNeeded > 0)
                    {
                        if (pos == numFields)
                        {
                            line = sr.ReadLine();
                            pos = 0;
                        }
                        else
                        {
                            line = line.Remove(0, fieldWidth);
                        }
                    }
                }
            }
        }
        #endregion Private methods
    }
}
