using System;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace PDCLogger
{
    public class LogEventArgs : EventArgs
    {
        public LogEventArgs()
        {
            ThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            ThreadName = System.Threading.Thread.CurrentThread.Name;
        }

        public enum LogEventType
        {
            Debug,
            Error,
            Exception,
            Info,
            Warning,
        }

        public Exception Exception { get; set; }

        public String String { get; set; }

        public LogEventType Type { get; set; }

        #region System.Runtime.CompilerServices

        [Description("The method that called the log function. by default, populated from System.Runtime.CompilerServices.CallerMemberName")]
        public string Caller { get; set; }

        [Description("The code file containing the method that called the log function. by default, populated from System.Runtime.CompilerServices.CallerFilePath")]
        public string File { get; set; }

        [Description("The line of the code file containing the method that called the log function. by default, populated from System.Runtime.CompilerServices.CallerLineNumber")]
        public int LineNumber { get; set; }

        #endregion System.Runtime.CompilerServices

        #region Threading information

        [Description("The id of the thead that called the log function. by default, populated from System.Threading.Thread.CurrentThread.ManagedThreadId")]
        public int ThreadId { get; private set; }

        [Description("The name of the thead that called the log function. by default, populated from System.Threading.Thread.CurrentThread.Name")]
        public string ThreadName { get; private set; }

        #endregion Threading information
    }
}