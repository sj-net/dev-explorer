using FileExplorer.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models
{
    public class TabControlModel<T> : ObservableObject
    {
        private string uniqueName;

        public string UniqueName
        {
            get { return uniqueName; }
            set
            {
                uniqueName = value;
                RaisePropertyChanged(nameof(UniqueName));
            }
        }

        public List<T> Items { get; set; } = new();
    }
}

