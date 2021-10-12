using System.Drawing;
using System.ComponentModel;

namespace MyLogger
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class RTBColors
    {
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color Background { get; set; }

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color Foreground { get; set; }
    }


    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ConsoleColors
    {
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public System.ConsoleColor Background { get; set; }

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public System.ConsoleColor Foreground { get; set; }
    }


}