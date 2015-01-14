using ScenarioTools.DataClasses;

using SoftwareProductions.Utilities.Undo;

namespace ScenarioTools.UndoItems
{
    public class NamefileChangeUndoItem : UndoItemBase
    {
        private string _oldNamefileRelativePath;
        private string _newNamefileRelativePath;
        private StringHolder _namefileRelativePath; // Will point to MainForm._namefileRelativePath

        public NamefileChangeUndoItem(string oldNamefileRelativePath, string newNamefileRelativePath, StringHolder namefileRelativePath)
        {
            _oldNamefileRelativePath = oldNamefileRelativePath;
            _newNamefileRelativePath = newNamefileRelativePath;
            _namefileRelativePath = namefileRelativePath;
        }

        public override void Undo()
        {
            _namefileRelativePath.String = _oldNamefileRelativePath;
        }

        public override void Redo()
        {
            _namefileRelativePath.String = _newNamefileRelativePath;
        }
    }
}
