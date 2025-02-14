﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ChildrenControlSys
{
    public class WindowsByClassFinder1
    {
        public delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lparam);

        [SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool EnumWindows(EnumWindowsDelegate lpEnumFunc, IntPtr lparam);

        [SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("User32", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr windowHandle, StringBuilder stringBuilder, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "GetWindowTextLength", SetLastError = true)]
        internal static extern int GetWindowTextLength(IntPtr hwnd);
        /// <summary>Find the windows matching the specified class name.</summary>
        public static IEnumerable<IntPtr> WindowsMatching(string className)
        {
            return new WindowsByClassFinder1(className)._result;
        }
        private WindowsByClassFinder1(string className)
        {
            _className = className;
            EnumWindows(callback, IntPtr.Zero);
        }
        private bool callback(IntPtr hWnd, IntPtr lparam)
        {
            if (GetClassName(hWnd, _apiResult, _apiResult.Capacity) != 0)
            {
                if (string.CompareOrdinal(_apiResult.ToString(), _className) == 0)
                {
                    _result.Add(hWnd);
                }
            }
            return true; // Keep enumerating.
        }
        public static IEnumerable<string> WindowTitlesForClass(string className)
        {
            foreach (var windowHandle in WindowsMatchingClassName(className))
            {
                int length = GetWindowTextLength(windowHandle);
                StringBuilder sb = new StringBuilder(length + 1);
                GetWindowText(windowHandle, sb, sb.Capacity);
                yield return sb.ToString();
            }
        }
        public static void CloseBrowsers(bool closeAllBrowsers)
        {
            if (closeAllBrowsers)
            {
             
                //string sPath = Path.Combine(Environment.CurrentDirectory,  "configs.xml");
                string sPath = Path.Combine(@"C:\Users\khanh\source\repos\ChildrenControlSys\ChildrenControlSys\bin\Debug", "configs.xml");
                XDocument xmlDoc = XDocument.Load(sPath);
                IEnumerable<XElement> keywordElements = xmlDoc.Element("configs").Elements("browsers").Elements("browser");

                Process[] processes = Process.GetProcessesByName("browser");
                foreach (Process p in processes)
                {
                    p.Kill();
                }
                processes = Process.GetProcessesByName("msedge");
                foreach (Process p in processes)
                {
                    p.Kill();
                }

                processes = Process.GetProcessesByName("firefox");
                foreach (Process p in processes)
                {
                    p.Kill();
                }

                processes = Process.GetProcessesByName("chrome");
                foreach (Process p in processes)
                {
                    p.Kill();
                }
            }            
        }
        public static List<string> GetProcesses()
        {
            List<string> processList = new List<string>();
            Process[] processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                processList.Add(process.MainWindowTitle);
            }
            return processList;
        }
        public static IEnumerable<IntPtr> WindowsMatchingClassName(string className)
        {
            if (string.IsNullOrWhiteSpace(className))
                throw new ArgumentOutOfRangeException("className", className, "className can't be null or blank.");

            return WindowsMatching(className);
        }

        private readonly string _className;
        private readonly List<IntPtr> _result = new List<IntPtr>();
        private readonly StringBuilder _apiResult = new StringBuilder(1024);
    }
}
