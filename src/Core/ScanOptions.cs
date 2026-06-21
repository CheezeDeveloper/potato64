namespace Potato64.Core
{
    public class ScanOptions
    {
        public string RootPath            { get; set; } = @"C:\";
        public bool   IncludeHiddenFiles  { get; set; } = true;
        public bool   IncludeSystemFiles  { get; set; } = true;
        public bool   FollowSymlinks      { get; set; } = false;
        public long   MinFileSizeBytes    { get; set; } = 0;
        public List<string> ExcludePaths  { get; set; } = new();
    }
}