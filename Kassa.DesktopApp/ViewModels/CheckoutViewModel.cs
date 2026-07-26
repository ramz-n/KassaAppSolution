using Kassa.Application.Interfaces;
using Kassa.DesktopApp.Common;
using Kassa.Domain.Entities;

namespace Kassa.DesktopApp.ViewModels
{
    public class CheckoutViewModel
    {
        private readonly IProductRepository _productRepository;
        public Cashier CurrentCashier { get; private set; } = null!;

        public RelayCommand LogoutCommand { get; }
        public RelayCommand OpenProductsCommand { get; }


        public event EventHandler? LogoutRequested;
        public event EventHandler? OpenProductsRequested;

        public CheckoutViewModel(IProductRepository productRepository)
        {
            _productRepository = productRepository;

            LogoutCommand = new RelayCommand(() => LogoutRequested?.Invoke(this, EventArgs.Empty));
            OpenProductsCommand = new RelayCommand(() => OpenProductsRequested?.Invoke(this, EventArgs.Empty));
        }

        public void Initialize(Cashier cashier)
        {
            CurrentCashier = cashier;
        }
    }
}
