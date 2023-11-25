using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Yorot.Installer.Views;

public partial class UpdateInstaller : AUC
{
    public UpdateInstaller()
    {
        InitializeComponent();
    }

    public override bool CanCloseApp => true;

    private void RestartInstaller(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void CancelUpdate(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void UpdateClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void SkipUpdate(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    public override Button[] GetButtons()
    {
        return Array.Empty<Button>();
    }
}