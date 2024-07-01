using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;

namespace ChildrenControlSys
{
    public partial class ChildrenControlSystem : ServiceBase
    {
        Thread m_thread = null;
        private static System.Timers.Timer aTimer;

        public ChildrenControlSystem()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Create a timer with a ten second interval.
            //1000 * 60 * 1
            aTimer = new System.Timers.Timer(15000);

            // Hook up the Elapsed event for the timer.
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            // Set the Interval to 2 seconds (2000 milliseconds).
            //aTimer.Interval = 2000;
            aTimer.Enabled = true;
          
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                //Close browsers
                LogUtil.WriteLog(string.Format("The Elapsed event was raised at {0}", e.SignalTime));
                //WindowsByClassFinder.CloseBrowsers(true);

                //On-Off Internet
                string sPath = Path.Combine(@"C:\Users\khanh\source\repos\ChildrenControlSys\ChildrenControlSys\bin\Debug", "configs.xml");
                XDocument xmlDoc = XDocument.Load(sPath);
                var inet = xmlDoc.Descendants("configs").First().Element("internet").Value.ToString().Trim();
                LogUtil.WriteLog(inet);

                if ((inet =="Yes") || (inet == "yes"))
                {
                    if (!ParentalController.IsExisted("iNetPolicy"))
                    {
                        ParentalController.AddFirewallRules("iNetPolicy");
                        LogUtil.WriteLog("Add rule");
                    }                    
                }
                else
                {
                    //Remove all policies
                    ParentalController.RemoveFirewallRules("iNetPolicy");
                    LogUtil.WriteLog("Remove rule");
                } 
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("Error {0}: ", ex.Message));
            }            
        }

        private void ThreadProc()
        {
            // (1000 milliseconds = 1 second, 1 * 60 * 1000 = 60000)
            int interval = 60000; // 1 minutes    
            int elapsed = 0;
            // because we don't want to use 100% of the CPU, we will be 
            // sleeping for 1 second between checks to see if it's time to 
            int waitTime = 1000; // 10 second
            try
            {
                while (true)
                {
                    // if enough time has passed
                    if (interval >= elapsed)
                    {
                        // reset how much time has passed to zero
                        elapsed = 0;
                        //Call services
                        WindowsByClassFinder.CloseBrowsers(true);
                        //GetEmployees();
                    }
                    // Sleep for 1 second
                    Thread.Sleep(waitTime);
                    // indicate that 1 additional second has passed
                    elapsed += waitTime;
                    WriteLog(elapsed.ToString());
                }
            }
            catch (ThreadAbortException ex)
            {
                WriteLog(ex.Message);
            }
        }

        public void WriteLog(string text)
        {
            // Set a variable to the Documents path.
            //string docPath = Environment.CurrentDirectory;
            string docPath = @"C:\Users\khanh\source\repos\ChildrenControlSys\ChildrenControlSys\bin\Debug";

            // Append text to an existing file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, @"logs.txt"), true))
            {
                outputFile.WriteLine(text);
            }
        }

        protected override void OnStop()
        {
            //WindowsByClassFinder.CloseBrowsers(false);
        }
    }
}
