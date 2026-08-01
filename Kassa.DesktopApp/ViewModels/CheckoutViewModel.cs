using Kassa.Application.Cart;
using Kassa.Application.Interfaces;
using Kassa.Application.Services;
using Kassa.DesktopApp.Common;
using Kassa.DesktopApp.Services;
using Kassa.Domain.Entities;
using Kassa.Domain.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace Kassa.DesktopApp.ViewModels
{
    public class CheckoutViewModel : INotifyPropertyChanged
    {
        private readonly IProductRepository _productRepository;
        private readonly IKassaSessionRepository _kassSessionRepository;
        private readonly ICartService _cart;
        private readonly ICheckoutService _checkoutService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IEsewaPaymentService _esewaPaymentService;
        public Cashier CurrentCashier { get; private set; } = null!;
        public KassaSession? OpenSession { get; private set; }

        public RelayCommand LogoutCommand { get; }
        public RelayCommandAsync ScanBarcodeCommand { get; }
        public RelayCommand OpenProductsCommand { get; }
        public RelayCommandAsync OpenKassaCommand { get; }
        public RelayCommandAsync CloseKassaCommand { get; }
        public RelayCommand RemoveScannedItemCommand { get; }
        public RelayCommand ClearCartCommand { get; }
        public RelayCommand StartPaymentCommand { get; }
        public RelayCommand CancelPaymentCommand { get; }
        public RelayCommandAsync CompleteSaleCommand { get; }


        public event EventHandler? LogoutRequested;
        public event EventHandler? OpenProductsRequested;
        public event EventHandler<Transaction>? SaleCompleted;

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

        private bool _isPaymentPanelOpen;
        public bool IsPaymentPanelOpen { 
            get => _isPaymentPanelOpen;
            set
            {
                if(_isPaymentPanelOpen != value)
                {
                    _isPaymentPanelOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        private PaymentMethod _selectedPaymentMethod = PaymentMethod.Cash;
        public PaymentMethod SelectedPaymentMethod { 
            get => _selectedPaymentMethod;
            set
            {
                if (value != _selectedPaymentMethod)
                {
                    _selectedPaymentMethod = value;
                    OnPropertyChanged();
                }
                if(value == PaymentMethod.Esewa)
                {
                    RefreshEsewaQr();
                } else
                {
                    EsewaQrcode = null;
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
        private string _amountReceived = string.Empty;
        public string AmountReceived { 
            get => _amountReceived; 
            set {
                if (_amountReceived != value)
                {
                    _amountReceived = value;
                    OnPropertyChanged();
                }
            }
        }
        private BitmapImage? _esewaQrcode;
        public BitmapImage? EsewaQrcode
        {
            get => _esewaQrcode;
            private set
            {
                if (_esewaQrcode != value)
                {
                    _esewaQrcode = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ChangeDuePreview
        {
            get
            {
                if (decimal.TryParse(AmountReceived, out var received))
                {
                    var change = received - GrandTotal;
                    return change >= 0 ? $"Change: {change:C}" : "Insufficient amount";
                }
                return string.Empty;
            }
        }

        public void Initialize(Cashier cashier)
        {
            CurrentCashier = cashier;
            _cart.Clear();
            RefreshCart();
            _ = LoadOpenSessionAsync();
        }

        private const string EsewaMerchantCode = "EPAYTEST";
        public CheckoutViewModel(IProductRepository productRepository, IKassaSessionRepository kassaSessionRepository, ICartService cartService, ICheckoutService checkoutService, 
            ITransactionRepository transactionRepository, IEsewaPaymentService esewaPaymentService)
        {
            _productRepository = productRepository;
            _kassSessionRepository = kassaSessionRepository;
            _cart = cartService;
            _checkoutService = checkoutService;
            _transactionRepository = transactionRepository;
            _esewaPaymentService = esewaPaymentService;

            ScanBarcodeCommand = new RelayCommandAsync(ScanBarcodeAsync);
            ClearCartCommand = new RelayCommand(() => { _cart.Clear(); RefreshCart(); });
            StartPaymentCommand = new RelayCommand(
                () => { IsPaymentPanelOpen = true; if (SelectedPaymentMethod == PaymentMethod.Esewa) RefreshEsewaQr(); },
                () => CartScannedItems.Count > 0);
            CancelPaymentCommand = new RelayCommand(ResetPaymentPanel);
            CompleteSaleCommand = new RelayCommandAsync(CompleteSaleAsync);
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

            var cashSales = await _transactionRepository.SumCashSalesAsync(OpenSession.OpenedAt, DateTime.UtcNow, CurrentCashier.Id);
            OpenSession.ExpectedCash = OpenSession.StartingCash + cashSales;
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
            RefreshCart() ;
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
            StartPaymentCommand.RaiseCanExecuteChanged();
        }

        private void RecalculateTotals()
        {
            var summary = _cart.GetSummary();
            Subtotal = summary.Subtotal;
            TaxTotal = summary.TaxTotal;
            GrandTotal = summary.GrandTotal;
            OnPropertyChanged(nameof(ChangeDuePreview));
            StartPaymentCommand.RaiseCanExecuteChanged();

            if (IsPaymentPanelOpen && SelectedPaymentMethod == PaymentMethod.Esewa)
            {
                RefreshEsewaQr();
            }
        }

        private void RefreshEsewaQr()
        {
            if (GrandTotal <= 0)
            {
                EsewaQrcode = null;
                return;
            }

            var previewReference = $"{CurrentCashier.Id}-{DateTime.Now:yyyyMMddHHmmss}";
            var payload = _esewaPaymentService.BuildPaymentPayload(EsewaMerchantCode, GrandTotal,  previewReference);
            EsewaQrcode = QrCodeImageBuilder.GeneratePng(payload);
        }

        private void ResetPaymentPanel()
        {
            IsPaymentPanelOpen = false;
            AmountReceived = string.Empty;
            SelectedPaymentMethod = PaymentMethod.Cash;
        }

        private async Task CompleteSaleAsync()
        {
            decimal? amountTendered = null;
            if (SelectedPaymentMethod == PaymentMethod.Cash)
            {
                if (!decimal.TryParse(AmountReceived, out var received) || received < GrandTotal)
                {
                    StatusMessage = "Enter an amount received that covers the total.";
                    IsError = true;
                    return;
                }
                amountTendered = received;
            }

            var result = await _checkoutService.CompleteSaleAsync(_cart, CurrentCashier.Id, SelectedPaymentMethod, amountTendered);

            if (!result.Success)
            {
                StatusMessage = result.ErrorMessage;
                IsError = true;
                return;
            }

            StatusMessage = $"Sale completed. Receipt {result.Transaction!.ReceiptNumber}.";
            IsError = false;
            ResetPaymentPanel();
            RefreshCart();
            SaleCompleted?.Invoke(this, result.Transaction);
        }
    }
}
