namespace FileExplorer.Interfaces
{
    public interface IPageViewModel
    {
        public string UniqueName { get; set; }
        public IPageViewModel ViewModel { get; set; }
    }
}
