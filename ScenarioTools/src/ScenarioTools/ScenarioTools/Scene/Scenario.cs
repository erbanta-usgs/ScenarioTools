using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using HPdf;

using ScenarioTools.ModflowReaders;
using ScenarioTools.PdfWriting;
using ScenarioTools.Util;

using USGS.Puma.FiniteDifference;

namespace ScenarioTools.Scene
{
    public class Scenario : ITaggable
    {
        #region Fields
        private int _tag;
        private string _description;
        private TagLink _link;
        private List<ITaggable> _items;
        #endregion Fields

        /// <summary>
        /// Constructor for Scenario class
        /// </summary>
        public Scenario()
        {
            _link = new TagLink();
            _items = new List<ITaggable>();
            _description = "";
        }

        #region Properties
        public string Message { get; set; }
        /// <summary>
        /// Name of directory where all MODFLOW Name files reside
        /// </summary>
        public string NameFileDirectory { get; set; } 
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }
        public NamefileInfo NameFileInfo { get; set; }
        #endregion Properties

        public bool Export(CellCenteredArealGrid modelGrid, string projectDirectory, 
                           SMDataClasses.SMProjectSettings projectSettings,
                           NamefileInfo namefileInfo, ToolStripItem toolStripItem,
                           BackgroundWorker bgWorker, bool freeFormat, string exportMessage)
        {
            bool OK = true;
            int progressMin = 0;
            int progressMax = 100;
            int bgwProgress = progressMin;
            bgWorker.ReportProgress(bgwProgress);
            string masterNamefile = projectSettings.MasterNameFile;
            if (!File.Exists(masterNamefile))
            {
                string msg = "";
                if (masterNamefile == "")
                {
                    msg = "Please define valid master MODFLOW Name file (Project | Settings)";
                }
                else
                {
                    msg = "Error: Master MODFLOW Name file \"" + masterNamefile + "\" does not exist.";
                }
                MessageBox.Show(msg);
                return false;
            }

            NameFileDirectory = Path.GetDirectoryName(masterNamefile);
            string scenarioID = GetScenarioID();
            string scenarioDirectory = NameFileDirectory + Path.DirectorySeparatorChar + scenarioID;
            if (!Directory.Exists(scenarioDirectory))
            {
                Directory.CreateDirectory(scenarioDirectory);
            }
            NamefileInfo nfInfo = (NamefileInfo)namefileInfo.Clone();
            //
            // Add or replace name file entries that will be overridden by packages included in scenario
            Package package;
            bool exportPackage;
            string fileType = "";
            NameFileEntry nfe;
            int itemCount = Items.Count;
            int progInc = 1;
            if (itemCount > 0)
            {
                progInc = (progressMax - progressMin) / itemCount;
            }
            Directory.SetCurrentDirectory(NameFileDirectory);
            try
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    package = (Package)Items[i];
                    package.Populate(modelGrid);
                    exportPackage = true;
                    fileType = package.FileType();
                    nfe = nfInfo.GetEntry(fileType);
                    if (nfe == null)
                    {
                        if (package.FeatureCount() > 0)
                        {
                            // Add new namefile entry for package
                            nfe = new NameFileEntry();
                            nfe.Filename = package.FileName();
                            nfe.Type = fileType;
                            nfe.Access = InOutAccess.ScenarioInput;
                            nfe.Status = "OLD";
                            nfe.Unit = nfInfo.GetUnusedUnit();
                            nfInfo.Items.Add(nfe);
                        }
                        else
                        {
                            exportPackage = false;
                        }
                    }
                    else
                    {
                        // replace namefile entry
                        nfe.Filename = package.FileName();
                        nfe.Access = InOutAccess.ScenarioInput;
                        nfe.Status = "OLD";
                        package.InputFileAbsolutePath = NameFileDirectory + Path.DirectorySeparatorChar 
                                                        + GetScenarioID() + Path.DirectorySeparatorChar 
                                                        + nfe.Filename;
                    }
                    progressMin = bgwProgress;
                    progressMax = progressMin + progInc;
                    if (exportPackage)
                    {
                        try
                        {
                            package.NameFileEntry = nfe;
                            package.NameFileDirectory = this.NameFileDirectory;
                            if (!package.Export(modelGrid, toolStripItem, bgWorker, freeFormat))
                            {
                                OK = false;
                            }
                        }
                        catch
                        {
                            string msg = "An exception occurred while exporting package: " + package.Name;
                            MessageBox.Show(msg);
                        }
                    }
                }
                //
                if (OK)
                {
                    NameFileEntry nfEntry;
                    string absPath;
                    string oldRelPath;
                    string newRelPath;
                    for (int i = 0; i < nfInfo.Items.Count; i++)
                    {
                        nfEntry = nfInfo.Items[i];
                        // Revise path as needed, depending on input/output access
                        switch (nfEntry.Access)
                        {
                            case InOutAccess.Unknown:
                                // fall through -- Unknown access is treated like Input
                            case InOutAccess.Input:
                                oldRelPath = nfEntry.Filename;
                                // Ned TODO: Assignment of absPath is not correct for package controlled by scenario.
                                // Need alternative assignment in this case
                                absPath = FileUtil.Relative2Absolute(oldRelPath, NameFileDirectory);
                                newRelPath = FileUtil.GetRelativePath(absPath, NameFileDirectory);
                                nfEntry.Filename = newRelPath;
                                break;
                            case InOutAccess.ScenarioInput:
                                // fall through
                            case InOutAccess.Output:
                                oldRelPath = nfEntry.Filename;
                                absPath = FileUtil.Relative2Absolute(oldRelPath, NameFileDirectory);
                                nfEntry.Filename = this.GetScenarioID() + Path.DirectorySeparatorChar + Path.GetFileName(absPath);
                                break;
                        }
                    }
                    try
                    {
                        nfInfo.Namefile = scenarioID + ".nam";
                        Directory.SetCurrentDirectory(NameFileDirectory);
                        OK = nfInfo.Write(scenarioID, Description);
                    }
                    finally
                    {
                    }
                }
                exportPdf(exportMessage, nfInfo);
            }
            finally
            {
                Directory.SetCurrentDirectory(projectDirectory);
            }
            return OK;
        }

        private void exportPdf(string exportMessage, NamefileInfo namefileInfo)
        {
            // Path to which PDF file is to be saved.
            string path = this.NameFileDirectory + Path.DirectorySeparatorChar + GetScenarioID() + 
                          Path.DirectorySeparatorChar + GetScenarioID() + "_Export_Report.pdf"; 

            // Make the PDF and set the compression mode.
            try
            {
                HPdfDoc pdf = new HPdfDoc();
            pdf.SetCompressionMode(HPdfDoc.HPDF_COMP_ALL);

            // Get the fonts.
            HPdfFont fontHelvetica = pdf.GetFont("Helvetica", null);
            HPdfFont fontHelveticaBold = pdf.GetFont("Helvetica-Bold", null);
            HPdfFont footerFont = fontHelveticaBold;
            HPdfFont fontHelveticaOblique = pdf.GetFont("Helvetica-Oblique", null);

            // Write the title.
            HPdfPage page = pdf.AddPage();
            PdfHelper.SetPageSize(page);  // Set page size as 8.5 x 11
            HPdfPoint currentTextPos = new HPdfPoint();
            writeTitle(page, fontHelvetica, ref currentTextPos);

            // Initialize settings.
            float leadingFactor = 0.5f;
            float indent = 0.0f;
            float textHeight = 11;
            float paragraphSpacing = textHeight;
            float spaceAbove = textHeight / 2;
            currentTextPos.x = PdfHelper.MARGIN_LEFT;

            // Write Scenario description to the PDF
            
            // Write the export message sent from MainForm
            PdfHelper.WriteLines(pdf, paragraphSpacing, leadingFactor, indent, exportMessage,
                                 ref currentTextPos, fontHelvetica, textHeight);

            string line = "Scenario description:";
            PdfHelper.WriteLine(page, paragraphSpacing, leadingFactor, indent, line, 
                                ref currentTextPos, fontHelveticaBold, textHeight); 

            // Write the Scenario.Description
            PdfHelper.WriteLines(pdf, spaceAbove, leadingFactor, indent, Description, 
                                 ref currentTextPos, fontHelvetica, textHeight);

            // Write scenario metadata

            // Write summary (number of packages)
            string summary = "";
            if (Items.Count == 0)
            {
                summary = "This scenario uses no MODFLOW packages generated by Scenario Manager.";
            }
            else if (Items.Count == 1)
            {
                summary = "This scenario uses 1 MODFLOW package generated by Scenario Manager.";
            }
            else
            {
                summary = "This scenario uses " + Items.Count.ToString() + " MODFLOW packages generated by Scenario Manager.";
            }
            PdfHelper.WriteLines(pdf, spaceAbove, leadingFactor, indent, summary,
                                 ref currentTextPos, fontHelvetica, textHeight);

            indent = 18;
            // For each Package in scenario, write Package description to the PDF
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] is Package)
                {
                    ((Package)Items[i]).DescribeToPdf(pdf, ref currentTextPos, 
                                                      fontHelvetica, fontHelveticaBold, 
                                                      textHeight, indent);
                }
            }
            page = pdf.GetCurrentPage();

            // Return current text position x to left margin and set indent to 0.
            currentTextPos.x = PdfHelper.MARGIN_LEFT;
            indent = 0;

            // Write table of packages used in simulation but not controlled by Scenario Manager, if any
            int k = 0;
            for (int i = 0; i < namefileInfo.Items.Count; i++)
            {
                if (namefileInfo.Items[i].Access == InOutAccess.Input)
                {
                    k++;
                }
            }
            if (k > 0)
            {
                float lineHeight = (1 + leadingFactor) * textHeight;
                float headerLineThickness = 1;
                float tableHeight = paragraphSpacing + (k + 5) * lineHeight +  headerLineThickness;
                float availableHeight = currentTextPos.y - PdfHelper.MARGIN_BOTTOM;
                float noSpaceAbove = 0;
                currentTextPos.y = currentTextPos.y - paragraphSpacing;
                if (tableHeight > availableHeight)
                {
                    PdfHelper.AddNewPage(pdf, ref currentTextPos, ref noSpaceAbove, ref page, 
                                         fontHelvetica, textHeight);
                }
                line = "This scenario also uses the following MODFLOW packages and" + 
                       " opens the listed input files, which are not controlled by Scenario Manager.  " + 
                       "Files of type DATA or DATA(BINARY) may or may not be used in " +
                       "this scenario.  " +
                       "Path names in the table below are relative to folder \"" + 
                       NameFileDirectory + "\"";
                PdfHelper.WriteLines(pdf, paragraphSpacing, leadingFactor, indent, line,
                                     ref currentTextPos, fontHelvetica, textHeight);

                // Write table header
                currentTextPos.y = currentTextPos.y - paragraphSpacing;
                float tabStop = 81;
                PdfHelper.WriteLines(pdf, noSpaceAbove, leadingFactor, indent, "Package or", ref currentTextPos, fontHelvetica, textHeight);
                float currentY = currentTextPos.y;
                PdfHelper.WriteLines(pdf, noSpaceAbove, leadingFactor, indent, "File Type", ref currentTextPos, fontHelvetica, textHeight);
                currentTextPos.y = currentY;
                PdfHelper.WriteLines(pdf, noSpaceAbove, leadingFactor, tabStop, "Input file", ref currentTextPos, fontHelvetica, textHeight);

                // Draw line under header
                float headerSpace = 6;
                float lineY = currentTextPos.y - headerSpace;
                page.SetLineWidth(headerLineThickness);
                page.MoveTo(PdfHelper.MARGIN_LEFT, lineY);
                page.LineTo(page.GetWidth() - PdfHelper.MARGIN_RIGHT, lineY);
                page.Stroke();
                currentTextPos.y = lineY;

                // Write table rows
                for (int i = 0; i < namefileInfo.Items.Count; i++)
                {
                    if (namefileInfo.Items[i].Access == InOutAccess.Input)
                    {
                        currentY = currentTextPos.y;
                        PdfHelper.WriteLines(pdf, noSpaceAbove, leadingFactor, indent, namefileInfo.Items[i].Type, ref currentTextPos, fontHelvetica, textHeight);
                        currentTextPos.y = currentY;
                        PdfHelper.WriteLines(pdf, noSpaceAbove, leadingFactor, tabStop, namefileInfo.Items[i].Filename, ref currentTextPos, fontHelvetica, textHeight);
                    }
                }

                // Draw line under table
                lineY = currentTextPos.y - headerSpace;
                page.MoveTo(PdfHelper.MARGIN_LEFT, lineY);
                page.LineTo(page.GetWidth() - PdfHelper.MARGIN_RIGHT, lineY);
                page.Stroke();
            }

            // Finally, write version message
            currentTextPos.y = currentTextPos.y - spaceAbove;
            string versionMessage = "Scenario Manager version: " +
                                   Application.ProductVersion.ToString();
            PdfHelper.WriteLines(pdf, paragraphSpacing, leadingFactor, indent, versionMessage,
                                 ref currentTextPos, fontHelveticaOblique, 10.0f);

            // Save the report to a file.
            bool okToSave = true;
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch
                {
                    string errmsg = "Unable to delete existing file \"" + path + "\"";
                    MessageBox.Show(errmsg);
                    okToSave = false;
                }
            }
            if (okToSave)
            {
                try
                {
                    pdf.SaveToFile(path);
                }
                catch (Exception e)
                {
                    string errMsg = "Error encountered in saving PDF to path \"" + path + "\"";
                    MessageBox.Show(errMsg);
                }
            }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void writeTitle(HPdfPage page, HPdfFont font, ref HPdfPoint currentTextPos)
        {
            float titleFontHeight = 16.0f;
            float dateFontHeight = 13.0f;
            float titleLeading = 0.5f * titleFontHeight;
            float titleDateSpacing = 1.2f * titleLeading;
            page.SetFontAndSize(font, titleFontHeight);

            // Create the title and split it into words.
            string str = "Export report for scenario \"" + this.GetScenarioID() + "\"";
            string[] titleString = str.Split(new char[] { ' ' });

            // Combine the words into lines
            List<string> title = new List<string>();
            int index = 0;
            float maxWidth = page.GetWidth() * 2.0f / 3.0f;
            while (index < titleString.Length)
            {
                string line = "";

                // While adding another word does not make the title too long, add more words.
                bool tooLong = false;
                while (!tooLong && index < titleString.Length)
                {
                    // Add the next word.
                    line += (line.Equals("") ? "" : " ") + titleString[index];

                    // Check if adding another word will make the title too long. If so, flag.
                    if (index + 1 < titleString.Length)
                    {
                        string nextAdded = line + " " + titleString[index + 1];
                        if (page.TextWidth(nextAdded) > maxWidth)
                        {
                            tooLong = true;
                        }
                    }
                    index++;
                }
                title.Add(line);
            }

            // Draw the title.
            float yc = page.GetHeight() - PdfHelper.MARGIN_TOP - titleFontHeight;
            float xc;
            for (int i = 0; i < title.Count; i++)
            {
                // Center the text on the page
                xc = (page.GetWidth() - page.TextWidth(title[i])) / 2.0f;
                page.BeginText();
                page.MoveTextPos(xc, yc);                
                page.ShowText(title[i]);
                page.EndText();
                yc -= titleFontHeight;
                if (i < title.Count - 1)
                {
                    yc -= titleLeading;
                }
            }

            // Draw current time and date
            string dateTime = StringUtil.DateNowAsString(true);
            yc -= titleDateSpacing;
            page.SetFontAndSize(font, dateFontHeight);
            // Center the text on the page
            float xcA = (page.GetWidth() - page.TextWidth(dateTime)) / 2.0f;
            page.BeginText();
            page.MoveTextPos(xcA, yc);
            page.ShowText(dateTime);

            // Assign ending text position
            currentTextPos = page.GetCurrentTextPos();

            page.EndText();
        }

        public bool PrepareForExport(DiscretizationFile discretizationFile, 
                                     DateTime simulationStartTime)
        {
            bool OK = true;
            DateTime simulationEndTime = new DateTime();
            if (discretizationFile != null)
            {
                simulationEndTime = simulationStartTime + discretizationFile.GetSimulationTimeSpan();
            }
            if (OK)
            {
                Package package;
                for (int i = 0; i < Items.Count; i++)
                {
                    package = (Package)Items[i];
                    package.SimulationStartTime = simulationStartTime;
                    package.SimulationEndTime = simulationEndTime;
                    package.DiscretizationFile = discretizationFile;
                }
            }
            return OK;
        }

        public string GetFolder(SMDataClasses.SMProjectSettings projectSettings, string projectDirectory)
        {
            try
            {
                string masterNamefile = projectSettings.MasterNameFile;
                string masterNamefilePath = FileUtil.Relative2Absolute(masterNamefile, projectDirectory);
                string masterDirectory = Path.GetDirectoryName(masterNamefilePath);
                string scenarioID = GetScenarioID();
                string scenarioDirectory = masterDirectory + Path.DirectorySeparatorChar + scenarioID;
                return scenarioDirectory;
            }
            catch
            {
                return "";
            }
        }

        public string GetBatchCommand(bool addQuotesIfNeeded)
        {
            string command = "Run_" + GetScenarioID() + ".bat";
            if (addQuotesIfNeeded)
            {
                command = StringUtil.DoubleQuoteIfNeeded(command);
            }
            return command;
        }

        public string GetScenarioID()
        {
            return Link.TreeNode.Text;
        }

        public string Folder()
        {
            return GetScenarioID();
        }

        public string NameFile()
        {
            return GetScenarioID() + ".nam";
        }

        #region ITaggable members

        public void AssignFrom(ITaggable scenarioSource)
        {
            if (scenarioSource is Scenario)
            {
                Scenario ss = (Scenario)scenarioSource;
                Link.AssignFrom(ss.Link);
                Tag = ss.Tag;
                Description = ss.Description;
                Name = ss.Name;
                Items.Clear();
                List<ITaggable> tempItems = new List<ITaggable>();
                for (int i = 0; i < ss.Items.Count; i++)
                {
                    Package sourcePackage = (Package)ss.Items[i];
                    Package pkg;
                    switch (sourcePackage.Type)
                    {
                        case PackageType.ChdType:
                            pkg = new ChdPackage(this);
                            break;
                        case PackageType.RiverType:
                            pkg = new RiverPackage(this);
                            break;
                        case PackageType.WellType:
                            pkg = new WellPackage(this);
                            break;
                        case PackageType.RchType:
                            pkg = new RchPackage(this);
                            break;
                        case PackageType.GhbType:
                            pkg = new GhbPackage(this);
                            break;
                        case PackageType.NoType:
                            pkg = null;
                            break;
                        default:
                            pkg = null;
                            break;
                    }
                    pkg.AssignFrom(sourcePackage);
                    tempItems.Add(pkg);
                }
                Items = tempItems;
            }
        }

        public int GetNewTag()
        {
            _tag = _link.Owner.GetNextTag();
            return _tag;
        }

        public int Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }

        public string Name { get; set; }

        public ElementType ElemType
        {
            get
            {
                return ElementType.Scenario;
            }
        }

        public TagLink Link
        {
            get
            {
                return _link;
            }
            set
            {
                _link = value;
            }
        }

        public List <ITaggable> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
            }
        }

        public ITaggable Parent
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public void ReTag()
        {
            _link.Tag = _link.Owner.GetNextTag();
            // Invoke ReTag for all Packages.
            foreach (Package pkg in _items)
            {
                pkg.ReTag();
            }
        }

        public void ConnectList(List<ITaggable> owner)
        {
            TagUtilities.ConnectList(this, owner);
        }

        public void ConvertRelativePaths(string oldDirectoryPath, string newDirectoryPath)
        {
        }

        #endregion ITaggable members

        public object Clone()
        {
            Scenario scenario = new Scenario();
            scenario.Tag = this.Tag;
            scenario.Link = (TagLink)this._link.Clone();
            scenario.Link.ScenarioElement = scenario;
            scenario.Link.TreeNode = this.Link.TreeNode;
            scenario.Name = this.Name;
            scenario.Description = this._description;
            foreach (Package pkg in _items)
            {
                Package newPackage = (Package)pkg.Clone();
                //newPackage.Parent = this;
                scenario.Items.Add(newPackage);
            }
            return scenario;
        }

        public List<string> Describe()
        {
            List<string> descriptionList = new List<string>();

            // Start with scenario description
            string scenarioID = GetScenarioID();
            descriptionList.Add("Scenario ID: " + scenarioID);
            descriptionList.Add("Description: " + _description);
            int packageCount = 0;
            if (_items != null)
            {
                packageCount = _items.Count;
            }
            descriptionList.Add("Number of packages in \"" + scenarioID + "\" scenario: " + packageCount.ToString());


            // Add Describe() strings from all packages
            if (packageCount > 0)
            {
                List<string> pkgDescriptionList;
                string entry;
                descriptionList.Add("Package documentation:");
                descriptionList.Add("");

                // Iterate through packages
                for (int i = 0; i < _items.Count; i++)
                {
                    pkgDescriptionList = ((Package)_items[i]).Describe();
                    for (int k = 0; k < pkgDescriptionList.Count; k++)
                    {
                        entry = "    " + pkgDescriptionList[k];
                        descriptionList.Add(entry);
                    }
                    if (i < _items.Count - 1)
                    {
                        descriptionList.Add("");
                    }
                }
            }

            return descriptionList;
        }

        public void SelectTreeNode(TreeView treeView)
        {
            int nodeTag = -2;
            if (treeView != null)
            {
                for (int i = 0; i < treeView.Nodes.Count; i++)
                {
                    nodeTag = (int)treeView.Nodes[i].Tag;
                    if (nodeTag == this.Tag)
                    {
                        treeView.SelectedNode = treeView.Nodes[i];
                        return;
                    }
                }
            }
        }

    }
}
