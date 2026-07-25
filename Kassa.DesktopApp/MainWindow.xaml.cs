using Kassa.DesktopApp.ViewModels;
using Kassa.DesktopApp.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Kassa.DesktopApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowLoginScreen();
        }

        public void ShowLoginScreen()
        {
            var vm = App.AppHost.Services.GetRequiredService<LoginViewModel>();
            var view = new LoginView { DataContext = vm };
            
            MainContent.Content = view;
        }
    }
}
