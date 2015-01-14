using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ScenarioTools.Util;

namespace ScenarioTools.SMDataClasses
{
    public class SMProjectSettings : ICloneable
    {
        #region Fields

        private List<string> _nameFiles;

        #endregion Fields

        public SMProjectSettings()
        {
            MasterNameFile = "";
            _nameFiles = new List<string>();
            ModflowExecutable = "";
            BackgroundImageFile = "";
            MaxSimultaneousRuns = 1;
        }

        #region Properties

        public string MasterNameFile { get; set; }

        public int MaxSimultaneousRuns { get; set; }

        public string ModflowExecutable { get; set; }

        public string BackgroundImageFile { get; set; }

        public List<string> NameFiles
        {
            get
            {
                return _nameFiles;
            }
            set
            {
                _nameFiles = value;
            }
        }

        #endregion Properties

        #region Methods

        public object Clone()
        {
            SMProjectSettings smProjectSettingsClone = new SMProjectSettings();
            smProjectSettingsClone.MasterNameFile = this.MasterNameFile;
            for (int i = 0; i < this.NameFiles.Count; i++)
            {
                smProjectSettingsClone.NameFiles.Add(this.NameFiles[i]);
            }
            smProjectSettingsClone.MaxSimultaneousRuns = this.MaxSimultaneousRuns;
            smProjectSettingsClone.ModflowExecutable = this.ModflowExecutable;
            smProjectSettingsClone.BackgroundImageFile = this.BackgroundImageFile;
            return smProjectSettingsClone;
        }

        /// <summary>
        /// Converts MasterNameFile and all items in NameFiles to absolute paths
        /// </summary>
        /// <param name="refDir"></param>
        public void ConvertToAbsolute(string refDir)
        {
            MasterNameFile = FileUtil.Relative2Absolute(MasterNameFile, refDir);
            for (int i=0;i<NameFiles.Count;i++)
            {
                NameFiles[i] = FileUtil.Relative2Absolute(NameFiles[i], refDir);
            }
        }

        /// <summary>
        /// Converts MasterNameFile and all items in NameFiles to relative paths
        /// </summary>
        /// <param name="refDir"></param>
        public void ConvertToRelative(string refDir)
        {
            MasterNameFile = FileUtil.GetRelativePath(MasterNameFile, refDir);
            for (int i = 0; i < NameFiles.Count; i++)
            {
                NameFiles[i] = FileUtil.GetRelativePath(NameFiles[i], refDir);
            }
        }

        #endregion Methods
    }
}
