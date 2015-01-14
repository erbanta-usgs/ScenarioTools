using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

using ScenarioTools.DatasetCalculator;
using ScenarioTools.ModflowReaders;
using ScenarioTools.Reporting;

namespace ScenarioTools.Data_Providers
{
    public class DataProviderManager
    {
        public static IDataProvider[] GetDataProviders(DataConsumerTypeEnum dataConsumerType, IReportElement parentElement)
        {
            IReportMap reportMap = null;
            if (parentElement is IReportMap)
            {
                reportMap = (IReportMap)parentElement;
            }

            // Make an array of all available data providers.
            IDataProvider[] dataProviders = {
                                               // Graph Data Providers
                                               new DataProviderCbbAtPoint(),
                                               new DataProviderCbbGroup(),
                                               new DataProviderHeadAtPoint(),
                                               //new DataProviderHeadGroup(),
                                               new DataProviderObservedSeries(),
                                               new DataProviderCalculatedSeries(parentElement),

                                               // Map Data Providers
                                               new DataProviderCbbMap(),
                                               new DataProviderHeadMap(),
                                               new DataProviderCalculatedMap(reportMap)
                                           };

            // At this point, if we've been specified to return table data providers, we're going to switch to graph data providers.
            if (dataConsumerType == DataConsumerTypeEnum.Table)
            {
                dataConsumerType = DataConsumerTypeEnum.Chart;
            }

            // Make a list of appropriate data providers.
            List<IDataProvider> appropriateDataProviders = new List<IDataProvider>();
            foreach (IDataProvider dataProvider in dataProviders)
            {
                if (dataProvider.SupportsDataConsumerType(dataConsumerType))
                {
                    appropriateDataProviders.Add(dataProvider);
                }
            }

            // Return the appropriate data providers in an array.
            return appropriateDataProviders.ToArray();
        }

        public static IDataProviderMenu GetMenu(IDataProvider dataProvider)
        {
            // These are graph data series.
            if (dataProvider is DataProviderCalculatedSeries) return new DataProviderCalculatedSeriesMenu((DataProviderCalculatedSeries)dataProvider);

            else if (dataProvider is DataProviderCbbAtPoint) return new DataProviderCbbAtPointMenu((DataProviderCbbAtPoint)dataProvider);
            else if (dataProvider is DataProviderCbbGroup) return new DataProviderCbbGroupMenu((DataProviderCbbGroup)dataProvider);

            else if (dataProvider is DataProviderHeadAtPoint) return new DataProviderHeadAtPointMenu((DataProviderHeadAtPoint)dataProvider);
            else if (dataProvider is DataProviderHeadGroup) return new DataProviderHeadGroupMenu((DataProviderHeadGroup)dataProvider);

            else if (dataProvider is DataProviderObservedSeries) return new DataProviderObservedSeriesMenu((DataProviderObservedSeries)dataProvider);

            // These are the map data providers.
            if (dataProvider is DataProviderCalculatedMap) return new DataProviderCalculatedMapMenu((DataProviderCalculatedMap)dataProvider);

            else if (dataProvider is DataProviderCbbMap) return new DataProviderCbbMapMenu((DataProviderCbbMap)dataProvider);
            else if (dataProvider is DataProviderHeadMap) return new DataProviderHeadMapMenu((DataProviderHeadMap)dataProvider);
            
            // If the data provider is unrecognized, return null.
            return null;
        }
        
        /// <summary>
        /// Read and add to combo box unique descriptors from a MODFLOW binary file of the type used to record heads.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="comboBox"></param>
        public static void PopulateComboBoxFromHeadTypeBinaryFile(string filename, ComboBox comboBox, string dataDescriptor, out int minLayer, out int maxLayer)
        {
            minLayer = 999;
            maxLayer = 1;

            // Read and store unique descriptors from a MODFLOW binary file of the type used to record heads.
            // This type of file can contains any combination of arrays of head, drawdown, or concentration.

            // Clear contents of ComboBox
            comboBox.Items.Clear();

            // Determine the precision model of the heads file
            string identifier;
            int precision = PrecisionHelpers.BinaryDataFilePrecision(filename, out identifier);
            if (precision < 1 || precision > 2)
            {
                return;
            }
            int realSize = precision * 4;

            // Do data values represent concentrations?
            bool dataAreConcentrations = identifier.ToLower() == "concentration";

            // Declare variables that are used frequently
            int kstp, kper, ncol, nrow, ilay, i, ntrans;
            double totimd;
            string text;
            bool found;

            BinaryReader br = null;
            try
            {
                // Open the file.
                br = new BinaryReader(new FileStream(filename, FileMode.Open));
                long fileLength = br.BaseStream.Length;

                // Iterate through data sets
                while (br.BaseStream.Position < fileLength)
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

                    // If text has not already been added to ComboBox.Items, add it
                    found = false;
                    for (i = 0; i < comboBox.Items.Count; i++)
                    {
                        if (comboBox.Items[i].ToString() == text)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        comboBox.Items.Add(text);
                    }

                    ncol = br.ReadInt32();
                    nrow = br.ReadInt32();
                    ilay = br.ReadInt32();

                    if (ilay < minLayer)
                    {
                        minLayer = ilay;
                    }
                    if (ilay > maxLayer)
                    {
                        maxLayer = ilay;
                    }

                    br.BaseStream.Position += ncol * nrow * realSize;
                }
            }

            catch (Exception ex)
            {
            }

            // Assign default item
            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
                for (i = 0; i < comboBox.Items.Count; i++)
                {
                    if (comboBox.Items[i].ToString() == dataDescriptor)
                    {
                        comboBox.SelectedIndex = i;
                        break;
                    }
                }
            }

            if (br != null)
            {
                try
                {
                    br.Close();
                }
                catch { }
            }
        }
    }
}
