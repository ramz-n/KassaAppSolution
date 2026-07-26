using Kassa.DesktopApp.ViewModels;
using Kassa.DesktopApp.Views;
using Kassa.Domain.Entities;
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
            vm.LoginSucceeded += (_, cashier) => ShowCheckoutScreen(cashier);
            MainContent.Content = view;
        }

        public void ShowCheckoutScreen(Cashier cashier)
        {
            var vm = App.AppHost.Services.GetRequiredService<CheckoutViewModel>();
            vm.Initialize(cashier);
            var view = new CheckoutView { DataContext = vm };
            vm.LogoutRequested += (_, _) => ShowLoginScreen();
            vm.OpenProductsRequested += (_, _) => ShowProductsScreen(cashier);

            MainContent.Content = view;
        }

        public void ShowProductsScreen(Cashier cashier)
        {
            var vm = App.AppHost.Services.GetRequiredService<ProductViewModel>();
            var view = new ProductView { DataContext = vm };
            vm.BackRequested += (_, _) => ShowCheckoutScreen(cashier);
            MainContent.Content = view;
            _ = vm.LoadAsync();
        }
    }
}
