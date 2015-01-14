using ScenarioTools.SMDataClasses;

using SoftwareProductions.Utilities.Undo;

namespace ScenarioTools.UndoItems
{
    public class ProjectSettingsChangeUndoItem : UndoItemBase
    {
        private SMProjectSettings _oldProjectSettings;
        private SMProjectSettings _newProjectSettings;
        private SMProjectSettings _projectSettings;

        public ProjectSettingsChangeUndoItem(SMProjectSettings oldProjectSettings, 
                                             SMProjectSettings newProjectSettings, 
                                             SMProjectSettings projectSettings)
        {
            _oldProjectSettings = (SMProjectSettings)oldProjectSettings.Clone();
            _newProjectSettings = (SMProjectSettings)newProjectSettings.Clone();
            _projectSettings = projectSettings;
        }

        public override void Undo()
        {
            _projectSettings = _oldProjectSettings;
        }

        public override void Redo()
        {
            _projectSettings = _newProjectSettings;
        }
   }

}
