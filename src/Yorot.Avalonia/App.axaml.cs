using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Yorot_Avalonia.ViewModels;
using Yorot_Avalonia.Views;

namespace Yorot_Avalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IControlledApplicationLifetime desktop && YorotGlobal.Main != null)
            {
                desktop.Startup += Desktop_Startup;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void Desktop_Startup(object? sender, ControlledApplicationLifetimeStartupEventArgs e)
        {
            if (YorotGlobal.Main != null && !YorotGlobal.Main.OOBE)
            {
                new MainWindow
                {
                    DataContext = YorotGlobal.ViewModel,
                }.Show();
            }
            else
            {
                new OOBEWindow()
                {
                    DataContext = YorotGlobal.ViewModel,
                }.Show();
            }
        }
    }
}