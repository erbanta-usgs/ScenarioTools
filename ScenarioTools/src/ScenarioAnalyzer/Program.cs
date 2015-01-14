using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ScenarioTools;
using ScenarioTools.Reporting;

namespace ScenarioAnalyzer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                // Initialize global variables
                ScenarioTools.Spatial.StaticObjects.Initialize();
                GlobalStaticVariables.Initialize();

                // If an argument was provided, try to make a document from it.
                SADocument document = null;
                if (args.Length > 0)
                {
                    document = new SADocument();
                    document.InitFromSAFile(args[0], null);
                }

                //Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new SAMainForm(document));
            }
            catch (Exception e)
            {
                string message = "There was an error opening the application.\n\nERROR: " + e.Message;
                MessageBox.Show(message, "Error opening Scenario Analyzer application", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #region testing methods
        private static void makeRiverImage()
        {
            // Make the image.
            int nCols = 930;
            int nRows = 1730;
            Bitmap b = new Bitmap(nCols, nRows);

            // Open the cell file.
            StreamReader sr = File.OpenText(@"f:\sa\river_cells2.csv");

            // Read all of the cells in the file and mark the appropriate pixels.
            int index = 0;
            char[] delim = { ',' };
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                int cellIndex = 0;
                try {
                    cellIndex = int.Parse(line);
                } catch {}
                index++;

                if (cellIndex != 0)
                {
                    int col = cellIndex % nCols;
                    if (col == 0) {
                        col = nCols;                   
                    }
                    int row = (cellIndex - col) / nCols + 1;

                    int cellIndexVerify = computeCompactCellIndex(1, row, col, nCols, nRows);
                    if (cellIndex != cellIndexVerify)
                    {
                        Console.WriteLine("computation error: " + cellIndex + " != " + cellIndexVerify);
                    }

                    b.SetPixel(col - 1, row - 1, Color.Black);
                }
            }

            // Close the cell file.
            sr.Close();

            b.Save(@"f:\sa\river_cells.png");
        }
        private static int computeCompactCellIndex(int layerOneBased, int rowOneBased, int columnOneBased, int nCols, int nRows)
        {
            return (layerOneBased - 1) * nCols * nRows + (rowOneBased - 1) * nCols + columnOneBased;
        }
        #endregion
    }
}
