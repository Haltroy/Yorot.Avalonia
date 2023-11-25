using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;

namespace Yorot.Installer.Views;

public partial class InstallOptions : AUC
{
    public InstallOptions()
    {
        InitializeComponent();
    }

    public override bool CanCloseApp => false;

    private async void CustomInstallLocation(object? sender, RoutedEventArgs e)
    {
        await Task.Run(async () =>
        {
            if (Main is null || !Main.StorageProvider.CanPickFolder) return;

            var folder = await Main.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select an installation folder...",
                AllowMultiple = false
            });

            if (folder.Count > 0)
                await Dispatcher.UIThread.InvokeAsync(() => InstallLocation.Text = folder[0].Path.AbsolutePath);
        });
    }

    public override Button[] GetButtons()
    {
        var nextButton = new Button { Content = "Next" };
        var backButton = new Button { Content = "Back" };
        nextButton.Click += NextButtonOnClick;
        backButton.Click += BackButtonOnClick;
        return new[]
        {
            nextButton,
            backButton
        };
    }

    private void BackButtonOnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void NextButtonOnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}