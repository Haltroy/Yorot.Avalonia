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

        public bool IsBackDark => !HTAlt.Tools.IsBright(YorotGlobal.Main.CurrentTheme.BackColor);
        public bool IsForeDark => !HTAlt.Tools.IsBright(YorotGlobal.Main.CurrentTheme.ForeColor);
        public bool IsOverlayDark => !HTAlt.Tools.IsBright(YorotGlobal.Main.CurrentTheme.OverlayColor);
        public bool IsOverlayForeDark => !HTAlt.Tools.IsBright(YorotGlobal.Main.CurrentTheme.OverlayForeColor);
        public bool IsArtDark => !HTAlt.Tools.IsBright(YorotGlobal.Main.CurrentTheme.ArtColor);
        public bool IsArtForeDark => !HTAlt.Tools.IsBright(YorotGlobal.Main.CurrentTheme.ArtForeColor);

        #region Translations

        public string NewWindow => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.NewWindow");
        public string NewIncognitoWindow => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.NewIncognitoWindow");
        public string SearchOnPage => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.SearchOnPage");
        public string MatchCase => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.MatchCase");
        public string OtherBookmarks => YorotGlobal.Main.CurrentLanguage.GetItemText("UI.OtherBookmarks");

        #endregion Translations
    }
}