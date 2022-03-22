using System;
using System.Collections.Generic;
using System.Text;
using Gwen.Net.Renderer;

namespace Gwen.Net.RichText.Simple
{
    internal class LineBreaker : RichText.LineBreaker
    {
        public LineBreaker(RendererBase renderer, Font defaultFont)
            : base(renderer, defaultFont) {}

        public override List<TextBlock> LineBreak(Paragraph paragraph, int totalWidth)
        {
            List<Node> nodes = Split(paragraph);

            int lineWidth = totalWidth - paragraph.Margin.Left - paragraph.Margin.Right - paragraph.FirstIndent;

            List<TextBlock> textBlocks = new();

            var lineStart = 0;
            int lineStop;
            var w = 0;
            var y = 0;
            int x = paragraph.FirstIndent;
            var index = 0;

            while (index < nodes.Count)
            {
                w += nodes[index].Size.Width;

                if (w > lineWidth || index == nodes.Count - 1 || nodes[index].IsLineBreak)
                {
                    lineStop = index;

                    if (w > lineWidth)
                    {
                        if (lineStart == index)
                        {
                            index++; // Too long word to fit on the line
                        }
                        else
                        {
                            lineStop--;
                        }
                    }
                    else if (nodes[index].IsLineBreak)
                    {
                        lineStop--;
                        index++;
                    }
                    else
                    {
                        index++;
                    }

                    while (lineStop > lineStart && nodes[lineStop].IsSpace)
                    {
                        lineStop--;
                    }

                    var height = 0;
                    var baseline = 0;

                    for (int i = lineStart; i <= lineStop; i++)
                    {
                        height = Math.Max(height, nodes[i].Size.Height);
                        baseline = Math.Max(baseline, (int) ((TextPart) nodes[i].Part).Font.FontMetrics.Baseline);
                    }

                    StringBuilder str = new(capacity: 1000);
                    Part part = nodes[lineStart].Part;
                    int blockStart = lineStart;

                    for (int i = lineStart; i <= lineStop; i++)
                    {
                        if (i == lineStop || nodes[i + 1].Part != part)
                        {
                            TextBlock textBlock = new();
                            textBlock.Part = part;
                            str.Clear();

                            var h = 0;

                            for (int k = blockStart; k <= i; k++)
                            {
                                if (nodes[k].IsSpace)
                                {
                                    str.Append(value: ' ');
                                }
                                else
                                {
                                    str.Append(nodes[k].Text);
                                    h = Math.Max(h, nodes[k].Size.Height);
                                }
                            }

                            textBlock.Position = new Point(
                                x,
                                y + baseline - (int) ((TextPart) part).Font.FontMetrics.Baseline);

                            textBlock.Text = str.ToString();

                            textBlock.Size = new Size(
                                Renderer.MeasureText(((TextPart) part).Font, textBlock.Text).Width,
                                h);

                            x += textBlock.Size.Width;

                            textBlocks.Add(textBlock);

                            blockStart = i + 1;

                            if (blockStart <= lineStop)
                            {
                                part = nodes[blockStart].Part;
                            }
                        }
                    }

                    while (index < nodes.Count && nodes[index].IsSpace)
                    {
                        index++;
                    }

                    lineStart = index;
                    y += height;
                    x = paragraph.RemainigIndent;
                    w = 0;

                    lineWidth = totalWidth - paragraph.Margin.Left - paragraph.Margin.Right - paragraph.RemainigIndent;
                }
                else
                {
                    index++;
                }
            }

            return textBlocks;
        }

        private List<Node> Split(Paragraph paragraph)
        {
            List<Node> nodes = new();

            Font font = DefaultFont;

            for (var partIndex = 0; partIndex < paragraph.Parts.Count; partIndex++)
            {
                Part part = paragraph.Parts[partIndex];

                string[] words = part.Split(ref font);

                if (font == null)
                {
                    font = DefaultFont;
                }

                for (var wordIndex = 0; wordIndex < words.Length; wordIndex++)
                {
                    string word = words[wordIndex];

                    if (word[index: 0] == ' ')
                    {
                        nodes.Add(new Node(Renderer.MeasureText(font, word), part));
                    }
                    else if (word[index: 0] == '\n')
                    {
                        if (nodes.Count > 0)
                        {
                            if (nodes[nodes.Count - 1].IsLineBreak)
                            {
                                nodes.Add(new Node(" ", Renderer.MeasureText(font, " "), nodes[nodes.Count - 2].Part));
                            }

                            nodes.Add(new Node());
                        }
                    }
                    else
                    {
                        nodes.Add(new Node(word, Renderer.MeasureText(font, word), part));
                    }
                }
            }

            return nodes;
        }

        private class Node
        {
            public readonly Part Part;
            public readonly Size Size;
            public readonly string Text;

            public Node(string text, Size size, Part part)
            {
                Text = text;
                Size = size;
                Part = part;
            }

            public Node(Size size, Part part)
            {
                Text = null;
                Size = size;
                Part = part;
            }

            public Node()
            {
                Text = null;
                Size = Size.Zero;
                Part = null;
            }

            public bool IsSpace => Text == null && Part != null;

            public bool IsLineBreak => Part == null;

#if DEBUG
            public override string ToString()
            {
                if (Part == null)
                {
                    return "Node: LineBreak";
                }

                if (Text == null)
                {
                    return String.Format("Node: Width = {0} Value = Space", Size.Width);
                }

                return String.Format("Node: Width = {0} Value = \"{1}\"", Size.Width, Text);
            }
#endif
        }
    }
}
