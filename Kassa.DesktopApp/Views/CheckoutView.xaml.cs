using Kassa.DesktopApp.ViewModels;
using Kassa.Domain.Entities;
using System.Windows.Controls;

namespace Kassa.DesktopApp.Views
{
    public partial class CheckoutView : UserControl
    {
        public CheckoutView()
        {
            InitializeComponent();
            DataContextChanged += (_, e) =>
            {
                if (e.OldValue is CheckoutViewModel oldVm) oldVm.SaleCompleted -= OnSaleCompleted;
                if (e.NewValue is CheckoutViewModel newVm) newVm.SaleCompleted += OnSaleCompleted;
            };
            Loaded += (_, _) => BarcodeBox.Focus();
        }

        private void BarcodeBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter && DataContext is CheckoutViewModel vm && vm.ScanBarcodeCommand.CanExecute(null))
            {
                vm.ScanBarcodeCommand.Execute(null);
            }
        }

        private void OnSaleCompleted(object? sender, Transaction transaction)
        {
            var receiptWindow = new ReceiptWindow(transaction);
            receiptWindow.Owner = System.Windows.Window.GetWindow(this);
            receiptWindow.ShowDialog();
            BarcodeBox.Focus();
        }
    }
}
