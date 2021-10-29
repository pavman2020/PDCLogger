using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDCLogger
{
    /// <summary>
    /// the CachedLogger class allows the caller to capture all of the log messages during a certain time period - optionally only from a particular thread.
    /// </summary>
    public class CachedLogger : GenericLogger
    {
        private bool m_bPaused = false;

        private int? m_niThreadId = null;

        private object m_oLock = new object();

        /// <summary>
        /// cache the log messages from a specified logger object
        /// </summary>
        /// <param name="logger"></param>
        public CachedLogger(ILogger logger) : base(false)
        {
            logger.OnLog += HandleLoggerOnLog;
        }

        /// <summary>
        /// cache the log messages from a specified logger object but only from the specified thread id
        /// </summary>
        public CachedLogger(ILogger logger, int iThreadId) : this(logger)
        {
            m_niThreadId = iThreadId;
        }

        /// <summary>
        /// the messages cached during this object's lifetime
        /// </summary>
        public List<string> Messages { get; private set; } = new List<string>();

        /// <summary>
        /// pause caching messages
        /// </summary>
        public void Pause()
        {
            m_bPaused = true;
        }

        /// <summary>
        /// resume caching messages (if paused)
        /// </summary>
        public void Resume()
        {
            m_bPaused = false;
        }
        
        /// <summary>
        /// show the level of the messages
        /// </summary>
        public bool ShowLevel { get; set; } = true;

        /// <summary>
        /// this the thread id that generated the messages
        /// </summary>
        public bool ShowThread { get; set; } = true;

        /// <summary>
        /// show the timestamp of each message
        /// </summary>
        public bool ShowTimeStamp { get; set; } = true;

        private void HandleLoggerOnLog(object sender, LogEventArgs e)
        {
            if (IsDisposed)
                return;

            if (m_bPaused)
                return;

            if ((m_niThreadId.HasValue) && (m_niThreadId != e.ThreadId))
                return;

            try
            {
                Action<string, string, string> appendLineToList = new Action<string, string, string>((strPrefix, strMessage, strWhence) =>
                {
                    lock (m_oLock)
                    {
                        Messages.Add(string.Format("{0}{2}{3}{1} {4}"
                                        , ShowTimeStamp
                                            ? string.Format("{0} ", DateTime.Now.ToString("yyyyMMdd.HHmmss.fff"))
                                            : string.Empty
                                        , strMessage
                                        , ShowLevel
                                            ? string.Format("[{0}] ", strPrefix)
                                            : string.Empty
                                        , ShowThread
                                            ? string.Format("[Thd:{0}/{1}] ", e.ThreadId, e.ThreadName)
                                            : string.Empty
                                        , strWhence
                                       ).TrimEnd());
                    }
                });

                Action<bool, bool, string, Action> handler = new Action<bool, bool, string, Action>((bMute, bWhence, sPfx, fExtra) =>
                {
                    if (!bMute)
                    {
                        appendLineToList(sPfx, e.String, bWhence ? Utility.Whence(e.Caller, e.File, e.LineNumber) : string.Empty);
                        fExtra?.Invoke();
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
                                appendLineToList("X", strIndent + "Exception : " + ex.Message, string.Empty);
                                appendLineToList("X", strIndent + "    Stack : " + ex.StackTrace, string.Empty);
                                appendLineToList("X", strIndent + "   Source : " + ex.Source, string.Empty);
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