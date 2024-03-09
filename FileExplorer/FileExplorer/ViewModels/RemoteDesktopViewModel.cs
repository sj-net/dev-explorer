namespace FileExplorer.ViewModels
{
    using CommunityToolkit.Mvvm.Messaging;
    using FileExplorer.Commands;
    using FileExplorer.Interfaces;
    using FileExplorer.Models;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    public partial class RemoteDesktopViewModel : BasePageViewModel, IBookMarksViewModel
    {
        public RemoteDesktopViewModel()
        {
            BookMarksStoringPath = "C:/BookMarks/Servers.txt";
            BookMarksFolderPath = "C:/BookMarks";
            UniqueName = "Remote Desktop";

            InitializeCommands();
            InitializeBookMarks(BookMarksFolderPath, BookMarksStoringPath);
        }
        public void InitializeCommands()
        {
            SetServerIDCommand = new RelayCommand<string>((server) =>
            {
                ServerName = server;
            },
            (server) =>
            {
                return server is not null && server != string.Empty;
            });

            AddBookMarkCommand = new RelayCommand<string>((server) =>
            {
                Process.Start("mstsc.exe", $"/v:{ServerName}");

                if (!BookMarks.Any(A => A.DisplayName == server) && server != string.Empty)
                {
                    using (StreamWriter sw = File.AppendText(BookMarksStoringPath))
                    {
                        sw.WriteLine(server);
                    }

                    BookMarks.Clear();
                    File.ReadAllLines(BookMarksStoringPath).ToList().ForEach(path =>
                    {
                        if (path != null && path != string.Empty)
                        {
                            BookMarks.Add(new BookMarksModel(path, path));
                        }
                    });
                }
            },
            (server) =>
            {
                return server != null && ServerName != string.Empty;
            });

            DeleteBookMarkCommand = new RelayCommand<string>((server) =>
            {
                if (server != string.Empty && BookMarks.Any(_ => _.ReferenceMember == server))
                {
                    List<string> bookMarks = new List<string>(File.ReadAllLines(BookMarksStoringPath));
                    string bookMark = bookMarks.FirstOrDefault(_ => _ == server);
                    bookMarks.Remove(bookMark);

                    using (StreamWriter writer = new StreamWriter(BookMarksStoringPath))
                    {
                        foreach (string bm in bookMarks)
                        {
                            writer.WriteLine(bm);
                        }
                    }
                    if (bookMarks.Count() != BookMarks.Count())
                    {
                        BookMarks.Clear();
                        bookMarks.ForEach(path =>
                        {
                            if (path != null && path != string.Empty)
                            {
                                BookMarks.Add(new BookMarksModel(path, path));
                            }
                        });

                        StrongReferenceMessenger.Default.Send(new ToasterViewModel(ToasterType.Success, $"Bookmark deleted successfully"));
                    }
                }
            },
           (server) =>
           {
               return true;
           });
        }
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
