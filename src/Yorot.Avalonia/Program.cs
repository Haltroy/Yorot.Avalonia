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
        internal static CefAppImpl? app;
        private static DispatcherTimer? messagePump;
        private const int messagePumpDelay = 10;

        [STAThread]
        public static void Main(string[] args)
        {
            bool _i = args.Contains("--incognito");

            bool isCefSubprocess = false;

            // Don't get scared by this for loop, mostly CEF already puts
            // --type=something argument first.
            for (int i = 0; i < args.Length; i++)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(args[i], "\\-\\-type\\="))
                {
                    isCefSubprocess = true;
                    break;
                }
            }

            if (!isCefSubprocess)
            {
                YorotGlobal.Main = new YorotSpecial(YorotGlobal.YorotAppPath, "Yorot", YorotGlobal.CodeName, YorotGlobal.Version, YorotGlobal.VersionNo, args.Contains("-i") || _i);
                Yorot.StaticVariables.Locale = YorotGlobal.Main.Locale.ToString();
                Yorot.StaticVariables.UserAgent = YorotGlobal.Main.GetUserAgent("Chrome", YorotGlobal.ChromiumVersion);
                Yorot.StaticVariables.LastUser = YorotGlobal.Main.Profiles.Current.Name;

                if (args.Contains("--oobe"))
                {
                    YorotGlobal.Main.OOBE = true;
                }
#if DEBUG
                Console.WriteLine($"Main process started with arguments: {string.Join(' ', args)}");
#endif
            }
            else
            {
#if DEBUG
                Console.WriteLine($"Subprocess started with arguments: {string.Join(' ', args)}");
#endif
            }

            bool externalMessagePump = PlatformInfo.IsMacOS || args.Contains("--external-message-pump");

            string cefPath = string.Empty;

            var settings = new CefSettings();

            settings.LocalesDirPath = EngineLocalePath;
            settings.ResourcesDirPath = EnginePath;
            settings.Locale = Yorot.StaticVariables.Locale;
            settings.UserAgent = Yorot.StaticVariables.UserAgent;
            settings.UserDataPath = UserDataPath;

            settings.NoSandbox = true;
            settings.WindowlessRenderingEnabled = true;
            settings.LogSeverity = CefLogSeverity.Warning;

            App.FrameworkInitialized += App_FrameworkInitialized;
            App.FrameworkShutdown += App_FrameworkShutdown;

            settings.MultiThreadedMessageLoop = !externalMessagePump;
            settings.ExternalMessagePump = externalMessagePump;

            app = new CefAppImpl();
            app.ScheduleMessagePumpWorkCallback = OnScheduleMessagePumpWork;

            app.Initialize(EnginePath, settings);

            CefApi.RegisterSchemeHandlerFactory("yorot", "" /* <-- Optional */, new Handlers.YorotSchemeHandlerFactory());

            BuildAvaloniaApp()
                .StartWithCefNetApplicationLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();

        private static void App_FrameworkInitialized(object? sender, EventArgs e)
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

        private static void App_FrameworkShutdown(object? sender, EventArgs e)
        {
            messagePump?.Stop();
        }

        private static async void OnScheduleMessagePumpWork(long delayMs)
        {
            await Task.Delay((int)delayMs);
            Dispatcher.UIThread.Post(CefApi.DoMessageLoopWork);
        }

        private static string EnginePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".yorot", "engine");

        private static string EngineLocalePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".yorot", "engine", "locales");

        private static string UserDataPath => AvailableUser(Yorot.StaticVariables.LastUser);

        private static string AvailableUser(string userName)
        {
            return (!string.IsNullOrWhiteSpace(userName) || userName == "root" || userName == "incognito") ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".yorot", "user", userName, "cache") : null;
        }
    }
}