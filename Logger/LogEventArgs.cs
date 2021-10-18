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

        /// <summary>
        /// the log event type
        /// </summary>
        public enum LogEventType
        {
            Debug,
            Error,
            Exception,
            Info,
            Warning,
        }

        /// <summary>
        /// an exception (if applicable) to be logged
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// the text
        /// </summary>
        public String String { get; set; }

        /// <summary>
        /// the type of log
        /// </summary>
        public LogEventType Type { get; set; }

        #region System.Runtime.CompilerServices

        /// <summary>
        /// The method that called the log function. by default, populated from System.Runtime.CompilerServices.CallerMemberName
        /// </summary>
        [Description("The method that called the log function. by default, populated from System.Runtime.CompilerServices.CallerMemberName")]
        public string Caller { get; set; }

        /// <summary>
        /// The code file containing the method that called the log function. by default, populated from System.Runtime.CompilerServices.CallerFilePath
        /// </summary>
        [Description("The code file containing the method that called the log function. by default, populated from System.Runtime.CompilerServices.CallerFilePath")]
        public string File { get; set; }

        /// <summary>
        /// The line of the code file containing the method that called the log function. by default, populated from System.Runtime.CompilerServices.CallerLineNumber
        /// </summary>
        [Description("The line of the code file containing the method that called the log function. by default, populated from System.Runtime.CompilerServices.CallerLineNumber")]
        public int LineNumber { get; set; }

        #endregion System.Runtime.CompilerServices

        #region Threading information

        /// <summary>
        /// The id of the thead that called the log function. by default, populated from System.Threading.Thread.CurrentThread.ManagedThreadId
        /// </summary>
        [Description("The id of the thead that called the log function. by default, populated from System.Threading.Thread.CurrentThread.ManagedThreadId")]
        public int ThreadId { get; private set; }

        /// <summary>
        /// The name of the thead that called the log function. by default, populated from System.Threading.Thread.CurrentThread.Name
        /// </summary>
        [Description("The name of the thead that called the log function. by default, populated from System.Threading.Thread.CurrentThread.Name")]
        public string ThreadName { get; private set; }

        #endregion Threading information
    }
}