using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace BrowserBookmarkManager
{
    public static class BookmarkManagerSettings
    {
        /// <summary>
        /// Throws exceptions if an item is not found.
        /// </summary>
        public static bool ThrowExceptionOnNotFound { get; set; }
    }

    public static class Tools
    {
        public static XmlNode JSONToXml(string json)
        {
            XmlDocument doc = new XmlDocument();
            if (!string.IsNullOrWhiteSpace(json))
            {
                using (var xmlReader = XDocument.Load(JsonReaderWriterFactory.CreateJsonReader(
        Encoding.ASCII.GetBytes(json), new XmlDictionaryReaderQuotas())).CreateReader())
                {
                    doc.Load(xmlReader);
                }
            }
            return doc.DocumentElement;
        }
    }

    // TODO: Need testing on other OSes, most stuff is janky because we are reading files that we don't have control on.
    public static class Loader
    {
        public static BookmarkRoot LoadChromiumBookmarks(string json, BookmarkBrowser browser = BookmarkBrowser.Chromium)
        {
            var doc = Tools.JSONToXml(json);

            BookmarkRoot root = new BookmarkRoot();

            if (doc is null || doc.ChildNodes.Count <= 0)
            {
                return root;
            }

            for (int rooti = 0; rooti < doc.ChildNodes.Count; rooti++)
            {
                var roots = doc.ChildNodes[rooti];
                if (roots.Name.ToLower() == "roots")
                {
                    for (int topleveli = 0; topleveli < roots.ChildNodes.Count; topleveli++)
                    {
                        var toplevel = roots.ChildNodes[topleveli];
                        Bookmark toplevelb = new Bookmark();
                        toplevelb.Browser = browser;
                        for (int i = 0; i < toplevel.ChildNodes.Count; i++)
                        {
                            var node = toplevel.ChildNodes[i];
                            switch (node.Name.ToLower())
                            {
                                case "children":
                                    toplevelb.Data.Add(new BookmarkData("FolderContent", ReadChildren(node, browser)));
                                    break;

                                default:
                                    toplevelb.Data.Add(new BookmarkData(node.Name.ToLower(), node.InnerXml));
                                    break;
                            }
                        }
                        root.Bookmarks.Add(toplevelb);
                    }
                }
            }
            return root;
        }

        private static List<Bookmark> ReadChildren(XmlNode parent, BookmarkBrowser browser = BookmarkBrowser.Chromium)
        {
            var list = new List<Bookmark>();
            for (int i = 0; i < parent.ChildNodes.Count; i++)
            {
                var node = parent.ChildNodes[i];
                Bookmark bookmark = new Bookmark();
                bookmark.Browser = browser;
                for (int featurei = 0; featurei < node.ChildNodes.Count; featurei++)
                {
                    switch (node.ChildNodes[featurei].Name.ToLower())
                    {
                        case "children":
                            bookmark.Data.Add(new BookmarkData("Children", ReadChildren(node.ChildNodes[featurei])));
                            break;

                        default:
                            bookmark.Data.Add(new BookmarkData(node.ChildNodes[featurei].Name.ToLower(), node.ChildNodes[featurei].InnerXml));
                            break;
                    }
                }
                list.Add(bookmark);
            }
            return list;
        }

        public static BookmarkRoot LoadChromiumBookmarksFromFile(string file, BookmarkBrowser browser = BookmarkBrowser.Chromium)
        {
            using (var reader = new StreamReader(new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite)))
            {
                return LoadChromiumBookmarks(reader.ReadToEnd(), browser);
            }
        }

        public static BookmarkRoot LoadChromiumBookmarksFromData(string folder, BookmarkBrowser browser = BookmarkBrowser.Chromium)
        {
            BookmarkRoot mainRoot = new BookmarkRoot();
            var profiles = Directory.GetDirectories(folder);
            for (int i = 0; i < profiles.Length; i++)
            {
                if (profiles[i].Contains("Profile") || profiles[i].Contains("Default"))
                {
                    string bookmarkFile = Path.Combine(profiles[i], "Bookmarks");
                    mainRoot = mainRoot.Combine(LoadChromiumBookmarksFromFile(bookmarkFile, browser));
                }
            }
            return mainRoot;
        }

        public static BookmarkRoot LoadEdgeBookmarks()
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Microsoft",
                "Edge",
                "User Data");
            return LoadChromiumBookmarksFromData(path, BookmarkBrowser.Edge);
        }

        public static BookmarkRoot LoadChromeBookmarks()
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Google",
                "Chrome",
                "User Data");
            return LoadChromiumBookmarksFromData(path, BookmarkBrowser.Chrome);
        }

        public static BookmarkRoot LoadOperaBookmarks()
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Opera Software",
                "Opera Stable",
                "Bookmarks");
            return LoadChromiumBookmarksFromFile(path, BookmarkBrowser.Opera);
        }

        public static BookmarkRoot LoadOperaGXBookmarks()
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Opera Software",
                "Opera GX Stable",
                "Bookmarks");
            return LoadChromiumBookmarksFromFile(path, BookmarkBrowser.OperaGX);
        }

        public static BookmarkRoot LoadFromHTML(string html)
        {
            XmlDocument doc = new XmlDocument();

            // We need to play with the html file so it looks like an XML file.
            // RegEx is suited better for this, also we have to use multiple regex for multiple stuff
            // why tf is this thing so broken?

            // Replace the DOCTYPE thing on top with a XML header and root node head
            html = Regex.Replace(html, "\\<\\!DOCTYPE\\s.*", "<?xml version=\"1.0\" encoding=\"utf-8\"?><root>") + "</root>";

            // Remove the META thing, we don't need it
            html = Regex.Replace(html, "\\<(meta|META).*((\\>)|((\\n)?.*meta\\>))", "");

            // Replaces & with &amp; so XML won't die. Somehow HTML says it's ok to use it so most just put & instead of &amp;.
            html = html.Replace("&", "&amp;");

            // Some browsers don't put closing tags, we also don't need them too.
            //html = Regex.Replace(html, "\\<(\\/)?D(L|T)\\>", "");
            html = html.Replace("<DT>", "<DT />");

            // Remove empty stuff that also don't have a closing tag.
            html = Regex.Replace(html, "\\<[a-z]\\>$", "", (RegexOptions.Multiline | RegexOptions.IgnoreCase));

            doc.LoadXml(html);
            var docroot = doc.DocumentElement;

            BookmarkRoot root = new BookmarkRoot();
            List<string> LastName = new List<string>();

            for (int i = 0; i < docroot.ChildNodes.Count; i++)
            {
                var node = docroot.ChildNodes[i];
                switch (node.Name.ToLowerInvariant())
                {
                    case "h1":
                    case "h2":
                    case "h3":
                    case "h4":
                    case "h5":
                    case "h6":
                        LastName.Add(node.InnerXml);
                        break;

                    // ignored stuff
                    case "dt":
                    case "title":
                        break;

                    case "dl":
                        Bookmark folder = new Bookmark() { Browser = BookmarkBrowser.HTML };
                        folder.Data.Add(new BookmarkData("Name", LastName.Last()));
                        folder.Data.Add(new BookmarkData("type", "folder"));
                        var _folder = new BookmarkData("FolderContent", new List<Bookmark>());
                        folder.Data.Add(_folder);
                        HTMLAddChildren(LastName, _folder.Value as List<Bookmark>, root, node);
                        root.Bookmarks.Add(folder);
                        break;

                    default:
                        if (node.OuterXml.StartsWith("<!--"))
                        {
                            break;
                        }
                        Bookmark bookmark = new Bookmark() { Browser = BookmarkBrowser.HTML };
                        for (int attr_i = 0; attr_i < node.Attributes.Count; attr_i++)
                        {
                            var attr = node.Attributes[attr_i];
                            bookmark.Data.Add(new BookmarkData(attr.Name, attr.Value));
                        }
                        bookmark.Data.Add(new BookmarkData("Name", node.InnerXml));
                        root.Bookmarks.Add(bookmark);
                        break;
                }
            }
            return root;
        }

        private static void HTMLAddChildren(List<string> LastNames, List<Bookmark> folder, BookmarkRoot root, XmlNode rootnode)
        {
            for (int i = 0; i < rootnode.ChildNodes.Count; i++)
            {
                var node = rootnode.ChildNodes[i];
                switch (node.Name.ToLowerInvariant())
                {
                    case "h1":
                    case "h2":
                    case "h3":
                    case "h4":
                    case "h5":
                    case "h6":
                        LastNames.Add(node.InnerXml);
                        break;

                    // ignored stuff
                    case "dt":
                    case "title":
                        break;

                    case "dl":
                        Bookmark nfolder = new Bookmark() { Browser = BookmarkBrowser.HTML };
                        nfolder.Data.Add(new BookmarkData("Name", LastNames.Last()));
                        nfolder.Data.Add(new BookmarkData("type", "folder"));
                        var _folder = new BookmarkData("FolderContent", new List<Bookmark>());
                        nfolder.Data.Add(_folder);
                        HTMLAddChildren(LastNames, _folder.Value as List<Bookmark>, root, node);
                        folder.Add(nfolder);
                        break;

                    default:
                        if (node.OuterXml.StartsWith("<!--"))
                        {
                            break;
                        }
                        Bookmark bookmark = new Bookmark() { Browser = BookmarkBrowser.HTML };
                        for (int attr_i = 0; attr_i < node.Attributes.Count; attr_i++)
                        {
                            var attr = node.Attributes[attr_i];
                            bookmark.Data.Add(new BookmarkData(attr.Name, attr.Value));
                        }
                        bookmark.Data.Add(new BookmarkData("Name", node.InnerXml));
                        folder.Add(bookmark);
                        break;
                }
            }
            LastNames.Remove(LastNames.Last());
        }

        public static BookmarkRoot LoadFromHTMLFile(string file)
        {
            using (var reader = new StreamReader(new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite)))
            {
                return LoadFromHTML(reader.ReadToEnd());
            }
        }

        public static BookmarkRoot LoadYorotBookmarks(string path)
        {
            // One up if someone selects the user folder instead of root folder
            if (!System.IO.Directory.Exists(Path.Combine(path, "user")))
            {
                path = new DirectoryInfo(path).Parent.FullName;
            }

            // One up again if someone selects a specific user folder instead of root folder
            if (!System.IO.Directory.Exists(Path.Combine(path, "user")))
            {
                path = new DirectoryInfo(path).Parent.FullName;
            }

            BookmarkRoot root = new BookmarkRoot();

            var profiles = System.IO.Directory.GetDirectories(Path.Combine(path, "user"));
            for (int i = 0; i < profiles.Length; i++)
            {
                var profileName = profiles[i].Replace(path, "");
                var favoritesFile = Path.Combine(profiles[i], "favorites.ycf");

                root = root.Combine(LoadSpecificYorotBookmarks(favoritesFile));
            }

            return root;
        }

        public static BookmarkRoot LoadSpecificYorotBookmarks(string file)
        {
            BookmarkRoot root = new BookmarkRoot();

            string xmlfile = "";

            using (var reader = new StreamReader(new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite)))
            {
                xmlfile = reader.ReadToEnd();
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlfile);

            var rootNode = doc.DocumentElement;

            for (int ı = 0; ı < rootNode.ChildNodes.Count; ı++)
            {
                XmlNode node = rootNode.ChildNodes[ı];
                switch (node.Name.ToLowerInvariant())
                {
                    case "favorites":
                        for (int i = 0; i < node.ChildNodes.Count; i++)
                        {
                            XmlNode subnode = node.ChildNodes[i];
                            switch (subnode.Name.ToLowerInvariant())
                            {
                                case "favorite":
                                    root.Bookmarks.Add(LoadYorotBookmark(subnode, false));
                                    break;

                                case "folder":
                                    root.Bookmarks.Add(LoadYorotBookmark(subnode, true));
                                    break;
                            }
                        }
                        break;
                }
            }

            return root;
        }

        private static Bookmark LoadYorotBookmark(XmlNode node, bool IsFolder)
        {
            Bookmark bookmark = new Bookmark();
            // NAME
            if (node.Attributes["Name"] != null)
            {
                bookmark.Data.Add(new BookmarkData("id", node.Attributes["Name"].Value));
            }
            // TEXT
            if (node.Attributes["Text"] != null)
            {
                bookmark.Data.Add(new BookmarkData("name", node.Attributes["Text"].Value));
            }
            else
            {
                bookmark.Data.Add(new BookmarkData("name", ""));
            }
            // ICON
            if (node.Attributes["Icon"] != null)
            {
                bookmark.Data.Add(new BookmarkData("icon", node.Attributes["Icon"].Value));
            }

            if (IsFolder)
            {
                List<Bookmark> content = new List<Bookmark>();
                bookmark.Data.Add(new BookmarkData("type", "folder"));
                bookmark.Data.Add(new BookmarkData("FolderContent", content));

                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    XmlNode subnode = node.ChildNodes[i];
                    switch (subnode.Name.ToLowerInvariant())
                    {
                        case "favorite":
                            content.Add(LoadYorotBookmark(subnode, false));
                            break;

                        case "folder":
                            content.Add(LoadYorotBookmark(subnode, true));
                            break;
                    }
                }
            }
            else
            {
                if (node.Attributes["Uext"] != null)
                {
                    bookmark.Data.Add(new BookmarkData("url", node.Attributes["Uext"].Value));
                }
                else
                {
                    bookmark.Data.Add(new BookmarkData("url", ""));
                }
                bookmark.Data.Add(new BookmarkData("type", "favorite"));
            }
            return bookmark;
        }
    }

    public class BookmarkRoot
    {
        public List<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();

        public BookmarkRoot Combine(BookmarkRoot root)
        {
            Bookmarks.AddRange(root.Bookmarks.ToArray());
            return this;
        }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string data, string container) : base($"Cannot find \"{data}\" in \"{container}\".")
        {
        }
    }

    public class Bookmark
    {
        public List<BookmarkData> Data { get; set; } = new List<BookmarkData>();

        public BookmarkBrowser Browser { get; set; } = BookmarkBrowser.Other;

        public string Name
        {
            get
            {
                var names = Data.FindAll(it => it.Name.ToLower() == "name");
                if (names.Count > 0)
                {
                    var val = names[0].Value;
                    if (val is string res)
                    {
                        return res;
                    }
                    else
                    {
                        throw new InvalidCastException($"Cannot cast \"{val.GetType().FullName}\" into a string.");
                    }
                }
                else
                {
                    if (BookmarkManagerSettings.ThrowExceptionOnNotFound)
                    {
                        throw new NotFoundException("name", nameof(Data));
                    }
                    return null;
                }
            }
        }

        public bool? IsFolder
        {
            get
            {
                var names = Data.FindAll(it => it.Name.ToLower() == "type");
                if (names.Count > 0)
                {
                    var val = names[0].Value;
                    if (val is string str)
                    {
                        return str.ToLower() == "folder";
                    }
                    else
                    {
                        throw new InvalidCastException($"Cannot cast \"{val.GetType().FullName}\" into a boolean.");
                    }
                }
                else
                {
                    if (BookmarkManagerSettings.ThrowExceptionOnNotFound)
                    {
                        throw new NotFoundException("type", nameof(Data));
                    }
                    return null;
                }
            }
        }

        public string Url
        {
            get
            {
                var names = Data.FindAll(it => it.Name.ToLower() == "url" || it.Name.ToLower() == "href");
                if (names.Count > 0)
                {
                    var val = names[0].Value;
                    if (val is string res)
                    {
                        return res;
                    }
                    else
                    {
                        throw new InvalidCastException($"Cannot cast \"{val.GetType().FullName}\" into a string.");
                    }
                }
                else
                {
                    if (BookmarkManagerSettings.ThrowExceptionOnNotFound)
                    {
                        throw new NotFoundException("url/address", nameof(Data));
                    }
                    return null;
                }
            }
        }

        public List<Bookmark> Bookmarks
        {
            get
            {
                var names = Data.FindAll(it => it.Name == "FolderContent");
                if (names.Count > 0)
                {
                    var val = names[0].Value;
                    if (val is List<Bookmark> res)
                    {
                        return res;
                    }
                    else
                    {
                        throw new InvalidCastException($"Cannot cast \"{val.GetType().FullName}\" into a list of bookmarks.");
                    }
                }
                else
                {
                    if (BookmarkManagerSettings.ThrowExceptionOnNotFound)
                    {
                        throw new NotFoundException("FolderContent", nameof(Data));
                    }
                    return null;
                }
            }
        }
    }

    public class BookmarkData
    {
        public BookmarkData(string name, object value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Name { get; set; }

        public object Value { get; set; }
    }

    public enum BookmarkBrowser
    {
        Other,
        Chromium,
        Chrome,
        Edge,
        Opera,
        OperaGX,
        HTML,
        Yorot
    }
}