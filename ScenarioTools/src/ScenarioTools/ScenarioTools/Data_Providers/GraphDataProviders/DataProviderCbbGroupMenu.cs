using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ScenarioTools.DataClasses;
using ScenarioTools.Dialogs;
using ScenarioTools.LogAndErrorProcessing;
using ScenarioTools.ModflowReaders;

namespace ScenarioTools.Data_Providers
{
    public partial class DataProviderCbbGroupMenu : UserControl, IDataProviderMenu
    {
        DataProviderCbbGroup dataProvider;

        public DataProviderCbbGroupMenu(DataProviderCbbGroup dataProvider)
        {
            InitializeComponent();

            // Store a reference to the data provider.
            this.dataProvider = dataProvider;

            inCheckMethod = false;
        }
        private void DataProviderCbbGroupMenu_Load(object sender, EventArgs e)
        {
            // Populate the CBB file text box.
            textBoxCbbFile.Text = dataProvider.CbbFile;

            // Populate the layer text boxes.
            textBoxLayerStart.Text = dataProvider.StartLayer + "";
            textBoxLayerEnd.Text = dataProvider.EndLayer + "";

            // Populate the identifier combo box.
            int numLayers;
            CbbHelper.PopulateIdentifierComboBox(textBoxCbbFile.Text, comboBoxDataset, dataProvider.DataIdentifier, out numLayers);

            // Refresh the group tree.
            GroupHelper.RefreshGroupTree(treeViewGroups);

            // Get the list of selected groups from the data provider.
            CellGroupProvider[] selectedGroups = dataProvider.CellGroupProviders;

            // Check the selected groups in the group tree.
            if (selectedGroups != null)
            {
                GroupHelper.SetSelectedGroups(treeViewGroups, selectedGroups);
            }

            // Add a handler to the tree to update child nodes with the state of their parents.
            treeViewGroups.AfterCheck += new TreeViewEventHandler(treeViewGroups_AfterCheck);

            // Assign properties
            Dock = DockStyle.Fill;
        }
        private bool inCheckMethod;
        void treeViewGroups_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (!inCheckMethod)
            {
                inCheckMethod = true;

                // Get the node.
                TreeNode clickedNode = e.Node;

                // If this node has children, it is a file node. Update all of its children with its check state.
                if (clickedNode.Nodes.Count > 0)
                {
                    foreach (TreeNode node in clickedNode.Nodes)
                    {
                        node.Checked = clickedNode.Checked;
                    }
                }

                // Check for tree check state consistency.
                GroupHelper.EnsureTreeNodeConsistency(treeViewGroups);

                inCheckMethod = false;
            }
        }

        #region IDataProviderMenu Members

        public bool UpdateDataProvider(out string errorMessage)
        {
            errorMessage = "";
            try
            {
                // We are going to go step by step to determine whether any 
                // piece of information has changed that warrants invalidation 
                // of the data provider.
                bool needsInvalidation = false;

                // If the CBB file has changed, update it and flag that 
                // the data provider needs invalidation.
                string cbbFile = textBoxCbbFile.Text;
                if (!cbbFile.Equals(dataProvider.CbbFile))
                {
                    dataProvider.CbbFile = cbbFile;
                    needsInvalidation = true;
                }

                // If the data identifier has changed, update it and flag 
                // that the data provider needs invalidation.
                if (comboBoxDataset.SelectedItem != null)
                {
                    string dataIdentifier = comboBoxDataset.SelectedItem.ToString();
                    if (!dataIdentifier.Equals(dataProvider.DataIdentifier))
                    {
                        dataProvider.DataIdentifier = dataIdentifier;
                        needsInvalidation = true;
                    }
                }

                // If the start layer is parseable and has changed, update
                // it and flag that the data provider needs invalidation.
                int startLayer;
                if (int.TryParse(textBoxLayerStart.Text, out startLayer))
                {
                    if (startLayer != dataProvider.StartLayer)
                    {
                        dataProvider.StartLayer = startLayer;
                        needsInvalidation = true;
                    }
                }
                else
                {
                    errorMessage = "Error parsing start layer";
                    return false;
                }

                // If the end layer is parseable and has changed, update it 
                // and flag that the data provider needs invalidation.
                int endLayer;
                if (int.TryParse(textBoxLayerEnd.Text, out endLayer))
                {
                    if (endLayer != dataProvider.EndLayer)
                    {
                        dataProvider.EndLayer = endLayer;
                        needsInvalidation = true;
                    }
                    if (startLayer > endLayer)
                    {
                        errorMessage = "Error: Start layer is greater than end layer.";
                        return false;
                    }
                    if (startLayer < 1)
                    {
                        errorMessage = "Error: Start layer is less than one.";
                        return false;
                    }
                }
                else
                {
                    errorMessage = "Error parsing end layer";
                    return false;
                }

                // Get the selected groups.
                CellGroupProvider[] selectedGroups = GroupHelper.GetSelectedGroups(treeViewGroups);

                // If the groups to not match the data provider's groups, 
                // update them and flag the data provider for invalidation.
                if (!CellGroupProvider.ArrayEquals(selectedGroups, dataProvider.CellGroupProviders))
                {
                    dataProvider.CellGroupProviders = selectedGroups;
                    needsInvalidation = true;
                }

                // If necessary, invalidate the dataset.
                if (needsInvalidation)
                {
                    dataProvider.InvalidateDataset();
                }
                return true;
            }
            catch
            {
                errorMessage = "Unidentified error trapped.";
                return false;
            }
        }

        #endregion

        private void buttonAddGroup_Click(object sender, EventArgs e)
        {
            // Allow the user to add a group file.
            GroupHelper.AddGroupFile();

            // Refresh the group tree.
            GroupHelper.RefreshGroupTree(treeViewGroups);
        }
        private void buttonBrowseCbbFile_Click(object sender, EventArgs e)
        {
            // Make and show an open-file dialog. Only continue if accepted.
            OpenFileDialog dialog = DialogHelper.GetCbbFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBoxCbbFile.Text = dialog.FileName;
            }
        }

        private void textBoxCbbFile_TextChanged(object sender, EventArgs e)
        {
            // Populate the identifier box.
            int numLayers;
            CbbHelper.PopulateIdentifierComboBox(textBoxCbbFile.Text, comboBoxDataset, dataProvider.DataIdentifier, out numLayers);
        }

        private void buttonClearGroups_Click(object sender, EventArgs e)
        {
            this.treeViewGroups.Nodes.Clear();
        }
    }
}
