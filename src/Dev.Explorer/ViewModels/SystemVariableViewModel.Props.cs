namespace FileExplorer.ViewModels
{
    using System.Collections.Generic;
    
    public partial class SystemVariableViewModel : BasePageViewModel
    {
        public Dictionary<string, string> EnvironmentVariables { get; set; } = new();
        public Dictionary<string, string> UserVariables { get; set; } = new();
        public Dictionary<string, string> SystemVariables { get; set; } = new();
    }
}
