using System.Collections.ObjectModel;

namespace LOQ_Script_Gui
{
    class RunOrder
    {
        public readonly static string AllTrans = "All TRANS first";
        public readonly static string TransFirst = "Alternate - TRANS first";
        public readonly static string AllSans = "All SANS first";
        public readonly static string SansFirst = "Alternate - SANS first";

        static ObservableCollection<string> choices = new ObservableCollection<string>()
        {
            AllTrans, TransFirst, AllSans, SansFirst
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
