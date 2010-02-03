using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace IconExtractor
{
    class Program
    {
        [DllImport("Shell32.dll")]
        public extern static int ExtractIconEx(string libName, int iconIndex,
            IntPtr[] largeIcon, IntPtr[] smallIcon, int nIcons);

        static void Main(string[] args)
        {
            IntPtr[] LargeIcon = new IntPtr[100];
            IntPtr[] SmallIcon = new IntPtr[100];

            ExtractIconEx("xpsrchvw.exe", 0, LargeIcon, SmallIcon, 100);

            for (int i = 0; i < 100; i++)
            {
                Icon tmp = Icon.FromHandle(LargeIcon[i]);
                tmp.ToBitmap().Save("test" + i + ".bmp");
                tmp.Dispose();
            }
        }
    }
}
