using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScenarioManager.ProjectSettings
{
    public enum FileNameConvention 
    {
        /// <summary>
        /// Names of output files include scenario name; files reside in master folder
        /// </summary>
        FileScenario = 0,       
        /// <summary>
        /// Names of output files are as listed in name file; files reside in scenario folder
        /// </summary>
        FolderScenario = 1,
        /// <summary>
        /// Names of output files include scenario name; files reside in scenario folder
        /// </summary>
        FolderFileScenario = 2
    }

    public class SMProjectSettings : ICloneable
    {
        #region Fields

        private string _masterNameFile;
        private List<string> _nameFiles;
        private FileNameConvention _nameConvention;

        #endregion Fields

        public SMProjectSettings()
        {
            _masterNameFile = "";
            _nameFiles = new List<string>();
            _nameConvention = FileNameConvention.FolderFileScenario;
        }

        #region Properties

        public string MasterNameFile
        {
            get
            {
                return _masterNameFile;
            }
            set
            {
                _masterNameFile = value;
            }
        }

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

        public FileNameConvention NameConvention
        {
            get
            {
                return _nameConvention;
            }
            set
            {
                _nameConvention = value;
            }
        }

        public object Clone()
        {
            SMProjectSettings smProjectSettingsClone = new SMProjectSettings();
            smProjectSettingsClone.MasterNameFile = this.MasterNameFile;
            smProjectSettingsClone.NameConvention = this.NameConvention;
            for (int i = 0; i < this.NameFiles.Count; i++)
            {
                smProjectSettingsClone.NameFiles.Add(this.NameFiles[i]);
            }
            return smProjectSettingsClone;
        }

        #endregion Properties


    }
}
