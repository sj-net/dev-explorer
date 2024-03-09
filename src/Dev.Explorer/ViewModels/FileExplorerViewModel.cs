namespace FileExplorer.ViewModels
{
    using CommunityToolkit.Mvvm.Messaging;

    using FileExplorer.Commands;
    using FileExplorer.Models;
    using FileExplorer.Utilities;
    using LibGit2Sharp;

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows;

    public partial class FileExplorerViewModel : BasePageViewModel
    {
        public FileExplorerViewModel()
        {
            BookMarksStoringPath = "C:/BookMarks/BookMarks.txt";
            BookMarksFolderPath = "C:/BookMarks";
            UniqueName = "Dev Explorer";

            InitializeBookMarks(BookMarksFolderPath, BookMarksStoringPath);
            InitializeDrives();
            InitializeCommands();

            // Running apps will be fetched on launching.
            // TO DO: Need to do the refreshing of current running apps.
            GetCurrentRunningApps();
        }

        static long CalculateFolderSize(string folderPath)
        {
            long size = 0;

            try
            {
                // Calculate size of all files in the folder and its subfolders
                foreach (string filePath in Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    size += fileInfo.Length;
                }
            }
            catch (Exception ex)
            {
                StrongReferenceMessenger.Default.Send(new ToasterViewModel(ToasterType.Error, ex.Message));
            }

            return size;
        }
        private void GetUpdatedNavItems(string currentPath)
        {
            List<string> navItems = new List<string>(currentPath.Split('\\').Where(_ => _ != string.Empty));
            var getSelectedDrive = Directories.FirstOrDefault(a => a.DrivePath == $"{navItems[0]}\\");

            Directories.ToList().ForEach(a =>
            {
                a.IsSelected = false;
                if (a == getSelectedDrive)
                {
                    a.IsSelected = true;
                }
            });

            NavItems.Clear();
            string path = string.Empty;

            for (int i = 0; i < navItems.Count; i++)
            {
                if(i == 0)
                {
                    path += $"{navItems[i]}\\";
                }
                else if(i == 1)
                {
                    path += $"{navItems[i]}";
                }
                else
                {
                    path += $"\\{navItems[i]}";
                }
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                NavItems.Add(new DirectoriesModel() { FilePath = directoryInfo.FullName, FileName = directoryInfo.Name });
            }

            var lastItem = NavItems.LastOrDefault();
            NavItems.ToList().ForEach(a =>
                {
                    if (a == lastItem)
                    {
                        a.IsLastFile = true;
                    }
                });
            RaisePropertyChanged(nameof(NavItems));
        }
        private void RefreshFiles(string currentPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(currentPath);
            List<DirectoryInfo> folders = new List<DirectoryInfo>(directoryInfo.GetDirectories());
            List<FileInfo> files = new List<FileInfo>(directoryInfo.GetFiles());
            PowerScriptFiles.Clear();
            DirectoryFiles.Clear();
            HiddenFiles.Clear();

            if (folders.Any() || files.Any())
            {
                foreach (var folder in folders)
                {
                    if ((folder.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    {
                        // if you want folder size use the commented code.
                        // HiddenFiles.Add(new DirectoriesModel() { FileName = folder.Name, FilePath = folder.FullName, CreationTime = folder.CreationTime, FileSize = CalculateFolderSize(folder.FullName) });
                        HiddenFiles.Add(new DirectoriesModel() { FileName = folder.Name, FilePath = folder.FullName, CreationTime = folder.CreationTime });
                    }
                    else
                    {
                        // DirectoryFiles.Add(new DirectoriesModel() { FileName = folder.Name, FilePath = folder.FullName, CreationTime = folder.CreationTime, FileSize = CalculateFolderSize(folder.FullName) });
                        DirectoryFiles.Add(new DirectoriesModel() { FileName = folder.Name, FilePath = folder.FullName, CreationTime = folder.CreationTime });
                    }
                }

                foreach (var file in files)
                {
                    if ((file.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    {
                        HiddenFiles.Add(new DirectoriesModel() { FilePath = file.FullName, FileName = file.Name, CreationTime = file.CreationTime, IsFile = true, FileSize = file.Length });
                    }
                    else
                    {
                        DirectoryFiles.Add(new DirectoriesModel() { FilePath = file.FullName, FileName = file.Name, CreationTime = file.CreationTime, IsFile = true, FileSize = file.Length });
                        if (file.Name.EndsWith(".ps1"))
                        {
                            PowerScriptFiles.Add(new DirectoriesModel() { FilePath = file.FullName, FileName = file.Name, CreationTime = file.CreationTime, FileSize = file.Length });
                        }
                    }
                }
            }

            AreCommandsAllowed = HiddenFiles.Any(
                 _ => _.FileName == ".git" &&
                 new FileInfo(_.FilePath).Attributes == (FileAttributes.Hidden | FileAttributes.Directory));

            GitBranches.Clear();
            SelectedBranch = string.Empty;
            if (AreCommandsAllowed)
            {
                SetGitBranches(currentPath);
                SelectedBranch = GetCurrentGitBranch(currentPath, "git", "rev-parse --abbrev-ref HEAD");
            }
            if (CanShowHiddenFiles)
            {
                HiddenFiles.ForEach(_ => DirectoryFiles.Add(_));
            }
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
                    DirectoryInfo directoryInfo = new DirectoryInfo(path);
                    BookMarks.Add(new BookMarksModel(directoryInfo.Name, directoryInfo.FullName));
                }
            });
        }
        private void InitializeDrives()
        {
            List<string> drives = new List<string>(Directory.GetLogicalDrives());
            drives.ForEach(drive =>
            {
                if (drive != null && drive != string.Empty)
                {
                    Directories.Add(new DrivesModel() { DrivePath = drive, DriveName = System.IO.Path.GetDirectoryName(drive) });
                }
            });
        }
        private void InitializeCommands()
        {
            LoadFoldersCommand = new RelayCommand<DirectoriesModel>((item) =>
            {
                try
                {
                    if (!item.IsFile)
                    {
                        LoadData(item.FilePath);
                    }
                    else
                    {
                        Process.Start("explorer.exe", item.FilePath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            },
            (folderPath) =>
            {
                FoldersCount = DirectoryFiles.Where(_ => !_.IsFile).Count();
                FilesCount = DirectoryFiles.Where(_ => _.IsFile).Count();
                return true;
            });

            LoadDrivesCommand = new RelayCommand<string>((drive) =>
            {
                try
                {
                    LoadData(drive);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            },
            (drive) =>
            {
                return true;
            });
            
            BookMarkOnClickCommand = new RelayCommand<string>((bookmark) =>
            {
                try
                {
                    LoadData(bookmark);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            },
            (bookmark) =>
            {
                return true;
            });
            
            OpenTerminalCommand = new RelayCommand<string>((path) =>
            {
                if (path == "Terminal")
                {
                    try
                    {
                        if (IsAdmin)
                        {
                            Process cmd = new Process()
                            {
                                StartInfo = {
                             Verb = "runas",
                             WorkingDirectory = ResultPath,
                             FileName = "cmd.exe",
                             UseShellExecute = true  }
                            };
                            cmd.Start();
                            string output = cmd.StandardOutput.ReadToEnd();
                            string error = cmd.StandardError.ReadToEnd();
                            cmd.WaitForExit();
                            SetMessage(cmd, output, error);
                        }
                        else
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "cmd.exe",
                                WorkingDirectory = ResultPath,
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        StrongReferenceMessenger.Default.Send(new ToasterViewModel(ToasterType.Info, ex.Message));
                    }
                }
                else if (path == "Power Shell")
                {
                    try
                    {
                        if (IsAdmin)
                        {
                            Process cmd = new Process()
                            {
                                StartInfo = {
                             Verb = "runas",
                             WorkingDirectory = ResultPath,
                             FileName = "powershell.exe",
                             UseShellExecute = true  }
                            };
                            cmd.Start();
                            string output = cmd.StandardOutput.ReadToEnd();
                            string error = cmd.StandardError.ReadToEnd();
                            cmd.WaitForExit();
                            SetMessage(cmd, output, error);
                        }
                        else
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "powershell.exe",
                                WorkingDirectory = ResultPath,
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        StrongReferenceMessenger.Default.Send(new ToasterViewModel(ToasterType.Info, ex.Message));
                    }
                }
            },
                (path) =>
                {
                    if (ResultPath != string.Empty && ResultPath is not null)
                    {
                        return true;
                    }
                    return false;
                });

            ShowHiddenFilesCommand = new RelayCommand<bool>((state) =>
                {
                    if (state)
                    {
                        HiddenFiles.ForEach(_ =>
                        {
                            DirectoryFiles.Add(_);
                        });
                    }
                    else
                    {
                        HiddenFiles.ForEach(_ =>
                        {
                            DirectoryFiles.Remove(_);
                        });
                    }
                },
                (state) =>
                {
                    return true;
                });

            AddBookMarkCommand = new RelayCommand<string>((url) =>
            {
                if (!BookMarks.Any(A => A.DisplayName == url) && url != string.Empty)
                {
                    using (StreamWriter sw = File.AppendText(BookMarksStoringPath))
                    {
                        sw.WriteLine(url);
                    }

                    BookMarks.Clear();
                    File.ReadAllLines(BookMarksStoringPath).ToList().ForEach(path =>
                    {
                        if (path != null && path != string.Empty)
                        {
                            DirectoryInfo directoryInfo = new DirectoryInfo(path);
                            BookMarks.Add(new BookMarksModel(directoryInfo.Name, directoryInfo.FullName));
                            StrongReferenceMessenger.Default.Send(new ToasterViewModel(ToasterType.Success, $"Bookmark added successfully"));
                        }
                    });
                }
            },
            (url) =>
            {
                if (ResultPath != null && ResultPath != string.Empty && !BookMarks.Any(A => A.ReferenceMember == url))
                {
                    return true;
                }

                return false;
            });

            DeleteBookMarkCommand = new RelayCommand<string>((removeurl) =>
            {
                if (removeurl != string.Empty && BookMarks.Any(_ => _.ReferenceMember == removeurl))
                {
                    List<string> bookMarks = new List<string>(File.ReadAllLines(BookMarksStoringPath));
                    string bookMark = bookMarks.FirstOrDefault(_ => _ == removeurl);
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
                                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                                BookMarks.Add(new BookMarksModel(directoryInfo.Name, directoryInfo.FullName));
                            }
                        });

                        StrongReferenceMessenger.Default.Send(new ToasterViewModel(ToasterType.Success, $"Bookmark deleted successfully"));
                    }
                }
            },
            (url) =>
            {
                return true;
            });

            CopyCommand = new RelayCommand<string>((url) =>
            {
                Clipboard.SetText(url);
                StrongReferenceMessenger.Default.Send(new ToasterViewModel(ToasterType.Success, $"Copied path : {url}"));
            },
            (url) =>
            {
                if (url != string.Empty)
                {
                    return true;
                }
                return false;
            });

            GitPullCommand = new RelayCommand<string>((path) =>
            {
                try
                {
                    if (path is not null && SelectedBranch is not null)
                    {
                        string selectedBranch = GetSelectedBranch(path);
                        ExecuteGitCommands(path, "git", $"pull origin {selectedBranch}");
                    }
                }
                catch (Exception ex)
                {
                    StrongReferenceMessenger.Default.Send(new ToasterViewModel(ToasterType.Error, ex.Message));
                }
            },
            (path) =>
            {
                return AreCommandsAllowed;
            });

            GitCheckoutCommand = new RelayCommand<string>((path) =>
            {
                if (path is not null)
                {
                    string selectedBranch = GetSelectedBranch(path);
                    ExecuteGitCommands(path, "git", $"checkout {selectedBranch}");
                }
            },
            (path) =>
            {
                return AreCommandsAllowed;
            });

            GitFetchCommand = new RelayCommand<string>((path) =>
            {
                if (path is not null)
                {
                    ExecuteGitCommands(path, "git", "fetch");
                }
            },
            (path) =>
            {
                return AreCommandsAllowed;
            });

            GitCleanCommand = new RelayCommand<string>((path) =>
            {
                if (path is not null)
                {
                    ExecuteGitCommands(path, "git", "clean -ffdx");
                }
            },
            (path) =>
            {
                return AreCommandsAllowed;
            });

            ShowMessageCommand = new RelayCommand<string>((message) =>
            {
                string msg = string.Empty;
                if (message != null && message != string.Empty)
                {
                    msg = $"{message} Command not implemeted";
                }
                else
                {
                    msg = "Command for this control not implemeted";
                }

                // ExecuteScript(message);
                StrongReferenceMessenger.Default.Send(new ToasterViewModel(ToasterType.Info, msg));
            },
            (message) =>
            {
                return true;
            });

            CleanBinFilesCommand = new RelayCommand<string>((binFile) =>
            {
                try
                {
                    string[] binFolders = Directory.GetDirectories(binFile, "bin", SearchOption.AllDirectories);

                    if (binFolders.Any())
                    {
                        foreach (string binFolder in binFolders)
                        {
                            Directory.Delete(binFolder, true);
                        }

                        OutputMessage = $"Successfully removed bin files in {System.IO.Path.GetFileName(binFile)}";
                        StrongReferenceMessenger.Default.Send(new ToasterViewModel(ToasterType.Success, OutputMessage));
                    }
                    else
                    {
                        OutputMessage = $"No bin folders are detected in {System.IO.Path.GetFileName(binFile)}";
                        StrongReferenceMessenger.Default.Send(new ToasterViewModel(ToasterType.Warning, OutputMessage));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            },
            (binFile) =>
            {
                return AreCommandsAllowed;
            });

            CleanObjFilesCommand = new RelayCommand<string>((objFile) =>
            {
                try
                {
                    string[] objFolders = Directory.GetDirectories(objFile, "obj", SearchOption.AllDirectories);

                    if (objFolders.Any())
                    {
                        foreach (string objFolder in objFolders)
                        {
                            Directory.Delete(objFolder, true);
                        }

                        OutputMessage = $"Successfully removed obj files in {System.IO.Path.GetFileName(objFile)}";
                        StrongReferenceMessenger.Default.Send(new ToasterViewModel(ToasterType.Success, OutputMessage));
                    }
                    else
                    {
                        OutputMessage = $"No bin folders obj detected in {System.IO.Path.GetFileName(objFile)}";
                        StrongReferenceMessenger.Default.Send(new ToasterViewModel(ToasterType.Warning, OutputMessage));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            },
            (objFile) =>
            {
                return AreCommandsAllowed;
            });

            OpenRunningAppCommand = new RelayCommand<string>((appName) =>
            {
                OpenRunningApplication(appName);
            },
            (appName) =>
            {
                return true;
            });

            ExecuteScriptFileCommand = new RelayCommand<DirectoriesModel>((script) =>
            {
                Process process = new Process();
                try
                {
                    ProcessStartInfo processStartInfo = new ProcessStartInfo()
                    {
                        FileName = "powershell.exe",
                        WorkingDirectory = ResultPath,
                        RedirectStandardOutput = false,
                        RedirectStandardError = false,
                        Arguments = $"-ExecutionPolicy Bypass -File \"{script.FilePath}\"",
                    };

                    process.StartInfo = processStartInfo;
                    process.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            },
            (script) =>
            {
                return true;
            });
        }
        private void LoadData(string path)
        {
            ResultPath = path;
            GetUpdatedNavItems(ResultPath);
            RefreshFiles(ResultPath);
        }
        private void ExecuteGitCommands(string path, string commandType, string argument)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                WorkingDirectory = path,
                FileName = commandType,
                Arguments = argument,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = new Process
            {
                StartInfo = processStartInfo
            };

            process.Start();
            OutputMessage = $"{argument} started";
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            SetMessage(process, output, error);
        }   
        private void SetMessage(Process prcs, string opt, string err)
        {
            if (prcs.ExitCode == 0)
            {
                OutputMessage = opt == string.Empty ? err : opt;
                StrongReferenceMessenger.Default.Send(new ToasterViewModel(ToasterType.Success, "Executed successfully."));
            }
            else
            {
                OutputMessage = err;
                StrongReferenceMessenger.Default.Send(new ToasterViewModel(ToasterType.Error, "Execution failed."));
            }
        }
        private void SetGitBranches(string repositoryPath)
        {
            ObservableCollection<TabControlModel<string>> allBranches = new();
            using (var repo = new Repository(repositoryPath))
            {
                var localBranches = new TabControlModel<string>() { UniqueName = "Local", Items = new List<string>(repo.Branches.Where(_ => !_.IsRemote).Select(_ => _.FriendlyName)) };
                var remoteBranches = new TabControlModel<string>() { UniqueName = "Remote", Items = new List<string>(repo.Branches.Where(_ => _.IsRemote).Select(_ => _.FriendlyName)) };
                allBranches.Add(localBranches);
                allBranches.Add(remoteBranches);
            }

            GitBranches = new();
            GitBranches = allBranches;
            RaisePropertyChanged(nameof(GitBranches));
        }
        private string GetCurrentGitBranch(string path, string commandType, string argument)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                WorkingDirectory = path,
                FileName = commandType,
                Arguments = argument,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = new Process()
            {
                StartInfo = processStartInfo
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output.Trim();
        }
        private string GetSelectedBranch(string path)
        {
            using (var repo = new Repository(path))
            {
                var remoteBranches = new TabControlModel<string>() { UniqueName = "Remote", Items = new List<string>(repo.Branches.Where(_ => _.IsRemote).Select(_ => _.FriendlyName)) };

                if (remoteBranches.Items.Any(_ => _ == SelectedBranch))
                {
                    SelectedBranch = SelectedBranch.Replace("origin/", string.Empty);
                }
            }

            return SelectedBranch;
        }
        private void GetCurrentRunningApps()
        {
            Process[] processes = Process.GetProcesses();
            List<Process> runningApps = new();

            foreach (var process in processes)
            {
                if (process.MainWindowTitle != string.Empty)
                {
                    runningApps.Add(process);
                }
            }
            if (runningApps.Any())
            {
                RunningApps = new ObservableCollection<Process>(runningApps);
                RaisePropertyChanged(nameof(RunningApps));
            }
        }
        public void OpenRunningApplication(string applicationName)
        {
            var process = RunningApps.FirstOrDefault(_ => _.ProcessName == applicationName);
            if (process != null)
            {
                IntPtr mainWindowHandle = process.MainWindowHandle;
                NativeMethods.ShowWindow(mainWindowHandle, NativeMethods.SW_RESTORE);
                NativeMethods.SetForegroundWindow(mainWindowHandle);
            }
        }
    }
}
