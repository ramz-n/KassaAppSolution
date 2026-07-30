using Kassa.Application.Cart;
using Kassa.Domain.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kassa.DesktopApp.ViewModels
{
    public class CartScannedItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ScannedItem Model { get; }
        private readonly Action _onChanged;

        public string ProductName => Model.ProductName;
        public string Barcode => Model.Barcode;
        public string UnitLabel => Model.UnitType == UnitType.Weight ? "kg" : "pcs";

        public decimal Quantity
        {
            get => Model.Quantity;
            set
            {
                if (value <= 0) return;
                Model.Quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(NetTotal));
                _onChanged();
            }
        }

        public decimal UnitPrice => Model.UnitPrice;

        public decimal DiscountAmount
        {
            get => Model.DiscountAmount;
            set
            {
                Model.DiscountAmount = Math.Max(0, Math.Min(value, Model.GrossTotal));
                OnPropertyChanged();
                OnPropertyChanged(nameof(NetTotal));
                _onChanged();
            }
        }

        public decimal NetTotal => Model.NetTotal;

        public CartScannedItemViewModel(ScannedItem model, Action onChanged)
        {
            Model = model;
            _onChanged = onChanged;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
