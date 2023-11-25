namespace Yorot.Standard
{
    public abstract class YorotApp
    {
        private string _name = string.Empty;
        public abstract string Name { get; }
        public abstract string DisplayName { get; }
        public abstract AuthorInfo Author { get; set; }
        public abstract Version Version { get; set; }
        public abstract IconPath IconPath { get; set; }
    }

    public abstract class YorotBinaryApp : YorotApp
    {
    }

    public class YorotWebApp : YorotApp
    {
    }
}