using Kassa.Application.Interfaces;
using Kassa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kassa.Infrastructure.Repositories
{
    public class TransactionRepository: ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public Task<int> CountForDateAsync(DateOnly date)
        {
            var localStart = date.ToDateTime(TimeOnly.MinValue);

            var start = DateTime.SpecifyKind(localStart, DateTimeKind.Utc);
            var end = start.AddDays(1);
            return _context.Transactions.CountAsync(t => t.Timestamp >= start && t.Timestamp < end);
        }
    }
}
