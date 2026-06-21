namespace Potato64.Core
{
    public class ScanResult
    {
        public string      RootPath    { get; init; } = "";
        public DateTime    ScannedAt   { get; init; }
        public FolderEntry RootFolder  { get; init; } = new();
        public long        TotalBytes  { get; set; }
        public int         TotalFiles  { get; set; }
        public long        DriveTotal  { get; init; }
        public long        DriveFree   { get; init; }

        // Extension -> total bytes
        public Dictionary<string, long> ExtensionSizes { get; } = new();

        // Extension -> file count
        public Dictionary<string, int> ExtensionCounts { get; } = new();

        public double UsagePercent =>
            DriveTotal > 0 ? (double)(DriveTotal - DriveFree) / DriveTotal * 100 : 0;
    }
}