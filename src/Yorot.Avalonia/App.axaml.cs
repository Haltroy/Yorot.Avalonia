using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CefNet;
using System;
using System.Linq;
using Yorot_Avalonia.ViewModels;
using Yorot_Avalonia.Views;

namespace Yorot_Avalonia
{
    public class App : Application
    {
        public static event EventHandler FrameworkInitialized;

        public static event EventHandler FrameworkShutdown;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                if (YorotGlobal.Main != null)
                {
                    YorotGlobal.Main.Init();
                    if (!YorotGlobal.Main.OOBE)
                    {
                        desktop.MainWindow = new MainWindow
                        {
                            DataContext = YorotGlobal.ViewModel,
                        };
                    }
                    else
                    {
                        desktop.MainWindow = new OOBEWindow()
                        {
                            DataContext = YorotGlobal.ViewModel,
                        };
                    }
                }
                desktop.Startup += Desktop_Startup;
                desktop.Exit += Desktop_Exit;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void Desktop_Exit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            FrameworkShutdown?.Invoke(this, EventArgs.Empty);
        }

        private void Desktop_Startup(object? sender, ControlledApplicationLifetimeStartupEventArgs e)
        {
            FrameworkInitialized?.Invoke(this, EventArgs.Empty);
        }
    }
}