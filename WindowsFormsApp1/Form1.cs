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

        private PDCLogger.FileLogger Logger;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Logger = new PDCLogger.FileLogger(PDCLogger.FileLogger.Interval.Hourly, Logger_OnNewFileIntervalReached, false);

            Logger.OnLog += loggerRichTextBox1.HandleLogEvent;

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

        private PDCLogger.LogEventArgs.LogEventType eventType = PDCLogger.LogEventArgs.LogEventType.Debug;

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            try
            {
                switch (eventType)
                {
                    case PDCLogger.LogEventArgs.LogEventType.Debug:
                        Logger.LogDebug(RandomString(80));
                        eventType = PDCLogger.LogEventArgs.LogEventType.Error;
                        break;

                    case PDCLogger.LogEventArgs.LogEventType.Error:
                        Logger.LogError(RandomString(80));
                        eventType = PDCLogger.LogEventArgs.LogEventType.Exception;
                        break;

                    case PDCLogger.LogEventArgs.LogEventType.Exception:
                        try
                        {
                            int i = 0; int j = 5 / i;
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(RandomString(80), new Exception("OUTER", new Exception("Inner1", new Exception("Inner2", ex))));
                        }
                        eventType = PDCLogger.LogEventArgs.LogEventType.Info;
                        break;

                    case PDCLogger.LogEventArgs.LogEventType.Info:
                        Logger.LogInfo(RandomString(80));
                        eventType = PDCLogger.LogEventArgs.LogEventType.Warning;
                        break;

                    case PDCLogger.LogEventArgs.LogEventType.Warning:
                        Logger.LogWarning(RandomString(80));
                        eventType = PDCLogger.LogEventArgs.LogEventType.Debug;
                        break;
                }

                Thread t = new Thread(new ThreadStart(new Action(() =>
                {
                    PDCLogger.CachedLogger cl = null;
                    if (1 == spawn)
                    {
                        cl = new PDCLogger.CachedLogger(Logger, System.Threading.Thread.CurrentThread.ManagedThreadId);
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