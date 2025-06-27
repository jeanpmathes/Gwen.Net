using System;
using System.Collections.Generic;
using Gwen.Net.Renderer;

namespace Gwen.Net.RichText.KnuthPlass
{
    internal class LeftFormatter : Formatter
    {
        public LeftFormatter(RendererBase renderer, Font defaultFont)
            : base(renderer, defaultFont) {}

        public override List<Node> FormatParagraph(Paragraph paragraph)
        {
            List<Node> nodes = new();

            Font font = defaultFont;
            Int32 width, height;

            for (var partIndex = 0; partIndex < paragraph.Parts.Count; partIndex++)
            {
                Part part = paragraph.Parts[partIndex];

                String[] words = part.Split(ref font);

                if (font == null)
                {
                    font = defaultFont;
                }

                for (var wordIndex = 0; wordIndex < words.Length; wordIndex++)
                {
                    String word = words[wordIndex];

                    if (word[index: 0] == ' ')
                    {
                        continue;
                    }

                    MeasureText(font, word, out width, out height);

                    nodes.Add(new BoxNode(width, word, part, height));

                    if (wordIndex < words.Length - 1 || partIndex < paragraph.Parts.Count - 1)
                    {
                        nodes.Add(new GlueNode(width: 0, stretch: 12, shrink: 0));
                        nodes.Add(new PenaltyNode(width: 0, penalty: 0, flagged: 0));
                        MeasureText(font, " ", out width, out height);
                        nodes.Add(new GlueNode(width, stretch: -12, shrink: 0));
                    }
                    else
                    {
                        nodes.Add(new GlueNode(width: 0, LineBreaker.Infinity, shrink: 0));
                        nodes.Add(new PenaltyNode(width: 0, -LineBreaker.Infinity, flagged: 1));
                    }
                }
            }

            return nodes;
        }
    }
}
