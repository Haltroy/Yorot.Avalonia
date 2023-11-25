using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Yorot.Installer.Views;

public partial class LoadFlavorList : AUC
{
    public LoadFlavorList()
    {
        InitializeComponent();
    }

    public override bool CanCloseApp => true;

    private async void Init(object? sender, EventArgs e)
    {
        await Task.Run(() =>
        {
            // TODO
            Thread.Sleep(5000);
            Dispatcher.UIThread.InvokeAsync(() => { SwitchTo(new ActionMenu()); });
        });
    }

    public override Button[] GetButtons()
    {
        return Array.Empty<Button>();
    }
}