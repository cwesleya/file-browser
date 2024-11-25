namespace FileBrowser.Models
{
    public class DirectoryItem
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public DirectoryItem(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}