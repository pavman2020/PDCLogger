using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private MyLogger.FileLogger Logger;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string strFilename = Filename;
            Logger = new MyLogger.FileLogger(strFilename, MyLogger.FileLogger.Interval.Hourly);

            Logger.OnLog += loggerRichTextBox1.HandleLogEvent;
            Logger.OnNewFileIntervalReached += Logger_OnNewFileIntervalReached;

            // loggerRichTextBox1.OnlyThreadIds.Add(System.Threading.Thread.CurrentThread.ManagedThreadId);

            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            timer1.Enabled = true;
        }

        private string Filename { get { return string.Format("C:\\temp\\temp\\test_{0,3:D3}.log", ++iIter); } }

        private void Logger_OnNewFileIntervalReached(object sender, EventArgs e)
        {
            string strFilename = Filename;
            Logger.Filename = strFilename;
        }

        private int iIter = 0;

        #region "simulation"

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = " ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private MyLogger.LogEventArgs.LogEventType eventType = MyLogger.LogEventArgs.LogEventType.Debug;

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            try
            {
                switch (eventType)
                {
                    case MyLogger.LogEventArgs.LogEventType.Debug:
                        Logger.LogDebug(RandomString(80));
                        eventType = MyLogger.LogEventArgs.LogEventType.Error;
                        break;

                    case MyLogger.LogEventArgs.LogEventType.Error:
                        Logger.LogError(RandomString(80));
                        eventType = MyLogger.LogEventArgs.LogEventType.Exception;
                        break;

                    case MyLogger.LogEventArgs.LogEventType.Exception:
                        Logger.LogException(RandomString(80), new Exception("OUTER", new Exception("Inner1", new Exception("Inner2"))));
                        eventType = MyLogger.LogEventArgs.LogEventType.Info;
                        break;

                    case MyLogger.LogEventArgs.LogEventType.Info:
                        Logger.LogInfo(RandomString(80));
                        eventType = MyLogger.LogEventArgs.LogEventType.Warning;
                        break;

                    case MyLogger.LogEventArgs.LogEventType.Warning:
                        Logger.LogWarning(RandomString(80));
                        eventType = MyLogger.LogEventArgs.LogEventType.Debug;
                        break;
                }

                Thread t = new Thread(new ThreadStart(new Action(() =>
                {
                    MyLogger.CachedLogger cl = null;
                    if (1 == spawn)
                    {
                        cl = new MyLogger.CachedLogger(Logger, System.Threading.Thread.CurrentThread.ManagedThreadId);
                    }

                    for (int i = 0; i < 4;)
                    {
                        Logger.LogDebug(string.Format("Iteration number {0} of 4", ++i));
                        Thread.Sleep(random.Next(1000, 5000));
                    }
                    Logger.LogInfo("Iterative Thread Done");

                    if (null != cl)
                    {
                        cl.Pause();
                        foreach (string s in cl.Messages)
                        {
                            Logger.LogInfo(s);
                        }
                        cl.Resume();
                    }
                })))
                { Name = string.Format("Spawn # {0}", ++spawn) };
                t.Start();
            }
            catch { }
            finally { timer1.Enabled = true; }
        }

        private int spawn = 0;

        #endregion "simulation"
    }
}