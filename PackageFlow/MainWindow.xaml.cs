using System;
using Wpf.Ui.Controls;

namespace PackageFlow;

public partial class MainWindow : FluentWindow
{
    public MainWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        RootNavigation.SetServiceProvider(serviceProvider);
    }
}