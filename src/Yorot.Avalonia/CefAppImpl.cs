using Avalonia.Controls;
using CefNet;
using HTAlt;
using System;
using System.Runtime.InteropServices;

namespace Yorot_Avalonia
{
    internal class CefAppImpl : CefNetApplication
    {
        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            base.OnBeforeCommandLineProcessing(processType, commandLine);

            //Output.WriteLine("Chrome Command Line String:" + commandLine.CommandLineString, LogLevel.Info);

            //commandLine.AppendSwitchWithValue("proxy-server", "127.0.0.1:8888");

            //commandLine.AppendSwitch("ignore-certificate-errors");
            //commandLine.AppendSwitchWithValue("remote-debugging-port", "9222");

            //enable-devtools-experiments
            //commandLine.AppendSwitch("enable-devtools-experiments");

            //e.CommandLine.AppendSwitchWithValue("user-agent", "Mozilla/5.0 (Windows 10.0) WebKa/" + DateTime.UtcNow.Ticks);

            //("force-device-scale-factor", "1");

            //commandLine.AppendSwitch("disable-gpu");
            //commandLine.AppendSwitch("disable-gpu-compositing");
            //commandLine.AppendSwitch("disable-gpu-vsync");

            commandLine.AppendSwitch("enable-begin-frame-scheduling");
            commandLine.AppendSwitch("enable-media-stream");

            //commandLine.AppendSwitchWithValue("enable-blink-features", "CSSPseudoHas");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                commandLine.AppendSwitch("no-zygote");
                commandLine.AppendSwitch("no-sandbox");
            }
        }

        protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            base.OnContextCreated(browser, frame, context);
            // TODO: use this to make V8 functions and polyfill/add stuff
            CefV8Value window = context.GetGlobal();
            var fnhandler = new V8Func();
            window.SetValue("testfunc", CefV8Value.CreateFunction("testfunc", fnhandler), CefV8PropertyAttribute.ReadOnly);
        }

        public Action<long> ScheduleMessagePumpWorkCallback { get; set; }

        protected override void OnScheduleMessagePumpWork(long delayMs)
        {
            ScheduleMessagePumpWorkCallback(delayMs);
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