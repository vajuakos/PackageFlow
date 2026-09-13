using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PackageFlow.Data.Services.Auth;
using Wpf.Ui.Controls;

namespace PackageFlow.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IUserSessionService _userSessionService;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public event Action? LoginSucceeded;

    public LoginViewModel(IAuthService authService, IUserSessionService userSessionService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _userSessionService = userSessionService ?? throw new ArgumentNullException(nameof(userSessionService));
    }

    [RelayCommand]
    private void Login(PasswordBox? passwordBox)
    {
        var loginResult =  _authService.Login(Username, passwordBox?.Password);

        if (loginResult.IsLoginSucceed)
        {
            ErrorMessage = string.Empty;
            LoginSucceeded?.Invoke();

            _userSessionService.SetSession(loginResult.CurrentUser);
        }
        else
        {
            ErrorMessage = "Érvénytelen felhasználónév vagy jelszó!";
        }
    }
}