using System.ComponentModel;
using System;

namespace LOQ_Script_Gui
{
    class Estimation : INotifyPropertyChanged, IDataErrorInfo
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

        private double countRate = 40;
        public double CountRate
        {
            get
            {
                return countRate;
            }
            set
            {
                countRate = value;
                OnPropertyChanged("CountRate");
            }
        }

        private double moveTime = 120;
        public double MoveTime
        {
            get
            {
                return moveTime;
            }
            set
            {
                moveTime = value;
                OnPropertyChanged("MoveTime");
            }
        }

        private string scriptTime;
        public string ScriptTime
        {
            get
            {
                return scriptTime;
            }
            set
            {
                scriptTime = value;
                OnPropertyChanged("ScriptTime");
            }
        }

        private bool isValid;
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
                    case "CountRate":
                        if (CountRate < 0)
                        {
                            errorMessage = "Count rate cannot be negative!";
                        }
                        break;
                    case "MoveTime":
                        if (MoveTime < 0)
                        {
                            errorMessage = "Move time cannot be negative!";
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
