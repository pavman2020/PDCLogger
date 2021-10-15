using System;
using SRCS = System.Runtime.CompilerServices;

namespace PDCLogger
{
    public interface ILogger
    {
        /// <summary>
        /// Event is raised when text has been logged. the eventArgs (typeof<LogEventArgs>) contains information about the text logged
        /// </summary>
        event EventHandler<LogEventArgs> OnLog;

        /// <summary>
        /// logs Debug text
        /// </summary>
        /// <param name="str">the text to be logged</param>
        /// <param name="strCaller">autofilled by System.Runtime.CompilerServices.CallerMemberName</param>
        /// <param name="strFile">autofilled by System.Runtime.CompilerServices.CallerFilePath</param>
        /// <param name="iLineNo">autofilled by System.Runtime.CompilerServices.CallerLineNumber</param>
        void LogDebug(string str, [SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0);

        /// <summary>
        /// logs Error text
        /// </summary>
        /// <param name="str">the text to be logged</param>
        /// <param name="strCaller">autofilled by System.Runtime.CompilerServices.CallerMemberName</param>
        /// <param name="strFile">autofilled by System.Runtime.CompilerServices.CallerFilePath</param>
        /// <param name="iLineNo">autofilled by System.Runtime.CompilerServices.CallerLineNumber</param>
        void LogError(string str, [SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0);

        /// <summary>
        /// logs an Exception with text
        /// </summary>
        /// <param name="str">the text to be logged</param>
        /// <param name="ex">the Exception object</param>
        /// <param name="strCaller">autofilled by System.Runtime.CompilerServices.CallerMemberName</param>
        /// <param name="strFile">autofilled by System.Runtime.CompilerServices.CallerFilePath</param>
        /// <param name="iLineNo">autofilled by System.Runtime.CompilerServices.CallerLineNumber</param>
        void LogException(string str, Exception ex, [SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0);

        /// <summary>
        /// logs Info text
        /// </summary>
        /// <param name="str">the text to be logged</param>
        /// <param name="strCaller">autofilled by System.Runtime.CompilerServices.CallerMemberName</param>
        /// <param name="strFile">autofilled by System.Runtime.CompilerServices.CallerFilePath</param>
        /// <param name="iLineNo">autofilled by System.Runtime.CompilerServices.CallerLineNumber</param>
        void LogInfo(string str, [SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0);

        /// <summary>
        /// logs Warning text
        /// </summary>
        /// <param name="str">the text to be logged</param>
        /// <param name="strCaller">autofilled by System.Runtime.CompilerServices.CallerMemberName</param>
        /// <param name="strFile">autofilled by System.Runtime.CompilerServices.CallerFilePath</param>
        /// <param name="iLineNo">autofilled by System.Runtime.CompilerServices.CallerLineNumber</param>
        void LogWarning(string str, [SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0);

        /// <summary>
        /// Retrieves the last line logged
        /// </summary>
        string LastLine { get; }
    }
}