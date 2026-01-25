using System;
using System.Collections.Generic;
using Gwen.Net.Legacy.Renderer;

namespace Gwen.Net.Legacy.RichText
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
