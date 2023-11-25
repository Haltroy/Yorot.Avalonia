using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Yorot.Installer.Views;

public partial class ActionMenu : AUC
{
    public ActionMenu()
    {
        InitializeComponent();
    }

    public override bool CanCloseApp => true;

    public override Button[] GetButtons()
    {
        return Array.Empty<Button>();
    }

    private void InstallClick(object? sender, RoutedEventArgs e)
    {
        SwitchTo(new FlavorPick());
    }

    private void ModifyClick(object? sender, RoutedEventArgs e)
    {
        SwitchTo(new UpdateMenu());
    }
}