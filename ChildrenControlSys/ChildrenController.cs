using System;
using System.Collections;
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
        private static System.Timers.Timer aTimer2;

        private List<string> keywords = new List<string>();
        private List<string> applications = new List<string>();

        private string sPath = Path.Combine(@"C:\Users\khanh\source\repos\ChildrenControlSys\ChildrenControlSys\bin\Debug", "configs.xml");
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
                // Load XML from a file
                XDocument xmlDoc = XDocument.Load(sPath);

                // Get all 'keywords' elements within the 'configs' element
                IEnumerable<XElement> keywordElements = xmlDoc.Element("configs").Elements("keywords").Elements("keyword");
                
                // Get all 'author' elements in the XML document
                IEnumerable<XElement> applicationElements = xmlDoc.Element("configs").Elements("applications").Elements("application");
                
                //Close browsers
                LogUtil.WriteLog(string.Format("The Elapsed event was raised at {0}", e.SignalTime));
                

                //On-Off Internet               
                //XDocument xmlDoc = XDocument.Load(sPath);
                var blockiNet = xmlDoc.Descendants("configs").First().Element("blockiNet").Value.ToString().Trim();
                var blockKeyword = xmlDoc.Descendants("configs").First().Element("blockKeyword").Value.ToString().Trim();
                var blockApplication = xmlDoc.Descendants("configs").First().Element("blockApplication").Value.ToString().Trim();
                var blockBrowser = xmlDoc.Descendants("configs").First().Element("blockBrowser").Value.ToString().Trim();

                
                if (blockKeyword.Trim().ToLower() == "yes")
                {
                    try
                    {
                        var titles = WindowsByClassFinder.ChromeWindowTitles();
                        foreach (var keyword in keywordElements)
                        {                           
                            foreach (var title in titles)
                            {
                                LogUtil.WriteLog(title);
                                if (title.ToString().Contains(keyword.Value))
                                {
                                    WindowTitle.CloseWindow(keyword.Value, title);
                                    LogUtil.WriteLog(keyword.Value + ":" + title);
                                }
                            }                            
                        }                                                
                    }
                    catch (Exception ex) {
                        LogUtil.WriteLog(ex.Message);
                    }
                }
                else { }

                if ((blockApplication == "Yes") || (blockApplication == "yes"))
                {
                    foreach(var app in applications)
                    {
                        ParentalController.BlockApps(app);
                    }
                }

                if ((blockBrowser == "Yes") || (blockKeyword == "yes"))
                {
                    WindowsByClassFinder.CloseBrowsers(true);
                }
                
                if ((blockiNet == "Yes") || (blockiNet == "yes"))
                {
                    if (!ParentalController.IsExisted("iNetPolicy"))
                    {
                        ParentalController.AddFirewallRules("iNetPolicy");
                        LogUtil.WriteLog("Add rule");
                    }
                    else
                    {
                        //Remove all policies
                        ParentalController.RemoveFirewallRules("iNetPolicy");
                        LogUtil.WriteLog("Remove rule");
                    }                    
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
