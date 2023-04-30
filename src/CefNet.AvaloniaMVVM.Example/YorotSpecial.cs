﻿using HTAlt;
using System;
using System.Collections.Generic;
using LibFoster;
using Yorot;
using CefNet;
using System.Reflection;
using Avalonia.Controls;
using Avalonia;
using FluentAvalonia.Styling;
using MessageBox.Avalonia.Models;
using System.Threading.Tasks;
using System.Drawing;
using Yorot_Avalonia.Views;

namespace Yorot_Avalonia
{
    public class YorotSpecial : YorotMain
    {
        public YorotSpecial(string appPath, string name, string codename, string version, int verno, bool isIncognito = false) : base(appPath, name, codename, version, verno, "Yorot-Avalonia", YorotGlobal.isPreOut, isIncognito)
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

            StaticVariables.Locale = Locale.ToString();
            StaticVariables.LastUser = Profiles.Current.Name;

            var packages = GetPackages();

            string packlist = "";

            for (int i = 0; i < packages.Length / 2; i++)
            {
                packlist += "<a><b>" + packages[i, 0] + ":</b>" + packages[i, 1] + "</a></br>" + Environment.NewLine;
            }

            RegisterWebSource("yorot://jquery.min", YorotTools.ReadResource("WebSources.jquery.min.js"), "text/javascript", false, true);
            RegisterWebSource("yorot://newtab", YorotTools.ReadResource("WebSources.newtab.html"), "text/html", false, false);
            RegisterWebSource("yorot://test", YorotTools.ReadResource("WebSources.test.html"), "text/html", false, false);
            RegisterWebSource("yorot://license", YorotTools.ReadResource("WebSources.license.html"), "text/html", false, false);
            RegisterWebSource("yorot://links", YorotTools.ReadResource("WebSources.links.html"), "text/html", false, false);
            RegisterWebSource("yorot://noint", YorotTools.ReadResource("WebSources.noint.html"), "text/html", true, true);
            RegisterWebSource("yorot://technical", YorotTools.ReadResource("WebSources.technical.html").Replace("[Parameter.Packages]", packlist), "text/html", false, false);
            RegisterWebSource("yorot://error", YorotTools.ReadResource("WebSources.error.html"), "text/html", true, true);
            RegisterWebSource("yorot://certerror", YorotTools.ReadResource("WebSources.certerror.html"), "text/html", true, true);
            RegisterWebSource("yorot://incognito", YorotTools.ReadResource("WebSources.incognito.html"), "text/html", false, false);
            RegisterWebSource("yorot://map", YorotTools.ReadResource("WebSources.map.html"), "text/html", true, true);
            RegisterWebSource("yorot://empty", "", "text/html", true, true);
            RegisterWebSource("yorot://search", "<meta http-equiv=\"refresh\" content=\"0; URL=[Parameter.q]\" />\r\n", "text/html", true, true);
            RegisterWebSource("yorot://homepage", "<meta http-equiv=\"refresh\" content=\"0; URL=[Info.Homepage]\" />\r\n", "text/html", false, false);
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

        private async Task<YorotPermissionMode> OnPermissionRequestAsync(YorotPermission permission, YorotPermissionMode requested, string ClassType, string name, string title)
        {
            // TODO: In here, everything should work fine. Except the below which throws a InvalidOperationException
            // Maybe at this time MessageBox.Avalonia wasn't ready yet?
            var box = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxCustomWindow(new MessageBox.Avalonia.DTO.MessageBoxCustomParams()
            {
                Icon = MessageBox.Avalonia.Enums.Icon.Question,
                ContentTitle = "Yorot",
                ContentHeader = YorotGlobal.Main.CurrentLanguage.GetItemText("Permission.Request" + ClassType).Replace("[Parameter.name]", name).Replace("[Parameter.title]", title).Replace("[Parameter.permission]", YorotGlobal.Main.CurrentLanguage.GetItemText("Permission." + permission.ID)),
                ContentMessage = YorotGlobal.Main.CurrentLanguage.GetItemText("Permission." + permission.ID, false),
                ButtonDefinitions = new[]
                            {
                            new ButtonDefinition() { Name = YorotGlobal.Main.CurrentLanguage.GetItemText("Permission.Allow")},
                            new ButtonDefinition() { Name = YorotGlobal.Main.CurrentLanguage.GetItemText("Permission.AllowOnce")},
                            new ButtonDefinition() { Name = YorotGlobal.Main.CurrentLanguage.GetItemText("Permission.Deny")},
                        },
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            });

            var result = await box.Show();

            if (result == YorotGlobal.Main.CurrentLanguage.GetItemText("Permission.Allow"))
            {
                return requested;
            }
            else if (result == YorotGlobal.Main.CurrentLanguage.GetItemText("Permission.AllowOnce"))
            {
                return YorotPermissionMode.AllowOneTime;
            }
            else
            {
                return YorotPermissionMode.Deny;
            }
        }

        public override System.Threading.Tasks.Task<YorotPermissionMode> OnPermissionRequest(YorotPermission permission, YorotPermissionMode requested)
        {
            if (permission.Allowance != requested)
            {
                switch (permission.Requestor)
                {
                    case YorotApp _:
                        var app = permission.Requestor as YorotApp;

                        return Avalonia.Threading.Dispatcher.UIThread.InvokeAsync<YorotPermissionMode>(() => { return OnPermissionRequestAsync(permission, requested, "App", app.AppCodeName, app.AppName); });

                    case YorotExtension _:
                        var ext = permission.Requestor as YorotExtension;

                        return Avalonia.Threading.Dispatcher.UIThread.InvokeAsync<YorotPermissionMode>(() =>
                        {
                            return OnPermissionRequestAsync(permission, requested, "Ext", ext.CodeName, ext.Name);
                        });

                    case YorotTheme _:
                        var theme = permission.Requestor as YorotTheme;

                        return Avalonia.Threading.Dispatcher.UIThread.InvokeAsync<YorotPermissionMode>(() =>
                        {
                            return OnPermissionRequestAsync(permission, requested, "Theme", theme.CodeName, theme.Name);
                        });

                    case YorotLanguage _:
                        var lang = permission.Requestor as YorotLanguage;

                        return Avalonia.Threading.Dispatcher.UIThread.InvokeAsync<YorotPermissionMode>(() =>
                        {
                            return OnPermissionRequestAsync(permission, requested, "Lang", lang.CodeName, lang.Name);
                        });

                    case YorotSite _:
                        var site = permission.Requestor as YorotSite;

                        return Avalonia.Threading.Dispatcher.UIThread.InvokeAsync<YorotPermissionMode>(() =>
                        {
                            return OnPermissionRequestAsync(permission, requested, "Site", site.Url, site.Name);
                        });

                    default:

                        return Avalonia.Threading.Dispatcher.UIThread.InvokeAsync<YorotPermissionMode>(() =>
                        {
                            return OnPermissionRequestAsync(permission, requested, permission.Requestor.GetType().FullName ?? "Unknown", "", "");
                        });
                }
            }
            else
            {
                return new Task<YorotPermissionMode>(() => { return requested; });
            }
        }

        public List<Views.MainWindow> MainForms { get; set; } = new List<Views.MainWindow>();
        public Views.MainWindow MainForm { get => MainForms[0]; }

        public List<object> UIs { get; set; } = new List<object>();

        public void UpdateUIs()
        {
            if (CurrentSettings != null && CurrentTheme != null)
            {
                FluentAvaloniaTheme? fluentTheme = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>();
                if (fluentTheme != null)
                {
                    fluentTheme.CustomAccentColor = Avalonia.Media.Color.Parse(CurrentTheme.OverlayColor.ToHex());
                }
            }
            for (int i = 0; i < UIs.Count; i++)
            {
                switch (UIs[i])
                {
                    case Window:
                        (UIs[i] as Window).RefreshWindow();
                        break;

                    case UserControl:
                        (UIs[i] as UserControl).RefreshUserControl();
                        break;
                }
            }
        }

        public override void OnThemeChange(YorotTheme theme)
        {
            UpdateUIs();
        }

        public override void OnFavoriteChange(YorotFavFolder fav)
        {
            for (int i = 0; i < MainForms.Count; i++)
            {
                // TODO: update this specific favorite
            }
        }

        public override void OnLanguageChange(YorotLanguage lang)
        {
            UpdateUIs();
        }

        public override void OnSiteChange(YorotSite site)
        {
            // TODO: refresh tabs that are on this site
        }

        public override void OnDownloadChange(YorotSite site)
        {
            // TODO: Update this specific download
        }

        public override void OnAppListChanged()
        {
            // TODO: Update App List
        }

        public override void OnLangListChanged()
        {
            // TODO: Update Language list on settings
        }

        public override void OnThemeListChanged()
        {
            // TODO: Update Theme list on settings
        }

        public override void OnExtListChanged()
        {
            // TODO: Update Extension list on settings and windows
        }

        public override void OnDownloadListChanged()
        {
            // TODO: Update download list on its app
        }

        public override void OnHistoryChanged()
        {
            // TODO: Update History list on settings
        }

        public override void OnProfileChange(YorotProfile profile)
        {
            // TODO: Update this profile on profile flyout & settings
        }

        public override void OnProfileListChanged()
        {
            // TODO: Update porfile list on settings
        }
    }
}