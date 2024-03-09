namespace FileExplorer.ViewModels
{
    using FileExplorer.Commands;
    using FileExplorer.Interfaces;
    using System.Collections.ObjectModel;

    public partial class RemoteDesktopViewModel : BasePageViewModel, IBookMarksViewModel
    {
        private string serverName;

        public string ServerName
        {
            get { return serverName; }
            set
            {
                serverName = value;
                RaisePropertyChanged(nameof(ServerName));
            }
        }
        public string BookMarksFolderPath { get; set; }
        public string BookMarksStoringPath { get; set; }

        public ObservableCollection<IBookMark> BookMarks { get; set; } = new();
        public RelayCommand<string> SetServerIDCommand { get; set; }
        public RelayCommand<string> AddBookMarkCommand { get; set; }
        public RelayCommand<string> DeleteBookMarkCommand { get; set; }
    }
}
