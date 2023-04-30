using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System.Reactive.Subjects;
using System.Xml;
using Yorot;

namespace Yorot_Avalonia.Views
{
    public partial class FavoriteWindow : Window
    {
        private TreeView? TreeView;
        private TextBox? tbUrl;
        private TextBox? tbText;
        private System.Reactive.Subjects.Subject<bool>? EnableUrl;

        public FavoriteWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            EnableUrl = new();
            EnableUrl.OnNext(false);

            var mainGrid = this.FindControl<Grid>("Content");

            var textGrid = mainGrid.FindControl<Grid>("TextUrl");

            textGrid.FindControl<TextBlock>("UrlText").Bind(IsEnabledProperty, EnableUrl);

            tbUrl = textGrid.FindControl<TextBox>("Url");
            tbText = textGrid.FindControl<TextBox>("Text");

            tbUrl.PropertyChanged += TbUrl_PropertyChanged;
            tbText.PropertyChanged += TbText_PropertyChanged;

            tbUrl.Bind(IsEnabledProperty, EnableUrl);

            TreeView = mainGrid.FindControl<TreeView>("Favorites");

            for (int i = 0; i < YorotGlobal.Main.CurrentSettings.FavManager.Favorites.Count; i++)
            {
                var fav = YorotGlobal.Main.CurrentSettings.FavManager.Favorites[i];
                RefreshFavorites(fav);
            }
        }

        private void RefreshFavorites(YorotFavFolder favFolder, TreeViewItem? parent = null)
        {
            TreeViewItem item = new()
            {
                Header = favFolder.Text,
                Tag = favFolder
            };
            if (favFolder is not YorotFavorite)
            {
                for (int i = 0; i < favFolder.Favorites.Count; i++)
                {
                    RefreshFavorites(favFolder.Favorites[i], item);
                }
            }
            if (parent != null && parent.Items is AvaloniaList<object> parentlist)
            {
                parentlist.Add(item);
            }
            else if (TreeView != null && TreeView.Items is AvaloniaList<object> treelist)
            {
                treelist.Add(item);
            }
        }

        private void TbText_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (EnableUrl != null && tbText != null && TreeView != null && TreeView.SelectedItem is TreeViewItem item && item.Tag is YorotFavFolder fav && e.Property == TextBox.TextProperty)
            {
                fav.Text = tbText.Text;
                item.Header = tbText.Text;
            }
        }

        private void TbUrl_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (EnableUrl != null && tbUrl != null && TreeView != null && TreeView.SelectedItem is TreeViewItem item && item.Tag is YorotFavorite fav && e.Property == TextBox.TextProperty)
            {
                fav.Url = tbUrl.Text;
            }
        }

        private void SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (EnableUrl != null && tbUrl != null && tbText != null && TreeView != null && TreeView.SelectedItem is TreeViewItem item)
            {
                switch (item.Tag)
                {
                    case YorotFavorite _:
                        EnableUrl.OnNext(true);
                        tbUrl.Text = (item.Tag as YorotFavorite).Url;
                        break;

                    case YorotFavFolder _:
                        EnableUrl.OnNext(false);
                        break;
                }
                tbText.Text = (item.Tag as YorotFavFolder).Text;
            }
        }

        private System.Collections.Generic.List<bool> ClipboardCopy { get; set; } = new();
        private System.Collections.Generic.List<TreeViewItem> Clipboard { get; set; } = new();

        private void Cut(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (TreeView != null && TreeView.SelectedItem is TreeViewItem item)
            {
                Clipboard.Add(item);
                ClipboardCopy.Add(false);
                if (item.Parent is TreeViewItem parent && parent.Items is AvaloniaList<object> list)
                {
                    list.Remove(item);
                }
                else if (TreeView != null && TreeView.Items is AvaloniaList<object> treelist)
                {
                    treelist.Remove(item);
                }
            }
        }

        private void Copy(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (TreeView != null && TreeView.SelectedItem is TreeViewItem item)
            {
                Clipboard.Add(item);
                ClipboardCopy.Add(true);
            }
        }

        private void Paste(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (TreeView != null && TreeView.SelectedItem is TreeViewItem item)
            {
                var pasteItem = Clipboard[Clipboard.Count - 1];
                if (item.Tag is YorotFavorite fav)
                {
                    if (item.Parent is TreeViewItem parent && parent.Items is AvaloniaList<object> parentlist)
                    {
                        DoPaste(pasteItem, parentlist);
                    }
                    else if (TreeView != null && TreeView.Items is AvaloniaList<object> treelist)
                    {
                        DoPaste(pasteItem, treelist);
                    }
                }
                else if (item.Tag is YorotFavFolder folder && item.Items is AvaloniaList<object> itemlist)
                {
                    DoPaste(pasteItem, itemlist);
                }
            }
        }

        private void DoPaste(TreeViewItem pasteItem, AvaloniaList<object> itemlist)
        {
            if (ClipboardCopy[ClipboardCopy.Count - 1])
            {
                var clone = CloneItem(pasteItem);
                itemlist.Add(clone);
                if (pasteItem.Parent is TreeViewItem parent && parent.Tag is YorotFavFolder fav)
                {
                    fav.Favorites.Add(clone.Tag as YorotFavFolder);
                }
            }
            else
            {
                itemlist.Add(pasteItem);
                Clipboard.Remove(pasteItem);
                ClipboardCopy.RemoveAt(Clipboard.Count - 1);
            }
        }

        private TreeViewItem CloneItem(TreeViewItem item)
        {
            if (YorotGlobal.Main is null) { throw new System.Exception("YorotMain is not initialized."); }
            TreeViewItem clone = new()
            {
                Name = item.Name,
                Header = item.Header,
            };
            XmlDocument doc = new XmlDocument();
            if (item.Tag is YorotFavorite fav)
            {
                doc.LoadXml(fav.ToXml());
                clone.Tag = new YorotFavorite(YorotGlobal.Main.CurrentSettings.FavManager, doc.DocumentElement);
            }
            else if (item.Tag is YorotFavFolder folder)
            {
                doc.LoadXml(folder.ToXml());
                var clonefolder = new YorotFavFolder(YorotGlobal.Main.CurrentSettings.FavManager, doc.DocumentElement);
                clone.Tag = clonefolder;
                RefreshFavorites(clonefolder, clone);
            }
            return clone;
        }

        private void Add(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tbUrl != null && tbText != null && TreeView != null && TreeView.SelectedItem is TreeViewItem item)
            {
                XmlDocument doc = new XmlDocument();
                var randomText = HTAlt.Tools.GenerateRandomText();
                doc.LoadXml($"<Favorite Name=\"{randomText}\" Text=\"{tbText.Text}\" Url=\"{tbUrl.Text}\" />");
                TreeViewItem newItem = new()
                {
                    Name = randomText,
                    Header = tbText.Text,
                    Tag = new YorotFavorite(YorotGlobal.Main.CurrentSettings.FavManager, doc.DocumentElement),
                };
                if (item.Parent is TreeViewItem parent && parent.Items is AvaloniaList<object> list)
                {
                    list.Add(newItem);
                }
                else if (TreeView != null && TreeView.Items is AvaloniaList<object> treelist)
                {
                    treelist.Add(newItem);
                }
            }
        }

        private void Remove(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (TreeView != null && TreeView.SelectedItem is TreeViewItem item)
            {
                if (item.Parent is TreeViewItem parent && parent.Items is AvaloniaList<object> parentlist)
                {
                    parentlist.Remove(item);
                }
                else if (TreeView != null && TreeView.Items is AvaloniaList<object> treelist)
                {
                    treelist.Remove(item);
                }
            }
        }

        private void NewFolder(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (tbText != null && TreeView != null && TreeView.SelectedItem is TreeViewItem item && item.Parent is TreeViewItem parent && parent.Items is AvaloniaList<object> list)
            {
                XmlDocument doc = new XmlDocument();
                var randomText = HTAlt.Tools.GenerateRandomText();
                doc.LoadXml($"<Favorite Name=\"{randomText}\" Text=\"{tbText.Text}\" />");
                TreeViewItem newItem = new()
                {
                    Name = randomText,
                    Header = tbText.Text,
                    Tag = new YorotFavFolder(YorotGlobal.Main.CurrentSettings.FavManager, doc.DocumentElement),
                };
                list.Add(newItem);
            }
        }
    }
}