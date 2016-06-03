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
using System.Windows.Shapes;
using System.IO;
using System.Windows.Xps.Packaging;
using System.Windows.Documents;

namespace LOQ_Script_Gui.Dialogs
{
    /// <summary>
    /// Interaction logic for PreviewDialog.xaml
    /// </summary>
    public partial class PreviewDialog : Window
    {
        public PreviewDialog()
        {
            InitializeComponent();
        }

        public void SetScript(string script)
        {
            txtScript.Text = script;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
