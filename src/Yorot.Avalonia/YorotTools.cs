using System;
using System.Drawing;
using Yorot;

namespace Yorot_Avalonia
{
    public static class YorotTools
    {
        public static System.Drawing.Image GetAppIcon(YorotApp app)
        {
            if (app.isSystemApp)
            {
                switch (app.AppIcon)
                {
                    default:
                    case "yorot.png":
                        return Properties.Resources.Yorot;

                    case "settings.png":
                        return Properties.Resources.Settings;

                    case "store.png":
                        return Properties.Resources.store;

                    case "calc.png":
                        return Properties.Resources.calc;

                    case "calendar.png":
                        return Properties.Resources.calendar;

                    case "notepad.png":
                        return Properties.Resources.notepad;

                    case "console.png":
                        return Properties.Resources.console;

                    case "colman.png":
                        return Properties.Resources.colman;

                    case "fileman.png":
                        return Properties.Resources.fileman;

                    case "yopad.png":
                        return Properties.Resources.kopad;

                    case "spacepass.png":
                        return Properties.Resources.spacepass;

                    case "downloads.png":
                        return Properties.Resources.downloads;
                }
            }
            else
            {
                return HTAlt.Tools.ReadFile(YorotGlobal.Main.AppsFolder + app.AppCodeName + "\\" + app.AppIcon, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        public static System.Drawing.Image GetProfilePicture(YorotProfile profile)
        {
            if (System.IO.File.Exists(profile.Path + "picture.png"))
            {
                Image img = YorotGlobal.Main.Profiles.Current.Picture;
                return YorotTools.ClipToCircle(img, new PointF(img.Width / 2, img.Height / 2), img.Width / 2);
            }
            else
            {
                return Properties.Resources.default_pofile_pic;
            }
        }

        public static System.Drawing.Image ThemeThumbnail(YorotTheme theme)
        {
            if (theme.isDefaultTheme)
            {
                switch (theme.ThumbLoc)
                {
                    default: case "YorotLight.png": return Properties.Resources.YorotLight;
                    case "YorotStone.png": return Properties.Resources.YorotStone;
                    case "YorotRazor.png": return Properties.Resources.YorotRazor;
                    case "YorotDark.png": return Properties.Resources.YorotDark;
                    case "YorotShadow.png": return Properties.Resources.YorotShadow;
                    case "YorotDeepBlue.png": return Properties.Resources.YorotDeepBlue;
                }
            }
            else
            {
                return HTAlt.Tools.ReadFile(YorotGlobal.Main.ThemesFolder + theme.CodeName + @"\" + theme.ThumbLoc, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        // Thanks to Tempuslight & krlzlx from StackOwerflow https://stackoverflow.com/a/47205281
        public static System.Drawing.Image ClipToCircle(System.Drawing.Image srcImage, System.Drawing.PointF center, float radius)
        {
            System.Drawing.Image dstImage = new System.Drawing.Bitmap(srcImage.Width, srcImage.Height, srcImage.PixelFormat);

            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(dstImage))
            {
                System.Drawing.RectangleF r = new System.Drawing.RectangleF(center.X - radius, center.Y - radius,
                                                         radius * 2, radius * 2);

                // enables smoothing of the edge of the circle (less pixelated)
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // fills background color
                using (System.Drawing.Brush br = new System.Drawing.SolidBrush(System.Drawing.Color.Transparent))
                {
                    g.FillRectangle(br, 0, 0, dstImage.Width, dstImage.Height);
                }

                // adds the new ellipse & draws the image again
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddEllipse(r);
                g.SetClip(path);
                g.DrawImage(srcImage, 0, 0);

                return dstImage;
            }
        }

        internal static System.Drawing.Image GetExtIcon(YorotExtension ext)
        {
            return HTAlt.Tools.ReadFile(YorotGlobal.Main.ExtFolder + ext.CodeName + "\\" + ext.Icon, System.Drawing.Imaging.ImageFormat.Png);
        }

        internal static System.Drawing.Image GetSiteIcon(YorotSite site)
        {
            if (System.IO.File.Exists(YorotGlobal.Main.SiteIconCache + site.Url))
            {
                return HTAlt.Tools.ReadFile(YorotGlobal.Main.SiteIconCache + site.Url, System.Drawing.Imaging.ImageFormat.Png);
            }
            else
            {
                return HTAlt.Tools.IsBright(YorotGlobal.Main.CurrentTheme.BackColor) ? Properties.Resources.globe_b : Properties.Resources.globe_w;
            }
        }
    }
}