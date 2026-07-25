using Kassa.Application.Interfaces;
using Kassa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kassa.Infrastructure.Repositories
{
    public class CashierRepository: ICashierRepository
    {
        private readonly AppDbContext _dbContext;

        public CashierRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Task<Cashier?> GetByIdAsync(int id)
        {
            return _dbContext.Cashiers.FirstOrDefaultAsync(c => c.Id == id);
        }

        public Task<Cashier?> GetByIdAndPincodeAsync(int cashierId, string pinCode)
        {
            return _dbContext.Cashiers.FirstOrDefaultAsync(c => c.Id == cashierId && c.PinCode == pinCode && c.IsActive);
        }

        public Task<List<Cashier>> GetActiveAsync() 
        { 
            return _dbContext.Cashiers.Where(c => c.IsActive).ToListAsync();
        }
    }
}
