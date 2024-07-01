using System;
using System.IO;

namespace ChildrenControlSys
{
    public class LogUtil
    {
        public static void WriteLog(string text)
        {
            string docPath = @"C:\Users\khanh\source\repos\ChildrenControlSys\ChildrenControlSys\bin\Debug";
            //string docPath = Environment.CurrentDirectory;
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, @"logs.txt"), true))
            {
                outputFile.WriteLine(text);
            }
        }
    }
}