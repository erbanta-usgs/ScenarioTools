using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ScenarioTools.Reporting;

namespace ScenarioTools.Dialogs
{
    public partial class DataSeriesMenuContourBox : UserControl
    {
        private DataSeries.ContourSpecificationMethod _contourSpecificationType;
        private float _startContour;
        private float _contourInterval;
        private float[] _contourValues;

        public DataSeriesMenuContourBox()
        {
            InitializeComponent();
        }
        public DataSeries.ContourSpecificationMethod ContourSpecificationType
        {
            get
            {
                // Get the value from the form.
                if (radioButtonEqualInterval.Checked)
                {
                    _contourSpecificationType = DataSeries.ContourSpecificationMethod.EqualInterval;
                }
                else if (radioButtonListOfValues.Checked)
                {
                    _contourSpecificationType = DataSeries.ContourSpecificationMethod.ValueList;
                }
                else if (radioButtonAutomatic.Checked)
                {
                    _contourSpecificationType = DataSeries.ContourSpecificationMethod.Automatic;
                }

                // Return the value.
                return _contourSpecificationType;
            }
            set
            {
                _contourSpecificationType = value;

                // If equal interval is specified, setup the form appropriately.
                if (_contourSpecificationType == DataSeries.ContourSpecificationMethod.EqualInterval)
                {
                    // Suspend form layout.
                    this.SuspendLayout();

                    // Show both text boxes and labels.
                    this.labelStartingContour.Visible = true;
                    this.labelContourInterval.Visible = true;
                    this.textBoxStartingContour.Visible = true;
                    this.textBoxContourInterval.Visible = true;

                    // Set the text of both labels appropriately.
                    this.labelStartingContour.Text = "Starting Contour";
                    this.labelContourInterval.Text = "Contour Interval";

                    // Set the text of the text boxes appropriately.
                    this.textBoxStartingContour.Text = _startContour + "";
                    this.textBoxContourInterval.Text = _contourInterval + "";

                    // Resume form layout.
                    this.ResumeLayout();
                }

                // If value-list is specified, setup the form appropriately.
                else if (_contourSpecificationType == DataSeries.ContourSpecificationMethod.ValueList)
                {
                    // Suspend form layout.
                    this.SuspendLayout();

                    // Show just the first text box and label.
                    this.labelStartingContour.Visible = true;
                    this.labelContourInterval.Visible = false;
                    this.textBoxStartingContour.Visible = true;
                    this.textBoxContourInterval.Visible = false;

                    // Set the text of the label appropriately.
                    this.labelStartingContour.Text = "Contour Values";

                    // Set the text of the text box appropriately.
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < _contourValues.Length; i++) {
                        sb.Append(_contourValues[i]);
                        if (i != _contourValues.Length - 1) {
                            sb.Append(", ");
                        }
                    }
                    this.textBoxStartingContour.Text = sb.ToString();

                    // Resume form layout.
                    this.ResumeLayout();
                }
                else if (_contourSpecificationType == DataSeries.ContourSpecificationMethod.Automatic)
                {
                    // Suspend form layout.
                    this.SuspendLayout();

                    // Make text boxes and labels invisible.
                    this.labelStartingContour.Visible = false;
                    this.labelContourInterval.Visible = false;
                    this.textBoxStartingContour.Visible = false;
                    this.textBoxContourInterval.Visible = false;

                    // Resume form layout.
                    this.ResumeLayout();
                }
            }
        }
        public void UpdateRadioButtons()
        {
            switch (this.ContourSpecificationType)
            {
                case (DataSeries.ContourSpecificationMethod.Automatic):
                    {
                        // Automatic-interval is specified; check the automatic button.
                        radioButtonAutomatic.Checked = true;
                        break;
                    }
                case (DataSeries.ContourSpecificationMethod.EqualInterval):
                    {
                        // Equal-interval is specified; check the equal-interval button.
                        radioButtonEqualInterval.Checked = true;
                        break;
                    }
                case (DataSeries.ContourSpecificationMethod.ValueList):
                    {
                        // List is specified; check the list button.
                        radioButtonListOfValues.Checked = true;
                        break;
                    }
            }
        }
        public float StartContour
        {
            get
            {
                // Only try to get the value from the form if equal interval is specified.
                if (_contourSpecificationType == DataSeries.ContourSpecificationMethod.EqualInterval)
                {
                    // Only return the text box value if the text box is parseable.
                    float startContourLocal;
                    if (float.TryParse(textBoxStartingContour.Text, out startContourLocal))
                    {
                        return startContourLocal;
                    }
                }

                // In all other cases, return the backing variable value.
                return _startContour;
            }
            set
            {
                // Store the value in the backing variable.
                _startContour = value;

                // If equal-interval contours are selected, set the value in the text box.
                if (this.ContourSpecificationType == DataSeries.ContourSpecificationMethod.EqualInterval)
                {
                    this.textBoxStartingContour.Text = _startContour + "";
                }
            }
        }
        public float ContourInterval
        {
            get
            {
                // Only try to get the value from the form if equal interval is specified.
                if (_contourSpecificationType == DataSeries.ContourSpecificationMethod.EqualInterval)
                {
                    // Only return the text box value if the text box is parseable.
                    float contourIntervalLocal;
                    if (float.TryParse(textBoxContourInterval.Text, out contourIntervalLocal))
                    {
                        return contourIntervalLocal;
                    }
                }

                // In all other cases, return the backing variable value.
                return _contourInterval;
            }
            set
            {
                // Store the value in the backing variable.
                _contourInterval = value;

                // If equal-interval contours are selected, set the value in the text box.
                if (this.ContourSpecificationType == DataSeries.ContourSpecificationMethod.EqualInterval)
                {
                    this.textBoxContourInterval.Text = _contourInterval + "";
                }
            }
        }
        public float[] ContourValues
        {
            get
            {
                // If value-list is specified, try to refresh the contour-value array.
                if (this.ContourSpecificationType == DataSeries.ContourSpecificationMethod.ValueList)
                {
                    try
                    {
                        // Make the list and get the string from the text box.
                        List<float> newContourValues = new List<float>();
                        string valueString = textBoxStartingContour.Text;

                        // Parse the list.
                        string[] split = valueString.Split(new char[] { ',' });
                        for (int i = 0; i < split.Length; i++)
                        {
                            // If the value is good add it to the list.
                            float value;
                            if (float.TryParse(split[i], out value))
                            {
                                newContourValues.Add(value);
                            }
                        }

                        // If the list is non-empty, sort it and replace the existing list.
                        if (newContourValues.Count > 0)
                        {
                            newContourValues.Sort();
                            _contourValues = newContourValues.ToArray();
                        }
                    }
                    catch { }

                    // Return the contour values.
                    return _contourValues;
                }

                return _contourValues;
            }
            set
            {
                _contourValues = value;
            }
        }

        private void radioButtonListOfValues_CheckedChanged(object sender, EventArgs e)
        {
            this.ContourSpecificationType = DataSeries.ContourSpecificationMethod.ValueList;
        }

        private void radioButtonEqualInterval_CheckedChanged(object sender, EventArgs e)
        {
            this.ContourSpecificationType = DataSeries.ContourSpecificationMethod.EqualInterval;
        }

        private void radioButtonAutomatic_CheckedChanged(object sender, EventArgs e)
        {
            this.ContourSpecificationType = DataSeries.ContourSpecificationMethod.Automatic;
        }
   
    }
}
