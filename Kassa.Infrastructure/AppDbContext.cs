using Kassa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kassa.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Cashier> Cashiers => Set<Cashier>();
        public DbSet<KassaSession> KassaSessions => Set<KassaSession>();
        public DbSet<Transaction> Transactions => Set<Transaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(e =>
            {
                e.Property(p => p.Id).UseIdentityByDefaultColumn();
                e.Property(p => p.UnitType).HasConversion<string>();
            });
            modelBuilder.Entity<Cashier>(e =>
            {
                e.Property(c => c.PinCode).HasMaxLength(4);
            });

            modelBuilder.Entity<KassaSession>(e =>
            {
                e.HasOne(s => s.Cashier)
                    .WithMany()
                    .HasForeignKey(s => s.CashierId);
            });

            modelBuilder.Entity<Transaction>(e =>
            {
                e.Property(t => t.Timestamp)
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                );
            });
        }
    }
}
