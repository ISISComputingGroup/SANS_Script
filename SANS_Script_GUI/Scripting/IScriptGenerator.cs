using System;
using System.Collections.ObjectModel;

namespace LOQ_Script_Gui.Scripting
{
    interface IScriptGenerator
    {
        string GenerateScript(ObservableCollection<Experiment> runs, ExperimentSettings settings);
        string EstimateRunTime(ObservableCollection<Experiment> runs, ExperimentSettings settings, Estimation estimation);
    }
}
