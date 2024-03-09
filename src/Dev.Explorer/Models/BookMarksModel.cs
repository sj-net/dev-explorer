using FileExplorer.Interfaces;

namespace FileExplorer.Models
{
    public class BookMarksModel : IBookMark
    {
        public string DisplayName { get; set; }
        public string ReferenceMember { get; set; }
        public BookMarksModel(string displayName, string referenceMember)
        {
            DisplayName = displayName;
            ReferenceMember = referenceMember;
        }
    }
}
