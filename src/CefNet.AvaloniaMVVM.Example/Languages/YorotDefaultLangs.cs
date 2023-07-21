using HTAlt;
using System;
using System.Text;
using Yorot;

namespace Yorot_Avalonia
{
    /// <summary>
    /// Static class containing default language configurations.
    /// </summary>
    public static class YorotDefaultLangs
    {
        /// <summary>
        /// Generates the default languages.
        /// </summary>
        /// <param name="langLoc">Location of the language folder.</param>
        public static void GenLangs(string langLoc, bool force = false)
        {
            if (!langLoc.EndsWith(System.IO.Path.DirectorySeparatorChar)) { langLoc += System.IO.Path.DirectorySeparatorChar; }
            var d = YorotDefaultLanguages.DefaultLangList;
            for (int i = 0; i < d.Length; i++)
            {
                string l = d[i];
                string _ll = langLoc + l + System.IO.Path.DirectorySeparatorChar;
                if (!System.IO.Directory.Exists(_ll)) { System.IO.Directory.CreateDirectory(_ll); }
                string _l = _ll + l + ".ylf";

                if (force || YorotGlobal.isPreOut || !System.IO.File.Exists(_l))
                {
                    System.IO.File.Create(_l).Close();

                    HTAlt.Tools.WriteFile(_l, GetDefaultLang(l), Encoding.Unicode);
                }
            }
        }

        private static string LangDefaultUrl => "https://raw.githubusercontent.com/Haltroy/Yopad/main/Languages/[LANG]/.foster";

        /// <summary>
        /// An array of languages that are default to this Yorot flavor.
        /// </summary>
        /// <summary>
        /// Gets the default configuration from <paramref name="codeName"/>.
        /// </summary>
        /// <param name="codeName">Code Name of the language.</param>
        /// <returns><see cref="string"/></returns>
        private static string GetDefaultLang(string codeName)
        {
            switch (codeName.ToLowerEnglish())
            {
                default:
                    Output.WriteLine("[DefaultLangs] Cannot find language \"" + codeName + "\" (might not be implemented yet).", LogLevel.Warning);
                    return "";

                case "com.haltroy.english":
                case "com.haltroy.english-gb":
                case "com.haltroy.english-us":
                    return YorotTools.ReadResource("Languages.English-US.ylf");

                case "com.haltroy.turkish":
                    return YorotTools.ReadResource("Languages.Turkish.ylf");
            }
        }
    }
}