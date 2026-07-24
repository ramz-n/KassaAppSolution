using Kassa.Application.Interfaces;
using Kassa.Domain.Entities;
using System.Collections.ObjectModel;

namespace Kassa.DesktopApp.ViewModels
{
    public class ProductViewModel
    {
        private readonly IProductRepository _productRepository;

        public ObservableCollection<Product> Products { get; set; } = new();

        public ProductViewModel(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task LoadAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            Products.Clear();

            foreach (var product in products) 
            {
                Products.Add(product);
            }
        }
    }
}
