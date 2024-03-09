namespace FileExplorer.Models
{
    using FileExplorer.ViewModels;
    using System;

    public class DirectoriesModel : ObservableObject
    {
        private string path;
        private string fileName;
        private bool isLastFile;
        private bool isFile;
        private DateTime creationTime;
        private double fileSize;

        public string FilePath
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
                RaisePropertyChanged(nameof(FilePath));
            }
        }
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
                RaisePropertyChanged(nameof(FileName));
            }
        }
        public bool IsLastFile
        {
            get
            {
                return isLastFile;
            }
            set
            {
                isLastFile = value;
                RaisePropertyChanged(nameof(IsLastFile));
            }
        }
        public bool IsFile
        {
            get
            {
                return isFile;
            }
            set
            {
                isFile = value;
                RaisePropertyChanged(nameof(IsFile));
            }
        }
        public DateTime CreationTime
        {
            get
            {
                return creationTime;

            }
            set
            {
                creationTime = value;
                RaisePropertyChanged(nameof(CreationTime));
            }
        }

        public string FormattedCreationTime
        {
            get
            {
                return CreationTime.ToString("dd-MM-yyyy HH:mm:ss");
            }
        }

        public double FileSize
        {
            get
            {
                return fileSize;
            }
            set
            {
                fileSize = value;
                RaisePropertyChanged(nameof(FileSize));
            }
        }

        public string FormattedFileSize
        {
            get
            {
                var size = Math.Round(FileSize, 2);

                if (size > 1024 * 1024 * 1024)
                {
                    return $"{Math.Round(size / (1024 * 1024 * 1024), 2)} GB";
                }
                else if (size > 1024 * 1024)
                {
                    return $"{Math.Round(size / (1024 * 1024), 2)} MB";
                }
                else if (size > 1024)
                {
                    return $"{Math.Round(size / 1024, 2)} KB";
                }

                return $"{Math.Round(size, 2)} Bytes";
            }
        }
    }
}
