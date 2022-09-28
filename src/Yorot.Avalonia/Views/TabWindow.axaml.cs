using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using Xilium.CefGlue;
using Xilium.CefGlue.Avalonia;
using Yorot.Handlers;

namespace Yorot_Avalonia.Views
{
    public partial class TabWindow : UserControl
    {
        public TabWindow()
        {
            InitializeComponent();
        }

        private string _startUrl = "https:/google.com"; // TODO: Change this

        public TabWindow(string url = "yorot://newtab")
        {
            _startUrl = url;
            InitializeComponent();
        }

        public TabWindow(MainWindow window, string url = "yorot://newtab")
        {
            _startUrl = url;
            mainWindow = window;
            InitializeComponent();
        }

        private MainWindow mainWindow;
        private AvaloniaCefBrowser browser;

        private Grid? Content;
        private TextBox? tbUrl;
        private System.Reactive.Subjects.Subject<bool> IsNavigated;
        private System.Reactive.Subjects.Subject<bool> IsNavigating;
        private System.Reactive.Subjects.Subject<bool> CanGoBack;
        private System.Reactive.Subjects.Subject<bool> CanGoForward;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            Content = this.FindControl<Grid>("Content");
            var stackPanel1 = Content.FindControl<DockPanel>("dockPanel1");
            tbUrl = stackPanel1.FindControl<TextBox>("tbUrl");
            IsNavigating = new System.Reactive.Subjects.Subject<bool>();
            IsNavigated = new System.Reactive.Subjects.Subject<bool>();
            CanGoBack = new System.Reactive.Subjects.Subject<bool>();
            CanGoForward = new System.Reactive.Subjects.Subject<bool>();

            IsNavigating.OnNext(true);
            IsNavigated.OnNext(false);
            CanGoBack.OnNext(false);
            CanGoForward.OnNext(false);

            stackPanel1.FindControl<Button>("reload").Bind(IsVisibleProperty, IsNavigated);
            stackPanel1.FindControl<Button>("stop").Bind(IsVisibleProperty, IsNavigating);
            stackPanel1.FindControl<Button>("goback").Bind(IsEnabledProperty, CanGoBack);
            stackPanel1.FindControl<Button>("goforward").Bind(IsEnabledProperty, CanGoForward);

            //var tabpanel = new TabPanel();
            //Content.Children.Add(tabpanel);

            // TODO: Find a way to make a tabbed form.
            // TODO: Complete the rest of the visuals for full navigation experience.

            DockPanel dockPanel = Content.FindControl<DockPanel>("cefpanel");

            CefBrowserSettings cefBrowserSettings = new CefBrowserSettings();

            var browser = new AvaloniaCefBrowser()
            {
                Address = _startUrl,
                DisplayHandler = new YorotDisplayHandler(this),
                RequestHandler = new YorotRequestHandler(this),
            };

            dockPanel.Children.Add(browser);

            //wvControl1 = Content.FindControl<WebViewControl.WebView>("webView1");

            //wvControl1.AllowDeveloperTools = true;

            //wvControl1.BeforeNavigate += WvControl1_BeforeNavigate;

            //wvControl1.TitleChanged += WvControl1_TitleChanged;

            //wvControl1.LoadFailed += WvControl1_LoadFailed;

            //wvControl1.Navigated += WvControl1_Navigated;

            //Handlers.YorotWebView webView = new Handlers.YorotWebView(this)
            //{
            //    Background = Avalonia.Media.Brush.Parse("#FFFFFF"),
            //    InitialUrl = "https://google.com", // TODO: Change this to the user's homepage when you are done with schemes (or something like SchemeHandler from CefSharp).
            //    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
            //    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            //    IsVisible = true,
            //};
            //webView.LoadError += WebView1_LoadError;
            //webView.Navigated += WebView_Navigated;
            //webView.Navigating += WebView1_Navigating;
            //webView.DocumentTitleChanged += WebView_DocumentTitleChanged;
            //dockPanel.Children.Add(webView);
            //webView.BrowserCreated += WebView_BrowserCreated;
            //webView1 = webView;
        }

        private void urlkeydown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Enter)
            {
                gobutton(sender, e);
            }
        }

        private void goback(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender != null && browser != null && browser.CanGoBack)
            {
            }
        }

        private void goforward(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender != null && browser != null && browser.CanGoForward)
            {
            }
        }

        private void gohome(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender != null && browser != null && browser.CanGoForward)
            {
                browser.Address = YorotGlobal.Main.CurrentSettings.HomePage;
            }
        }

        private void reload(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            browser.Reload();
        }

        private void stop(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
        }

        private void gobutton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            //webView1.Navigate(tbUrl.Text);
            //wvControl1.LoadUrl(tbUrl.Text);
        }

        internal void ChangeAddress(string url, bool load = false)
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(new System.Action(() =>
            {
                tbUrl.Text = url;
                if (load)
                {
                    browser.Address = url;
                }
            }), Avalonia.Threading.DispatcherPriority.Normal);
        }

        internal void ChangeTitle(string text)
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(new System.Action(() =>
            {
                if (Parent is DockPanel dockPanel && dockPanel.Parent is TabItem tabItem)
                {
                    tabItem.Header = text;
                    if (tabItem.Parent is TabControl tabControl && tabControl.SelectedItem == tabItem && mainWindow != null)
                    {
                        mainWindow.Tabs_SelectionChanged(this, null);
                    }
                }
            }), Avalonia.Threading.DispatcherPriority.Normal);
        }
    }
}