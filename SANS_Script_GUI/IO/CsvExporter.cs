using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;

namespace LOQ_Script_Gui
{
    class CsvExporter
    {
        public static void Export(string file, ObservableCollection<Experiment> runs)
        {
            using(StreamWriter sw = new StreamWriter(file))
            {
                foreach(Experiment exp in runs)
                {
                    // Position
                    WriteParameter(sw, exp.Position);

                    // Trans
                    WriteParameter(sw, exp.Trans.ToString());
                    
                    // Trans wait
                    WriteParameter(sw, exp.TransWait);
                    
                    // Sans
                    WriteParameter(sw, exp.Sans.ToString());

                    // Sans wait
                    WriteParameter(sw, exp.SansWait);

                    // Period
                    WriteParameter(sw, exp.Period);

                    // Sample ID
                    WriteParameter(sw, exp.Sample);

                    // Thickness
                    WriteParameter(sw, exp.Thickness);

                    // Temperature 1
                    WriteParameter(sw, exp.Temperature1);

                    // Temperature 2
                    WriteParameter(sw, exp.Temperature2);

                    // Field
                    WriteParameter(sw, exp.Field);

                    // Shear rate 1
                    WriteParameter(sw, exp.ShearRate1);

                    // Shear rate 2
                    WriteParameter(sw, exp.ShearRate2);

                    // Shear angle 1
                    WriteParameter(sw, exp.ShearAngle1);

                    // Shear angle 2
                    WriteParameter(sw, exp.ShearAngle2);

                    // Pre-command
                    WriteParameter(sw, exp.PreCommand);

                    // Post-command
                    WriteParameter(sw, exp.PostCommand);

                    // RB
                    WriteParameter(sw, exp.RbNumber);

                    sw.WriteLine();
                }
            }
        }

        private static void WriteParameter(StreamWriter sw, string par)
        {
            if (!string.IsNullOrWhiteSpace(par))
            {
                sw.Write(string.Format("{0}, ", par));
            }
            else
            {
                sw.Write(", ");
            }
        }
    }
}
