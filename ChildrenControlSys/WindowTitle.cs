using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ChildrenControlSys
{
    public class WindowTitle
    {
        public delegate bool EnumDelegate(IntPtr hWnd, int lParam);


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);


        //[DllImport("user32.dll", EntryPoint = "GetWindowText",
        //ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        //public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows",
        ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpEnumCallbackFunction, IntPtr lParam);


        //*************
        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;

        /// <summary>
        /// //
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="title"></param>
        public delegate bool EnumedWindow(IntPtr handleWindow, ArrayList handles);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumedWindow lpEnumFunc, ArrayList lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);
        public static ArrayList GetWindows()
        {
            ArrayList windowHandles = new ArrayList();
            EnumedWindow callBackPtr = GetWindowHandle;
            EnumWindows(callBackPtr, windowHandles);

            return windowHandles;
        }

        private static bool GetWindowHandle(IntPtr windowHandle, ArrayList windowHandles)
        {
            // Allocate correct string length first
            int length = GetWindowTextLength(windowHandle);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(windowHandle, sb, sb.Capacity);
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                windowHandles.Add(sb.ToString());
            }
            return true;
        }
        public static void CloseWindow(string keyword, string title)
        {            
            // Retrieve the handler of the window
            int iHandle = FindWindow(title,keyword);
            if (iHandle > 0)
            {
                // Close the window using API
                SendMessage(iHandle, WM_SYSCOMMAND, SC_CLOSE, 0);
            }
        }

        public static List<string> GetProcesses()
        {
            Process[] processList = Process.GetProcesses();

            List<string> windowTitles = new List<string>();

            foreach (Process proc in processList)
            {
                StringBuilder Buff = new StringBuilder(256);

                if (GetWindowText(proc.MainWindowHandle, Buff, 256) > 0)
                {
                    windowTitles.Add(Buff.ToString());
                }
            }
            return windowTitles;
        }
        //*************
        public static List<string> GetWindowText()
        {
            var collection = new List<string>();
            var windowTitles = new List<string>();

            EnumDelegate filter = delegate (IntPtr hWnd, int lParam)
            {
                StringBuilder strbTitle = new StringBuilder(255);
                int nLength = GetWindowText(hWnd, strbTitle, strbTitle.Capacity + 1);
                string strTitle = strbTitle.ToString();

                if (IsWindowVisible(hWnd) && string.IsNullOrEmpty(strTitle) == false)
                {
                    collection.Add(strTitle);
                }
                return true;
            };

            if (EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero))
            {
                foreach (var item in collection)
                {
                    windowTitles.Add(item);
                }
            }
            return windowTitles;
        }
    }
}
