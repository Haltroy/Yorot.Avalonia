using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;
using Xilium.CefGlue.Common.Handlers;
using Yorot_Avalonia.Views;

namespace Yorot.Handlers
{
    internal class YorotDisplayHandler : DisplayHandler
    {
        public TabWindow TabWindow;

        public YorotDisplayHandler(TabWindow tab)
        {
            TabWindow = tab;
        }

        protected override void OnAddressChange(CefBrowser browser, CefFrame frame, string url)
        {
            if (frame.IsMain)
            {
                TabWindow.ChangeAddress(url);
            }
            base.OnAddressChange(browser, frame, url);
        }

        protected override bool OnAutoResize(CefBrowser browser, ref CefSize newSize)
        {
            return base.OnAutoResize(browser, ref newSize);
        }

        protected override bool OnConsoleMessage(CefBrowser browser, CefLogSeverity level, string message, string source, int line)
        {
            return base.OnConsoleMessage(browser, level, message, source, line);
        }

        protected override bool OnCursorChange(CefBrowser browser, IntPtr cursorHandle, CefCursorType type, CefCursorInfo customCursorInfo)
        {
            return base.OnCursorChange(browser, cursorHandle, type, customCursorInfo);
        }

        protected override void OnFaviconUrlChange(CefBrowser browser, string[] iconUrls)
        {
            base.OnFaviconUrlChange(browser, iconUrls);
        }

        protected override void OnFullscreenModeChange(CefBrowser browser, bool fullscreen)
        {
            base.OnFullscreenModeChange(browser, fullscreen);
        }

        protected override void OnLoadingProgressChange(CefBrowser browser, double progress)
        {
            base.OnLoadingProgressChange(browser, progress);
        }

        protected override void OnStatusMessage(CefBrowser browser, string value)
        {
            base.OnStatusMessage(browser, value);
        }

        protected override void OnTitleChange(CefBrowser browser, string title)
        {
            if (TabWindow != null) { TabWindow.ChangeTitle(title); }
            base.OnTitleChange(browser, title);
        }

        protected override bool OnTooltip(CefBrowser browser, string text)
        {
            return base.OnTooltip(browser, text);
        }
    }
}