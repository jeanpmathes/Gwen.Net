using System;
using System.Collections.Generic;

namespace Gwen.Net.Legacy.RichText
{
    public class Paragraph
    {
        public Paragraph(Margin margin = new(), Int32 firstIndent = 0, Int32 remainingIndent = 0)
        {
            Margin = margin;
            FirstIndent = firstIndent;
            RemainigIndent = remainingIndent;
        }

        public List<Part> Parts { get; } = new();

        public Margin Margin { get; }

        public Int32 FirstIndent { get; }

        public Int32 RemainigIndent { get; }

        public Paragraph Text(String text)
        {
            Parts.Add(new TextPart(text));

            return this;
        }

        public Paragraph Text(String text, Color color)
        {
            Parts.Add(new TextPart(text, color));

            return this;
        }

        public Paragraph Link(String text, String link, Color? color = null, Color? hoverColor = null,
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