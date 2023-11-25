using System.IO;
using System.Xml;
using Yorot.Standard;

namespace Yorot.Installer;

public class YorotFlavorManager
{
    internal static string FlavorInfoPath => Path.Combine(Directory.GetCurrentDirectory(), "flavors");
    internal static string TempPath => Path.Combine(Path.GetTempPath(), "yorotinstaller");
    internal static string TempImagePath => Path.Combine(TempPath, "images");
    internal static string TempFlavorPath => Path.Combine(TempPath, "flavors");
    internal static string TempPackagePath => Path.Combine(TempPath, "packages");
    internal static string TempGitPath => Path.Combine(TempPath, "git");
    public void Init()
    {
        if (!Directory.Exists(FlavorInfoPath)) Directory.CreateDirectory(FlavorInfoPath);
        if (!Directory.Exists(TempPath)) Directory.CreateDirectory(TempPath);
        if (!Directory.Exists(TempImagePath)) Directory.CreateDirectory(TempImagePath);
        if (!Directory.Exists(TempFlavorPath)) Directory.CreateDirectory(TempFlavorPath);
        if (!Directory.Exists(TempPackagePath)) Directory.CreateDirectory(TempPackagePath);
        if (!Directory.Exists(TempGitPath)) Directory.CreateDirectory(TempGitPath);

        var flavors = Directory.GetFiles(FlavorInfoPath);
    }
    public YorotFlavor[] Flavors { get; set; }

    public bool CheckforInstallerUpdate()
    {
        // TODO
    }

    public void UpdateInstaller()
    {
        // TODO
    }

    public void LoadFromWeb()
    {
        // TODO
    }

    public void LoadFromLocalInstallation()
    {
        // TODO
    }

    public void LoadSpecificFlavor(YorotFlavor flavor)
    {
        // TODO
    }

    public Stream GetIcon(string path)
    {
        // TODO
    }
}

public class YorotFlavor
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Version? InstalledVersion { get; set; }
    public Version LatestVersion { get; set; }
    public bool IsInstalled => InstalledVersion != null;

    public Architectures[] SupportedPlatforms { get; set; }
    public bool AllowSwitchSourceBuild { get; set; }
    public bool SourceBuildDefault { get; set; }
    public string ID { get; set; }
    public string IconUrl { get; set; }
    public AdvertisementImage[] Adverts { get; set; }
    public ShortcutInfo Shotcuts { get; set; }
    public string FlavorInfoUrl { get; set; }

    public void ParseXml(XmlNode root)
    {
        // TODO
    }
}

public class ShortcutInfo
{
    public string AppPath { get; set; }
    public string Arguments { get; set; }
    public string IconPath { get; set; }
    public bool IsBrowser { get; set; }
}

public class PackageInfo
{
    public string PackageFile { get; set; }
    public Architectures Architecture { get; set; }
}

public enum PackageType
{
    Zip,
    Tar,
    TarGZip,
    TarZstd,
    TarBZip,
    TarXZ,
    Bundle,
    WinExe,
    WinMSI,
    LinuxAppImage
}

public class SourceBuildScheme
{
    public RequiredPackage[] RequiredPackages { get; set; }
    public string[] Commands { get; set; }
    public Architectures Architecture { get; set; }
}

public class AdvertisementImage
{
    public string ImageUrl { get; set; }
    public string RedirectLink { get; set; }
}

public class RequiredPackage
{
    public string CommandToCheck { get; set; }
    private string LinkToDownloadPage { get; set; }
}

public enum Architectures
{
    NoArch,
    FrameworkDependent,
    Win_x86,
    Win_x64,
    Win_ARM,
    Win_ARM64,
    OSX_x64,
    OSX_ARM64,
    Linux_x86,
    Linux_x64,
    Linux_ARM,
    Linux_ARM64
}