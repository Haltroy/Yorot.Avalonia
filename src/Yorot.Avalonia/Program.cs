using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using CefNet;
using HTAlt;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yorot_Avalonia.Views;

namespace Yorot_Avalonia
{
    internal class Program
    {
        private static CefAppImpl app;
        private static DispatcherTimer messagePump;
        private const int messagePumpDelay = 10;

        [STAThread]
        public static void Main(string[] args)
        {
            //bool exists = System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1;
            //if (exists && !_i)
            //{
            //    Output.WriteLine("<Yorot.Program> App already running. Passing arguments...", LogLevel.Info);
            //    YorotGlobal.Main.Wolfhook.SendWolf(string.Join(Environment.NewLine, args));
            //    return;
            //}

            YorotGlobal.Main = new YorotSpecial(YorotGlobal.YorotAppPath, "Yorot", YorotGlobal.CodeName, YorotGlobal.Version, YorotGlobal.VersionNo, args.Contains("-i") || args.Contains("--incognito"));
            if (args.Contains("--oobe"))
            {
                YorotGlobal.Main.OOBE = true;
            }

            // CEF

            bool externalMessagePump = PlatformInfo.IsMacOS || args.Contains("--external-message-pump");

            var settings = new CefSettings();
            settings.LocalesDirPath = YorotGlobal.Main.EngineLocaleFolder;
            settings.ResourcesDirPath = YorotGlobal.Main.EngineFolder;
            settings.NoSandbox = true;
            settings.WindowlessRenderingEnabled = true;
            settings.LogSeverity = CefLogSeverity.Warning;

            App.FrameworkInitialized += App_FrameworkInitialized;
            App.FrameworkShutdown += App_FrameworkShutdown;

            settings.MultiThreadedMessageLoop = !externalMessagePump;
            settings.ExternalMessagePump = externalMessagePump;

            settings.Locale = YorotGlobal.Main.Locale.ToString();
            settings.UserDataPath = YorotGlobal.Main.Profiles.Current.CacheLoc;
            settings.UserAgent = YorotGlobal.Main.GetUserAgent("Chrome", YorotGlobal.ChromiumVersion);

            app = new CefAppImpl();
            app.CefProcessMessageReceived += App_CefProcessMessageReceived;
            app.ScheduleMessagePumpWorkCallback = OnScheduleMessagePumpWork;

            Output.WriteLine($"Starting Engine \"Chromium Embedded Framework - CefNet\" on \"{YorotGlobal.Main.EngineFolder}\".", HTAlt.LogLevel.Info);

            string _engineset = Environment.NewLine +
                                $"          UserAgent: \"{settings.UserAgent}\"" + Environment.NewLine +
                                $"          UserDataPath: \"{settings.UserDataPath}\"" + Environment.NewLine +
                                $"          Locale: \"{settings.Locale}\"" + Environment.NewLine +
                                $"          LogSeverity: \"{settings.LogSeverity.ToString()}\"" + Environment.NewLine +
                                $"          WindowlessRenderingEnabled: \"{(settings.WindowlessRenderingEnabled ? "true" : "false")}\"" + Environment.NewLine +
                                $"          MultiThreadedMessageLoop: \"{(settings.MultiThreadedMessageLoop ? "true" : "false")}\"" + Environment.NewLine +
                                $"          ExternalMessagePump: \"{(settings.ExternalMessagePump ? "true" : "false")}\"" + Environment.NewLine +
                                $"          MultiThreadedMessageLoop: \"{(settings.MultiThreadedMessageLoop ? "true" : "false")}\"" + Environment.NewLine +
                                $"          NoSandBox: \"{(settings.NoSandbox ? "true" : "false")}\"" + Environment.NewLine +
                                $"          LocalePath: \"{settings.LocalesDirPath}\"" + Environment.NewLine +
                                $"          ResourcesDirPath: \"{settings.ResourcesDirPath}\"" + Environment.NewLine +
                                $"          Arguments: \"{string.Join(' ', args)}\"" + Environment.NewLine;

            Output.WriteLine($"Engine Settings: \"{_engineset}\".", HTAlt.LogLevel.Info);

            app.Initialize(YorotGlobal.Main.EngineFolder, settings);

            CefApi.RegisterSchemeHandlerFactory("yorot", "", new Handlers.YorotSchemeHandlerFactory());

            BuildAvaloniaApp()
            //    .With(new AvaloniaNativePlatformOptions { UseGpu = !PlatformInfo.IsMacOS })
                .StartWithCefNetApplicationLifetime(args, Avalonia.Controls.ShutdownMode.OnExplicitShutdown);

            Output.WriteLine("Exiting...", HTAlt.LogLevel.Info);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();

        private static void App_FrameworkInitialized(object sender, EventArgs e)
        {
            if (CefNetApplication.Instance.UsesExternalMessageLoop)
            {
                messagePump = new DispatcherTimer(TimeSpan.FromMilliseconds(messagePumpDelay), DispatcherPriority.Normal, (s, e) =>
                {
                    CefApi.DoMessageLoopWork();
                    Dispatcher.UIThread.RunJobs();
                });
                messagePump.Start();
            }
        }

        private static void App_FrameworkShutdown(object sender, EventArgs e)
        {
            messagePump?.Stop();
        }

        private static async void OnScheduleMessagePumpWork(long delayMs)
        {
            await Task.Delay((int)delayMs);
            Dispatcher.UIThread.Post(CefApi.DoMessageLoopWork);
        }

        private static string GetProjectPath()
        {
            string projectPath = Path.GetDirectoryName(typeof(App).Assembly.Location);
            string projectName = PlatformInfo.IsMacOS ? "AvaloniaApp.app" : "AvaloniaApp";
            string rootPath = Path.GetPathRoot(projectPath);
            while (Path.GetFileName(projectPath) != projectName)
            {
                if (projectPath == rootPath)
                    throw new DirectoryNotFoundException("Could not find the project directory.");
                projectPath = Path.GetDirectoryName(projectPath);
            }
            return projectPath;
        }

        private static void App_CefProcessMessageReceived(object sender, CefProcessMessageReceivedEventArgs e)
        {
            if (e.Name == "TestV8ValueTypes")
            {
                TestV8ValueTypes(e.Frame);
                e.Handled = true;
                return;
            }

            if (e.Name == "MessageBox.Show")
            {
                string message = e.Message.ArgumentList.GetString(0);
                Dispatcher.UIThread.Post(() =>
                {
                    var msgbox = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("Title", message, MessageBox.Avalonia.Enums.ButtonEnum.Ok);
                    msgbox.Show();
                });
                e.Handled = true;
                return;
            }
        }

        private static void TestV8ValueTypes(CefFrame frame)
        {
            var sb = new StringBuilder();
            CefV8Context context = frame.V8Context;
            if (!context.Enter())
                return;
            try
            {
                sb.Append("typeof 1 = ").Append(context.Eval("1", null).Type).AppendLine();
                sb.Append("typeof true = ").Append(context.Eval("true", null).Type).AppendLine();
                sb.Append("typeof 'string' = ").Append(context.Eval("'string'", null).Type).AppendLine();
                sb.Append("typeof 2.2 = ").Append(context.Eval("2.2", null).Type).AppendLine();
                sb.Append("typeof null = ").Append(context.Eval("null", null).Type).AppendLine();
                sb.Append("typeof new Object() = ").Append(context.Eval("new Object()", null).Type).AppendLine();
                sb.Append("typeof undefined = ").Append(context.Eval("undefined", null).Type).AppendLine();
                sb.Append("typeof new Date() = ").Append(context.Eval("new Date()", null).Type).AppendLine();
                sb.Append("(window == window) = ").Append(context.Eval("window", null) == context.Eval("window", null)).AppendLine();
            }
            finally
            {
                context.Exit();
            }
            var message = new CefProcessMessage("MessageBox.Show");
            message.ArgumentList.SetString(0, sb.ToString());
            frame.SendProcessMessage(CefProcessId.Browser, message);
        }
    }
}