using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;

using ScenarioTools.DataClasses;
using ScenarioTools.Geometry;
using ScenarioTools.Reporting;
using USGS.Puma.Modflow;

namespace ScenarioTools
{
    /// <summary>
    /// Contains global variables to be used in either SM or SA
    /// </summary>
    public static class GlobalStaticVariables
    {
        #region Private fields
        private static string _backgroundImageFile;
        #endregion Private fields

        #region Public properties
        public static BackgroundWorker GlobalBackgroundWorker { get; set; }
        public static Bitmap GlobalBitmap { get; set; }
        public static ImageLayer BackgroundImageLayer { get; set; }
        public static string BackgroundImageFile 
        {
            get
            {
                return _backgroundImageFile;
            }
            set
            {
                if (value != _backgroundImageFile)
                {
                    BackgroundImageLayer = null;
                    GlobalBitmap = null;
                    removeBackgroundImageExtentFromList();
                }
                _backgroundImageFile = value;
            }
        }
        public static string CurrentDirectory { get; set; }
        public static List<Extent> Extents { private get; set; }
        public static DataClasses.TemporalReference GlobalTemporalReference { get; set; }
        public static DataClasses.LengthReference.ModflowLengthUnit GlobalLengthUnit { get; set; }
        public static ModflowNameData ModflowNameData { get; set; }
        public static DisFileData DisFileData { get; set; }
        public static BasFileData BasFileData { get; set; }
        public static DataClasses.MapEnums.BlankingMode BlankingMode { get; set; }
        public static int BlankingLayer { get; set; }
        public static bool[,] Blanking { get; set; }
        #endregion Public properties

        #region Delegates
        public static MyDelegate DoEvents;
        public delegate void MyDelegate();
        #endregion Delegates

        #region Public methods
        public static void DefineBlanking(DataClasses.MapEnums.BlankingMode blankingMode, int blankingLayer)
        {
            BlankingMode = blankingMode;
            BlankingLayer = blankingLayer;
            defineBlanking();
        }
        public static void Initialize()
        {
            _backgroundImageFile = "";
            BackgroundImageFile = ""; // Property does some processing
            GlobalBackgroundWorker = null;
            GlobalBitmap = null;
            BackgroundImageLayer = null;
            CurrentDirectory = "";
            Extents=new List<Extent>();
            GlobalTemporalReference = new TemporalReference(TimeHelper.DefaultDateTime(), TemporalReference.ModflowTimeUnit.seconds);
            GlobalLengthUnit = new LengthReference.ModflowLengthUnit();
            ModflowNameData = new ModflowNameData();
            DisFileData = null;
            BasFileData = null;
            BlankingMode = MapEnums.BlankingMode.None;
            BlankingLayer = 1;
            Blanking = null;
        }
        #endregion Public methods

        #region Private methods
        private static void defineBlanking()
        {
            try
            {
                int nLay = DisFileData.LayerCount;
                int nRow = DisFileData.RowCount;
                int nCol = DisFileData.ColumnCount;
                int iplus1, jplus1;
                USGS.Puma.Modflow.ModflowDataArray2d<int> iBound2d;
                switch (BlankingMode)
                {
                    case DataClasses.MapEnums.BlankingMode.None:
                        break;
                    case DataClasses.MapEnums.BlankingMode.BySpecifiedLayer:
                        Blanking = new bool[nRow, nCol];
                        iBound2d = (USGS.Puma.Modflow.ModflowDataArray2d<int>)BasFileData.GetIBound(BlankingLayer);
                        for (int i = 0; i < nRow; i++)
                        {
                            iplus1 = i + 1;
                            for (int j = 0; j < nCol; j++)
                            {
                                jplus1 = j + 1;
                                Blanking[i, j] = iBound2d.DataArray[iplus1, jplus1] == 0;
                            }
                        }
                        break;
                    case DataClasses.MapEnums.BlankingMode.AnyLayerInactive:
                        Blanking = new bool[nRow, nCol];
                        for (int i = 0; i < nRow; i++)
                        {
                            for (int j = 0; j < nCol; j++)
                            {
                                Blanking[i, j] = false;
                            }
                        }
                        for (int k = 1; k <= nLay; k++)
                        {
                            iBound2d = (USGS.Puma.Modflow.ModflowDataArray2d<int>)BasFileData.GetIBound(k);
                            for (int i = 0; i < nRow; i++)
                            {
                                iplus1 = i + 1;
                                for (int j = 0; j < nCol; j++)
                                {
                                    jplus1 = j + 1;
                                    if (iBound2d.DataArray[iplus1, jplus1] == 0)
                                    {
                                        Blanking[i, j] = true;
                                    }
                                }
                            }
                        }
                        break;
                    case DataClasses.MapEnums.BlankingMode.AllLayersInactive:
                        Blanking = new bool[nRow, nCol];
                        for (int i = 0; i < nRow; i++)
                        {
                            for (int j = 0; j < nCol; j++)
                            {
                                Blanking[i, j] = true;
                            }
                        }
                        for (int k = 1; k <= nLay; k++)
                        {
                            iBound2d = (USGS.Puma.Modflow.ModflowDataArray2d<int>)BasFileData.GetIBound(k);
                            for (int i = 0; i < nRow; i++)
                            {
                                iplus1 = i + 1;
                                for (int j = 0; j < nCol; j++)
                                {
                                    jplus1 = j + 1;
                                    if (iBound2d.DataArray[iplus1, jplus1] != 0)
                                    {
                                        Blanking[i, j] = false;
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            catch
            {
            }
        }
        private static void removeBackgroundImageExtentFromList()
        {
            if (Extents != null)
            {
                for (int i = 0; i < Extents.Count; i++)
                {
                    if (Extents[i].Name == WorkspaceUtil.BACKGROUND_IMAGE_EXTENT_NAME)
                    {
                        Extents.Remove(Extents[i]);
                        break;
                    }
                }
            }
        }
        #endregion Private methods
    }
}
