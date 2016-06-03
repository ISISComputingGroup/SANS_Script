using System.Collections.ObjectModel;

namespace LOQ_Script_Gui
{
    class WaitForUnits
    {
        public readonly static string MicroAmps = "Microamp-hours";
        public readonly static string Minutes = "Minutes";
        public readonly static string Seconds = "Seconds";
        public readonly static string Frames = "Frames";

        static ObservableCollection<string> choices = new ObservableCollection<string>()
        {
            MicroAmps, Minutes, Seconds, Frames
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
