using System;
using System.Windows;
using PackageFlow.ViewModels;
using PackageFlow.Views;
using Wpf.Ui.Controls;

namespace PackageFlow;

public partial class MainWindow : FluentWindow
{
    private readonly LoginViewModel _loginViewModel;

    public MainWindow(IServiceProvider serviceProvider, LoginViewModel loginViewModel)
    {
        InitializeComponent();
        _loginViewModel = loginViewModel;

        RootNavigation.SetServiceProvider(serviceProvider);

        _loginViewModel.LoginSucceeded += OnLoginSucceeded;

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        ShowLogin();
    }

    private void ShowLogin()
    {
        RootNavigation.PaneDisplayMode = NavigationViewPaneDisplayMode.LeftMinimal;
        RootNavigation.IsPaneOpen = false;
        RootNavigation.Navigate(typeof(LoginPage));
    }

    private void OnLoginSucceeded()
    {
        RootNavigation.PaneDisplayMode = NavigationViewPaneDisplayMode.Left;
    }
}