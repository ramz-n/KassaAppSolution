using Kassa.Domain.Entities;

namespace Kassa.Application.Interfaces
{
    public interface IKassaSessionRepository
    {
        Task<KassaSession?> GetOpenSessionAsync(int cashierId);
        Task AddAsync(KassaSession session);
        Task UpdateAsync(KassaSession session);
    }
}
