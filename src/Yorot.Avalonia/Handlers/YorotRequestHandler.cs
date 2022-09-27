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
    internal class YorotRequestHandler : RequestHandler
    {
        public TabWindow TabWindow;

        public YorotRequestHandler(TabWindow tabWindow)
        {
            TabWindow = tabWindow;
        }

        protected override bool GetAuthCredentials(CefBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, CefAuthCallback callback)
        {
            return base.GetAuthCredentials(browser, originUrl, isProxy, host, port, realm, scheme, callback);
        }

        protected override CefResourceRequestHandler GetResourceRequestHandler(CefBrowser browser, CefFrame frame, CefRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            return base.GetResourceRequestHandler(browser, frame, request, isNavigation, isDownload, requestInitiator, ref disableDefaultHandling);
        }

        protected override bool OnBeforeBrowse(CefBrowser browser, CefFrame frame, CefRequest request, bool userGesture, bool isRedirect)
        {
            return base.OnBeforeBrowse(browser, frame, request, userGesture, isRedirect);
        }

        protected override bool OnCertificateError(CefBrowser browser, CefErrorCode certError, string requestUrl, CefSslInfo sslInfo, CefRequestCallback callback)
        {
            return base.OnCertificateError(browser, certError, requestUrl, sslInfo, callback);
        }

        protected override void OnDocumentAvailableInMainFrame(CefBrowser browser)
        {
            base.OnDocumentAvailableInMainFrame(browser);
        }

        protected override bool OnOpenUrlFromTab(CefBrowser browser, CefFrame frame, string targetUrl, CefWindowOpenDisposition targetDisposition, bool userGesture)
        {
            return base.OnOpenUrlFromTab(browser, frame, targetUrl, targetDisposition, userGesture);
        }

        protected override void OnPluginCrashed(CefBrowser browser, string pluginPath)
        {
            base.OnPluginCrashed(browser, pluginPath);
        }

        protected override bool OnQuotaRequest(CefBrowser browser, string originUrl, long newSize, CefRequestCallback callback)
        {
            return base.OnQuotaRequest(browser, originUrl, newSize, callback);
        }

        protected override void OnRenderProcessTerminated(CefBrowser browser, CefTerminationStatus status)
        {
            base.OnRenderProcessTerminated(browser, status);
        }

        protected override void OnRenderViewReady(CefBrowser browser)
        {
            base.OnRenderViewReady(browser);
        }

        protected override bool OnSelectClientCertificate(CefBrowser browser, bool isProxy, string host, int port, CefX509Certificate[] certificates, CefSelectClientCertificateCallback callback)
        {
            return base.OnSelectClientCertificate(browser, isProxy, host, port, certificates, callback);
        }
    }
}