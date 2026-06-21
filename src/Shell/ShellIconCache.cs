using System.Drawing;

namespace Potato64.Shell
{
    /// <summary>
    /// Caches shell icons by extension so we only call SHGetFileInfo once per type.
    /// </summary>
    public class ShellIconCache : IDisposable
    {
        private readonly Dictionary<string, Icon?> _cache = new();
        private bool _disposed;

        public Icon? Get(string extension)
        {
            var ext = extension.ToLowerInvariant();

            if (_cache.TryGetValue(ext, out var cached))
                return cached;

            var icon = ShellIcon.GetIconForExtension(ext);
            _cache[ext] = icon;
            return icon;
        }

        public ImageList ToImageList()
        {
            var imageList = new ImageList
            {
                ImageSize  = new Size(16, 16),
                ColorDepth = ColorDepth.Depth32Bit
            };

            foreach (var (ext, icon) in _cache)
            {
                if (icon is null) continue;
                imageList.Images.Add(ext, icon.ToBitmap());
            }

            return imageList;
        }

        public void Dispose()
        {
            if (_disposed) return;
            foreach (var icon in _cache.Values)
                icon?.Dispose();
            _disposed = true;
        }
    }
}