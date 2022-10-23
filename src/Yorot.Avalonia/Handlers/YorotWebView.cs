using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefNet;
using CefNet.Avalonia;
using CefNet.Internal;

namespace Yorot_Avalonia.Handlers
{
    internal class YorotWebView : WebView
    {
        public object Window;
        public YorotGlue Glue;

        public YorotWebView(object window)
        {
            Window = window;
        }

        public YorotWebView(object window, WebView opener) : base(opener)
        {
            Window = window;
        }

        protected override WebViewGlue CreateWebViewGlue()
        {
            Glue = new YorotGlue(Window, this);
            return Glue;
        }
    }
}