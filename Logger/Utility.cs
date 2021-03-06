using System.Text;
using SRCS = System.Runtime.CompilerServices;

namespace PDCLogger
{
    public static class Utility
    {

        /// <summary>
        /// a static function which shows the stack trace
        /// </summary>
        /// <param name="strCaller">autofilled by System.Runtime.CompilerServices.CallerMemberName</param>
        /// <param name="strFile">autofilled by System.Runtime.CompilerServices.CallerFilePath</param>
        /// <param name="iLineNo">autofilled by System.Runtime.CompilerServices.CallerLineNumber</param>
        /// <returns>a string containing the stack trace (with CR-LF at each end of line)</returns>
        public static string StackTrace([SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Whence(strCaller, strFile, iLineNo));

            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(true);
            System.Diagnostics.StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)

            if (null != stackFrames)
                try
                {
                    bool bFirst = true;
                    sb.Append("\r\n   Stack Trace:");
                    foreach (System.Diagnostics.StackFrame oSF in stackFrames)
                        if (bFirst)
                            bFirst = false;  // so we skip "StackTrace"
                        else
                        {
                            System.Reflection.MethodBase oMB = oSF.GetMethod();
                            sb.Append(string.Format("\r\n     {1} in {0}  ({2}:{3})", oMB.Module, oMB.Name, oSF.GetFileName(), oSF.GetFileLineNumber()));
                        }
                }
                catch { }

            return sb.ToString();
        }

        /// <summary>
        /// shows where in the source code the call to Whence() was made.
        /// </summary>
        /// <param name="strCaller">autofilled by System.Runtime.CompilerServices.CallerMemberName</param>
        /// <param name="strFile">autofilled by System.Runtime.CompilerServices.CallerFilePath</param>
        /// <param name="iLineNo">autofilled by System.Runtime.CompilerServices.CallerLineNumber</param>
        /// <returns>returns a string showing where the routine was called.</returns>
        public static string Whence([SRCS.CallerMemberName] string strCaller = null, [SRCS.CallerFilePath] string strFile = null, [SRCS.CallerLineNumber] int iLineNo = 0)
        {
            return string.Format("{0} at {1}:{2}", strCaller, strFile, iLineNo);
        }
    }
}