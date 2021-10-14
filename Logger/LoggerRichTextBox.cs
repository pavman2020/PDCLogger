using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;
using System.Threading;
using System.Windows.Forms.Design;

namespace PDCLogger
{
    public class LoggerRichTextBox : RichTextBox
    {
        private const string MyDefaultFont = "Lucida Console, 10pt";

        private object m_oHandlingLogEvent = new object();

        public LoggerRichTextBox()
        {
            this.Font = (Font)new FontConverter().ConvertFromString(MyDefaultFont);
        }

        [DefaultValue(typeof(Font), MyDefaultFont)]
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public override Font Font { get { return base.Font; } set { base.Font = value; } }

        [Category("Appearance")]
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Flags<RTBColors> LevelColoration { get; set; } = new Flags<RTBColors>()
        {
            Debug = new RTBColors() { Background = Color.MediumBlue, Foreground = Color.White },
            Info = new RTBColors() { Background = Color.White, Foreground = Color.Black },
            Error = new RTBColors() { Background = Color.Red, Foreground = Color.White },
            Exception = new RTBColors() { Background = Color.Red, Foreground = Color.Yellow },
            Warning = new RTBColors() { Background = Color.Yellow, Foreground = Color.Black },
        };

        [Category("Appearance")]
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Flags<bool> Mute { get; set; } = new Flags<bool>();

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public List<int> OnlyThreadIds { get; private set; } = new List<int>();

        [DefaultValue(true)]
        [Category("Behavior")]
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public bool ScrollToCaretOnAdd { get; set; } = true;

        [DefaultValue(true)]
        [Category("Appearance")]
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public bool ShowLevel { get; set; } = true;

        [DefaultValue(true)]
        [Category("Appearance")]
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public bool ShowThread { get; set; } = true;

        [DefaultValue(true)]
        [Category("Appearance")]
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public bool ShowTimestamp { get; set; } = true;

        [Category("Appearance")]
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Flags<bool> ShowWhence { get; set; } = new Flags<bool>() { Debug = true, Error = true, Exception = true };

        public void AppendLine(string str, Color foreColor, Color backColor)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(() => { AppendLine(str, foreColor, backColor); }));
                }
                else
                {
                    SelectionStart = TextLength;
                    SelectionBackColor = backColor;
                    SelectionColor = foreColor;
                    AppendText(str.TrimEnd());
                    AppendText("\r\n");
                }
            }
            catch { }
        }

        public void HandleLogEvent(object sender, LogEventArgs e)
        {
            if (OnlyThreadIds.Count > 0)
                if (!OnlyThreadIds.Contains(e.ThreadId))
                    return;

            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(() => { HandleLogEvent(sender, e); }));
            else
                lock (m_oHandlingLogEvent)
                    try
                    {
                        int iRestoreSelStart = SelectionStart;
                        int iRestoreSelLen = SelectionLength;

                        Func<string, string> prefix = new Func<string, string>((spfx) =>
                         {
                             return string.Format("{0}{1}{2}"
                                                    , ShowTimestamp ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") : string.Empty
                                                    , ShowLevel ? string.Format("[{0}] ", spfx) : string.Empty
                                                    , ShowThread ? string.Format("[Thd:{0}/{1}] ", e.ThreadId, e.ThreadName) : string.Empty
                                                 ).TrimStart();
                         });

                        switch (e.Type)
                        {
                            case LogEventArgs.LogEventType.Debug:
                                if (!Mute.Debug) AppendLine(string.Format("{0} {1} {2}", prefix("D"), e.String, ShowWhence.Debug ? Utility.Whence(e.Caller, e.File, e.LineNumber) : string.Empty), LevelColoration.Debug.Foreground, LevelColoration.Debug.Background);
                                break;

                            case LogEventArgs.LogEventType.Error:
                                if (!Mute.Error) AppendLine(string.Format("{0} {1} {2}", prefix("E"), e.String, ShowWhence.Error ? Utility.Whence(e.Caller, e.File, e.LineNumber) : string.Empty), LevelColoration.Error.Foreground, LevelColoration.Error.Background);
                                break;

                            case LogEventArgs.LogEventType.Exception:
                                if (!Mute.Exception)
                                {
                                    AppendLine(string.Format("{0} {1} {2}", prefix("X"), e.String, ShowWhence.Exception ? Utility.Whence(e.Caller, e.File, e.LineNumber) : string.Empty), LevelColoration.Exception.Foreground, LevelColoration.Exception.Background);

                                    string strIndent = "     ";
                                    for (Exception ex = e.Exception; null != ex; ex = ex.InnerException)
                                    {
                                        AppendLine(string.Format("{0} {1}Exception : {2}", prefix("X"), strIndent, ex.Message), LevelColoration.Exception.Foreground, LevelColoration.Exception.Background);
                                        AppendLine(string.Format("{0} {1}    Stack : {2}", prefix("X"), strIndent, ex.StackTrace), LevelColoration.Exception.Foreground, LevelColoration.Exception.Background);
                                        AppendLine(string.Format("{0} {1}   Source : {2}", prefix("X"), strIndent, ex.Source), LevelColoration.Exception.Foreground, LevelColoration.Exception.Background);
                                        strIndent += "     ";
                                    }
                                }
                                break;

                            case LogEventArgs.LogEventType.Warning:
                                if (!Mute.Warning) AppendLine(string.Format("{0} {1} {2}", prefix("W"), e.String, ShowWhence.Warning ? Utility.Whence(e.Caller, e.File, e.LineNumber) : string.Empty), LevelColoration.Warning.Foreground, LevelColoration.Warning.Background);
                                break;

                            default:
                                if (!Mute.Info) AppendLine(string.Format("{0} {1} {2}", prefix("I"), e.String, ShowWhence.Info ? Utility.Whence(e.Caller, e.File, e.LineNumber) : string.Empty), LevelColoration.Info.Foreground, LevelColoration.Info.Background);
                                break;
                        }

                        if (ScrollToCaretOnAdd)
                            ScrollToCaret();
                        else
                        {
                            SelectionStart = iRestoreSelStart;
                            SelectionLength = iRestoreSelLen;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
        }
    }
}