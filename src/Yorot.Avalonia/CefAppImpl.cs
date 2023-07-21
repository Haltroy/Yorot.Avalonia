using CefNet;
using HTAlt;
using System;
using System.Runtime.InteropServices;

namespace Yorot_Avalonia
{
    public class CefAppImpl : CefNetApplication
    {
        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            base.OnBeforeCommandLineProcessing(processType, commandLine);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                commandLine.AppendSwitch("no-zygote");
                commandLine.AppendSwitch("no-sandbox");
            }
        }

        public Action<long> ScheduleMessagePumpWorkCallback { get; set; }

        protected override void OnScheduleMessagePumpWork(long delayMs)
        {
            ScheduleMessagePumpWorkCallback(delayMs);
        }

        protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            base.OnContextCreated(browser, frame, context);
            // TODO: use this to make V8 functions and polyfill/add stuff
            CefV8Value global = context.GetGlobal();
            var window = global.GetValue("window");

            var notification = CefV8Value.CreateObject();
            window.SetValue("Notification", notification, CefV8PropertyAttribute.ReadOnly);

            var fnhandler = new V8Func();
            notification.SetValue("requestPermission", CefV8Value.CreateFunction("requestPermission", fnhandler), CefV8PropertyAttribute.ReadOnly);
        }

        private class V8Func : CefV8Handler
        {
            protected override bool Execute(string name, CefV8Value @object, CefV8Value[] arguments, ref CefV8Value retval, ref string exception)
            {
                exception = null;
                if (arguments.Length > 0)
                {
                    Output.WriteLine("TEST: " + (arguments[0].IsString ? arguments[0].GetStringValue() : "Not_A_String"), LogLevel.Info);
                }
                retval = new CefV8Value(1);
                return true;
            }
        }
    }
}