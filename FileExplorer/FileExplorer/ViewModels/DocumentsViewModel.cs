namespace FileExplorer.ViewModels
{
    using FileExplorer.Commands;
    using FileExplorer.Interfaces;
    using FileExplorer.Models;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;

    public class DocumentsViewModel : BasePageViewModel, IBookMarksViewModel
    {
        private string documentName;
        private string documentURL;

        public string DocumnetName
        {
            get
            {
                return documentName;
            }
            set
            {
                documentName = value;
                RaisePropertyChanged(nameof(documentName));
            }
        }
        public string DocumnetURL
        {
            get
            {
                return documentURL;
            }
            set
            {
                documentURL = value;
                RaisePropertyChanged(nameof(documentURL));
            }
        }

        public DocumentsViewModel()
        {
            BookMarksStoringPath = "C:/BookMarks/ImportantDocuments.txt";
            BookMarksFolderPath = "C:/BookMarks";
            UniqueName = "Important Documents";

            InitializeBookMarks(BookMarksFolderPath, BookMarksStoringPath);
        }

        public string BookMarksFolderPath { get; private set; }

        public string BookMarksStoringPath { get; private set; }

        public ObservableCollection<IBookMark> BookMarks { get; private set; } = new();

        public RelayCommand<string> AddBookMarkCommand { get; private set; }

        public RelayCommand<string> DeleteBookMarkCommand { get; private set; }

        public void InitializeBookMarks(string folderPath, string storingPath)
        {
            Directory.CreateDirectory(BookMarksFolderPath);
            using (StreamWriter writer = new StreamWriter(BookMarksStoringPath, true)) ;
            List<string> bookMarks = new List<string>(File.ReadAllLines(BookMarksStoringPath));

            File.ReadAllLines(BookMarksStoringPath).ToList().ForEach(path =>
            {
                if (path != null && path != string.Empty)
                {
                    BookMarks.Add(new BookMarksModel(path, path));
                }
            });
        }
    }
}
