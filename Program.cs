using System;
using System.Windows.Forms;

namespace Potato64 {
    internal static class Program {
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.SystemAware); // Fixes the overlapping text
            Application.Run(new Potato64.UI.MainForm());
        }
    }
}