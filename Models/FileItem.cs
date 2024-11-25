namespace FileBrowser.Models
{
    public class FileItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }

        public FileItem(string name, string path, long size)
        {
            Name = name;
            Path = path;
            Size = size;
        }
    }
}