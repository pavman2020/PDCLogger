using System;
using SRCS = System.Runtime.CompilerServices;

namespace PDCLogger
{
    public abstract class GenericLogger : ILogger, IDisposable
    {
        #region "Construction / Destruction"

        private bool m_bDisposedValue;

        ~GenericLogger()
        {
            Dispose(disposing: false);
        }

        protected virtual void Dispose(bool disposing)
        {
            // classes derived from this, should override the dispose and do their business there

            if (!m_bDisposedValue)
                m_bDisposedValue = true;
        }

        #endregion "Construction / Destruction"

        /// <summary>
        /// Event is raised when text has been logged. the eventArgs (typeof<LogEventArgs>) contains information about the text logged
        /// </summary>
        public event EventHandler<LogEventArgs> OnLog;

        /// <summary>
        /// allows program to mute message levels
        /// </summary>
        public Flags<bool> Mute { get; set; } = new Flags<bool>();

        /// <summary>
        /// allows programs to dictate if program source code location is logged 
        /// </summary>
        public Flags<bool> ShowWhence { get; set; } = new Flags<bool>() { Debug = true, Exception = true, Error = true };

        #region "IDisposable"

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion "IDisposable"

        #region "ILogger"

        /// <summary>
        /// logs Debug text
        /// </summary>
        /// <param name="str">the text to be logged</param>
        /// <param name="strCaller">autofilled by System.Runtime.CompilerServices.CallerMemberName</param>
        /// <param name="strFile">autofilled by System.Runtime.CompilerServices.CallerFilePath</param>
        /// <param name="iLineNo">autofilled by System.Runtime.CompilerServices.CallerLineNumber</param>
        public void LogDebug(string str, [SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0)
        {
            try { OnLog?.Invoke(this, new LogEventArgs() { Type = LogEventArgs.LogEventType.Debug, String = str, Caller = strCaller, File = strFile, LineNumber = iLineNo }); } catch { }

            try { lock (LastLine) { LastLine = str; } } catch { }
        }

        /// <summary>
        /// logs Error text
        /// </summary>
        /// <param name="str">the text to be logged</param>
        /// <param name="strCaller">autofilled by System.Runtime.CompilerServices.CallerMemberName</param>
        /// <param name="strFile">autofilled by System.Runtime.CompilerServices.CallerFilePath</param>
        /// <param name="iLineNo">autofilled by System.Runtime.CompilerServices.CallerLineNumber</param>
        public void LogError(string str, [SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0)
        {
            try { OnLog?.Invoke(this, new LogEventArgs() { Type = LogEventArgs.LogEventType.Error, String = str, Caller = strCaller, File = strFile, LineNumber = iLineNo }); } catch { }

            try { lock (LastLine) { LastLine = str; } } catch { }
        }

        /// <summary>
        /// logs an Exception with text
        /// </summary>
        /// <param name="str">the text to be logged</param>
        /// <param name="ex">the Exception object</param>
        /// <param name="strCaller">autofilled by System.Runtime.CompilerServices.CallerMemberName</param>
        /// <param name="strFile">autofilled by System.Runtime.CompilerServices.CallerFilePath</param>
        /// <param name="iLineNo">autofilled by System.Runtime.CompilerServices.CallerLineNumber</param>
        public void LogException(string str, Exception ex, [SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0)
        {
            try { OnLog?.Invoke(this, new LogEventArgs() { Type = LogEventArgs.LogEventType.Exception, String = str, Exception = ex, Caller = strCaller, File = strFile, LineNumber = iLineNo }); } catch { }

            try { lock (LastLine) { LastLine = str; } } catch { }
        }

        /// <summary>
        /// logs Info text
        /// </summary>
        /// <param name="str">the text to be logged</param>
        /// <param name="strCaller">autofilled by System.Runtime.CompilerServices.CallerMemberName</param>
        /// <param name="strFile">autofilled by System.Runtime.CompilerServices.CallerFilePath</param>
        /// <param name="iLineNo">autofilled by System.Runtime.CompilerServices.CallerLineNumber</param>
        public void LogInfo(string str, [SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0)
        {
            try { OnLog?.Invoke(this, new LogEventArgs() { Type = LogEventArgs.LogEventType.Info, String = str, Caller = strCaller, File = strFile, LineNumber = iLineNo }); } catch { }

            try { lock (LastLine) { LastLine = str; } } catch { }
        }

        /// <summary>
        /// logs Warning text
        /// </summary>
        /// <param name="str">the text to be logged</param>
        /// <param name="strCaller">autofilled by System.Runtime.CompilerServices.CallerMemberName</param>
        /// <param name="strFile">autofilled by System.Runtime.CompilerServices.CallerFilePath</param>
        /// <param name="iLineNo">autofilled by System.Runtime.CompilerServices.CallerLineNumber</param>
        public void LogWarning(string str, [SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0)
        {
            try { OnLog?.Invoke(this, new LogEventArgs() { Type = LogEventArgs.LogEventType.Warning, String = str, Caller = strCaller, File = strFile, LineNumber = iLineNo }); } catch { }

            try { lock (LastLine) { LastLine = str; } } catch { }
        }

        /// <summary>
        /// Retrieves the last line logged
        /// </summary>
        public string LastLine { get; private set; }

        #endregion "ILogger"
    }
}