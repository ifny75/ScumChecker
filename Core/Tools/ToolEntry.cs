namespace ScumChecker.Core.Tools
{
    public sealed class ToolEntry
    {
        public string Name { get; set; } = "";
        public string Status { get; set; } = "Not found";
        public string Path { get; set; } = "";
        public string DownloadUrl { get; set; } = "";
    }
}
