using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace LOQ_Script_Gui.Scripting
{
    class OpenGenieScriptGenerator : IScriptGenerator
    {
        private bool isSans2d = false;

        public OpenGenieScriptGenerator(bool isSans2d)
        {
            this.isSans2d = isSans2d;
        }

        public string GenerateScript(ObservableCollection<Experiment> runs, ExperimentSettings settings)
        {
            string script = "# Script created by LOQ Script at " + DateTime.Now + Environment.NewLine;
            script += "# Script MUST be loaded in to Open GENIE using the loadscript command." + Environment.NewLine + Environment.NewLine;

            if (isSans2d)
            {
                script += "QUALIFIERS /test" + Environment.NewLine;
            }

            script += string.Format("LOCAL transCount = 0 sansCount = 0 numTRANS={0} numSANS={1}", settings.NumTrans, settings.NumSans);
            script += Environment.NewLine + Environment.NewLine;

            script += "SETSCRIPTNAME THIS_PROCEDURE()" + Environment.NewLine + Environment.NewLine;

            script += string.Format("Sample_par width={0} height={1} geometry=\"{2}\"", settings.SampleWidth, settings.SampleHeight, settings.SampleGeometry);
            script += Environment.NewLine + Environment.NewLine;

            if (isSans2d)
            {
                string positionlist = "\"";

                foreach (Experiment exp in runs)
                {
                    if (!string.IsNullOrWhiteSpace(exp.Position))
                    {
                        positionlist += exp.Position + ",";
                    }
                }

                if (positionlist.Length > 1)
                {
                    if (positionlist.EndsWith(","))
                    {
                        positionlist = positionlist.Substring(0, positionlist.Length - 1);
                    }

                    positionlist += "\"";

                    script += string.Format("IF NOT CHECK_MOVE_POS({0})", positionlist);
                    script += Environment.NewLine;
                    script += "    PRINTEN \"Invalid positions - script aborted\"" + Environment.NewLine;
                    script += "    RETURN" + Environment.NewLine;
                    script += "ENDIF" + Environment.NewLine + Environment.NewLine;

                }

                script += "IF test" + Environment.NewLine;
                script += "    RETURN" + Environment.NewLine;
                script += "ENDIF" + Environment.NewLine + Environment.NewLine;
            }

            if (settings.Order == RunOrder.AllTrans)
            {
                // Do all trans first
                if (settings.LoopPerRun)
                {
                    script += DoAllTransLoopEach(runs, settings, true, true);
                    script += DoAllSansLoopEach(runs, settings, true, true);
                }
                else
                {
                    script += "LOOP" + Environment.NewLine + Environment.NewLine;
                    script += DoAllTrans(runs, settings, true, true);
                    script += DoAllSans(runs, settings, true, true);
                    script += "EXITIF ((transCount >= numTRANS) AND (sansCount >= numSANS))" + Environment.NewLine + Environment.NewLine;
                    script += "ENDLOOP" + Environment.NewLine + Environment.NewLine;
                }
            }
            else if (settings.Order == RunOrder.TransFirst)
            {
                // Do Trans-Sans-Trans-Sans...
                if (settings.LoopPerRun)
                {
                    foreach (Experiment exp in runs)
                    {
                        script += DoSingleTransLoopEach(exp, settings, true, false);
                        script += DoSingleSansLoopEach(exp, settings, false, true);
                    }
                }
                else
                {
                    script += "LOOP" + Environment.NewLine + Environment.NewLine;

                    foreach (Experiment exp in runs)
                    {
                        script += DoSingleTrans(exp, settings, true, false);
                        script += DoSingleSans(exp, settings, false, true);
                    }

                    script += "transCount = transCount + 1" + Environment.NewLine;
                    script += "sansCount = sansCount + 1" + Environment.NewLine + Environment.NewLine;
                    script += "EXITIF ((transCount >= numTRANS) AND (sansCount >= numSANS))" + Environment.NewLine + Environment.NewLine;
                    script += "ENDLOOP" + Environment.NewLine + Environment.NewLine;
                }
            }
            else if (settings.Order == RunOrder.AllSans)
            {
                // Do All Sans first
                if (settings.LoopPerRun)
                {
                    script += DoAllSansLoopEach(runs, settings, true, true);
                    script += DoAllTransLoopEach(runs, settings, true, true);
                }
                else
                {
                    script += "LOOP" + Environment.NewLine + Environment.NewLine;
                    script += DoAllSans(runs, settings, true, true);
                    script += DoAllTrans(runs, settings, true, true);
                    script += "EXITIF ((transCount >= numTRANS) AND (sansCount >= numSANS))" + Environment.NewLine + Environment.NewLine;
                    script += "ENDLOOP" + Environment.NewLine + Environment.NewLine;
                }
            }
            else if (settings.Order == RunOrder.SansFirst)
            {
                // Do Sans-Trans-Sans-Trans...
                if (settings.LoopPerRun)
                {
                    foreach (Experiment exp in runs)
                    {
                        script += DoSingleSansLoopEach(exp, settings, true, false);
                        script += DoSingleTransLoopEach(exp, settings, false, true);
                    }
                }
                else
                {
                    script += "LOOP" + Environment.NewLine + Environment.NewLine;

                    foreach (Experiment exp in runs)
                    {
                        script += DoSingleSans(exp, settings, true, false);
                        script += DoSingleTrans(exp, settings, false, true);
                    }

                    script += "transCount = transCount + 1" + Environment.NewLine;
                    script += "sansCount = sansCount + 1" + Environment.NewLine + Environment.NewLine;
                    script += "EXITIF ((transCount >= numTRANS) AND (sansCount >= numSANS))" + Environment.NewLine + Environment.NewLine;
                    script += "ENDLOOP" + Environment.NewLine + Environment.NewLine;
                }
            }

            //script += "ENDPROCEDURE" + Environment.NewLine;
            script += "PRINTN \"SCRIPT COMPLETED \"" + Environment.NewLine + Environment.NewLine;

            return script;
        }

        private static string DoSingleSans(Experiment exp, ExperimentSettings settings, bool doPreCom, bool doPostCom)
        {
            string script = "";

            if (settings.NumSans > 0)
            {
                if (exp.Trans > 0)
                {
                    script += "IF (sansCount < numSANS)" + Environment.NewLine;
                    script += setSansAperture(settings.A2SettingSans);
                    script += DoSingleRun(false, exp, false, doPreCom, doPostCom);
                    script += "ENDIF" + Environment.NewLine + Environment.NewLine;
                }
            }
            return script;
        }

        private static string DoSingleTrans(Experiment exp, ExperimentSettings settings, bool doPreCom, bool doPostCom)
        {
            string script = "";

            if (settings.NumTrans > 0)
            {
                if (exp.Trans > 0)
                {
                    script += "IF (transCount < numTRANS)" + Environment.NewLine;
                    script += setTransAperture(settings.A2SettingTrans);
                    script += DoSingleRun(true, exp, false, doPreCom, doPostCom);
                    script += "ENDIF" + Environment.NewLine + Environment.NewLine;
                }
            }
            return script;
        }

        private static string DoSingleSansLoopEach(Experiment exp, ExperimentSettings settings, bool doPreCom, bool doPostCom)
        {
            string script = "";

            if (settings.NumSans > 0)
            {
                if (exp.Sans > 0)
                {
                    script += setSansAperture(settings.A2SettingSans);
                    script += DoSingleRun(false, exp, true, doPreCom, doPostCom) + Environment.NewLine;
                }
            }
            return script;
        }

        private static string DoSingleTransLoopEach(Experiment exp, ExperimentSettings settings, bool doPreCom, bool doPostCom)
        {
            string script = "";

            if (settings.NumTrans > 0)
            {
                if (exp.Trans > 0)
                {
                    script += setTransAperture(settings.A2SettingTrans);
                    script += DoSingleRun(true, exp, true, doPreCom, doPostCom) + Environment.NewLine;
                }
            }
            return script;
        }

        private static string DoAllSans(ObservableCollection<Experiment> runs, ExperimentSettings settings, bool doPreCom, bool doPostCom)
        {
            string script = "";

            if (settings.NumSans > 0)
            {
                script += setSansAperture(settings.A2SettingSans);

                script += "IF sansCount < numSANS" + Environment.NewLine + Environment.NewLine;

                // Loop through each experiment
                foreach (Experiment exp in runs)
                {
                    script += DoSingleRun(false, exp, false, doPreCom, doPostCom) + Environment.NewLine;
                }

                script += "sansCount = sansCount + 1" + Environment.NewLine + Environment.NewLine;
                script += "ENDIF" + Environment.NewLine + Environment.NewLine;
            }

            return script;
        }

        private static string DoAllTrans(ObservableCollection<Experiment> runs, ExperimentSettings settings, bool doPreCom, bool doPostCom)
        {
            string script = "";

            if (settings.NumTrans > 0)
            {
                script += setTransAperture(settings.A2SettingTrans);

                script += "IF transCount < numTRANS" + Environment.NewLine + Environment.NewLine;

                // Loop through each experiment
                foreach (Experiment exp in runs)
                {
                    script += DoSingleRun(true, exp, false, doPreCom, doPostCom) + Environment.NewLine;
                }

                script += "transCount = transCount + 1" + Environment.NewLine + Environment.NewLine;
                script += "ENDIF" + Environment.NewLine + Environment.NewLine;
            }

            return script;
        }

        private static string DoAllSansLoopEach(ObservableCollection<Experiment> runs, ExperimentSettings settings, bool doPreCom, bool doPostCom)
        {
            string script = "";

            if (settings.NumSans > 0)
            {
                script += setSansAperture(settings.A2SettingSans);

                // Loop through each experiment
                foreach (Experiment exp in runs)
                {
                    script += DoSingleRun(false, exp, true, doPreCom, doPostCom) + Environment.NewLine;
                }
            }

            return script;
        }

        private static string DoAllTransLoopEach(ObservableCollection<Experiment> runs, ExperimentSettings settings, bool doPreCom, bool doPostCom)
        {
            string script = "";

            if (settings.NumTrans > 0)
            {
                script += setTransAperture(settings.A2SettingTrans);

                // Loop through each experiment
                foreach (Experiment exp in runs)
                {
                    script += DoSingleRun(true, exp, true, doPreCom, doPostCom) + Environment.NewLine;
                }
            }

            return script;
        }

        private static string setSansAperture(string A2SizeSans)
        {
            string script = "";

            if (A2SizeSans == A2Setting.Large)
            {
                script += "DO_SANS /LARGE" + Environment.NewLine + Environment.NewLine;
            }
            else if (A2SizeSans == A2Setting.Medium)
            {
                script += "DO_SANS /MEDIUM" + Environment.NewLine + Environment.NewLine;
            }
            else if (A2SizeSans == A2Setting.Small)
            {
                script += "DO_SANS /SMALL" + Environment.NewLine + Environment.NewLine;
            }
            else if (A2SizeSans == A2Setting.Trans)
            {
                script += "DO_SANS /TRANS" + Environment.NewLine + Environment.NewLine;
            }

            return script;
        }

        private static string setTransAperture(string A2SizeTrans)
        {
            string script = "";

            if (string.IsNullOrWhiteSpace(A2SizeTrans))
            {
                script += "DO_TRANS" + Environment.NewLine + Environment.NewLine;
            }
            else
            {
                if (A2SizeTrans == A2Setting.Large)
                {
                    script += "DO_TRANS /LARGE" + Environment.NewLine + Environment.NewLine;
                }
                else if (A2SizeTrans == A2Setting.Medium)
                {
                    script += "DO_TRANS /MEDIUM" + Environment.NewLine + Environment.NewLine;
                }
                else if (A2SizeTrans == A2Setting.Small)
                {
                    script += "DO_TRANS /SMALL" + Environment.NewLine + Environment.NewLine;
                }
                else if (A2SizeTrans == A2Setting.Trans)
                {
                    script += "DO_TRANS /TRANS" + Environment.NewLine + Environment.NewLine;
                }
            }

            return script;
        }

        private static string DoSingleRun(bool isTrans, Experiment exp, bool isLoop, bool doPreCom, bool doPostCom)
        {
            string script = "";

            if (exp.Trans > 0)
            {
                if (!string.IsNullOrWhiteSpace(exp.RbNumber))
                {
                    script += "change rbno=" + exp.RbNumber + Environment.NewLine;
                }

                if (!string.IsNullOrWhiteSpace(exp.Position))
                {
                    if (doPreCom)
                    {
                        script += DoSampleEnvironment(exp);
                    }

                    if (isLoop)
                    {
                        if (isTrans)
                        {
                            script += "LOOP transCount FROM 1 TO numTRANS" + Environment.NewLine;
                        }
                        else
                        {
                            script += "LOOP sansCount FROM 1 TO numSANS" + Environment.NewLine;
                        }

                        script += CreateMoveCommand(isTrans, exp);
                        script += "ENDLOOP" + Environment.NewLine;
                    }
                    else
                    {
                        script += CreateMoveCommand(isTrans, exp);
                    }

                    // Check for post-command - always run last
                    if (doPostCom && !string.IsNullOrWhiteSpace(exp.PostCommand))
                    {
                        script += exp.PostCommand + Environment.NewLine;
                    }
                }
            }

            return script;
        }

        private static string DoSampleEnvironment(Experiment exp)
        {
            string script = "";

            // Set Temps first
            if (!string.IsNullOrWhiteSpace(exp.Temperature1))
            {
                script += "CSET TEMP=" + exp.Temperature1 + Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(exp.Temperature2))
            {
                script += "CSET TEMP2=" + exp.Temperature2 + Environment.NewLine;
            }

            // Check for pre-command - always run immediately after the temp is set
            if (!string.IsNullOrWhiteSpace(exp.PreCommand))
            {
                script += exp.PreCommand + Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(exp.Field))
            {
                script += "CSET FIELD=" + exp.Field + Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(exp.ShearRate1))
            {
                script += "CSET SHEAR_RATE_1=" + exp.ShearRate1 + Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(exp.ShearRate2))
            {
                script += "CSET SHEAR_RATE_2=" + exp.ShearRate2 + Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(exp.ShearAngle1))
            {
                script += "CSET SHEAR_ANGLE_1=" + exp.ShearAngle1 + Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(exp.ShearAngle2))
            {
                script += "CSET SHEAR_ANGLE_2=" + exp.ShearAngle2 + Environment.NewLine;
            }

            return script;
        }

        private static string CreateMoveCommand(bool isTrans, Experiment exp)
        {
            string script = "MOVE";

            if (!string.IsNullOrWhiteSpace(exp.Position))
            {
                script += " pos=\"" + exp.Position + "\"";
            }

            if (!string.IsNullOrWhiteSpace(exp.Thickness))
            {
                script += " thick=" + exp.Thickness;
            }
            else
            {
                script += " thick=1";
            }

            if (!string.IsNullOrWhiteSpace(exp.Period))
            {
                script += " period=" + exp.Period;
            }

            if (isTrans)
            {
                script += WriteWaitFor(exp.TransWait, exp.Trans);

                if (!string.IsNullOrWhiteSpace(exp.Sample))
                {
                    script += string.Format(" title=\"{0}_TRANS\"", exp.Sample);
                }
            }
            else
            {
                script += WriteWaitFor(exp.SansWait, exp.Sans);

                if (!string.IsNullOrWhiteSpace(exp.Sample))
                {
                    script += string.Format(" title=\"{0}_SANS\"", exp.Sample);
                }
            }

            return script + Environment.NewLine;

        }

        private static string WriteWaitFor(string waitfor, double length)
        {
            string script = "";

            if (!string.IsNullOrWhiteSpace(waitfor))
            {
                if (waitfor.ToLower() == WaitForUnits.MicroAmps.ToLower())
                {
                    script += " uAhr=" + length;
                }
                else if (waitfor.ToLower() == WaitForUnits.Minutes.ToLower())
                {
                    script += " min=" + length;
                }
                else if (waitfor.ToLower() == WaitForUnits.Seconds.ToLower())
                {
                    script += " seconds=" + length;
                }
                else if (waitfor.ToLower() == WaitForUnits.Frames.ToLower())
                {
                    script += " frame=" + length;
                }
                else
                {
                    // If in doubt use micro-amps
                    script += " uAhr=" + length;
                }
            }
            else
            {
                // If in doubt use micro-amps
                script += " uAhr=" + length;
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
