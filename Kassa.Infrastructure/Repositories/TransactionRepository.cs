using Kassa.Application.Interfaces;
using Kassa.Domain.Entities;
using Kassa.Domain.Enums;
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

        public Task<List<Transaction>> GetByDateRangeAsync(DateTime from, DateTime to, int? cashierId = null)
        {
            return _context.Transactions
                .Include(t => t.Cashier)
                .Where(t => t.Timestamp >= from && t.Timestamp <= to )
                .Where(t => cashierId == null || t.CashierId == cashierId)
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();
        }

        public Task<Transaction?> GetByIdWithLinesAsync(int id)
        {
            return _context.Transactions.Include(t => t.Cashier)
                .Include(t => t.Lines)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<decimal> SumCashSalesAsync(DateTime from, DateTime to, int cashierId)
        {
            var sum = await _context.Transactions
            .Where(t => t.Timestamp >= from && t.Timestamp <= to
                        && t.CashierId == cashierId
                        && t.PaymentMethod == PaymentMethod.Cash)
            .SumAsync(t => (decimal?)t.Total);
            return sum ?? 0m;
        }
    }
}
