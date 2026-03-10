using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiskMover
{
    internal class FolderFileDialog
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct BROWSEINFO
        {
            internal IntPtr hwndOwner;
            internal IntPtr pidlRoot;
            internal string pszDisplayName;
            internal string lpszTitle;
            internal int ulFlags;
            internal IntPtr lpfnCallBack;
            internal IntPtr lParam;
            internal int iImage;
        }

        // Dialog flags
        private enum BrowseFlags : int 
        {
            BIF_RETURNONLYFSDIRS = 0x0001,
            BIF_DONTGOBELOWDOMAIN = 0x0002,
            BIF_STATUSTEXT = 0x0004,
            BIF_RETURNFSANCESTORS = 0x0008,
            BIF_EDITBOX = 0x0010,
            BIF_VALIDATE = 0x0020,
            BIF_NEWDIALOGSTYLE = 0x0040,
            BIF_BROWSEFORCOMPUTER = 0x1000,
            BIF_BROWSEFORPRINTER = 0x2000,
            BIF_BROWSEINCLUDEFILES = 0x4000
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHBrowseForFolder(ref BROWSEINFO lpbi);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SHGetPathFromIDList(IntPtr pidl, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder pszPath);

        [DllImport("ole32.dll")]
        private static extern void CoTaskMemFree(IntPtr pv);

        /// <summary>
        /// Show the dialog to select a file or folder. It uses the SHBrowseForFolder API with specific flags to allow both files and folders to be selected.
        /// </summary>
        /// <param name="owner">The parent window handle</param>
        /// <param name="title">The dialog title</param>
        /// <returns>Full path if selected, empty string if invalid</returns>
        public static string ShowDialog(IWin32Window owner, string title = "Select a file or folder")
        {
            var info = new BROWSEINFO();
            info.hwndOwner = owner?.Handle ?? IntPtr.Zero; 
            info.lpszTitle = title;                         
            info.pszDisplayName = new string('\0', 256);

            // Flag config:
            // BIF_NEWDIALOGSTYLE: Use a modern dialog style.
            // BIF_EDITBOX: Show an edit box for manual path entry.
            // BIF_BROWSEINCLUDEFILES: Allow selection of files in addition to folders.
            // BIF_RETURNONLYFSDIRS: Restrict selection to filesystem items (folders and files), excluding virtual items like "This PC".
            info.ulFlags = (int)(BrowseFlags.BIF_NEWDIALOGSTYLE |
                                 BrowseFlags.BIF_EDITBOX |
                                 BrowseFlags.BIF_BROWSEINCLUDEFILES |
                                 BrowseFlags.BIF_RETURNONLYFSDIRS);

            IntPtr pidl = SHBrowseForFolder(ref info);

            // If user selected something, pidl will be a valid pointer. If user canceled, it will be IntPtr.Zero.
            if (pidl != IntPtr.Zero)
            {
                try
                {
                    // Converts the PIDL to a file system path.
                    StringBuilder path = new StringBuilder(260); // MAX_PATH = 260
                    if (SHGetPathFromIDList(pidl, path))
                    {
                        return path.ToString();
                    }
                    else
                    {
                        // If failed to get path, return empty string. This can happen if the user selects a virtual item that doesn't have a file system path.
                        return string.Empty;
                    }
                }
                finally
                {
                    // Free the PIDL memory allocated by SHBrowseForFolder to prevent memory leaks.
                    CoTaskMemFree(pidl);
                }
            }

            // Cancelled or invalid selection, return empty string.
            return string.Empty;
        }
    }
}
