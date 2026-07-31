using Kassa.Domain.Entities;

namespace Kassa.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByBarcodeAsync(string barcode);
        Task<Product?> GetProductByIdAsync(int id);
        Task<List<Product>> SearchProductByNameAsync(string search);
        Task<List<Product>> GetLowStockProductsAsync();
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task<bool> TryDecrementStockAsync(int productId, decimal quantity, byte[] rowVersion);
    }
}
