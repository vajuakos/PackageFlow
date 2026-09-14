using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PackageFlow.Data.Services.Auth;
using PackageFlow.Views;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace PackageFlow.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IUserSessionService _userSessionService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public LoginViewModel(
        IAuthService authService,
        IUserSessionService userSessionService,
        INavigationService navigationService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _userSessionService = userSessionService ?? throw new ArgumentNullException(nameof(userSessionService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
    }

    [RelayCommand]
    private void Login(PasswordBox? passwordBox)
    {
        var loginResult = _authService.Login(Username, passwordBox?.Password);

        if (loginResult.IsLoginSucceed)
        {
            ErrorMessage = string.Empty;
            _userSessionService.SetSession(loginResult.CurrentUser);

            _navigationService.Navigate(typeof(PackageDashboard));
        }
        else
        {
            ErrorMessage = "Érvénytelen felhasználónév vagy jelszó!";
        }
    }

    [RelayCommand]
    private void GuestContinue()
    {
        ErrorMessage = string.Empty;
        _userSessionService.ClearSession();

        _navigationService.Navigate(typeof(PackageDashboard));
    }
}