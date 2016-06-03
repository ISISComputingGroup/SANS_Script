using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LOQ_Script_Gui
{
    class InstrumentVM : INotifyPropertyChanged
    {
        List<string> loqApertureSizes = new List<string>() 
        {
            A2Setting.Large, A2Setting.Medium
        };

        List<string> sans2dApertureSizes = new List<string>() 
        {
            A2Setting.Large, A2Setting.Medium, A2Setting.Small, A2Setting.Trans
        };

        List<string> larmorApertureSizes = new List<string>() 
        {
            A2Setting.LarmorLarge, A2Setting.LarmorMedium, A2Setting.LarmorSmall
        };


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private bool isLarmor = true;
        public bool IsLarmor
        {
            get
            {
                return isLarmor;
            }
            set
            {
                isLarmor = value;
                if (value)
                {
                    IsLoq = false;
                    IsSans2d = false;
                    InstrumentInfo.ScriptGenerator = new Scripting.GeniePythonScriptGenerator();
                    InstrumentInfo.A2Label = "A1, S1 Setting:";
                    InstrumentInfo.A2Choices = new ObservableCollection<string>(larmorApertureSizes);
                    InstrumentInfo.A2TransEnabled = true;
                    InstrumentInfo.CollectionModeEnabled = true;
                    estimation.CountRate = 40;
                    settings.A2SettingSans = A2Setting.LarmorMedium;
                    settings.A2SettingTrans = A2Setting.LarmorMedium;
                }

                OnPropertyChanged("IsLarmor");
            }
        }

        private bool isLoq = false;
        public bool IsLoq
        {
            get
            {
                return isLoq;
            }
            set
            {
                isLoq = value;
                if (value)
                {
                    IsLarmor = false;
                    IsSans2d = false;
                    InstrumentInfo.ScriptGenerator = new Scripting.OpenGenieScriptGenerator(false);
                    InstrumentInfo.A2Label = "A2 Setting:";
                    InstrumentInfo.A2Choices = new ObservableCollection<string>(loqApertureSizes);
                    InstrumentInfo.A2TransEnabled = false;
                    InstrumentInfo.CollectionModeEnabled = false;
                    estimation.CountRate = 170;
                    settings.A2SettingSans = A2Setting.Large;
                    settings.A2SettingTrans = A2Setting.Large;
                }

                OnPropertyChanged("IsLoq");
            }
        }

        private bool isSans2d = false;
        public bool IsSans2d
        {
            get
            {
                return isSans2d;
            }
            set
            {
                isSans2d = value;
                if (value)
                {
                    IsLarmor = false;
                    IsLoq = false;
                    InstrumentInfo.ScriptGenerator = new Scripting.OpenGenieScriptGenerator(true);
                    InstrumentInfo.A2Label = "A6 Setting:";
                    InstrumentInfo.A2Choices = new ObservableCollection<string>(sans2dApertureSizes);
                    InstrumentInfo.A2TransEnabled = true;
                    InstrumentInfo.CollectionModeEnabled = false;
                    estimation.CountRate = 40;
                    settings.A2SettingSans = A2Setting.Large;
                    settings.A2SettingTrans = A2Setting.Large;
                }

                OnPropertyChanged("IsSans2d");
            }
        }

        ExperimentSettings settings;
        Estimation estimation;

        public InstrumentVM(ExperimentSettings settings, Estimation estimation)
        {
            this.settings = settings;
            this.estimation = estimation;
        }
    }
}
