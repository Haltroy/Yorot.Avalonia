using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TabTest
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private YorotTabControl testTabControl;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            var mainDock = this.FindControl<DockPanel>("mainDock");
            testTabControl = new YorotTabControl();
            testTabControl.Items.Add(new YorotTabItem() { Text = "Test" });
            mainDock.Children.Add(testTabControl);
        }
    }
}