using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Yorot.Installer.Views;

public partial class ProgressMenu : AUC
{
    public ProgressMenu()
    {
        InitializeComponent();
    }

    public override bool CanCloseApp => false;

    private void CarouselPrevious(object? sender, RoutedEventArgs e)
    {
        if (AdCarousel.SelectedIndex > 0) AdCarousel.SelectedIndex--;
    }

    private void CarouselNext(object? sender, RoutedEventArgs e)
    {
        if (AdCarousel.SelectedIndex < AdCarousel.Items.Count) AdCarousel.SelectedIndex++;
    }

    public override Button[] GetButtons()
    {
        var cancelButton = new Button { Content = "Cancel" };
        cancelButton.Click += CancelButtonOnClick;
        return new[] { cancelButton };
    }

    private void CancelButtonOnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}