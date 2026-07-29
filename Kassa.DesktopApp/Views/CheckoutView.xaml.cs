using Kassa.DesktopApp.ViewModels;
using System.Windows.Controls;

namespace Kassa.DesktopApp.Views
{
    public partial class CheckoutView : UserControl
    {
        public CheckoutView()
        {
            InitializeComponent();
        }

        private void BarcodeBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter && DataContext is CheckoutViewModel vm && vm.ScanBarcodeCommand.CanExecute(null))
            {
                vm.ScanBarcodeCommand.Execute(null);
            }
        }
    }
}
