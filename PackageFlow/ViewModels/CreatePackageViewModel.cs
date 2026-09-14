using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PackageFlow.Core.DTOs;
using PackageFlow.Core.Enums;
using PackageFlow.Data.Services.Auth;
using PackageFlow.Data.Services.PackageHandler;
using PackageFlow.Views;
using Wpf.Ui;

namespace PackageFlow.ViewModels;

public partial class CreatePackageViewModel : ObservableObject
{
    private readonly IPackageHandlerService _packageService;
    private readonly IUserSessionService _userSessionService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private CreatePackageRequest _request = new();

    public IReadOnlyList<PackageSizeOption> PackageSizes { get; } =
    [
        new PackageSizeOption(PackageSize.Small,  "S – Kis doboz (pl. telefon, könyv)"),
        new PackageSizeOption(PackageSize.Medium,  "M – Közepes doboz (pl. cipősdoboz)"),
        new PackageSizeOption(PackageSize.Large,  "L – Nagy csomag (max. 15 kg)"),
        new PackageSizeOption(PackageSize.ExtraLarge, "XL – Extra méret / Nehéz küldemény")
    ];

    public CreatePackageViewModel(
        IPackageHandlerService packageService,
        IUserSessionService userSessionService,
        INavigationService navigationService)
    {
        _packageService = packageService;
        _userSessionService = userSessionService;
        _navigationService = navigationService;

        PrefillSenderFieldsIfLoggedIn();
    }

    private void PrefillSenderFieldsIfLoggedIn()
    {
        var currentUser = _userSessionService.CurrentUser;
        if (currentUser != null)
        {
            Request.SenderUserId = currentUser.Id;
            Request.SenderName = currentUser.FullName ?? currentUser.Username;
            Request.SenderEmail = currentUser.Email ?? string.Empty;
            Request.SenderPhone = currentUser.PhoneNumber ?? string.Empty;
        }
    }

    [RelayCommand]
    private void SubmitPackage()
    {
        Request.SenderUserId = _userSessionService.CurrentUser?.Id;

        _packageService.CreatePackage(Request);

        _navigationService.Navigate(typeof(PackageDashboard));
    }

    [RelayCommand]
    private void NavigateBack()
    {
        ResetForm();
        _navigationService.Navigate(typeof(PackageDashboard));
    }

    private void ResetForm()
    {
        Request = new CreatePackageRequest();
        PrefillSenderFieldsIfLoggedIn();
    }

    public record PackageSizeOption(PackageSize Value, string DisplayText);
}