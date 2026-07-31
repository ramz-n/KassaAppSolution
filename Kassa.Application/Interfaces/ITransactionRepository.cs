using Kassa.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kassa.Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
        Task<int> CountForDateAsync(DateOnly date);
    }
}
