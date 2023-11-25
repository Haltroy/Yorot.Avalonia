using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Yorot.Installer.Views;

namespace Yorot.Installer;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        if (Background is Brush brush) brush.Opacity = 0.5;
        SwitchTo(new LoadFlavorList());
    }

    public void SwitchTo(AUC auc)
    {
        auc.Main = this;
        var current = ContentCarousel.SelectedItem;
        ContentCarousel.Items.Add(auc);
        ContentCarousel.SelectedItem = auc;
        ButtonPanel.Children.Clear();
        ButtonPanel.Children.AddRange(auc.GetButtons());
        var objectList = ContentCarousel.Items.Except(new[] { current, auc });
        foreach (var removeItem in objectList)
            ContentCarousel.Items.Remove(removeItem);
    }

    private void UseSystemThemeChanged(object? sender, RoutedEventArgs e)
    {
        if (Application.Current is not null)
            Application.Current.RequestedThemeVariant =
                UseSystemTheme.IsChecked is true ? ThemeVariant.Default
                : LightDarkMode.IsChecked is false ? ThemeVariant.Dark : ThemeVariant.Light;
        if (Background is Brush brush) brush.Opacity = 0.5;
    }

    private void LightDarkModeChanged(object? sender, RoutedEventArgs e)
    {
        if (Application.Current is not null)
            Application.Current.RequestedThemeVariant =
                UseSystemTheme.IsChecked is true ? ThemeVariant.Default
                : LightDarkMode.IsChecked is false ? ThemeVariant.Dark : ThemeVariant.Light;
        if (Background is Brush brush) brush.Opacity = 0.5;
    }

    private void SendFeedbackClick(object? sender, RoutedEventArgs e)
    {
        Process.Start("https://github.com/haltroy/Yorot.Avalonia/issues/new/");
    }

    private void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (ContentCarousel.SelectedItem is AUC { CanCloseApp: false })
            // TODO: Ask to close
            e.Cancel = true;
    }
}