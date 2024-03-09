namespace FileExplorer.ViewModels
{
    using FileExplorer.Commands;
    using FileExplorer.Interfaces;
    using FileExplorer.Models;

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    public partial class FileExplorerViewModel : BasePageViewModel, IBookMarksViewModel
    {
        private string resultPath = string.Empty;
        private bool canShowHiddenFiles;
        private bool areCommandsAllowed;
        private int foldersCount;
        private int filessCount;
        private string outputMessage;
        private string selectedBranch;
        private string searchPath;
        private bool isPopUpOpen;
        private bool isadmin;

        public string ResultPath
        {
            get { return resultPath; }
            set
            {
                resultPath = value;
                RaisePropertyChanged(nameof(ResultPath));
            }
        }
        public bool IsAdmin
        {
            get { return isadmin; }
            set
            {
                isadmin = value;
                RaisePropertyChanged(nameof(IsAdmin));
            }
        }

        public bool CanShowHiddenFiles
        {
            get { return canShowHiddenFiles; }
            set
            {
                canShowHiddenFiles = value;
                RaisePropertyChanged(nameof(CanShowHiddenFiles));
            }
        }
        public bool AreCommandsAllowed
        {
            get { return areCommandsAllowed; }
            set
            {
                areCommandsAllowed = value;
                RaisePropertyChanged(nameof(AreCommandsAllowed));
            }
        }
        public int FoldersCount
        {
            get
            {
                return foldersCount;
            }
            set
            {
                foldersCount = value;
                RaisePropertyChanged(nameof(FoldersCount));
            }
        }
        public int FilesCount
        {
            get
            {
                return filessCount;
            }
            set
            {
                filessCount = value;
                RaisePropertyChanged(nameof(FilesCount));
            }
        }
        public string OutputMessage
        {
            get { return outputMessage; }
            set
            {
                outputMessage = value;
                RaisePropertyChanged(nameof(OutputMessage));
            }
        }
        public string SearchPath
        {
            get { return searchPath; }
            set
            {
                searchPath = value;
                RaisePropertyChanged(nameof(SearchPath));
            }
        }
        public string SelectedBranch
        {
            get
            {
                return selectedBranch;
            }
            set
            {
                if (selectedBranch != value && value != null)
                {
                    selectedBranch = value;
                    IsPopUpOpen = false;
                }
                RaisePropertyChanged(nameof(SelectedBranch));
            }
        }
        public bool IsPopUpOpen
        {
            get
            {
                return isPopUpOpen;
            }
            set
            {
                isPopUpOpen = value;
                RaisePropertyChanged(nameof(IsPopUpOpen));
            }
        }
        public string BookMarksFolderPath { get; set; }
        public string BookMarksStoringPath { get; set; }

        public ObservableCollection<DrivesModel> Directories { get; set; } = new();
        public ObservableCollection<DirectoriesModel> DirectoryFiles { get; set; } = new();
        public ObservableCollection<DirectoriesModel> NavItems { get; set; } = new();
        public ObservableCollection<DirectoriesModel> PowerScriptFiles { get; set; } = new();
        public ObservableCollection<IBookMark> BookMarks { get; set; } = new();
        public ObservableCollection<TabControlModel<string>> GitBranches { get; set; } = new();
        public ObservableCollection<Process> RunningApps { get; set; } = new();
        public List<DirectoriesModel> HiddenFiles { get; set; } = new();
        public RelayCommand<DirectoriesModel> LoadFoldersCommand { get; set; }
        public RelayCommand<string> LoadDrivesCommand { get; set; }
        public RelayCommand<string> BookMarkOnClickCommand { get; set; }
        public RelayCommand<bool> ShowHiddenFilesCommand { get; set; }
        public RelayCommand<string> AddBookMarkCommand { get; set; }
        public RelayCommand<string> DeleteBookMarkCommand { get; set; }
        public RelayCommand<string> CopyCommand { get; set; }
        public RelayCommand<string> GitPullCommand { get; set; }
        public RelayCommand<string> GitCheckoutCommand { get; set; }
        public RelayCommand<string> GitFetchCommand { get; set; }
        public RelayCommand<string> GitCleanCommand { get; set; }
        public RelayCommand<string> ShowMessageCommand { get; set; }
        public RelayCommand<string> CleanBinFilesCommand { get; set; }
        public RelayCommand<string> CleanObjFilesCommand { get; set; }
        public RelayCommand<string> OpenTerminalCommand { get; set; }
        public RelayCommand<string> OpenRunningAppCommand { get; set; }
        public RelayCommand<DirectoriesModel> ExecuteScriptFileCommand { get; private set; }
    }
}
