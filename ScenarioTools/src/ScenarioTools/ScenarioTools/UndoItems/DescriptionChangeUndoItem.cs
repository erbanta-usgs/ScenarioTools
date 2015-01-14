using System.Windows.Forms;

using SoftwareProductions.Utilities.Undo;

namespace ScenarioTools.UndoItems
{
    public class DescriptionChangeUndoItem : UndoItemBase
    {
        /* This doesn't work because there is not a robust way to associate
         * the correct scenario element / tree node with the description change.
         */
        private string _oldDescription;
        private string _newDescription;
        private TreeView _treeView;
        private TreeNode _selectedTreeNode;
        private TextBox _textBoxDescription;

        public DescriptionChangeUndoItem(TreeView treeView, TextBox textBoxDescription, 
                                         string oldDescription, string newDescription)
        {
            _treeView = treeView;
            if (treeView.SelectedNode != null)
            {
                _selectedTreeNode = treeView.SelectedNode;
            }
            _textBoxDescription = textBoxDescription;
            _oldDescription = oldDescription; 
            _newDescription = newDescription; 
        }

        public override void Undo()
        {
            // Select original TreeNode so that correct scenario element gets revised.
            //_treeView.SelectedNode = _selectedTreeNode;
            _textBoxDescription.Text = _oldDescription;
        }

        public override void Redo()
        {
            // Select original TreeNode so that correct scenario element gets revised.
            //_treeView.SelectedNode = _selectedTreeNode;
            _textBoxDescription.Text = _newDescription;
        }
    }
}
