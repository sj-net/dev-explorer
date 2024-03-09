namespace FileExplorer.ViewModels
{
    using System;
    using System.Collections;
    using System.Linq;

    public partial class SystemVariableViewModel : BasePageViewModel
    {
        public SystemVariableViewModel()
        {
            UniqueName = "Environment Variables";

            Initialize();
        }

        public void Initialize()
        {
            /*To get all Environmentvariables */
            var r = Environment.GetEnvironmentVariables().Cast<DictionaryEntry>().OrderBy(entry => entry.Key);

            foreach (DictionaryEntry variable in r)
            {
                EnvironmentVariables.Add(variable.Key.ToString(), variable.Value.ToString());
            }

            /* To get user variables */
            var user = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User).Cast<DictionaryEntry>().OrderBy(entry => entry.Key);

            foreach (var va in user)
            {
                UserVariables.Add(va.Key.ToString(), va.Value.ToString());
            }

            /* To get system variables */
            var system = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine).Cast<DictionaryEntry>().OrderBy(entry => entry.Key);

            foreach (var va in system)
            {
                SystemVariables.Add(va.Key.ToString(), va.Value.ToString());
            }

        }
    }
}
