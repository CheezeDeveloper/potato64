using System.IO;

namespace Potato64.Core
{
    public class Scanner
    {
        private readonly ScanOptions _options;

        public event Action<string>? ProgressChanged;
        public event Action<int>?    FileCountChanged;

        public Scanner(ScanOptions options)
        {
            _options = options;
        }

        public Task<ScanResult> ScanAsync(CancellationToken token = default)
        {
            return Task.Run(() => Scan(token), token);
        }

        private ScanResult Scan(CancellationToken token)
        {
            var driveInfo = new DriveInfo(
                Path.GetPathRoot(_options.RootPath) ?? _options.RootPath
            );

            var result = new ScanResult
            {
                RootPath   = _options.RootPath,
                ScannedAt  = DateTime.Now,
                DriveTotal = driveInfo.TotalSize,
                DriveFree  = driveInfo.AvailableFreeSpace,
                RootFolder = new FolderEntry
                {
                    FullPath = _options.RootPath,
                    Name     = _options.RootPath
                }
            };

            ScanFolder(result.RootFolder, result, token);

            return result;
        }

        private void ScanFolder(
            FolderEntry   folder,
            ScanResult    result,
            CancellationToken token)
        {
            // Check if user hit "Cancel" in the UI
            if (token.IsCancellationRequested) return;
            
            if (_options.ExcludePaths.Contains(folder.FullPath)) return;

            ProgressChanged?.Invoke(folder.FullPath);

            try
            {
                var fileInfos = Directory
                    .EnumerateFiles(folder.FullPath)
                    .Select(f => new FileInfo(f));

                foreach (var fi in fileInfos)
                {
                    if (token.IsCancellationRequested) return;
                    if (!ShouldInclude(fi)) continue;
                    if (fi.Length < _options.MinFileSizeBytes) continue;

                    var entry = new FileEntry
                    {
                        FullPath     = fi.FullName,
                        Name         = fi.Name,
                        Extension    = fi.Extension.ToLowerInvariant(),
                        SizeBytes    = fi.Length,
                        LastModified = fi.LastWriteTime
                    };

                    folder.Files.Add(entry);
                    folder.TotalBytes += fi.Length;
                    folder.FileCount++;

                    result.TotalBytes += fi.Length;
                    result.TotalFiles++;

                    var ext = string.IsNullOrEmpty(entry.Extension)
                        ? "(no extension)"
                        : entry.Extension;

                    result.ExtensionSizes.TryGetValue(ext, out long existing);
                    result.ExtensionSizes[ext] = existing + fi.Length;

                    result.ExtensionCounts.TryGetValue(ext, out int count);
                    result.ExtensionCounts[ext] = count + 1;

                    FileCountChanged?.Invoke(result.TotalFiles);
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (IOException)                 { }

            try
            {
                foreach (var dir in Directory.EnumerateDirectories(folder.FullPath))
                {
                    if (token.IsCancellationRequested) return;

                    var di = new DirectoryInfo(dir);

                    if (!_options.FollowSymlinks &&
                        di.Attributes.HasFlag(FileAttributes.ReparsePoint))
                        continue;

                    var sub = new FolderEntry
                    {
                        FullPath = di.FullName,
                        Name     = di.Name
                    };

                    folder.SubFolders.Add(sub);
                    ScanFolder(sub, result, token);

                    folder.TotalBytes += sub.TotalBytes;
                    folder.FileCount  += sub.FileCount;
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (IOException)                 { }
        }

        private bool ShouldInclude(FileInfo fi)
        {
            if (!_options.IncludeHiddenFiles &&
                fi.Attributes.HasFlag(FileAttributes.Hidden))
                return false;

            if (!_options.IncludeSystemFiles &&
                fi.Attributes.HasFlag(FileAttributes.System))
                return false;

            return true;
        }
    }
}