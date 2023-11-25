using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace Yorot.Installer.Views;

public partial class FlavorPick : AUC
{
    private readonly Button CancelButton;
    private readonly Button NextButton;

    public FlavorPick()
    {
        InitializeComponent();
        NextButton = new Button { Content = "Next", IsEnabled = false, IsDefault = true };
        NextButton.Click += NextButtonOnClick;

        CancelButton = new Button { Content = "Cancel", IsEnabled = false, IsCancel = true };
        CancelButton.Click += CancelButtonOnClick;
    }

    public override bool CanCloseApp => false;

    // TODO: public FlavorPick WithFlavors(YorotFlavor[] flavors)

    private static Control GenerateFlavorItem( /* YorotFlavor flavor */)
    {
        var flavorButton = new ToggleButton { Margin = new Thickness(10) }; // TODO: Tag = lavor

        var panel1 = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing = 5,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(10)
        };

        var flavorLogo = new Image
        {
            Width = 86,
            Height = 86
            // TODO: Source = flavor.Icon
        };

        var flavorTitle = new TextBlock
        {
            FontSize = 17.5,
            HorizontalAlignment = HorizontalAlignment.Center
            // TODO: Text = flavor.Title
        };

        var flavorDescription = new TextBlock
        {
            HorizontalAlignment = HorizontalAlignment.Center
            // TODO: Text = flavor.Description
        };

        var installPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 5
        };

        var installMethod = new ToggleSwitch
        {
            OnContent = "From Source",
            OffContent = "Package"
        };

        flavorButton.IsCheckedChanged += FlavorCheckedChanged;
        flavorButton.Content = panel1;
        panel1.Children.Add(flavorLogo);
        panel1.Children.Add(flavorTitle);
        panel1.Children.Add(flavorDescription);
        panel1.Children.Add(installPanel);

        installPanel.Children.Add(new TextBlock { Text = "Method:", VerticalAlignment = VerticalAlignment.Center });
        installPanel.Children.Add(installMethod);
        installMethod.IsCheckedChanged += (sender, args) =>
        {
            // TODO
        };

        return flavorButton;
    }

    private static void FlavorCheckedChanged(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void CancelButtonOnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void NextButtonOnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    public override Button[] GetButtons()
    {
        return new[] { NextButton, CancelButton };
    }
}