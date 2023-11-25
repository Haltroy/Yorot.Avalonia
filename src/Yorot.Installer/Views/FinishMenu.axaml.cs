using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace Yorot.Installer.Views;

public partial class FinishMenu : AUC
{
    private YorotInstallerFinishInfo? Info;

    public FinishMenu()
    {
        InitializeComponent();
    }

    public override bool CanCloseApp => true;

    public override Button[] GetButtons()
    {
        var finish = new Button { Content = "Finish" };
        finish.Click += FinishOnClick;
        return new[] { finish };
    }

    public FinishMenu WithInfo(YorotInstallerFinishInfo info)
    {
        Info = info;
        return this;
    }

    private void FinishOnClick(object? sender, RoutedEventArgs e)
    {
        if (RunFlavor.IsChecked is true && FlavorList.SelectedItem is ComboBoxItem
            {
                // TODO:
                /* Tag: YorotFlavor flavor */
            })
        {
            // TODO: System.Diagnostics.Process.Start(flavor.ExecutablePath);
        }

        if (Application.Current is
            { ApplicationLifetime: IClassicDesktopStyleApplicationLifetime lifetime }) lifetime.Shutdown();
    }

    private async void SaveToFileClick(object? sender, RoutedEventArgs e)
    {
        await Task.Run(async () =>
        {
            if (!Main.StorageProvider.CanSave) return;

            var file = await Main.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save log to...",
                FileTypeChoices = new[]
                {
                    FilePickerFileTypes.TextPlain,
                    FilePickerFileTypes.All
                },
                ShowOverwritePrompt = true
            });

            if (file is not null)
            {
                await using var stream = await file.OpenWriteAsync();
                await using var writer = new StreamWriter(stream, Encoding.UTF8);
                await writer.WriteLineAsync(ErrorText.Text);
            }
        });
    }

    private void Init(object? sender, EventArgs e)
    {
        if (Info is null) return;
        ErrorPanel.IsVisible = Info.IsFail;
        ErrorText.Text = Info.ErrorMessage;
        if (!Info.IsFail) RunFlavor.IsChecked = true;
        // TODO: Load flavors here
    }

    public class YorotInstallerFinishInfo
    {
        public bool IsFail { get; set; }
        public string ErrorMessage { get; set; }

        // public YorotFlavor[] Flavors {get;set;}
    }
}