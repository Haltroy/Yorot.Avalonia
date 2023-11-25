using System;
using System.Data.SqlTypes;
using System.Globalization;

namespace Yorot.Standard
{
    public class Version
    {
        public Version()
        {
        }

        public Version(int major, int minor, int minorRevision, int build, VersionStatus status)
        {
            Major = major;
            Minor = minor;
            MinorRevision = minorRevision;
            Build = build;
            Status = status;
        }

        public int Major { get; set; }
        public int Minor { get; set; }
        public int MinorRevision { get; set; }
        public int Build { get; set; }
        public VersionStatus Status { get; set; }

        public static Version Parse(string version)
        {
            var info = version.Split('.');
            var _Major = 0;
            var _Minor = 0;
            var _MinorRevision = 0;
            var _Build = 0;
            VersionStatus _Status = null;
            if (info.Length > 0 && int.TryParse(info[0], NumberStyles.None, null, out var major)) _Major = major;
            if (info.Length > 1 && int.TryParse(info[1], NumberStyles.None, null, out var minor)) _Minor = minor;
            if (info.Length > 2 && int.TryParse(info[2], NumberStyles.None, null, out var minorRevision))
                _MinorRevision = minorRevision;
            if (info.Length > 3 && int.TryParse(info[3], NumberStyles.None, null, out var build)) _Build = build;
            if (info.Length > 4) _Status = VersionStatus.Parse(info[4]);
            return new Version(_Major, _Minor, _MinorRevision, _Build, _Status);
        }

        public override string ToString()
        {
            return ToString();
        }

        public string ToString(bool full = false)
        {
            var version = string.Empty;
            var versionParse = new string[5];

            versionParse[0] = Major + "";
            versionParse[0] = Minor > 0 ? Minor + "" : MinorRevision > 0 ? "0" : Build > 0 ? "0" : "";
            versionParse[0] = MinorRevision > 0 ? MinorRevision + "" : Build > 0 ? "0" : "";
            versionParse[0] = Build > 0 ? Build + "" : "";
            versionParse[0] = !Status.IsNull ? Status.ToString() : "";

            for (var i = 0; i < versionParse.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(versionParse[i]))
                    version += versionParse[i];

                if (i + 1 >= versionParse.Length)
                    version += ".";
            }

            return version;
        }

        public class VersionStatus : INullable
        {
            public enum StatusType
            {
                /// <summary>
                ///     Indicates that this version is a pre-release.
                /// </summary>
                PreRelease,

                /// <summary>
                ///     Indicates that this version is a development test release (like from git repository).
                /// </summary>
                Dev,

                /// <summary>
                ///     Indicates that this version is a hotfix to fix a critical issue.
                /// </summary>
                Hotfix
            }

            public StatusType Type { get; set; }
            public int Index { get; set; }

            public bool IsNull { get; }

            public static VersionStatus Parse(string versionStatus)
            {
                throw new NotImplementedException();
            }

            public override string ToString()
            {
                return Type.ToString() + Index;
            }
        }
    }
}