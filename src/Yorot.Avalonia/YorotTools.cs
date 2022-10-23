using System;
using System.Drawing;
using Yorot;
using System.IO;
using System.Reflection;
using System.Drawing.Imaging;
using Avalonia.Platform;
using Avalonia;

namespace Yorot_Avalonia
{
    public static class YorotTools
    {
        /// <summary>
        /// Sets the <paramref name="site"/> image.
        /// </summary>
        /// <param name="site"><see cref="YorotSite"/></param>
        /// <param name="image">Favicon</param>
        /// <param name="main"><see cref="YorotMain"/></param>
        public static void SetSiteIcon(YorotSite site, Avalonia.Media.Imaging.Bitmap image, YorotMain main)
        {
            using (FileStream fs = new FileStream(main.AppPath + System.IO.Path.DirectorySeparatorChar + "favicons" + System.IO.Path.DirectorySeparatorChar + HTAlt.Tools.GetBaseURL(site.Url) + ".ico", FileMode.Create))
            {
                image.Save(fs);
            }
        }

        public static Avalonia.Media.IImage GetProfilePicture(YorotProfile profile)
        {
            return new Avalonia.Media.Imaging.Bitmap(HTAlt.Tools.ReadFile(profile.PicturePath));
        }

        public static Avalonia.Media.IImage ThemeThumbnail(YorotTheme theme)
        {
            if (theme.isDefaultTheme)
            {
                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                switch (theme.ThumbLoc)
                {
                    default: case "YorotLight.png": return new Avalonia.Media.Imaging.Bitmap(assets.Open(new Uri("avares://Yorot/Assets/YorotLight.png")));
                    case "YorotStone.png": return new Avalonia.Media.Imaging.Bitmap(assets.Open(new Uri("avares://Yorot/Assets/YorotStone.png")));
                    case "YorotRazor.png": return new Avalonia.Media.Imaging.Bitmap(assets.Open(new Uri("avares://Yorot/Assets/YorotRazor.png")));
                    case "YorotDark.png": return new Avalonia.Media.Imaging.Bitmap(assets.Open(new Uri("avares://Yorot/Assets/YorotDark.png")));
                    case "YorotShadow.png": return new Avalonia.Media.Imaging.Bitmap(assets.Open(new Uri("avares://Yorot/Assets/YorotShadow.png")));
                    case "YorotDeepBlue.png": return new Avalonia.Media.Imaging.Bitmap(assets.Open(new Uri("avares://Yorot/Assets/YorotDeepBlue.png")));
                }
            }
            else
            {
                return new Avalonia.Media.Imaging.Bitmap(HTAlt.Tools.ReadFile(YorotGlobal.Main.ThemesFolder + theme.CodeName + @"\" + theme.ThumbLoc));
            }
        }

        internal static Avalonia.Media.Imaging.Bitmap GetExtIcon(YorotExtension ext)
        {
            return new Avalonia.Media.Imaging.Bitmap(HTAlt.Tools.ReadFile(YorotGlobal.Main.ExtFolder + ext.CodeName + System.IO.Path.DirectorySeparatorChar + ext.Icon));
        }

        internal static Avalonia.Media.Imaging.Bitmap GetSiteIcon(YorotSite site)
        {
            if (System.IO.File.Exists(YorotGlobal.Main.SiteIconCache + site.Url))
            {
                return new Avalonia.Media.Imaging.Bitmap(HTAlt.Tools.ReadFile(YorotGlobal.Main.SiteIconCache + site.Url));
            }
            else
            {
                return new Avalonia.Media.Imaging.Bitmap(AvaloniaLocator.Current.GetService<IAssetLoader>().Open(new Uri("avares://Yorot_Avalonia/Assets/globe" + (YorotGlobal.Main.CurrentTheme.BackColor.IsBright ? " - b" : "-w") + ".png")));
            }
        }

        #region Graveyard

        //public static Avalonia.Media.Imaging.Bitmap ConvertToAvaloniaBitmap(this Image bitmap)
        //{
        //    if (bitmap == null)
        //        return null;
        //    System.Drawing.Bitmap bitmapTmp = new System.Drawing.Bitmap(bitmap);
        //    var bitmapdata = bitmapTmp.LockBits(new Rectangle(0, 0, bitmapTmp.Width, bitmapTmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        //    Avalonia.Media.Imaging.Bitmap bitmap1 = new(Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul,
        //        bitmapdata.Scan0,
        //        new Avalonia.PixelSize(bitmapdata.Width, bitmapdata.Height),
        //        new Avalonia.Vector(96, 96),
        //        bitmapdata.Stride);
        //    bitmapTmp.UnlockBits(bitmapdata);
        //    bitmapTmp.Dispose();
        //    return bitmap1;
        //}

        // NOTE: We might not need this if we can implement a way to read app icon from stream that is embedded to the app itself.
        // That way, any Yorot flavor can get the app icon and the apps' icon would be embedded to the DLL file of that app.
        // Also, we avoid using System.Drawing as much as possible cuz its no longer cross-platform.

        //public static System.Drawing.Image GetAppIcon(YorotApp app)
        //{
        //    if (app.isSystemApp)
        //    {
        //        switch (app.AppIcon)
        //        {
        //            default:
        //            case "yorot.png":
        //                return Properties.Resources.Yorot;

        //            case "settings.png":
        //                return Properties.Resources.Settings;

        //            case "store.png":
        //                return Properties.Resources.store;

        //            case "calc.png":
        //                return Properties.Resources.calc;

        //            case "calendar.png":
        //                return Properties.Resources.calendar;

        //            case "notepad.png":
        //                return Properties.Resources.notepad;

        //            case "console.png":
        //                return Properties.Resources.console;

        //            case "colman.png":
        //                return Properties.Resources.colman;

        //            case "fileman.png":
        //                return Properties.Resources.fileman;

        //            case "yopad.png":
        //                return Properties.Resources.kopad;

        //            case "downloads.png":
        //                return Properties.Resources.downloads;
        //        }
        //    }
        //    else
        //    {
        //        return HTAlt.Tools.ReadFile(YorotGlobal.Main.AppsFolder + app.AppCodeName + System.IO.Path.DirectorySeparatorChar + app.AppIcon, System.Drawing.Imaging.ImageFormat.Png);
        //    }
        //}

        //public static System.Drawing.Image ClipToCircle(System.Drawing.Image srcImage)
        //{
        //    return ClipToCircle(srcImage, new PointF(srcImage.Width / 2, srcImage.Height / 2), srcImage.Width / 2);
        //}

        // Thanks to Tempuslight & krlzlx from StackOwerflow https://stackoverflow.com/a/47205281
        //public static System.Drawing.Image ClipToCircle(System.Drawing.Image srcImage, System.Drawing.PointF center, float radius)
        //{
        //    System.Drawing.Image dstImage = new System.Drawing.Bitmap(srcImage.Width, srcImage.Height, srcImage.PixelFormat);

        //    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(dstImage))
        //    {
        //        System.Drawing.RectangleF r = new System.Drawing.RectangleF(center.X - radius, center.Y - radius,
        //                                                 radius * 2, radius * 2);

        //        // enables smoothing of the edge of the circle (less pixelated)
        //        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        //        // fills background color
        //        using (System.Drawing.Brush br = new System.Drawing.SolidBrush(System.Drawing.Color.Transparent))
        //        {
        //            g.FillRectangle(br, 0, 0, dstImage.Width, dstImage.Height);
        //        }

        //        // adds the new ellipse & draws the image again
        //        System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
        //        path.AddEllipse(r);
        //        g.SetClip(path);
        //        g.DrawImage(srcImage, 0, 0);

        //        return dstImage;
        //    }
        //}

        #endregion Graveyard
    }
}