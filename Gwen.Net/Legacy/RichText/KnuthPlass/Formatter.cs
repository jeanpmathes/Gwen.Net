using System;
using System.Collections.Generic;
using Gwen.Net.Legacy.Renderer;

namespace Gwen.Net.Legacy.RichText.KnuthPlass
{
    internal abstract class Formatter
    {
        protected readonly Font defaultFont;
        protected readonly RendererBase renderer;

        public Formatter(RendererBase renderer, Font defaultFont)
        {
            this.renderer = renderer;
            this.defaultFont = defaultFont;
        }

        public void MeasureText(Font font, String text, out Int32 width, out Int32 height)
        {
            Size p = renderer.MeasureText(font, text);
            width = p.Width;
            height = p.Height;
        }

        public Size MeasureText(Font font, String text)
        {
            Size size = renderer.MeasureText(font, text);

            return size;
        }

        public abstract List<Node> FormatParagraph(Paragraph paragraph);
    }
}
