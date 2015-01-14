using System.Collections.Generic;
using System.Windows.Forms;

using ScenarioTools.Scene;

using SoftwareProductions.Utilities.Undo;

namespace ScenarioTools.UndoItems
{
    public class ScenarioCopyUndoItem : UndoItemBase
    {
        private TreeView _treeView;
        private List<ITaggable> _owner;
        private List<TagLink> _tagLinks;
        private TreeNode _addedTreeNode;
        private Scenario _addedScenario;
        private TagLink _addedLink;
        private int _scenarioIndex;
        private int _linkIndex;

        public ScenarioCopyUndoItem(TreeView treeView, List<ITaggable> owner, 
                                    List<TagLink> tagLinks, int newScenarioIndex, 
                                    int newLinkIndex)
        {
            // Store pointers and value types
            _treeView = treeView;
            _owner = owner;
            _tagLinks = tagLinks;
            _scenarioIndex = newScenarioIndex;
            _linkIndex = newLinkIndex;

            // Clone and store added objects
            int nodeIndex = treeView.Nodes.Count - 1;
            _addedTreeNode = (TreeNode)treeView.Nodes[nodeIndex].Clone();
            _addedScenario = (Scenario)owner[newScenarioIndex].Clone();
            _addedLink = (TagLink)tagLinks[_linkIndex].Clone();
        }

        public override void Undo()
        {
            int lastElement = _owner.Count - 1;
            for (int i = lastElement; i >= _scenarioIndex; i--)
            {
                _tagLinks.Remove(_owner[i].Link);
                _owner.Remove(_owner[i]);
            }

            int nodeIndex = _treeView.Nodes.Count - 1;
            _treeView.Nodes[nodeIndex].Remove();
        }

        public override void Redo()
        {
            _treeView.Nodes.Add(_addedTreeNode);
            _owner.Add(_addedScenario);
            _addedScenario.Tag = _addedLink.Tag;
            _tagLinks.Add(_addedLink);
            _owner[_owner.Count - 1].ConnectList(_owner);
            _treeView.Nodes[_treeView.Nodes.Count - 1].ExpandAll();
        }
    }
}
