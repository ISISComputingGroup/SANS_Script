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

namespace LOQ_Script_Gui
{
    /// <summary>
    /// Interaction logic for SettingsPanel.xaml
    /// </summary>
    public partial class SettingsPanel : UserControl
    {
        public event EventHandler OnPreviewClicked;
        public event EventHandler OnWriteClicked;

        public SettingsPanel()
        {
            InitializeComponent();
        }

        private void btnPreviewScript_Click(object sender, RoutedEventArgs e)
        {
            if (OnPreviewClicked != null)
            {
                OnPreviewClicked(sender, e);
            }
        }

        private void btnWriteScript_Click(object sender, RoutedEventArgs e)
        {
            if (OnWriteClicked != null)
            {
                OnWriteClicked(sender, e);
            }
        }
    }
}
