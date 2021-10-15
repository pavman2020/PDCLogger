using System;
using SRCS = System.Runtime.CompilerServices;

namespace PDCLogger
{
    public interface ILogger
    {
        event EventHandler<LogEventArgs> OnLog;

        void LogDebug(string str, [SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0);

        void LogError(string str, [SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0);

        void LogException(string str, Exception ex, [SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0);

        void LogInfo(string str, [SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0);

        void LogWarning(string str, [SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0);

        string LastLine { get; }
    }
}