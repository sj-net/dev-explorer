using FileExplorer.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace FileExplorer
{
    public class Program
    {
        internal static SplashScreen SplashScreen { get; private set; }

        [STAThread]
        public static void Main(string[] args)
        {
            IServiceProvider provider;
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<WindowViewModel>();
            services.AddSingleton<FileExplorerViewModel>();
            services.AddSingleton<SystemVariableViewModel>();
            services.AddSingleton<RemoteDesktopViewModel>();
            services.AddSingleton<DocumentsViewModel>();

            provider = services.BuildServiceProvider();

            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
