using Kassa.Application.Cart;
using Kassa.Application.Interfaces;
using Kassa.Application.Services;
using Kassa.DesktopApp.ViewModels;
using Kassa.Infrastructure;
using Kassa.Infrastructure.Repositories;
using Kassa.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Windows;

namespace Kassa.DesktopApp
{
    public partial class App : System.Windows.Application
    {
        public static IHost AppHost { get; private set; } = null!;

        protected override async void OnStartup(StartupEventArgs e)
        {
            var culture = new System.Globalization.CultureInfo("en-Np");

            culture.NumberFormat.CurrencySymbol = "Rs. ";

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(System.Windows.Markup.XmlLanguage.GetLanguage(culture.IetfLanguageTag))
            );

            AppHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((_, config) =>
                {
                    config.SetBasePath(AppContext.BaseDirectory);
                    config.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        var connectionString = context.Configuration.GetConnectionString("DefaultConnection")
                           ?? throw new InvalidOperationException("Connection String is not set properly");

                        options.UseNpgsql(connectionString);

                    });

                    services.AddScoped<IProductRepository, ProductRepository>();
                    services.AddScoped<ICashierRepository, CashierRepository>();
                    services.AddScoped<IKassaSessionRepository, KassaSessionRepository>();

                    services.AddSingleton<ITaxCalculator, TaxCalculator>();

                    services.AddTransient<ICartService, CartService>();

                    services.AddTransient<ProductViewModel>();
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<CheckoutViewModel>();

                    services.AddTransient<MainWindow>();
                })
                .Build();

            await AppHost.StartAsync();

            using (var scope = AppHost.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                Console.WriteLine("Seeding database...");
                await Seeder.SeedAsync(db);
                Console.WriteLine("Database seeding completed.");
            }

            var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost.StopAsync();
            base.OnExit(e);
        }
    }
}
