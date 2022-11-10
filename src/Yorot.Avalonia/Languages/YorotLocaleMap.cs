using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yorot;

namespace Yorot_Avalonia
{
    internal class YorotLocaleMap
    {
        public YorotLocaleMap()
        {
            NewItem("AF", YorotLocale.fa, "com.haltroy.persian", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("AL", YorotLocale.en_US, "com.haltroy.albanian", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("DZ", YorotLocale.ar, "com.haltroy.arabic", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("AO", YorotLocale.pt_PT, "com.haltroy.portuguese", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("AR", YorotLocale.es_419, "com.haltroy.spanish", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("AM", YorotLocale.en_US, "com.haltroy.armenian", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("AU", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("AT", YorotLocale.de, "com.haltroy.german", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("AZ", YorotLocale.en_US, "com.haltroy.azerbaijani", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("BS", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("BD", YorotLocale.bn, "com.haltroy.bengali", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("BY", YorotLocale.en_US, "com.haltroy.belarusian", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("BE", YorotLocale.nl, "com.haltroy.dutch", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("BZ", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("BJ", YorotLocale.fr, "com.haltroy.french", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("BT", YorotLocale.en_US, "com.haltroy.dzongkha", new YorotDateAndTime[] { YorotDateAndTime.YMD });
            NewItem("BO", YorotLocale.es_419, "com.haltroy.spanish", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("BA", YorotLocale.en_US, "com.haltroy.bosnian", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("BW", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("BR", YorotLocale.pt_BR, "com.haltroy.portuguese", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("BN", YorotLocale.ms, "com.haltroy.malay", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("BG", YorotLocale.bg, "com.haltroy.bulgarian", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("BF", YorotLocale.fr, "com.haltroy.french", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("BI", YorotLocale.fr, "com.haltroy.french", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("KH", YorotLocale.en_US, "com.haltroy.khmer", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("CM", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("CA", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.MDY, YorotDateAndTime.YMD });
            NewItem("CF", YorotLocale.fr, "com.haltroy.french", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("TD", YorotLocale.ar, "com.haltroy.arabic", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("CL", YorotLocale.en_US, "com.haltroy.chilean spanish", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("CN", YorotLocale.zh_CN, "com.haltroy.chinese simplified", new YorotDateAndTime[] { YorotDateAndTime.YMD });
            NewItem("CO", YorotLocale.es_419, "com.haltroy.spanish", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("CG", YorotLocale.fr, "com.haltroy.french", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("CR", YorotLocale.es_419, "com.haltroy.spanish", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("HR", YorotLocale.hr, "com.haltroy.croatian", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("CU", YorotLocale.es_419, "com.haltroy.spanish", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("CY", YorotLocale.el, "com.haltroy.greek", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("CZ", YorotLocale.cs, "com.haltroy.czech", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("DK", YorotLocale.da, "com.haltroy.danish", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("DJ", YorotLocale.ar, "com.haltroy.arabic", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("DO", YorotLocale.es_419, "com.haltroy.spanish", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("EC", YorotLocale.es_419, "com.haltroy.spanish", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("EG", YorotLocale.ar, "com.haltroy.arabic", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("SV", YorotLocale.es_419, "com.haltroy.spanish", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("GQ", YorotLocale.fr, "com.haltroy.french", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("ER", YorotLocale.en_US, "com.haltroy.tigrinya", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.MDY, YorotDateAndTime.YMD });
            NewItem("EE", YorotLocale.et, "com.haltroy.estonian", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("ET", YorotLocale.en_US, "com.haltroy.afar", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.MDY });
            NewItem("FI", YorotLocale.fi, "com.haltroy.finnish", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.MDY });
            NewItem("FJ", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("TF", YorotLocale.fr, "com.haltroy.french", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("GA", YorotLocale.fr, "com.haltroy.french", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("GM", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("GE", YorotLocale.en_US, "com.haltroy.georgian", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("DE", YorotLocale.de, "com.haltroy.german", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("GH", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.MDY, YorotDateAndTime.YMD });
            NewItem("GR", YorotLocale.el, "com.haltroy.greek", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("GL", YorotLocale.en_US, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.MDY });
            NewItem("GT", YorotLocale.es_419, "com.haltroy.spanish", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("GN", YorotLocale.fr, "com.haltroy.french", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.MDY, YorotDateAndTime.YMD });
            NewItem("GW", YorotLocale.pt_PT, "com.haltroy.portuguese", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("GY", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("HT", YorotLocale.fr, "com.haltroy.french", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("HN", YorotLocale.es_419, "com.haltroy.spanish", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("HU", YorotLocale.hu, "com.haltroy.hungarian", new YorotDateAndTime[] { YorotDateAndTime.YMD });
            NewItem("IS", YorotLocale.en_US, "com.haltroy.icelandic", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("IN", YorotLocale.hi, "com.haltroy.hindi", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.MDY, YorotDateAndTime.YMD });
            NewItem("ID", YorotLocale.id, "com.haltroy.indonesian", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.MDY });
            NewItem("IQ", YorotLocale.ar, "com.haltroy.arabic", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("IE", YorotLocale.en_US, "com.haltroy.irish", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("IT", YorotLocale.it, "com.haltroy.italian", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("JM", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("JP", YorotLocale.ja, "com.haltroy.japanese", new YorotDateAndTime[] { YorotDateAndTime.YMD });
            NewItem("JO", YorotLocale.ar, "com.haltroy.arabic", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("KZ", YorotLocale.en_US, "com.haltroy.kazakh", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("KE", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.MDY, YorotDateAndTime.YMD });
            NewItem("KP", YorotLocale.ko, "com.haltroy.korean", new YorotDateAndTime[] { YorotDateAndTime.YMD });
            NewItem("KR", YorotLocale.ko, "com.haltroy.korean", new YorotDateAndTime[] { YorotDateAndTime.YMD });
            NewItem("KSV", YorotLocale.en_US, "com.haltroy.albanian", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("KW", YorotLocale.en_US, "com.haltroy.standard arabic", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("LV", YorotLocale.lv, "com.haltroy.latvian", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("LB", YorotLocale.ar, "com.haltroy.arabic", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("LS", YorotLocale.en_US, "com.haltroy.sotho", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("LR", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("LY", YorotLocale.ar, "com.haltroy.arabic", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("LT", YorotLocale.lt, "com.haltroy.lithuanian", new YorotDateAndTime[] { YorotDateAndTime.YMD });
            NewItem("LU", YorotLocale.fr, "com.haltroy.french", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("MG", YorotLocale.fr, "com.haltroy.french", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("MW", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("MY", YorotLocale.ms, "com.haltroy.malay", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("ML", YorotLocale.fr, "com.haltroy.french", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("MR", YorotLocale.ar, "com.haltroy.arabic", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("MX", YorotLocale.es_419, "com.haltroy.spanish", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("MD", YorotLocale.ro, "com.haltroy.romanian", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("MN", YorotLocale.en_US, "com.haltroy.mongolian", new YorotDateAndTime[] { YorotDateAndTime.YMD });
            NewItem("ME", YorotLocale.en_US, "com.haltroy.montenegrin", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("MA", YorotLocale.ar, "com.haltroy.arabic", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("MZ", YorotLocale.pt_PT, "com.haltroy.portuguese", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("MM", YorotLocale.en_US, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("NA", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("NP", YorotLocale.en_US, "com.haltroy.nepali", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.MDY, YorotDateAndTime.YMD });
            NewItem("NL", YorotLocale.nl, "com.haltroy.dutch", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("NC", YorotLocale.en_US, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("NZ", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("NI", YorotLocale.es_419, "com.haltroy.spanish", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("NE", YorotLocale.fr, "com.haltroy.french", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("NG", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.MDY });
            NewItem("NO", YorotLocale.no, "com.haltroy.norwegian", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.MDY, YorotDateAndTime.YMD });
            NewItem("OM", YorotLocale.ar, "com.haltroy.arabic", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("PK", YorotLocale.en_US, "com.haltroy.urdu", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("IL", YorotLocale.ar, "com.haltroy.arabic", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("PA", YorotLocale.es_419, "com.haltroy.spanish", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.MDY });
            NewItem("PG", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("PY", YorotLocale.es_419, "com.haltroy.spanish", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("PE", YorotLocale.en_US, "com.haltroy.peruvian spanish", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("PH", YorotLocale.fil, "com.haltroy.filipino", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.MDY });
            NewItem("PL", YorotLocale.pl, "com.haltroy.polish", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("PT", YorotLocale.pt_PT, "com.haltroy.portuguese", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("PR", YorotLocale.en_US, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.MDY });
            NewItem("QA", YorotLocale.ar, "com.haltroy.arabic", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("RO", YorotLocale.ro, "com.haltroy.romanian", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("RW", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("SA", YorotLocale.ar, "com.haltroy.arabic", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("SN", YorotLocale.fr, "com.haltroy.french", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("RS", YorotLocale.sr, "com.haltroy.serbian", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("SL", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("SK", YorotLocale.sk, "com.haltroy.slovak", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("SI", YorotLocale.en_US, "com.haltroy.slovene", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("SO", YorotLocale.ar, "com.haltroy.arabic", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("ZA", YorotLocale.en_US, "com.haltroy.afrikaans", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("ES", YorotLocale.es, "com.haltroy.spanish", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("LK", YorotLocale.en_US, "com.haltroy.sinhala", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.MDY, YorotDateAndTime.YMD });
            NewItem("SD", YorotLocale.ar, "com.haltroy.arabic", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("SS", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("SR", YorotLocale.nl, "com.haltroy.dutch", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("SE", YorotLocale.sv, "com.haltroy.swedish", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("CH", YorotLocale.fr, "com.haltroy.french", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("TW", YorotLocale.zh_TW, "com.haltroy.chinese traditional", new YorotDateAndTime[] { YorotDateAndTime.YMD });
            NewItem("TJ", YorotLocale.en_US, "com.haltroy.tajik", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("TZ", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("TH", YorotLocale.th, "com.haltroy.thai", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("TG", YorotLocale.fr, "com.haltroy.french", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.MDY });
            NewItem("TT", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("TN", YorotLocale.ar, "com.haltroy.arabic", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("TR", YorotLocale.tr, "com.haltroy.turkish", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("TM", YorotLocale.en_US, "com.haltroy.turkmen", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("UG", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("UA", YorotLocale.uk, "com.haltroy.ukrainian", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("AE", YorotLocale.ar, "com.haltroy.arabic", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("GB", YorotLocale.en_US, "com.haltroy.english_us", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("UY", YorotLocale.es_419, "com.haltroy.spanish", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("UZ", YorotLocale.en_US, "com.haltroy.uzbek", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.YMD });
            NewItem("VU", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("VN", YorotLocale.vi, "com.haltroy.vietnamese", new YorotDateAndTime[] { YorotDateAndTime.DMY, YorotDateAndTime.MDY, YorotDateAndTime.YMD });
            NewItem("YE", YorotLocale.ar, "com.haltroy.arabic", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("ZM", YorotLocale.en_GB, "com.haltroy.english", new YorotDateAndTime[] { YorotDateAndTime.DMY });
            NewItem("ZW", YorotLocale.en_US, "com.haltroy.chewa", new YorotDateAndTime[] { YorotDateAndTime.YMD });
            NewItem("RU", YorotLocale.ru, "com.haltroy.russian", new YorotDateAndTime[] { });
            NewItem("SML", YorotLocale.ar, "com.haltroy.arabic", new YorotDateAndTime[] { });
            NewItem("KKTC", YorotLocale.tr, "com.haltroy.turkish", new YorotDateAndTime[] { });
            NewItem("EH", YorotLocale.en_US, "com.haltroy.tamazight", new YorotDateAndTime[] { });
            NewItem("MK", YorotLocale.en_US, "com.haltroy.english", new YorotDateAndTime[] { });
            NewItem("FK", YorotLocale.en_US, "com.haltroy.english", new YorotDateAndTime[] { });
            NewItem("CI", YorotLocale.en_US, "com.haltroy.english", new YorotDateAndTime[] { });
            NewItem("CD", YorotLocale.fr, "com.haltroy.french", new YorotDateAndTime[] { });
            NewItem("SZ", YorotLocale.en_US, "com.haltroy.english", new YorotDateAndTime[] { });
            NewItem("SY", YorotLocale.ar, "com.haltroy.arabic", new YorotDateAndTime[] { });
            NewItem("KG", YorotLocale.en_US, "com.haltroy.kyrgyz", new YorotDateAndTime[] { });
            NewItem("SB", YorotLocale.en_US, "com.haltroy.english", new YorotDateAndTime[] { });
            NewItem("US", YorotLocale.en_US, "com.haltroy.english", new YorotDateAndTime[] { });
            NewItem("LA", YorotLocale.en_US, "com.haltroy.english", new YorotDateAndTime[] { });
            NewItem("TL", YorotLocale.en_US, "com.haltroy.english", new YorotDateAndTime[] { });
            NewItem("VE", YorotLocale.es_419, "com.haltroy.spanish", new YorotDateAndTime[] { });
            NewItem("IR", YorotLocale.fa, "com.haltroy.persian", new YorotDateAndTime[] { });
        }

        public void NewItem(string id, YorotLocale locale, string languageID, YorotDateAndTime[] dateTimeFormats)
        {
            Items.Add(new YorotLocaleMapItem(id, locale, languageID, dateTimeFormats));
        }

        public List<YorotLocaleMapItem> Items { get; set; } = new List<YorotLocaleMapItem>();

        public YorotLocaleMapItem GetMapItem(string id)
        {
            var list = Items.FindAll(it => it.ID == id);
            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }

        public class YorotLocaleMapItem
        {
            public YorotLocaleMapItem(string id, YorotLocale locale, string languageID, YorotDateAndTime[] dateTimeFormats)
            {
                ID = id ?? throw new ArgumentNullException(nameof(id));
                Locale = locale;
                LanguageID = languageID ?? throw new ArgumentNullException(nameof(languageID));
                DateTimeFormats = dateTimeFormats ?? throw new ArgumentNullException(nameof(dateTimeFormats));
            }

            public string ID { get; set; }
            public YorotLocale Locale { get; set; }
            public string LanguageID { get; set; }
            public YorotDateAndTime[] DateTimeFormats { get; set; }
        }
    }
}