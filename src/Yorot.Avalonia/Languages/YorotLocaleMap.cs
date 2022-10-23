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
            // TODO: Add more, using Turkey as example here cuz ben turkuz korkmuycan sonm,ez bu safakalr
            NewItem("TR", "tr", "com.haltroy.turkish", new YorotDateAndTime[] { YorotDateAndTime.DMY });
        }

        public void NewItem(string id, string locale, string languageID, YorotDateAndTime[] dateTimeFormats)
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
            public YorotLocaleMapItem(string id, string locale, string languageID, YorotDateAndTime[] dateTimeFormats)
            {
                ID = id ?? throw new ArgumentNullException(nameof(id));
                Locale = locale ?? throw new ArgumentNullException(nameof(locale));
                LanguageID = languageID ?? throw new ArgumentNullException(nameof(languageID));
                DateTimeFormats = dateTimeFormats ?? throw new ArgumentNullException(nameof(dateTimeFormats));
            }

            public string ID { get; set; }
            public string Locale { get; set; }
            public string LanguageID { get; set; }
            public YorotDateAndTime[] DateTimeFormats { get; set; }
        }
    }
}