using System;
using System.Collections.Generic;
using System.IO;
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

        // Delegate for the callback function used by SHBrowseForFolder to set the initial directory when the dialog is initialized.
        private delegate int BrowseCallbackProc(IntPtr hwnd, uint msg, IntPtr lParam, IntPtr lpData);

        // Constants for the callback messages and parameters
        private const int BFFM_INITIALIZED = 1;
        private const int BFFM_SETSELECTION = 0x400 + 103; // WM_USER + 103


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, string lParam);

        // Callback function to set the initial directory when the dialog is initialized. It checks if the message is BFFM_INITIALIZED and then sets the selection to the initial directory provided in lpData.
        private static int BrowseCallback(IntPtr hwnd, uint uMsg, IntPtr lParam, IntPtr lpData)
        {
            
            if (uMsg == BFFM_INITIALIZED)
            {
                // Set the initial directory when the dialog is initialized
                string initialDir = Marshal.PtrToStringAuto(lpData);
                if (!string.IsNullOrEmpty(initialDir))
                {
                    SendMessage(hwnd, BFFM_SETSELECTION, 1, initialDir);
                }
            }
            return 0;
        }
        /// <summary>
        /// Show the dialog to select a file or folder. It uses the SHBrowseForFolder API with specific flags to allow both files and folders to be selected.
        /// </summary>
        /// <param name="owner">The parent window handle</param>
        /// <param name="title">The dialog title</param>
        /// <param name="initialDirectory"> Optional initial directory (can be null)</param>
        /// <returns>Full path if selected, empty string if invalid</returns>
        public static string ShowDialog(IWin32Window owner, string title = "Select a file or folder", string initialDirectory = null)
        {

            var info = new BROWSEINFO();
            info.hwndOwner = owner?.Handle ?? IntPtr.Zero; 
            info.lpszTitle = title;                         
            info.pszDisplayName = new string('\0', 256);

            info.pidlRoot = GetPidlFromSpecialFolder(Environment.SpecialFolder.MyComputer);

            // Flag config:
            // BIF_NEWDIALOGSTYLE: Use a modern dialog style.
            // BIF_EDITBOX: Show an edit box for manual path entry.
            // BIF_BROWSEINCLUDEFILES: Allow selection of files in addition to folders.
            // BIF_RETURNONLYFSDIRS: Restrict selection to filesystem items (folders and files), excluding virtual items like "This PC".
            info.ulFlags = (int)(BrowseFlags.BIF_NEWDIALOGSTYLE |
                     BrowseFlags.BIF_EDITBOX |
                     BrowseFlags.BIF_BROWSEINCLUDEFILES |
                     BrowseFlags.BIF_RETURNONLYFSDIRS |
                     BrowseFlags.BIF_BROWSEFORCOMPUTER);

            if (!string.IsNullOrEmpty(initialDirectory) && Directory.Exists(initialDirectory))
            {
                info.lpfnCallBack = Marshal.GetFunctionPointerForDelegate(
                    new BrowseCallbackProc(BrowseCallback));

                info.lParam = Marshal.StringToHGlobalAuto(initialDirectory);
            }
            IntPtr pidl = SHBrowseForFolder(ref info);

            // Free the memory allocated for the initial directory string if it was set, to prevent memory leaks.
            if (info.lParam != IntPtr.Zero)
                Marshal.FreeHGlobal(info.lParam);

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

        // Helper method to get the PIDL for a special folder like "My Computer". This allows the dialog to start at that location.
        private static IntPtr GetPidlFromSpecialFolder(Environment.SpecialFolder folder)
        {
            IntPtr pidl = IntPtr.Zero;
            int bytes = (int)SHGetFolderLocation(IntPtr.Zero, (int)folder, IntPtr.Zero, 0, out pidl);
            return pidl;
        }

        
        [DllImport("shell32.dll")]
        private static extern int SHGetFolderLocation(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwReserved, out IntPtr ppidl);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHParseDisplayName(
        [MarshalAs(UnmanagedType.LPWStr)] string pszName,
        IntPtr pbc,
        out IntPtr ppidl,
        uint sfgaoIn,
        out uint psfgaoOut);

        /// <summary>
        /// Converts a file system path to a PIDL 
        /// </summary>
        private static IntPtr GetPidlFromPath(string path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                return IntPtr.Zero;

            try
            {
                IntPtr pidl;
                uint sfgao;

                int result = SHParseDisplayName(path, IntPtr.Zero, out pidl, 0, out sfgao);

                if (result == 0)
                    return pidl;
                else
                    return IntPtr.Zero;
            }
            catch
            {
                return IntPtr.Zero;
            }
        }
    }
}
