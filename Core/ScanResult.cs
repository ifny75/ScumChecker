namespace ScumChecker.Core
{
    public enum Severity { Info, Low, Medium, High }

    public enum FindingGroup
    {
        SystemInfo,
        DevTools,
        Suspicious,
        HighRisk
    }

    public sealed class ScanItem
    {
        public Severity Severity { get; set; } = Severity.Info;
        public FindingGroup Group { get; set; } = FindingGroup.SystemInfo;

        public string Category { get; set; } = "General";

        // ✅ новое имя
        public string What { get; set; } = "";

        public string Reason { get; set; } = "";
        public string Recommendation { get; set; } = "";
        public string Details { get; set; } = "";

        public string? EvidencePath { get; set; }
        public string? Url { get; set; }

        // ✅ обратная совместимость со старым кодом:
        // теперь старые места типа: Title = "Suspicious filename" снова компилируются
        public string Title
        {
            get => What;
            set => What = value;
        }
    }

    public sealed class ScanResult
    {
        public List<ScanItem> Items { get; } = new();
    }
}
