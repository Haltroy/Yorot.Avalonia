using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CefNet;
using CefNet.Avalonia;
using FluentAvalonia.UI.Controls;
using System;
using System.Collections.Generic;
using Yorot;
using Yorot_Avalonia.Handlers;

namespace Yorot_Avalonia.Views
{
    public partial class PopupWindow : Window
    {
        public System.Reactive.Subjects.Subject<bool>? IsPageSafe;
        public System.Reactive.Subjects.Subject<bool>? IsPageUsedCookie;
        public System.Reactive.Subjects.Subject<bool>? IsPageUnsafe;

        public PopupWindow()
        {
            InitializeComponent();
        }

        public YorotSite CurrentSite;

        private string _startUrl = "yorot://blank";

        public PopupWindow(string startUrl)
        {
            _startUrl = startUrl;
            InitializeComponent();
        }

        private TextBox? Urlbox;
        private YorotWebView webView;
        public MainWindow? mainWindow;
        public ToggleSwitch? AllowMic;
        public ToggleSwitch? AllowCam;
        public ToggleSwitch? AllowCookies;
        public ToggleSwitch? AllowYS;
        public ToggleSwitch? AllowPopup;
        public ToggleSwitch? AllowNotif;
        public ToggleSwitch? AllowNotifBoot;
        public Avalonia.Controls.ComboBox? NotifPriority;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            IsPageSafe = new System.Reactive.Subjects.Subject<bool>();
            IsPageUsedCookie = new System.Reactive.Subjects.Subject<bool>();
            IsPageUnsafe = new System.Reactive.Subjects.Subject<bool>();

            IsPageSafe.OnNext(true);
            IsPageUsedCookie.OnNext(false);
            IsPageUnsafe.OnNext(false);

            var contentpanel = this.FindControl<DockPanel>("ContentPanel");
            var urlbox = contentpanel.FindControl<Grid>("UrlBar");
            Urlbox = urlbox.FindControl<TextBox>("Url");

            var pageinfo = urlbox.FindControl<Avalonia.Controls.Button>("SecurityInfo");
            pageinfo.Click += (sender, e) =>
            {
                if (pageinfo.ContextFlyout != null)
                {
                    pageinfo.ContextFlyout.ShowAt(pageinfo);
                }
            };
            if (pageinfo.Content is Panel panel)
            {
                panel.FindControl<Image>("WebsiteGood").Bind(IsVisibleProperty, IsPageSafe);
                panel.FindControl<Image>("WebsiteMeh").Bind(IsVisibleProperty, IsPageUsedCookie);
                panel.FindControl<Image>("WebsiteBad").Bind(IsVisibleProperty, IsPageUnsafe);
            }

            if (pageinfo.ContextFlyout is Flyout flyout && flyout.Content is StackPanel stackPanel)
            {
                var pageSafe = stackPanel.Children[0] as Panel;
                (pageSafe.Children[0] as Panel).Bind(IsVisibleProperty, IsPageSafe);
                (pageSafe.Children[1] as Panel).Bind(IsVisibleProperty, IsPageUsedCookie);

                AllowMic = stackPanel.Children[1] as ToggleSwitch;
                AllowCam = stackPanel.Children[2] as ToggleSwitch;
                AllowCookies = stackPanel.Children[3] as ToggleSwitch;
                AllowYS = stackPanel.Children[4] as ToggleSwitch;
                AllowPopup = stackPanel.Children[5] as ToggleSwitch;
                AllowNotif = stackPanel.Children[6] as ToggleSwitch;
                AllowNotifBoot = stackPanel.Children[7] as ToggleSwitch;

                var notifPanel = stackPanel.Children[8] as StackPanel;
                NotifPriority = notifPanel.Children[1] as Avalonia.Controls.ComboBox;
            }

            KeyDown += (sender, e) =>
            {
                if (e.Key == Avalonia.Input.Key.LeftShift || e.Key == Avalonia.Input.Key.RightShift || e.KeyModifiers.HasFlag(Avalonia.Input.KeyModifiers.Shift))
                {
                    shiftPressed = true;
                }
            };

            KeyUp += (sender, e) =>
            {
                if (e.Key == Avalonia.Input.Key.LeftShift || e.Key == Avalonia.Input.Key.RightShift || e.KeyModifiers.HasFlag(Avalonia.Input.KeyModifiers.Shift))
                {
                    shiftPressed = false;
                }
            };

            webView = new YorotWebView(this);

            webView.Navigating += (sender, e) =>
            {
                IsPageSafe.OnNext(true);
                IsPageUsedCookie.OnNext(false);
                IsPageUnsafe.OnNext(false);
            };

            webView.DocumentTitleChanged += (sender, e) => { Title = e.Title; };

            webView.AddressChange += (sender, e) => { if (Urlbox != null) { Urlbox.Text = e.Url; } };

            webView.LoadError += (sender, e) => { if (e.Frame.IsMain) { webView.Navigate("yorot://error?e=" + e.ErrorCode.ToString() + "&u=" + e.FailedUrl); } };

            contentpanel.FindControl<DockPanel>("cefpanel").Children.Add(webView);
        }

        private bool shiftPressed = false;

        internal void redirectTo(string url, string title = "")
        {
            webView.Navigate(url);
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

        private void PageSettings(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // TODO: Open Settings for this page.
        }

        private void NotifPriorityChange(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is Avalonia.Controls.ComboBox cBox)
            {
                CurrentSite.Permissions.notifPriority = cBox.SelectedIndex - 1;
            }
        }

        private void AllowMicChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.ComboBox cBox)
            {
                CurrentSite.Permissions.allowMic.Allowance = shiftPressed ? YorotPermissionMode.AllowOneTime : YorotPermissionMode.Allow;
            }
        }

        private void DisableMicrophone(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.ComboBox cBox)
            {
                CurrentSite.Permissions.allowMic.Allowance = YorotPermissionMode.Deny;
            }
        }

        private void AllowCamChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.ComboBox cBox)
            {
                CurrentSite.Permissions.allowCam.Allowance = shiftPressed ? YorotPermissionMode.AllowOneTime : YorotPermissionMode.Allow;
            }
        }

        private void DisableCamera(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.ComboBox cBox)
            {
                CurrentSite.Permissions.allowCam.Allowance = YorotPermissionMode.Deny;
            }
        }

        private void AllowCookieChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.ComboBox cBox)
            {
                CurrentSite.Permissions.allowCookies.Allowance = shiftPressed ? YorotPermissionMode.AllowOneTime : YorotPermissionMode.Allow;
            }
        }

        private void DisableCookie(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.ComboBox cBox)
            {
                CurrentSite.Permissions.allowCookies.Allowance = YorotPermissionMode.Deny;
            }
        }

        private void AllowPopupChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.ComboBox cBox)
            {
                CurrentSite.Permissions.allowPopup.Allowance = shiftPressed ? YorotPermissionMode.AllowOneTime : YorotPermissionMode.Allow;
            }
        }

        private void DisablePopup(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.ComboBox cBox)
            {
                CurrentSite.Permissions.allowPopup.Allowance = YorotPermissionMode.Deny;
            }
        }

        private void AllowNotifChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.ComboBox cBox)
            {
                CurrentSite.Permissions.allowNotif.Allowance = shiftPressed ? YorotPermissionMode.AllowOneTime : YorotPermissionMode.Allow;
            }
        }

        private void DisableNotif(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.ComboBox cBox)
            {
                CurrentSite.Permissions.allowNotif.Allowance = YorotPermissionMode.Deny;
            }
        }

        private void AllowNotifBootChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.ComboBox cBox)
            {
                CurrentSite.Permissions.startNotifOnBoot = true;
            }
        }

        private void DisableNotifBoot(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.ComboBox cBox)
            {
                CurrentSite.Permissions.startNotifOnBoot = false;
            }
        }

        private void AllowYSChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.ComboBox cBox)
            {
                CurrentSite.Permissions.allowYS.Allowance = shiftPressed ? YorotPermissionMode.AllowOneTime : YorotPermissionMode.Allow;
            }
        }

        private void DisableYS(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.ComboBox cBox)
            {
                CurrentSite.Permissions.allowYS.Allowance = YorotPermissionMode.Deny;
            }
        }

        private Avalonia.Controls.Flyout? RightClickMenu;
        private YorotGlue.ContextMenuParams? lastRCMParams;

        internal void ShowContextMenu(YorotGlue.ContextMenuParams menuParams, CefFrame frame)
        {
            if (menuParams is null) { throw new ArgumentNullException(nameof(menuParams)); }
            if (frame is null) { throw new ArgumentNullException(nameof(frame)); }
            if (webView != null && YorotGlobal.Main != null && mainWindow != null)
            {
                if (RightClickMenu is null || lastRCMParams is null || !lastRCMParams.Equals(menuParams))
                {
                    lastRCMParams = menuParams;
                    RightClickMenu = new() { ShowMode = FlyoutShowMode.TransientWithDismissOnPointerMoveAway };

                    var _content = new StackPanel() { Orientation = Avalonia.Layout.Orientation.Vertical, };

                    RightClickMenu.Content = _content;

                    Avalonia.Controls.MenuItem refresh = new()
                    {
                        Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.Refresh"),
                    };
                    refresh.Click += (sender, e) => { webView.Reload(); RightClickMenu.Hide(); };
                    _content.Children.Add(refresh);

                    Avalonia.Controls.MenuItem refreshNoCache = new()
                    {
                        Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.RefreshNoCache"),
                    };
                    refreshNoCache.Click += (sender, e) => { webView.ReloadIgnoreCache(); RightClickMenu.Hide(); };
                    _content.Children.Add(refreshNoCache);

                    Avalonia.Controls.MenuItem stopbutton = new()
                    {
                        Header = YorotGlobal.Main.CurrentLanguage.GetItemText("RightClick.Stop"),
                    };
                    stopbutton.Click += (sender, e) => { webView.Stop(); RightClickMenu.Hide(); };
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
                        downloadImage.Click += (sender, e) => { webView.DownloadFile(menuParams.SourceUrl); RightClickMenu.Hide(); };
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
                            webView.DownloadFile(menuParams.LinkUrl);
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
                        webView.Print();
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
                                    webView.PrintToPdf(filename, new CefPdfPrintSettings()
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
                    devtools.Click += (sender, e) => { webView.ShowDevTools(); RightClickMenu.Hide(); };
                    _content.Children.Add(devtools);
                }
                RightClickMenu.ShowAt(webView, true);
            }
        }
    }
}