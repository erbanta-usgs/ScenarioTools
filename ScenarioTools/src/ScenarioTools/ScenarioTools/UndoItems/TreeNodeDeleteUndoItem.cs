﻿using System.Collections.Generic;
using System.Windows.Forms;

using ScenarioTools.Scene;

using SoftwareProductions.Utilities.Undo;

namespace ScenarioTools.UndoItems
{
    public class TreeNodeDeleteUndoItem : UndoItemBase
    {
        private TreeView _treeView;
        private TreeNode _treeNode;
        private int _treeLevel;
        private int _index0;
        private int _index1;
        private int _index2;
        private List<ITaggable> _scenarioElements;
        private ITaggable _taggableItem;

        /// <summary>
        /// Initializes a new instance of the TreeNodeDeleteUndoItem class, 
        /// with the specified TreeView, TreeNode, Tree level, and Index values.
        /// </summary>
        /// <param name="list">The TreeView that this undo item represents a delete operation on.</param>
        /// <param name="item">The TreeNode that was deleted from the list.</param>
        /// <param name="treeLevel">The tree level of the deleted item.</param>
        /// <param name="index0">
        /// The index at tree-level 0 that the item was deleted from. 
        /// </param>
        /// <param name="index1">
        /// The index at tree-level 1 that the item was deleted from if treeLevel > 0.  
        /// </param>
        /// <param name="index2">
        /// The index at tree-level 2 that the item was deleted from if treeLevel > 1.  
        /// </param>
        public TreeNodeDeleteUndoItem(TreeView treeView, TreeNode treeNode, int treeLevel, int index0, int index1, int index2, List<ITaggable> scenarioElements, ITaggable taggableItem)
		{
			_treeView = treeView;
			_treeNode = (TreeNode)treeNode.Clone();
            _treeLevel = treeLevel;
			_index0 = index0;
            _index1 = index1;
            _index2 = index2;
            _scenarioElements = scenarioElements;
            _taggableItem = (ITaggable)taggableItem.Clone();
            Tag = (int)treeNode.Tag;
        }

        /// <summary>
        /// Reverses the modification that this UndoItem represents.
        /// </summary>
        public override void Undo()
        {
            // Restore taggable item to _scenarioElements
            _scenarioElements.Add(_taggableItem);

            if (_index0 >= 0)
            {
                if (_treeLevel == 0)
                {
                    _treeView.Nodes.Insert(_index0, _treeNode);
                }
                else if (_treeLevel == 1)
                {
                    _treeView.Nodes[_index0].Nodes.Insert(_index1, _treeNode);
                    // Add package to its Parent's Items
                    ITaggable parent = _taggableItem.Parent;
                    if (parent != null)
                    {
                        parent.Items.Add(_taggableItem);
                    }
                }
                else if (_treeLevel == 2)
                {
                    _treeView.Nodes[_index0].Nodes[_index1].Nodes.Insert(_index2, _treeNode);
                    // Add featureset to its Parent's Items
                    ITaggable parent = _taggableItem.Parent;
                    if (parent != null)
                    {
                        parent.Items.Add(_taggableItem);
                    }
                }
                _treeView.Nodes[_index0].Expand();
            }
            else
            {
                _treeView.Nodes.Add(_treeNode);
            }
            _treeView.SelectedNode = _treeNode;
        }

        /// <summary>
        /// Re-performs the previously undone modification that this UndoItem represents.
        /// </summary>
        public override void Redo()
        {
            // Re-delete taggable Item from _scenarioElements
            ITaggable deletedItem = null;
            foreach (ITaggable e in _scenarioElements)
            {
                if (e.Tag == (int)this.Tag)
                {
                    deletedItem = e;
                }
            }
            if (deletedItem != null)
            {
                _scenarioElements.Remove(deletedItem);
            }

            if (_index0 >= 0)
            {
                if (_treeLevel == 0)
                {
                    _treeView.Nodes[_index0].Remove();
                }
                else if (_treeLevel == 1)
                {
                    _treeView.Nodes[_index0].Nodes[_index1].Remove();
                    // Remove package from its Parent's Items
                    ITaggable parent = _taggableItem.Parent;
                    if (parent != null)
                    {
                        parent.Items.Remove(_taggableItem);
                    }
                }
                else if (_treeLevel == 2)
                {
                    _treeView.Nodes[_index0].Nodes[_index1].Nodes[_index2].Remove();
                    // Remove featureset from its Parent's Items
                    ITaggable parent = _taggableItem.Parent;
                    if (parent != null)
                    {
                        parent.Items.Remove(_taggableItem);
                    }
                }
            }
            else
            {
                _treeView.Nodes.Remove(_treeNode);
            }
        }
    }
}