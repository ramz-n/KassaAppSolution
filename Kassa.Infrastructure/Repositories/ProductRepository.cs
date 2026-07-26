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
    }
}
