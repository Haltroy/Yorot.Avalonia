using Avalonia;
using Avalonia.ReactiveUI;
using CefNet;
using HTAlt;
using System;
using System.Linq;

namespace Yorot_Avalonia
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            bool _i = args.Contains("-i") || args.Contains("--incognito");
            bool exists = System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1;
            YorotGlobal.Main = new YorotSpecial(YorotGlobal.YorotAppPath, "Yorot", YorotGlobal.CodeName, YorotGlobal.Version, YorotGlobal.VersionNo, args.Contains("-i") || args.Contains("--incognito"));
            if (exists && !_i)
            {
                Output.WriteLine("<Yorot.Program> App already running. Passing arguments...", LogLevel.Info);
                YorotGlobal.Main.Wolfhook.SendWolf(string.Join(Environment.NewLine, args));
            }

            BuildAvaloniaApp().With(new AvaloniaNativePlatformOptions { UseGpu = !PlatformInfo.IsMacOS }).StartWithCefNetApplicationLifetime(args); ;
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}