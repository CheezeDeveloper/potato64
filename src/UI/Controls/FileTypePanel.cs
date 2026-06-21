using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Potato64.Core;
using Potato64.Shell;

namespace Potato64.UI.Controls {
    public class FileTypePanel : UserControl {
        private ListView _lv;
        private ShellIconCache _cache;
        public event Action<string>? ExtensionSelected;

        public FileTypePanel(ShellIconCache cache) {
            _cache = cache;
            _lv = new ListView { View = View.Details, Dock = DockStyle.Fill, FullRowSelect = true, HeaderStyle = ColumnHeaderStyle.Nonclickable };
            _lv.Columns.Add("Type", 80); _lv.Columns.Add("Size", 80); _lv.Columns.Add("Files", 60);
            _lv.SelectedIndexChanged += (s, e) => { if (_lv.SelectedItems.Count > 0) ExtensionSelected?.Invoke(_lv.SelectedItems[0].Text); };
            this.Controls.Add(_lv);
        }

        public void LoadResult(ScanResult res) {
            _lv.Items.Clear();
            var imgList = new ImageList { ImageSize = new Size(16, 16), ColorDepth = ColorDepth.Depth32Bit };
            _lv.SmallImageList = imgList;
            int i = 0;
            foreach (var ext in res.ExtensionSizes.OrderByDescending(x => x.Value)) {
                var icon = _cache.Get(ext.Key);
                if (icon != null) imgList.Images.Add(icon);
                var item = new ListViewItem(ext.Key) { ImageIndex = i++ };
                item.SubItems.Add(FormatSize(ext.Value));
                item.SubItems.Add(res.ExtensionCounts[ext.Key].ToString());
                _lv.Items.Add(item);
            }
        }
        private string FormatSize(long b) => b > 1073741824 ? $"{b/1073741824.0:F1} GB" : b > 1048576 ? $"{b/1048576.0:F1} MB" : $"{b/1024.0:F1} KB";
        public void Clear() => _lv.Items.Clear();
    }
}