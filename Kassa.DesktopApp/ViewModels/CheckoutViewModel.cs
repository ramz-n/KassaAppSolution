using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Kassa.Application.Interfaces;
using Kassa.DesktopApp.Common;
using Kassa.Domain.Entities;

namespace Kassa.DesktopApp.ViewModels
{
    public class CheckoutViewModel : INotifyPropertyChanged
    {
        private readonly IProductRepository _productRepository;
        private readonly IKassaSessionRepository _kassSessionRepository;

        public Cashier CurrentCashier { get; private set; } = null!;
        public KassaSession? OpenSession { get; private set; }

        public RelayCommand LogoutCommand { get; }
        public RelayCommand OpenProductsCommand { get; }
        public RelayCommandAsync OpenKassaCommand { get; }
        public RelayCommandAsync CloseKassaCommand { get; }

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

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Initialize(Cashier cashier)
        {
            CurrentCashier = cashier;
            _ = LoadOpenSessionAsync();
        }

        public CheckoutViewModel(IProductRepository productRepository, IKassaSessionRepository kassaSessionRepository)
        {
            _productRepository = productRepository;
            _kassSessionRepository = kassaSessionRepository;

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
                OpenedAt = DateTime.Now,
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
            OpenSession.ClosedAt = DateTime.Now;
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
        }
    }
}
