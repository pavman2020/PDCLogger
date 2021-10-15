namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            PDCLogger.Flags<PDCLogger.RTBColors> flags_11 = new PDCLogger.Flags<PDCLogger.RTBColors>();
            PDCLogger.RTBColors rtbColors1 = new PDCLogger.RTBColors();
            PDCLogger.RTBColors rtbColors2 = new PDCLogger.RTBColors();
            PDCLogger.RTBColors rtbColors3 = new PDCLogger.RTBColors();
            PDCLogger.RTBColors rtbColors4 = new PDCLogger.RTBColors();
            PDCLogger.RTBColors rtbColors5 = new PDCLogger.RTBColors();
            PDCLogger.Flags<bool> flags_12 = new PDCLogger.Flags<bool>();
            this.loggerRichTextBox1 = new PDCLogger.LoggerRichTextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // loggerRichTextBox1
            // 
            this.loggerRichTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            rtbColors1.Background = System.Drawing.Color.DodgerBlue;
            rtbColors1.Foreground = System.Drawing.Color.Indigo;
            flags_11.Debug = rtbColors1;
            rtbColors2.Background = System.Drawing.Color.Red;
            rtbColors2.Foreground = System.Drawing.Color.White;
            flags_11.Error = rtbColors2;
            rtbColors3.Background = System.Drawing.Color.Red;
            rtbColors3.Foreground = System.Drawing.Color.Yellow;
            flags_11.Exception = rtbColors3;
            rtbColors4.Background = System.Drawing.Color.White;
            rtbColors4.Foreground = System.Drawing.Color.Black;
            flags_11.Info = rtbColors4;
            rtbColors5.Background = System.Drawing.Color.Yellow;
            rtbColors5.Foreground = System.Drawing.Color.Black;
            flags_11.Warning = rtbColors5;
            this.loggerRichTextBox1.LevelColoration = flags_11;
            this.loggerRichTextBox1.Location = new System.Drawing.Point(13, 13);
            flags_12.Debug = false;
            flags_12.Error = false;
            flags_12.Exception = false;
            flags_12.Info = false;
            flags_12.Warning = false;
            this.loggerRichTextBox1.Mute = flags_12;
            this.loggerRichTextBox1.Name = "loggerRichTextBox1";
            this.loggerRichTextBox1.Size = new System.Drawing.Size(946, 406);
            this.loggerRichTextBox1.TabIndex = 0;
            this.loggerRichTextBox1.Text = "";
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(971, 431);
            this.Controls.Add(this.loggerRichTextBox1);
            this.Name = "Form1";
            this.ResumeLayout(false);

        }

        #endregion


        private PDCLogger.LoggerRichTextBox loggerRichTextBox1;
        private System.Windows.Forms.Timer timer1;
    }
}

