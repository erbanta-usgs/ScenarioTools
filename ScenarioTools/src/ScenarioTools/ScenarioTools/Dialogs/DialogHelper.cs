using System.Windows.Forms;

namespace ScenarioTools.Dialogs
{
    public class DialogHelper
    {
        public static OpenFileDialog GetHeadsFileDialog()
        {
            // Make the dialog.
            OpenFileDialog dialog = new OpenFileDialog();

            // Set the file filter.
            dialog.Filter = "MODFLOW Binary Head Files (*.hds)|*.hds|MODFLOW Binary Drawdown Files (*.ddn)|*.ddn|SEAWAT or MT3D Binary Concentration Files (*.ucn)|*.ucn|All MODFLOW Binary Array Files|*.hds;*.hdd;*.ddn;*.bhd;*.bdd;*.bdn;*.ucn|All Files (*.*)|*.*";

            // Return the result.
            return dialog;
        }
        public static OpenFileDialog GetNameFileDialog()
        {
            // Make the dialog.
            OpenFileDialog dialog = new OpenFileDialog();

            // Set the file filter.
            dialog.Filter = "MODFLOW Name Files (*.nam)|*.nam|All Files (*.*)|*.*";

            // Return the result.
            return dialog;
        }

        public static OpenFileDialog GetDiscretizationFileDialog()
        {
            // Make the dialog.
            OpenFileDialog dialog = new OpenFileDialog();

            // Set the file filter.
            dialog.Filter = "MODFLOW Discretization Files (*.dis)|*.dis|All Files (*.*)|*.*";

            // Return the result.
            return dialog;
        }

        public static OpenFileDialog GetCbbFileDialog()
        {
            // Make the dialog.
            OpenFileDialog dialog = new OpenFileDialog();

            // Set the file filter.
            dialog.Filter = "MODFLOW Cell-By-Cell Files (*.cbb, *.cbc)|*.cbb;*.cbc|All Files (*.*)|*.*";

            // Return the result.
            return dialog;
        }

        public static OpenFileDialog GetExecutableFileDialog()
        {
            // Make the dialog.
            OpenFileDialog dialog = new OpenFileDialog();

            // Set the file filter.
            dialog.Filter = "Executable Files (*.exe)|*.exe";

            // Return the result.
            return dialog;
        }


        public static OpenFileDialog GetSmpFileDialog()
        {
            // Make the dialog.
            OpenFileDialog dialog = new OpenFileDialog();

            // Set the file filter.
            dialog.Filter = "Borehole Sample Files (*.smp)|*.smp|All Files (*.*)|*.*";

            // Return the result.
            return dialog;
        }

        public static SaveFileDialog GetXmlSaveFileDialog()
        {
            // Make the dialog.
            SaveFileDialog dialog = new SaveFileDialog();

            // Set the file filter.
            dialog.Filter = "Extensible Markup Language (XML) Files (*.xml)|*.xml|All Files (*.*)|*.*";

            // Return the result.
            return dialog;
        }

        public static OpenFileDialog GetXmlOpenFileDialog()
        {
            // Make the dialog.
            OpenFileDialog dialog = new OpenFileDialog();

            // Set the file filter.
            dialog.Filter = "Extensible Markup Language (XML) Files (*.xml)|*.xml|All Files (*.*)|*.*";

            // Return the result.
            return dialog;
        }

        public static OpenFileDialog GetOpenGroupFileDialog()
        {
            // Make the dialog.
            OpenFileDialog dialog = new OpenFileDialog();

            // Set the file filter.
            dialog.Filter = "All Files (*.*)|*.*";

            // Return the result.
            return dialog;
        }

        public static OpenFileDialog GetOpenSADialog()
        {
            // Make the dialog.
            OpenFileDialog dialog = new OpenFileDialog();

            // Set the file filter.
            dialog.Filter = "Scenario Analyzer Files (*.sa)|*.sa|All Files (*.*)|*.*";

            // Return the result.
            return dialog;
        }

        public static SaveFileDialog getSaveSADialog()
        {
            // Make the dialog.
            SaveFileDialog dialog = new SaveFileDialog();

            // Set the file filter.
            dialog.Filter = "Scenario Analyzer Files (*.sa)|*.sa|All Files (*.*)|*.*";

            // Return the result.
            return dialog;
        }

        public static SaveFileDialog GetCsvSaveFileDialog()
        {
            // Make the dialog.
            SaveFileDialog dialog = new SaveFileDialog();

            // Set the file filter.
            dialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";

            // Return the result.
            return dialog;
        }

        public static SaveFileDialog GetShapefileSaveDialog()
        {
            // Make the dialog.
            SaveFileDialog dialog = new SaveFileDialog();

            // Set the file filter.
            dialog.Filter = "Shapefiles (*.shp)|*.shp|All Files (*.*)|*.*";

            // Return the result.
            return dialog;
        }
        public static OpenFileDialog GetShapefileOpenDialog()
        {
            // Make the dialog.
            OpenFileDialog dialog = new OpenFileDialog();

            // Set the file filter.
            dialog.Filter = "Shapefiles (*.shp)|*.shp|All Files (*.*)|*.*";

            // Return the result.
            return dialog;
        }
        public static OpenFileDialog GetImageFileOpenDialog()
        {
            // Make the dialog.
            OpenFileDialog dialog = new OpenFileDialog();

            // Set the file filter.
            dialog.Filter = "TIFF Files (*.tif)|*.tif|All Files (*.*)|*.*";

            // Return the result.
            return dialog;
        }

        public static SaveFileDialog GetSaveTextDialog()
        {
            // Make the dialog.
            SaveFileDialog dialog = new SaveFileDialog();

            // Set the file filter.
            dialog.Filter = "Text files (*.txt)|*.txt|All Files (*.*)|*.*";

            // Return the result.
            return dialog;
        }
    }
}
