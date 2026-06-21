using Potato64.Core;

namespace Potato64.Export
{
    public static class TextExporter
    {
        public static void Export(ScanResult result, string outputPath)
        {
            using var writer = new StreamWriter(outputPath);

            writer.WriteLine("Potato64 Disk Analysis");
            writer.WriteLine($"Path:    {result.RootPath}");
            writer.WriteLine($"Scanned: {result.ScannedAt:yyyy-MM-dd HH:mm:ss}");
            writer.WriteLine(new string('-', 50));
            writer.WriteLine();

            writer.WriteLine("Disk Usage:");
            writer.WriteLine($"  Used:  {FormatSize(result.DriveTotal - result.DriveFree)}");
            writer.WriteLine($"  Free:  {FormatSize(result.DriveFree)}");
            writer.WriteLine($"  Total: {FormatSize(result.DriveTotal)}");
            writer.WriteLine($"  Usage: {result.UsagePercent:F1}%");
            writer.WriteLine();

            writer.WriteLine("Top File Types:");
            foreach (var (ext, bytes) in result.ExtensionSizes
                         .OrderByDescending(kv => kv.Value)
                         .Take(20))
            {
                int count   = result.ExtensionCounts.GetValueOrDefault(ext, 0);
                double pct  = result.TotalBytes > 0
                    ? (double)bytes / result.TotalBytes * 100 : 0;
                writer.WriteLine(
                    $"  {ext,-20} {FormatSize(bytes),10}  " +
                    $"({pct,5:F1}%)  {count} files"
                );
            }

            writer.WriteLine();
            writer.WriteLine("Top Folders:");
            WriteTopFolders(writer, result.RootFolder, 0, 15);
        }

        private static void WriteTopFolders(
            StreamWriter  writer,
            FolderEntry   folder,
            int           depth,
            int           maxEntries)
        {
            if (depth > 3) return;

            var sorted = folder.SubFolders
                .OrderByDescending(f => f.TotalBytes)
                .Take(maxEntries);

            foreach (var sub in sorted)
            {
                writer.WriteLine(
                    $"  {new string(' ', depth * 2)}{sub.FullPath,-45}" +
                    $"  {FormatSize(sub.TotalBytes),10}"
                );
                WriteTopFolders(writer, sub, depth + 1, 5);
            }
        }

        private static string FormatSize(long bytes) => bytes switch
        {
            >= 1_073_741_824 => $"{bytes / 1_073_741_824.0:F1} GB",
            >= 1_048_576     => $"{bytes / 1_048_576.0:F1} MB",
            >= 1_024         => $"{bytes / 1_024.0:F1} KB",
            _                => $"{bytes} B"
        };
    }
}