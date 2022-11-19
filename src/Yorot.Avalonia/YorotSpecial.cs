using HTAlt;
using System;
using System.Collections.Generic;
using LibFoster;
using Yorot;
using CefNet;
using System.Reflection;

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
            // Yorot-Avalonia

            var packages = GetPackages();

            string packlist = "";

            for (int i = 0; i < packages.Length / 2; i++)
            {
                packlist += "<a><b>" + packages[i, 0] + ":</b>" + packages[i, 1] + "</a></br>" + Environment.NewLine;
            }

            RegisterWebSource("yorot://newtab", YorotTools.ReadResource("Yorot_Avalonia.WebSources.newtab.html"), "text/html", false, false);
            RegisterWebSource("yorot://test", YorotTools.ReadResource("Yorot_Avalonia.WebSources.test.html"), "text/html", false, false);
            RegisterWebSource("yorot://license", YorotTools.ReadResource("Yorot_Avalonia.WebSources.license.html"), "text/html", false, false);
            RegisterWebSource("yorot://links", YorotTools.ReadResource("Yorot_Avalonia.WebSources.links.html"), "text/html", false, false);
            RegisterWebSource("yorot://noint", YorotTools.ReadResource("Yorot_Avalonia.WebSources.noint.html"), "text/html", true, true);
            RegisterWebSource("yorot://technical", YorotTools.ReadResource("Yorot_Avalonia.WebSources.technical.html").Replace("[Parameter.Packages]", packlist), "text/html", false, false);
            RegisterWebSource("yorot://error", YorotTools.ReadResource("Yorot_Avalonia.WebSources.error.html"), "text/html", true, true);
            RegisterWebSource("yorot://certerror", YorotTools.ReadResource("Yorot_Avalonia.WebSources.certerror.html"), "text/html", true, true);
            RegisterWebSource("yorot://incognito", YorotTools.ReadResource("Yorot_Avalonia.WebSources.incognito.html"), "text/html", false, false);
            RegisterWebSource("yorot://map", YorotTools.ReadResource("Yorot_Avalonia.WebSources.map.html"), "text/html", true, true);
            RegisterWebSource("yorot://empty", "", "text/html", true, true);
            RegisterWebSource("yorot://search", "<meta http-equiv=\"refresh\" content=\"0; URL=[Parameter.q]\" />\r\n", "text/html", true, true);
            RegisterWebSource("yorot://homepage", "<meta http-equiv=\"refresh\" content=\"0; URL=[Info.Homepage]\" />\r\n", "text/html", true, true);
            RegisterWebSource("yorot://dad", "<meta http-equiv=\"refresh\" content=\"0; URL=https://haltroy.com\" />\r\n", "text/html", false, false);
            RegisterWebSource("yorot://me", "<meta http-equiv=\"refresh\" content=\"0; URL=https://haltroy.com/yorot/\" />\r\n", "text/html", false, false);

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

        public static string[,] GetPackages()
        {
            var referencedAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            var list = new string[referencedAssemblies.Length, 2];
            for (int i = 0; i < referencedAssemblies.Length; i++)
            {
                AssemblyName? assembly = referencedAssemblies[i];
                if (assembly != null)
                {
                    list[i, 0] = assembly.Name ?? "/!\\ [Unknown Package]";
                    list[i, 1] = assembly.Version != null ? assembly.Version.ToString() : "";
                }
            }
            return list;
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
                }
            }
            return requested;
        }

        public List<Views.MainWindow> MainForms { get; set; } = new List<Views.MainWindow>();
        public Views.MainWindow MainForm { get => MainForms[0]; }
    }
}