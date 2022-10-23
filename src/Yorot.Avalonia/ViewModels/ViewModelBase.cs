using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using HTAlt;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yorot_Avalonia.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        public Avalonia.Media.IBrush BackColor { get => Avalonia.Media.Brush.Parse(YorotGlobal.Main != null ? YorotGlobal.Main.CurrentTheme.BackColor.ToHex() : Yorot.DefaultThemes.YorotLight.BackColor.ToHex()); }
        public Avalonia.Media.IBrush BackColor2 { get => Avalonia.Media.Brush.Parse(YorotGlobal.Main != null ? YorotGlobal.Main.CurrentTheme.BackColor2.ToHex() : Yorot.DefaultThemes.YorotLight.BackColor2.ToHex()); }
        public Avalonia.Media.IBrush BackColor3 { get => Avalonia.Media.Brush.Parse(YorotGlobal.Main != null ? YorotGlobal.Main.CurrentTheme.BackColor3.ToHex() : Yorot.DefaultThemes.YorotLight.BackColor3.ToHex()); }
        public Avalonia.Media.IBrush BackColor4 { get => Avalonia.Media.Brush.Parse(YorotGlobal.Main != null ? YorotGlobal.Main.CurrentTheme.BackColor4.ToHex() : Yorot.DefaultThemes.YorotLight.BackColor4.ToHex()); }
        public Avalonia.Media.IBrush ForeColor { get => Avalonia.Media.Brush.Parse(YorotGlobal.Main != null ? YorotGlobal.Main.CurrentTheme.ForeColor.ToHex() : Yorot.DefaultThemes.YorotLight.ForeColor.ToHex()); }
        public Avalonia.Media.IBrush OverlayColor { get => Avalonia.Media.Brush.Parse(YorotGlobal.Main != null ? YorotGlobal.Main.CurrentTheme.OverlayColor.ToHex() : Yorot.DefaultThemes.YorotLight.OverlayColor.ToHex()); }
        public Avalonia.Media.IBrush OverlayColor2 { get => Avalonia.Media.Brush.Parse(YorotGlobal.Main != null ? YorotGlobal.Main.CurrentTheme.OverlayColor2.ToHex() : Yorot.DefaultThemes.YorotLight.OverlayColor2.ToHex()); }
        public Avalonia.Media.IBrush OverlayColor3 { get => Avalonia.Media.Brush.Parse(YorotGlobal.Main != null ? YorotGlobal.Main.CurrentTheme.OverlayColor3.ToHex() : Yorot.DefaultThemes.YorotLight.OverlayColor3.ToHex()); }
        public Avalonia.Media.IBrush OverlayColor4 { get => Avalonia.Media.Brush.Parse(YorotGlobal.Main != null ? YorotGlobal.Main.CurrentTheme.OverlayColor4.ToHex() : Yorot.DefaultThemes.YorotLight.OverlayColor4.ToHex()); }
        public Avalonia.Media.IBrush ArtColor { get => Avalonia.Media.Brush.Parse(YorotGlobal.Main != null ? YorotGlobal.Main.CurrentTheme.ArtColor.ToHex() : Yorot.DefaultThemes.YorotLight.ArtColor.ToHex()); }
        public Avalonia.Media.IBrush ArtColor2 { get => Avalonia.Media.Brush.Parse(YorotGlobal.Main != null ? YorotGlobal.Main.CurrentTheme.ArtColor2.ToHex() : Yorot.DefaultThemes.YorotLight.ArtColor2.ToHex()); }
        public Avalonia.Media.IBrush ArtColor3 { get => Avalonia.Media.Brush.Parse(YorotGlobal.Main != null ? YorotGlobal.Main.CurrentTheme.ArtColor3.ToHex() : Yorot.DefaultThemes.YorotLight.ArtColor3.ToHex()); }
        public Avalonia.Media.IBrush ArtColor4 { get => Avalonia.Media.Brush.Parse(YorotGlobal.Main != null ? YorotGlobal.Main.CurrentTheme.ArtColor4.ToHex() : Yorot.DefaultThemes.YorotLight.ArtColor4.ToHex()); }
        public Avalonia.Media.IBrush OverlayForeColor { get => Avalonia.Media.Brush.Parse(YorotGlobal.Main != null ? YorotGlobal.Main.CurrentTheme.OverlayForeColor.ToHex() : Yorot.DefaultThemes.YorotLight.OverlayForeColor.ToHex()); }
        public Avalonia.Media.IBrush ArtForeColor { get => Avalonia.Media.Brush.Parse(YorotGlobal.Main != null ? YorotGlobal.Main.CurrentTheme.ArtForeColor.ToHex() : Yorot.DefaultThemes.YorotLight.ArtForeColor.ToHex()); }

        public bool IsBackDark => !YorotGlobal.Main.CurrentTheme.BackColor.IsBright;
        public bool IsForeDark => !YorotGlobal.Main.CurrentTheme.ForeColor.IsBright;
        public bool IsOverlayDark => !YorotGlobal.Main.CurrentTheme.OverlayColor.IsBright;
        public bool IsOverlayForeDark => !YorotGlobal.Main.CurrentTheme.OverlayForeColor.IsBright;
        public bool IsArtDark => !YorotGlobal.Main.CurrentTheme.ArtColor.IsBright;
        public bool IsArtForeDark => !YorotGlobal.Main.CurrentTheme.ArtForeColor.IsBright;

        public string UserName => YorotGlobal.Main.Profiles.Current.Text;

        public IImage UserProfilePicture
        {
            get
            {
                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                return !System.IO.File.Exists(YorotGlobal.Main.Profiles.Current.PicturePath)
                    ? new Avalonia.Media.Imaging.Bitmap(assets.Open(new Uri("avares://Yorot/Assets/userdefault" + (IsBackDark ? "-w" : "-b") + ".png")))
                    : YorotTools.GetProfilePicture(YorotGlobal.Main.Profiles.Current);
            }
        }

        #region Translations

        #region Locale

        public string Locale_ar => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.ar");
        public string Locale_am => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.am");
        public string Locale_bg => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.bg");
        public string Locale_bn => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.bn");
        public string Locale_ca => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.ca");
        public string Locale_cs => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.cs");
        public string Locale_da => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.da");
        public string Locale_de => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.de");
        public string Locale_el => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.el");
        public string Locale_en => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.en");
        public string Locale_en_GB => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.en_GB");
        public string Locale_en_US => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.en_US");
        public string Locale_es => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.es");
        public string Locale_es_419 => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.es_419");
        public string Locale_et => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.et");
        public string Locale_fa => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.fa");
        public string Locale_fi => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.fi");
        public string Locale_fil => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.fil");
        public string Locale_fr => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.fr");
        public string Locale_gu => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.gu");
        public string Locale_he => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.he");
        public string Locale_hi => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.hi");
        public string Locale_hr => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.hr");
        public string Locale_hu => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.hu");
        public string Locale_id => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.id");
        public string Locale_it => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.it");
        public string Locale_ja => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.ja");
        public string Locale_kn => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.kn");
        public string Locale_ko => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.ko");
        public string Locale_lt => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.lt");
        public string Locale_lv => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.lv");
        public string Locale_ml => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.ml");
        public string Locale_mr => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.mr");
        public string Locale_ms => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.ms");
        public string Locale_nl => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.nl");
        public string Locale_no => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.no");
        public string Locale_pl => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.pl");
        public string Locale_pt_BR => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.pt_BR");
        public string Locale_pt_PT => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.pt_PT");
        public string Locale_ro => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.ro");
        public string Locale_ru => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.ru");
        public string Locale_sk => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.sk");
        public string Locale_sl => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.sl");
        public string Locale_sr => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.sr");
        public string Locale_sv => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.sv");
        public string Locale_sw => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.sw");
        public string Locale_ta => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.ta");
        public string Locale_te => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.te");
        public string Locale_th => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.th");
        public string Locale_tr => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.tr");
        public string Locale_uk => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.uk");
        public string Locale_vi => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.vi");
        public string Locale_zh_CN => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.zh_CN");
        public string Locale_zh_TW => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.zh_TW");
        public string Locale_ => YorotGlobal.Main.CurrentLanguage.GetItemText("Locales.");

        #endregion Locale

        #region OOBE

        public string OOBELang => YorotGlobal.Main.CurrentLanguage.GetItemText("OOBE.Lang");
        public string OOBELocale => YorotGlobal.Main.CurrentLanguage.GetItemText("OOBE.Locale");
        public string OOBEDateTimeFormat => YorotGlobal.Main.CurrentLanguage.GetItemText("OOBE.DateTimeFormat");
        public string OOBEDateTimeDMY => YorotGlobal.Main.CurrentLanguage.GetItemText("OOBE.DateTimeDMY");
        public string OOBEDateTimeMDY => YorotGlobal.Main.CurrentLanguage.GetItemText("OOBE.DateTimeMDY");
        public string OOBEDateTimeYMD => YorotGlobal.Main.CurrentLanguage.GetItemText("OOBE.DateTimeYMD");
        public string OOBEWelcome => YorotGlobal.Main.CurrentLanguage.GetItemText("OOBE.Welcome");
        public string OOBEProfileCreation => YorotGlobal.Main.CurrentLanguage.GetItemText("OOBE.ProfileCreation");
        public string OOBEProfileName => YorotGlobal.Main.CurrentLanguage.GetItemText("OOBE.ProfileName");
        public string OOBEProfileUserName => YorotGlobal.Main.CurrentLanguage.GetItemText("OOBE.ProfileUserName");
        public string OOBEThemeTitle => YorotGlobal.Main.CurrentLanguage.GetItemText("OOBE.ThemeTitle");
        public string OOBEThemeDesc => YorotGlobal.Main.CurrentLanguage.GetItemText("OOBE.ThemeDesc");
        public string OOBEThemeHint => YorotGlobal.Main.CurrentLanguage.GetItemText("OOBE.ThemeHint");
        public string OOBEImportTitle => YorotGlobal.Main.CurrentLanguage.GetItemText("OOBE.ImportTitle");
        public string OOBEImportDesc => YorotGlobal.Main.CurrentLanguage.GetItemText("OOBE.ImportDesc");
        public string OOBEOtherFlavor => YorotGlobal.Main.CurrentLanguage.GetItemText("OOBE.OtherFlavor");
        public string OOBEChromium => YorotGlobal.Main.CurrentLanguage.GetItemText("OOBE.Chromium");
        public string OOBEHTML => YorotGlobal.Main.CurrentLanguage.GetItemText("OOBE.HTML");
        public string OOBEBack => YorotGlobal.Main.CurrentLanguage.GetItemText("OOBE.Back");
        public string OOBENext => YorotGlobal.Main.CurrentLanguage.GetItemText("OOBE.Next");
        public string OOBEFinish => YorotGlobal.Main.CurrentLanguage.GetItemText("OOBE.Finish");

        #endregion OOBE

        public string NewWindow => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.NewWindow");
        public string NewIncognitoWindow => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.NewIncognitoWindow");
        public string SearchOnPage => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.SearchOnPage");
        public string MatchCase => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.MatchCase");
        public string OtherBookmarks => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.OtherBookmarks");
        public string ProfileChangeName => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.ProfileChangeName");
        public string ProfileChangeImage => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.ProfileChangeImage");
        public string ProfileSwitch => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.ProfileSwitch");
        public string ConnectionSafe => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.ConnectionSafe");
        public string ConnectionNotSafe => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.ConnectionNotSafe");
        public string PageSafe => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.PageSafe");
        public string PageSafeDesc => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.PageSafeDesc");
        public string PageUsedCookie => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.PageUsedCookies");
        public string PageUsedCookieDesc => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.PageUsedCookiesDesc");
        public string AllowMicrophone => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.AllowMicrophone");
        public string AllowCamera => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.AllowCamera");
        public string AllowYS => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.AllowYS");
        public string AllowNotif => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.AllowNotif");
        public string StartNotifOnBoot => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.StartNotifOnBoot");
        public string NotifPriority => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.NotifPriority");
        public string NotifPriority1 => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.NotifPriority1");
        public string NotifPriority2 => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.NotifPriority2");
        public string NotifPriority3 => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.NotifPriority3");

        #endregion Translations
    }
}