using System;
using System.Reactive.Subjects;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Yorot.Installer.Views;

public partial class UpdateMenu : AUC
{
    private readonly Subject<bool> AllowInstallUpdate = new();
    private readonly Subject<bool> AllowUninstall = new();

    public UpdateMenu()
    {
        InitializeComponent();
    }

    public override bool CanCloseApp => true;

    public override Button[] GetButtons()
    {
        var installUpdateAll = new Button { Content = "Install/Update Selected..." };
        installUpdateAll.Bind(IsEnabledProperty, AllowInstallUpdate);
        installUpdateAll.Click += InstallUpdateSelected;
        var uninstallAll = new Button { Content = "Uninstall Selected..." };
        uninstallAll.Bind(IsEnabledProperty, AllowUninstall);
        uninstallAll.Click += UninstallSelected;
        return new[] { installUpdateAll, uninstallAll };
    }

    private void UninstallSelected(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void InstallUpdateSelected(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}