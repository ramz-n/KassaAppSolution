using Kassa.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kassa.Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddTransactionAsync(Transaction transaction);
        Task<int> CountForDateAsync(DateOnly date);
        Task<List<Transaction>> GetByDateRangeAsync(DateTime from, DateTime to, int? cashierId = null);
        Task<Transaction?> GetByIdWithLinesAsync(int id);
        Task<decimal> SumCashSalesAsync(DateTime from, DateTime to, int cashierId);
    }
}
