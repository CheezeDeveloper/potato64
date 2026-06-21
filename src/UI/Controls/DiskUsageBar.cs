using System.Drawing;
using System.Windows.Forms;

namespace Potato64.UI.Controls {
    public class DiskUsageBar : Control {
        public long UsedBytes { get; set; }
        public long TotalBytes { get; set; }
        public DiskUsageBar() { DoubleBuffered = true; }
        protected override void OnPaint(PaintEventArgs e) {
            var g = e.Graphics; g.Clear(SystemColors.Control);
            int bw = Width - 40, bh = 20;
            g.DrawRectangle(Pens.Black, 20, 10, bw, bh);
            if (TotalBytes > 0) {
                float pct = (float)UsedBytes / TotalBytes;
                g.FillRectangle(Brushes.Black, 21, 11, (bw - 1) * pct, bh - 1);
                g.DrawString($"{UsedBytes/1073741824.0:F1} GB used / {TotalBytes/1073741824.0:F1} GB total ({(pct*100):F1}%)", SystemFonts.DefaultFont, Brushes.Black, 20, 35);
            }
        }
    }
}