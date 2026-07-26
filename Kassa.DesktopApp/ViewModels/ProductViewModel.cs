using Kassa.Application.Interfaces;
using Kassa.DesktopApp.Common;
using Kassa.Domain.Entities;
using System.Collections.ObjectModel;

namespace Kassa.DesktopApp.ViewModels
{
    public class ProductViewModel
    {
        private readonly IProductRepository _productRepository;

        private string _editBarcode = string.Empty;
        private string _editName = string.Empty;
        private string _editPrice = "0.00";
        private string _editTax = "0.13";
        private string _editStock = "0";
        private string? _errorMessage;

        public ObservableCollection<Product> Products { get; } = new();
        public ObservableCollection<Product> LowStockProducts { get; } = new();

        private Product? _selectedProduct;
        public RelayCommand NewProductCommand { get; }
        public RelayCommand BackCommand { get; }


        public event EventHandler? BackRequested;

        public Product? SelectedProduct
        {
            get
            {
                return _selectedProduct;
            }
            set
            {
                if (_selectedProduct != value)
                {
                    _selectedProduct = value;
                    LoadEditFields(value);
                }
            }
        }

        public string EditBarcode {
            get => _editBarcode;
            set 
            {
                if (_editBarcode != value)
                {
                    _editBarcode = value;
                }
            }
        }

        public string EditName { 
            get => _editName; 
            set 
            {
                if (_editName != value) 
                { 
                    _editName = value;
                }
            } 
        }

        public string EditPrice
        {
            get => _editPrice;
            set
            {
                if (_editPrice != value)
                {
                    _editPrice = value;
                }
            }
        }

        public string EditTax { 
            get => _editTax;
            set
            {
                if(_editPrice != value)
                {
                    _editPrice= value;
                }
            }
        }

        public string EditStock { 
            get => _editStock;
            set
            {
                if (_editStock != value)
                {
                    _editStock = value;
                }
            }
        }

        public string? ErrorMessage { 
            get => _errorMessage;
            set
            { 
                if(_errorMessage != value)
                {
                    _errorMessage = value;
                }  
            }
        }


        public ProductViewModel(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            NewProductCommand = new RelayCommand(() => SelectedProduct = null);
            BackCommand = new RelayCommand(() => BackRequested?.Invoke(this, EventArgs.Empty));
        }

        
        public async Task LoadAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            Products.Clear();

            foreach (var product in products) 
            {
                Products.Add(product);
            }

            var lowStock = await _productRepository.GetLowStockProductsAsync();
            LowStockProducts.Clear();

            foreach(var product in lowStock)
            {
                LowStockProducts.Add(product);
            }
        }

        private void LoadEditFields(Product? p)
        {
            ErrorMessage = null;
            EditBarcode = p?.Barcode ?? string.Empty;
            EditName = p?.ProductName ?? string.Empty;
            EditPrice = (p?.Price ?? 0m).ToString("0.00");
            EditTax = (p?.Tax ?? 0.21m).ToString("0.00");
            EditStock = (p?.StockQty ?? 0m).ToString("0.###");
        }
    }
}
