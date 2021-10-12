using System;
using SRCS = System.Runtime.CompilerServices;
using STT = System.Threading.Thread;

namespace MyLogger
{
    public abstract class GenericLogger : ILogger, IDisposable
    {
        private bool m_bDisposedValue;

        ~GenericLogger()
        {
            Dispose(disposing: false);
        }

        public event EventHandler<LogEventArgs> OnLog;

        public string LastLine { get; private set; }

        public Flags<bool> Mute { get; set; } = new Flags<bool>();

        public Flags<bool> ShowWhence { get; set; } = new Flags<bool>() { Debug = true, Exception = true, Error = true };

        public static string Whence([SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0)
        {
            return string.Format("{0} at {1}:{2}", strCaller, strFile, iLineNo);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void LogDebug(string str, [SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0)
        {
            try { OnLog?.Invoke(this, new LogEventArgs() { Type = LogEventArgs.LogEventType.Debug, String = str, Caller = strCaller, File = strFile, LineNumber = iLineNo }); } catch { }

            try { lock (LastLine) { LastLine = str; } } catch { }
        }

        public void LogError(string str, [SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0)
        {
            try { OnLog?.Invoke(this, new LogEventArgs() { Type = LogEventArgs.LogEventType.Error, String = str, Caller = strCaller, File = strFile, LineNumber = iLineNo }); } catch { }

            try { lock (LastLine) { LastLine = str; } } catch { }
        }

        public void LogException(string str, Exception ex, [SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0)
        {
            try { OnLog?.Invoke(this, new LogEventArgs() { Type = LogEventArgs.LogEventType.Exception, String = str, Exception = ex, Caller = strCaller, File = strFile, LineNumber = iLineNo }); } catch { }

            try { lock (LastLine) { LastLine = str; } } catch { }
        }

        public void LogInfo(string str, [SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0)
        {
            try { OnLog?.Invoke(this, new LogEventArgs() { Type = LogEventArgs.LogEventType.Info, String = str, Caller = strCaller, File = strFile, LineNumber = iLineNo }); } catch { }

            try { lock (LastLine) { LastLine = str; } } catch { }
        }

        public void LogWarning(string str, [SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0)
        {
            try { OnLog?.Invoke(this, new LogEventArgs() { Type = LogEventArgs.LogEventType.Warning, String = str, Caller = strCaller, File = strFile, LineNumber = iLineNo }); } catch { }

            try { lock (LastLine) { LastLine = str; } } catch { }
        }

        protected virtual void Dispose(bool disposing)
        {
            // classes derived from this, should override the dispose and do their business there

            if (!m_bDisposedValue)
                m_bDisposedValue = true;
        }



    }
}