using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Potato64.Core;
using Potato64.Shell;

namespace Potato64.UI.Controls {
    public class FileListView : UserControl {
        private ListView _lv;
        private ShellIconCache _cache;
        public FileListView(ShellIconCache cache) {
            _cache = cache;
            _lv = new ListView { View = View.Details, Dock = DockStyle.Fill, FullRowSelect = true };
            _lv.Columns.Add("Name", 300); _lv.Columns.Add("Size", 100); _lv.Columns.Add("Modified", 150);
            this.Controls.Add(_lv);
        }
        public void LoadFiles(IEnumerable<FileEntry> files) {
            _lv.Items.Clear();
            var imgList = new ImageList { ImageSize = new Size(16, 16), ColorDepth = ColorDepth.Depth32Bit };
            _lv.SmallImageList = imgList;
            int i = 0;
            foreach (var f in files) {
                var icon = _cache.Get(f.Extension);
                if (icon != null) imgList.Images.Add(icon);
                var item = new ListViewItem(f.Name) { ImageIndex = i++ };
                item.SubItems.Add(f.FriendlySize);
                item.SubItems.Add(f.LastModified.ToShortDateString());
                _lv.Items.Add(item);
            }
        }
        public void Clear() => _lv.Items.Clear();
    }
}