using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using CefNet;
using CefNet.Avalonia;
using DynamicData;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Yorot;
using Yorot.AppForms;
using Yorot_Avalonia.Handlers;
using Yorot_Avalonia.ViewModels;

namespace Yorot_Avalonia.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.Opened += (sender, e) =>
            {
                if (!YorotGlobal.Main.CurrentSettings.SessionManager.PreviousShutdownWasSafe)
                {
                    MessageBox box = new(YorotGlobal.Main.CurrentLanguage.GetItemText("UI.RestoreSessionTitle"), YorotGlobal.Main.CurrentLanguage.GetItemText("UI.RestoreSessionDesc"), new MessageBoxButton[] { new MessageBoxButton.Yes(), new MessageBoxButton.No() });
                    RunDialog(box, new Action(() =>
                    {
                        if (box.DialogResult is MessageBoxButton.Yes && YorotGlobal.Main.CurrentSettings.SessionManager.Systems[YorotGlobal.Main.CurrentSettings.SessionManager.Systems.Count - 2] is SessionSystem session)
                        {
                            NewTab(session: session);
                        }
                    }));
                }
            };
        }

        private StackPanel? frmMain;
        private Grid? sidebarGrid;
        private DockPanel? Sidebar;
        private Panel? SidebarSplitter;
        public FluentAvalonia.UI.Controls.TabView? tabs;
        private Panel? AppGrid;
        public TextBox? findText;
        public Avalonia.Controls.CheckBox? MatchCase;
        public ToggleSwitch? AllowMic;
        public ToggleSwitch? AllowCam;
        public ToggleSwitch? AllowCookies;
        public ToggleSwitch? AllowYS;
        public ToggleSwitch? AllowPopup;
        public ToggleSwitch? AllowNotif;
        public ToggleSwitch? AllowNotifBoot;
        public Avalonia.Controls.ComboBox? NotifPriority;

        private Avalonia.Controls.MenuItem? other_bookmarks;
        private Avalonia.Controls.Menu? favoritesmenu;
        public TextBox? tbUrl;
        public System.Reactive.Subjects.Subject<bool>? IsNavigated;
        public System.Reactive.Subjects.Subject<bool>? IsNavigating;
        public System.Reactive.Subjects.Subject<bool>? CanGoBack;
        public System.Reactive.Subjects.Subject<bool>? CanGoForward;
        public System.Reactive.Subjects.Subject<bool>? CanZoomIn;
        public System.Reactive.Subjects.Subject<bool>? CanZoomOut;
        public System.Reactive.Subjects.Subject<bool>? IsFavorited;
        public System.Reactive.Subjects.Subject<bool>? IsNotFavorited;
        public System.Reactive.Subjects.Subject<bool>? IsMuted;
        public System.Reactive.Subjects.Subject<bool>? IsUnmuted;
        public System.Reactive.Subjects.Subject<bool>? IsPageSafe;
        public System.Reactive.Subjects.Subject<bool>? IsPageUsedCookie;
        public System.Reactive.Subjects.Subject<bool>? IsPageUnsafe;
        public System.Reactive.Subjects.Subject<string>? FindCount;
        public System.Reactive.Subjects.Subject<string>? ZoomLevel;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            if (YorotGlobal.Main != null)
            {
                Width = YorotGlobal.Main.CurrentSettings.LastSize.Width;
                Height = YorotGlobal.Main.CurrentSettings.LastSize.Height;
                Position = new PixelPoint(YorotGlobal.Main.CurrentSettings.LastLocation.X, YorotGlobal.Main.CurrentSettings.LastLocation.Y);
            }
            frmMain = this.FindControl<StackPanel>("frmMain");
            sidebarGrid = frmMain.FindControl<Grid>("sidebarGrid");
            Sidebar = sidebarGrid.FindControl<DockPanel>("Sidebar");
            AppGrid = Sidebar.FindControl<WrapPanel>("AppGrid");
            SidebarSplitter = sidebarGrid.FindControl<Panel>("SidebarSplitter");
            var contentcanvas = sidebarGrid.FindControl<Canvas>("ContentCanvas");
            tabs = contentcanvas.FindControl<TabView>("Tabs");
            tabs.AddTabButtonClick += Tabs_AddTabButtonClick;
            tabs.TabCloseRequested += Tabs_TabCloseRequested;
            tabs.TabDroppedOutside += Tabs_TabDroppedOutside;
            tabs.SelectionChanged += Tabs_SelectionChanged;

            this.PropertyChanged += MainWindow_PropertyChanged;
            this.Closed += MainWindow_Closed;

            var stackPanel1 = contentcanvas.FindControl<Grid>("dockPanel1");
            tbUrl = stackPanel1.FindControl<TextBox>("tbUrl");
            tbUrl.PropertyChanged += TbUrl_PropertyChanged;
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
            FindCount = new System.Reactive.Subjects.Subject<string>();
            ZoomLevel = new System.Reactive.Subjects.Subject<string>();

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
            FindCount.OnNext("");
            ZoomLevel.OnNext("");

            stackPanel1.FindControl<Avalonia.Controls.Button>("reload").Bind(IsVisibleProperty, IsNavigated);
            stackPanel1.FindControl<Avalonia.Controls.Button>("stop").Bind(IsVisibleProperty, IsNavigating);
            var goback = stackPanel1.FindControl<Avalonia.Controls.Button>("goback");
            goback.Bind(IsEnabledProperty, CanGoBack);
            var goforward = stackPanel1.FindControl<Avalonia.Controls.Button>("goforward");
            goforward.Bind(IsEnabledProperty, CanGoForward);
            stackPanel1.FindControl<Avalonia.Controls.Button>("favoritebutton").Bind(IsVisibleProperty, IsNotFavorited);
            stackPanel1.FindControl<Avalonia.Controls.Button>("favoritedbutton").Bind(IsVisibleProperty, IsFavorited);
            stackPanel1.FindControl<Avalonia.Controls.Button>("extbutton").ContextFlyout.Opening += Flyout_Opened;

            var securitybutton = stackPanel1.FindControl<Avalonia.Controls.Button>("SecurityInfo");

            if (securitybutton.Content is Panel securitypanel)
            {
                securitypanel.FindControl<Avalonia.Controls.Image>("WebsiteGood").Bind(IsVisibleProperty, IsPageSafe);
                securitypanel.FindControl<Avalonia.Controls.Image>("WebsiteMeh").Bind(IsVisibleProperty, IsPageUsedCookie);
                securitypanel.FindControl<Avalonia.Controls.Image>("WebsiteBad").Bind(IsVisibleProperty, IsPageUnsafe);
            }

            if (securitybutton.ContextFlyout is Flyout flyout && flyout.Content is StackPanel stackPanel)
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

            var backflyout = new Avalonia.Controls.MenuFlyout();
            goback.ContextFlyout = backflyout;

            backflyout.Opening += (sender, e) =>
            {
                if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.SessionSystem is SessionSystem sessionSystem)
                {
                    if (backflyout.Items is AvaloniaList<object> list)
                    {
                        list.Clear();
                        var before = sessionSystem.Before();
                        for (int i = before.Length - 1; i > 0; i--)
                        {
                            Avalonia.Controls.MenuItem mitem = new() { Name = i + "", Header = before[i].Title, Tag = before[i] };
                            mitem.Click += window.backforwarditem_click;
                            list.Add(mitem);
                        }
                    }
                }
            };

            var forwardflyout = new Avalonia.Controls.MenuFlyout();
            goforward.ContextFlyout = forwardflyout;

            forwardflyout.Opening += (sender, e) =>
            {
                if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.SessionSystem is SessionSystem sessionSystem)
                {
                    if (forwardflyout.Items is AvaloniaList<object> list)
                    {
                        list.Clear();
                        var after = sessionSystem.After();
                        for (int i = 0; i < after.Length; i++)
                        {
                            Avalonia.Controls.MenuItem mitem = new() { Name = i + "", Header = after[i].Title, Tag = after[i] };
                            mitem.Click += window.backforwarditem_click;
                            list.Add(mitem);
                        }
                    }
                }
            };

            var dotmenu = stackPanel1.FindControl<Avalonia.Controls.Button>("dotmenu").ContextFlyout;

            dotmenu.Placement = FlyoutPlacementMode.Bottom;
            dotmenu.ShowMode = FlyoutShowMode.Standard;

            if (dotmenu is Flyout dotflayout && dotflayout.Content is StackPanel menu)
            {
                // None of these are safe, changing the FlyOut content fucks stuff but Avalonia forced my hand. Too bad!

#pragma warning disable CS8602 // Dereference of a possibly null reference.

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
                (_featurespanel.Children[2] as Avalonia.Controls.Button).Click += screenshotbutton_click;
                (_featurespanel.Children[3] as Avalonia.Controls.Button).Click += savebutton_click;
                (_featurespanel.Children[4] as Avalonia.Controls.Button).Click += devtoolsbutton_click;

                // New Window
                (menu.Children[1] as Avalonia.Controls.Button).Click += (sender, e) => { NewWindow(); };
                (menu.Children[2] as Avalonia.Controls.Button).Click += NewIncWindow;
                (menu.Children[3] as Avalonia.Controls.Button).Click += ManageBookmarks;

                var _grid = menu.Children[4] as Grid;
                (_grid.Children[2] as TextBlock).Bind(TextBlock.TextProperty, FindCount);
                (_grid.Children[1] as Avalonia.Controls.Button).Click += findnextbutton_click;
                findText = (_grid.Children[0] as TextBox);
                findText.PropertyChanged += FindText_PropertyChanged;
                MatchCase = (_grid.Children[3] as Avalonia.Controls.CheckBox);
                MatchCase.Checked += MatchCaseChecked;
                MatchCase.Unchecked += MatchCaseUnchecked;

                var _zoom = menu.Children[5] as StackPanel;
                (_zoom.Children[1] as TextBlock).Bind(TextBlock.TextProperty, ZoomLevel);
                var zoomin = (_zoom.Children[2] as Avalonia.Controls.Button);
                zoomin.Bind(IsEnabledProperty, CanZoomIn);
                zoomin.Click += zoominbutton_click;
                var zoomout = (_zoom.Children[0] as Avalonia.Controls.Button);
                zoomout.Bind(IsEnabledProperty, CanZoomOut);
                zoomout.Click += zoomoutbutton_click;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
            this.KeyUp += TabWindow_KeyUp;

            favoritesmenu = contentcanvas.FindControl<Avalonia.Controls.Menu>("Favorites");
            other_bookmarks = favoritesmenu.FindControl<Avalonia.Controls.MenuItem>("other_bookmarks");

            RefreshFavorites(true);

            if (YorotGlobal.Main != null && YorotGlobal.Main.MainForms.Count <= 0)
            {
                YorotGlobal.Main.MainForms.Add(this);
                NewTab(url: "yorot://homepage", switchTo: true);
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
            if (sender is TextBox textbox && e.Property == TextBox.TextProperty && tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window)
            {
                window.tbUrl = textbox.Text;
            }
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window)
            {
                _tabSwitching = true;
                IsNavigated.OnNext(window.IsNavigated);
                IsNavigating.OnNext(window.IsNavigating);
                CanGoBack.OnNext(window.CanGoBack);
                CanGoForward.OnNext(window.CanGoForward);
                CanZoomIn.OnNext(window.CanZoomIn);
                CanZoomOut.OnNext(window.CanZoomOut);
                IsFavorited.OnNext(window.IsFavorited);
                IsNotFavorited.OnNext(window.IsNotFavorited);
                IsMuted.OnNext(window.IsMuted);
                IsUnmuted.OnNext(window.IsUnmuted);
                IsPageSafe.OnNext(window.IsPageSafe);
                IsPageUsedCookie.OnNext(window.IsPageUsedCookie);
                IsPageUnsafe.OnNext(window.IsPageUnsafe);
                tbUrl.Text = window.tbUrl;
                FindCount.OnNext(window.FindCount);
                ZoomLevel.OnNext(window.ZoomLevel);
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
        }

        private void ManageBookmarks(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            FavoriteWindow favorite = new() { DataContext = YorotGlobal.ViewModel };
            favorite.Show();
            favorite.BringIntoView();
        }

        private async void ProfileChangeName(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var msgbox = new MessageBox("Yorot", YorotGlobal.Main.CurrentLanguage.GetItemText("UI.ChangeProfileNameDesc"), YorotGlobal.Main.Profiles.Current.Text, new MessageBoxButton[] { new MessageBoxButton.Ok(), new MessageBoxButton.Cancel(), }, true);
            await msgbox.ShowDialog(this);
            if (msgbox.DialogResult is MessageBoxButton.Ok)
            {
                YorotGlobal.Main.Profiles.Current.Text = msgbox.Prompt;
            }
        }

        private void NotifPriorityChange(object? sender, SelectionChangedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.notifPriority = cBox.SelectedIndex - 1;
            }
        }

        private void AllowMicChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowMic.Allowance = shiftPressed ? YorotPermissionMode.AllowOneTime : YorotPermissionMode.Allow;
            }
        }

        private void DisableMicrophone(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowMic.Allowance = YorotPermissionMode.Deny;
            }
        }

        private void AllowCamChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowCam.Allowance = shiftPressed ? YorotPermissionMode.AllowOneTime : YorotPermissionMode.Allow;
            }
        }

        private void DisableCamera(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowCam.Allowance = YorotPermissionMode.Deny;
            }
        }

        private void AllowCookieChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowCookies.Allowance = shiftPressed ? YorotPermissionMode.AllowOneTime : YorotPermissionMode.Allow;
            }
        }

        private void DisableCookie(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowCookies.Allowance = YorotPermissionMode.Deny;
            }
        }

        private void AllowPopupChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowPopup.Allowance = shiftPressed ? YorotPermissionMode.AllowOneTime : YorotPermissionMode.Allow;
            }
        }

        private void DisablePopup(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowPopup.Allowance = YorotPermissionMode.Deny;
            }
        }

        private void AllowNotifChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowNotif.Allowance = shiftPressed ? YorotPermissionMode.AllowOneTime : YorotPermissionMode.Allow;
            }
        }

        private void DisableNotif(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowNotif.Allowance = YorotPermissionMode.Deny;
            }
        }

        private void AllowNotifBootChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.startNotifOnBoot = true;
            }
        }

        private void DisableNotifBoot(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.startNotifOnBoot = false;
            }
        }

        private void AllowYSChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
            {
                window.CurrentSite.Permissions.allowYS.Allowance = shiftPressed ? YorotPermissionMode.AllowOneTime : YorotPermissionMode.Allow;
            }
        }

        private void DisableYS(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dock && dock.Children[0] is TabWindow window && sender is Avalonia.Controls.ComboBox cBox)
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

        private void stop(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
            {
                webView.Stop();
            }
        }

        private void goback(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.SessionSystem is SessionSystem sessionSystem && window.webView1 != null && sessionSystem.CanGoBack)
            {
                window.bypassThisDeletion = true;
                sessionSystem.GoBack();
            }
        }

        private void goforward(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.SessionSystem is SessionSystem sessionSystem && window.webView1 != null && sessionSystem.CanGoForward)
            {
                window.bypassThisDeletion = true;
                sessionSystem.GoForward();
            }
        }

        private void urlkeydown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Enter)
            {
                if (tbUrl != null && tabs != null && YorotGlobal.Main != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
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

        private void gohome(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.SessionSystem is SessionSystem sessionSystem && window.webView1 != null && sessionSystem.CanGoForward && YorotGlobal.Main != null)
            {
                window.redirectTo(YorotGlobal.Main.CurrentSettings.HomePage, "");
                window.webView1.Navigate(YorotGlobal.Main.CurrentSettings.HomePage);
            }
        }

        private void reload(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
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

        private void favorite(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (IsFavorited is null || YorotGlobal.Main is null || IsNotFavorited is null) { return; }
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window)
            {
                var favbars = YorotGlobal.Main.CurrentSettings.FavManager.Favorites.FindAll(it => it.Name == "FavBar");
                if (favbars.Count > 0)
                {
                    var favbar = favbars[0];
                    favbar.Favorites.Add(new YorotFavorite(favbar, window.url, window.title));
                }
                else
                {
                    var favbar = new YorotFavFolder(YorotGlobal.Main.CurrentSettings.FavManager, "FavBar", "Favorites Bar");
                    YorotGlobal.Main.CurrentSettings.FavManager.Favorites.Add(favbar);
                    favbar.Favorites.Add(new YorotFavorite(favbar, window.url, window.title));
                }
            }
            RefreshFavorites(true);
            IsFavorited.OnNext(true);
            IsNotFavorited.OnNext(false);
        }

        private void unfavorite(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (YorotGlobal.Main is null || IsFavorited is null || tbUrl is null || IsNotFavorited is null) { return; }
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window)
            {
                var favs = YorotGlobal.Main.CurrentSettings.FavManager.GetFavorite(window.url);
                if (favs.Count > 0)
                {
                    favs[0].ParentFolder.Favorites.Remove(favs[0]);
                    RefreshFavorites(true);
                    IsFavorited.OnNext(true);
                    IsNotFavorited.OnNext(false);
                }
            }
        }

        public async void RunSaveFileDialog(string title, string[] filetypes, Action<string> OnSuccess)
        {
            if (YorotGlobal.Main != null)
            {
                SaveFileDialog saveFileDialog = new()
                {
                    Title = string.IsNullOrWhiteSpace(title) ? YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.SaveFileDialog") : title
                };
                List<FileDialogFilter> Filters = new();

                for (int i = 0; i < filetypes.Length; i++)
                {
                    FileDialogFilter filter = new();
                    List<string> extension = new()
                    {
                        filetypes[i]
                    };
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

        public async void RunDialog(Window window, Action OnSuccess)
        {
            await window.ShowDialog(this);
            OnSuccess();
        }

        private void printbutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
            {
                webView.Print();
            }
        }

        private void screenshotbutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (YorotGlobal.Main != null && tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
            {
                RunSaveFileDialog(YorotGlobal.Main.CurrentLanguage.GetItemText("UI.SaveScreenshot"),
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
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
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
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
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
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
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
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView && findText != null)
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
            if (sender is Avalonia.Controls.CheckBox checkBox && checkBox.IsChecked.HasValue && tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
            {
                window.MatchCase = checkBox.IsChecked.Value;
                if (webView != null && window.Searching && findText != null && window.Searching)
                {
                    webView.Find(window.findText, false, window.MatchCase, window.findNext);
                }
            }
        }

        private bool _tabSwitching = false;

        private void FindText_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (_tabSwitching) { _tabSwitching = false; return; }
            if (e.Property == TextBox.TextProperty && tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView && findText != null)
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
            if (tabs != null && tabs.SelectedItem is TabViewItem item && YorotGlobal.Main != null && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView)
            {
                var task = webView.GetMainFrame().GetSourceAsync(System.Threading.CancellationToken.None);
                if (task.IsCompletedSuccessfully)
                {
                    RunSaveFileDialog(YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.SaveFileDialog"),
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
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView && IsMuted != null && IsUnmuted != null)
            {
                webView.AudioMuted = true;
                IsMuted.OnNext(true);
                IsUnmuted.OnNext(false);
            }
        }

        private void unmutebutton_click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView && IsMuted != null && IsUnmuted != null)
            {
                webView.AudioMuted = false;
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

        private void favitem_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tabs != null && tabs.SelectedItem is TabViewItem item && item.Content is DockPanel dockPanel1 && dockPanel1.Children[0] is TabWindow window && window.webView1 is YorotWebView webView && sender is Avalonia.Controls.MenuItem mitem && mitem.Tag is YorotFavorite fav)
            {
                webView.Navigate(fav.Url);
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

        private void Tabs_TabDroppedOutside(TabView sender, TabViewTabDroppedOutsideEventArgs args)
        {
            if (YorotGlobal.Main is null) { return; }
            MainWindow window = new MainWindow();
            YorotGlobal.Main.MainForms.Add(window);
            if (window.tabs != null && window.tabs.TabItems is AvaloniaList<object> list && sender.TabItems is AvaloniaList<object> list2)
            {
                list2.Remove(args.Tab);
                list.Add(args.Tab);
            }
        }

        private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            if (sender.TabItems is AvaloniaList<object> list)
            {
                list.Remove(args.Item);
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
            }
        }

        public void NewTab(string url = "yorot://newtab", int index = -1, bool switchTo = false, SessionSystem? session = null)
        {
            if (tabs is null) { return; }
            TabViewItem item = new TabViewItem() { Header = "New Tab" };
            item.Bind(ForegroundProperty, tabs.GetObservable(ForegroundProperty));
            DockPanel panel = new();
            item.Content = panel;
            var tabform = new TabWindow(session) { mainWindow = this, _startUrl = url, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch };
            panel.Children.Add(tabform);
            if (tabs.TabItems is AvaloniaList<object> itemList)
            {
                itemList.Insert(index == -1 ? (itemList.Count > 0 ? itemList.Count : 0) : index, item);
                if (switchTo)
                {
                    tabs.SelectedItem = item;
                }
            }
        }

        public void NewWindow(string url = "yorot://newtab")
        {
            if (YorotGlobal.Main is null) { return; }
            var mainform = new MainWindow()
            {
                DataContext = YorotGlobal.ViewModel,
                IsVisible = true,
                IsEnabled = true,
                WindowState = WindowState.Normal,
                ShowInTaskbar = true,
                Position = new PixelPoint(YorotGlobal.Main.CurrentSettings.LastLocation.X, YorotGlobal.Main.CurrentSettings.LastLocation.Y),
                Width = YorotGlobal.Main.CurrentSettings.LastSize.Width,
                Height = YorotGlobal.Main.CurrentSettings.LastSize.Height
            };
            mainform.Activate();
            mainform.Show();
            mainform.BringIntoView();

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