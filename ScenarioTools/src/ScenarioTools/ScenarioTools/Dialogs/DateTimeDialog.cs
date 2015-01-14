using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScenarioTools.Dialogs
{
    public partial class DateTimeDialog : Form
    {
        #region Fields

        private DateTime _dateTime;

        #endregion Fields

        public DateTimeDialog()
        {
            InitializeComponent();
            _dateTime = new DateTime();
        }

        #region Properties

        public DateTime DateTime
        {
            get
            {
                return _dateTime;
            }
            set
            {
                _dateTime = value;
            }
        }

        #endregion Properties

        #region Methods

        private void RefreshControls()
        {
            udYear.Value = _dateTime.Year;
            comboBoxMonth.SelectedIndex = _dateTime.Month - 1;
            udDay.Value = _dateTime.Day;
            udHour.Value = _dateTime.Hour;
            udMinute.Value = _dateTime.Minute;
            udSecond.Value = _dateTime.Second;
            AssignDayMax();
        }

        private bool AssignDateTime()
        {
            bool OK = true;
            int year = System.Convert.ToInt32(udYear.Value);
            int month = comboBoxMonth.SelectedIndex + 1;
            int day = System.Convert.ToInt32(udDay.Value);
            int hour = System.Convert.ToInt32(udHour.Value);
            int minute = System.Convert.ToInt32(udMinute.Value);
            int second = System.Convert.ToInt32(udSecond.Value);
            try
            {
                _dateTime = new DateTime(year, month, day, hour, minute, second);
            }
            catch
            {
                OK = false;
            }
            return OK;
        }

        #endregion Methods

        private void btnOK_Click(object sender, EventArgs e)
        {
            bool OK = AssignDateTime();
        }

        private void lbMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            AssignDayMax();
        }

        private void AssignDayMax()
        {
            int i = comboBoxMonth.SelectedIndex + 1;
            if (i == 1 || i == 3 || i == 5 || i == 7 || i == 8 || i == 10 || i == 12)
            {
                udDay.Maximum = 31;
            }
            else if (i == 4 || i == 6 || i == 9 || i == 11)
            {
                udDay.Maximum = 30;
            }
            else
            {
                if (DateTime.IsLeapYear(Convert.ToInt32(udYear.Value)))
                {
                    udDay.Maximum = 29;
                }
                else
                {
                    udDay.Maximum = 28;
                }
            }
        }

        private void udYear_ValueChanged(object sender, EventArgs e)
        {
            AssignDayMax();
        }

        private void DateTimeDialog_Activated(object sender, EventArgs e)
        {
            RefreshControls();
        }

        private void lbMonth_SelectedValueChanged(object sender, EventArgs e)
        {
            AssignDayMax();
        }

    }
}
