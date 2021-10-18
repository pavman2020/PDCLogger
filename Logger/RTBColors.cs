using System.Drawing;
using System.ComponentModel;

namespace PDCLogger
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class RTBColors
    {
        /// <summary>
        /// The background color of the Rich Text Box text
        /// </summary>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color Background { get; set; }

        /// <summary>
        /// The foreground color of the Rich Text Box text
        /// </summary>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color Foreground { get; set; }
    }


    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ConsoleColors
    {
        /// <summary>
        /// the background color of the Console text
        /// </summary>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public System.ConsoleColor Background { get; set; }

        /// <summary>
        /// the foreground color of the Console text
        /// </summary>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public System.ConsoleColor Foreground { get; set; }
    }


}