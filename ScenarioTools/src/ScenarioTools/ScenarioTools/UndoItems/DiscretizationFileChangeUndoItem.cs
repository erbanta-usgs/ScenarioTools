using ScenarioTools.DataClasses;

using SoftwareProductions.Utilities.Undo;

namespace ScenarioTools.UndoItems
{
    public class DiscretizationFileChangeUndoItem : UndoItemBase
    {
        private string _oldDiscretizationFileAbsolutePath;
        private string _newDiscretizationFileAbsolutePath;
        private StringHolder _discretizationFileAbsolutePath; // Will point to MainForm._discretizationFileAbsolutePath

        public DiscretizationFileChangeUndoItem(string oldDiscretizationFileAbsolutePath,
                                                string newDiscretizationFileAbsolutePath,
                                                StringHolder discretizationFileAbsolutePath)
        {
            _oldDiscretizationFileAbsolutePath = oldDiscretizationFileAbsolutePath;
            _newDiscretizationFileAbsolutePath = newDiscretizationFileAbsolutePath;
            _discretizationFileAbsolutePath = discretizationFileAbsolutePath;
        }

        public override void  Undo()
        {
 	        _discretizationFileAbsolutePath.String = _oldDiscretizationFileAbsolutePath;
        }

        public override void  Redo()
        {
 	        _discretizationFileAbsolutePath.String = _newDiscretizationFileAbsolutePath;
        }

    }
}
