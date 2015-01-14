using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using USGS.Puma;
using USGS.Puma.Core;
using USGS.Puma.FiniteDifference;
using USGS.Puma.Utilities;
using USGS.Puma.Modflow;

namespace ModflowOutputViewer
{
    public partial class ReferenceDataLinkOptionDialog : Form
    {
        public ReferenceDataLinkOptionDialog()
        {
            InitializeComponent();

            cboLinkOption.Items.Add("Link to current time step and model layer");
            cboLinkOption.Items.Add("Link to current time step and model layer below");
            cboLinkOption.Items.Add("Link to current time step and model layer above");
            cboLinkOption.Items.Add("Specify time step and link to current model layer");
            cboLinkOption.Items.Add("Specify time step and model layer");
            cboLinkOption.SelectedIndex = 0;

        }
        public ReferenceDataLinkOptionDialog(ReferenceDataTimeStepLinkOption timeStepLinkOption, ReferenceDataModelLayerLinkOption modelLayerLinkOption, BinaryLayerReader fileReader) : this()
        {
            ReferenceDataReader = fileReader;
            if (ReferenceDataReader != null)
            { txtReferenceFile.Text = ReferenceDataReader.Filename; }
            int index = FindIndex(timeStepLinkOption, modelLayerLinkOption);
            if (index > -1)
            { cboLinkOption.SelectedIndex = index; }
        }

        #region Public Methods


        public int FindIndex(ReferenceDataTimeStepLinkOption timeStepLinkOption, ReferenceDataModelLayerLinkOption modelLayerLinkOption)
        {
            if (timeStepLinkOption == ReferenceDataTimeStepLinkOption.CurrentTimeStep)
            {
                switch (modelLayerLinkOption)
                {
                    case ReferenceDataModelLayerLinkOption.CurrentModelLayer:
                        return 0;
                    case ReferenceDataModelLayerLinkOption.ModelLayerBelow:
                        return 1;
                    case ReferenceDataModelLayerLinkOption.ModelLayerAbove:
                        return 2;
                    default:
                        return -1;
                }
            }
            else if (timeStepLinkOption == ReferenceDataTimeStepLinkOption.SpecifyTimeStep)
            {
                switch (modelLayerLinkOption)
                {
                    case ReferenceDataModelLayerLinkOption.CurrentModelLayer:
                        return 3;
                    case ReferenceDataModelLayerLinkOption.SpecifyModelLayer:
                        return 4;
                    default:
                        return -1;
                }
            }
            else
            { return -1; }

        }

        private BinaryLayerReader _ReferenceDataReader = null;
        public BinaryLayerReader ReferenceDataReader
        {
            get { return _ReferenceDataReader; }
            set 
            { 
                _ReferenceDataReader = value; 
            }
        }


        private ReferenceDataTimeStepLinkOption _TimeStepLinkOption;
        public ReferenceDataTimeStepLinkOption TimeStepLinkOption
        {
            get { return _TimeStepLinkOption; }
        }

        private ReferenceDataModelLayerLinkOption _ModelLayerLinkOption;
        public ReferenceDataModelLayerLinkOption ModelLayerLinkOption
        {
            get { return _ModelLayerLinkOption; }
        }

        private int _SpecifiedStressPeriod;
        public int SpecifiedStressPeriod
        {
            get { return _SpecifiedStressPeriod; }
            set { _SpecifiedStressPeriod = value; }
        }

        private int _SpecifiedTimeStep;
        public int SpecifiedTimeStep
        {
            get { return _SpecifiedTimeStep; }
            set { _SpecifiedTimeStep = value; }
        }

        private int _SpecifiedModelLayer;
        public int SpecifiedModelLayer
        {
            get { return _SpecifiedModelLayer; }
            set { _SpecifiedModelLayer = value; }
        }
        #endregion


        private void btnSpecifiedInfo_Click(object sender, EventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void cboLinkOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetLinkOptions(cboLinkOption.SelectedIndex);
        }

        private void SetLinkOptions(int index)
        {
            switch (index)
            {
                case 0:
                    _TimeStepLinkOption = ReferenceDataTimeStepLinkOption.CurrentTimeStep;
                    _ModelLayerLinkOption = ReferenceDataModelLayerLinkOption.CurrentModelLayer;
                    btnSelectSpecifiedInfo.Visible = false;
                    lblSpecifiedInfo.Text = "";
                    break;
                case 1:
                    _TimeStepLinkOption = ReferenceDataTimeStepLinkOption.CurrentTimeStep;
                    _ModelLayerLinkOption = ReferenceDataModelLayerLinkOption.ModelLayerBelow;
                    btnSelectSpecifiedInfo.Visible = false;
                    lblSpecifiedInfo.Text = "";
                    break;
                case 2:
                    _TimeStepLinkOption = ReferenceDataTimeStepLinkOption.CurrentTimeStep;
                    _ModelLayerLinkOption = ReferenceDataModelLayerLinkOption.ModelLayerAbove;
                    btnSelectSpecifiedInfo.Visible = false;
                    lblSpecifiedInfo.Text = "";
                    break;
                case 3:
                    _TimeStepLinkOption = ReferenceDataTimeStepLinkOption.SpecifyTimeStep;
                    _ModelLayerLinkOption = ReferenceDataModelLayerLinkOption.CurrentModelLayer;
                    if (ReferenceDataReader != null)
                    {
                        btnSelectSpecifiedInfo.Visible = true;
                        LayerDataRecordHeader header = ReferenceDataReader.ReadRecordHeader(0);
                        SpecifiedStressPeriod = header.StressPeriod;
                        SpecifiedTimeStep = header.TimeStep;
                        SpecifiedModelLayer = 0;
                        lblSpecifiedInfo.Text = "Period " + header.StressPeriod.ToString() + ", Step " + header.TimeStep.ToString();
                    }
                    break;
                case 4:
                    _TimeStepLinkOption = ReferenceDataTimeStepLinkOption.SpecifyTimeStep;
                    _ModelLayerLinkOption = ReferenceDataModelLayerLinkOption.SpecifyModelLayer;
                    if (ReferenceDataReader != null)
                    {
                        btnSelectSpecifiedInfo.Visible = true;
                        LayerDataRecordHeader header = ReferenceDataReader.ReadRecordHeader(0);
                        SpecifiedStressPeriod = header.StressPeriod;
                        SpecifiedTimeStep = header.TimeStep;
                        SpecifiedModelLayer = header.Layer;
                        lblSpecifiedInfo.Text = "Period " + header.StressPeriod.ToString() + ", Step " + header.TimeStep.ToString() + ", Layer " + header.Layer.ToString();
                    }
                    break;
                default:
                    throw new ArgumentException("Specified option is not supported.");
                    break;
            }
        }

    }
}
