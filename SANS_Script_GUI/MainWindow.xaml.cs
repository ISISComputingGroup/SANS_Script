using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO;

namespace LOQ_Script_Gui
{
    public partial class MainWindow : Window
    {
        DataGridVM data = new DataGridVM();
        SettingsPanelVM settings;
        Estimation estimate = new Estimation();
        InstrumentInfo instInfo;
        InstrumentVM instrument;

        public MainWindow()
        {
            InitializeComponent();

            instInfo = new InstrumentInfo();
            settings = new SettingsPanelVM(instInfo);

            dataGrid.DataContext = data;

            pnlSettings.DataContext = settings;
            pnlSettings.OnPreviewClicked += pnlSettings_OnPreviewClicked;
            pnlSettings.OnWriteClicked += pnlSettings_OnWriteClicked;

            pnlEstimate.DataContext = estimate;
            pnlEstimate.OnCalculateClicked += pnlEstimate_OnCalculateClicked;

            instrument = new InstrumentVM(settings.Experiment, estimate);
            mnuMain.DataContext = instrument;

            string inst = Properties.Settings.Default.LastInstrument;
            switch (inst)
            {
                case "LARMOR":
                    instrument.IsLarmor = true;
                    break;
                case "LOQ":
                    instrument.IsLoq = true;
                    break;
                case "SANS2D":
                    instrument.IsSans2d = true;
                    break;
                default:
                    instrument.IsLarmor = true;
                    break;
            }
        }

        void pnlSettings_OnWriteClicked(object sender, EventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            if (instrument.IsLarmor)
            {
                dlg.DefaultExt = ".py";
                dlg.Filter = "genie_python Files|*.py";
            }
            else
            {
                dlg.DefaultExt = ".gcl";
                dlg.Filter = "Open Genie Files|*.gcl";
            }

            if ((bool)dlg.ShowDialog())
            {
                string script = InstrumentInfo.ScriptGenerator.GenerateScript(data.Runs, settings.Experiment);

                using (StreamWriter sw = new StreamWriter(dlg.FileName))
                {
                    sw.Write(script);
                }
            }
        }

        void pnlSettings_OnPreviewClicked(object sender, EventArgs e)
        {
            string script = InstrumentInfo.ScriptGenerator.GenerateScript(data.Runs, settings.Experiment);
            Dialogs.PreviewDialog preview = new Dialogs.PreviewDialog();
            preview.SetScript(script);

            preview.ShowDialog();
        }

        void pnlEstimate_OnCalculateClicked(object sender, EventArgs e)
        {
            estimate.ScriptTime = InstrumentInfo.ScriptGenerator.EstimateRunTime(data.Runs, settings.Experiment, estimate);
        }

        private void btnCreateNewTable_Click(object sender, RoutedEventArgs e)
        {
            data.Runs.Clear();
        }

        private void btnOpenExcel_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "Excel Files|*.xls;*.xlsx";
            dlg.Multiselect = false;

            if ((bool)dlg.ShowDialog())
            {
                try
                {
                    ExcelIO excel = new ExcelIO();
                    excel.OpenExcelWorkBook(dlg.FileName);
                    List<Experiment> exps = excel.ExtractExperiments();
                    data.Runs = new ObservableCollection<Experiment>(exps);
                }
                catch (Exception err)
                {
                    MessageBox.Show("Could not open Excel spreadsheet", "SANS Script - Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void mnuExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelIO excel = new ExcelIO();
            excel.ExportDataToExcel(data.Runs);
        }

        private void mnuSaveCsv_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV File|*.csv"; 

            if ((bool)dlg.ShowDialog())
            {
                try
                {
                    CsvExporter.Export(dlg.FileName, data.Runs);
                }
                catch (Exception err)
                {
                    MessageBox.Show("Could not save as CSV", "SANS Script - Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (instrument.IsLarmor)
            {
                Properties.Settings.Default.LastInstrument = "LARMOR";
            }
            else if (instrument.IsLoq)
            {
                Properties.Settings.Default.LastInstrument = "LOQ";
            }
            else if (instrument.IsSans2d)
            {
                Properties.Settings.Default.LastInstrument = "SANS2D";
            }

            Properties.Settings.Default.Save();
        }
    }
}
