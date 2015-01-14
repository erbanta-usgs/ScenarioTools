using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ScenarioTools.Reporting;

namespace ScenarioTools.Data_Providers
{
    public partial class DataProviderCalculatedMapMenu : UserControl, IDataProviderMenu
    {
        private DataProviderCalculatedMap dataProvider;

        public DataProviderCalculatedMapMenu(DataProviderCalculatedMap dataProvider)
        {
            InitializeComponent();

            // Store a reference to the data provider.
            this.dataProvider = dataProvider;

            // Add an event handler for double-clicking the data series in the list.
            listBoxDataSeries.DoubleClick += new EventHandler(listBoxDataSeries_DoubleClick);

            // Add event handlers for all of the buttons.
            addButtonHandler(button0, "0");
            addButtonHandler(button1, "1");
            addButtonHandler(button2, "2");
            addButtonHandler(button3, "3");
            addButtonHandler(button4, "4");
            addButtonHandler(button5, "5");
            addButtonHandler(button6, "6");
            addButtonHandler(button7, "7");
            addButtonHandler(button8, "8");
            addButtonHandler(button9, "9");

            addButtonHandler(buttonAdd, "+");
            addButtonHandler(buttonSubtract, "-");
            addButtonHandler(buttonMultiply, "*");
            addButtonHandler(buttonDivide, "/");
            addButtonHandler(buttonLeftParens, "(");
            addButtonHandler(buttonRightParens, ")");

            addButtonHandlerFunction(buttonMinimum, "minimum");
            addButtonHandlerFunction(buttonMaximum, "maximum");
            addButtonHandlerFunction(buttonAverage, "average");
            //addButtonHandlerFunction(buttonSum, "sum");
        }

        public string Expression
        {
            get
            {
                return textBoxExpression.Text;
            }
        }

        private void addButtonHandler(Button button, string text)
        {
            // Set the text as the button tag.
            button.Tag = text;

            // Add the event handler.
            button.Click += new EventHandler(button_Click);
        }
        private void addButtonHandlerFunction(Button button, string text)
        {
            // Set the text as the button tag.
            button.Tag = text;

            // Add the event handler.
            button.Click += new EventHandler(buttonFunction_Click);
        }

        void button_Click(object sender, EventArgs e)
        {
            // Cast the sender to a button.
            Button button = (Button)sender;

            // Add the appropriate text to the expression box.
            textBoxExpression.SelectedText = (string)button.Tag + " ";
        }
        void buttonFunction_Click(object sender, EventArgs e)
        {
            // Cast the sender to a button.
            Button button = (Button)sender;

            // Add the appropriate text to the expression box.
            textBoxExpression.SelectedText = (string)button.Tag + "()";
            textBoxExpression.SelectionStart -= 1;
        }

        void listBoxDataSeries_DoubleClick(object sender, EventArgs e)
        {
            // Determine the item that was clicked.
            object itemClicked = listBoxDataSeries.SelectedItem;
            if (itemClicked != null)
            {
                textBoxExpression.SelectedText = "[" + itemClicked + "] ";
            }
        }

        #region IDataProviderMenu Members

        public bool UpdateDataProvider(out string errorMessage)
        {
            errorMessage = "";
            // Update the expression with the data provider.
            dataProvider.Expression = textBoxExpression.Text;

            // Invalidate the dataset.
            dataProvider.InvalidateDataset();
            return true;
        }

        #endregion

        private void DataProviderCalculatedMapMenu_Load(object sender, EventArgs e)
        {
            // Populate the expression box with the existing expression.
            textBoxExpression.Text = dataProvider.Expression;

            // Populate the data series box with other data series in the map element.
            if (dataProvider.ParentSTMap != null)
            {
                long uniqueID = this.dataProvider.GetUniqueIdentifier();
                ReportElementSTMap parentSTMap = dataProvider.ParentSTMap;
                for (int i = 0; i < parentSTMap.NumDataSeries; i++)
                {
                    // Do not add data series belonging to this.dataProvider
                    if (parentSTMap.GetDataSeries(i).DataProvider.GetUniqueIdentifier() != uniqueID)
                    {
                        listBoxDataSeries.Items.Add(parentSTMap.GetDataSeries(i).Name);
                    }
                }
            }
        }
    }
}
