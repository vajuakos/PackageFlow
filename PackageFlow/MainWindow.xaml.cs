using PackageFlow.Data.Services.Auth;
using PackageFlow.Views;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace PackageFlow;

public partial class MainWindow : FluentWindow
{
    private readonly INavigationService _navigationService;
    private readonly IUserSessionService _userSessionService;

    public MainWindow(INavigationService navigationService, IUserSessionService userSessionService)
    {
        InitializeComponent();
        _navigationService = navigationService;
        _userSessionService = userSessionService;

        _navigationService.SetNavigationControl(RootNavigation);

        RootNavigation.Navigated += (sender, args) =>
        {
            if (args.Page is LoginPage || !_userSessionService.IsAuthenticated)
            {
                RootNavigation.PaneDisplayMode = NavigationViewPaneDisplayMode.LeftMinimal;
                RootNavigation.IsPaneOpen = false;
            }
            else
            {
                RootNavigation.PaneDisplayMode = NavigationViewPaneDisplayMode.Left;
            }
        };

        Loaded += (_, _) =>
        {
            _navigationService.Navigate(typeof(LoginPage));
        };
    }
}