using Microsoft.CSharp.RuntimeBinder;
using Shell32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace JsxBinParser
{
    internal static class ExplorerHelper
    {
        /// 释放命令行管理程序分配的ITEMIDLIST结构
        /// Frees an ITEMIDLIST structure allocated by the Shell.
        /// </summary>
        /// <param name="pidlList"></param>
        [DllImport("shell32.dll", ExactSpelling = true)]
        public static extern void ILFree(IntPtr pidlList);
        /// <summary>
        /// 返回与指定文件路径关联的ITEMIDLIST结构。
        /// Returns the ITEMIDLIST structure associated with a specified file path.
        /// </summary>
        /// <param name="pszPath"></param>
        /// <returns></returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern IntPtr ILCreateFromPathW(string pszPath);
        /// <summary>
        /// 打开一个Windows资源管理器窗口，其中选择了特定文件夹中的指定项目。
        /// Opens a Windows Explorer window with specified items in a particular folder selected.
        /// </summary>
        /// <param name="pidlList"></param>
        /// <param name="cild"></param>
        /// <param name="children"></param>
        /// <param name="dwFlags"></param>
        /// <returns></returns>
        [DllImport("shell32.dll", ExactSpelling = true)]
        public static extern int SHOpenFolderAndSelectItems(IntPtr pidlList, uint cild, IntPtr children, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_SHOW = 5;

        private static Shell _shell;
        private static readonly object _lockObj = new object();

        private static Shell GetShell()
        {
            if (_shell == null)
            {
                lock (_lockObj)
                {
                    _shell ??= new Shell();
                }
            }
            return _shell;
        }

        /// <summary>
        /// 打开文件夹或激活
        /// </summary>
        public static void OpenOrFocusFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                return;
            }

            string folderName = Path.GetFileName(folderPath.TrimEnd('\\'));

            IntPtr foundHwnd = IntPtr.Zero;
            EnumWindows((hWnd, lParam) =>
            {
                // 只查找类名为 CabinetWClass 或 ExploreWClass 的资源管理器
                StringBuilder className = new StringBuilder(256);
                GetClassName(hWnd, className, className.Capacity);
                string cls = className.ToString();
                if (cls != "CabinetWClass" && cls != "ExploreWClass")
                {
                    return true;
                }

                StringBuilder title = new StringBuilder(1024);
                GetWindowText(hWnd, title, title.Capacity);
                string windowTitle = title.ToString().Trim();

                if (string.Equals(windowTitle, folderName, StringComparison.OrdinalIgnoreCase))
                {
                    foundHwnd = hWnd;
                    return false; // 停止枚举
                }
                return true;
            }, IntPtr.Zero);

            if (foundHwnd != IntPtr.Zero)
            {
                ShowWindow(foundHwnd, SW_SHOW);
                SetForegroundWindow(foundHwnd);

                return;
            }

            try
            {
                string targetUrl = new Uri(folderPath).AbsoluteUri;
                Shell shell = GetShell(); // 使用缓存的 Shell 对象，二次调用极快

                foreach (dynamic window in shell.Windows())
                {
                    try
                    {
                        string currentUrl = window.LocationURL;
                        if (!string.IsNullOrEmpty(currentUrl) &&
                            currentUrl.Equals(targetUrl, StringComparison.OrdinalIgnoreCase))
                        {
                            IntPtr hWnd = (IntPtr)window.HWND;
                            ShowWindow(hWnd, SW_SHOW);
                            SetForegroundWindow(hWnd);
                            return;
                        }
                    }
                    catch (RuntimeBinderException) {}
                }
            }
            catch (Exception)
            {

            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = folderPath,
                UseShellExecute = true
            };
            Process.Start(startInfo);
        }

        public static void WarmUp()
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    var shell = GetShell();
                    var _ = shell.Windows();
                }
                catch
                {

                }
            });

            //var shell = GetShell();
            //var _ = shell.Windows();
        }

        public static void OpenFolderAndSelectFile(string fileFullName)
        {
            fileFullName = Path.GetFullPath(fileFullName);
            var pidlList = ILCreateFromPathW(fileFullName);

            if (pidlList == IntPtr.Zero)
            {
                return;
            }

            try
            {
                Marshal.ThrowExceptionForHR(SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0));
            }
            finally
            {
                ILFree(pidlList);
            }
        }
    }
}
