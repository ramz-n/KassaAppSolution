using Kassa.Application.Interfaces;
using Kassa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kassa.Infrastructure.Repositories
{
    public class KassaSessionRepository: IKassaSessionRepository
    {
        private readonly AppDbContext _context;

        public KassaSessionRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public Task<KassaSession?> GetOpenSessionAsync(int cashierId)
        {
            return _context.KassaSessions.FirstOrDefaultAsync(s => s.CashierId == cashierId && !s.IsClosed);
        }

        public async Task AddAsync(KassaSession session)
        { 
            _context.KassaSessions.Add(session);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(KassaSession session)
        {
            _context.KassaSessions.Update(session);
            await _context.SaveChangesAsync();
        }
    }
}
