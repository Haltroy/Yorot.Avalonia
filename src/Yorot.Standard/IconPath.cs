namespace Yorot.Standard
{
    public class IconPath
    {
        public IconPath()
        {
        }

        public IconPath(string[] path)
        {
            Path = path;
        }

        public IconPath(string path) : this(path.Split('.'))
        {
        }

        public string[] Path { get; set; }
    }
}