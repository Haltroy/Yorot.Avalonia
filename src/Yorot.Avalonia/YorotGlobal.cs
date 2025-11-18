using System;
using System.IO;
using System.Xml.Linq;

namespace Yorot_Avalonia
{
    /// <summary>
    /// Yorot Global Static Variables.
    /// </summary>
    public static class YorotGlobal
    {
        /// <summary>
        /// Version of the Chromium engine.
        /// </summary>
        public static string ChromiumVersion = "105.3.18.0";

        /// <summary>
        /// Version Number of this Yorot version.
        /// </summary>
        public static int VersionNo = 1;

        /// <summary>
        /// Codename of current Yorot version.
        /// </summary>
        public static string CodeName = "indev";

        public static ViewModels.ViewModelBase ViewModel { get; set; } = new ViewModels.ViewModelBase();

        public static string Arch
        {
            get
            {
                string arch = string.Empty;
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
                {
                    arch = "linux";
                }
                else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                {
                    arch = "win";
                }
                else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.FreeBSD))
                {
                    arch = "freebsd";
                }
                else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
                {
                    arch = "osx";
                }
                arch += "-" + Environment.OSVersion.Version.ToString();

                switch (System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture)
                {
                    case System.Runtime.InteropServices.Architecture.X86:
                        arch += "-x86";
                        break;

                    case System.Runtime.InteropServices.Architecture.X64:
                        arch += "-x64";
                        break;

                    case System.Runtime.InteropServices.Architecture.Arm:
                        arch += "-arm";
                        break;

                    case System.Runtime.InteropServices.Architecture.Arm64:
                        arch += "-arm64";
                        break;
                }
                return arch;
            }
        }

        /// <summary>
        /// Folder where Yorot.exe is hosted on.
        /// </summary>
        public static string AppExePath = Environment.CommandLine;

        /// <summary>
        /// Application location.
        /// </summary>
        public static string YorotAppPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + System.IO.Path.DirectorySeparatorChar + ".yorot" + System.IO.Path.DirectorySeparatorChar;

        /// <summary>
        /// Yorot Main
        /// </summary>
        public static YorotSpecial? Main;

        /// <summary>
        /// <c>true</c> if this session is a PreOut, otherwise <c>false</c>.
        /// </summary>
        public static bool isPreOut = true;

        public static string UserAgent
        {
            get
            {
                string osInfo = "";

                var os = System.Environment.OSVersion;

                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                {
                    osInfo = "Windows NT " + os.Version.Major + "." + os.Version.Minor + "; [WINPROC]; [PROC]";
                }
                else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
                {
                    // TODO: (LONG-TERM) If Avalonia gets a Wayland support, figure out which one we use.
                    // Currently, Avalonia is X11 only so this is not a problem.
                    osInfo = "X11; Linux [XPROC]";
                }
                else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
                {
                    osInfo = "Macintosh; [OSXPROC] Mac OS X" + os.Version.ToString().Replace(".", "_").Replace(" ", "_");
                }
                switch (System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture)
                {
                    case System.Runtime.InteropServices.Architecture.X86:
                        osInfo = osInfo.Replace("[PROC]", "x86").Replace("[WINPROC]", "Win32").Replace("[OSXPROC]", "Intel").Replace("[XPROC]", "i386");
                        break;

                    case System.Runtime.InteropServices.Architecture.X64:
                        osInfo = osInfo.Replace("[PROC]", "x64").Replace("[WINPROC]", "Win64").Replace("[OSXPROC]", "Intel").Replace("[XPROC]", "x86_64");
                        break;

                    case System.Runtime.InteropServices.Architecture.Arm:
                        osInfo = osInfo.Replace("[PROC]", "ARM").Replace("[WINPROC]", "WinARM").Replace("[OSXPROC]", "M1").Replace("[XPROC]", "ARM");
                        break;

                    case System.Runtime.InteropServices.Architecture.Arm64:
                        osInfo = osInfo.Replace("[PROC]", "Aarch64").Replace("[WINPROC]", "WinARM64").Replace("[OSXPROC]", "M1").Replace("[XPROC]", "ARM64");
                        break;
                }
                return $"Mozilla/5.0 ({osInfo}) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{ChromiumVersion} Safari/537.36 Yorot/{Version}";
            }
        }

        /// <summary>
        /// Version of Yorot.
        /// </summary>
        public static string Version = isPreOut ? CodeName : System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();

        /// <summary>
        /// Version Control (HTUPDATE) URL.
        /// </summary>
        public static string HTULoc = "https://raw.githubusercontent.com/Haltroy/Yorot/main/Yorot.htupdate";
    }
}