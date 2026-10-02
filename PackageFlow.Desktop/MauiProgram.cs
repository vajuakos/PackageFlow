using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;
using PackageFlow.Desktop.Services;
using PackageFlow.Desktop.Services.Authentication;

namespace PackageFlow.Desktop
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                config.SnackbarConfiguration.RequireInteraction = false;
                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 5000;
                config.SnackbarConfiguration.HideTransitionDuration = 300;
                config.SnackbarConfiguration.ShowTransitionDuration = 200;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            });

            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<CustomAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
                sp.GetRequiredService<CustomAuthenticationStateProvider>());

            builder.Services.AddScoped(sp =>
                new HttpClient { BaseAddress = new Uri("https://localhost:7130/") });

            builder.Services.AddScoped<IAuthService, AuthService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
