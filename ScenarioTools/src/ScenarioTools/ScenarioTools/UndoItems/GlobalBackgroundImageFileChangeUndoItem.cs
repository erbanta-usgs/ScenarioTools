using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SoftwareProductions.Utilities.Undo;

namespace ScenarioTools.UndoItems
{
    public class GlobalBackgroundImageFileChangeUndoItem : UndoItemBase
    {
        #region Fields
        private readonly string _oldBackgroundImageFile;
        private readonly string _newBackgroundImageFile;
        #endregion Fields

        #region Constructor
        public GlobalBackgroundImageFileChangeUndoItem(string oldBackgroundImageFile, string newBackgroundImageFile)
        {
            _oldBackgroundImageFile = oldBackgroundImageFile;
            _newBackgroundImageFile = newBackgroundImageFile;
        }
        #endregion Constructor

        #region UndoItemBase abstract methods
        public override void Undo()
        {
            ScenarioTools.GlobalStaticVariables.BackgroundImageFile = _oldBackgroundImageFile;
        }
        public override void Redo()
        {
            ScenarioTools.GlobalStaticVariables.BackgroundImageFile = _newBackgroundImageFile;
        }
        #endregion UndoItemBase abstract methods
    }
}
