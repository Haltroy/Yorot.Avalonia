using System;
using System.IO;
using System.Text;

namespace Yorot
{
    internal class StaticVariables
    {
        public static string GetUserFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".yorot", "svar");

        public static string LastUserFilePath => Path.Combine(GetUserFolder, "lastuser");

        public static string LocaleFilePath => Path.Combine(GetUserFolder, "locale");

        public static string UserAgentFilePath => Path.Combine(GetUserFolder, "useragent");

        public static string LastUser
        {
            get => CheckAndFixFolderAndFile(LastUserFilePath);
            set => CheckAndFixFolderAndFile(LastUserFilePath, value);
        }

        public static string Locale
        {
            get => CheckAndFixFolderAndFile(LocaleFilePath);
            set => CheckAndFixFolderAndFile(LocaleFilePath, value);
        }

        private static string CheckAndFixFolderAndFile(string file, string? data = null)
        {
            if (!Directory.Exists(GetUserFolder))
            {
                Directory.CreateDirectory(GetUserFolder);
            }
            if (!File.Exists(file))
            {
                var fileStream = File.Create(file);
                if (data != null)
                {
                    using (StreamWriter writer = new StreamWriter(fileStream, Encoding.Unicode))
                    {
                        writer.WriteLine(data);
                    }
                }
                fileStream.Close();
            }
            return HTAlt.Tools.ReadFile(file, Encoding.Unicode);
        }

        public static string UserAgent
        {
            get => CheckAndFixFolderAndFile(UserAgentFilePath, "");
            set => CheckAndFixFolderAndFile(UserAgentFilePath, value);
        }
    }
}