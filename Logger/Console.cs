using System;

namespace PDCLogger
{
    public static class Console
    {
        public static Flags<ConsoleColors> Colors { get; set; } = new Flags<ConsoleColors>()
        {
            Debug = new ConsoleColors() { Background = ConsoleColor.Black, Foreground = ConsoleColor.Green },
            Error = new ConsoleColors() { Background = ConsoleColor.Black, Foreground = ConsoleColor.Red },
            Exception = new ConsoleColors() { Background = ConsoleColor.Black, Foreground = ConsoleColor.Magenta },
            Info = new ConsoleColors() { Background = ConsoleColor.Black, Foreground = ConsoleColor.White },
            Warning = new ConsoleColors() { Background = ConsoleColor.Black, Foreground = ConsoleColor.Yellow },
        };

        /// <summary>
        /// a static event handler for the LogEventArgs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnLog(object sender, LogEventArgs e)
        {
            ConsoleColor fg = System.Console.ForegroundColor;
            ConsoleColor bg = System.Console.BackgroundColor;

            try
            {
                switch (e.Type)
                {
                    case LogEventArgs.LogEventType.Debug:
                        System.Console.ForegroundColor = Colors.Debug.Foreground;
                        System.Console.BackgroundColor = Colors.Debug.Background;
                        break;

                    case LogEventArgs.LogEventType.Error:
                        System.Console.ForegroundColor = Colors.Error.Foreground;
                        System.Console.BackgroundColor = Colors.Error.Background;
                        break;

                    case LogEventArgs.LogEventType.Exception:
                        System.Console.ForegroundColor = Colors.Exception.Foreground;
                        System.Console.BackgroundColor = Colors.Exception.Background;
                        break;

                    case LogEventArgs.LogEventType.Info:
                        System.Console.ForegroundColor = Colors.Info.Foreground;
                        System.Console.BackgroundColor = Colors.Info.Background;
                        break;

                    case LogEventArgs.LogEventType.Warning:
                        System.Console.ForegroundColor = Colors.Warning.Foreground;
                        System.Console.BackgroundColor = Colors.Warning.Background;
                        break;
                }
                System.Console.WriteLine(e.String);

                _LogException(e.Exception, string.Empty);
            }
            catch { }
            finally
            {
                System.Console.ForegroundColor = fg;
                System.Console.BackgroundColor = bg;
            }
        }

        private static void _LogException(Exception ex, string strIndent)
        {
            if (null == ex) return;

            System.Console.WriteLine(string.Format("{0}EXCEPTION: {1}", strIndent, ex.Message));
            System.Console.WriteLine(string.Format("{0}  SOURCE: {1}", strIndent, ex.Source));
            System.Console.WriteLine(string.Format("{0}  STACK: {1}", strIndent, ex.StackTrace));

            _LogException(ex.InnerException, string.Format("{0}    ", strIndent));
        }
    }
}