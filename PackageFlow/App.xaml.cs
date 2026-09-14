using Microsoft.EntityFrameworkCore;
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

        #region Database connection
        var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(localFolder, "PackageFlow");

        Directory.CreateDirectory(appFolder);

        var dbPath = Path.Combine(appFolder, "packageflow.db");

        services.AddDbContext<AppDbContext>(opt => opt.UseSqlite($"Data Source={dbPath}"));
        #endregion

        services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=packageflow.db"));
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