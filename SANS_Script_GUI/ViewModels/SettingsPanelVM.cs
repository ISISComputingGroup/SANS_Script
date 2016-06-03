using System.Collections.ObjectModel;
using System.ComponentModel;
using System;

namespace LOQ_Script_Gui
{
    class SettingsPanelVM : INotifyPropertyChanged
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

        // Provides the combo box options
        public static ObservableCollection<string> OrderChoices
        {
            get
            {
                return RunOrder.Choices;
            }
        }

        // Provides the combo box options
        public static ObservableCollection<string> GeometryChoices
        {
            get
            {
                return Geometry.Choices;
            }
        }

        // Provides the combo box options
        public static ObservableCollection<string> CollectionModeChoices
        {
            get
            {
                return DataCollection.Choices;
            }
        }

        ExperimentSettings experiment = new ExperimentSettings();
        public ExperimentSettings Experiment
        {
            get
            {
                return experiment;
            }
            set
            {
                experiment = value;
                OnPropertyChanged("Experiment");
            }
        }

        InstrumentInfo instInfo;
        public InstrumentInfo InstInfo
        {
            get
            {
                return instInfo;
            }
            set
            {
                instInfo = value;
                OnPropertyChanged("InstInfo");
            }
        }

        public SettingsPanelVM()
        {
            // Need a default constructor to keep the databinding happy!
            // The other constructor should be called ASAP
            instInfo = new InstrumentInfo();
        }

        public SettingsPanelVM(InstrumentInfo instInfo)
        {
            this.instInfo = instInfo;
        }
    }
}
