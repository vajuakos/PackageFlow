using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PackageFlow.Data.Context;
using PackageFlow.Data.Seed;
using PackageFlow.Data.Services.Auth;
using PackageFlow.Data.Services.PackageHandler;
using PackageFlow.Services;
using PackageFlow.ViewModels;
using PackageFlow.Views;
using System.IO;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Abstractions;

namespace PackageFlow;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        #region Database connection
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));
        #endregion

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPackageHandlerService, PackageHandlerService>();

        // An instance of service created only once during the life-time
        services.AddSingleton<IUserSessionService, UserSessionService>();
        services.AddSingleton<INavigationViewPageProvider, PageService>();
        services.AddSingleton<INavigationService, NavigationService>();

        services.AddSingleton<MainWindow>();
        services.AddSingleton<LoginViewModel>();
        services.AddSingleton<LoginPage>();
        services.AddSingleton<PackageDashboardViewModel>();
        services.AddSingleton<PackageDashboard>();
        services.AddSingleton<CreatePackagePage>();
        services.AddSingleton<CreatePackageViewModel>();

        Services = services.BuildServiceProvider();

        using (var scope = Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
            DatabaseSeeder.Seed(db);
        }

        Services.GetRequiredService<MainWindow>().Show();
    }
}