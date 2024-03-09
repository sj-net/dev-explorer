namespace FileExplorer.ViewModels
{
    using FileExplorer.Interfaces;

    public class BasePageViewModel : ObservableObject, IPageViewModel
    {
        public string UniqueName { get; set; }
        public IPageViewModel ViewModel { get; set; }

        public BasePageViewModel()
        {
            ViewModel = this;
        }
    }
}
