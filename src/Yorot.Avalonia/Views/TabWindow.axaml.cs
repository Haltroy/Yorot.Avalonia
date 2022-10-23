using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using CefNet;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yorot;
using Yorot.AppForms;
using Yorot_Avalonia.Handlers;
using Yorot_Avalonia.ViewModels;

namespace Yorot_Avalonia.Views
{
    public partial class TabWindow : UserControl
    {
        public TabWindow()
        {
            InitializeComponent();
        }

        public string _startUrl = "yorot://newtab";

        public MainWindow? mainWindow;
        public System.Reactive.Subjects.Subject<bool>? IsPageSafe;
        public System.Reactive.Subjects.Subject<bool>? IsPageUsedCookie;
        public System.Reactive.Subjects.Subject<bool>? IsPageUnsafe;
        public SessionSystem? SessionSystem;

        private Grid? ContentGrid;
        private Avalonia.Controls.Menu? favoritesmenu;
        private TextBox? tbUrl;
        private System.Reactive.Subjects.Subject<bool>? IsNavigated;
        private System.Reactive.Subjects.Subject<bool>? IsNavigating;
        private System.Reactive.Subjects.Subject<bool>? CanGoBack;
        private System.Reactive.Subjects.Subject<bool>? CanGoForward;
        private System.Reactive.Subjects.Subject<bool>? CanZoomIn;
        private System.Reactive.Subjects.Subject<bool>? CanZoomOut;
        private System.Reactive.Subjects.Subject<bool>? IsFavorited;
        private System.Reactive.Subjects.Subject<bool>? IsNotFavorited;
        private System.Reactive.Subjects.Subject<bool>? IsMuted;
        private System.Reactive.Subjects.Subject<bool>? IsUnmuted;
        private Avalonia.Controls.MenuItem? other_bookmarks;

        private void InitializeComponent()
        {
            SessionSystem = new(YorotGlobal.Main);
            SessionSystem.LoadPage += (url) => { if (webView1 != null) { webView1.Navigate(url); } };
            SessionSystem.Sessions.Add(new Session(_startUrl));
            SessionSystem.SelectedSession = SessionSystem.Sessions[0];
            SessionSystem.SelectedIndex = 0;
            AvaloniaXamlLoader.Load(this);
            ContentGrid = this.FindControl<Grid>("Content");
            var stackPanel1 = ContentGrid.FindControl<DockPanel>("dockPanel1");
            tbUrl = stackPanel1.FindControl<TextBox>("tbUrl");
            IsNavigating = new System.Reactive.Subjects.Subject<bool>();
            IsNavigated = new System.Reactive.Subjects.Subject<bool>();
            CanGoBack = new System.Reactive.Subjects.Subject<bool>();
            CanGoForward = new System.Reactive.Subjects.Subject<bool>();
            IsFavorited = new System.Reactive.Subjects.Subject<bool>();
            IsNotFavorited = new System.Reactive.Subjects.Subject<bool>();
            IsMuted = new System.Reactive.Subjects.Subject<bool>();
            IsUnmuted = new System.Reactive.Subjects.Subject<bool>();
            IsPageSafe = new System.Reactive.Subjects.Subject<bool>();
            IsPageUsedCookie = new System.Reactive.Subjects.Subject<bool>();
            IsPageUnsafe = new System.Reactive.Subjects.Subject<bool>();
            CanZoomIn = new System.Reactive.Subjects.Subject<bool>();
            CanZoomOut = new System.Reactive.Subjects.Subject<bool>();

            IsNavigating.OnNext(true);
            IsNavigated.OnNext(false);
            CanGoBack.OnNext(false);
            CanGoForward.OnNext(false);
            IsFavorited.OnNext(false);
            IsNotFavorited.OnNext(true);
            IsMuted.OnNext(false);
            IsUnmuted.OnNext(true);
            IsPageSafe.OnNext(true);
            IsPageUsedCookie.OnNext(false);
            IsPageUnsafe.OnNext(false);
            CanZoomIn.OnNext(true);
            CanZoomOut.OnNext(true);

            stackPanel1.FindControl<Avalonia.Controls.Button>("reload").Bind(IsVisibleProperty, IsNavigated);
            stackPanel1.FindControl<Avalonia.Controls.Button>("stop").Bind(IsVisibleProperty, IsNavigating);
            var goback = stackPanel1.FindControl<Avalonia.Controls.Button>("goback");
            goback.Bind(IsEnabledProperty, CanGoBack);
            var goforward = stackPanel1.FindControl<Avalonia.Controls.Button>("goforward");
            goforward.Bind(IsEnabledProperty, CanGoForward);
            stackPanel1.FindControl<Avalonia.Controls.Button>("favoritebutton").Bind(IsVisibleProperty, IsNotFavorited);
            stackPanel1.FindControl<Avalonia.Controls.Button>("favoritedbutton").Bind(IsVisibleProperty, IsFavorited);

            var securitybutton = stackPanel1.FindControl<Avalonia.Controls.Button>("SecurityInfo");

            if (securitybutton.Content is Panel securitypanel)
            {
                securitypanel.FindControl<Avalonia.Controls.Image>("WebsiteGood").Bind(IsVisibleProperty, IsPageSafe);
                securitypanel.FindControl<Avalonia.Controls.Image>("WebsiteMeh").Bind(IsVisibleProperty, IsPageUsedCookie);
                securitypanel.FindControl<Avalonia.Controls.Image>("WebsiteBad").Bind(IsVisibleProperty, IsPageUnsafe);
            }

            var backflyout = new Avalonia.Controls.MenuFlyout();
            goback.ContextFlyout = backflyout;

            backflyout.Opening += (sender, e) =>
            {
                if (backflyout.Items is AvaloniaList<object> list)
                {
                    list.Clear();
                    var before = SessionSystem.Before();
                    for (int i = before.Length - 1; i > 0; i--)
                    {
                        Avalonia.Controls.MenuItem item = new() { Name = i + "", Header = before[i].Title, Tag = before[i] };
                        item.Click += backforwarditem_click;
                        list.Add(item);
                    }
                }
            };

            var forwardflyout = new Avalonia.Controls.MenuFlyout();
            goforward.ContextFlyout = forwardflyout;

            forwardflyout.Opening += (sender, e) =>
            {
                if (forwardflyout.Items is AvaloniaList<object> list)
                {
                    list.Clear();
                    var after = SessionSystem.After();
                    for (int i = 0; i < after.Length; i++)
                    {
                        Avalonia.Controls.MenuItem item = new() { Name = i + "", Header = after[i].Title, Tag = after[i] };
                        item.Click += backforwarditem_click;
                        list.Add(item);
                    }
                }
            };

            var dotmenu = stackPanel1.FindControl<Avalonia.Controls.Button>("dotmenu").ContextFlyout;

            dotmenu.Placement = FlyoutPlacementMode.Bottom;
            dotmenu.ShowMode = FlyoutShowMode.Standard;

            if (dotmenu is Flyout dotflayout && dotflayout.Content is StackPanel menu)
            {
                // None of these are safe, chaning the FlyOut content fucks stuff but Avalonia forced my hand. Too bad!

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                // Not Soviet Russia, short of "Cut Copy Paste". Don't laugh.
                //var _ccp = menu.Children[0] as StackPanel;
                //(_ccp.Children[0] as Avalonia.Controls.Button).Click += cutbutton_click;
                //(_ccp.Children[1] as Avalonia.Controls.Button).Click += copybutton_click;

                //(_ccp.Children[2] as Avalonia.Controls.Button).Click += pastebutton_click;

                // New Window
                (menu.Children[1] as Avalonia.Controls.Button).Click += NewWindow;
                (menu.Children[2] as Avalonia.Controls.Button).Click += NewIncWindow;

                // features panel
                var _featurespanel = menu.Children[0] as StackPanel;
                var _panel = (_featurespanel.Children[0] as Panel);
                var mutebutton = _panel.Children[0] as Avalonia.Controls.Button;
                mutebutton.Bind(IsVisibleProperty, IsUnmuted);
                mutebutton.Click += mutebutton_click;
                var unmutebutton = _panel.Children[1] as Avalonia.Controls.Button;
                unmutebutton.Bind(IsVisibleProperty, IsMuted);
                unmutebutton.Click += unmutebutton_click;
                (_featurespanel.Children[1] as Avalonia.Controls.Button).Click += printbutton_click;
                (_featurespanel.Children[1] as Avalonia.Controls.Button).Click += screenshotbutton_click;
                (_featurespanel.Children[1] as Avalonia.Controls.Button).Click += savebutton_click;
                (_featurespanel.Children[1] as Avalonia.Controls.Button).Click += devtoolsbutton_click;

                var _grid = menu.Children[3] as Grid;
                FindCount = (_grid.Children[2] as TextBlock);
                (_grid.Children[1] as Avalonia.Controls.Button).Click += findnextbutton_click;
                findText = (_grid.Children[0] as TextBox);
                findText.PropertyChanged += FindText_PropertyChanged;
                var _mCase = (_grid.Children[3] as Avalonia.Controls.CheckBox);
                _mCase.Checked += MatchCaseChecked;
                _mCase.Unchecked += MatchCaseUnchecked;

                var _zoom = menu.Children[4] as StackPanel;
                ZoomLevel = _zoom.Children[1] as TextBlock;
                var zoomin = (_zoom.Children[2] as Avalonia.Controls.Button);
                zoomin.Bind(IsEnabledProperty, CanZoomIn);
                zoomin.Click += zoominbutton_click;
                var zoomout = (_zoom.Children[0] as Avalonia.Controls.Button);
                zoomout.Bind(IsEnabledProperty, CanZoomOut);
                zoomout.Click += zoomoutbutton_click;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }

            this.KeyDown += TabWindow_KeyDown;
            this.KeyUp += TabWindow_KeyUp;

            var dockPanel = ContentGrid.FindControl<DockPanel>("cefpanel");
            favoritesmenu = ContentGrid.FindControl<Avalonia.Controls.Menu>("Favorites");
            other_bookmarks = favoritesmenu.FindControl<Avalonia.Controls.MenuItem>("other_bookmarks");

            if (dockPanel is null)
            {
                throw new Exception("The panel named \"cefpanel\" that should host CEF was missing. (Is it removed from TabWindow.axaml file?)");
            }

            Handlers.YorotWebView webView = new(this)
            {
                Window = this,
                Background = Avalonia.Media.Brush.Parse("#FFFFFF"),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                IsVisible = true,
            };

            webView.Glue.Window = this;
            webView.BrowserCreated += (sender, e) => { webView.Navigate(_startUrl); };
            webView.LoadError += WebView1_LoadError;
            webView.Navigated += WebView_Navigated;
            webView.Navigating += WebView1_Navigating;
            webView.AddressChange += WebView1_AddressChange;
            webView.DocumentTitleChanged += WebView_DocumentTitleChanged;
            dockPanel.Children.Add(webView);
            webView1 = webView;

            redirectTo(_startUrl, "");

            RefreshFavorites(true);
        }

        private void backforwarditem_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.MenuItem item && item.Tag is Session session && SessionSystem != null && webView1 != null)
            {
                SessionSystem.SelectedIndex = SessionSystem.Sessions.IndexOf(session);
                SessionSystem.SelectedSession = session;
                webView1.Navigate(session.Url);
            }
        }

        private void WebView1_AddressChange(object? sender, CefNet.AddressChangeEventArgs e)
        {
            if (SessionSystem != null && e.IsMainFrame && SessionSystem.Sessions.Count != 0)
            {
                url = e.Url;
                if (e.Url != SessionSystem.Sessions[SessionSystem.Sessions.Count - 1].ToString())
                {
                    redirectTo(e.Url, title);
                }
            }
        }

        private void TabWindow_KeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.LeftShift || e.Key == Avalonia.Input.Key.RightShift)
            {
                shiftPressed = false;
            }
        }

        private bool shiftPressed = false;

        private void TabWindow_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.LeftShift || e.Key == Avalonia.Input.Key.RightShift)
            {
                shiftPressed = true;
            }
        }

        private string CachedFavs = "";

        private void RefreshFavorites(bool force = false)
        {
            if (YorotGlobal.Main != null && favoritesmenu != null && tbUrl != null)
            {
                if (force || (YorotGlobal.Main.CurrentSettings.FavManager.ToXml() != CachedFavs))
                {
                    favoritesmenu.IsVisible = YorotGlobal.Main.CurrentSettings.FavManager.ShowFavorites;
                    CachedFavs = YorotGlobal.Main.CurrentSettings.FavManager.ToXml();
                    if ((favoritesmenu != null && favoritesmenu.Items is AvaloniaList<object> list))
                    {
                        list.Clear();
                        if (other_bookmarks != null)
                        {
                            list.Add(other_bookmarks);
                        }
                        var favbars = YorotGlobal.Main.CurrentSettings.FavManager.Favorites.FindAll(it => it.Name == "FavBar");
                        if (favbars.Count > 0)
                        {
                            for (int i = 0; i < favbars[0].Favorites.Count; i++)
                            {
                                var fav = favbars[0].Favorites[i];
                                Avalonia.Controls.MenuItem item = new();
                                item.Bind(BackgroundProperty, tbUrl.GetBindingObservable(BackgroundProperty));
                                item.Bind(ForegroundProperty, tbUrl.GetBindingObservable(ForegroundProperty));
                                item.Header = fav.Text;
                                item.Tag = fav;
                                //item.Icon = fav.Icon; // TODO
                                list.Add(item);
                                if (fav is not YorotFavorite && fav is YorotFavFolder)
                                {
                                    AddFavorites(item, fav);
                                }
                                else
                                {
                                    item.Click += favitem_Click;
                                }
                            }
                        }
                        else
                        {
                            YorotGlobal.Main.CurrentSettings.FavManager.Favorites.Add(new YorotFavFolder(YorotGlobal.Main.CurrentSettings.FavManager, "FavBar", "Favorites Bar"));
                        }
                    }
                    if (other_bookmarks != null && other_bookmarks.Items is AvaloniaList<object> oblist)
                    {
                        for (int i = 0; i < YorotGlobal.Main.CurrentSettings.FavManager.Favorites.Count; i++)
                        {
                            var fav = YorotGlobal.Main.CurrentSettings.FavManager.Favorites[i];
                            if (fav.Name != "FavBar")
                            {
                                Avalonia.Controls.MenuItem item = new()
                                {
                                    Header = fav.Text,
                                    Tag = fav
                                };
                                item.Bind(BackgroundProperty, tbUrl.GetBindingObservable(BackgroundProperty));
                                item.Bind(ForegroundProperty, tbUrl.GetBindingObservable(ForegroundProperty));
                                //item.Icon = fav.Icon; // TODO
                                oblist.Add(item);
                                if (fav is not YorotFavorite && fav is YorotFavFolder)
                                {
                                    AddFavorites(item, fav);
                                }
                                else
                                {
                                    item.Click += favitem_Click;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AddFavorites(Avalonia.Controls.MenuItem item, YorotFavFolder favFolder)
        {
            if (item.Items is AvaloniaList<object> list)
            {
                for (int i = 0; i < favFolder.Favorites.Count; i++)
                {
                    Avalonia.Controls.MenuItem newitem = new();
                    var fav = favFolder.Favorites[i];
                    newitem.Header = fav.Text;
                    newitem.Tag = fav;
                    //newitem.Icon = fav.Icon; // TODO
                    list.Add(newitem);
                    if (fav is not YorotFavorite && fav is YorotFavFolder)
                    {
                        AddFavorites(newitem, fav);
                    }
                    else
                    {
                        newitem.Click += favitem_Click;
                    }
                }
            }
        }

        private void favitem_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (webView1 != null && sender is Avalonia.Controls.MenuItem item && item.Tag is YorotFavorite fav)
            {
                webView1.Navigate(fav.Url);
            }
        }

        private YorotWebView? webView1;

        private void WebView_DocumentTitleChanged(object? sender, CefNet.DocumentTitleChangedEventArgs e)
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(new System.Action(() =>
            {
                if (Parent is DockPanel dockPanel && dockPanel.Parent is TabViewItem tabItem && SessionSystem != null && YorotGlobal.Main != null)
                {
                    int si = SessionSystem.SelectedIndex;
                    if (si != -1)
                    {
                        if (SessionSystem.Sessions[si].Url == url)
                        {
                            YorotBrowserWebSource source = YorotGlobal.Main.GetWebSource(url);
                            if (source != null)
                            {
                                if (!source.IgnoreOnSessionList)
                                {
                                    SessionSystem.Sessions[si].Title = e.Title;
                                }
                            }
                            else
                            {
                                SessionSystem.Sessions[si].Title = e.Title;
                            }
                        }
                    }
                    title = e.Title;
                    tabItem.Header = e.Title;
                }
            }), Avalonia.Threading.DispatcherPriority.Normal);
        }

        private void WebView1_LoadError(object? sender, CefNet.LoadErrorEventArgs e)
        {
            if (webView1 != null && e.Frame.IsMain)
            {
                webView1.Navigate("yorot://error?e=" + e.ErrorCode + "&u=" + e.FailedUrl);
            }
        }

        private void WebView1_Navigating(object? sender, CefNet.BeforeBrowseEventArgs e)
        {
            if (e.Frame.IsMain && IsNavigating != null && IsNavigated != null)
            {
                IsNavigated.OnNext(false);
                IsNavigating.OnNext(true);
                IsPageSafe.OnNext(true);
                IsPageUsedCookie.OnNext(false);
                IsPageUnsafe.OnNext(false);
            }
        }

        private void cutbutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (webView1 != null)
            {
                webView1.GetMainFrame().Cut();
            }
        }

        private void copybutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (webView1 != null)
            {
                webView1.GetMainFrame().Copy();
            }
        }

        private void pastebutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (webView1 != null)
            {
                webView1.GetMainFrame().Paste();
            }
        }

        private void printbutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (webView1 != null)
            {
                webView1.Print();
            }
        }

        private void screenshotbutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (webView1 != null)
            {
                // TODO: Find a way to take a screnshot of page
                //webView1.GetMainFrame().TakeAScreenShot();
            }
        }

        private void devtoolsbutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (webView1 != null)
            {
                webView1.ShowDevTools();
            }
        }

        public TextBlock? FindCount;
        public TextBlock? ZoomLevel;
        private bool Searching = false;
        private TextBox? findText;
        private bool findNext = false;
        private bool matchCase = false;

        private void findbackbutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (webView1 != null && findText != null)
            {
                Searching = true;
                webView1.Find(findText.Text, false, matchCase, findNext);
            }
        }

        private int zoomLevel = 0; // For some reason, the web view's ZoomLevel is broken, always showing up 0.

        private void zoominbutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (webView1 != null)
            {
                if (zoomLevel < 500)
                {
                    if (zoomLevel < 0)
                    {
                        zoomLevel += 10;
                    }
                    else if (zoomLevel < 300)
                    {
                        zoomLevel += 25;
                    }
                    else if (zoomLevel >= 300)
                    {
                        zoomLevel += 50;
                    }

                    CanZoomOut.OnNext(true);
                    webView1.ZoomLevel = ((double)zoomLevel / 100);
                    if (ZoomLevel != null)
                    {
                        ZoomLevel.Text = (zoomLevel + 100) + "%";
                    }

                    if (zoomLevel >= 500) { CanZoomIn.OnNext(false); }
                }
                else
                {
                    CanZoomIn.OnNext(false);
                }
            }
        }

        private void zoomoutbutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (webView1 != null)
            {
                if (zoomLevel > -100)
                {
                    if (zoomLevel > 300)
                    {
                        zoomLevel -= 50;
                    }
                    else if (zoomLevel > 0)
                    {
                        zoomLevel -= 25;
                    }
                    else if (zoomLevel <= 0)
                    {
                        zoomLevel -= 10;
                    }

                    CanZoomIn.OnNext(true);
                    webView1.ZoomLevel = ((double)zoomLevel / 100);
                    if (ZoomLevel != null)
                    {
                        ZoomLevel.Text = (zoomLevel + 100) + "%";
                    }

                    if (zoomLevel <= -100)
                    {
                        CanZoomOut.OnNext(false);
                    }
                }
                else
                {
                    CanZoomOut.OnNext(false);
                }
            }
        }

        private void findnextbutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (webView1 != null && findText != null)
            {
                if (string.IsNullOrWhiteSpace(findText.Text))
                {
                    Searching = false;
                    webView1.StopFinding(true);
                }
                else
                {
                    Searching = true;
                    webView1.Find(findText.Text, true, matchCase, true);
                }
            }
        }

        private void MatchCaseChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.CheckBox checkBox && checkBox.IsChecked.HasValue)
            {
                matchCase = checkBox.IsChecked.Value;
                if (webView1 != null && Searching && findText != null && Searching)
                {
                    webView1.Find(findText.Text, false, matchCase, findNext);
                }
            }
        }

        private void FindText_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == TextBox.TextProperty && webView1 != null && findText != null)
            {
                if (!string.IsNullOrWhiteSpace(findText.Text))
                {
                    if (!Searching)
                    {
                        Searching = true;
                        webView1.Find(findText.Text, true, matchCase, true);
                    }
                    else
                    {
                        Searching = true;
                        webView1.Find(findText.Text, true, matchCase, findNext);
                    }
                }
                else
                {
                    webView1.StopFinding(true);
                    Searching = false;
                }
            }
        }

        private void MatchCaseUnchecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MatchCaseChecked(sender, e);
        }

        private void NewWindow(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (YorotGlobal.Main != null)
            {
                YorotGlobal.Main.MainForm.NewWindow();
            }
        }

        private void NewIncWindow(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().ProcessName, "-i");
        }

        private async void savebutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (webView1 != null && YorotGlobal.Main != null)
            {
                var task = webView1.GetMainFrame().GetSourceAsync(System.Threading.CancellationToken.None);
                if (task.IsCompletedSuccessfully)
                {
                    RunSaveFileDialog(YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.SaveFileDialog"),
                                new string[] { "html" },
                                new Action<string>((filename) =>
                                {
                                    HTAlt.Tools.WriteFile(filename, task.Result, System.Text.Encoding.UTF8);
                                }
                                ));
                }
            }
        }

        private void mutebutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (webView1 != null && IsMuted != null && IsUnmuted != null)
            {
                webView1.AudioMuted = true;
                IsMuted.OnNext(true);
                IsUnmuted.OnNext(false);
            }
        }

        private void unmutebutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (webView1 != null && IsMuted != null && IsUnmuted != null)
            {
                webView1.AudioMuted = false;
                IsMuted.OnNext(false);
                IsUnmuted.OnNext(true);
            }
        }

        private void openmenu(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.Control cntrl && cntrl.ContextFlyout != null)
            {
                cntrl.ContextFlyout.ShowAt(cntrl, true);
            }
        }

        private void WebView_Navigated(object? sender, CefNet.NavigatedEventArgs e)
        {
            if (YorotGlobal.Main != null && CanZoomIn != null && CanZoomOut != null && SessionSystem != null && webView1 != null && tbUrl != null && e.Frame.IsMain && IsNavigating != null && CanGoBack != null && CanGoForward != null && IsNavigated != null && IsFavorited != null && IsNotFavorited != null)
            {
                IsNavigated.OnNext(true);
                IsNavigating.OnNext(false);
                CanZoomIn.OnNext(true);
                CanZoomOut.OnNext(true);
                zoomLevel = 0;
                url = e.Url;
                CanGoBack.OnNext(SessionSystem.CanGoBack);
                CanGoForward.OnNext(SessionSystem.CanGoForward);
                tbUrl.Text = e.Url;
                var favorited = YorotGlobal.Main.CurrentSettings.FavManager.isFavorited(e.Url);
                IsFavorited.OnNext(favorited);
                IsNotFavorited.OnNext(!favorited);
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
            if (sender != null && SessionSystem != null && webView1 != null && SessionSystem.CanGoBack)
            {
                bypassThisDeletion = true;
                SessionSystem.GoBack();
            }
        }

        private void goforward(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender != null && SessionSystem != null && webView1 != null && SessionSystem.CanGoForward)
            {
                bypassThisDeletion = true;
                SessionSystem.GoForward();
            }
        }

        private void gohome(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender != null && webView1 != null && webView1.CanGoForward && YorotGlobal.Main != null)
            {
                redirectTo(YorotGlobal.Main.CurrentSettings.HomePage, "");
                webView1.Navigate(YorotGlobal.Main.CurrentSettings.HomePage);
            }
        }

        private void reload(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (webView1 is null) { return; }
            if (shiftPressed)
            {
                webView1.ReloadIgnoreCache();
            }
            else
            {
                webView1.Reload();
            }
        }

        public bool bypassThisDeletion = false;
        public bool indexChanged = false;

        public void redirectTo(string url, string title = "")
        {
            if (SessionSystem != null)
            {
                SessionSystem.SkipAdd = bypassThisDeletion;
                bypassThisDeletion = false;
                SessionSystem.Add(url, title);
            }
        }

        private void stop(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (webView1 is null) { return; }
            webView1.Stop();
        }

        private void gobutton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (webView1 is null || YorotGlobal.Main is null) { return; }
            if (tbUrl != null)
            {
                if (HTAlt.Tools.ValidUrl(tbUrl.Text, new string[] { "yorot" }, false))
                {
                    redirectTo(tbUrl.Text, "");
                    webView1.Navigate(tbUrl.Text);
                }
                else
                {
                    var searchUrl = YorotGlobal.Main.CurrentSettings.SearchEngine.Search(tbUrl.Text);
                    redirectTo(searchUrl, "");
                    webView1.Navigate(searchUrl);
                }
            }
        }

        private string title = "";
        private string url = "";

        private void favorite(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (IsFavorited is null || YorotGlobal.Main is null || IsNotFavorited is null) { return; }
            var favbars = YorotGlobal.Main.CurrentSettings.FavManager.Favorites.FindAll(it => it.Name == "FavBar");
            if (favbars.Count > 0)
            {
                var favbar = favbars[0];
                favbar.Favorites.Add(new YorotFavorite(favbar, url, title));
            }
            else
            {
                var favbar = new YorotFavFolder(YorotGlobal.Main.CurrentSettings.FavManager, "FavBar", "Favorites Bar");
                YorotGlobal.Main.CurrentSettings.FavManager.Favorites.Add(favbar);
                favbar.Favorites.Add(new YorotFavorite(favbar, url, title));
            }
            RefreshFavorites(true);
            IsFavorited.OnNext(true);
            IsNotFavorited.OnNext(false);
        }

        private void unfavorite(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (YorotGlobal.Main is null || IsFavorited is null || tbUrl is null || IsNotFavorited is null) { return; }
            var favs = YorotGlobal.Main.CurrentSettings.FavManager.GetFavorite(tbUrl.Text);
            if (favs.Count > 0)
            {
                favs[0].ParentFolder.Favorites.Remove(favs[0]);
                RefreshFavorites(true);
                IsFavorited.OnNext(true);
                IsNotFavorited.OnNext(false);
            }
        }

        private async void RunSaveFileDialog(string title, string[] filetypes, Action<string> OnSuccess)
        {
            if (YorotGlobal.Main != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    Title = YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.SaveFileDialog")
                };
                List<FileDialogFilter> Filters = new List<FileDialogFilter>();
                FileDialogFilter filter = new FileDialogFilter();

                for (int i = 0; i < filetypes.Length; i++)
                {
                    List<string> extension = new List<string>();
                    extension.Add(filetypes[i]);
                    filter.Extensions = extension;
                    filter.Name = YorotGlobal.Main.CurrentLanguage.GetItemText("FileTypes." + filetypes[i].ToUpperInvariant());
                    Filters.Add(filter);
                }
                saveFileDialog.Filters = Filters;

                saveFileDialog.DefaultExtension = filetypes[0];

                saveFileDialog.Directory = YorotGlobal.Main.CurrentSettings.DownloadManager.DownloadFolder;

                var filename = await saveFileDialog.ShowAsync(YorotGlobal.Main.MainForm);

                if (!string.IsNullOrWhiteSpace(filename))
                {
                    OnSuccess(filename);
                }
            }
        }

        private Avalonia.Controls.Flyout? RightClickMenu;
        private YorotGlue.ContextMenuParams? lastRCMParams;

        internal void ShowContextMenu(YorotGlue.ContextMenuParams menuParams, CefFrame frame)
        {
            if (menuParams is null) { throw new ArgumentNullException(nameof(menuParams)); }
            if (frame is null) { throw new ArgumentNullException(nameof(frame)); }
            if (webView1 != null && YorotGlobal.Main != null && mainWindow != null)
            {
                if (RightClickMenu is null || lastRCMParams is null || !lastRCMParams.Equals(menuParams))
                {
                    lastRCMParams = menuParams;
                    RightClickMenu = new() { ShowMode = FlyoutShowMode.TransientWithDismissOnPointerMoveAway };

                    var _content = new StackPanel() { Orientation = Avalonia.Layout.Orientation.Vertical, };

                    RightClickMenu.Content = _content;

                    Avalonia.Controls.MenuItem back = new()
                    {
                        Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.Back"),
                    };
                    back.Click += (sender, e) => { goback(this, null); RightClickMenu.Hide(); };
                    _content.Children.Add(back);

                    Avalonia.Controls.MenuItem forward = new()
                    {
                        Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.Forward"),
                    };
                    forward.Click += (sender, e) => { goforward(this, null); RightClickMenu.Hide(); };
                    _content.Children.Add(forward);

                    Avalonia.Controls.MenuItem refresh = new()
                    {
                        Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.Refresh"),
                    };
                    refresh.Click += (sender, e) => { reload(this, null); RightClickMenu.Hide(); };
                    _content.Children.Add(refresh);

                    Avalonia.Controls.MenuItem refreshNoCache = new()
                    {
                        Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.RefreshNoCache"),
                    };
                    refreshNoCache.Click += (sender, e) => { shiftPressed = true; reload(this, null); shiftPressed = false; RightClickMenu.Hide(); };
                    _content.Children.Add(refreshNoCache);

                    Avalonia.Controls.MenuItem stopbutton = new()
                    {
                        Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.Stop"),
                    };
                    stopbutton.Click += (sender, e) => { stop(this, null); RightClickMenu.Hide(); };
                    _content.Children.Add(stopbutton);

                    Avalonia.Controls.MenuItem selectall = new()
                    {
                        Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.SelectAll"),
                    };
                    selectall.Click += (sender, e) => { frame.SelectAll(); RightClickMenu.Hide(); };
                    _content.Children.Add(selectall);

                    _content.Children.Add(new Avalonia.Controls.Button() { IsEnabled = false, Height = 5, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch });

                    if (menuParams.IsTextSelected)
                    {
                        // TODO: Add TTS Read

                        Avalonia.Controls.MenuItem searchOn = new()
                        {
                            Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.SearchOn").Replace("[Parameter.Word]", menuParams.SelectedText).Replace("[Parameter.SE]", YorotGlobal.Main.CurrentSettings.SearchEngine.Name),
                        };
                        searchOn.Click += (sender, e) => { mainWindow.NewTab(YorotGlobal.Main.CurrentSettings.SearchEngine.Search(menuParams.SelectedText), switchTo: true); RightClickMenu.Hide(); };
                        _content.Children.Add(searchOn);

                        if (menuParams.IsEditable)
                        {
                            _content.Children.Add(new Avalonia.Controls.Button() { IsEnabled = false, Height = 5, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch });
                        }

                        Avalonia.Controls.MenuItem copy = new()
                        {
                            Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.Copy"),
                        };
                        copy.Click += (sender, e) => { frame.Copy(); RightClickMenu.Hide(); };
                        _content.Children.Add(copy);

                        if (!menuParams.IsEditable)
                        {
                            _content.Children.Add(new Avalonia.Controls.Button() { IsEnabled = false, Height = 5, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch });
                        }
                    }

                    if (menuParams.IsEditable)
                    {
                        Avalonia.Controls.MenuItem cut = new()
                        {
                            Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.Cut"),
                        };
                        cut.Click += (sender, e) => { frame.Cut(); RightClickMenu.Hide(); };
                        _content.Children.Add(cut);

                        Avalonia.Controls.MenuItem paste = new()
                        {
                            Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.Paste"),
                        };
                        paste.Click += (sender, e) => { frame.Paste(); RightClickMenu.Hide(); };
                        _content.Children.Add(paste);

                        Avalonia.Controls.MenuItem undo = new()
                        {
                            Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.Undo"),
                        };
                        undo.Click += (sender, e) => { frame.Undo(); RightClickMenu.Hide(); };
                        _content.Children.Add(undo);

                        Avalonia.Controls.MenuItem redo = new()
                        {
                            Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.Redo"),
                        };
                        redo.Click += (sender, e) => { frame.Redo(); RightClickMenu.Hide(); };
                        _content.Children.Add(redo);

                        Avalonia.Controls.MenuItem del = new()
                        {
                            Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.Delete"),
                        };
                        del.Click += (sender, e) => { frame.Del(); RightClickMenu.Hide(); };
                        _content.Children.Add(del);

                        _content.Children.Add(new Avalonia.Controls.Button() { IsEnabled = false, Height = 5, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch });
                    }

                    if (menuParams.IsImage)
                    {
                        Avalonia.Controls.MenuItem openImage = new()
                        {
                            Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.OpenImageInNewTab"),
                        };
                        openImage.Click += (sender, e) => { mainWindow.NewTab(menuParams.SourceUrl, switchTo: true); RightClickMenu.Hide(); };
                        _content.Children.Add(openImage);

                        Avalonia.Controls.MenuItem copySourceUrl = new()
                        {
                            Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.CopyImageUrl"),
                        };
                        copySourceUrl.Click += (sender, e) => { if (Application.Current != null && Application.Current.Clipboard != null) { Application.Current.Clipboard.SetTextAsync(menuParams.SourceUrl); } RightClickMenu.Hide(); };
                        _content.Children.Add(copySourceUrl);

                        Avalonia.Controls.MenuItem downloadImage = new()
                        {
                            Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.DownloadImage"),
                        };
                        downloadImage.Click += (sender, e) => { webView1.DownloadFile(menuParams.SourceUrl); RightClickMenu.Hide(); };
                        _content.Children.Add(downloadImage);

                        _content.Children.Add(new Avalonia.Controls.Button() { IsEnabled = false, Height = 5, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch });
                    }

                    if (menuParams.IsSelectedUrl)
                    {
                        Avalonia.Controls.MenuItem openInNewTab = new()
                        {
                            Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.OpenLinkInNewTab"),
                        };
                        openInNewTab.Click += (sender, e) => { mainWindow.NewTab(menuParams.LinkUrl, switchTo: true); RightClickMenu.Hide(); };
                        _content.Children.Add(openInNewTab);

                        Avalonia.Controls.MenuItem openInBackground = new()
                        {
                            Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.OpenLinkInBackground"),
                        };
                        openInBackground.Click += (sender, e) => { mainWindow.NewTab(menuParams.LinkUrl, switchTo: false); RightClickMenu.Hide(); };
                        _content.Children.Add(openInBackground);

                        Avalonia.Controls.MenuItem openInNewWindow = new()
                        {
                            Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.OpenLinkInNewWindow"),
                        };
                        openInNewWindow.Click += (sender, e) => { mainWindow.NewWindow(menuParams.LinkUrl); RightClickMenu.Hide(); };
                        _content.Children.Add(openInNewWindow);

                        Avalonia.Controls.MenuItem openInNewIncWindow = new()
                        {
                            Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.OpenLinkInNewIncWindow"),
                        };
                        openInNewIncWindow.Click += (sender, e) =>
                        {
                            System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().ProcessName,
                                $"-i \"{menuParams.LinkUrl}\"");
                            RightClickMenu.Hide();
                        };
                        _content.Children.Add(openInNewIncWindow);

                        Avalonia.Controls.MenuItem saveAs = new()
                        {
                            Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.SaveAs"),
                        };
                        saveAs.Click += (sender, e) =>
                        {
                            webView1.DownloadFile(menuParams.LinkUrl);
                            RightClickMenu.Hide();
                        };
                        _content.Children.Add(saveAs);

                        Avalonia.Controls.MenuItem copyLinkAddress = new()
                        {
                            Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.CopyLink"),
                        };
                        copyLinkAddress.Click += (sender, e) => { if (Application.Current != null && Application.Current.Clipboard != null) { Application.Current.Clipboard.SetTextAsync(menuParams.LinkUrl); } RightClickMenu.Hide(); };
                        _content.Children.Add(copyLinkAddress);

                        _content.Children.Add(new Avalonia.Controls.Button() { IsEnabled = false, Height = 5, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch });
                    }

                    Avalonia.Controls.MenuItem savePage = new()
                    {
                        Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.SavePageAs"),
                    };
                    savePage.Click += (sender, e) =>
                    {
                        var task = frame.GetSourceAsync(System.Threading.CancellationToken.None);
                        if (task.IsCompletedSuccessfully)
                        {
                            RunSaveFileDialog(YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.SaveFileDialog"),
                                new string[] { "html" },
                                new Action<string>((filename) =>
                                {
                                    HTAlt.Tools.WriteFile(filename, task.Result, System.Text.Encoding.UTF8);
                                }
                                ));
                        }
                        RightClickMenu.Hide();
                    };
                    _content.Children.Add(savePage);

                    Avalonia.Controls.MenuItem printPage = new()
                    {
                        Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.Print"),
                    };
                    printPage.Click += (sender, e) =>
                    {
                        webView1.Print();
                        RightClickMenu.Hide();
                    };
                    _content.Children.Add(printPage);

                    Avalonia.Controls.MenuItem printPdf = new()
                    {
                        Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.PrintToPDF"),
                    };
                    printPdf.Click += (sender, e) =>
                    {
                        RunSaveFileDialog(YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.SaveFileDialog"),
                                new string[] { "pdf" },
                                new Action<string>((filename) =>
                                {
                                    webView1.PrintToPdf(filename, new CefPdfPrintSettings()
                                    {
                                        // TODO
                                    });
                                }
                                ));
                        RightClickMenu.Hide();
                    };
                    _content.Children.Add(printPdf);

                    Avalonia.Controls.MenuItem viewSource = new()
                    {
                        Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.ViewSource"),
                    };
                    viewSource.Click += (sender, e) => { frame.ViewSource(); RightClickMenu.Hide(); };
                    _content.Children.Add(viewSource);

                    Avalonia.Controls.MenuItem devtools = new()
                    {
                        Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.ShowDevTools"),
                    };
                    devtools.Click += (sender, e) => { webView1.ShowDevTools(); RightClickMenu.Hide(); };
                    _content.Children.Add(devtools);
                }
                RightClickMenu.ShowAt(webView1, true);
            }
        }
    }
}