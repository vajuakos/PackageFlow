using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PackageFlow.Core.Models;
using PackageFlow.Data.Services.PackageHandler;
using PackageFlow.Views;
using Wpf.Ui;

namespace PackageFlow.ViewModels;

public partial class PackageDashboardViewModel : ObservableObject
{
    private readonly IPackageHandlerService _packageService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _trackingSearchNumber = string.Empty;

    [ObservableProperty]
    private Package? _foundPackage;

    [ObservableProperty]
    private bool _isPackageNotFound;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public PackageDashboardViewModel(
        IPackageHandlerService packageService,
        INavigationService navigationService)
    {
        _packageService = packageService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    private void SearchPackage()
    {
        ErrorMessage = string.Empty;
        IsPackageNotFound = false;

        if (string.IsNullOrWhiteSpace(TrackingSearchNumber))
        {
            FoundPackage = null;
            ErrorMessage = "Kérlek, add meg a csomagszámot!";
            return;
        }

        var result = _packageService.GetPackageByTrackingNumber(TrackingSearchNumber);

        if (result is null)
        {
            FoundPackage = null;
            IsPackageNotFound = true;
            ErrorMessage = $"Nem található küldemény ezzel a számmal: {TrackingSearchNumber.Trim()}";
        }
        else
        {
            FoundPackage = result;
        }
    }

    [RelayCommand]
    private void NavigateToCreatePackage()
    {
        _navigationService.Navigate(typeof(CreatePackagePage));
    }
}