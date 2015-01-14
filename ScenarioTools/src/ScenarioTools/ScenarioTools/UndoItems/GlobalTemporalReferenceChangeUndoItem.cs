using ScenarioTools.DataClasses;

using SoftwareProductions.Utilities.Undo;

namespace ScenarioTools.UndoItems
{
    public class GlobalTemporalReferenceChangeUndoItem : UndoItemBase
    {
        #region Fields
        private TemporalReference _oldTemporalReference;
        private TemporalReference _newTemporalReference;
        #endregion Fields

        #region Constructor
        public GlobalTemporalReferenceChangeUndoItem(TemporalReference oldTemporalReference,
            TemporalReference newTemporalReference)
        {
            _oldTemporalReference = oldTemporalReference;
            _newTemporalReference = newTemporalReference;
        }
        #endregion Constructor

        #region Abstract methods of UndoItemBase
        public override void Undo()
        {
            // Restore old global temporal reference
            GlobalStaticVariables.GlobalTemporalReference = _oldTemporalReference;
        }
        public override void Redo()
        {
            // Restore new global temporal reference
            GlobalStaticVariables.GlobalTemporalReference = _newTemporalReference;
        }
        #endregion Abstract methods of UndoItemBase

    }
}
