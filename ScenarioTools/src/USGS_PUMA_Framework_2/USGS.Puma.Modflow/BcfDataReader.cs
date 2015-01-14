﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using USGS.Puma;
using USGS.Puma.Core;
using USGS.Puma.IO;
using USGS.Puma.Utilities;
using USGS.Puma.Modflow;

namespace USGS.Puma.Modflow
{
    public class BcfDataReader
    {
        #region Public Methods
        public BcfFileData Read(ModflowNameData nameData, int layerCount, int rowCount, int columnCount)
        {
            List<string> tokens = null;
            IGenericNumberUtility<float> gnu = new GenericNumberUtility() as IGenericNumberUtility<float>;
            BcfFileData bcfData = null;
            ModflowDataArrayReader<float> mdaReader = null;

            string filename = "";
            List<NameFileItem> nfItems = nameData.GetItemsAsList("BCF6");
            if (nfItems.Count == 0)
                throw new Exception("Name file data does not include a BCF file.");

            filename = nfItems[0].FileName;
            if (string.IsNullOrEmpty(filename))
                throw new Exception("The BCF filename is invalid.");

            if (!Path.IsPathRooted(filename))
                filename = Path.Combine(nameData.ParentDirectory, filename);

            // Open file and read data
            using (StreamReader reader = new StreamReader(filename))
            {
                mdaReader = new ModflowDataArrayReader<float>(reader, nameData);
                string line = null;

                bcfData = new BcfFileData(layerCount, rowCount, columnCount);

                // Read the comment lines.
                List<string> comments = new List<string>();
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (line[0] != '#') break;
                    comments.Add(line);
                }
                // Add comments
                for (int i = 0; i < comments.Count; i++)
                { bcfData.Comments.Add(comments[i]); }

                // The current line should be the first line, so process it.
                tokens = StringUtility.ParseAsFortranFreeFormat(line, false);
                if (tokens != null)
                {
                    bcfData.IBcfCb = int.Parse(tokens[0]);
                    bcfData.HDry = gnu.Parse(tokens[1]);
                    bcfData.IWdFlg = int.Parse(tokens[2]);
                    bcfData.WetFct = gnu.Parse(tokens[3]);
                    bcfData.IWetIt = int.Parse(tokens[4]);
                    bcfData.IHdWet = int.Parse(tokens[5]);
                }


                // Return the bcfData object
                return bcfData;

            }
        }
        public BcfFileData Read(string modflowNameFile, int layerCount, int rowCount, int columnCount)
        {
            // Read the MODFLOW name file
            ModflowNameData nfData = ModflowNameFileReader.Read(modflowNameFile);

            // If nfData is null it means something went wrong reading the name
            // file. If so, return null.
            if (nfData == null) return null;

            // The nfData has valid name file data, so read the DIS file.
            return Read(nfData, layerCount, rowCount, columnCount);

        }
        #endregion

    }
}
