namespace Potato64.Core
{
    public class FileEntry
    {
        public string FullPath   { get; init; } = "";
        public string Name       { get; init; } = "";
        public string Extension  { get; init; } = "";
        public long   SizeBytes  { get; init; }
        public DateTime LastModified { get; init; }

        public string FriendlySize => FormatSize(SizeBytes);

        private static string FormatSize(long bytes) => bytes switch
        {
            >= 1_073_741_824 => $"{bytes / 1_073_741_824.0:F1} GB",
            >= 1_048_576     => $"{bytes / 1_048_576.0:F1} MB",
            >= 1_024         => $"{bytes / 1_024.0:F1} KB",
            _                => $"{bytes} B"
        };
    }
}