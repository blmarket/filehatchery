using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;

namespace ShellApi
{
    /// <summary>
    /// 윈도우 Shell에서 File의 정보를 가져오기 위한 구조체의 형식입니다.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO
    {
        public IntPtr hIcon;
        public IntPtr iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SHFILEOPSTRUCT
    {
        public IntPtr hwnd;   
        // Window handle to the dialog box to display 
        // information about the status of the file 
        // operation. 

        public UInt32 wFunc;   
        // Value that indicates which operation to 
        // perform.

        public IntPtr pFrom;   
        // Address of a buffer to specify one or more 
        // source file names. These names must be 
        // fully qualified paths. Standard MicrosoftÂ®   
        // MS-DOSÂ® wild cards, such as "*", are 
        // permitted in the file-name position. 
        // Although this member is declared as a 
        // null-terminated string, it is used as a 
        // buffer to hold multiple file names. Each 
        // file name must be terminated by a single 
        // NULL character. An additional NULL 
        // character must be appended to the end of 
        // the final name to indicate the end of pFrom. 

        public IntPtr pTo;   
        // Address of a buffer to contain the name of 
        // the destination file or directory. This 
        // parameter must be set to NULL if it is not 
        // used. Like pFrom, the pTo member is also a 
        // double-null terminated string and is handled 
        // in much the same way. 

        public UInt16 fFlags;   
        // Flags that control the file operation. 

        public Int32 fAnyOperationsAborted;
        // Value that receives TRUE if the user aborted 
        // any file operations before they were 
        // completed, or FALSE otherwise. 

        public IntPtr hNameMappings;
        // A handle to a name mapping object containing 
        // the old and new names of the renamed files. 
        // This member is used only if the 
        // fFlags member includes the 
        // FOF_WANTMAPPINGHANDLE flag.

        [MarshalAs(UnmanagedType.LPWStr)]
        public String lpszProgressTitle;
        // Address of a string to use as the title of 
        // a progress dialog box. This member is used 
        // only if fFlags includes the 
        // FOF_SIMPLEPROGRESS flag.
    }

    public class Win32
    {
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0;    // 'Large icon
        public const uint SHGFI_SMALLICON = 0x1;    // 'Small icon

        [DllImport("shell32.dll", CharSet=CharSet.Unicode)]
        public static extern IntPtr SHGetFileInfo(string pszPath,
                                    uint dwFileAttributes,
                                    ref SHFILEINFO psfi,
                                    uint cbSizeFileInfo,
                                    uint uFlags);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern Int32 SHFileOperation(
            ref SHFILEOPSTRUCT lpFileOp);       // Address of an SHFILEOPSTRUCT 

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int DestroyIcon(IntPtr hIcon);

        public static System.Drawing.Icon getIcon(string fullPath)
        {
            SHFILEINFO shInfo = new SHFILEINFO();
            Win32.SHGetFileInfo(fullPath, 0, ref shInfo, (uint)Marshal.SizeOf(shInfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);
            if (shInfo.hIcon == (IntPtr)0)
                return null;
            Icon ret = (Icon)Icon.FromHandle(shInfo.hIcon).Clone();
            Win32.DestroyIcon(shInfo.hIcon);
            return ret;
        }

        public static void SHExecute(FileInfo file, string argument, bool asAdmin)
        {
            SHExecute(file.FullName, argument, asAdmin);
        }

        public static void SHExecute(FileInfo file, bool asAdmin)
        {
            SHExecute(file, "", asAdmin);
        }

        public static void SHExecute(string filename, string argument, bool asAdmin)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            if (asAdmin)
            {
                proc.StartInfo.Verb = "runas";
            }
            proc.StartInfo.FileName = filename;
            proc.StartInfo.Arguments = argument;
            proc.StartInfo.UseShellExecute = true;
            try
            {
                proc.Start();
            }
            catch
            {
                throw;
            }
        }

        String StringArrayToMultiString(String[] stringArray)
        {
            String multiString = "";

            if (stringArray == null)
                return "";

            for (int i = 0; i < stringArray.Length; i++)
                multiString += stringArray[i] + '\0';

            multiString += '\0';

            return multiString;
        }
    }
}

