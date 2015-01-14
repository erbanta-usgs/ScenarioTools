using ScenarioTools.DataClasses;

using SoftwareProductions.Utilities.Undo;

namespace ScenarioTools.UndoItems
{
    public class ModelGridChangeUndoItem : UndoItemBase
    {
        private string _oldModelGridShapefileAbsolutePath;
        private string _newModelGridShapefileAbsolutePath;
        private StringHolder _modelGridShapefileAbsolutePath; // Will point to MainForm._gridShapefileAbsolutePath

        public ModelGridChangeUndoItem(string oldModelGridShapefileAbsolutePath, 
                                       string newModelGridShapefileAbsolutePath,
                                       StringHolder modelGridShapefileAbsolutePath)
        {
            _oldModelGridShapefileAbsolutePath = oldModelGridShapefileAbsolutePath;
            _newModelGridShapefileAbsolutePath = newModelGridShapefileAbsolutePath;
            _modelGridShapefileAbsolutePath = modelGridShapefileAbsolutePath;
        }

        public override void Undo()
        {
            _modelGridShapefileAbsolutePath.String = _oldModelGridShapefileAbsolutePath;
        }

        public override void Redo()
        {
            _modelGridShapefileAbsolutePath.String = _newModelGridShapefileAbsolutePath;
        }

    }
}
