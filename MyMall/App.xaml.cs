using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyMall.Data;
using MyMall.Data.Repositories;
using MyMall.Services.Implementations;
using MyMall.Services.Interfaces;
using MyMall.ViewModels;
using MyMall.ViewModels.Admin;
using MyMall.ViewModels.Cashier;
using MyMall.Views;
using MyMall.Views.Admin;
using System.Windows;

namespace MyMall;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var services = new ServiceCollection();

        // DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddTransient<ProductsViewModel>();
        // ViewModels
        services.AddTransient<LoginViewModel>();

        // Windows
        services.AddTransient<LoginWindow>();
        services.AddTransient<LoginWindow>();
        services.AddTransient<AdminMainWindow>();
        Services = services.BuildServiceProvider();
        services.AddTransient<CategoriesViewModel>();
        services.AddTransient<ProductEditViewModel>();
        services.AddTransient<CashierMainViewModel>();
        services.AddTransient<SaleViewModel>();
        services.AddTransient<MyMall.Views.Cashier.CashierMainWindow>();
        services.AddTransient<ReportsViewModel>();
        services.AddTransient<SalesHistoryViewModel>();
        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<AdminMainViewModel>();
        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<AdminMainViewModel>();
        services.AddTransient<ProductsViewModel>();
        // Windows



        var adminHash = MyMall.Common.PasswordHasher.Hash("admin123");
        var cashierHash = MyMall.Common.PasswordHasher.Hash("cashier123");
        System.Diagnostics.Debug.WriteLine($"ADMIN HASH: {adminHash}");
        System.Diagnostics.Debug.WriteLine($"CASHIER HASH: {cashierHash}");
        Services = services.BuildServiceProvider();
        var loginWindow = Services.GetRequiredService<LoginWindow>();
        loginWindow.Show();
    }
}