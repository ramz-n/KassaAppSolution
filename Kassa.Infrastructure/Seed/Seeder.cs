using Kassa.Domain.Entities;

namespace Kassa.Infrastructure.Seed
{
    public static class Seeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (!context.Products.Any() )
            {
                var products = new List<Product>()
                {
                   new() {Id=1, ProductName="Coffee", Price=500, Barcode="8710400001011"  },
                   new() {Id=2, ProductName="Milk", Price=200, Barcode="8710400001022"  },
                   new() {Id=3, ProductName="Tea", Price=60, Barcode="8710400001011"  },
                };
                context.Products.AddRange(products);     
            }

            if(!context.Cashiers.Any())
            {
                var cashiers = new List<Cashier>()
                {
                   new() { Name="Qaium", PinCode="1234", IsActive=true },
                   new() {Name="Ram", PinCode="1111", IsActive=true },
                   new() { Name="Yusuf", PinCode="2222", IsActive=true },
                };
                context.Cashiers.AddRange(cashiers);
            }

            await context.SaveChangesAsync();
        }
    }
}
