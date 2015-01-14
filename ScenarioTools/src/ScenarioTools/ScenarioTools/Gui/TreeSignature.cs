using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ScenarioTools.Gui
{
    public class TreeSignature
    {
        private TreeSignatureNode[] nodes;

        private TreeSignature(TreeSignatureNode[] nodes)
        {
            this.nodes = nodes;
        }
        public static TreeSignature GetTreeSignature(TreeView tree)
        {
            TreeSignatureNode[] nodes = new TreeSignatureNode[tree.Nodes.Count];
            for (int i = 0; i < tree.Nodes.Count; i++)
            {
                nodes[i] = GetNodeSignature(tree.Nodes[i]);
            }

            return new TreeSignature(nodes);
        }
        private static TreeSignatureNode GetNodeSignature(TreeNode node)
        {
            // Get the expansion state of the node.
            bool isSelected = node.IsSelected;
            bool isExpanded = node.IsExpanded;

            // Get the signatures of the child nodes.
            TreeSignatureNode[] nodes = new TreeSignatureNode[node.Nodes.Count];
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                nodes[i] = GetNodeSignature(node.Nodes[i]);
            }

            // Return the signature node.
            return new TreeSignatureNode(isSelected, isExpanded, nodes);
        }
        private class TreeSignatureNode
        {
            private bool isExpanded;
            private bool isSelected;
            private TreeSignatureNode[] nodes;

            internal TreeSignatureNode(bool isSelected, bool isExpanded, TreeSignatureNode[] nodes)
            {
                this.isSelected = isSelected;
                this.isExpanded = isExpanded;
                this.nodes = nodes;
            }

            internal void Impose(TreeView treeView, TreeNode treeNode)
            {
                try
                {
                    if (isExpanded)
                    {
                        treeNode.Expand();
                    }
                    if (isSelected)
                    {
                        treeView.SelectedNode = treeNode;
                    }
                    for (int i = 0; i < nodes.Length; i++)
                    {
                        nodes[i].Impose(treeView, treeNode.Nodes[i]);
                    }
                }
                catch { }
            }
        }

        public void Impose(TreeView treeView)
        {
            int index = 0;
            try
            {
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodes[i].Impose(treeView, treeView.Nodes[i]);
                    index++;
                }
            }
            catch { }

            for (int i = index; i < treeView.Nodes.Count; i++)
            {
                treeView.Nodes[i].Expand();
            }
        }
    }
}
