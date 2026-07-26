using Kassa.DesktopApp.ViewModels;
using System.Windows.Controls;

namespace Kassa.DesktopApp.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void PinBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.PinCode = PinBox.Password;
            }
        }
    }
}
