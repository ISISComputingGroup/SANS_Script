using System;
using System.Collections.Generic;
using System.ComponentModel;
using LOQ_Script_Gui.Scripting;
using System.Collections.ObjectModel;

namespace LOQ_Script_Gui
{
    /// <summary>
    /// Contains the information needed to set the GUI up for the specified instrument.
    /// </summary>
    class InstrumentInfo : INotifyPropertyChanged
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

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged = delegate { };

        protected static void OnStaticPropertyChanged(string propertyName)
        {
            if (StaticPropertyChanged != null)
                StaticPropertyChanged(null, new PropertyChangedEventArgs(propertyName));
        }

        static string a2Label = "A2 Setting:";
        public static string A2Label
        {
            get
            {
                return a2Label;
            }
            set
            {
                a2Label = value;
                OnStaticPropertyChanged("A2Label");
            }
        }

        // Provides the combo box options
        // These values can change depending on the instrument
        static ObservableCollection<string> a2Choices = new ObservableCollection<string>()
        {
            A2Setting.Large, A2Setting.Medium, A2Setting.Small, A2Setting.Trans
        };

        public static ObservableCollection<string> A2Choices
        {
            get
            {
                return a2Choices;
            }

            set
            {
                a2Choices = value;
                OnStaticPropertyChanged("A2Choices");
            }
        }

        static bool a2TransEnabled = true;
        public static bool A2TransEnabled
        {
            get
            {
                return a2TransEnabled;
            }
            set
            {
                a2TransEnabled = value;
                OnStaticPropertyChanged("A2TransEnabled");
            }
        }

        static bool collectionModeEnabled = false;
        public static bool CollectionModeEnabled
        {
            get
            {
                return collectionModeEnabled;
            }
            set
            {
                collectionModeEnabled = value;
                OnStaticPropertyChanged("CollectionModeEnabled");
            }
        }

        public static IScriptGenerator ScriptGenerator { get; set; }
    }
}
