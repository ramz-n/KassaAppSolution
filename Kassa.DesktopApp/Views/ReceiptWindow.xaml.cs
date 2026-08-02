using Kassa.Domain.Entities;
using Kassa.Domain.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Kassa.DesktopApp.Views
{
    public partial class ReceiptWindow : Window
    {
        public ReceiptWindow(Transaction transaction)
        {
            InitializeComponent();
            RenderReceipt(transaction);
        }

        private void RenderReceipt(Transaction t)
        {
            void AddLine(string text, bool bold = false, double size = 13)
            {
                ReceiptPanel.Children.Add(new TextBlock
                {
                    Text = text,
                    FontWeight = bold ? FontWeights.Bold : FontWeights.Normal,
                    FontSize = size,
                    Margin = new Thickness(0, 1, 0, 1),
                    Foreground = Brushes.Black
                });
            }

            AddLine("QRCoders - KASSA", true, 18);
            AddLine("Bharatpur-10, Chitwan");
            AddLine("--------------------------------");
            AddLine($"Receipt: {t.ReceiptNumber}");
            AddLine($"Date: {t.Timestamp:yyyy-MM-dd HH:mm}");
            AddLine($"Cashier: {t.Cashier?.Name ?? t.CashierId.ToString()}");
            AddLine("--------------------------------");

            foreach (var line in t.Lines)
            {
                var qtyLabel = line.Quantity % 1 == 0 ? line.Quantity.ToString("0") : line.Quantity.ToString("0.###");
                AddLine($"{line.ProductName}");
                AddLine($"  {qtyLabel} x {String.Format("{0,10:'Rs.'0.00}", line.UnitPrice)}  = {String.Format("{0,10:'Rs.'0.00}", line.LineTotal)}" +
                        (line.DiscountAmount > 0 ? $"  (-{String.Format("{0,10:'Rs.'0.00}", line.DiscountAmount)})" : ""));
            }

            AddLine("--------------------------------");
            AddLine($"Subtotal:      {String.Format("{0,10:'Rs.'0.00}", t.Subtotal)}");
            AddLine($"Tax:           {String.Format("{0,10:'Rs.'0.00}", t.TaxTotal)}");
            AddLine($"TOTAL:         {String.Format("{0,10:'Rs.'0.00}", t.Total)}", true, 16);
            AddLine("--------------------------------");
            AddLine($"Payment: {t.PaymentMethod}");
            if (t.PaymentMethod == PaymentMethod.Cash)
            {
                AddLine($"Received:      {String.Format("{0,10:'Rs.'0.00}", t.AmountTendered)}");
                AddLine($"Change:        {String.Format("{0,10:'Rs.'0.00}", t.ChangeGiven)}");
            }
            AddLine("--------------------------------");
            AddLine("Thank you for shopping with us!", false, 12);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
