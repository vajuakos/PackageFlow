using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PackageFlow.Data.Context;
using PackageFlow.Data.Seed;
using PackageFlow.Data.Services.Auth;
using PackageFlow.ViewModels;
using PackageFlow.Views;

namespace PackageFlow;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=packageflow.db"));
        services.AddScoped<IAuthService, AuthService>();

        // An instance of service created only once during the life-time
        services.AddSingleton<IUserSessionService, UserSessionService>();

        services.AddSingleton<MainWindow>();
        services.AddSingleton<LoginViewModel>();
        services.AddSingleton<LoginPage>();
        services.AddSingleton<DashboardViewModel>();
        services.AddSingleton<DashboardPage>();

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