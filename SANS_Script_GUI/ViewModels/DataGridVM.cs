using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LOQ_Script_Gui
{
    class DataGridVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        ObservableCollection<Experiment> runs = new ObservableCollection<Experiment>();
        public ObservableCollection<Experiment> Runs
        {
            get
            {
                return runs;
            }
            set
            {
                runs = value;
                OnPropertyChanged("Runs");
            }
        }

        public static ObservableCollection<string> WaitForChoices
        {
            get
            {
                return WaitForUnits.Choices;
            }
        }
    }
}
