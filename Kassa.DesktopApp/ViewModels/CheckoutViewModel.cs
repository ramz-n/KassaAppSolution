using Kassa.Application.Cart;
using Kassa.Application.Interfaces;
using Kassa.DesktopApp.Common;
using Kassa.Domain.Entities;
using Kassa.Domain.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kassa.DesktopApp.ViewModels
{
    public class CheckoutViewModel : INotifyPropertyChanged
    {
        private readonly IProductRepository _productRepository;
        private readonly IKassaSessionRepository _kassSessionRepository;
        private readonly ICartService _cart;
        public Cashier CurrentCashier { get; private set; } = null!;
        public KassaSession? OpenSession { get; private set; }

        public RelayCommand LogoutCommand { get; }
        public RelayCommandAsync ScanBarcodeCommand { get; }
        public RelayCommand OpenProductsCommand { get; }
        public RelayCommandAsync OpenKassaCommand { get; }
        public RelayCommandAsync CloseKassaCommand { get; }
        public RelayCommand RemoveScannedItemCommand { get; }

        public event EventHandler? LogoutRequested;
        public event EventHandler? OpenProductsRequested;

        private string? _statusMessage;
        public string? StatusMessage
        {
            get => _statusMessage;
            private set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isError;
        public bool IsError
        {
            get => _isError;
            private set
            {
                if (_isError != value)
                {
                    _isError = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _barcodeInput = string.Empty;
        public string BarcodeInput
        {
            get => _barcodeInput;
            set
            {
                if (_barcodeInput != value)
                {
                    _barcodeInput = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<CartScannedItemViewModel> CartScannedItems { get; } = new();

        private decimal _subtotal;
        private decimal _taxTotal;
        private decimal _grandTotal;
        public decimal Subtotal { 
            get => _subtotal;
            private set 
            {
                if (_subtotal != value) {
                    _subtotal = value;
                    OnPropertyChanged();
                }
            } 
        }
        public decimal TaxTotal {
            get => _taxTotal;
            private set
            {
                if (_taxTotal != value)
                {
                    _taxTotal = value;
                    OnPropertyChanged();
                }
            }
        }
        public decimal GrandTotal {
            get => _grandTotal;
            private set
            {
                if (_grandTotal != value)
                {
                    _grandTotal  = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _amountTenderedInput = string.Empty;
        public string AmountTenderedInput { 
            get => _amountTenderedInput; 
            set {
                if (_amountTenderedInput != value)
                {
                    _amountTenderedInput = value;
                    OnPropertyChanged();
                }
            }
        }


        public string ChangeDuePreview
        {
            get
            {
                if (decimal.TryParse(AmountTenderedInput, out var tendered))
                {
                    var change = tendered - GrandTotal;
                    return change >= 0 ? $"Change: {change:C}" : "Insufficient amount";
                }
                return string.Empty;
            }
        }

        public void Initialize(Cashier cashier)
        {
            CurrentCashier = cashier;
            _ = LoadOpenSessionAsync();
        }

        public CheckoutViewModel(IProductRepository productRepository, IKassaSessionRepository kassaSessionRepository, ICartService cartService)
        {
            _productRepository = productRepository;
            _kassSessionRepository = kassaSessionRepository;
            _cart = cartService;

            ScanBarcodeCommand = new RelayCommandAsync(ScanBarcodeAsync);
            RemoveScannedItemCommand = new RelayCommand(p => RemoveScannedItem(p as CartScannedItemViewModel));
            LogoutCommand = new RelayCommand(() => LogoutRequested?.Invoke(this, EventArgs.Empty));
            OpenProductsCommand = new RelayCommand(() => OpenProductsRequested?.Invoke(this, EventArgs.Empty));
            OpenKassaCommand = new RelayCommandAsync(OpenKassaAsync);
            CloseKassaCommand = new RelayCommandAsync(CloseKassaAsync);
        }

        private async Task LoadOpenSessionAsync()
        {
            OpenSession = await _kassSessionRepository.GetOpenSessionAsync(CurrentCashier.Id);
            OnPropertyChanged(nameof(OpenSession));
        }

        private async Task OpenKassaAsync()
        {
            if (OpenSession != null) return;

            OpenSession = new KassaSession
            {
                CashierId = CurrentCashier.Id,
                OpenedAt = DateTime.UtcNow,
                StartingCash = 1000.00m,
                IsClosed = false
            };

            await _kassSessionRepository.AddAsync(OpenSession);
            OnPropertyChanged(nameof(OpenSession));
            StatusMessage = "Kassa opened with a starting cash of 1000.00";
            IsError = false;
        }

        private async Task CloseKassaAsync()
        {
            if (OpenSession is null) return;

            OpenSession.ExpectedCash = OpenSession.StartingCash + 1000.00m;
            OpenSession.CountedCash = OpenSession.ExpectedCash;
            OpenSession.CashDifference = OpenSession.CountedCash - OpenSession.ExpectedCash;
            OpenSession.ClosedAt = DateTime.UtcNow;
            OpenSession.IsClosed = true;

            await _kassSessionRepository.UpdateAsync(OpenSession);
            StatusMessage = $"Kassa closed. Expected Cash: Expected cash: {OpenSession.ExpectedCash:C}.";
            IsError = false;
            OpenSession = null;
            OnPropertyChanged(nameof(OpenSession));
        }

        private async Task ScanBarcodeAsync()
        {
            var barcode = BarcodeInput.Trim();
            BarcodeInput = string.Empty;
            if (string.IsNullOrEmpty(barcode)) return;

            var product = await _productRepository.GetProductByBarcodeAsync(barcode);
            if (product is null)
            {
                var matches = await _productRepository.SearchProductByNameAsync(barcode);
                product = matches.FirstOrDefault();
            }
            if (product is null)
            {
                StatusMessage = $"Product with barcode or name '{barcode}' not found.";
                IsError = true;
                return;
            }
            if (product.StockQty <= 0)
            {
                StatusMessage = $"Product '{product.ProductName}' is out of stock.";
                IsError = true;
                return;
            }

            // Implement add to cart later 
            var qty = product.UnitType == UnitType.Weight ? 0.5m : 1m;
            _cart.AddProduct(product, qty);
            RefreshCart();
            StatusMessage = $"Added: {product.ProductName}";
            IsError = false;
        }

        private void RemoveScannedItem(CartScannedItemViewModel? scannedProduct)
        {
            if (scannedProduct is null) return;
            _cart.RemoveScannedItem(scannedProduct.Model);
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RefreshCart()
        {
            CartScannedItems.Clear();
            foreach (var line in _cart.ScannedItems)
                CartScannedItems.Add(new CartScannedItemViewModel(line, RecalculateTotals));

            RecalculateTotals();

            //Add payment functionality later
        }

        private void RecalculateTotals()
        {
            var summary = _cart.GetSummary();
            Subtotal = summary.Subtotal;
            TaxTotal = summary.TaxTotal;
            GrandTotal = summary.GrandTotal;
            OnPropertyChanged(nameof(ChangeDuePreview));
        }
    }
}
