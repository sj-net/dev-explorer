namespace FileExplorer.Models
{
    using FileExplorer.ViewModels;
    
    public class DrivesModel : ObservableObject
    {
        private string driveName;
        private string drivePath;
        private bool isSelected;

        public string DriveName
        {
            get { return driveName; }
            set
            {
                driveName = value;
                RaisePropertyChanged(nameof(DriveName));
            }
        }
        public string DrivePath
        {
            get { return drivePath; }
            set
            {
                drivePath = value;
                RaisePropertyChanged(nameof(DrivePath));
            }
        }
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                RaisePropertyChanged(nameof(IsSelected));
            }
        }
    }
}
