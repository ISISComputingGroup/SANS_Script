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
    /// Interaction logic for EstimationPanel.xaml
    /// </summary>
    public partial class EstimationPanel : UserControl
    {
        public event EventHandler OnCalculateClicked;

        public EstimationPanel()
        {
            InitializeComponent();
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (OnCalculateClicked != null)
            {
                OnCalculateClicked(sender, e);
            }
        }
    }
}
