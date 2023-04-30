using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Mixins;
using Avalonia.Styling;
using Avalonia.VisualTree;
using CefNet;
using DynamicData;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using MessageBox.Avalonia.Models;
using System;
using System.Linq;
using System.Reactive.Linq;
using Yorot;
using Yorot_Avalonia.Handlers;

namespace Yorot_Avalonia.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Opened += (sender, e) =>
            {
                if (!YorotGlobal.Main.CurrentSettings.SessionManager.PreviousShutdownWasSafe)
                {
                    var box = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxCustomWindow(new MessageBox.Avalonia.DTO.MessageBoxCustomParams()
                    {
                        WindowIcon = this.Icon,
                        Icon = MessageBox.Avalonia.Enums.Icon.Question,
                        ContentTitle = "Yorot",
                        ContentHeader = YorotGlobal.Main.CurrentLanguage.GetItemText("UI.RestoreSessionTitle"),
                        ContentMessage = YorotGlobal.Main.CurrentLanguage.GetItemText("UI.RestoreSessionDesc"),
                        ButtonDefinitions = new[]
                        {
                            new ButtonDefinition() { Name = YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.Yes")},
                            new ButtonDefinition() { Name = YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.No")},
                        },
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    });
                    Dialogs.RunMessageBoxDialog(box, this, new Action<string>((result) =>
                    {
                        if (result == YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.Yes") && YorotGlobal.Main.CurrentSettings.SessionManager.Systems[YorotGlobal.Main.CurrentSettings.SessionManager.Systems.Count - 2] is SessionSystem session)
                        {
                            NewTab(session: session);
                        }
                    }));
                }
            };
        }

        private void MainWindow_Initialized(object? sender, EventArgs e)
        {
            if (YorotGlobal.Main != null && YorotGlobal.Main.CurrentSettings != null)
            {
                Width = YorotGlobal.Main.CurrentSettings.LastSize.Width;
                Height = YorotGlobal.Main.CurrentSettings.LastSize.Height;
                Position = new PixelPoint(YorotGlobal.Main.CurrentSettings.LastLocation.X, YorotGlobal.Main.CurrentSettings.LastLocation.Y);
            }
            RefreshFavorites(true);
        }

        private void backflyout_opening(object? s, EventArgs e)
        {
            if (Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.SessionSystem is SessionSystem sessionSystem && goback.ContextFlyout is Flyout flyout && flyout.Content is StackPanel backflyout)
            {
                backflyout.Children.Clear();
                var before = sessionSystem.Before();
                for (int i = before.Length - 1; i > 0; i--)
                {
                    Avalonia.Controls.MenuItem mitem = new() { Name = i + "", Header = before[i].Title, Tag = before[i] };
                    mitem.Click += window.backforwarditem_click;
                    backflyout.Children.Add(mitem);
                }
            }
        }

        private void forwardflyout_opening(object? s, EventArgs e)
        {
            if (Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.SessionSystem is SessionSystem sessionSystem && goforward.ContextFlyout is Flyout flyout && flyout.Content is StackPanel backflyout)
            {
                backflyout.Children.Clear();
                var after = sessionSystem.After();
                for (int i = 0; i < after.Length; i++)
                {
                    Avalonia.Controls.MenuItem mitem = new() { Name = i + "", Header = after[i].Title, Tag = after[i] };
                    mitem.Click += window.backforwarditem_click;
                    backflyout.Children.Add(mitem);
                }
            }
        }

        private void opened(object? sender, EventArgs e)
        {
            if (YorotGlobal.Main != null && YorotGlobal.Main.MainForms.Count <= 0)
            {
                YorotGlobal.Main.MainForms.Add(this);
                NewTab(url: "yorot://homepage", switchTo: true);
            }
        }

        private void Tabs_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (sender is Control control)
            {
                var pos = e.GetPosition(control);
                if (pos.Y < 41)
                {
                    this.BeginMoveDrag(e);
                }
            }
        }

        private void ManageExtensions(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // TODO
        }

        private void Flyout_Opened(object? sender, EventArgs e)
        {
            if (sender is Flyout flyout && flyout.Content is StackPanel panel)
            {
                var textblock = panel.Children[0];
                var button = panel.Children[panel.Children.Count - 1];

                panel.Children.Clear();

                panel.Children.Add(textblock);
                for (int i = 0; i < YorotGlobal.Main.Extensions.Extensions.Count; i++)
                {
                    var ext = YorotGlobal.Main.Extensions.Extensions[i];

                    Avalonia.Controls.Button extbutton = new()
                    {
                        Tag = ext
                    };

                    StackPanel extpanel = new() { Spacing = 5, Orientation = Avalonia.Layout.Orientation.Horizontal };

                    Avalonia.Controls.Image exticon = new() { Source = YorotTools.GetExtIcon(ext), Width = 32, Height = 32 };
                    extpanel.Children.Add(exticon);

                    TextBlock exttitle = new() { Text = ext.Name };
                    extpanel.Children.Add(exttitle);

                    // TODO: Add pinning

                    // TODO: Add settings

                    extbutton.Content = extpanel;

                    extbutton.Click += (sender, e) =>
                    {
                        // TODO:
                        switch (ext.PopupLaunchMode)
                        {
                            case YorotExtensionLaunchMode.Raw:
                                break;

                            case YorotExtensionLaunchMode.FromFile:
                                break;

                            case YorotExtensionLaunchMode.TabRaw:
                                break;

                            case YorotExtensionLaunchMode.TabFile:
                                break;

                            case YorotExtensionLaunchMode.PopupRaw:
                                break;

                            case YorotExtensionLaunchMode.PopupFile:
                                break;

                            case YorotExtensionLaunchMode.AppForms:
                                break;
                        }
                    };

                    panel.Children.Add(extbutton);
                }
                panel.Children.Add(button);
            }
        }

        private void TbUrl_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (sender is TextBox textbox && e.Property == TextBox.TextProperty && Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window)
            {
                window.tbUrl = textbox.Text;
            }
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window)
            {
                _Tabswitching = true;
                reload.IsVisible = window.IsNavigated;
                stop.IsVisible = window.IsNavigating;
                goback.IsEnabled = window.CanGoBack;
                goback.IsEnabled = window.CanGoForward;
                zoomin.IsEnabled = window.CanZoomIn;
                zoomout.IsEnabled = window.CanZoomOut;
                favoritedbutton.IsVisible = window.IsFavorited;
                favoritebutton.IsVisible = window.IsNotFavorited;
                UnmuteButton.IsVisible = window.IsMuted;
                MuteButton.IsVisible = window.IsUnmuted;
                PageSafeInfo.IsVisible = window.IsPageSafe;
                WebsiteGood.IsVisible = window.IsPageSafe;
                PageUsedCookieInfo.IsVisible = window.IsPageUsedCookie;
                WebsiteMeh.IsVisible = window.IsPageUsedCookie;
                PageUnsafeInfo.IsVisible = window.IsPageUnsafe;
                WebsiteBad.IsVisible = window.IsPageUnsafe;
                tbUrl.Text = window.tbUrl;
                FindCount.Text = window.FindCount;
                ZoomLevel.Text = window.ZoomLevel;
                findText.Text = window.findText;
                MatchCase.IsChecked = window.MatchCase;
                if (window.CurrentSite is null)
                {
                    window.CurrentSite = YorotGlobal.Main.CurrentSettings.SiteMan.GetSite(window.url);
                }
                AllowMic.IsChecked = window.CurrentSite.Permissions.allowMic.Allowance == YorotPermissionMode.Allow || window.CurrentSite.Permissions.allowMic.Allowance == YorotPermissionMode.AllowOneTime;
                AllowCam.IsChecked = window.CurrentSite.Permissions.allowCam.Allowance == YorotPermissionMode.Allow || window.CurrentSite.Permissions.allowCam.Allowance == YorotPermissionMode.AllowOneTime;
                AllowCookies.IsChecked = window.CurrentSite.Permissions.allowCookies.Allowance == YorotPermissionMode.Allow || window.CurrentSite.Permissions.allowCookies.Allowance == YorotPermissionMode.AllowOneTime;
                AllowYS.IsChecked = window.CurrentSite.Permissions.allowYS.Allowance == YorotPermissionMode.Allow || window.CurrentSite.Permissions.allowYS.Allowance == YorotPermissionMode.AllowOneTime;
                AllowPopup.IsChecked = window.CurrentSite.Permissions.allowPopup.Allowance == YorotPermissionMode.Allow || window.CurrentSite.Permissions.allowPopup.Allowance == YorotPermissionMode.AllowOneTime;
                AllowNotif.IsChecked = window.CurrentSite.Permissions.allowNotif.Allowance == YorotPermissionMode.Allow || window.CurrentSite.Permissions.allowNotif.Allowance == YorotPermissionMode.AllowOneTime;
                AllowNotifBoot.IsChecked = window.CurrentSite.Permissions.startNotifOnBoot;
                NotifPriority.SelectedIndex = window.CurrentSite.Permissions.notifPriority + 1;
            }
            if (Tabs.TabItems.Count() <= 0)
            {
                Close();
            }
        }

        private void ManageBookmarks(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            FavoriteWindow favorite = new() { DataContext = YorotGlobal.ViewModel };
            favorite.Show();
            favorite.BringIntoView();
        }

        private async void ProfileChangeName(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var msgbox = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxInputWindow(new MessageBox.Avalonia.DTO.MessageBoxInputParams()
            {
                WindowIcon = this.Icon,
                Icon = MessageBox.Avalonia.Enums.Icon.Setting,
                ContentTitle = "Yorot",
                ContentMessage = YorotGlobal.Main.CurrentLanguage.GetItemText("UI.ChangeProfileNameDesc"),
                InputDefaultValue = YorotGlobal.Main.Profiles.Current.Text,
                ButtonDefinitions = new[]
                {
                    new ButtonDefinition {Name = YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.OK")},
                    new ButtonDefinition {Name = YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.Cancel")}
                },
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            });
            var result = await msgbox.ShowDialog(this);
            if (result.Button == YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.OK"))
            {
                YorotGlobal.Main.Profiles.Current.Text = result.Message;
            }
        }

        private void NotifPriorityChange(object? sender, SelectionChangedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.notifPriority = cBox.SelectedIndex - 1;
            }
        }

        private void AllowMicChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowMic.Allowance = shiftPressed ? YorotPermissionMode.AllowOneTime : YorotPermissionMode.Allow;
            }
        }

        private void DisableMicrophone(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowMic.Allowance = YorotPermissionMode.Deny;
            }
        }

        private void AllowCamChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowCam.Allowance = shiftPressed ? YorotPermissionMode.AllowOneTime : YorotPermissionMode.Allow;
            }
        }

        private void DisableCamera(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowCam.Allowance = YorotPermissionMode.Deny;
            }
        }

        private void AllowCookieChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowCookies.Allowance = shiftPressed ? YorotPermissionMode.AllowOneTime : YorotPermissionMode.Allow;
            }
        }

        private void DisableCookie(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowCookies.Allowance = YorotPermissionMode.Deny;
            }
        }

        private void AllowPopupChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowPopup.Allowance = shiftPressed ? YorotPermissionMode.AllowOneTime : YorotPermissionMode.Allow;
            }
        }

        private void DisablePopup(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowPopup.Allowance = YorotPermissionMode.Deny;
            }
        }

        private void AllowNotifChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowNotif.Allowance = shiftPressed ? YorotPermissionMode.AllowOneTime : YorotPermissionMode.Allow;
            }
        }

        private void DisableNotif(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowNotif.Allowance = YorotPermissionMode.Deny;
            }
        }

        private void AllowNotifBootChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.startNotifOnBoot = true;
            }
        }

        private void DisableNotifBoot(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.startNotifOnBoot = false;
            }
        }

        private void AllowYSChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowYS.Allowance = shiftPressed ? YorotPermissionMode.AllowOneTime : YorotPermissionMode.Allow;
            }
        }

        private void DisableYS(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowYS.Allowance = YorotPermissionMode.Deny;
            }
        }

        private void ProfileChangeImage(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // TODO: Open an image file, copy it into the profile's folder as "picture.png" and then refresh everything
        }

        private void ProfileSwitch(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // TODO: Show a switch menu for profile, switch to it and restart
        }

        private void ProfileSettings(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // TODO: After implementing the settings menu
        }

        private void TabWindow_KeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Avalonia.Input.Key.LeftShift:
                case Avalonia.Input.Key.RightShift:
                    shiftPressed = false;
                    break;
            }
        }

        private bool shiftPressed = false;

        private void Stop(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
            {
                webView.Stop();
            }
        }

        private void GoBack(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.SessionSystem is SessionSystem sessionSystem && window.webView1 != null && sessionSystem.CanGoBack)
            {
                window.bypassThisDeletion = true;
                sessionSystem.GoBack();
            }
        }

        private void GoForward(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.SessionSystem is SessionSystem sessionSystem && window.webView1 != null && sessionSystem.CanGoForward)
            {
                window.bypassThisDeletion = true;
                sessionSystem.GoForward();
            }
        }

        private void urlkeydown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Enter)
            {
                if (tbUrl != null && Tabs != null && YorotGlobal.Main != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
                {
                    if (HTAlt.Tools.ValidUrl(tbUrl.Text, new string[] { "yorot" }, false))
                    {
                        window.redirectTo(tbUrl.Text, "");
                        webView.Navigate(tbUrl.Text);
                    }
                    else
                    {
                        var searchUrl = YorotGlobal.Main.CurrentSettings.SearchEngine.Search(tbUrl.Text);
                        window.redirectTo(searchUrl, "");
                        webView.Navigate(searchUrl);
                    }
                }
            }
        }

        private void GoHome(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.SessionSystem is SessionSystem sessionSystem && window.webView1 != null && sessionSystem.CanGoForward && YorotGlobal.Main != null)
            {
                window.redirectTo(YorotGlobal.Main.CurrentSettings.HomePage, "");
                window.webView1.Navigate(YorotGlobal.Main.CurrentSettings.HomePage);
            }
        }

        private void Reload(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
            {
                if (shiftPressed)
                {
                    webView.ReloadIgnoreCache();
                }
                else
                {
                    webView.Reload();
                }
            }
        }

        private void Favorite(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (YorotGlobal.Main is null) { return; }
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window)
            {
                YorotGlobal.Main.CurrentSettings.FavManager.Favorites.Add(new YorotFavorite(YorotGlobal.Main.CurrentSettings.FavManager.RootFolder, window.url, window.title));
            }
            RefreshFavorites(true);
            favoritedbutton.IsVisible = true;
            favoritebutton.IsVisible = false;
        }

        private void Unfavorite(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (YorotGlobal.Main is null || tbUrl is null) { return; }
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window)
            {
                var favs = YorotGlobal.Main.CurrentSettings.FavManager.GetFavorite(window.url);
                if (favs.Count > 0)
                {
                    if (favs[0].ParentFolder is null)
                    {
                        YorotGlobal.Main.CurrentSettings.FavManager.RootFolder.Favorites.Remove(favs[0]);
                    }
                    else
                    {
                        favs[0].ParentFolder.Favorites.Remove(favs[0]);
                    }
                    RefreshFavorites(true);
                    favoritedbutton.IsVisible = true;
                    favoritebutton.IsVisible = false;
                }
            }
        }

        private void printbutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
            {
                webView.Print();
            }
        }

        private void screenshotbutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (YorotGlobal.Main != null && Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
            {
                Dialogs.RunSaveFileDialog(this, YorotGlobal.Main.CurrentLanguage.GetItemText("UI.SaveScreenshot"),
                    new string[] { "png" },
                    (path) =>
                    {
                        var pixelSize = new PixelSize((int)webView.Bounds.Width, (int)webView.Bounds.Height);
                        using (Avalonia.Media.Imaging.RenderTargetBitmap bitmap = new(pixelSize))
                        {
                            bitmap.Render(webView);
                            bitmap.Save(path);
                        }
                    }
                    );
            }
        }

        private void devtoolsbutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
            {
                webView.ShowDevTools();
            }
        }

        private void NewWindow(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            NewWindow();
        }

        private void zoominbutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
            {
                if (window.zoomLevel < 500)
                {
                    if (window.zoomLevel < 0)
                    {
                        window.zoomLevel += 10;
                    }
                    else if (window.zoomLevel < 300)
                    {
                        window.zoomLevel += 25;
                    }
                    else if (window.zoomLevel >= 300)
                    {
                        window.zoomLevel += 50;
                    }

                    window.CanZoomOut = true;
                    webView.ZoomLevel = ((double)window.zoomLevel / 100);
                    if (window.ZoomLevel != null)
                    {
                        window.ZoomLevel = (window.zoomLevel + 100) + "%";
                    }

                    if (window.zoomLevel >= 500) { window.CanZoomIn = false; }
                }
                else
                {
                    window.CanZoomIn = false;
                }
            }
        }

        private void zoomoutbutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
            {
                if (window.zoomLevel > -100)
                {
                    if (window.zoomLevel > 300)
                    {
                        window.zoomLevel -= 50;
                    }
                    else if (window.zoomLevel > 0)
                    {
                        window.zoomLevel -= 25;
                    }
                    else if (window.zoomLevel <= 0)
                    {
                        window.zoomLevel -= 10;
                    }

                    window.CanZoomIn = true;
                    webView.ZoomLevel = ((double)window.zoomLevel / 100);
                    if (ZoomLevel != null)
                    {
                        window.ZoomLevel = (window.zoomLevel + 100) + "%";
                    }

                    if (window.zoomLevel <= -100)
                    {
                        window.CanZoomOut = false;
                    }
                }
                else
                {
                    window.CanZoomOut = false;
                }
            }
        }

        private void findnextbutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView && findText != null)
            {
                if (string.IsNullOrWhiteSpace(findText.Text))
                {
                    window.Searching = false;
                    webView.StopFinding(true);
                }
                else
                {
                    window.Searching = true;
                    webView.Find(window.findText, true, window.MatchCase, true);
                }
            }
        }

        private void MatchCaseChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.CheckBox checkBox && checkBox.IsChecked.HasValue && Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
            {
                window.MatchCase = checkBox.IsChecked.Value;
                if (webView != null && window.Searching && findText != null && window.Searching)
                {
                    webView.Find(window.findText, false, window.MatchCase, window.findNext);
                }
            }
        }

        private bool _Tabswitching = false;

        private void FindText_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (_Tabswitching) { _Tabswitching = false; return; }
            if (e.Property == TextBox.TextProperty && Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.IsAlive && window.webView1 is YorotWebView webView && findText != null)
            {
                window.findText = findText.Text;
                if (!string.IsNullOrWhiteSpace(window.findText))
                {
                    if (!window.Searching)
                    {
                        window.Searching = true;
                        webView.Find(window.findText, true, window.MatchCase, true);
                    }
                    else
                    {
                        window.Searching = true;
                        webView.Find(window.findText, true, window.MatchCase, window.findNext);
                    }
                }
                else
                {
                    webView.StopFinding(true);
                    window.Searching = false;
                }
            }
        }

        private void MatchCaseUnchecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MatchCaseChecked(sender, e);
        }

        private void PageSettings(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // TODO: Open Settings for this page.
        }

        private void NewIncWindow(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().ProcessName, "-i");
        }

        private async void savebutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && YorotGlobal.Main != null && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
            {
                var task = webView.GetMainFrame().GetSourceAsync(System.Threading.CancellationToken.None);
                if (task.IsCompletedSuccessfully)
                {
                    Dialogs.RunSaveFileDialog(this, YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.SaveFileDialog"),
                                new string[] { "html" },
                                new Action<string>((filename) =>
                                {
                                    if (!System.IO.File.Exists(filename))
                                    {
                                        System.IO.File.Create(filename).Close();
                                    }
                                    HTAlt.Tools.WriteFile(filename, task.Result, System.Text.Encoding.UTF8);
                                }
                                ));
                }
            }
        }

        private void mutebutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
            {
                webView.AudioMuted = true;
                MuteButton.IsVisible = false;
                UnmuteButton.IsVisible = true;
            }
        }

        private void unmutebutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
            {
                webView.AudioMuted = false;
                MuteButton.IsVisible = true;
                UnmuteButton.IsVisible = false;
            }
        }

        private void openmenu(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Avalonia.Controls.Control cntrl && cntrl.ContextFlyout != null)
            {
                cntrl.ContextFlyout.ShowAt(cntrl, true);
            }
        }

        private void favitem_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Tabs != null && Tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView && sender is Avalonia.Controls.MenuItem mitem && mitem.Tag is YorotFavorite fav)
            {
                webView.Navigate(fav.Url);
            }
        }

        private string CachedFavs = "";

        private void RefreshFavorites(bool force = false)
        {
            if (YorotGlobal.Main != null && tbUrl != null)
            {
                if (force || (YorotGlobal.Main.CurrentSettings.FavManager.ToXml() != CachedFavs))
                {
                    Favorites.IsVisible = YorotGlobal.Main.CurrentSettings.FavManager.ShowFavorites;
                    CachedFavs = YorotGlobal.Main.CurrentSettings.FavManager.ToXml();
                    if ((Favorites != null && Favorites.Items is AvaloniaList<object> list))
                    {
                        list.Clear();
                        var favbars = YorotGlobal.Main.CurrentSettings.FavManager.RootFolder;
                        if (favbars.Favorites.Count > 0)
                        {
                            for (int i = 0; i < favbars.Favorites.Count; i++)
                            {
                                var fav = favbars.Favorites[i];
                                Avalonia.Controls.MenuItem item = new();
                                item.Bind(BackgroundProperty, tbUrl.GetBindingObservable(BackgroundProperty));
                                item.Bind(ForegroundProperty, tbUrl.GetBindingObservable(ForegroundProperty));
                                item.Header = fav.Text;
                                item.Tag = fav;
                                //item.Icon = fav.Icon; // TODO
                                list.Add(item);
                                if (fav is not YorotFavorite and not null)
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
                    if (fav is not YorotFavorite and not null)
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

        private void Tabs_TabstripDragOver(object? sender, Avalonia.Input.DragEventArgs e)
        {
            e.Handled = true;
            e.DragEffects = Avalonia.Input.DragDropEffects.Move;
        }

        private void Tabs_TabDroppedOutside(TabView sender, TabViewTabDroppedOutsideEventArgs args)
        {
            if (YorotGlobal.Main is null || (YorotGlobal.Main.MainForms.Count <= 1 && Tabs.TabItems.Count() <= 1)) { return; }

            bool isAttached = false;
            for (int i = 0; i < YorotGlobal.Main.MainForms.Count; i++)
            {
                var mainform = YorotGlobal.Main.MainForms[i];
                if (mainform.WindowState == WindowState.Minimized || mainform.Tabs is null) { continue; }

                var tabBound = new Rect(Position.X + args.Tab.Bounds.X, Position.Y + args.Tab.Bounds.Y, args.Tab.Bounds.Width, args.Tab.Bounds.Height);

                var formBound = new Rect(mainform.Position.X, mainform.Position.Y, mainform.Bounds.Width, mainform.Bounds.Height);

                if (formBound.Intersects(tabBound))
                {
                    isAttached = true;

                    if (sender.TabItems is AvaloniaList<object> list && Tabs != null && mainform.Tabs != null && mainform.Tabs.TabItems is AvaloniaList<object> list2)
                    {
                        list.Remove(args.Tab);
                        list2.Add(args.Tab);

                        if (list.Count <= 0)
                        {
                            Close();
                        }
                    }

                    break;
                }
            }
            if (!isAttached)
            {
                var window = new MainWindow()
                {
                    DataContext = YorotGlobal.ViewModel,
                    //IsVisible = true,
                    //IsEnabled = true,
                    WindowState = WindowState.Normal,
                    ShowInTaskbar = true,
                    Position = new PixelPoint(YorotGlobal.Main.CurrentSettings.LastLocation.X, YorotGlobal.Main.CurrentSettings.LastLocation.Y),
                    Width = YorotGlobal.Main.CurrentSettings.LastSize.Width,
                    Height = YorotGlobal.Main.CurrentSettings.LastSize.Height,
                    Bounds = new Rect(YorotGlobal.Main.CurrentSettings.LastLocation.X, YorotGlobal.Main.CurrentSettings.LastLocation.Y, YorotGlobal.Main.CurrentSettings.LastSize.Width, YorotGlobal.Main.CurrentSettings.LastSize.Height),
                };

                window.Show();
                window.Activate();
                window.BringIntoView();

                YorotGlobal.Main.MainForms.Add(window);

                if (sender.TabItems is AvaloniaList<object> list && Tabs != null && window.Tabs.TabItems is AvaloniaList<object> list2)
                {
                    list.Remove(args.Tab);
                    list2.Add(args.Tab);

                    if (list.Count <= 0)
                    {
                        Close();
                    }
                }
            }
        }

        private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            if (sender.TabItems is AvaloniaList<object> list)
            {
                var item = args.Item as TabViewItem;
                var dockPanel = item.Content as DockPanel;
                var tabWindow = dockPanel.Children[0] as TabWindow;

                YorotGlobal.Main.CurrentSettings.SessionManager.Close(tabWindow.SessionSystem);

                tabWindow.SessionSystem.RemoveAllEvents();

                tabWindow.webView1.Stop();
                tabWindow.webView1.Navigate("about:blank");
                //tabWindow.webView1 = null;

                tabWindow.Content = null;

                list.Remove(item);

                dockPanel.Children.Clear();

                dockPanel.GetVisualChildren().ToList().Clear();

                item.Content = null;

                tabWindow.webView1.DisposeWith(new System.Reactive.Disposables.CompositeDisposable(tabWindow));

                tabWindow.Dispose();

                GC.Collect();

                if (list.Count <= 0)
                {
                    Close();
                }
            }
        }

        private void Tabs_AddTabButtonClick(TabView sender, System.EventArgs args)
        {
            NewTab(switchTo: true);
        }

        private void MainWindow_Closed(object? sender, System.EventArgs e)
        {
            if (YorotGlobal.Main is null) { return; }
            YorotGlobal.Main.MainForms.Remove(this);
            if (YorotGlobal.Main.MainForms.Count <= 0)
            {
                YorotGlobal.Main.CurrentSettings.LastLocation = new System.Drawing.Point(Position.X, Position.Y);
                YorotGlobal.Main.CurrentSettings.LastSize = new System.Drawing.Size((int)Width, (int)Height);
                YorotGlobal.Main.Shutdown();
                CefNetApplication.Instance.Shutdown();
                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime app)
                {
                    app.Shutdown(0);
                }
            }
        }

        public void NewTab(string url = "yorot://newtab", int index = -1, bool switchTo = false, SessionSystem? session = null)
        {
            if (Tabs is null) { return; }
            TabViewItem item = new TabViewItem() { Header = "New Tab" };
            //item.Styles.Add(Styles);
            item.Bind(BackgroundProperty, Tabs.GetObservable(BackgroundProperty));
            item.Bind(ForegroundProperty, Tabs.GetObservable(ForegroundProperty));
            var style1 = new Avalonia.Styling.Style(new Func<Selector?, Selector>((s) =>
            {
                return
                    //Selectors.OfType(
                    Selectors.Template(
                        Selectors.Class(
                            Selectors.OfType(s, typeof(TabViewItem))
                            , ":selected")
                        )
                //, typeof(ContentPresenter))
                ;
            }));
            var backstyle = Styles[12] as Style;
            var setter1 = backstyle.Setters[0];
            var setter2 = backstyle.Setters[1];
            style1.Setters.Add(setter1);
            style1.Setters.Add(setter2);
            item.Styles.Add(style1);
            DockPanel panel = new();
            item.Content = panel;
            var tabform = new TabWindow(session) { mainWindow = this, _startUrl = url, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch };
            panel.Children.Add(tabform);
            if (Tabs.TabItems is AvaloniaList<object> itemList)
            {
                itemList.Insert(index == -1 ? (itemList.Count > 0 ? itemList.Count : 0) : index, item);
                if (switchTo)
                {
                    Tabs.SelectedItem = item;
                }
            }
        }

        private void NewWindowClicked(object? s, Avalonia.Interactivity.RoutedEventArgs e) => NewWindow();

        public void NewWindow(string url = "yorot://newtab")
        {
            if (YorotGlobal.Main is null) { return; }
            var mainform = new MainWindow()
            {
                DataContext = YorotGlobal.ViewModel,
                //IsVisible = true,
                //IsEnabled = true,
                WindowState = WindowState.Normal,
                ShowInTaskbar = true,
                Position = new PixelPoint(YorotGlobal.Main.CurrentSettings.LastLocation.X, YorotGlobal.Main.CurrentSettings.LastLocation.Y),
                Width = YorotGlobal.Main.CurrentSettings.LastSize.Width,
                Height = YorotGlobal.Main.CurrentSettings.LastSize.Height,
                Bounds = new Rect(YorotGlobal.Main.CurrentSettings.LastLocation.X, YorotGlobal.Main.CurrentSettings.LastLocation.Y, YorotGlobal.Main.CurrentSettings.LastSize.Width, YorotGlobal.Main.CurrentSettings.LastSize.Height),
            };

            mainform.Show();
            mainform.Activate();
            mainform.BringIntoView();

            mainform.NewTab(url);

            YorotGlobal.Main.MainForms.Add(mainform);
        }

        private void MainWindow_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == WidthProperty && prevAnimation == 1)
            {
                if (Sidebar != null)
                {
                    Sidebar.Width = Width - 5;
                }
            }
        }

        private void Panel_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (!(sender is null) && sender is Panel)
            {
                if (sender is Panel cntrl)
                {
                    cntrl.Background = Avalonia.Media.Brush.Parse("#00FF00");
                }
            }
        }

        private bool _sidebarPressed = false;
        private byte prevAnimation = 0;
        private bool isAnimating = false;
        private readonly int _animSpeed = 2;
        private int _cycleSkip = 0;

        private void sidebarPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                isAnimating = true;
                DoAnimation(prevAnimation == 0 ? (byte)1 : (byte)0);
            }
            else
            {
                _sidebarPressed = true;
            }
        }

        private bool _fullScreen = false;

        private void keyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Avalonia.Input.Key.F10:
                    if (Sidebar is null) { break; }
                    _fullScreen = !_fullScreen;
                    double sidebarW = Sidebar.Width + 0;
                    this.WindowState = !_fullScreen ? WindowState.Normal : WindowState.FullScreen;
                    isAnimating = true;
                    Sidebar.Width = sidebarW;
                    DoAnimation(prevAnimation != (byte)1 ? (_fullScreen ? (byte)2 : (byte)3) : (byte)1);
                    break;

                case Avalonia.Input.Key.Left:
                    if (e.KeyModifiers == Avalonia.Input.KeyModifiers.Control)
                    {
                        isAnimating = true;
                        switch (prevAnimation)
                        {
                            default:
                            case 0:
                                DoAnimation(!_fullScreen ? (byte)0 : (byte)2);
                                break;

                            case 1:
                                DoAnimation((byte)4);
                                break;

                            case 2:
                                DoAnimation(!_fullScreen ? (byte)0 : (byte)2);
                                break;

                            case 3:
                                DoAnimation(!_fullScreen ? (byte)0 : (byte)2);
                                break;

                            case 4:
                                DoAnimation((byte)0);
                                break;
                        }
                    }
                    break;

                case Avalonia.Input.Key.Right:
                    if (e.KeyModifiers == Avalonia.Input.KeyModifiers.Control)
                    {
                        isAnimating = true;
                        switch (prevAnimation)
                        {
                            default:
                            case 0:
                                DoAnimation((byte)4);
                                break;

                            case 1:
                                DoAnimation((byte)1);
                                break;

                            case 2:
                                DoAnimation((byte)3);
                                break;

                            case 3:
                                DoAnimation((byte)4);
                                break;

                            case 4:
                                DoAnimation((byte)1);
                                break;
                        }
                    }
                    break;

                case Avalonia.Input.Key.LeftShift:
                case Avalonia.Input.Key.RightShift:
                    shiftPressed = true;
                    break;

                // TODO: Remove, random theme test
                case Avalonia.Input.Key.F2:
                    YorotTheme random = YorotGlobal.Main.ThemeMan.Themes[new Random().Next(0, YorotGlobal.Main.ThemeMan.Themes.Count)];
                    YorotGlobal.Main.CurrentSettings.CurrentTheme = random;
                    var box = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxCustomWindow(new MessageBox.Avalonia.DTO.MessageBoxCustomParams()
                    {
                        WindowIcon = this.Icon,
                        Icon = MessageBox.Avalonia.Enums.Icon.Question,
                        ContentTitle = "Yorot",
                        ContentHeader = "Switched to:" + random.CodeName,
                        ContentMessage = $"ViewModel: {YorotGlobal.ViewModel.BackColor.ToString()} Panel: {Sidebar.Background.ToString()} Theme: {random.BackColor.ToHex()}",
                        ButtonDefinitions = new[]
                        {
                            new ButtonDefinition() { Name = "OK"},
                        },
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    });
                    Dialogs.RunMessageBoxDialog(box, this, new Action<string>((result) => { }));
                    this.DataContext = null;
                    this.DataContext = YorotGlobal.ViewModel;
                    this.InvalidateVisual();
                    break;
            }
        }

        private async void DoAnimation(byte direction)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    switch (isAnimating)
                    {
                        case false:
                            if (Sidebar is null) { break; }
                            prevAnimation = direction;
                            switch (direction)
                            {
                                case 3:
                                case 0: // Rollback to normal
                                    Sidebar.MinWidth = _fullScreen ? 0 : 55;
                                    Sidebar.Width = 55;
                                    break;

                                case 1: // Fullscreen
                                    Sidebar.MinWidth = 55;
                                    Sidebar.Width = Width - 5;
                                    break;

                                case 2: // Hide completely
                                    Sidebar.MinWidth = 0;
                                    Sidebar.Width = 0;
                                    break;

                                case 4:
                                    Sidebar.MinWidth = 0;
                                    Sidebar.Width = Width / 2;
                                    break;
                            }
                            break;

                        case true:
                            if (_cycleSkip == 10)
                            {
                                if (Sidebar is null) { break; }
                                _cycleSkip = 0;
                                switch (direction)
                                {
                                    case 0: // Rollback to normal
                                        Sidebar.MinWidth = 55;
                                        Sidebar.Width = Sidebar.Width != 55 ? Sidebar.Width - _animSpeed : 55;
                                        isAnimating = Sidebar.Width > 55;
                                        break;

                                    case 1: // Fullscreen
                                        double _animMax = Width - 5;
                                        Sidebar.MinWidth = 55;
                                        Sidebar.Width = Sidebar.Width != _animMax ? Sidebar.Width + _animSpeed : _animMax;
                                        isAnimating = Sidebar.Width < _animMax;
                                        break;

                                    case 2: // Hide completely
                                        Sidebar.MinWidth = 0;
                                        Sidebar.Width = Sidebar.Width != 0 ? Sidebar.Width - _animSpeed : 0;
                                        isAnimating = Sidebar.Width > 0;
                                        break;

                                    case 3: // Unhide
                                        Sidebar.MinWidth = 0;
                                        Sidebar.Width = Sidebar.Width != 55 ? Sidebar.Width + _animSpeed : 55;
                                        isAnimating = Sidebar.Width < 55;
                                        break;

                                    case 4: // Show Menu
                                        double _animMid = Width / 2;
                                        Sidebar.MinWidth = 55;
                                        Sidebar.Width = Sidebar.Width != _animMid ? Sidebar.Width + _animSpeed : _animMid;
                                        isAnimating = Sidebar.Width < _animMid;
                                        break;
                                }
                            }
                            else
                            {
                                _cycleSkip++;
                            }
                            DoAnimation(direction);
                            break;
                    }
                });
            });
        }

        private void sidebarMove(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            if (_sidebarPressed && Sidebar != null)
            {
                var a = e.GetCurrentPoint(frmMain);
                Sidebar.Width = a.Position.X;
            }
        }

        private void sidebarRelease(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            if (_sidebarPressed == false || Sidebar is null) { return; }
            _sidebarPressed = false;
            if (Sidebar.Width > (this.Width / 2))
            {
                isAnimating = true;
                DoAnimation((prevAnimation == 0 || prevAnimation == 4) ? (byte)1 : (byte)0);
            }
            else if (Sidebar.Width < 300 && Sidebar.Width > 55)
            {
                isAnimating = true;
                DoAnimation(_fullScreen ? (byte)3 : (byte)0);
            }
            else if (Sidebar.Width < 55 && _fullScreen)
            {
                isAnimating = true;
                DoAnimation((byte)2);
            }
        }
    }
}