using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ScenarioTools.ModflowReaders
{
    public class CbbHelper
    {
        public static void PopulateIdentifierComboBox(string cbbFile, ComboBox comboBox, string dataIdentifier, out int numLayers)
        {
            // Populate the combo box.
            bool isCompact;
            int precision = PrecisionHelpers.BudgetFilePrecision(cbbFile, out isCompact);
            string[] identifiers = CbbReader.GetCbbHeaders(cbbFile, precision, out numLayers);
            comboBox.Items.Clear();
            for (int i = 0; i < identifiers.Length; i++)
            {
                comboBox.Items.Add(identifiers[i]);
            }

            // Select the appropriate item in the combo box.
            if (dataIdentifier != null)
            {
                for (int i = 0; i < comboBox.Items.Count; i++)
                {
                    if (comboBox.Items[i].ToString().Trim().ToLower().Equals(dataIdentifier.ToLower().Trim()))
                    {
                        comboBox.SelectedIndex = i;
                    }
                }
            }

            // If no item is selected, select the first item in the box.
            if (comboBox.SelectedIndex < 0)
            {
                if (comboBox.Items.Count > 0)
                {
                    comboBox.SelectedIndex = 0;
                }
            }
        }
    }
}
