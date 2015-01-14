using ScenarioTools.Scene;

using SoftwareProductions.Utilities.Undo;

namespace ScenarioTools.UndoItems
{
    public class CbcFlagChangeUndoItem : UndoItemBase
    {
        #region Fields
        private Package _package;
        private int _oldCbcFlag;
        private int _newCbcFlag;
        #endregion Fields

        public CbcFlagChangeUndoItem(Package package, int oldCbcFlag, int newCbcFlag)
        {
            _package = package;
            _oldCbcFlag = oldCbcFlag;
            _newCbcFlag = newCbcFlag;
        }

        #region UndoItemBase methods
        public override void Undo()
        {
            _package.CbcFlag = _oldCbcFlag;
        }
        public override void Redo()
        {
            _package.CbcFlag = _newCbcFlag;
        }
        #endregion UndoItemBase methods
    }

}
