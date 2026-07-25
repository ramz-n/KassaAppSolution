using Kassa.Application.Interfaces;
using Kassa.DesktopApp.Common;
using Kassa.Domain.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Kassa.DesktopApp.ViewModels
{
    public class LoginViewModel: INotifyPropertyChanged
    {
        private readonly ICashierRepository _cashierRepository;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Cashier> Cashiers { get; } = new();

        private Cashier? _selectedCashier;
        private string _pin = string.Empty;

        private string? _errorMessage;

        public Cashier? SelectedCashier
        {
            get => _selectedCashier;
            set 
            { 
                if(_selectedCashier != value) 
                {
                    _selectedCashier = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PinCode
        {
            get => _pin;
            set
            {
                if(_pin != value)
                {
                    _pin = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if(_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }
        public ICommand LoginCommand { get; }

        public event EventHandler<Cashier>? LoginSucceeded;

        public LoginViewModel(ICashierRepository cashierRepository)
        {
            _cashierRepository = cashierRepository;
            LoginCommand = new RelayCommandAsync(LoginAsync);
            _ = LoadCashiersAsync();
        }

        private async Task LoadCashiersAsync()
        {
            var cashiers = await _cashierRepository.GetActiveAsync();
            Cashiers.Clear();
            foreach (var cashier in cashiers)
            {
                Cashiers.Add(cashier);
            }
            SelectedCashier = Cashiers.FirstOrDefault();
        }

        private async Task LoginAsync()
        {
            if(SelectedCashier is null)
            {
                ErrorMessage = "Please select your name.";
                return;
            }

            var cashier = await _cashierRepository.GetByIdAndPincodeAsync(SelectedCashier.Id, PinCode);
            if(cashier is null)
            {
                ErrorMessage = "Invalid PIN code. Please try again.";
                PinCode = string.Empty;
                OnPropertyChanged(nameof(PinCode));
                return;
            }
            PinCode = string.Empty;
            LoginSucceeded?.Invoke(this, cashier);
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
