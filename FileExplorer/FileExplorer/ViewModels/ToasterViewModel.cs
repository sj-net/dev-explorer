using System;

namespace FileExplorer.ViewModels
{
    public enum ToasterType
    {
        Success = 0,
        Error = 1,
        Warning = 2,
        Info = 3,
    }
    public class ToasterViewModel
    {
        public ToasterViewModel(ToasterType type, string message, bool canShowInUI = true)
        {
            Type = type;
            Message = message ?? throw new ArgumentNullException(nameof(message));
            CanShowInUI = canShowInUI;
        }
        public ToasterType Type { get; set; }
        public string Message { get; set; }
        public bool CanShowInUI { get; set; }
        public int Timer { get; set; } = 3000;
    }
}
