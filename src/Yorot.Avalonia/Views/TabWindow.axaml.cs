using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using FluentAvalonia.UI.Controls;
using System;
using System.Collections.Generic;
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

        private string _startUrl = "yorot://newtab";

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

        private MainWindow? mainWindow;

        public SessionSystem? SessionSystem;

        private Grid? ContentGrid;
        private Avalonia.Controls.Menu? favoritesmenu;
        private TextBox? tbUrl;
        private System.Reactive.Subjects.Subject<bool>? IsNavigated;
        private System.Reactive.Subjects.Subject<bool>? IsNavigating;
        private System.Reactive.Subjects.Subject<bool>? CanGoBack;
        private System.Reactive.Subjects.Subject<bool>? CanGoForward;
        private System.Reactive.Subjects.Subject<bool>? IsFavorited;
        private System.Reactive.Subjects.Subject<bool>? IsNotFavorited;
        private System.Reactive.Subjects.Subject<bool>? IsMuted;
        private System.Reactive.Subjects.Subject<bool>? IsUnmuted;
        private Avalonia.Controls.MenuItem? other_bookmarks;

        private void InitializeComponent()
        {
            SessionSystem = new(YorotGlobal.Main);
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

            IsNavigating.OnNext(true);
            IsNavigated.OnNext(false);
            CanGoBack.OnNext(false);
            CanGoForward.OnNext(false);
            IsFavorited.OnNext(false);
            IsNotFavorited.OnNext(true);
            IsMuted.OnNext(false);
            IsUnmuted.OnNext(true);

            stackPanel1.FindControl<Avalonia.Controls.Button>("reload").Bind(IsVisibleProperty, IsNavigated);
            stackPanel1.FindControl<Avalonia.Controls.Button>("stop").Bind(IsVisibleProperty, IsNavigating);
            stackPanel1.FindControl<Avalonia.Controls.Button>("goback").Bind(IsEnabledProperty, CanGoBack);
            stackPanel1.FindControl<Avalonia.Controls.Button>("goforward").Bind(IsEnabledProperty, CanGoForward);
            stackPanel1.FindControl<Avalonia.Controls.Button>("favoritebutton").Bind(IsVisibleProperty, IsNotFavorited);
            stackPanel1.FindControl<Avalonia.Controls.Button>("favoritedbutton").Bind(IsVisibleProperty, IsFavorited);
            var dotmenu = stackPanel1.FindControl<Avalonia.Controls.Button>("dotmenu").ContextFlyout;

            dotmenu.Placement = FlyoutPlacementMode.Bottom;
            dotmenu.ShowMode = FlyoutShowMode.Standard;

            if (dotmenu is Flyout dotflayout && dotflayout.Content is StackPanel menu)
            {
                // None of these are safe, chaning the FlyOut content fucks stuff but Avalonia forced my hand. Too bad!

                // Not Soviet Russia, short of "Cut Copy Paste". Don't laugh.
                var _ccp = menu.Children[0] as StackPanel;
                (_ccp.Children[0] as Avalonia.Controls.Button).Click += cutbutton_click;
                (_ccp.Children[1] as Avalonia.Controls.Button).Click += copybutton_click;
                (_ccp.Children[2] as Avalonia.Controls.Button).Click += pastebutton_click;

                // New Window
                (menu.Children[2] as Avalonia.Controls.Button).Click += NewWindow;
                (menu.Children[3] as Avalonia.Controls.Button).Click += NewIncWindow;

                // features panel
                var _featurespanel = menu.Children[1] as StackPanel;
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

                var _grid = menu.Children[4] as Grid;
                FindCount = (_grid.Children[3] as TextBlock);
                (_grid.Children[2] as Avalonia.Controls.Button).Click += findnextbutton_click;
                (_grid.Children[0] as Avalonia.Controls.Button).Click += findbackbutton_click;
                (_grid.Children[1] as TextBox).PropertyChanged += Search_PropertyChanged;
                var _mCase = (_grid.Children[4] as Avalonia.Controls.CheckBox);
                _mCase.Checked += MatchCaseChecked;
                _mCase.Unchecked += MatchCaseUnchecked;

                var _zoom = menu.Children[5] as StackPanel;
                ZoomLevel = _zoom.Children[1] as TextBlock;
                (_zoom.Children[0] as Avalonia.Controls.Button).Click += zoominbutton_click;
                (_zoom.Children[2] as Avalonia.Controls.Button).Click += zoomoutbutton_click;
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
                Background = Avalonia.Media.Brush.Parse("#FFFFFF"),
                InitialUrl = _startUrl,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                IsVisible = true,
            };
            webView.LoadError += WebView1_LoadError;
            webView.Navigated += WebView_Navigated;
            webView.Navigating += WebView1_Navigating;
            webView.DocumentTitleChanged += WebView_DocumentTitleChanged;
            dockPanel.Children.Add(webView);
            webView1 = webView;

            RefreshFavorites(true);
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
                if (Parent is DockPanel dockPanel && dockPanel.Parent is TabViewItem tabItem)
                {
                    title = e.Title;
                    tabItem.Header = e.Title;
                }
            }), Avalonia.Threading.DispatcherPriority.Normal);
        }

        private void WebView1_LoadError(object? sender, CefNet.LoadErrorEventArgs e)
        {
            if (webView1 is null) { return; }
            if (e.Frame.IsMain)
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
        private string findText = "";
        private bool findNext = false;
        private bool matchCase = false;

        private void findbackbutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (webView1 != null)
            {
                Searching = true;
                webView1.Find(findText, false, matchCase, findNext);
            }
        }

        private void zoominbutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (webView1 != null)
            {
                if (webView1.ZoomLevel == 0)
                {
                    webView1.ZoomLevel = 125;
                }
                else if (webView1.ZoomLevel < 300)
                {
                    webView1.ZoomLevel += 25;
                }
                if (ZoomLevel != null)
                {
                    ZoomLevel.Text = ((webView1.ZoomLevel * 100) + 100) + "%";
                }
            }
        }

        private void zoomoutbutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (webView1 != null)
            {
                if (webView1.ZoomLevel == 0)
                {
                    webView1.ZoomLevel = 90;
                }
                else if (webView1.ZoomLevel > 0)
                {
                    webView1.ZoomLevel -= 10;
                }
                if (ZoomLevel != null)
                {
                    ZoomLevel.Text = ((webView1.ZoomLevel * 100) + 100) + "%";
                }
            }
        }

        private void findnextbutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (webView1 != null)
            {
                Searching = true;
                webView1.Find(findText, true, matchCase, findNext);
            }
        }

        private void MatchCaseChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.CheckBox checkBox && checkBox.IsChecked.HasValue)
            {
                matchCase = checkBox.IsChecked.Value;
                if (webView1 != null && Searching)
                {
                    webView1.Find(findText, false, matchCase, findNext);
                }
            }
        }

        private void Search_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (sender is TextBox tb && webView1 != null)
            {
                if (!string.IsNullOrWhiteSpace(tb.Text))
                {
                    Searching = true;
                    findText = tb.Text;
                    webView1.Find(findText, true, matchCase, findNext);
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
                var mainform = new MainWindow() { IsVisible = true };
                mainform.Show();
                YorotGlobal.Main.MainForms.Add(mainform);
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
                    SaveFileDialog saveFileDialog = new SaveFileDialog()
                    {
                        Title = YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.SaveFileDialog")
                    };
                    List<FileDialogFilter> Filters = new List<FileDialogFilter>();
                    FileDialogFilter filter = new FileDialogFilter();
                    List<string> extension = new List<string>();
                    extension.Add("html");
                    filter.Extensions = extension;
                    filter.Name = YorotGlobal.Main.CurrentLanguage.GetItemText("FileTypes.HTML");
                    Filters.Add(filter);
                    saveFileDialog.Filters = Filters;

                    saveFileDialog.DefaultExtension = "html";

                    saveFileDialog.Directory = YorotGlobal.Main.CurrentSettings.DownloadManager.DownloadFolder;

                    var filename = await saveFileDialog.ShowAsync(YorotGlobal.Main.MainForm);

                    if (!string.IsNullOrWhiteSpace(filename))
                    {
                        HTAlt.Tools.WriteFile(filename, task.Result, System.Text.Encoding.UTF8);
                    }
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

        private void dotmenu(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.Control cntrl && cntrl.ContextFlyout != null)
            {
                cntrl.ContextFlyout.ShowAt(cntrl);
            }
        }

        private void WebView_Navigated(object? sender, CefNet.NavigatedEventArgs e)
        {
            if (YorotGlobal.Main != null && webView1 != null && tbUrl != null && e.Frame.IsMain && IsNavigating != null && CanGoBack != null && CanGoForward != null && IsNavigated != null && IsFavorited != null && IsNotFavorited != null)
            {
                IsNavigated.OnNext(true);
                IsNavigating.OnNext(false);
                url = e.Url;
                CanGoBack.OnNext(webView1.CanGoBack);
                CanGoForward.OnNext(webView1.CanGoForward);
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
            if (sender != null && webView1 != null && webView1.CanGoForward && YorotGlobal.Main != null)
            {
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
                    webView1.Navigate(tbUrl.Text);
                }
                else
                {
                    webView1.Navigate(YorotGlobal.Main.CurrentSettings.SearchEngine.Search(tbUrl.Text));
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
    }
}