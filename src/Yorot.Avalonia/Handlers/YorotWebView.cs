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
        private Views.TabWindow tabWindow;

        public YorotWebView(Views.TabWindow tabWindow)
        {
            this.tabWindow = tabWindow;
        }

        public YorotWebView(Views.TabWindow tabWindow, WebView opener) : base(opener)
        {
            this.tabWindow = tabWindow;
        }

        protected override WebViewGlue CreateWebViewGlue()
        {
            return new YorotGlue(tabWindow, this);
        }
    }
}