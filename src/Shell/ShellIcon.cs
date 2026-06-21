using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Potato64.Shell
{
    public static class ShellIcon
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct SHFILEINFO { public IntPtr hIcon; public int iIcon; public uint dwAttr; [MarshalAs(UnmanagedType.ByValTStr, SizeConst=260)] public string szName; [MarshalAs(UnmanagedType.ByValTStr, SizeConst=80)] public string szType; }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SHGetFileInfo(string path, uint attr, ref SHFILEINFO psfi, uint cb, uint flags);

        [DllImport("user32.dll")]
        static extern bool DestroyIcon(IntPtr hIcon);

        public static Icon? GetIconForExtension(string ext)
        {
            var shfi = new SHFILEINFO();
            const uint flags = 0x100 | 0x1 | 0x10; // Icon | Small | UseAttributes
            IntPtr res = SHGetFileInfo("file" + ext, 0x80, ref shfi, (uint)Marshal.SizeOf(shfi), flags);
            if (res == IntPtr.Zero || shfi.hIcon == IntPtr.Zero) return null;
            Icon icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
            DestroyIcon(shfi.hIcon);
            return icon;
        }
    }
}