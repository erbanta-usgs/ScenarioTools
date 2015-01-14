using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

using ScenarioTools.Dialogs;


namespace ScenarioTools.DataClasses
{
    public class GroupHelper
    {
        private static List<CellGroupFile> cellGroupFiles;

        public static void AddGroupFile()
        {
            // Show a dialog for opening a group file.
            OpenFileDialog dialog = DialogHelper.GetOpenGroupFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Get the name of the file and make a group file.
                CellGroupFile groupFile = CellGroupFile.FromFile(dialog.FileName);

                // If the group file is good (contains at least one group) and is new, 
                // add it to the cell group files list.
                if (groupFile.NumGroups > 0)
                {
                    if (cellGroupFiles.Contains(groupFile))
                    {
                        MessageBox.Show("The file " + dialog.FileName + " is already in the list.", 
                            "Duplicate Group File", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                        AddGroupFile();
                    }
                    else
                    {
                        cellGroupFiles.Add(groupFile);
                    }
                }

                // Otherwise, display a message and redisplay the dialog.
                else
                {
                    MessageBox.Show("The file " + dialog.FileName + " does not contain any valid groups.", 
                        "Invalid Group File", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    AddGroupFile();
                }
            }

        }
        public static CellGroupFile[] GetCellGroups()
        {
            // If the cell group list is null, make a new list.
            if (cellGroupFiles == null)
            {
                cellGroupFiles = new List<CellGroupFile>();
            }

            // Return the list as an array.
            return cellGroupFiles.ToArray();
        }
        private static void addCellGroup(CellGroupFile groupFile)
        {
            // If the cell group list is null, make a new list.
            if (cellGroupFiles == null)
            {
                cellGroupFiles = new List<CellGroupFile>();
            }

            // Add the group to the list.
            cellGroupFiles.Add(groupFile);
        }

        public static void RegisterGroups(CellGroupProvider[] cellGroups)
        {
            // If the current group list is null, make it.
            if (cellGroupFiles == null)
            {
                cellGroupFiles = new List<CellGroupFile>();
            }

            // Add all files that are not currently contained in the list.
            foreach (CellGroupProvider group in cellGroups)
            {
                CellGroupFile groupFile = group is CellGroupFile ? (CellGroupFile)group : ((CellGroup)group).Parent;
                if (!cellGroupFiles.Contains(groupFile))
                {
                    cellGroupFiles.Add(groupFile);
                }
            }
        }
        public static void RefreshGroupTree(TreeView treeView)
        {
            // Get the groups that are currently selected.
            CellGroupProvider[] selectedGroups = GroupHelper.GetSelectedGroups(treeView);

            // Get the group files that are currently in the workspace.
            CellGroupFile[] cellGroupFiles = GroupHelper.GetCellGroups();

            // Clear the current tree.
            treeView.Nodes.Clear();

            // Add the current groups to the tree.
            for (int i = 0; i < cellGroupFiles.Length; i++)
            {
                // Make a node for the file.
                CellGroupFile cellGroupFile = cellGroupFiles[i];
                string filename = Path.GetFileName(cellGroupFile.Name);
                TreeNode fileNode = treeView.Nodes.Add(filename);
                fileNode.ToolTipText = cellGroupFile.Name;
                fileNode.Tag = cellGroupFile;

                // Make sub-nodes for the groups in the file.
                for (int j = 0; j < cellGroupFile.NumGroups; j++)
                {
                    TreeNode groupNode = fileNode.Nodes.Add(cellGroupFile[j].Name);
                    groupNode.Tag = cellGroupFile[j];
                }

                // Expand the file node.
                fileNode.Expand();
            }

            // Select the groups that were previously selected.
            SetSelectedGroups(treeView, selectedGroups);
        }
        public static CellGroupProvider[] GetSelectedGroups(TreeView treeView)
        {
            // Add all selected groups to a list.
            List<CellGroupProvider> groups = new List<CellGroupProvider>();
            foreach (TreeNode fileNode in treeView.Nodes)
            {
                // If the file node is checked, add it to the list.
                if (fileNode.Checked)
                {
                    groups.Add((CellGroupProvider)fileNode.Tag);
                }

                // Otherwise, add any children that are checked.
                else
                {
                    foreach (TreeNode groupNode in fileNode.Nodes)
                    {
                        if (groupNode.Checked)
                        {
                            groups.Add((CellGroupProvider)groupNode.Tag);
                        }
                    }
                }
            }

            // Return the result as an array.
            return groups.ToArray();
        }
        public static void SetSelectedGroups(TreeView treeView, CellGroupProvider[] selectedGroups)
        {
            // Make a list of the selected groups.
            List<CellGroupProvider> selectedGroupsList = new List<CellGroupProvider>(selectedGroups);

            foreach (TreeNode fileNode in treeView.Nodes)
            {
                // Check whether the file is a selected group. If so, check it and all of its children.
                CellGroupFile groupFile = (CellGroupFile)fileNode.Tag;
                if (selectedGroupsList.Contains(groupFile))
                {
                    fileNode.Checked = true;
                    foreach (TreeNode childNode in fileNode.Nodes)
                    {
                        childNode.Checked = true;
                    }
                }

                // Otherwise, check the children. If they are in the selected list, check them.
                else
                {
                    foreach (TreeNode groupNode in fileNode.Nodes)
                    {
                        // Get the group from the node.
                        CellGroup group = (CellGroup)groupNode.Tag;

                        // If the group is contained in the selected groups, check the node.
                        if (selectedGroupsList.Contains(group))
                        {
                            groupNode.Checked = true;
                        }
                    }
                }
            }
        }
        public static void EnsureTreeNodeConsistency(TreeView treeView)
        {
            // Ensure that the check state of each file node is consistent with the group nodes beneath it.
            foreach (TreeNode fileNode in treeView.Nodes)
            {
                // Determine the state of all children of the file node.
                bool hasChecked = false;
                bool hasUnchecked = false;
                foreach (TreeNode node in fileNode.Nodes)
                {
                    if (node.Checked)
                    {
                        hasChecked = true;
                    }
                    else
                    {
                        hasUnchecked = true;
                    }
                }

                // If the file node has both checked and unchecked children, mark it as indeterminate.
                if (hasChecked && hasUnchecked)
                {
                    // There is no setting for indeterminate, so mark as false.
                    fileNode.Checked = false;
                }
                else if (hasUnchecked)
                {
                    fileNode.Checked = false;
                }
                else
                {
                    fileNode.Checked = true;
                }
            }
        }
    }
}
