using System.Data.SqlTypes;

namespace Yorot.Standard.YorotAppForms
{
    public class Control
    {
        public delegate bool OnDockChangedDelegate(Dock before, Dock after);

        public delegate bool OnHeightChangedDelegate(int before, int after);

        public delegate bool OnMaximumHeightChangedDelegate(int before, int after);

        public delegate bool OnMaximumWidthChangedDelegate(int before, int after);

        public delegate bool OnMaximumXChangedDelegate(int before, int after);

        public delegate bool OnMaximumYChangedDelegate(int before, int after);

        public delegate bool OnMinimumHeightChangedDelegate(int before, int after);

        public delegate bool OnMinimumWidthChangedDelegate(int before, int after);

        public delegate bool OnMinimumXChangedDelegate(int before, int after);

        public delegate bool OnMinimumYChangedDelegate(int before, int after);

        public delegate bool OnNameChangedDelegate(string before, string after);

        public delegate bool OnRecommendedHeightChangedDelegate(int before, int after);

        public delegate bool OnRecommendedWidthChangedDelegate(int before, int after);

        public delegate bool OnRecommendedXChangedDelegate(int before, int after);

        public delegate bool OnRecommendedYChangedDelegate(int before, int after);

        public delegate void OnRenderDelegate(DrawingContent content);

        public delegate bool OnWidthChangedDelegate(int before, int after);

        public delegate bool OnXChangedDelegate(int before, int after);

        public delegate bool OnYChangedDelegate(int before, int after);

        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int MaximumX { get; set; }
        public int MaximumY { get; set; }
        public int MaximumWidth { get; set; }
        public int MaximumHeight { get; set; }
        public int MinimumX { get; set; }
        public int MinimumY { get; set; }
        public int MinimumWidth { get; set; }
        public int MinimumHeight { get; set; }

        public int RecommendedX { get; set; }
        public int RecommendedY { get; set; }
        public int RecommendedWidth { get; set; }
        public int RecommendedHeight { get; set; }
        public Dock Dock { get; set; }

        public event OnRenderDelegate OnRender;
        public event OnDockChangedDelegate OnDockChanged;
        public event OnHeightChangedDelegate OnHeightChanged;
        public event OnMaximumHeightChangedDelegate OnMaximumHeightChanged;
        public event OnMaximumWidthChangedDelegate OnMaximumWidthChanged;
        public event OnMaximumXChangedDelegate OnMaximumXChanged;
        public event OnMaximumYChangedDelegate OnMaximumYChanged;
        public event OnMinimumHeightChangedDelegate OnMinimumHeightChanged;
        public event OnMinimumWidthChangedDelegate OnMinimumWidthChanged;
        public event OnMinimumXChangedDelegate OnMinimumXChanged;
        public event OnMinimumYChangedDelegate OnMinimumYChanged;
        public event OnNameChangedDelegate OnNameChanged;
        public event OnRecommendedHeightChangedDelegate OnRecommendedHeightChanged;
        public event OnRecommendedWidthChangedDelegate OnRecommendedWidthChanged;
        public event OnRecommendedXChangedDelegate OnRecommendedXChanged;
        public event OnRecommendedYChangedDelegate OnRecommendedYChanged;
        public event OnWidthChangedDelegate OnWidthChanged;
        public event OnXChangedDelegate OnXChanged;
        public event OnYChangedDelegate OnYChanged;
    }

    public enum Dock
    {
        Left,
        Right,
        Top,
        Bottom,
        Fill
    }

    public class DrawingContent
    {
        public Renderer Renderer { get; }

        public void DrawRectangle(int x, int y, int width, int height, Brush brush, bool fill = false,
            int cornerRadius = 0)
        {
            // TODO
        }

        public void DrawSquare(int x, int y, int size, Brush brush, bool fill = false, int cornerRadius = 0)
        {
            DrawRectangle(x, y, size, size, brush, fill, cornerRadius);
        }

        public void DrawEllipse(int x, int y, int r1, int r2, Brush brush, bool fill = false)
        {
            // TODO
        }

        public void DrawCircle(int x, int y, int radius, Brush brush, bool fill = false)
        {
            DrawEllipse(x, y, radius, radius, brush, fill);
        }

        public void DrawVectors(string path, int x, int y, Brush brush)
        {
            // TODO: parse path to vector array then use it in DrawVectors below
        }

        public void DrawVectors(VectorPath[] vector, int x, int y, Brush brush)
        {
            // TODO
        }

        public void DrawText(string text, float size, Brush brush, FontWeight weight = FontWeight.Regular,
            TextWrapping wrapping = null, TextFeatures features = TextFeatures.None)
        {
            // TODO
        }
    }

    public enum TextFeatures
    {
        None,
        Italic,

        /* Sans */
        Understrike,
        Strikethrough,

        /* From the creator of Understrike */
        Abovestrike
    }

    public class TextWrapping : INullable
    {
        public int AtChar { get; set; }
        public int AtWord { get; set; }
        public int AtWidth { get; set; }
        public int AtHeight { get; set; }
        public bool IsNull { get; }
    }

    public enum FontWeight
    {
        SuperLight,
        Light,
        Regular,
        Bold,
        SuperBold
    }

    public class VectorPath
    {
        public string Command { get; set; }
        public int[] Arguments { get; set; }
    }

    public abstract class Brush
    {
    }

    public class SolidColorBrush : Brush
    {
    }

    public class GradientBrush : Brush
    {
    }

    public class TileBrush : Brush
    {
    }

    public abstract class Renderer
    {
    }
}