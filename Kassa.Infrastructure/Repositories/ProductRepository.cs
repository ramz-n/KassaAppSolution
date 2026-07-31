using Kassa.Application.Interfaces;
using Kassa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kassa.Infrastructure.Repositories
{
    public class ProductRepository: IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Product>> GetAllProductsAsync()
        {
            return _context.Products.ToListAsync();
        }

        public Task<Product?> GetProductByBarcodeAsync(string barcode)
        {
            return _context.Products.FirstOrDefaultAsync(p => p.Barcode == barcode);
        }

        public Task<Product?> GetProductByIdAsync(int id)
        {
            return _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task<List<Product>> SearchProductByNameAsync(string search)
        {
            return _context.Products
                .Where(p => EF.Functions.ILike(p.ProductName, $"%{search}%"))
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        public Task<List<Product>> GetLowStockProductsAsync()
        {
            return _context.Products
                .Where(p => p.StockQty < p.LowStockQty)
                .OrderBy(p => p.StockQty)
                .ToListAsync();
        }

        public async Task AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TryDecrementStockAsync(int productId, decimal quantity, byte[] rowVersion)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product is null) return false;
            if (product.StockQty < quantity) return false;

            product.StockQty -= quantity;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }
    }
}
