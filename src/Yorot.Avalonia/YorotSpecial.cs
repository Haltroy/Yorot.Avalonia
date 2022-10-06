using HTAlt;
using System;
using System.Collections.Generic;
using LibFoster;
using Yorot;
using CefNet;

namespace Yorot_Avalonia
{
    public class YorotSpecial : YorotMain
    {
        public YorotSpecial(string appPath, string name, string codename, string version, int verno, bool isIncognito = false) : base(appPath, name, codename, version, verno, "Yorot-Win32", YorotGlobal.isPreOut, isIncognito)
        {
#if DEBUG
#else
            YorotUpdate = new Foster(name, YorotGlobal.HTULoc, YorotGlobal.AppExePath, YorotGlobal.VersionNo, YorotGlobal.Arch);
#endif
        }

        public override void BeforeInit()
        {
            Output.LogDirPath = AppPath + System.IO.Path.DirectorySeparatorChar + "logs" + System.IO.Path.DirectorySeparatorChar;
            YorotDefaultLangs.GenLangs(LangFolder);
        }

        public Foster? YorotUpdate = null;

        public bool IsUpToDate => YorotUpdate != null ? YorotUpdate.IsUpToDate : false;

        public override void AfterInit()
        {
            // CEF

            var settings = new CefSettings();
            settings.LocalesDirPath = EngineLocaleFolder;
            settings.ResourcesDirPath = EngineFolder;
            settings.NoSandbox = true;
            // These two should be always true, otherwise it won't create the browser obejct or display anything.
            settings.MultiThreadedMessageLoop = true;
            settings.WindowlessRenderingEnabled = true;

            settings.Locale = "tr";

            settings.UserDataPath = Profiles.Current.CacheLoc;
            settings.UserAgent = GetUserAgent("Chrome", YorotGlobal.ChromiumVersion);

            var app = new CefNetApplication();
            app.Initialize(EngineFolder, settings);

            CefApi.RegisterSchemeHandlerFactory("yorot", "", new Handlers.YorotSchemeHandlerFactory());

            // Yorot-Avalonia

            RegisterWebSource("yorot://newtab", Properties.Resources.newtab, "text/html", false);
            RegisterWebSource("yorot://test", Properties.Resources.test, "text/html", false);
            RegisterWebSource("yorot://license", Properties.Resources.license, "text/html", false);
            RegisterWebSource("yorot://links", Properties.Resources.links, "text/html", false);
            RegisterWebSource("yorot://noint", Properties.Resources.noint, "text/html", true);
            RegisterWebSource("yorot://technical", Properties.Resources.technical, "text/html", false);
            RegisterWebSource("yorot://error", Properties.Resources.error, "text/html", true);
            RegisterWebSource("yorot://certerror", Properties.Resources.certerror, "text/html", true);
            RegisterWebSource("yorot://incognito", Properties.Resources.incognito, "text/html", false);
            RegisterWebSource("yorot://search", "<meta http-equiv=\"refresh\" content=\"0; URL=[Parameter.q]\" />\r\n", "text/html", true);
            RegisterWebSource("yorot://homepage", "<meta http-equiv=\"refresh\" content=\"0; URL=[Info.Homepage]\" />\r\n", "text/html", true);

            // Yopad
            Yopad.YopadLog += new Foster.OnLogEntryDelegate((sender, e) => { Output.WriteLine(e.LogEntry, (HTAlt.LogLevel)((int)e.Level)); });
            Yopad.YopadProgress += new Yopad.YopadProgressEventHandler((sender, e) => { Output.WriteLine("[Yopad] Progress: " + e.Percentage100); });
#if DEBUG
#else
            Yopad.RefreshRepos(true);
#endif
            long addonCount = 0;
            for (int i = 0; i < Yopad.Repositories.Count; i++)
            {
                var repo = Yopad.Repositories[i];
                for (int j = 0; j < repo.Addons.Count; j++)
                {
                    var list = repo.Addons[j];
                    for (int ı = 0; ı < list.Categories.Count; ı++)
                    {
                        var cat = list.Categories[ı];
                        addonCount += cat.Addons.Count;
                    }
                }
            }
        }

        public override string CurrentEngineVer
        {
            get => YorotGlobal.ChromiumVersion;
            set { }
        }

        public override YorotPermissionMode OnPermissionRequest(YorotPermission permission, YorotPermissionMode requested)
        {
            if (permission.Allowance != requested)
            {
                // TODO
                switch (permission.Requestor)
                {
                    case YorotApp _:
                        Console.WriteLine("Permission \"" + permission.ID + "\" request accepted (NOT_IMPLEMENTED) from ID\"" + ((YorotApp)permission.Requestor).AppCodeName + "\" " + permission.Allowance.ToString() + " => " + requested.ToString());
                        break;

                    case YorotExtension _:
                        Console.WriteLine("Permission \"" + permission.ID + "\" request accepted (NOT_IMPLEMENTED) from ID\"" + ((YorotExtension)permission.Requestor).CodeName + "\" " + permission.Allowance.ToString() + " => " + requested.ToString());
                        break;

                    case YorotTheme _:
                        Console.WriteLine("Permission \"" + permission.ID + "\" request accepted (NOT_IMPLEMENTED) from ID\"" + ((YorotTheme)permission.Requestor).CodeName + "\" " + permission.Allowance.ToString() + " => " + requested.ToString());
                        break;

                    case YorotLanguage _:
                        Console.WriteLine("Permission \"" + permission.ID + "\" request accepted (NOT_IMPLEMENTED) from ID\"" + ((YorotLanguage)permission.Requestor).CodeName + "\" " + permission.Allowance.ToString() + " => " + requested.ToString());
                        break;

                    case ExpPack _:
                        Console.WriteLine("Permission \"" + permission.ID + "\" request accepted (NOT_IMPLEMENTED) from ID\"" + ((ExpPack)permission.Requestor).CodeName + "\" " + permission.Allowance.ToString() + " => " + requested.ToString());
                        break;
                }
                permission.Allowance = requested;
            }
            return requested;
        }

        public List<Views.MainWindow> MainForms { get; set; } = new List<Views.MainWindow>();
        public Views.MainWindow MainForm { get => MainForms[0]; }
    }
}