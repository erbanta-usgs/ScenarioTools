using System.Windows.Forms;

using SoftwareProductions.Utilities.Undo;

namespace ScenarioTools.UndoItems
{
    public class TreeNodeRenameUndoItem : UndoItemBase
    {
        private TreeView _treeView;
        private int _treeLevel;
        private string _oldText;
        private string _newText;
        private int _level0Index;
        private int _level1Index;
        private int _level2Index;

		/// <summary>
		/// Initializes a new instance of the TreeNodeRenameUndoItem class, 
		/// with the specified TreeView, TreeNode, old name, and new name.
		/// </summary>
		/// <param name="treeView">The TreeView that this undo item represents a move operation on.</param>
		/// <param name="treeNode">The TreeNode that was renamed, in its position in TreeView.</param>
        /// <param name="newNodeText">Name of the TreeNode before being renamed.</param>
        public TreeNodeRenameUndoItem(TreeView treeView, TreeNode treeNode, string newNodeText)
        {
            _treeView = treeView;
            _treeLevel = treeNode.Level;
            _newText = newNodeText;
            _oldText = treeNode.Text;
            _level0Index = _level1Index = _level2Index = -1;
            switch (_treeLevel)
            {
                case 0:
                    _level0Index = treeNode.Index;
                    break;
                case 1:
                    _level1Index = treeNode.Index;
                    _level0Index = treeNode.Parent.Index;
                    break;
                case 2:
                    _level2Index = treeNode.Index;
                    _level1Index = treeNode.Parent.Index;
                    _level0Index = treeNode.Parent.Parent.Index;
                    break;
            }
        }

        /// <summary>
        /// Reverses the modification that this UndoItem represents.
        /// </summary>
        public override void Undo()
        {
            switch (_treeLevel)
            {
                case 0:
                    _treeView.Nodes[_level0Index].Text = _oldText;
                    break;
                case 1:
                    _treeView.Nodes[_level0Index].Nodes[_level1Index].Text = _oldText;
                    break;
                case 2:
                    _treeView.Nodes[_level0Index].Nodes[_level1Index].Nodes[_level2Index].Text = _oldText;
                    break;
            }
        }

        /// <summary>
        /// Re-performs the previously undone modification that this UndoItem represents.
        /// </summary>
        public override void Redo()
        {
            switch (_treeLevel)
            {
                case 0:
                    _treeView.Nodes[_level0Index].Text = _newText;
                    break;
                case 1:
                    _treeView.Nodes[_level0Index].Nodes[_level1Index].Text = _newText;
                    break;
                case 2:
                    _treeView.Nodes[_level0Index].Nodes[_level1Index].Nodes[_level2Index].Text = _newText;
                    break;
            }
        }
    }
}
