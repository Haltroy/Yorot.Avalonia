using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;

namespace TabTest
{
    internal class YorotTabControl : Avalonia.Controls.Control
    {
        public List<YorotTabItem> Items { get; set; } = new List<YorotTabItem>();
        public int SelectedIndex = 0;

        public override void Render(DrawingContext context)
        {
            context.DrawRectangle(new Avalonia.Media.Pen(Avalonia.Media.Brush.Parse("#000000"), 5), new Rect(5, 5, 100, 100), 25);
            context.FillRectangle(Avalonia.Media.Brush.Parse("#0000FF"), new Rect(5, 5, 100, 100), 25);
            context.DrawText(Avalonia.Media.Brush.Parse("#FF0000"), new Point(5, 5), new FormattedText(Items[0].Text, Avalonia.Media.Typeface.Default, 25, TextAlignment.Center, TextWrapping.WrapWithOverflow, new Size(100, 100)));
            //base.Render(context);
        }

        protected override void OnPointerEnter(PointerEventArgs e)
        {
            base.OnPointerEnter(e);
        }

        protected override void OnPointerLeave(PointerEventArgs e)
        {
            base.OnPointerLeave(e);
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
        }

        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            base.OnPointerWheelChanged(e);
        }
    }

    internal class YorotTabItem : Avalonia.Controls.Control
    {
        public string Text { get; set; }
    }
}