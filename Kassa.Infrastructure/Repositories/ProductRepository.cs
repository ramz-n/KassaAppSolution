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
    }
}
