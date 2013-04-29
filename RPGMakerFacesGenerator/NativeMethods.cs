using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace RPGMakerFacesGenerator
{
    internal class NativeMethods
    {
        [DllImport("user32.dll")]
        public extern static IntPtr GetDesktopWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern UInt64 BitBlt
             (IntPtr hDestDC,
             int x,
             int y,
             int nWidth,
             int nHeight,
             IntPtr hSrcDC,
             int xSrc,
             int ySrc,
             System.Int32 dwRop);
    }
}
