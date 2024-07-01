using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildrenControlSys
{
    public class ChildrenControlServices
    {
		private static string lastWindowTitle;
		#region Windows API Functions Declarations
		[System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
		public static extern int GetWindowText(IntPtr hwnd, string lpString, int cch);
		[System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
		private static extern IntPtr GetForegroundWindow();
		[System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
		private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out Int32 lpdwProcessId);
		#endregion

		#region User-defined Functions
		public static Int32 GetWindowProcessID(IntPtr hwnd)
		{
			Int32 pid;
			GetWindowThreadProcessId(hwnd, out pid);
			return pid;
		}
		public static IntPtr getforegroundWindow()
		{
			return GetForegroundWindow();
		}
		public static string ActiveApplTitle()
		{
			IntPtr hwnd = getforegroundWindow();
			if (hwnd.Equals(IntPtr.Zero)) return "";
			string lpText = new string((char)0, 100);
			int intLength = GetWindowText(hwnd, lpText, lpText.Length);
			if ((intLength <= 0) || (intLength > lpText.Length)) return "unknown";
			return lpText.Trim();
		}
		#endregion

		public static void BlockWebAndApp(string[] keywords)
		{
			Process[] processes = Process.GetProcesses();
			List<string> processNames = new List<string>();
			List<string> processTitles = new List<string>();

			foreach (Process p in processes)
			{
				processNames.Add(p.ProcessName);
				processTitles.Add(p.MainWindowTitle);
			}
			foreach (string kw in keywords)
			{
				for (int i = 0; i < processTitles.Count; i++)
				{
					if (processTitles[i].Contains(kw))
					{
						string processName = processNames[i];
						Process[] pes = Process.GetProcessesByName(processName);
						foreach (Process p in pes)
						{
							p.Kill();
						}
					}
				}
			}
		}
	}
}
