using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace LOQ_Script_Gui.Scripting
{
    class GeniePythonScriptGenerator : IScriptGenerator
    {
        string indent = "    ";

        public string GenerateScript(ObservableCollection<Experiment> runs, ExperimentSettings settings)
        {
            string script = "# Script created by LOQ Script at " + DateTime.Now + Environment.NewLine;
            script += GenerateHeader();
            script += GenerateSetup(settings);
            script += GenerateRuns(runs, settings);
            script += Environment.NewLine;

            return script;
        }

        private string GetIndentString(int n)
        {
            return new StringBuilder().Insert(0, indent, n).ToString();
        }

        private string GenerateHeader()
        {
            string script = "import LSS.SANSroutines as lm" + Environment.NewLine + Environment.NewLine;
            script += "def my_script():" + Environment.NewLine;
            script += GetIndentString(1) + "from genie_python.genie import *" + Environment.NewLine;

            return script;
        }

        private string GenerateSetup(ExperimentSettings settings)
        {
            string script = "";

            // TODO: Set number of available periods

            script += GetIndentString(1) + "lm.setuplarmor_normal()" + Environment.NewLine;
            script += GetIndentString(1) + string.Format("set_sample_par('width', '{0}')", settings.SampleWidth) + Environment.NewLine;
            script += GetIndentString(1) + string.Format("set_sample_par('height', '{0}')", settings.SampleWidth) + Environment.NewLine;
            script += GetIndentString(1) + string.Format("set_sample_par('geometry', '{0}')", settings.SampleGeometry) + Environment.NewLine;

            return script;
        }

        private string GenerateRuns(ObservableCollection<Experiment> runs, ExperimentSettings settings)
        {
            string script = "";

            if (runs.Count == 0)
            {
                // Nothing to do
                return script;
            }

            if (settings.Order == RunOrder.AllTrans)
            {
                // All TRANS first
                script += GetIndentString(1) + string.Format("for i in range({0}):", settings.NumTrans) + Environment.NewLine;

                foreach (Experiment exp in runs)
                {
                    int indent = 2;
                    script = DoTransRun(script, exp, indent, settings);
                }

                // Then the SANS
                script += GetIndentString(1) + string.Format("for i in range({0}):", settings.NumSans) + Environment.NewLine;

                foreach (Experiment exp in runs)
                {
                    int indent = 2;
                    script = DoSansRun(script, exp, indent, settings);
                }
            }
            else if (settings.Order == RunOrder.AllSans)
            {
                // All SANS first
                script += GetIndentString(1) + string.Format("for i in range({0}):", settings.NumSans) + Environment.NewLine;

                foreach (Experiment exp in runs)
                {
                    int indent = 2;
                    script = DoSansRun(script, exp, indent, settings);
                }

                // Then the TRANS
                script += GetIndentString(1) + string.Format("for i in range({0}):", settings.NumTrans) + Environment.NewLine;

                foreach (Experiment exp in runs)
                {
                    int indent = 2;
                    script = DoTransRun(script, exp, indent, settings);
                }
            }
            else if (settings.Order == RunOrder.TransFirst || settings.Order == RunOrder.SansFirst)
            {
                // TRANS - SANS - TRANS - SANS etc. or SANS - TRANS - SANS - TRANS etc
                // Note: number of SANS and TRANS runs may be different
                script += GetIndentString(1) + string.Format("num_sans = {0}", settings.NumSans) + Environment.NewLine;
                script += GetIndentString(1) + string.Format("num_trans = {0}", settings.NumTrans) + Environment.NewLine;
                script += GetIndentString(1) + "count = 0" + Environment.NewLine;
                script += GetIndentString(1) + "while True:" + Environment.NewLine;

                int indent = 2;

                foreach (Experiment exp in runs)
                {
                    if (settings.Order == RunOrder.TransFirst)
                    {
                        script += GetIndentString(indent) + "if count < num_trans:" + Environment.NewLine;
                        script = DoTransRun(script, exp, indent + 1, settings);
                        script += GetIndentString(indent) + "if count < num_sans:" + Environment.NewLine;
                        script = DoSansRun(script, exp, indent + 1, settings);
                    }
                    else
                    {
                        script += GetIndentString(indent) + "if count < num_sans:" + Environment.NewLine;
                        script = DoSansRun(script, exp, indent + 1, settings);
                        script += GetIndentString(indent) + "if count < num_trans:" + Environment.NewLine;
                        script = DoTransRun(script, exp, indent + 1, settings);
                    }
                }

                script += GetIndentString(indent) + "count += 1" + Environment.NewLine;
                script += GetIndentString(indent) + "if count >= num_trans and count >= num_sans: break" + Environment.NewLine;
            }

            return script;
        }

        private string DoTransRun(string script, Experiment exp, int indent, ExperimentSettings settings)
        {
            script += DoSampleEnvironment(exp, indent);
            script += SetAperture(settings.A2SettingTrans, indent);

            string waitfor = "uamps"; // Default to uamps
            int rtype = 0; // Default to histogram mode

            if (settings.CollectionMode == DataCollection.Events)
            {
                rtype = 1;
            }

            if (exp.TransWait == WaitForUnits.Frames)
            {
                waitfor = "frames";
            }
            else if (exp.TransWait == WaitForUnits.Seconds)
            {
                waitfor = "seconds";
            }
            else if (exp.TransWait == WaitForUnits.Minutes)
            {
                waitfor = "minutes";
            }

            script += GetIndentString(indent) + string.Format("lm.dotrans_normal(position='{0}', title='{1}', {2}={3}, thickness={4}, rtype={5})",
                    exp.Position, exp.Sample, waitfor, exp.Trans, exp.Thickness, rtype) + Environment.NewLine;

            script += GetIndentString(indent) + exp.PostCommand + Environment.NewLine;
            return script;
        }

        private string DoSansRun(string script, Experiment exp, int indent, ExperimentSettings settings)
        {
            script += DoSampleEnvironment(exp, indent);
            script += SetAperture(settings.A2SettingSans, indent);

            string waitfor = "uamps"; // Default to uamps
            int rtype = 0; // Default to histogram mode

            if (settings.CollectionMode == DataCollection.Events)
            {
                rtype = 1;
            }

            if (exp.SansWait == WaitForUnits.Frames)
            {
                waitfor = "frames";
            }
            else if (exp.SansWait == WaitForUnits.Seconds)
            {
                waitfor = "seconds";
            }
            else if (exp.SansWait == WaitForUnits.Minutes)
            {
                waitfor = "minutes";
            }

            script += GetIndentString(indent) + string.Format("lm.dosans_normal(position='{0}', title='{1}', {2}={3}, thickness={4}, rtype={5})",
                    exp.Position, exp.Sample, waitfor, exp.Sans, exp.Thickness, rtype) + Environment.NewLine;

            script += GetIndentString(indent) + exp.PostCommand + Environment.NewLine;
            return script;
        }

        private string SetAperture(string size, int indent)
        {
            string script = "";

            if (size == A2Setting.Large || size == A2Setting.LarmorLarge)
            {
                script += GetIndentString(indent) + "set_aperture('large')" + Environment.NewLine;
            }
            else if (size == A2Setting.Medium || size == A2Setting.LarmorMedium)
            {
                script += GetIndentString(indent) + "set_aperture('medium')" + Environment.NewLine;
            }
            else if (size == A2Setting.Small || size == A2Setting.LarmorSmall)
            {
                script += GetIndentString(indent) + "set_aperture('small')" + Environment.NewLine;
            }

            return script;
        }

        private string DoSampleEnvironment(Experiment exp, int indent)
        {
            string script = "";

            // Set Temps first
            if (!string.IsNullOrWhiteSpace(exp.Temperature1))
            {
                script += GetIndentString(indent) + string.Format("cset(TEMP={0})", exp.Temperature1) + Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(exp.Temperature2))
            {
                script += GetIndentString(indent) + string.Format("cset(TEMP2={0})", exp.Temperature2) + Environment.NewLine;
            }

            // Check for pre-command - always run immediately after the temp is set
            if (!string.IsNullOrWhiteSpace(exp.PreCommand))
            {
                script += GetIndentString(indent) + exp.PreCommand + Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(exp.Field))
            {
                script += GetIndentString(indent) + string.Format("cset(FIELD={0})", exp.Field) + Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(exp.ShearRate1))
            {
                script += GetIndentString(indent) + string.Format("cset(SHEAR_RATE_1={0})", exp.ShearRate1) + Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(exp.ShearRate2))
            {
                script += GetIndentString(indent) + string.Format("cset(SHEAR_RATE_2={0})", exp.ShearRate2) + Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(exp.ShearAngle1))
            {
                script += GetIndentString(indent) + string.Format("cset(SHEAR_ANGLE_1={0})", exp.ShearAngle1) + Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(exp.ShearAngle2))
            {
                script += GetIndentString(indent) + string.Format("cset(SHEAR_ANGLE_2={0})", exp.ShearAngle1) + Environment.NewLine;
            }

            return script;
        }

        public string EstimateRunTime(ObservableCollection<Experiment> runs, ExperimentSettings settings, Estimation estimation)
        {
            double minutes = 0;

            if (settings.NumSans > 0)
            {
                minutes += CalculateTime(runs, true, estimation.CountRate, estimation.MoveTime) * settings.NumSans;
            }

            if (settings.NumTrans > 0)
            {
                minutes += CalculateTime(runs, false, estimation.CountRate, estimation.MoveTime) * settings.NumTrans;
            }

            DateTime time = new DateTime(1, 1, 1, 0, 0, 0);

            time = time.AddMinutes(minutes);

            return time.ToLongTimeString();
        }

        private static double CalculateTime(ObservableCollection<Experiment> runs, bool isSans, double countRate, double secondsBetween)
        {
            double minutes = 0;

            foreach (Experiment exp in runs)
            {
                if (!string.IsNullOrWhiteSpace(exp.Position))
                {
                    if (isSans)
                    {
                        minutes += GetMinutes(exp.SansWait, exp.Sans, countRate);
                    }
                    else
                    {
                        minutes += GetMinutes(exp.TransWait, exp.Trans, countRate);
                    }

                    minutes += secondsBetween / 60;
                }
            }

            return minutes;
        }

        private static double GetMinutes(string wait, double length, double countRate)
        {
            if (wait == WaitForUnits.Minutes)
            {
                return length;
            }
            else if (wait == WaitForUnits.Seconds)
            {
                return length / 60.0;
            }
            else if (wait == WaitForUnits.Frames)
            {
                return length / (25.0 * 60.0);
            }
            else
            {
                // If in doubt use micro-amps
                return length / (countRate / 60.0);
            }
        }
    }
}
