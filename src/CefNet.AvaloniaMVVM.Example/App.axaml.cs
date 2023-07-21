using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Yorot_Avalonia.ViewModels;
using Yorot_Avalonia.Views;

namespace Yorot_Avalonia;

public partial class App : Application
{
    public static event EventHandler? FrameworkInitialized;

    public static event EventHandler? FrameworkShutdown;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            YorotGlobal.Main = new YorotSpecial(YorotGlobal.YorotAppPath, "Yorot", YorotGlobal.CodeName, YorotGlobal.Version, YorotGlobal.VersionNo, desktop.Args.Contains("-i"));
            YorotGlobal.Main.CompleteInit();
            YorotGlobal.ViewModel = new ViewModelBase();

            if (desktop.Args.Contains("--oobe"))
            {
                YorotGlobal.Main.OOBE = true;
            }

            if (YorotGlobal.Main.OOBE)
            {
                desktop.MainWindow = new OOBEWindow
                {
                    DataContext = YorotGlobal.ViewModel,
                };
            }
            else
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = YorotGlobal.ViewModel,
                };
            }
            desktop.Startup += (s, e) => FrameworkInitialized?.Invoke(this, EventArgs.Empty);
            desktop.Exit += (s, e) => FrameworkShutdown?.Invoke(this, EventArgs.Empty);
        }

        base.OnFrameworkInitializationCompleted();
    }
}