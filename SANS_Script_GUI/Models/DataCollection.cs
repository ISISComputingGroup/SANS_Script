using System.Collections.ObjectModel;

namespace LOQ_Script_Gui
{
    class DataCollection
    {
        public readonly static string Histogram = "Histogram";
        public readonly static string Events = "Events";

        static ObservableCollection<string> choices = new ObservableCollection<string>()
        {
            Histogram, Events
        };

        public static ObservableCollection<string> Choices
        {
            get
            {
                return choices;
            }
        }
    }
}
