using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ScenarioTools.DataClasses;
using ScenarioTools.Dialogs;

namespace ScenarioTools.Data_Providers
{
    public partial class DataProviderHeadGroupMenu : UserControl, IDataProviderMenu
    {
        DataProviderHeadGroup dataProvider;

        public DataProviderHeadGroupMenu(DataProviderHeadGroup dataProvider)
        {
            InitializeComponent();

            // Store a reference to the data provider.
            this.dataProvider = dataProvider;

            inCheckMethod = false;
        }

        private void DataProviderHeadGroupMenu_Load(object sender, EventArgs e)
        {
            // Populate the heads file text box.
            textBoxHeadsFile.Text = dataProvider.HeadsFile;

            // Populate the layer text boxes.
            textBoxLayerStart.Text = dataProvider.StartLayer + "";
            textBoxLayerEnd.Text = dataProvider.EndLayer + "";

            // Populate the combo box
            int minLayer;
            int maxLayer;
            Data_Providers.DataProviderManager.PopulateComboBoxFromHeadTypeBinaryFile(dataProvider.HeadsFile, comboBox1, dataProvider.DataDescriptor, out minLayer, out maxLayer);

            // Refresh the group tree.
            GroupHelper.RefreshGroupTree(treeViewGroups);

            // Get the list of selected groups from the data provider.
            CellGroupProvider[] selectedGroups = dataProvider.CellGroups;

            // Check the selected groups in the group tree.
            if (selectedGroups != null)
            {
                GroupHelper.SetSelectedGroups(treeViewGroups, selectedGroups);
            }

            // Add a handler to the tree to update child nodes with the state of their parents.
            treeViewGroups.AfterCheck += new TreeViewEventHandler(treeViewGroups_AfterCheck);

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
            bool ok = true;
            // We are going to go step by step to determine whether any piece of information has changed that warrants invalidation the data provider.
            bool needsInvalidation = false;

            // If the heads file has changed, update it and flag that the data provider needs invalidation.
            string headsFile = textBoxHeadsFile.Text;
            if (!headsFile.Equals(dataProvider.HeadsFile))
            {
                dataProvider.HeadsFile = headsFile;
                needsInvalidation = true;
            }

            // If the data identifier has changed, update it and flag that the data provider needs invalidation.
            if (comboBox1.SelectedItem != null)
            {
                string descriptor = comboBox1.SelectedItem.ToString();
                if (!descriptor.Equals(dataProvider.DataDescriptor))
                {
                    dataProvider.DataDescriptor = descriptor;
                    needsInvalidation = true;
                }
            }

            // If the start layer is parseable and has changed, update it and flag that the data provider needs invalidation.
            int startLayer;
            if (int.TryParse(textBoxLayerStart.Text, out startLayer))
            {
                if (startLayer != dataProvider.StartLayer) {
                    dataProvider.StartLayer = startLayer;
                    needsInvalidation = true;
                }
            }
            else
            {
                ok = false;
            }

            // If the end layer is parseable and has changed, update it and flag that the data provider needs invalidation.
            int endLayer;
            if (int.TryParse(textBoxLayerEnd.Text, out endLayer))
            {
                if (endLayer != dataProvider.EndLayer) {
                    dataProvider.EndLayer = endLayer;
                    needsInvalidation = true;
                }
            }
            else
            {
                ok = false;
            }

            // Get the selected groups.
            CellGroupProvider[] selectedGroups = GroupHelper.GetSelectedGroups(treeViewGroups);

            // If the groups to not match the data provider's groups, update them and flag the data provider for invalidation.
            if (!CellGroupProvider.ArrayEquals(selectedGroups, dataProvider.CellGroups))
            {
                dataProvider.CellGroups = selectedGroups;
                needsInvalidation = true;
            }

            // If necessary, invalidate the dataset.
            if (needsInvalidation)
            {
                dataProvider.InvalidateDataset();
            }

            if (!ok)
            {
                errorMessage = "Error parsing start layer or end layer.";
            }
            return ok;
        }

        #endregion        

        private void buttonAddGroup_Click(object sender, EventArgs e)
        {
            // Allow the user to add a group file.
            GroupHelper.AddGroupFile();

            // Refresh the group tree.
            GroupHelper.RefreshGroupTree(treeViewGroups);
        }
        private void buttonBrowseHeadsFile_Click(object sender, EventArgs e)
        {
            // Make and show an open-file dialog. Only continue if accepted.
            OpenFileDialog dialog = DialogHelper.GetHeadsFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBoxHeadsFile.Text = dialog.FileName;

                // Populate the combo box
                int minLayer;
                int maxLayer;
                Data_Providers.DataProviderManager.PopulateComboBoxFromHeadTypeBinaryFile(textBoxHeadsFile.Text, comboBox1, dataProvider.DataDescriptor, out minLayer, out maxLayer);
            }
        }

        private void buttonClearCellGroups_Click(object sender, EventArgs e)
        {
            this.treeViewGroups.Nodes.Clear();
        }
    }
}
