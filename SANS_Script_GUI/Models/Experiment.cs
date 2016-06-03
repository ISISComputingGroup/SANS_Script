using System.ComponentModel;

namespace LOQ_Script_Gui
{
    class Experiment : INotifyPropertyChanged
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

        private string position;
        public string Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                OnPropertyChanged("Position");
            }
        }

        private double trans;
        public double Trans
        {
            get
            {
                return trans;
            }
            set
            {
                trans = value;
                OnPropertyChanged("Trans");
            }
        }

        private string transWait;
        public string TransWait
        {
            get
            {
                return transWait;
            }
            set
            {
                transWait = value;
                OnPropertyChanged("TransWait");
            }
        }

        private double sans;
        public double Sans
        {
            get
            {
                return sans;
            }
            set
            {
                sans = value;
                OnPropertyChanged("Sans");
            }
        }

        private string sansWait;
        public string SansWait
        {
            get
            {
                return sansWait;
            }
            set
            {
                sansWait = value;
                OnPropertyChanged("SansWait");
            }
        }

        private string period;
        public string Period
        {
            get
            {
                return period;
            }
            set
            {
                period = value;
                OnPropertyChanged("Period");
            }
        }

        private string sample;
        public string Sample
        {
            get
            {
                return sample;
            }
            set
            {
                sample = value;
                OnPropertyChanged("Sample");
            }
        }

        private string thickness;
        public string Thickness
        {
            get
            {
                return thickness;
            }
            set
            {
                thickness = value;
                OnPropertyChanged("Thickness");
            }
        }

        private string temperature1;
        public string Temperature1
        {
            get
            {
                return temperature1;
            }
            set
            {
                temperature1 = value;
                OnPropertyChanged("Temperature1");
            }
        }

        private string temperature2;
        public string Temperature2
        {
            get
            {
                return temperature2;
            }
            set
            {
                temperature2 = value;
                OnPropertyChanged("Temperature2");
            }
        }

        private string field;
        public string Field
        {
            get
            {
                return field;
            }
            set
            {
                field = value;
                OnPropertyChanged("Field");
            }
        }

        private string shearRate1;
        public string ShearRate1
        {
            get
            {
                return shearRate1;
            }
            set
            {
                shearRate1 = value;
                OnPropertyChanged("ShearRate1");
            }
        }

        private string shearRate2;
        public string ShearRate2
        {
            get
            {
                return shearRate2;
            }
            set
            {
                shearRate2 = value;
                OnPropertyChanged("ShearRate2");
            }
        }

        private string shearAngle1;
        public string ShearAngle1
        {
            get
            {
                return shearAngle1;
            }
            set
            {
                shearAngle1 = value;
                OnPropertyChanged("ShearAngle1");
            }
        }

        private string shearAngle2;
        public string ShearAngle2
        {
            get
            {
                return shearAngle2;
            }
            set
            {
                shearAngle2 = value;
                OnPropertyChanged("ShearAngle2");
            }
        }

        private string preCommand;
        public string PreCommand
        {
            get
            {
                return preCommand;
            }
            set
            {
                preCommand = value;
                OnPropertyChanged("PreCommand");
            }
        }

        private string postCommand;
        public string PostCommand
        {
            get
            {
                return postCommand;
            }
            set
            {
                postCommand = value;
                OnPropertyChanged("PostCommand");
            }
        }

        private string rbNumber;
        public string RbNumber
        {
            get
            {
                return rbNumber;
            }
            set
            {
                rbNumber = value;
                OnPropertyChanged("RbNumber");
            }
        }

        public Experiment()
        {
            sans = 0;
            trans = 0;
        }
    }
}
