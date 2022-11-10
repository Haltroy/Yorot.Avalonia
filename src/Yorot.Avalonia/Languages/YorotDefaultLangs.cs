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
                    Output.WriteLine("[DefaultLangs] Cannot find language \"" + codeName + "\". Loaded com.haltroy.english-us.", LogLevel.Warning);
                    return YorotTools.ReadResource("Yorot_Avalonia.Languages.English-US.ylf");

                case "com.haltroy.english":
                case "com.haltroy.english-us":
                    return YorotTools.ReadResource("Yorot_Avalonia.Languages.English-US.ylf");

                case "com.haltroy.english-gb":
                    return YorotTools.ReadResource("Yorot_Avalonia.Languages.English-GB.ylf");

                case "com.haltroy.turkish":
                    return YorotTools.ReadResource("Yorot_Avalonia.Languages.Turkish.ylf");

                case "com.haltroy.persian":

                case "com.haltroy.albanian":

                case "com.haltroy.arabic":

                case "com.haltroy.portuguese":

                case "com.haltroy.spanish":

                case "com.haltroy.armenian":

                case "com.haltroy.german":

                case "com.haltroy.azerbaijani":

                case "com.haltroy.bengali":

                case "com.haltroy.belarusian":

                case "com.haltroy.dutch":

                case "com.haltroy.french":

                case "com.haltroy.dzongkha":

                case "com.haltroy.bosnian":

                case "com.haltroy.malay":

                case "com.haltroy.bulgarian":

                case "com.haltroy.khmer":

                case "com.haltroy.chinese_simplified":

                case "com.haltroy.croatian":

                case "com.haltroy.greek":

                case "com.haltroy.czech":

                case "com.haltroy.danish":

                case "com.haltroy.tigrinya":

                case "com.haltroy.estonian":

                case "com.haltroy.afar":

                case "com.haltroy.finnish":

                case "com.haltroy.georgian":

                case "com.haltroy.hungarian":

                case "com.haltroy.icelandic":

                case "com.haltroy.hindi":

                case "com.haltroy.indonesian":

                case "com.haltroy.irish":

                case "com.haltroy.italian":

                case "com.haltroy.japanese":

                case "com.haltroy.kazakh":

                case "com.haltroy.korean":

                case "com.haltroy.latvian":

                case "com.haltroy.sotho":

                case "com.haltroy.lithuanian":

                case "com.haltroy.romanian":

                case "com.haltroy.mongolian":

                case "com.haltroy.montenegrin":

                case "com.haltroy.nepali":

                case "com.haltroy.norwegian":

                case "com.haltroy.urdu":

                case "com.haltroy.filipino":

                case "com.haltroy.polish":

                case "com.haltroy.serbian":

                case "com.haltroy.slovak":

                case "com.haltroy.slovene":

                case "com.haltroy.afrikaans":

                case "com.haltroy.sinhala":

                case "com.haltroy.swedish":

                case "com.haltroy.chinese_traditional":

                case "com.haltroy.tajik":

                case "com.haltroy.thai":

                case "com.haltroy.turkmen":

                case "com.haltroy.ukrainian":

                case "com.haltroy.english_us":

                case "com.haltroy.uzbek":

                case "com.haltroy.vietnamese":

                case "com.haltroy.chewa":

                case "com.haltroy.russian":

                case "com.haltroy.tamazight":

                case "com.haltroy.kyrgyz":
                    // TODO: Translate to these languages, do this last
                    Output.WriteLine("[DefaultLangs] Language \"" + codeName + "\" is not implemented yet! Loaded com.haltroy.english-us.", LogLevel.Warning);
                    return YorotTools.ReadResource("Yorot_Avalonia.Languages.English-US.ylf");
            }
        }
    }
}