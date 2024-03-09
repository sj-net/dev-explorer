namespace FileExplorer.ViewModels
{
    using CommunityToolkit.Mvvm.Messaging;
    using FileExplorer.Commands;
    using FileExplorer.Interfaces;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using FileExplorer.Utilities;

    public class WindowViewModel : ObservableObject
    {
        private WindowState currentWindowState;

        public ObservableCollection<IPageViewModel> Pages { get; set; } = new();
        public ObservableCollection<ToasterViewModel> Toasters { get; set; } = new();

        public WindowState CurrentWindowState
        {
            get
            {
                return currentWindowState;
            }
            set
            {
                currentWindowState = value;
                RaisePropertyChanged(nameof(CurrentWindowState));
            }
        }

        public RelayCommand<object> MinimizeWindowCommand { get; set; }
        public RelayCommand<object> WindowStateCommand { get; set; }
        public RelayCommand<object> WindowCloseCommand { get; set; }
        public RelayCommand<string> CloseToasterCommand { get; set; }

        public WindowViewModel()
        {
            CurrentWindowState = WindowState.Maximized;
            Pages.Add(Ioc.Default.GetService<FileExplorerViewModel>());
            Pages.Add(Ioc.Default.GetService<SystemVariableViewModel>());
            Pages.Add(Ioc.Default.GetService<RemoteDesktopViewModel>());
            Pages.Add(Ioc.Default.GetService<DocumentsViewModel>());

            StrongReferenceMessenger.Default.Register<ToasterViewModel>(this, (r, msg) =>
            {
                if (Toasters.LastOrDefault()?.Message != msg.Message)
                {
                    Toasters.Add(msg);
                    Task.Run(() => Task.Delay(msg.Timer).ContinueWith(_ =>
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
                        {
                            Toasters.Remove(msg);
                        });
                    }));
                }
            });

            MinimizeWindowCommand = new RelayCommand<object>((e) =>
            {
                if (e is not null && e is Window win)
                {
                    win.WindowState = WindowState.Minimized;
                }
            },
            (e) =>
            {
                return true;
            });

            WindowStateCommand = new RelayCommand<object>((e) =>
            {
                if (e is not null && e is Window win)
                {
                    win.WindowState = win.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                }
            },
            (e) =>
            {
                return true;
            });

            WindowCloseCommand = new RelayCommand<object>((e) =>
            {
                if (e is not null && e is Window win)
                {
                    win.Close();
                }
            },
            (e) =>
            {
                return true;
            });

            CloseToasterCommand = new RelayCommand<string>((toastmsg) =>
            {
                ToasterViewModel toaster = Toasters.FirstOrDefault(_ => _.Message == toastmsg);
                Toasters.Remove(toaster);
            },
            (toastmsg) =>
            {
                return true;
            });

        }
    }
}
