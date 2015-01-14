using System.Windows.Forms;

using SoftwareProductions.Utilities.Undo;

namespace ScenarioTools.UndoItems
{
    public class TreeNodeMoveUndoItem : UndoItemBase
    {
        // This undo item only supports tree-node moves where the tree node does not
        // change tree levels as a result of the move.
        private TreeView _treeView;
        private TreeNode _treeNode;
        private int _toTreeLevel;
        private int _fromIndex0;
        private int _fromIndex1;
        private int _fromIndex2;
        private int _toIndex0;
        private int _toIndex1;
        private int _toIndex2;

		/// <summary>
		/// Initializes a new instance of the TreeNodeMoveUndoItem class, 
		/// with the specified TreeView, TreeNode, Tree Level, and Index values.
		/// </summary>
		/// <param name="treeView">The TreeView that this undo item represents a move operation on.</param>
		/// <param name="treeNode">The TreeNode that was moved.</param>
        /// <param name="toTreeLevel">The tree level of node that becomes parent of TreeNode after move.</param>
		/// <param name="fromIndex0">The tree-level 0 index of TreeNode before move.</param>
        /// <param name="fromIndex1">The tree-level 1 index of TreeNode before move.</param>
        /// <param name="fromIndex2">The tree-level 2 index of TreeNode before move.</param>
        /// <param name="toIndex0">The tree-level 0 index of TreeNode after move.</param>
        /// <param name="toIndex1">The tree-level 1 index of TreeNode after move.</param>
        /// <param name="toIndex2">The tree-level 2 index of TreeNode after move.</param>
		public TreeNodeMoveUndoItem(TreeView treeView, TreeNode treeNode, int toTreeLevel, 
                   int fromIndex0, int fromIndex1, int fromIndex2, int toIndex0, int toIndex1, int toIndex2)
		{
			_treeView = treeView;
			_treeNode = (TreeNode)treeNode.Clone();
            _toTreeLevel = toTreeLevel;
			_fromIndex0 = fromIndex0;
            _fromIndex1 = fromIndex1;
            _fromIndex2 = fromIndex2;
            _toIndex0 = toIndex0;
            _toIndex1 = toIndex1;
            _toIndex2 = toIndex2;
        }

        /// <summary>
        /// Reverses the modification that this UndoItem represents.
        /// </summary>
        public override void Undo()
        {
            switch (_toTreeLevel)
            {
                case 0:
                    _treeView.Nodes[_toIndex0].Nodes[_toIndex1].Remove();
                    _treeView.Nodes[_fromIndex0].Nodes.Insert(_fromIndex1, _treeNode);
                    break;
                case 1:
                    _treeView.Nodes[_toIndex0].Nodes[_toIndex1].Nodes[_toIndex2].Remove();
                    _treeView.Nodes[_fromIndex0].Nodes[_fromIndex1].Nodes.Insert(_fromIndex2, _treeNode);
                    break;
                case 2:
                    // should never happen
                    break;
            }
        }

        /// <summary>
        /// Re-performs the previously undone modification that this UndoItem represents.
        /// </summary>
        public override void Redo()
        {
            switch (_toTreeLevel)
            {
                case 0:
                    _treeView.Nodes[_fromIndex0].Nodes[_fromIndex1].Remove();
                    _treeView.Nodes[_toIndex0].Nodes.Insert(_toIndex1, _treeNode);
                    break;
                case 1:
                    _treeView.Nodes[_fromIndex0].Nodes[_fromIndex1].Nodes[_fromIndex2].Remove();
                    _treeView.Nodes[_toIndex0].Nodes[_toIndex1].Nodes.Insert(_toIndex2, _treeNode);
                    break;
                case 2:
                    // should never happen
                    break;
            }
        }

    }
}
