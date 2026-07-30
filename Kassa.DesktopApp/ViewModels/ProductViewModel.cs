using Kassa.Application.Interfaces;
using Kassa.DesktopApp.Common;
using Kassa.Domain.Entities;
using Kassa.Domain.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kassa.DesktopApp.ViewModels
{
    public class ProductViewModel: INotifyPropertyChanged
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
        public RelayCommandAsync SaveCommand { get; }


        public event EventHandler? BackRequested;
        public event PropertyChangedEventHandler? PropertyChanged;

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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        public UnitType[] UnitTypes { get; } = Enum.GetValues<UnitType>();
        private UnitType _editUnitType = UnitType.Piece;
        public UnitType EditUnitType { 
            get => _editUnitType;
            set
            {
                if (_editUnitType != value)
                {
                    _editUnitType = value;
                    OnPropertyChanged();
                }
            }
        }

        public ProductViewModel(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            NewProductCommand = new RelayCommand(() => SelectedProduct = null);
            BackCommand = new RelayCommand(() => BackRequested?.Invoke(this, EventArgs.Empty));
            SaveCommand = new RelayCommandAsync(SaveProductAsync);
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
            EditTax = (p?.Tax ?? 0.13m).ToString("0.00");
            EditStock = (p?.StockQty ?? 0m).ToString("0.###");
            EditUnitType = p?.UnitType ?? UnitType.Piece;
        }

        private async Task SaveProductAsync()
        {
            if (string.IsNullOrWhiteSpace(EditBarcode) || string.IsNullOrWhiteSpace(EditName))
            {
                ErrorMessage = "Product barcode and name are required.";
                return;
            }
            if (!decimal.TryParse(EditPrice, out var price) || price < 0)
            {
                ErrorMessage = "Price must be a non-negative number.";
                return;
            }
            if (!decimal.TryParse(EditTax, out var taxRate) || taxRate < 0)
            {
                ErrorMessage = "Tax rate must be a non-negative number (e.g. 0.13 for 13%).";
                return;
            }
            if (!decimal.TryParse(EditStock, out var stock) || stock < 0)
            {
                ErrorMessage = "Stock must be a non-negative number.";
                return;
            }

            if (SelectedProduct is null)
            {
                var existing = await _productRepository.GetProductByBarcodeAsync(EditBarcode);
                if (existing != null)
                {
                    ErrorMessage = "A product with this barcode already exists.";
                    return;
                }

                var product = new Product
                {
                    Barcode = EditBarcode,
                    ProductName = EditName,
                    Price = price,
                    Tax = taxRate,
                    StockQty = stock,
                    UnitType = EditUnitType,
                };
                await _productRepository.AddProductAsync(product);
            }
            else
            {
                SelectedProduct.ProductName = EditName;
                SelectedProduct.Price = price;
                SelectedProduct.Tax = taxRate;
                SelectedProduct.StockQty = stock;
                SelectedProduct.UnitType = EditUnitType;
                await _productRepository.UpdateProductAsync(SelectedProduct);
            }

            ErrorMessage = null;
            await LoadAsync();
            SelectedProduct = null;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
