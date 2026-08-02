using Kassa.Application.Interfaces;
using Kassa.DesktopApp.Common;
using Kassa.Domain.Entities;
using Kassa.Domain.Enums;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kassa.DesktopApp.ViewModels
{
    public class SalesOverviewViewModel : INotifyPropertyChanged
    {
        public record TopProductRow(string ProductName, decimal QuantitySold, decimal Revenue);

        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly ITransactionRepository _transactionRepository;

        public ObservableCollection<Transaction> Transactions { get; } = new();

        private DateTime _fromDate = DateTime.Today;
        public DateTime FromDate { 
            get => _fromDate;
            set
            {
                if(_fromDate != value)
                {
                    _fromDate = value;
                    OnPropertyChanged(nameof(FromDate));
                }
            }
        }

        private DateTime _toDate = DateTime.Today.AddDays(1).AddSeconds(-1);
        public DateTime ToDate {
            get => _toDate;
            set
            {
                if (_toDate != value)
                {
                    _toDate = value;
                    OnPropertyChanged(nameof(ToDate));
                }
            }
        }

        private decimal _totalRevenue, _totalTax;
        private int _transactionCount;
        private decimal _cashRevenue, _esewaRevenue;

        public decimal TotalRevenue { 
            get => _totalRevenue; 
            set
            {
                if (_totalRevenue != value)
                {
                    _totalRevenue = value;
                    OnPropertyChanged(nameof(TotalRevenue));
                }
            }
        }
        public decimal TotalTax { 
            get => _totalTax; 
            set
            {
                if (_totalTax != value)
                {
                    _totalTax = value;
                    OnPropertyChanged(nameof(TotalTax));
                }
            }
        }
        public int TransactionCount { 
            get => _transactionCount; 
            set
            {
                if (_transactionCount != value)
                {
                    _transactionCount = value;
                    OnPropertyChanged(nameof(TransactionCount));
                }
            }
        }
        public decimal CashRevenue { 
            get => _cashRevenue; 
            private set
            {
                if (_cashRevenue != value)
                {
                    _cashRevenue = value;
                    OnPropertyChanged(nameof(CashRevenue));
                }
            }
        }
        public decimal EsewaRevenue { 
            get => _esewaRevenue; 
            private set
            {
                if (_esewaRevenue != value)
                {
                    _esewaRevenue = value;
                    OnPropertyChanged(nameof(EsewaRevenue));
                }
            }
        }

        public ObservableCollection<TopProductRow> TopProducts { get; } = new();

        public RelayCommandAsync RefreshCommand { get; }
        public RelayCommand BackCommand { get; }

        public event EventHandler? BackRequested;
        public SalesOverviewViewModel(ITransactionRepository transactionRepository) 
        {
            _transactionRepository = transactionRepository;
            RefreshCommand = new RelayCommandAsync(LoadAsync);
            BackCommand = new RelayCommand(() => BackRequested?.Invoke(this, EventArgs.Empty));
        }

        public async Task LoadAsync()
        {
            var transactions = await _transactionRepository.GetByDateRangeAsync(FromDate, ToDate);

            Transactions.Clear();
            foreach (var t in transactions) Transactions.Add(t);

            TotalRevenue = transactions.Sum(t => t.Total);
            TotalTax = transactions.Sum(t => t.TaxTotal);
            TransactionCount = transactions.Count;
            CashRevenue = transactions.Where(t => t.PaymentMethod == PaymentMethod.Cash).Sum(t => t.Total);
            EsewaRevenue = transactions.Where(t => t.PaymentMethod == PaymentMethod.Esewa).Sum(t => t.Total);

            var topProducts = transactions
                .SelectMany(t => t.Lines)
                .GroupBy(l => l.ProductName)
                .Select(g => new TopProductRow(g.Key, g.Sum(l => l.Quantity), g.Sum(l => l.LineTotal)))
                .OrderByDescending(r => r.Revenue)
                .Take(5)
                .ToList();

            TopProducts.Clear();
            foreach (var row in topProducts) TopProducts.Add(row);

            OnPropertyChanged(nameof(TotalRevenue));
            OnPropertyChanged(nameof(TotalTax));
            OnPropertyChanged(nameof(TransactionCount));
            OnPropertyChanged(nameof(CashRevenue));
            OnPropertyChanged(nameof(EsewaRevenue));
            OnPropertyChanged(nameof(TopProducts));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
