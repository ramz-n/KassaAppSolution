using Kassa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kassa.Infrastructure
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) 
        {
        }
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Cashier> Cashiers => Set<Cashier>();
    }
}
