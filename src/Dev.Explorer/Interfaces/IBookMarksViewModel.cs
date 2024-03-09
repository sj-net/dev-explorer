namespace FileExplorer.Interfaces
{
    using FileExplorer.Commands;
    using System.Collections.ObjectModel;

    public interface IBookMarksViewModel
    {
        string BookMarksFolderPath { get; }
        string BookMarksStoringPath { get; }
        ObservableCollection<IBookMark> BookMarks { get; }
        RelayCommand<string> AddBookMarkCommand { get; }
        RelayCommand<string> DeleteBookMarkCommand { get; }
        void InitializeBookMarks(string folderPath, string storingPath);
    }
}
