using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using CefNet;
using DynamicData;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using System.Collections.Generic;

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
        }

        private StackPanel? frmMain;
        private Grid? sidebarGrid;
        private DockPanel? Sidebar;
        private Panel? SidebarSplitter;
        public FluentAvalonia.UI.Controls.TabView? tabs;
        private Panel? AppGrid;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            frmMain = this.FindControl<StackPanel>("frmMain");
            sidebarGrid = frmMain.FindControl<Grid>("sidebarGrid");
            Sidebar = sidebarGrid.FindControl<DockPanel>("Sidebar");
            AppGrid = Sidebar.FindControl<WrapPanel>("AppGrid");
            SidebarSplitter = sidebarGrid.FindControl<Panel>("SidebarSplitter");
            tabs = sidebarGrid.FindControl<TabView>("Tabs");
            tabs.AddTabButtonClick += Tabs_AddTabButtonClick;
            tabs.TabCloseRequested += Tabs_TabCloseRequested;
            tabs.TabDroppedOutside += Tabs_TabDroppedOutside;
            Sidebar.Background = Avalonia.Media.Brush.Parse("#ebebeb");
            SidebarSplitter.Background = Avalonia.Media.Brush.Parse("#0080ff");

            this.PropertyChanged += MainWindow_PropertyChanged;
            this.Closed += MainWindow_Closed;

            if (YorotGlobal.Main != null && YorotGlobal.Main.MainForms.Count <= 0)
            {
                YorotGlobal.Main.MainForms.Add(this);
                NewTab(url: "yorot://homepage", switchTo: true);
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
                CefNetApplication.Instance.Shutdown();
            }
        }

        public void NewTab(string url = "yorot://newtab", int index = -1, bool switchTo = false)
        {
            if (tabs is null) { return; }
            TabViewItem item = new TabViewItem() { Header = "New Tab" };
            item.Bind(ForegroundProperty, tabs.GetObservable(ForegroundProperty));
            DockPanel panel = new();
            panel.Margin = new Thickness(0, 0, 55, 10);
            item.Content = panel;
            var tabform = new TabWindow(this, url) { VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch };
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
        private readonly int _animSpeed = 5;
        private int _cycleSkip = 0;

        private void sidebarPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
#pragma warning disable CS0618 // Yes this is obsolete but the other Double Tap is breaking a lot. Why tf is this obsolete even?
            if (e.ClickCount > 1)
#pragma warning restore CS0618 // e.ClickCount is obsolete.
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
            else if (Sidebar.Width < (this.Width / 2) && Sidebar.Width > 100)
            {
                isAnimating = true;
                DoAnimation(prevAnimation == 0 ? (byte)4 : (byte)0);
            }
            else if (Sidebar.Width < 100 && Sidebar.Width > 55)
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