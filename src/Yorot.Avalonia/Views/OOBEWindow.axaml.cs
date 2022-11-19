using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using CefNet;
using System.Reactive.Subjects;
using Yorot;
using Yorot_Avalonia.Handlers;

namespace Yorot_Avalonia.Views
{
    public partial class OOBEWindow : Window
    {
        public Subject<bool>? IsBackAllowed;
        public Subject<bool>? IsNextAllowed;
        public Subject<bool>? IsFinishAllowed;

        public OOBEWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private Carousel carousel;
        private ComboBox langBox;
        private ComboBox localeBox;
        private ComboBox datetimeBox;
        private TextBox profilename;
        private TextBox profileusername;
        private YorotLanguage selectedLang;
        private YorotTheme selectedTheme;
        private ToggleButton[] themes;
        private CheckBox importKorot;
        private CheckBox importChrome;
        private CheckBox importFirefox;
        private CheckBox importYorot;
        private CheckBox importEdge;
        private CheckBox importEdgeLegacy;
        private CheckBox importIE;
        private CheckBox importOpera;
        private CheckBox importChromium;
        private CheckBox importHTML;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            IsBackAllowed = new Subject<bool>();
            IsNextAllowed = new Subject<bool>();
            IsFinishAllowed = new Subject<bool>();

            var contentGrid = this.FindControl<Grid>("ContentGrid");
            carousel = contentGrid.FindControl<Carousel>("Carousel");
            var langselect = carousel.FindControl<StackPanel>("LangSelection");
            var langlocdft = langselect.FindControl<Grid>("LangLocDTF");
            langBox = langlocdft.FindControl<ComboBox>("LangList");

            if (langBox.Items is AvaloniaList<object> list)
            {
                for (int i = 0; i < YorotGlobal.Main.LangMan.Languages.Count; i++)
                {
                    var lang = YorotGlobal.Main.LangMan.Languages[i];
                    ComboBoxItem item = new()
                    {
                        Content = lang.Name,
                        Tag = lang,
                    };
                    list.Add(item);
                }
            }

            localeBox = langlocdft.FindControl<ComboBox>("LocList");

            localeBox.SelectionChanged += LocaleBox_SelectionChanged;

            datetimeBox = langlocdft.FindControl<ComboBox>("DateTimeList");
            langBox.SelectionChanged += (sender, e) =>
            {
                if (langBox.SelectedItem != null && langBox.SelectedItem is ComboBoxItem item && item.Tag is YorotLanguage lang)
                {
                    YorotGlobal.Main.CurrentSettings.CurrentLanguage = lang;
                    // TODO: Reload the entire window. Don't worry about the language thing because it is working, the UI isnt updated.
                }
                if (localeBox.SelectedIndex > -1 && langBox.SelectedIndex > -1 && datetimeBox.SelectedIndex > -1)
                {
                    IsNextAllowed.OnNext(true);
                }
                else
                {
                    IsNextAllowed.OnNext(true);
                }
            };

            datetimeBox.SelectionChanged += LocaleBox_SelectionChanged;

            var cefpanel = carousel.FindControl<Panel>("cefpanel");

            YorotWebView webview = new YorotWebView(this)
            {
                InitialUrl = "yorot://map",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
                Width = this.Width,
                Height = this.Height / 1.75,
            };

            webview.BrowserCreated += (sender, e) => { webview.Navigate("yorot://map"); };

            webview.DocumentTitleChanged += Webview_DocumentTitleChanged;

            webview.Navigating += Webview_Navigating;
            webview.Navigated += Webview_Navigated;
            webview.BeforeBrowse += Webview_BeforeBrowse;

            Closing += OOBEWindow_Closing;
            KeyDown += (sender, e) => { if (e.Key == Avalonia.Input.Key.F5) { webview.ReloadIgnoreCache(); webview.Navigate("yorot://map"); } };

            cefpanel.Children.Add(webview);

            var profilecreation = carousel.FindControl<StackPanel>("ProfileCreation");
            profilename = profilecreation.FindControl<TextBox>("ProfileName");
            profileusername = profilecreation.FindControl<TextBox>("ProfileCodeName");
            profilename.PropertyChanged += (sender, e) =>
            {
                if (e.Property == TextBox.TextProperty)
                {
                    profileusername.Text = profilename.Text
                .ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("\\", "")
                .Replace("/", "")
                .Replace(":", "")
                .Replace("*", "")
                .Replace("?", "")
                .Replace("\"", "")
                .Replace("<", "")
                .Replace(">", "")
                .Replace("|", "");
                    if (!string.IsNullOrWhiteSpace(profilename.Text) && !string.IsNullOrWhiteSpace(profileusername.Text) &&
                    !(profileusername.Text.Contains(" ") ||
                    profileusername.Text.Contains("\\") ||
                    profileusername.Text.Contains("/") ||
                    profileusername.Text.Contains(":") ||
                    profileusername.Text.Contains("*") ||
                    profileusername.Text.Contains("?") ||
                    profileusername.Text.Contains("\"") ||
                    profileusername.Text.Contains("<") ||
                    profileusername.Text.Contains(">") ||
                    profileusername.Text.Contains("|")))
                    {
                        IsNextAllowed.OnNext(true);
                    }
                    else
                    {
                        IsNextAllowed.OnNext(false);
                    }
                }
            };

            profileusername.PropertyChanged += (sender, e) =>
            {
                if (e.Property == TextBox.TextProperty)
                {
                    if (!string.IsNullOrWhiteSpace(profilename.Text) && !string.IsNullOrWhiteSpace(profileusername.Text) &&
                    !(profileusername.Text.Contains(" ") ||
                    profileusername.Text.Contains("\\") ||
                    profileusername.Text.Contains("/") ||
                    profileusername.Text.Contains(":") ||
                    profileusername.Text.Contains("*") ||
                    profileusername.Text.Contains("?") ||
                    profileusername.Text.Contains("\"") ||
                    profileusername.Text.Contains("<") ||
                    profileusername.Text.Contains(">") ||
                    profileusername.Text.Contains("|")))
                    {
                        IsNextAllowed.OnNext(true);
                    }
                    else
                    {
                        IsNextAllowed.OnNext(false);
                    }
                }
            };

            var themepanel = carousel.FindControl<StackPanel>("ThemeSelect").FindControl<WrapPanel>("Themes");

            var togglebuttons = new System.Collections.Generic.List<ToggleButton>();

            for (int i = 0; i < YorotGlobal.Main.ThemeMan.Themes.Count; i++)
            {
                var theme = YorotGlobal.Main.ThemeMan.Themes[i];
                ToggleButton button = new()
                {
                    Tag = theme,
                    Margin = new Thickness(5, 5, 5, 5),
                    IsChecked = YorotGlobal.Main.CurrentTheme == theme,
                };
                StackPanel pnl = new();
                Image img = new() { Source = YorotTools.ThemeThumbnail(theme), Width = 128, Height = 128 };
                pnl.Children.Add(img);
                TextBlock name = new() { Text = theme.Name };
                pnl.Children.Add(name);
                button.Content = pnl;

                togglebuttons.Add(button);

                button.Checked += (sender, e) =>
                {
                    for (int _i = 0; _i < themes.Length; _i++)
                    {
                        var themebutton = themes[_i];
                        if (themebutton != sender)
                        {
                            themebutton.IsChecked = false;
                        }
                    }
                    selectedTheme = theme;
                    YorotGlobal.Main.CurrentSettings.CurrentTheme = theme;
                };

                themepanel.Children.Add(button);
            }

            themes = togglebuttons.ToArray();

            var navbar = contentGrid.FindControl<StackPanel>("Navigation");
            var backbutton = navbar.FindControl<Button>("Back");
            var nextbutton = navbar.FindControl<Button>("Next");
            var finishbutton = navbar.FindControl<Button>("Finish");
            backbutton.Bind(IsVisibleProperty, IsBackAllowed);
            backbutton.Bind(IsEnabledProperty, IsBackAllowed);
            nextbutton.Bind(IsVisibleProperty, IsNextAllowed);
            nextbutton.Bind(IsEnabledProperty, IsNextAllowed);
            finishbutton.Bind(IsVisibleProperty, IsFinishAllowed);
            finishbutton.Bind(IsEnabledProperty, IsFinishAllowed);

            var importfrom = carousel.FindControl<StackPanel>("ImportFrom").FindControl<StackPanel>("Browsers");

            importKorot = importfrom.FindControl<CheckBox>("Korot");
            importChrome = importfrom.FindControl<CheckBox>("Chrome");
            importFirefox = importfrom.FindControl<CheckBox>("Firefox");
            importYorot = importfrom.FindControl<CheckBox>("OtherFlavor");
            importEdge = importfrom.FindControl<CheckBox>("Edge");
            importEdgeLegacy = importfrom.FindControl<CheckBox>("EdgeLegacy");
            importIE = importfrom.FindControl<CheckBox>("IntExp");
            importOpera = importfrom.FindControl<CheckBox>("Opera");
            importChromium = importfrom.FindControl<CheckBox>("Chromium");
            importHTML = importfrom.FindControl<CheckBox>("HTML");

            IsBackAllowed.OnNext(false);
            IsNextAllowed.OnNext(false);
            IsFinishAllowed.OnNext(false);
        }

        private void OOBEWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            CefNetApplication.Instance.Shutdown();
            CefNetApplication.Instance.Dispose();
        }

        private void Webview_BeforeBrowse(object? sender, CefNet.BeforeBrowseEventArgs e)
        {
            // Block navigation to other sites, only yorot://map
            if (e.RawUrl != "yorot://map")
            {
                if (sender is YorotWebView webview)
                {
                    webview.Navigate("yorot://map");
                }
            }
        }

        private void Webview_Navigated(object? sender, CefNet.NavigatedEventArgs e)
        {
            // Block navigation to other sites, only yorot://map
            if (e.Url != "yorot://map")
            {
                if (sender is YorotWebView webview)
                {
                    webview.Navigate("yorot://map");
                }
            }
        }

        private void LocaleBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (localeBox.SelectedIndex > -1 && langBox.SelectedIndex > -1 && datetimeBox.SelectedIndex > -1)
            {
                IsNextAllowed.OnNext(true);
            }
            else
            {
                IsNextAllowed.OnNext(true);
            }
        }

        private void Webview_Navigating(object? sender, CefNet.BeforeBrowseEventArgs e)
        {
            // Block navigation to other sites, only yorot://map
            if (e.RawUrl != "yorot://map")
            {
                if (sender is YorotWebView webview)
                {
                    webview.Navigate("yorot://map");
                }
            }
        }

        private YorotLocaleMap LocaleMap = new YorotLocaleMap();
        private YorotLocale selectedLocale;

        private void Webview_DocumentTitleChanged(object? sender, CefNet.DocumentTitleChangedEventArgs e)
        {
            var locale = LocaleMap.GetMapItem(e.Title);

            if (locale != null)
            {
                // Select Language

                if (YorotGlobal.Main.LangMan.GetLangByCN(locale.LanguageID) is YorotLanguage lang)
                {
                    if (langBox.Items is AvaloniaList<object> _list)
                    {
                        for (int i = 0; i < _list.Count; i++)
                        {
                            if (_list[i] is ComboBoxItem item)
                            {
                                if (item.Tag == lang)
                                {
                                    langBox.SelectedIndex = i;
                                    YorotGlobal.Main.CurrentSettings.CurrentLanguage = lang;
                                    selectedLang = lang;
                                    break;
                                }
                            }
                        }
                    }
                }

                // Select Locale
                if (localeBox != null && localeBox.Items is AvaloniaList<object> list)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] is ComboBoxItem item)
                        {
                            if (item.Name == locale.Locale.ToString())
                            {
                                localeBox.SelectedIndex = i;
                                selectedLocale = locale.Locale;
                                break;
                            }
                        }
                    }
                }

                // Set Date-Time Format

                if (locale.DateTimeFormats.Length > 0)
                {
                    if (locale.DateTimeFormats[0] == Yorot.YorotDateAndTime.DMY)
                    {
                        datetimeBox.SelectedIndex = 0;
                    }
                    else if (locale.DateTimeFormats[0] == Yorot.YorotDateAndTime.MDY)
                    {
                        datetimeBox.SelectedIndex = 1;
                    }
                    else if (locale.DateTimeFormats[0] == Yorot.YorotDateAndTime.YMD)
                    {
                        datetimeBox.SelectedIndex = 2;
                    }
                }
            }
        }

        public void CarouselNext(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (carousel != null && IsNextAllowed != null && IsBackAllowed != null && IsFinishAllowed != null)
            {
                carousel.Next();
                CheckIfNav();
            }
        }

        private void CheckIfNav()
        {
            if (carousel.SelectedIndex == carousel.ItemCount - 1)
            {
                IsNextAllowed.OnNext(false);
                IsFinishAllowed.OnNext(true);
                IsBackAllowed.OnNext(true);
            }
            else if (carousel.SelectedIndex == 0)
            {
                IsNextAllowed.OnNext(true);
                IsBackAllowed.OnNext(false);
                IsFinishAllowed.OnNext(false);
            }
            else
            {
                IsNextAllowed.OnNext(true);
                IsBackAllowed.OnNext(true);
                IsFinishAllowed.OnNext(false);
            }
            if (carousel.SelectedIndex == 1 && (string.IsNullOrWhiteSpace(profilename.Text) || string.IsNullOrWhiteSpace(profileusername.Text)))
            {
                IsNextAllowed.OnNext(false);
            }
        }

        public void CarouselPrevious(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (carousel != null && IsNextAllowed != null && IsBackAllowed != null && IsFinishAllowed != null)
            {
                carousel.Previous();
                CheckIfNav();
            }
        }

        public void Finish(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // Create the profile and switch to it
            var profile = new YorotProfile(profileusername.Text, profilename.Text, YorotGlobal.Main.Profiles);
            YorotGlobal.Main.Profiles.Profiles.Add(profile);
            YorotGlobal.Main.Profiles.Current = profile;
            profile.Settings.CurrentTheme = selectedTheme;
            profile.Settings.CurrentLanguage = selectedLang;
            profile.Manager.Main.Locale = selectedLocale;
            switch (datetimeBox.SelectedIndex)
            {
                case 0:
                    profile.Settings.DateFormat = YorotDateAndTime.DMY;
                    break;

                case 1:
                    profile.Settings.DateFormat = YorotDateAndTime.YMD;
                    break;

                case 2:
                    profile.Settings.DateFormat = YorotDateAndTime.MDY;
                    break;
            }
            // TODO: Import from other browsers

            // Shutdown
            YorotGlobal.Main.Shutdown(true);
            CefNetApplication.Instance.Shutdown();
            this.Close();
            System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
        }
    }
}