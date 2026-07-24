using Kassa.Domain.Entities;

namespace Kassa.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync();
    }
}
