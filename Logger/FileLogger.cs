using System;
using System.IO;

namespace PDCLogger
{
    public class FileLogger : GenericLogger
    {
        private bool m_bDisposedValue = false;

        private int m_iCurrentNumLines = 0;

        private long m_iDayFileOpened = 0;

        private object m_oLock = new object();

        private StreamWriter m_oStreamWriter = null;

        private string m_strFilename = string.Empty;

        #region "Constructor"

        /// <summary>
        ///
        /// </summary>
        /// <param name="bAttachConsole">if true (the default), the System.Console will be cc'd with every message </param>
        protected FileLogger(bool bAttachConsole = true) : base(bAttachConsole)
        {
            this.OnLog += HandleLoggerOnLog;
        }

        /// <summary>
        /// Create a logger attached to file. this file will be in use during the entire run, unless the Filename attribute is changed.
        /// </summary>
        /// <param name="strFilename">the fully pathed filename</param>
        /// <param name="bAttachConsole">if true (the default), the System.Console will be cc'd with every message </param>
        public FileLogger(string strFilename, bool bAttachConsole = true) : this(bAttachConsole)
        {
            Filename = strFilename;
        }

        /// <summary>
        /// Create a logger attached to file. this file will be in use until the file reaches 'iMaxLines' in length, and then the object will raise an event, which will be handled by the EventHandler 'ehNewFileIntervalReached'. This event handling must create and set this object's new filename
        /// </summary>
        /// <param name="strFilename">the fully pathed filename</param>
        /// <param name="iMaxEntries"></param>
        /// <param name="ehNewFileIntervalReached">EventHandler to indicate a new file is needed</param>
        /// <param name="bAttachConsole">if true (the default), the System.Console will be cc'd with every message </param>
        public FileLogger(string strFilename, int iMaxEntries, EventHandler ehNewFileIntervalReached, bool bAttachConsole = true) : this(strFilename, bAttachConsole)
        {
            NewFileAfter = Interval.AfterNLines;
            MaxEntries = iMaxEntries;
            OnNewFileIntervalReached += ehNewFileIntervalReached;
        }

        /// <summary>
        /// Create a logger attached to file. this file will be in use until a specified time interval is reached, and then the object will raise an event, which will be handled by the EventHandler 'ehNewFileIntervalReached'. This event handling must create and set this object's new filename
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="ehNewFileIntervalReached">EventHandler to indicate a new file is needed</param>
        /// <param name="bAttachConsole">if true (the default), the System.Console will be cc'd with every message </param>
        public FileLogger(Interval interval, EventHandler ehNewFileIntervalReached, bool bAttachConsole = true) : this(bAttachConsole)
        {
            switch (interval)
            {
                case Interval.AfterNLines:
                    throw new Exception(string.Format("To use {0} for interval, use the {1} constructor",
                        interval,
                        string.Format("{0}(string, int, EventHandler, bool)", typeof(FileLogger).ToString())
                        ));

                case Interval.Never:
                    throw new Exception(string.Format("To use {0} for interval, use the {1} constructor",
                        interval,
                        string.Format("{0}(string, bool)", typeof(FileLogger).ToString())
                        ));
            }

            NewFileAfter = interval;
            OnNewFileIntervalReached += ehNewFileIntervalReached;
        }

        #endregion "Constructor"

        public event EventHandler OnNewFileIntervalReached;

        public enum Interval
        {
            Never,
            Daily,
            Hourly,
            AfterNLines,
        }

        /// <summary>
        /// log file fully pathed filename - when set, the FileLogger object will open the file and begin using it log events
        /// if the path is incorrect - an exception will likely be thrown - the caller must catch and handle all exceptions
        /// if the file already exists, it will be opened and appended to.
        /// </summary>
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

        /// <summary>
        /// specifies the maximum number of entries to be written to the log file.
        /// this can only be set through construction
        /// </summary>
        public int? MaxEntries { get; private set; } = null;

        /// <summary>
        /// specifies when a new file should be created - can only be set through construction
        /// </summary>
        public Interval NewFileAfter { get; private set; } = Interval.Never;

        /// <summary>
        /// show the level of the message in the log file
        /// </summary>
        public bool ShowLevel { get; set; } = true;

        /// <summary>
        ///  show the thread id of the message  in the log file
        /// </summary>
        public bool ShowThread { get; set; } = true;

        /// <summary>
        ///  show the timestamp in the log file
        /// </summary>
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
                                if (((MaxEntries ?? 0) > 0) && (m_iCurrentNumLines >= (MaxEntries ?? 0)))
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

                        appendLineToFile(sPfx, e.String, bWhence ? Utility.Whence(e.Caller, e.File, e.LineNumber) : string.Empty);
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