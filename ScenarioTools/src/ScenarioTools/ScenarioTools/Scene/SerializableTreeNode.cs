using System.Collections.Generic;
using System.Windows.Forms;

namespace ScenarioTools.Scene
{
    public class SerializableTreeNode
    {
        // Fields
        private List<SerializableTreeNode> _items = new List<SerializableTreeNode>();

        // Constructor(s)

        /// <summary>
        /// Parameterless SerializableTreeNode constructor
        /// </summary>
        public SerializableTreeNode()
        {
            Tag = -1;
            ImageIndex = -1;
            Text = "";
        }

        /// <summary>
        /// Construct a SerializableTreeNode from a SerializableTreeNode
        /// </summary>
        /// <param name="node">a SerializableTreeNode to be copied</param>
        public SerializableTreeNode(SerializableTreeNode node) : this()
        {
            Tag = node.Tag;
            ImageIndex = node.ImageIndex;
            Text = node.Text;
            if (node.Items != null)
            {
                foreach (SerializableTreeNode item in node.Items)
                {
                    _items.Add(item);
                }
            }
        }

        /// <summary>
        /// Construct a SerializableTreeNode from a TreeNode
        /// </summary>
        /// <param name="treeNode">a TreeNode from which a SerializableTreeNode will be constructed</param>
        public SerializableTreeNode(TreeNode treeNode) : this()
        {
            Tag = (int)treeNode.Tag;
            ImageIndex = treeNode.ImageIndex;
            Text = treeNode.Text;
            if (treeNode.Nodes != null)
            {
                foreach (TreeNode node in treeNode.Nodes)
                {
                    _items.Add(new SerializableTreeNode(node));
                }
            }
        }

        // Properties

        public int Tag { get; set; }
        public int ImageIndex { get; set; }
        public string Text { get; set; }

        public List<SerializableTreeNode> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items.Clear();
                foreach (SerializableTreeNode node in value)
                {
                    _items.Add(new SerializableTreeNode(node));
                }
            }
        }

    }
}
