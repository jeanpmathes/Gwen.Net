using System.Collections.Generic;

namespace Gwen.Net.RichText
{
    public class Paragraph
    {
        public Paragraph(Margin margin = new(), int firstIndent = 0, int remainingIndent = 0)
        {
            Margin = margin;
            FirstIndent = firstIndent;
            RemainigIndent = remainingIndent;
        }

        public List<Part> Parts { get; } = new();

        public Margin Margin { get; }

        public int FirstIndent { get; }

        public int RemainigIndent { get; }

        public Paragraph Text(string text)
        {
            Parts.Add(new TextPart(text));

            return this;
        }

        public Paragraph Text(string text, Color color)
        {
            Parts.Add(new TextPart(text, color));

            return this;
        }

        public Paragraph Link(string text, string link, Color? color = null, Color? hoverColor = null,
            Font hoverFont = null)
        {
            Parts.Add(
                color == null
                    ? new LinkPart(text, link)
                    : new LinkPart(text, link, (Color)color, hoverColor, hoverFont));

            return this;
        }

        public Paragraph Font(Font font = null)
        {
            Parts.Add(new FontPart(font));

            return this;
        }

        public Paragraph LineBreak()
        {
            Parts.Add(new LineBreakPart());

            return this;
        }
    }
}