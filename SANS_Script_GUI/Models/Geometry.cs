using System.Collections.ObjectModel;

namespace LOQ_Script_Gui
{
    class Geometry
    {
        public readonly static string Disc = "Disc";
        public readonly static string Cylindrical = "Cylindrical";
        public readonly static string FlatPlate = "Flat Plate";
        public readonly static string SingleCrystal = "Single Crystal";

        static ObservableCollection<string> choices = new ObservableCollection<string>()
        {
            Disc, Cylindrical, FlatPlate, SingleCrystal
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
