using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Yorot_Avalonia.Views
{
    public partial class TabWindow : UserControl
    {
        public TabWindow()
        {
            InitializeComponent();
        }

        private Grid? Content;
        private TextBox? textbox;
        private System.Reactive.Subjects.Subject<bool> IsNavigated;
        private System.Reactive.Subjects.Subject<bool> IsNavigating;
        private System.Reactive.Subjects.Subject<bool> CanGoBack;
        private System.Reactive.Subjects.Subject<bool> CanGoForward;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            Content = this.FindControl<Grid>("Content");
            var stackPanel1 = Content.FindControl<StackPanel>("stackPanel1");
            textbox = stackPanel1.FindControl<TextBox>("tbUrl");
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

            Handlers.YorotWebView webView = new Handlers.YorotWebView(this)
            {
                Background = Avalonia.Media.Brush.Parse("#FFFFFF"),
                InitialUrl = "https://google.com", // TODO: Change this to the user's homepage when you are done with schemes (or something like SchemeHandler from CefSharp).
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                IsVisible = true,
            };
            webView.LoadError += WebView1_LoadError;
            webView.Navigated += WebView_Navigated;
            webView.Navigating += WebView1_Navigating;
            webView.DocumentTitleChanged += WebView_DocumentTitleChanged;
            dockPanel.Children.Add(webView);
            webView.BrowserCreated += WebView_BrowserCreated;
            webView1 = webView;
        }

        private void WebView1_LoadError(object? sender, CefNet.LoadErrorEventArgs e)
        {
            //webView1.Navigate("yorot://error");
        }

        private void WebView1_Navigating(object? sender, CefNet.BeforeBrowseEventArgs e)
        {
            IsNavigated.OnNext(false);
            IsNavigating.OnNext(true);
        }

        private Handlers.YorotWebView webView1;

        private void WebView_Navigated(object? sender, CefNet.NavigatedEventArgs e)
        {
            IsNavigated.OnNext(true);
            IsNavigating.OnNext(false);
            if (textbox != null)
            {
                CanGoBack.OnNext(webView1.CanGoBack);
                CanGoForward.OnNext(webView1.CanGoForward);
                textbox.Text = e.Url;
            }
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
            if (sender != null && webView1 != null && webView1.CanGoBack)
            {
                webView1.GoBack();
            }
        }

        private void goforward(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender != null && webView1 != null && webView1.CanGoForward)
            {
                webView1.GoForward();
            }
        }

        private void gohome(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender != null && webView1 != null && webView1.CanGoForward)
            {
                webView1.Navigate(YorotGlobal.Main.CurrentSettings.HomePage);
            }
        }

        private void reload(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            webView1.Reload();
        }

        private void stop(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            webView1.Stop();
        }

        private void gobutton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            webView1.Navigate(textbox.Text);
        }

        private void WebView_DocumentTitleChanged(object? sender, CefNet.DocumentTitleChangedEventArgs e)
        {
            if (sender != null && sender is Handlers.YorotWebView webView && this.Parent is DockPanel dockPanel && dockPanel.Parent is TabItem tabItem)
            {
                tabItem.Header = e.Title + " - Yorot";
            }
        }

        private void WebView_BrowserCreated(object? sender, System.EventArgs e)
        {
            if (sender != null && sender is Handlers.YorotWebView webView)
            {
                webView.Navigate("https://google.com");
            }
        }
    }
}