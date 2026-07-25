using Kassa.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kassa.Application.Interfaces
{
    public interface ICashierRepository
    {
        Task<Cashier?> GetByIdAsync(int id);
        Task<Cashier?> GetByIdAndPincodeAsync(int cashierId, string pinCode);
        Task<List<Cashier>> GetActiveAsync();
    }
}
