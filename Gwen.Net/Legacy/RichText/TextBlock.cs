using System;

namespace Gwen.Net.Legacy.RichText
{
    internal class TextBlock
    {
        public Part Part { get; set; }
        public Point Position { get; set; }
        public Size Size { get; set; }
        public String Text { get; set; }
    }
}
