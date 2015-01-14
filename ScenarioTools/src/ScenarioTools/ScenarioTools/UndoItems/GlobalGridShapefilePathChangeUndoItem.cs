using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SoftwareProductions.Utilities.Undo;

namespace ScenarioTools.UndoItems
{
    public class GlobalGridShapefilePathChangeUndoItem : UndoItemBase
    {
        #region Fields
        private string _oldGridShapefilePath;
        private string _newGridShapefilePath;
        #endregion Fields

        #region Constructor
        public GlobalGridShapefilePathChangeUndoItem(string oldGridShapefilePath, string newGridShapefilePath)
        {
            _oldGridShapefilePath = oldGridShapefilePath;
            _newGridShapefilePath = newGridShapefilePath;
        }
        #endregion Constructor

        #region UndoItemBase members
        public override void Undo()
        {
            ScenarioTools.Spatial.StaticObjects.GridShapefilePath = _oldGridShapefilePath;
        }
        public override void Redo()
        {
            ScenarioTools.Spatial.StaticObjects.GridShapefilePath = _newGridShapefilePath;
        }
        #endregion UndoItemBase members
    }
}
