using Avalonia.Controls;

namespace Yorot.Installer.Views;

public abstract class AUC : UserControl
{
    public MainWindow? Main;
    public abstract bool CanCloseApp { get; }
    public abstract Button[] GetButtons();

    public void SwitchTo(AUC auc)
    {
        Main?.SwitchTo(auc);
    }
}