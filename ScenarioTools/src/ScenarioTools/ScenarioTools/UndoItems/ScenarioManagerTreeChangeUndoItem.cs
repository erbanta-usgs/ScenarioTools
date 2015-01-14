using System.Collections.Generic;
using System.Windows.Forms;

using ScenarioTools.Scene;

using SoftwareProductions.Utilities.Undo;

namespace ScenarioTools.UndoItems
{
    public class ScenarioManagerTreeChangeUndoItem : UndoItemBase
    {
        #region Fields
        private TreeView _treeView;
        private TreeView _treeViewOld;
        private TreeView _treeViewNew;
        private List<ITaggable> _scenarioElements;
        private List<ITaggable> _scenarioElementsOld;
        private List<ITaggable> _scenarioElementsNew;
        #endregion Fields

        public ScenarioManagerTreeChangeUndoItem(TreeView treeView, TreeView treeViewOld, 
            TreeView treeViewNew, List<ITaggable> scenarioElements, 
            List<ITaggable> scenarioElementsOld, List<ITaggable> scenarioElementsNew)
        {
            // Store TreeNodes
            _treeView = treeView;
            _treeViewOld = new TreeView();
            for (int i = 0; i < treeViewOld.Nodes.Count; i++)
            {
                _treeViewOld.Nodes.Add((TreeNode)treeViewOld.Nodes[i].Clone());
            }
            _treeViewNew = new TreeView();
            for (int i = 0; i < treeViewNew.Nodes.Count; i++)
            {
                _treeViewNew.Nodes.Add((TreeNode)treeViewNew.Nodes[i].Clone());
            }

            // Store scenario elements
            _scenarioElements = scenarioElements;
            _scenarioElementsOld = new List<ITaggable>();
            for (int i = 0; i < scenarioElementsOld.Count; i++)
            {
                _scenarioElementsOld.Add((ITaggable)scenarioElementsOld[i].Clone());
            }
            _scenarioElementsNew = new List<ITaggable>();
            for (int i = 0; i < scenarioElementsNew.Count; i++)
            {
                _scenarioElementsNew.Add((ITaggable)scenarioElementsNew[i].Clone());
            }
        }

        #region Abstract methods of UndoItemBase
        public override void Undo()
        {
            // Undo changes to the TreeView
            _treeView.BeginUpdate();
            _treeView.Nodes.Clear();
            for (int i = 0; i < _treeViewOld.Nodes.Count; i++)
            {
                _treeView.Nodes.Add(makeTreeNode(_treeViewOld.Nodes[i]));
            }
            _treeView.EndUpdate();

            // Undo changes to scenarioElements
            _scenarioElements.Clear();
            for (int i = 0; i < _scenarioElementsOld.Count; i++)
            {
                _scenarioElements.Add(_scenarioElementsOld[i]);
            }
        }
        public override void Redo()
        {
            // Redo changes to the TreeView
            _treeView.BeginUpdate();
            _treeView.Nodes.Clear();
            for (int i = 0; i < _treeViewNew.Nodes.Count; i++)
            {
                _treeView.Nodes.Add(makeTreeNode(_treeViewNew.Nodes[i]));
            }
            _treeView.EndUpdate();

            // Redo changes to scenarioElements
            _scenarioElements.Clear();
            for (int i = 0; i < _scenarioElementsNew.Count; i++)
            {
                _scenarioElements.Add(_scenarioElementsNew[i]);
            }
        }
        #endregion Abstract methods of UndoItemBase

        private TreeNode makeTreeNode(TreeNode treeNode)
        {
            TreeNode newTreeNode = new TreeNode(treeNode.Text, treeNode.ImageIndex, 
                                                treeNode.SelectedImageIndex);
            newTreeNode.Tag = (int)treeNode.Tag;
            for (int i = 0; i < treeNode.Nodes.Count; i++)
            {
                newTreeNode.Nodes.Add(makeTreeNode(treeNode.Nodes[i]));
            }
            return newTreeNode;
        }
    }
}
