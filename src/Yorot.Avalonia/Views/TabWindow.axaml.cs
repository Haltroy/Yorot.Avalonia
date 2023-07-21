using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CefNet;
using FluentAvalonia.UI.Controls;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using Yorot;
using Yorot_Avalonia.Handlers;

namespace Yorot_Avalonia.Views
{
    public partial class TabWindow : UserControl, IDisposable
    {
        public TabWindow() : this(null)
        {
        }

        public TabWindow(SessionSystem? session)
        {
            if (session == null) { session = YorotGlobal.Main.CurrentSettings.SessionManager.GenerateNew(); }
            SessionSystem = session;
            InitializeComponent();
        }

        public bool IsAlive = false;
        public string _startUrl = "yorot://newtab";
        public string findText = "";

        public YorotSite CurrentSite;

        public MainWindow? mainWindow;

        public SessionSystem? SessionSystem;

        private Grid? ContentGrid;
        public YorotWebView? webView1;
        public bool Searching = false;
        // TODO: Add this function
#pragma warning disable IDE0044 // Add readonly modifier
        public bool findNext = false;
#pragma warning restore IDE0044 // Add readonly modifier
        public bool matchCase = false;

        // For some reason, the web view's ZoomLevel is broken, always showing up 0. So we have to implement ours.
        public int zoomLevel = 0;

        public bool bypassThisDeletion = false;
        public bool indexChanged = false;
        public string title = "";
        public string url = "";

        private Avalonia.Controls.Flyout? RightClickMenu;
        private YorotGlue.ContextMenuParams? lastRCMParams;
        private bool disposedValue;

        // tbUrl

        private string _tbUrl = ""; public string tbUrl
        { get => _tbUrl; set { _tbUrl = value; RefreshMainWindow(); } }

        // IsNavigated
        private bool _IsNavigated = true; public bool IsNavigated

        { get => _IsNavigated; set { _IsNavigated = value; RefreshMainWindow(); } }

        // ZoomLevel
        private string _ZoomLevel = "100%"; public string ZoomLevel

        { get => _ZoomLevel; set { _ZoomLevel = value; RefreshMainWindow(); } }

        // IsNavigating
        private bool _IsNavigating = false; public bool IsNavigating

        { get => _IsNavigating; set { _IsNavigating = value; RefreshMainWindow(); } }

        // CanGoBack
        private bool _CanGoBack = false; public bool CanGoBack

        { get => _CanGoBack; set { _CanGoBack = value; RefreshMainWindow(); } }

        // CanGoForward
        private bool _CanGoForward = false; public bool CanGoForward

        { get => _CanGoForward; set { _CanGoForward = value; RefreshMainWindow(); } }

        // CanZoomIn
        private bool _CanZoomIn = true; public bool CanZoomIn

        { get => _CanZoomIn; set { _CanZoomIn = value; RefreshMainWindow(); } }

        // CanZoomOut
        private bool _CanZoomOut = true; public bool CanZoomOut

        { get => _CanZoomOut; set { _CanZoomOut = value; RefreshMainWindow(); } }

        // IsFavorited
        private bool _IsFavorited = false; public bool IsFavorited

        { get => _IsFavorited; set { _IsFavorited = value; RefreshMainWindow(); } }

        // IsNotFavorited
        private bool _IsNotFavorited = true; public bool IsNotFavorited

        { get => _IsNotFavorited; set { _IsNotFavorited = value; RefreshMainWindow(); } }

        // IsMuted
        private bool _IsMuted = false; public bool IsMuted

        { get => _IsMuted; set { _IsMuted = value; RefreshMainWindow(); } }

        // IsUnmuted
        private bool _IsUnmuted = true; public bool IsUnmuted

        { get => _IsUnmuted; set { _IsUnmuted = value; RefreshMainWindow(); } }

        // IsPageSafe
        private bool _IsPageSafe = true; public bool IsPageSafe

        { get => _IsPageSafe; set { _IsPageSafe = value; RefreshMainWindow(); } }

        // IsPageUsedCookie
        private bool _IsPageUsedCookie = false; public bool IsPageUsedCookie

        { get => _IsPageUsedCookie; set { _IsPageUsedCookie = value; RefreshMainWindow(); } }

        // IsPageUnsafe
        private bool _IsPageUnsafe = false; public bool IsPageUnsafe

        { get => _IsPageUnsafe; set { _IsPageUnsafe = value; RefreshMainWindow(); } }

        //MatchCase
        private bool _MatchCase = false; public bool MatchCase

        { get => _MatchCase; set { _MatchCase = value; RefreshMainWindow(); } }

        // FindCount
        private string _FindCount = ""; public string FindCount

        { get => _FindCount; set { _FindCount = value; RefreshMainWindow(); } }

        private void RefreshMainWindow()
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (mainWindow != null &&
                        mainWindow.tabs != null &&
                        mainWindow.tabs.SelectedItem is TabViewItem item &&
                        item.Content is DockPanel panel &&
                        panel.Children[0] == this &&
                        mainWindow.IsNavigated != null &&
                        mainWindow.IsNavigating != null &&
                        mainWindow.CanGoBack != null &&
                        mainWindow.CanGoForward != null &&
                        mainWindow.CanZoomIn != null &&
                        mainWindow.CanZoomOut != null &&
                        mainWindow.IsFavorited != null &&
                        mainWindow.IsNotFavorited != null &&
                        mainWindow.IsMuted != null &&
                        mainWindow.IsUnmuted != null &&
                        mainWindow.IsPageSafe != null &&
                        mainWindow.IsPageUsedCookie != null &&
                        mainWindow.IsPageUnsafe != null &&
                        mainWindow.ZoomLevel != null &&
                        mainWindow.findText != null &&
                        mainWindow.MatchCase != null)
                {
                    mainWindow.IsNavigated.OnNext(IsNavigated);
                    mainWindow.IsNavigating.OnNext(IsNavigating);
                    mainWindow.CanGoBack.OnNext(CanGoBack);
                    mainWindow.CanGoForward.OnNext(CanGoForward);
                    mainWindow.CanZoomIn.OnNext(CanZoomIn);
                    mainWindow.CanZoomOut.OnNext(CanZoomOut);
                    mainWindow.IsFavorited.OnNext(IsFavorited);
                    mainWindow.IsNotFavorited.OnNext(IsNotFavorited);
                    mainWindow.IsMuted.OnNext(IsMuted);
                    mainWindow.IsUnmuted.OnNext(IsUnmuted);
                    mainWindow.IsPageSafe.OnNext(IsPageSafe);
                    mainWindow.IsPageUsedCookie.OnNext(IsPageUsedCookie);
                    mainWindow.IsPageUnsafe.OnNext(IsPageUnsafe);
                    mainWindow.ZoomLevel.OnNext(ZoomLevel);
                    mainWindow.tbUrl.Text = tbUrl;
                    mainWindow.FindCount.OnNext(FindCount);
                    mainWindow.findText.Text = findText;
                    mainWindow.MatchCase.IsChecked = MatchCase;
                    if (CurrentSite is null)
                    {
                        CurrentSite = YorotGlobal.Main.CurrentSettings.SiteMan.GetSite(string.IsNullOrWhiteSpace(url) ? _startUrl : url);
                    }
                    mainWindow.AllowMic.IsChecked = CurrentSite.Permissions.allowMic.Allowance == YorotPermissionMode.Allow || CurrentSite.Permissions.allowMic.Allowance == YorotPermissionMode.AllowOneTime;
                    mainWindow.AllowCam.IsChecked = CurrentSite.Permissions.allowCam.Allowance == YorotPermissionMode.Allow || CurrentSite.Permissions.allowCam.Allowance == YorotPermissionMode.AllowOneTime;
                    mainWindow.AllowCookies.IsChecked = CurrentSite.Permissions.allowCookies.Allowance == YorotPermissionMode.Allow || CurrentSite.Permissions.allowCookies.Allowance == YorotPermissionMode.AllowOneTime;
                    mainWindow.AllowYS.IsChecked = CurrentSite.Permissions.allowYS.Allowance == YorotPermissionMode.Allow || CurrentSite.Permissions.allowYS.Allowance == YorotPermissionMode.AllowOneTime;
                    mainWindow.AllowPopup.IsChecked = CurrentSite.Permissions.allowPopup.Allowance == YorotPermissionMode.Allow || CurrentSite.Permissions.allowPopup.Allowance == YorotPermissionMode.AllowOneTime;
                    mainWindow.AllowNotif.IsChecked = CurrentSite.Permissions.allowNotif.Allowance == YorotPermissionMode.Allow || CurrentSite.Permissions.allowNotif.Allowance == YorotPermissionMode.AllowOneTime;
                    mainWindow.AllowNotifBoot.IsChecked = CurrentSite.Permissions.startNotifOnBoot;
                    mainWindow.NotifPriority.SelectedIndex = CurrentSite.Permissions.notifPriority + 1;
                }
            }, Avalonia.Threading.DispatcherPriority.Input);
        }

        private void InitializeComponent()
        {
            SessionSystem.LoadPage += (url) => { if (webView1 != null) { webView1.Navigate(url); } };
            SessionSystem.Add(new Session(_startUrl));
            SessionSystem.SelectedSession = SessionSystem[0];
            SessionSystem.SelectedIndex = 0;
            AvaloniaXamlLoader.Load(this);
            ContentGrid = this.FindControl<Grid>("Content");

            var dockPanel = ContentGrid.FindControl<DockPanel>("cefpanel");
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
            webView.BrowserCreated += (sender, e) => { IsAlive = true; webView.ReloadIgnoreCache(); webView.Navigate(_startUrl); };
            webView.LoadError += WebView1_LoadError;
            webView.Navigated += WebView_Navigated;
            webView.Navigating += WebView1_Navigating;
            webView.AddressChange += WebView1_AddressChange;
            webView.DocumentTitleChanged += WebView_DocumentTitleChanged;
            dockPanel.Children.Add(webView);
            webView1 = webView;

            url = _startUrl;
            redirectTo(_startUrl, "");
        }

        private void WebView1_AddressChange(object? sender, CefNet.AddressChangeEventArgs e)
        {
            if (SessionSystem != null && e.IsMainFrame && SessionSystem.Count != 0)
            {
                url = e.Url;
                tbUrl = e.Url;
                if (e.Url != SessionSystem[^1].ToString())
                {
                    redirectTo(e.Url, title);
                }
            }
        }

        private void WebView_DocumentTitleChanged(object? sender, CefNet.DocumentTitleChangedEventArgs e)
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(new System.Action(() =>
            {
                if (Parent is DockPanel dockPanel && dockPanel.Parent is TabViewItem tabItem && SessionSystem != null && YorotGlobal.Main != null)
                {
                    int si = SessionSystem.SelectedIndex;
                    if (si != -1)
                    {
                        if (SessionSystem[si].Url == url)
                        {
                            YorotBrowserWebSource source = YorotGlobal.Main.GetWebSource(url);
                            if (source != null)
                            {
                                if (!source.IgnoreOnSessionList)
                                {
                                    SessionSystem[si].Title = e.Title;
                                }
                            }
                            else
                            {
                                SessionSystem[si].Title = e.Title;
                            }
                        }
                    }
                    CurrentSite.Name = e.Title;
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
            if (e.Frame.IsMain)
            {
                IsNavigated = false;
                IsNavigating = true;
                IsPageSafe = true;
                IsPageUsedCookie = false;
                IsPageUnsafe = false;
            }
        }

        public void backforwarditem_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.MenuItem item && item.Tag is Session session && SessionSystem != null && webView1 != null)
            {
                SessionSystem.SelectedIndex = SessionSystem.IndexOf(session);
                SessionSystem.SelectedSession = session;
                webView1.Navigate(session.Url);
            }
        }

        private void WebView_Navigated(object? sender, CefNet.NavigatedEventArgs e)
        {
            if (YorotGlobal.Main != null && SessionSystem != null && webView1 != null && e.Frame.IsMain)
            {
                IsNavigated = true;
                IsNavigating = false;
                CanZoomIn = true;
                CanZoomOut = true;
                zoomLevel = 0;
                url = e.Url;
                CanGoBack = SessionSystem.CanGoBack;
                CanGoForward = SessionSystem.CanGoForward;
                var favorited = YorotGlobal.Main.CurrentSettings.FavManager.isFavorited(e.Url);
                IsFavorited = favorited;
                IsNotFavorited = !favorited;
                tbUrl = e.Url;
            }
        }

        public void redirectTo(string url, string title = "")
        {
            if (SessionSystem != null)
            {
                SessionSystem.SkipAdd = bypassThisDeletion;
                bypassThisDeletion = false;
                SessionSystem.Add(url, title);
            }
        }

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
                    back.Click += (sender, e) => { if (SessionSystem != null && SessionSystem.CanGoBack) { SessionSystem.GoBack(); } RightClickMenu.Hide(); };
                    _content.Children.Add(back);

                    Avalonia.Controls.MenuItem forward = new()
                    {
                        Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.Forward"),
                    };
                    forward.Click += (sender, e) => { if (SessionSystem != null && SessionSystem.CanGoForward) { SessionSystem.GoForward(); } RightClickMenu.Hide(); };
                    _content.Children.Add(forward);

                    Avalonia.Controls.MenuItem refresh = new()
                    {
                        Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.Refresh"),
                    };
                    refresh.Click += (sender, e) => { webView1.Reload(); RightClickMenu.Hide(); };
                    _content.Children.Add(refresh);

                    Avalonia.Controls.MenuItem refreshNoCache = new()
                    {
                        Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.RefreshNoCache"),
                    };
                    refreshNoCache.Click += (sender, e) => { webView1.ReloadIgnoreCache(); RightClickMenu.Hide(); };
                    _content.Children.Add(refreshNoCache);

                    Avalonia.Controls.MenuItem stopbutton = new()
                    {
                        Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.Stop"),
                    };
                    stopbutton.Click += (sender, e) => { webView1.Stop(); RightClickMenu.Hide(); };
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
                            Dialogs.RunSaveFileDialog(mainWindow, YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.SaveFileDialog"),
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
                        Dialogs.RunSaveFileDialog(mainWindow, YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.SaveFileDialog"),
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer

                _startUrl = null;
                findText = null;
                CurrentSite = null;
                mainWindow = null;
                SessionSystem = null;
                ContentGrid = null;
                webView1 = null;
                title = null;
                url = null;
                _tbUrl = null;
                _ZoomLevel = null;
                _FindCount = null;

                Content = null;

                disposedValue = true;
            }
        }

        ~TabWindow()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}