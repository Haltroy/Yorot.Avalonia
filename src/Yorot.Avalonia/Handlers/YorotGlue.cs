using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using CefNet;
using CefNet.Avalonia;
using CefNet.Internal;
using CefNet.Net;
using FluentAvalonia.UI.Controls;
using Yorot;
using Yorot_Avalonia.Views;
using static Yorot.DefaultApps;

namespace Yorot_Avalonia.Handlers
{
    public class YorotSchemeHandlerFactory : CefSchemeHandlerFactory
    {
        protected override CefResourceHandler Create(CefBrowser browser, CefFrame frame, string schemeName, CefRequest request)
        {
            return base.Create(browser, frame, schemeName, request);
        }
    }

    public class YorotWebView : WebView
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

    public class YorotGlue : WebViewGlue
    {
        public object Window;

        public YorotGlue(object window, IChromiumWebViewPrivate view) : base(view)
        {
            Window = window;
        }

        protected override bool CanDownload(CefBrowser browser, string url, string requestMethod)
        {
            return base.CanDownload(browser, url, requestMethod);
        }

        protected override bool CanSaveCookie(CefBrowser browser, CefFrame frame, CefRequest request, CefResponse response, CefCookie cookie)
        {
            bool _cookie = false;
            if (Window is PopupWindow window && window.IsPageSafe != null && window.IsPageUsedCookie != null && window.IsPageUnsafe != null)
            {
                window.IsPageSafe.OnNext(false);
                window.IsPageUsedCookie.OnNext(true);
                window.IsPageUnsafe.OnNext(false);

                _cookie = window.CurrentSite.Permissions.allowCookies.Allowance == YorotPermissionMode.Allow || window.CurrentSite.Permissions.allowPopup.Allowance == YorotPermissionMode.AllowOneTime;
                if (window.CurrentSite.Permissions.allowCookies.Allowance == YorotPermissionMode.AllowOneTime)
                {
                    window.CurrentSite.Permissions.allowCookies.Allowance = YorotPermissionMode.Deny;
                }
            }
            if (Window is TabWindow tab)
            {
                tab.IsPageSafe = false;
                tab.IsPageUsedCookie = true;
                tab.IsPageUnsafe = false;

                _cookie = tab.CurrentSite.Permissions.allowPopup.Allowance == YorotPermissionMode.Allow || tab.CurrentSite.Permissions.allowPopup.Allowance == YorotPermissionMode.AllowOneTime;
                if (tab.CurrentSite.Permissions.allowPopup.Allowance == YorotPermissionMode.AllowOneTime)
                {
                    tab.CurrentSite.Permissions.allowPopup.Allowance = YorotPermissionMode.Deny;
                }
            }

            return _cookie;
        }

        protected override bool CanSendCookie(CefBrowser browser, CefFrame frame, CefRequest request, CefCookie cookie)
        {
            bool _cookie = false;
            if (Window is PopupWindow window && window.IsPageSafe != null && window.IsPageUsedCookie != null && window.IsPageUnsafe != null)
            {
                window.IsPageSafe.OnNext(true);
                window.IsPageUsedCookie.OnNext(false);
                window.IsPageUnsafe.OnNext(false);
                _cookie = window.CurrentSite.Permissions.allowCookies.Allowance == YorotPermissionMode.Allow || window.CurrentSite.Permissions.allowPopup.Allowance == YorotPermissionMode.AllowOneTime;
                if (window.CurrentSite.Permissions.allowCookies.Allowance == YorotPermissionMode.AllowOneTime)
                {
                    window.CurrentSite.Permissions.allowCookies.Allowance = YorotPermissionMode.Deny;
                }
            }
            if (Window is TabWindow tab)
            {
                tab.IsPageSafe = true;
                tab.IsPageUsedCookie = false;
                tab.IsPageUnsafe = false;
                _cookie = tab.CurrentSite.Permissions.allowPopup.Allowance == YorotPermissionMode.Allow || tab.CurrentSite.Permissions.allowPopup.Allowance == YorotPermissionMode.AllowOneTime;
                if (tab.CurrentSite.Permissions.allowPopup.Allowance == YorotPermissionMode.AllowOneTime)
                {
                    tab.CurrentSite.Permissions.allowPopup.Allowance = YorotPermissionMode.Deny;
                }
            }
            return _cookie;
        }

        protected override bool DoClose(CefBrowser browser)
        {
            return base.DoClose(browser);
        }

        protected override bool GetAudioParameters(CefBrowser browser, ref CefAudioParameters @params)
        {
            return base.GetAudioParameters(browser, ref @params);
        }

        protected override bool GetAuthCredentials(CefBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, CefAuthCallback callback)
        {
            return base.GetAuthCredentials(browser, originUrl, isProxy, host, port, realm, scheme, callback);
        }

        protected override CefCookieAccessFilter GetCookieAccessFilter(CefBrowser browser, CefFrame frame, CefRequest request)
        {
            return base.GetCookieAccessFilter(browser, frame, request);
        }

        protected override CefSize GetPdfPaperSize(CefBrowser browser, int deviceUnitsPerInch)
        {
            return base.GetPdfPaperSize(browser, deviceUnitsPerInch);
        }

        protected override CefResourceHandler GetResourceHandler(CefBrowser browser, CefFrame frame, CefRequest request)
        {
            var site = YorotGlobal.Main.CurrentSettings.SiteMan.GetSite(request.Url);
            if (Window is TabWindow tab)
            {
                tab.CurrentSite = site;
            }
            else if (Window is PopupWindow popup)
            {
                popup.CurrentSite = site;
            }
            if (request.Url.ToLowerInvariant().StartsWith("yorot://") && YorotGlobal.Main != null)
            {
                var url = request.Url;
                var arglist = new List<YorotBrowserWebSource.Argument>();
                int containsArg = request.Url.IndexOf('?');
                if (containsArg != -1)
                {
                    string args = request.Url.Substring(containsArg + 1);
                    url = url.Substring(0, url.Length - args.Length - 1);
                    int i = 0;
                    int count = 0; //TODO: Fix this
                    while (i < args.Length && count < 2)
                    {
                        int nextArg = args.IndexOf('&', i);
                        string arg = args.Substring(i, nextArg > 0 ? nextArg : args.Length - i);
                        string name = arg.Substring(0, arg.IndexOf('='));
                        string value = arg.Substring(name.Length + 1);
                        arglist.Add(new YorotBrowserWebSource.Argument(name, value));
                        i += arg.Length + 1;
                        count++;
                    }
                }
                var resource = YorotGlobal.Main.GetWebSource(url, arglist);
                if (resource != null)
                {
                    return new StringSource(resource[0], resource[1]);
                }
                else
                {
                    return new StringSource("Cannot find resource \"" + request.Url + "\".");
                }
            }
            return base.GetResourceHandler(browser, frame, request);
        }

        protected override CefResourceRequestHandler GetResourceRequestHandler(CefBrowser browser, CefFrame frame, CefRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref int disableDefaultHandling)
        {
            return base.GetResourceRequestHandler(browser, frame, request, isNavigation, isDownload, requestInitiator, ref disableDefaultHandling);
        }

        protected override CefResponseFilter GetResourceResponseFilter(CefBrowser browser, CefFrame frame, CefRequest request, CefResponse response)
        {
            return base.GetResourceResponseFilter(browser, frame, request, response);
        }

        protected override bool GetRootScreenRect(CefBrowser browser, ref CefRect rect)
        {
            return base.GetRootScreenRect(browser, ref rect);
        }

        protected override bool GetScreenInfo(CefBrowser browser, ref CefScreenInfo screenInfo)
        {
            return base.GetScreenInfo(browser, ref screenInfo);
        }

        protected override bool GetScreenPoint(CefBrowser browser, int viewX, int viewY, ref int screenX, ref int screenY)
        {
            return base.GetScreenPoint(browser, viewX, viewY, ref screenX, ref screenY);
        }

        protected override void GetViewRect(CefBrowser browser, ref CefRect rect)
        {
            base.GetViewRect(browser, ref rect);
        }

        protected override void OnAcceleratedPaint(CefBrowser browser, CefPaintElementType type, CefRect[] dirtyRects, IntPtr sharedHandle)
        {
            base.OnAcceleratedPaint(browser, type, dirtyRects, sharedHandle);
        }

        protected override void OnAddressChange(CefBrowser browser, CefFrame frame, string url)
        {
            base.OnAddressChange(browser, frame, url);
        }

        protected override void OnAfterCreated(CefBrowser browser)
        {
            base.OnAfterCreated(browser);
        }

        protected override void OnAudioStreamError(CefBrowser browser, string message)
        {
            base.OnAudioStreamError(browser, message);
        }

        protected override void OnAudioStreamPacket(CefBrowser browser, IntPtr data, int frames, long pts)
        {
            base.OnAudioStreamPacket(browser, data, frames, pts);
        }

        protected override void OnAudioStreamStarted(CefBrowser browser, CefAudioParameters @params, int channels)
        {
            base.OnAudioStreamStarted(browser, @params, channels);
        }

        protected override void OnAudioStreamStopped(CefBrowser browser)
        {
            base.OnAudioStreamStopped(browser);
        }

        protected override bool OnAutoResize(CefBrowser browser, CefSize newSize)
        {
            return base.OnAutoResize(browser, newSize);
        }

        protected override bool OnBeforeBrowse(CefBrowser browser, CefFrame frame, CefRequest request, bool userGesture, bool isRedirect)
        {
            return base.OnBeforeBrowse(browser, frame, request, userGesture, isRedirect);
        }

        protected override void OnBeforeClose(CefBrowser browser)
        {
            base.OnBeforeClose(browser);
        }

        protected override void OnBeforeDownload(CefBrowser browser, CefDownloadItem downloadItem, string suggestedName, CefBeforeDownloadCallback callback)
        {
            base.OnBeforeDownload(browser, downloadItem, suggestedName, callback);
        }

        protected override bool OnBeforePopup(CefBrowser browser, CefFrame frame, string targetUrl, string targetFrameName, CefWindowOpenDisposition targetDisposition, bool userGesture, CefPopupFeatures popupFeatures, CefWindowInfo windowInfo, ref CefClient client, CefBrowserSettings settings, ref CefDictionaryValue extraInfo, ref int noJavascriptAccess)
        {
            if (YorotGlobal.Main is null) { return true; }
            switch (targetDisposition)
            {
                case CefWindowOpenDisposition.SwitchToTab:
                case CefWindowOpenDisposition.Unknown:
                case CefWindowOpenDisposition.CurrentTab:
                case CefWindowOpenDisposition.SingletonTab:
                case CefWindowOpenDisposition.NewWindow:
                case CefWindowOpenDisposition.NewForegroundTab:
                    YorotGlobal.Main.MainForm.NewTab(targetUrl, -1, true);
                    break;

                case CefWindowOpenDisposition.NewBackgroundTab:
                    YorotGlobal.Main.MainForm.NewTab(targetUrl, -1, false);
                    break;

                case CefWindowOpenDisposition.NewPopup:

                    bool _popup = false;
                    if (Window is TabWindow tab)
                    {
                        _popup = tab.CurrentSite.Permissions.allowPopup.Allowance == YorotPermissionMode.Allow || tab.CurrentSite.Permissions.allowPopup.Allowance == YorotPermissionMode.AllowOneTime;
                        if (tab.CurrentSite.Permissions.allowPopup.Allowance == YorotPermissionMode.AllowOneTime)
                        {
                            tab.CurrentSite.Permissions.allowPopup.Allowance = YorotPermissionMode.Deny;
                        }
                    }
                    else if (Window is PopupWindow popup)
                    {
                        _popup = popup.CurrentSite.Permissions.allowPopup.Allowance == YorotPermissionMode.Allow || popup.CurrentSite.Permissions.allowPopup.Allowance == YorotPermissionMode.AllowOneTime;
                        if (popup.CurrentSite.Permissions.allowPopup.Allowance == YorotPermissionMode.AllowOneTime)
                        {
                            popup.CurrentSite.Permissions.allowPopup.Allowance = YorotPermissionMode.Deny;
                        }
                    }
                    if (!_popup) { return true; }
                    Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        PopupWindow window = new(targetUrl);
                        if (Window is TabWindow tab) { window.mainWindow = tab.mainWindow; }
                        window.Activate();
                        window.Show();
                        window.BringIntoView();
                    }, Avalonia.Threading.DispatcherPriority.Normal);
                    return true;

                case CefWindowOpenDisposition.SaveToDisk:
                    // TODO: Implement a save function here
                    return true;

                case CefWindowOpenDisposition.OffTheRecord:
                    // Ignored.
                    return true;

                case CefWindowOpenDisposition.IgnoreAction:
                case CefWindowOpenDisposition.NewPictureInPicture: // TODO: Use this for Picture-In-Picture mode
                    return base.OnBeforePopup(browser,
                                              frame,
                                              targetUrl,
                                              targetFrameName,
                                              targetDisposition,
                                              userGesture,
                                              popupFeatures,
                                              windowInfo,
                                              ref client,
                                              settings,
                                              ref extraInfo,
                                              ref noJavascriptAccess);
            }
            return true;
        }

        protected override CefReturnValue OnBeforeResourceLoad(CefBrowser browser, CefFrame frame, CefRequest request, CefCallback callback)
        {
            return base.OnBeforeResourceLoad(browser, frame, request, callback);
        }

        protected override bool OnBeforeUnloadDialog(CefBrowser browser, string messageText, bool isReload, CefJSDialogCallback callback)
        {
            return base.OnBeforeUnloadDialog(browser, messageText, isReload, callback);
        }

        protected override bool OnCertificateError(CefBrowser browser, CefErrorCode certError, string requestUrl, CefSSLInfo sslInfo, CefCallback callback)
        {
            if (Window is Views.TabWindow tabWindow)
            {
                tabWindow.redirectTo("yorot://certerror", "");
                if (tabWindow.IsPageSafe != null && tabWindow.IsPageUnsafe != null && tabWindow.IsPageUsedCookie != null)
                {
                    tabWindow.IsPageUnsafe = true;
                    tabWindow.IsPageSafe = false;
                    tabWindow.IsPageUsedCookie = false;
                }
            }
            if (Window is Views.PopupWindow popupWindow)
            {
                popupWindow.redirectTo("yorot://certerror", "");
                if (popupWindow.IsPageSafe != null && popupWindow.IsPageUnsafe != null && popupWindow.IsPageUsedCookie != null)
                {
                    popupWindow.IsPageUnsafe.OnNext(true);
                    popupWindow.IsPageSafe.OnNext(false);
                    popupWindow.IsPageUsedCookie.OnNext(false);
                }
            }
            return true;
        }

        protected override bool OnChromeCommand(CefBrowser browser, int commandId, CefWindowOpenDisposition disposition)
        {
            return base.OnChromeCommand(browser, commandId, disposition);
        }

        protected override bool OnConsoleMessage(CefBrowser browser, CefLogSeverity level, string message, string source, int line)
        {
            return base.OnConsoleMessage(browser, level, message, source, line);
        }

        protected override bool OnCursorChange(CefBrowser browser, IntPtr cursor, CefCursorType type, CefCursorInfo customCursorInfo)
        {
            return base.OnCursorChange(browser, cursor, type, customCursorInfo);
        }

        protected override void OnDialogClosed(CefBrowser browser)
        {
            base.OnDialogClosed(browser);
        }

        protected override void OnDocumentAvailableInMainFrame(CefBrowser browser)
        {
            base.OnDocumentAvailableInMainFrame(browser);
        }

        protected override void OnDownloadUpdated(CefBrowser browser, CefDownloadItem downloadItem, CefDownloadItemCallback callback)
        {
            base.OnDownloadUpdated(browser, downloadItem, callback);
        }

        protected override bool OnDragEnter(CefBrowser browser, CefDragData dragData, CefDragOperationsMask mask)
        {
            return base.OnDragEnter(browser, dragData, mask);
        }

        protected override void OnDraggableRegionsChanged(CefBrowser browser, CefFrame frame, CefDraggableRegion[] regions)
        {
            base.OnDraggableRegionsChanged(browser, frame, regions);
        }

        protected override void OnFaviconUrlChange(CefBrowser browser, CefStringList iconUrls)
        {
            if (Window is TabWindow tab && tab.Parent is TabViewItem item)
            {
                // TODO: Get Icon from URL and set here
                //item.IconSource =
            }
            else if (Window is PopupWindow popup)
            {
                // TODO: Get Icon from URL and set here
                //popup.Icon =
            }
        }

        protected override bool OnFileDialog(CefBrowser browser, CefFileDialogMode mode, string title, string defaultFilePath, CefStringList acceptFilters, CefFileDialogCallback callback)
        {
            return base.OnFileDialog(browser, mode, title, defaultFilePath, acceptFilters, callback);
        }

        protected override void OnFindResult(CefBrowser browser, int identifier, int count, CefRect selectionRect, int activeMatchOrdinal, bool finalUpdate)
        {
            if (Window is Views.TabWindow tabWindow && tabWindow.FindCount != null)
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    tabWindow.FindCount = activeMatchOrdinal + "/" + count;
                }, Avalonia.Threading.DispatcherPriority.Layout);
                return;
            }
            base.OnFindResult(browser, identifier, count, selectionRect, activeMatchOrdinal, finalUpdate);
        }

        protected override void OnFrameAttached(CefBrowser browser, CefFrame frame, bool reattached)
        {
            base.OnFrameAttached(browser, frame, reattached);
        }

        protected override void OnFrameCreated(CefBrowser browser, CefFrame frame)
        {
            base.OnFrameCreated(browser, frame);
        }

        protected override void OnFrameDetached(CefBrowser browser, CefFrame frame)
        {
            base.OnFrameDetached(browser, frame);
        }

        protected override void OnFullscreenModeChange(CefBrowser browser, bool fullscreen)
        {
            base.OnFullscreenModeChange(browser, fullscreen);
        }

        protected override void OnGotFocus(CefBrowser browser)
        {
            base.OnGotFocus(browser);
        }

        protected override void OnImeCompositionRangeChanged(CefBrowser browser, CefRange selectedRange, CefRect[] characterBounds)
        {
            base.OnImeCompositionRangeChanged(browser, selectedRange, characterBounds);
        }

        protected override bool OnJSDialog(CefBrowser browser, string originUrl, CefJSDialogType dialogType, string messageText, string defaultPromptText, CefJSDialogCallback callback, ref int suppressMessage)
        {
            if (YorotGlobal.Main is null)
            {
                return true;
            }
            YorotSite site = YorotGlobal.Main.CurrentSettings.SiteMan.GetSite(originUrl);
            if (!site.ShowMessageBoxes)
            {
                return true;
            }
            switch (dialogType)
            {
                case CefJSDialogType.Confirm:
                case CefJSDialogType.Alert:
                    var task = Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        var alertmessage = new MessageBox(
                            YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.MessageFromSite").Replace("[Parameter.Url]", originUrl),
                            messageText,
                            new MessageBoxButton[] { new MessageBoxButton.Ok(), new MessageBoxButton.Cancel()
                                // TODO: Add Image here
                            });
                        alertmessage.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>((sender, e) =>
                        {
                            site.ShowMessageBoxes = !alertmessage.DontShowThis;
                            if (alertmessage.DialogResult is MessageBoxButton.Ok)
                            {
                                callback.Continue(true, "");
                            }
                            else
                            {
                                callback.Continue(false, "");
                            }
                        });
                        alertmessage.ShowDialog(YorotGlobal.Main.MainForm);
                    }, Avalonia.Threading.DispatcherPriority.Input);

                    return true;

                default:
                case CefJSDialogType.Prompt:
                    var task2 = Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        var promptmessage = new MessageBox(
                            YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.MessageFromSite").Replace("[Parameter.Url]", originUrl),
                            messageText, defaultPromptText,
                            new MessageBoxButton[] { new MessageBoxButton.Ok(), new MessageBoxButton.Cancel()
                                // TODO: Add Image here
                            });
                        promptmessage.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>((sender, e) =>
                        {
                            site.ShowMessageBoxes = !promptmessage.DontShowThis;
                            if (promptmessage.DialogResult is MessageBoxButton.Ok)
                            {
                                callback.Continue(true, promptmessage.Prompt);
                            }
                            else
                            {
                                callback.Continue(false, "");
                            }
                        });
                        promptmessage.ShowDialog(YorotGlobal.Main.MainForm).Wait();
                    }, Avalonia.Threading.DispatcherPriority.Input);
                    return true;
            }
        }

        protected override bool OnKeyEvent(CefBrowser browser, CefKeyEvent @event, CefEventHandle osEvent)
        {
            return base.OnKeyEvent(browser, @event, osEvent);
        }

        protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
        {
            base.OnLoadEnd(browser, frame, httpStatusCode);
        }

        protected override void OnLoadError(CefBrowser browser, CefFrame frame, CefErrorCode errorCode, string errorText, string failedUrl)
        {
            base.OnLoadError(browser, frame, errorCode, errorText, failedUrl);
        }

        protected override void OnLoadingProgressChange(CefBrowser browser, double progress)
        {
            base.OnLoadingProgressChange(browser, progress);
        }

        protected override void OnLoadingStateChange(CefBrowser browser, bool isLoading, bool canGoBack, bool canGoForward)
        {
            base.OnLoadingStateChange(browser, isLoading, canGoBack, canGoForward);
        }

        protected override void OnLoadStart(CefBrowser browser, CefFrame frame, CefTransitionType transitionType)
        {
            base.OnLoadStart(browser, frame, transitionType);
        }

        protected override void OnMainFrameChanged(CefBrowser browser, CefFrame oldFrame, CefFrame newFrame)
        {
            base.OnMainFrameChanged(browser, oldFrame, newFrame);
        }

        protected override bool OnOpenUrlFromTab(CefBrowser browser, CefFrame frame, string targetUrl, CefWindowOpenDisposition targetDisposition, bool userGesture)
        {
            return base.OnOpenUrlFromTab(browser, frame, targetUrl, targetDisposition, userGesture);
        }

        protected override void OnPaint(CefBrowser browser, CefPaintElementType type, CefRect[] dirtyRects, IntPtr buffer, int width, int height)
        {
            base.OnPaint(browser, type, dirtyRects, buffer, width, height);
        }

        protected override void OnPdfPrintFinished(string path, bool success)
        {
            base.OnPdfPrintFinished(path, success);
        }

        protected override void OnPluginCrashed(CefBrowser browser, string pluginPath)
        {
            base.OnPluginCrashed(browser, pluginPath);
        }

        protected override void OnPopupShow(CefBrowser browser, bool show)
        {
            base.OnPopupShow(browser, show);
        }

        protected override void OnPopupSize(CefBrowser browser, CefRect rect)
        {
            base.OnPopupSize(browser, rect);
        }

        protected override bool OnPreKeyEvent(CefBrowser browser, CefKeyEvent @event, CefEventHandle osEvent, ref int isKeyboardShortcut)
        {
            return base.OnPreKeyEvent(browser, @event, osEvent, ref isKeyboardShortcut);
        }

        protected override bool OnPrintDialog(CefBrowser browser, bool hasSelection, CefPrintDialogCallback callback)
        {
            return base.OnPrintDialog(browser, hasSelection, callback);
        }

        protected override bool OnPrintJob(CefBrowser browser, string documentName, string pdfFilePath, CefPrintJobCallback callback)
        {
            return base.OnPrintJob(browser, documentName, pdfFilePath, callback);
        }

        protected override void OnPrintReset(CefBrowser browser)
        {
            base.OnPrintReset(browser);
        }

        protected override void OnPrintSettings(CefBrowser browser, CefPrintSettings settings, bool getDefaults)
        {
            base.OnPrintSettings(browser, settings, getDefaults);
        }

        protected override void OnPrintStart(CefBrowser browser)
        {
            base.OnPrintStart(browser);
        }

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefFrame frame, CefProcessId sourceProcess, CefProcessMessage message)
        {
            return base.OnProcessMessageReceived(browser, frame, sourceProcess, message);
        }

        protected override void OnProtocolExecution(CefBrowser browser, CefFrame frame, CefRequest request, ref int allowOsExecution)
        {
            base.OnProtocolExecution(browser, frame, request, ref allowOsExecution);
        }

        protected override bool OnQuotaRequest(CefBrowser browser, string originUrl, long newSize, CefCallback callback)
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

        protected override void OnResetDialogState(CefBrowser browser)
        {
            base.OnResetDialogState(browser);
        }

        protected override void OnResourceLoadComplete(CefBrowser browser, CefFrame frame, CefRequest request, CefResponse response, CefUrlRequestStatus status, long receivedContentLength)
        {
            base.OnResourceLoadComplete(browser, frame, request, response, status, receivedContentLength);
        }

        protected override void OnResourceRedirect(CefBrowser browser, CefFrame frame, CefRequest request, CefResponse response, ref string newUrl)
        {
            base.OnResourceRedirect(browser, frame, request, response, ref newUrl);
        }

        protected override bool OnResourceResponse(CefBrowser browser, CefFrame frame, CefRequest request, CefResponse response)
        {
            return base.OnResourceResponse(browser, frame, request, response);
        }

        protected override void OnScrollOffsetChanged(CefBrowser browser, double x, double y)
        {
            base.OnScrollOffsetChanged(browser, x, y);
        }

        protected override bool OnSelectClientCertificate(CefBrowser browser, bool isProxy, string host, int port, CefX509Certificate[] certificates, CefSelectClientCertificateCallback callback)
        {
            return base.OnSelectClientCertificate(browser, isProxy, host, port, certificates, callback);
        }

        protected override bool OnSetFocus(CefBrowser browser, CefFocusSource source)
        {
            return base.OnSetFocus(browser, source);
        }

        protected override void OnStatusMessage(CefBrowser browser, string message)
        {
            base.OnStatusMessage(browser, message);
        }

        protected override void OnTakeFocus(CefBrowser browser, bool next)
        {
            base.OnTakeFocus(browser, next);
        }

        protected override void OnTextSelectionChanged(CefBrowser browser, string selectedText, CefRange selectedRange)
        {
            base.OnTextSelectionChanged(browser, selectedText, selectedRange);
        }

        protected override void OnTitleChange(CefBrowser browser, string title)
        {
            base.OnTitleChange(browser, title);
        }

        protected override bool OnTooltip(CefBrowser browser, ref string text)
        {
            return base.OnTooltip(browser, ref text);
        }

        protected override void OnVirtualKeyboardRequested(CefBrowser browser, CefTextInputMode inputMode)
        {
            base.OnVirtualKeyboardRequested(browser, inputMode);
        }

        #region Context Menu

        internal class ContextMenuParams : IEquatable<ContextMenuParams?>
        {
            public ContextMenuParams(CefContextMenuParams menuParams)
            {
                SelectedText = menuParams.SelectionText;
                LinkUrl = menuParams.LinkUrl;
                UnfilteredLinkUrl = menuParams.UnfilteredLinkUrl;
                SourceUrl = menuParams.SourceUrl;
                FrameUrl = menuParams.FrameUrl;
                IsEditable = menuParams.IsEditable;
                IsImage = menuParams.HasImageContents;
                ImageTitle = menuParams.TitleText;
            }

            public string ImageTitle { get; }
            public string SelectedText { get; }
            public bool IsTextSelected => !string.IsNullOrWhiteSpace(SelectedText);
            public string LinkUrl { get; }
            public bool IsSelectedUrl => !string.IsNullOrWhiteSpace(LinkUrl) && !string.IsNullOrWhiteSpace(UnfilteredLinkUrl);
            public string UnfilteredLinkUrl { get; }
            public string SourceUrl { get; }
            public string PageUrl { get; }
            public string FrameUrl { get; }
            public bool IsEditable { get; }
            public bool IsImage { get; }

            public override bool Equals(object? obj)
            {
                return Equals(obj as ContextMenuParams);
            }

            public bool Equals(ContextMenuParams? other)
            {
                return other is not null &&
                       ImageTitle == other.ImageTitle &&
                       SelectedText == other.SelectedText &&
                       LinkUrl == other.LinkUrl &&
                       UnfilteredLinkUrl == other.UnfilteredLinkUrl &&
                       SourceUrl == other.SourceUrl &&
                       PageUrl == other.PageUrl &&
                       FrameUrl == other.FrameUrl &&
                       IsEditable == other.IsEditable;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(ImageTitle, SelectedText, LinkUrl, UnfilteredLinkUrl, SourceUrl, PageUrl, FrameUrl, IsEditable);
            }

            public static bool operator ==(ContextMenuParams? left, ContextMenuParams? right)
            {
                return EqualityComparer<ContextMenuParams>.Default.Equals(left, right);
            }

            public static bool operator !=(ContextMenuParams? left, ContextMenuParams? right)
            {
                return !(left == right);
            }
        }

        protected override bool RunContextMenu(CefBrowser browser, CefFrame frame, CefContextMenuParams menuParams, CefMenuModel model, CefRunContextMenuCallback callback)
        {
            ContextMenuParams param = new(menuParams);
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (Window is TabWindow tab)
                {
                    tab.ShowContextMenu(param, frame);
                }
                if (Window is PopupWindow popup)
                {
                    popup.ShowContextMenu(param, frame);
                }
            }, Avalonia.Threading.DispatcherPriority.Input);
            return true;
        }

        protected override void OnBeforeContextMenu(CefBrowser browser, CefFrame frame, CefContextMenuParams menuParams, CefMenuModel model)
        {
            // Ignored
        }

        protected override bool OnContextMenuCommand(CefBrowser browser, CefFrame frame, CefContextMenuParams menuParams, int commandId, CefEventFlags eventFlags)
        {
            return base.OnContextMenuCommand(browser, frame, menuParams, commandId, eventFlags);
        }

        protected override void OnContextMenuDismissed(CefBrowser browser, CefFrame frame)
        {
            // Ignored
        }

        #endregion Context Menu

        protected override bool StartDragging(CefBrowser browser, CefDragData dragData, CefDragOperationsMask allowedOps, int x, int y)
        {
            return base.StartDragging(browser, dragData, allowedOps, x, y);
        }

        protected override void UpdateDragCursor(CefBrowser browser, CefDragOperationsMask operation)
        {
            base.UpdateDragCursor(browser, operation);
        }
    }
}