using System;

namespace Gwen.Net.Legacy.RichText
{
    public class LinkPart : TextPart
    {
        public LinkPart(String text, String link)
            : base(text)
        {
            Link = link;
        }

        public LinkPart(String text, String link, Color color, Color? hoverColor = null, Font hoverFont = null)
            : base(text, color)
        {
            Link = link;

            if (hoverColor != null)
            {
                HoverColor = hoverColor;
            }

            if (hoverFont != null)
            {
                HoverFont = hoverFont;
            }
        }

        public String Link { get; }

        public Color? HoverColor { get; }

        public Font HoverFont { get; }

        public override String[] Split(ref Font splitFont)
        {
            Font = splitFont;

            return new[]
            {
                Text.Trim()
            };
        }
    }
}
