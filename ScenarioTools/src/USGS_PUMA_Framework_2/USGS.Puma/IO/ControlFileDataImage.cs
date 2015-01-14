using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;

namespace USGS.Puma.IO
{
    public class ControlFileDataImage : KeyedCollection<string,ControlFileBlock>
    {
        #region Fields
        private string _WorkingDirectory = "";
        private string _Filename = "";
        private string _LocalFilename = "";
        #endregion

        #region Constructors
        public ControlFileDataImage()
            : base()
        { }

        public ControlFileDataImage(string localFilename, string workingDirectory)
            : this()
        {
            LocalFilename = localFilename;
            WorkingDirectory = workingDirectory;
        }

        #endregion

        #region Overridden Methods
        protected override string GetKeyForItem(ControlFileBlock item)
        {
            return item.BlockName;
        }
        #endregion

        #region Properties
        public string LocalFilename
        {
            get { return _LocalFilename; }
            private set { _LocalFilename = value; }
        }

        public string Filename
        {
            get { return Path.Combine(WorkingDirectory,LocalFilename); }
        }

        public string WorkingDirectory
        {
            get { return _WorkingDirectory; }
            private set { _WorkingDirectory = value; }
        }


        #endregion

        #region Public Methods
        public void SetWorkingDirectoryForAllItems()
        {
            foreach (ControlFileBlock block in this)
            {
                block.WorkingDirectory = this.WorkingDirectory;
            }
        }

        public bool ContainsKey(string blockType, string blockLabel, string recordKey)
        {
            string block = blockType.Trim().ToLower();
            string label = blockLabel.Trim().ToLower();
            if (!string.IsNullOrEmpty(label))
            {
                block = block + ":" + label;
            }
            return ContainsKey(block, recordKey);
        }

        public bool ContainsKey(string block, string recordKey)
        {
            string blockName = block.Trim().ToLower();
            if (!ContainsBlock(blockName))
            { return false; }
            ControlFileBlock blockData = this[blockName];
            string key = BuildKey(recordKey);
            return blockData.Contains(key);
        }

        public bool ContainsBlock(string block)
        {
            string blockName = block.Trim().ToLower();
            return this.Contains(blockName);
        }

        public string[] GetBlockNames()
        {
            return this.Dictionary.Keys.ToArray();
        }

        public string[] GetBlockNames(string blockType)
        {
            string type = blockType.Trim().ToLower();
            List<string> items = new List<string>();
            foreach(ControlFileBlock item in this)
            {
                if (item.BlockType == type)
                {
                    items.Add(item.BlockName);
                }
            }
            return items.ToArray();
        }

        public ControlFileItem GetItemRecord(string block, string recordKey)
        {
            string blockName = block.Trim().ToLower();
            if (!this.ContainsBlock(blockName))
                return null;

            ControlFileBlock blockData = this[blockName];
            if (!blockData.Contains(recordKey))
                return null;

            return blockData[recordKey];

        }

        // Get and Set single value items
        public string GetValueAsText(string block, string recordKey)
        {
            ControlFileItem data = GetItemRecord(block, recordKey);
            if (data == null)
            {
                throw new Exception("Data record was not found.");
            }
            else
            {
                return data.GetValueAsText();
            }
        }

        public int GetValueAsInteger(string block, string recordKey)
        {
            ControlFileItem data = GetItemRecord(block, recordKey);
            if (data == null)
            {
                throw new Exception("Data record was not found.");
            }
            else
            {
                return data.GetValueAsInteger();
            }
        }

        public float GetValueAsSingle(string block, string recordKey)
        {
            ControlFileItem data = GetItemRecord(block, recordKey);
            if (data == null)
            {
                throw new Exception("Data record was not found.");
            }
            else
            {
                return data.GetValueAsSingle();
            }

        }

        public double GetValueAsDouble(string block, string recordKey)
        {
            ControlFileItem data = GetItemRecord(block, recordKey);
            if (data == null)
            {
                throw new Exception("Data record was not found.");
            }
            else
            {
                return data.GetValueAsDouble();
            }

        }

        public bool GetValueAsBoolean(string block, string recordKey)
        {
            ControlFileItem data = GetItemRecord(block, recordKey);
            if (data == null)
            {
                throw new Exception("Data record was not found.");
            }
            else
            {
                return data.GetValueAsBoolean();
            }

        }






        public void SetValue(string block, string recordKey, string value)
        {
            ControlFileItem data = GetItemRecord(block, recordKey);
            if (data == null)
            {
                throw new Exception("Data record was not found.");
            }
            else
            {
                data.SetValue(value);
            }
        }
        public void SetValue(string block, string recordKey, int value)
        {
            ControlFileItem data = GetItemRecord(block, recordKey);
            if (data == null)
            {
                throw new Exception("Data record was not found.");
            }
            else
            {
                data.SetValue(value);
            }
        }
        public void SetValue(string block, string recordKey, float value)
        {
            ControlFileItem data = GetItemRecord(block, recordKey);
            if (data == null)
            {
                throw new Exception("Data record was not found.");
            }
            else
            {
                data.SetValue(value);
            }
        }
        public void SetValue(string block, string recordKey, double value)
        {
            ControlFileItem data = GetItemRecord(block, recordKey);
            if (data == null)
            {
                throw new Exception("Data record was not found.");
            }
            else
            {
                data.SetValue(value);
            }
        }
        public void SetValue(string block, string recordKey, bool value)
        {
            ControlFileItem data = GetItemRecord(block, recordKey);
            if (data == null)
            {
                throw new Exception("Data record was not found.");
            }
            else
            {
                data.SetValue(value);
            }
        }


        #endregion

        #region Private Methods

        private string BuildKey(string text)
        {
            char[] spaceDelimiter = new char[1];
            spaceDelimiter[0] = ' ';

            string line = text.Trim();
            string[] lines = line.Split(spaceDelimiter, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();
            for (int n = 0; n < lines.Length; n++)
            {
                sb.Append(lines[n]).Append(" ");
            }

            line = sb.ToString().Trim();
            return line;
        }


        #endregion


    }
}
