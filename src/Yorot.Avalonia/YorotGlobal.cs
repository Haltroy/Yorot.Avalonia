using System;

namespace Yorot_Avalonia
{
    /// <summary>
    /// Yorot Global Static Variables.
    /// </summary>
    public static class YorotGlobal
    {
        public static ViewModels.ViewModelBase ViewModel { get; set; }

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

        /// <summary>
        /// Version of the Chromium engine.
        /// </summary>
        public static string ChromiumVersion = "105.3.18.0";

        /// <summary>
        /// Version of Yorot.
        /// </summary>
        public static string Version = isPreOut ? "indev1" : System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();

        /// <summary>
        /// Version Number of this Yorot version.
        /// </summary>
        public static int VersionNo = 1;

        /// <summary>
        /// Codename of current Yorot version.
        /// </summary>
        public static string CodeName = "indev";

        /// <summary>
        /// Placeholder text used by default apps.
        /// </summary>
        public static string DefaultaAppOriginPlaceHolder = "8 February 2021 00:19:00 GMT+3:00" + Environment.NewLine + "https://github.com/Haltroy/Yorot" + Environment.NewLine + "Yorot C# Embedded Code" + Environment.NewLine + "(<Source>)"; // LONG-TERM TODO: Change date on releases.

        /// <summary>
        /// Version Control (HTUPDATE) URL.
        /// </summary>
        public static string HTULoc = "https://raw.githubusercontent.com/Haltroy/Yorot/main/Yorot.htupdate";
    }
}