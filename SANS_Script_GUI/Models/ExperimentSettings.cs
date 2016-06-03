using System;
using System.ComponentModel;

namespace LOQ_Script_Gui
{
    class ExperimentSettings : INotifyPropertyChanged, IDataErrorInfo
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

        private string order = RunOrder.AllTrans;
        public string Order
        {
            get
            {
                return order;
            }
            set
            {
                order = value;
                OnPropertyChanged("Order");
            }
        }

        private int numSans = 1;
        public int NumSans
        {
            get
            {
                return numSans;
            }
            set
            {
                numSans = value;
                OnPropertyChanged("NumSans");
            }
        }

        private int numTrans = 1;
        public int NumTrans
        {
            get
            {
                return numTrans;
            }
            set
            {
                numTrans = value;
                OnPropertyChanged("NumTrans");
            }
        }

        private bool loopPerRun;
        public bool LoopPerRun
        {
            get
            {
                return loopPerRun;
            }
            set
            {
                loopPerRun = value;
                OnPropertyChanged("LoopPerRun");
            }
        }

        private string a2SettingSans = A2Setting.Large;
        public string A2SettingSans
        {
            get
            {
                return a2SettingSans;
            }
            set
            {
                a2SettingSans = value;
                OnPropertyChanged("A2SettingSans");
            }
        }

        private string a2SettingTrans = A2Setting.Large;
        public string A2SettingTrans
        {
            get
            {
                return a2SettingTrans;
            }
            set
            {
                a2SettingTrans = value;
                OnPropertyChanged("A2SettingTrans");
            }
        }

        private string sampleGeometry = Geometry.Disc;
        public string SampleGeometry
        {
            get
            {
                return sampleGeometry;
            }
            set
            {
                sampleGeometry = value;

                if (sampleGeometry == Geometry.Disc)
                {
                    SampleWidthEnabled = false;
                }
                else
                {
                    SampleWidthEnabled = true;
                }

                OnPropertyChanged("SampleGeometry");
            }
        }

        private double sampleHeight = 7;
        public double SampleHeight
        {
            get
            {
                return sampleHeight;
            }
            set
            {
                sampleHeight = value;
                if (!sampleWidthEnabled)
                {
                    // Width takes the same value as height
                    SampleWidth = value;
                }

                OnPropertyChanged("SampleHeight");
            }
        }

        private double sampleWidth = 7;
        public double SampleWidth
        {
            get
            {
                return sampleWidth;
            }
            set
            {
                sampleWidth = value;
                OnPropertyChanged("SampleWidth");
            }
        }

        private bool sampleWidthEnabled = false;
        public bool SampleWidthEnabled
        {
            get
            {
                return sampleWidthEnabled;
            }
            set
            {
                sampleWidthEnabled = value;
                if (!value)
                {
                    // Width takes the same value as height
                    SampleWidth = SampleHeight;
                }

                OnPropertyChanged("SampleWidthEnabled");
            }
        }

        private string collectionMode = DataCollection.Histogram;
        public string CollectionMode
        {
            get
            {
                return collectionMode;
            }
            set
            {
                collectionMode = value;
                OnPropertyChanged("CollectionMode");
            }
        }

        private bool isValid = true;
        public bool IsValid
        {
            get
            {
                return isValid;
            }
            set
            {
                isValid = value;
                OnPropertyChanged("IsValid");
            }
        }

        public string Error
        {
            get { return String.Empty; }
        }

        public string this[string columnName]
        {
            get
            {
                string errorMessage = String.Empty;

                switch (columnName)
                {
                    case "NumSans":
                        if (NumSans < 0)
                        {
                            errorMessage = "Number of SANS runs cannot be negative!";
                        }
                        break;
                    case "NumTrans":
                        if (NumTrans < 0)
                        {
                            errorMessage = "Number of TRANS runs cannot be negative!";
                        }
                        break;
                    case "SampleHeight":
                        if (SampleHeight <= 0 || SampleHeight > 20)
                        {
                            errorMessage = "Sample height must be between 0 and 20";
                        }
                        break;
                    case "SampleWidth":
                        if (SampleWidth <= 0 || SampleWidth > 20)
                        {
                            errorMessage = "Sample width must be between 0 and 20";
                        }
                        break;
                }

                if (string.IsNullOrWhiteSpace(errorMessage))
                {
                    IsValid = true;
                }
                else
                {
                    IsValid = false;
                }
                return errorMessage;
            }
        }
    }
}
