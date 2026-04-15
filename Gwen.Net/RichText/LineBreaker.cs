using System;
using System.Collections.Generic;
using Gwen.Net.Renderer;

namespace Gwen.Net.RichText
{
    internal abstract class LineBreaker
    {
        public LineBreaker(RendererBase renderer, Font defaultFont)
        {
            Renderer = renderer;
            DefaultFont = defaultFont;
        }

        public RendererBase Renderer { get; }

        public Font DefaultFont { get; }

        public abstract List<TextBlock> LineBreak(Paragraph currentParagraph, Int32 width);
    }
}
