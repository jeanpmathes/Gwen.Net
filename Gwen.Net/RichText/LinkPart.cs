namespace Gwen.Net.RichText
{
    public class LinkPart : TextPart
    {
        public LinkPart(string text, string link)
            : base(text)
        {
            Link = link;
        }

        public LinkPart(string text, string link, Color color, Color? hoverColor = null, Font hoverFont = null)
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

        public string Link { get; }

        public Color? HoverColor { get; }

        public Font HoverFont { get; }

        public override string[] Split(ref Font splitFont)
        {
            Font = splitFont;

            return new[]
            {
                Text.Trim()
            };
        }
    }
}
