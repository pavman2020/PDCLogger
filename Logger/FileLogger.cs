using System;
using System.IO;

namespace MyLogger
{
    public class FileLogger : GenericLogger
    {
        private bool m_bDisposedValue = false;

        private int m_iCurrentNumLines = 0;

        private long m_iDayFileOpened = 0;

        private object m_oLock = new object();

        private StreamWriter m_oStreamWriter = null;

        private string m_strFilename = string.Empty;

        public FileLogger()
        {
            this.OnLog += HandleLoggerOnLog;
        }

        public FileLogger(string strFilename) : this()
        {
            Filename = strFilename;
        }

        public FileLogger(Interval interval) : this(string.Empty, interval)
        {
        }

        public FileLogger(string strFilename, Interval interval) : this(strFilename)
        {
            NewFileAfter = interval;
        }

        public event EventHandler OnNewFileIntervalReached;

        public enum Interval
        {
            Never,
            Daily,
            Hourly,
            AfterNLines,
        }

        public string Filename
        {
            get { return m_strFilename; }

            set
            {
                lock (m_oLock)
                {
                    if (null != m_oStreamWriter)
                    {
                        try { m_oStreamWriter.Flush(); } catch { }
                        try { m_oStreamWriter.Close(); } catch { }
                        m_oStreamWriter = null;
                    }

                    m_strFilename = value;

                    if (!string.IsNullOrEmpty(m_strFilename))
                        m_oStreamWriter = File.AppendText(m_strFilename);

                    m_iCurrentNumLines = 0;

                    switch (NewFileAfter)
                    {
                        case Interval.Daily:
                            m_iDayFileOpened = CurrentDay;
                            break;

                        case Interval.Hourly:
                            m_iDayFileOpened = CurrentHour;
                            break;
                    }

                }
            }
        }

        public int MaxLines { get; set; } = 0;

        public Interval NewFileAfter { get; private set; } = Interval.Never;

        public bool ShowLevel { get; set; } = true;

        public bool ShowThread { get; set; } = true;

        public bool ShowTimeStamp { get; set; } = true;

        private long CurrentDay { get { return 100 * long.Parse(DateTime.Now.ToString("yyyyMMdd")); } }

        private long CurrentHour { get { return CurrentDay + DateTime.Now.Hour; } }

        protected override void Dispose(bool disposing)
        {
            if (!m_bDisposedValue)
            {
                //if (disposing)
                //{
                //}

                lock (m_oLock)
                {
                    if (null != m_oStreamWriter)
                    {
                        try { m_oStreamWriter.Flush(); } catch { }
                        try { m_oStreamWriter.Close(); } catch { }
                        try { m_oStreamWriter.Dispose(); } catch { }
                        m_oStreamWriter = null;
                    }
                }

                m_bDisposedValue = true;
            }
        }

        private void HandleLoggerOnLog(object o, LogEventArgs e)
        {
            try
            {
                Action<string, string, string> appendLineToFile = new Action<string, string, string>((strPrefix, strMessage, strWhence) =>
                {
                    lock (m_oLock)
                        if (null != m_oStreamWriter)
                        {
                            m_oStreamWriter.WriteLine(string.Format("{0}{1}{2}{3} {4}"
                                                                    , ShowTimeStamp
                                                                        ? string.Format("{0} ", DateTime.Now.ToString("yyyyMMdd.HHmmss.fff"))
                                                                        : string.Empty
                                                                    , ShowLevel
                                                                        ? string.Format("[{0}] ", strPrefix)
                                                                        : string.Empty
                                                                    , ShowThread
                                                                        ? string.IsNullOrEmpty(e.ThreadName)
                                                                            ? string.Format("[Thd:{0}] ", e.ThreadId)
                                                                            : string.Format("[Thd:{0}/{1}] ", e.ThreadId, e.ThreadName)
                                                                        : string.Empty
                                                                    , strMessage
                                                                    , strWhence
                                                                   ).TrimEnd());
                            m_oStreamWriter.Flush();
                        }
                });

                Action<bool, bool, string, Action> handler = new Action<bool, bool, string, Action>((bMute, bWhence, sPfx, fExtra) =>
                {
                    if (!bMute)
                    {
                        switch (NewFileAfter)
                        {
                            case Interval.AfterNLines:
                                if ((MaxLines > 0) && (m_iCurrentNumLines >= MaxLines))
                                    OnNewFileIntervalReached?.Invoke(this, new EventArgs());
                                break;

                            case Interval.Hourly:
                                if (m_iDayFileOpened != CurrentHour)
                                    OnNewFileIntervalReached?.Invoke(this, new EventArgs());
                                break;

                            case Interval.Daily:
                                if (m_iDayFileOpened != CurrentDay)
                                    OnNewFileIntervalReached?.Invoke(this, new EventArgs());
                                break;
                        }

                        appendLineToFile(sPfx, e.String, bWhence ? Whence(e.Caller, e.File, e.LineNumber) : string.Empty);
                        fExtra?.Invoke();
                        m_iCurrentNumLines++;
                    }
                });

                switch (e.Type)
                {
                    case LogEventArgs.LogEventType.Info:
                        handler(Mute.Info, ShowWhence.Info, "I", null);
                        break;

                    case LogEventArgs.LogEventType.Debug:
                        handler(Mute.Debug, ShowWhence.Debug, "D", null);
                        break;

                    case LogEventArgs.LogEventType.Error:
                        handler(Mute.Error, ShowWhence.Error, "E", null);
                        break;

                    case LogEventArgs.LogEventType.Warning:
                        handler(Mute.Warning, ShowWhence.Warning, "W", null);
                        break;

                    case LogEventArgs.LogEventType.Exception:
                        handler(Mute.Exception, ShowWhence.Exception, "X", new Action(() =>
                        {
                            string strIndent = "     ";
                            for (Exception ex = e.Exception; null != ex; ex = ex.InnerException)
                            {
                                appendLineToFile("X", strIndent + "Exception : " + ex.Message, string.Empty);
                                appendLineToFile("X", strIndent + "    Stack : " + ex.StackTrace, string.Empty);
                                appendLineToFile("X", strIndent + "   Source : " + ex.Source, string.Empty);
                                strIndent += "     ";
                            }
                        }));
                        break;
                }
            }
            catch { }
        }
    }
}