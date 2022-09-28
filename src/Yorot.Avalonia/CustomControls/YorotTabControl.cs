using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;

namespace Yorot.CustomControls
{
    internal class YorotTabControl : Avalonia.Controls.Control
    {
        public List<YorotTabıtem> Items { get; set; } = new List<YorotTabıtem>();

        public override void Render(DrawingContext context)
        {
            base.Render(context);
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

    internal class YorotTabıtem : Avalonia.Controls.Control
    {
    }
}