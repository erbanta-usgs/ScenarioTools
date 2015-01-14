using System.Collections.Generic;
using System.Windows.Forms;

using ScenarioTools.Scene;

using SoftwareProductions.Utilities.Undo;

namespace ScenarioTools.UndoItems
{
    public class ScenarioElementChangeUndoItem : UndoItemBase
    {
        private ITaggable _oldScenarioElement;
        private ITaggable _newScenarioElement;
        private TreeView _treeView;
        private List<ITaggable> _scenarioElements;

        public ScenarioElementChangeUndoItem(ITaggable oldScenarioElement, 
                                             ITaggable newScenarioElement, 
                                             TreeView treeView, 
                                             List<ITaggable> scenarioElements)
        {
            _oldScenarioElement = (ITaggable)oldScenarioElement.Clone();
            _newScenarioElement = (ITaggable)newScenarioElement.Clone();
            _treeView = treeView;
            _scenarioElements = scenarioElements;
            Tag = oldScenarioElement.Tag;
        }

        /// <summary>
        /// Reverses the modification that this UndoItem represents.
        /// </summary>
        public override void Undo()
        {
            for (int i = 0; i < _scenarioElements.Count; i++)
            {
                if (_scenarioElements[i].Tag == (int)this.Tag)
                {
                    _scenarioElements[i] = (ITaggable)_oldScenarioElement;
                    break;
                }
            }
            _oldScenarioElement.SelectTreeNode(_treeView);
        }

        public override void Redo()
        {
            for (int i = 0; i < _scenarioElements.Count; i++)
            {
                if (_scenarioElements[i].Tag == (int)this.Tag)
                {
                    _scenarioElements[i] = (ITaggable)_newScenarioElement;
                    break;
                }
            }
            _newScenarioElement.SelectTreeNode(_treeView);
        }
    }
}
