using System;
using System.Collections.Generic;
using System.Text;
using Gwen.Net.Renderer;

namespace Gwen.Net.RichText.KnuthPlass
{
    // Knuth and Plass line breaking algorithm
    //
    // Original JavaScript implementation by Bram Stein
    // from https://github.com/bramstein/typeset
    // licensed under the new BSD License.
    internal class LineBreaker : RichText.LineBreaker
    {
        public const int Infinity = 10000;

        public const int DemeritsLine = 10;
        public const int DemeritsFlagged = 100;
        public const int DemeritsFitness = 3000;

        private readonly LinkedList<BreakPoint> activeNodes = new();

        private readonly Formatter formatter;

        private List<Node> nodes;

        private Paragraph paragraph;

        private Sum sum = new(width: 0, stretch: 0, shrink: 0);
        private int tolerance;
        private int totalWidth;

        public LineBreaker(RendererBase renderer, Font defaultFont)
            : base(renderer, defaultFont)
        {
            formatter = new LeftFormatter(renderer, defaultFont);
        }

        public override List<TextBlock> LineBreak(Paragraph currentParagraph, int width)
        {
            List<TextBlock> textBlocks = null;

            // Todo: Find out why tolerance needs to be quite high sometimes, depending on the line width.
            // Maybe words need to be hyphenated or there is still a bug somewhere in the code.
            for (var currentTolerance = 4; currentTolerance < 30; currentTolerance += 2)
            {
                textBlocks = DoLineBreak(currentParagraph, formatter, width, currentTolerance);

                if (textBlocks != null)
                {
                    break;
                }
            }

            return textBlocks;
        }

        private int GetLineLength(int currentLine)
        {
            return totalWidth - paragraph.Margin.Left - paragraph.Margin.Right -
                   (currentLine == 1 ? paragraph.FirstIndent : paragraph.RemainigIndent);
        }

        private float ComputeCost(int end, Sum activeTotals, int currentLine)
        {
            int width = sum.Width - activeTotals.Width;

            int lineLength = GetLineLength(currentLine);

            if (nodes[end].Type == NodeType.Penalty)
            {
                width += nodes[end].Width;
            }

            if (width < lineLength)
            {
                int stretch = sum.Stretch - activeTotals.Stretch;

                if (stretch > 0)
                {
                    return (float) (lineLength - width) / stretch;
                }

                return Infinity;
            }

            if (width > lineLength)
            {
                int shrink = sum.Shrink - activeTotals.Shrink;

                if (shrink > 0)
                {
                    return (float) (lineLength - width) / shrink;
                }

                return Infinity;
            }

            return 0.0f;
        }

        private Sum ComputeSum(int breakPointIndex)
        {
            Sum result = new(sum.Width, sum.Stretch, sum.Shrink);

            for (int i = breakPointIndex; i < nodes.Count; i++)
            {
                if (nodes[i].Type == NodeType.Glue)
                {
                    result.Width += nodes[i].Width;
                    result.Stretch += ((GlueNode) nodes[i]).Stretch;
                    result.Shrink += ((GlueNode) nodes[i]).Shrink;
                }
                else if (nodes[i].Type == NodeType.Box || (nodes[i].Type == NodeType.Penalty &&
                                                             ((PenaltyNode) nodes[i]).Penalty == -Infinity &&
                                                             i > breakPointIndex))
                {
                    break;
                }
            }

            return result;
        }

        private void MainLoop(int index)
        {
            Node node = nodes[index];

            LinkedListNode<BreakPoint> active = activeNodes.First;
            var candidates = new Candidate[4];

            while (active != null)
            {
                candidates[0].Demerits = Infinity;
                candidates[1].Demerits = Infinity;
                candidates[2].Demerits = Infinity;
                candidates[3].Demerits = Infinity;

                while (active != null)
                {
                    LinkedListNode<BreakPoint> next = active.Next;
                    int currentLine = active.Value.Line + 1;
                    float ratio = ComputeCost(index, active.Value.Totals, currentLine);

                    if (ratio < -1 || (node.Type == NodeType.Penalty && ((PenaltyNode) node).Penalty == -Infinity))
                    {
                        activeNodes.Remove(active);
                    }

                    if (-1 <= ratio && ratio <= tolerance)
                    {
                        var badness = (int) (100.0f * Math.Pow(Math.Abs(ratio), y: 3));

                        var demerits = 0;

                        if (node.Type == NodeType.Penalty && ((PenaltyNode) node).Penalty >= 0)
                        {
                            demerits = ((DemeritsLine + badness) * (DemeritsLine + badness)) +
                                       (((PenaltyNode) node).Penalty * ((PenaltyNode) node).Penalty);
                        }
                        else if (node.Type == NodeType.Penalty && ((PenaltyNode) node).Penalty != -Infinity)
                        {
                            demerits = ((DemeritsLine + badness) * (DemeritsLine + badness)) -
                                       (((PenaltyNode) node).Penalty * ((PenaltyNode) node).Penalty);
                        }
                        else
                        {
                            demerits = (DemeritsLine + badness) * (DemeritsLine + badness);
                        }

                        if (node.Type == NodeType.Penalty && nodes[active.Value.Position].Type == NodeType.Penalty)
                        {
                            demerits += DemeritsFlagged * ((PenaltyNode) node).Flagged *
                                        ((PenaltyNode) nodes[active.Value.Position]).Flagged;
                        }

                        var currentClass = 0;

                        if (ratio < -0.5f)
                        {
                            currentClass = 0;
                        }
                        else if (ratio <= 0.5f)
                        {
                            currentClass = 1;
                        }
                        else if (ratio <= 1.0f)
                        {
                            currentClass = 2;
                        }
                        else
                        {
                            currentClass = 3;
                        }

                        if (Math.Abs(currentClass - active.Value.FitnessClass) > 1)
                        {
                            demerits += DemeritsFitness;
                        }

                        demerits += active.Value.Demerits;

                        if (demerits < candidates[currentClass].Demerits)
                        {
                            candidates[currentClass].Active = active;
                            candidates[currentClass].Demerits = demerits;
                            candidates[currentClass].Ratio = ratio;
                        }
                    }

                    active = next;

                    if (active != null && active.Value.Line >= currentLine)
                    {
                        break;
                    }
                }

                Sum tmpSum = ComputeSum(index);

                int fitnessClass;

                for (fitnessClass = 0; fitnessClass < candidates.Length; fitnessClass++)
                {
                    Candidate candidate = candidates[fitnessClass];

                    if (candidate.Demerits < Infinity)
                    {
                        var newNode = new LinkedListNode<BreakPoint>(
                            new BreakPoint(
                                index,
                                candidate.Demerits,
                                candidate.Ratio,
                                candidate.Active.Value.Line + 1,
                                fitnessClass,
                                tmpSum,
                                candidate.Active));

                        if (active != null)
                        {
                            activeNodes.AddBefore(active, newNode);
                        }
                        else
                        {
                            activeNodes.AddLast(newNode);
                        }
                    }
                }
            }
        }

        private List<TextBlock> DoLineBreak(Paragraph currentParagraph, Formatter currentFormatter, int width, int currentTolerance)
        {
            paragraph = currentParagraph;
            totalWidth = width;
            tolerance = currentTolerance;

            nodes = currentFormatter.FormatParagraph(currentParagraph);

            sum = new Sum(width: 0, stretch: 0, shrink: 0);

            activeNodes.Clear();

            activeNodes.AddLast(
                new BreakPoint(
                    position: 0,
                    demerits: 0,
                    ratio: 0,
                    line: 0,
                    fitnessClass: 0,
                    new Sum(width: 0, stretch: 0, shrink: 0),
                    previous: null));

            for (var index = 0; index < nodes.Count; index++)
            {
                Node node = nodes[index];

                if (node.Type == NodeType.Box)
                {
                    sum.Width += node.Width;
                }
                else if (node.Type == NodeType.Glue)
                {
                    if (index > 0 && nodes[index - 1].Type == NodeType.Box)
                    {
                        MainLoop(index);
                    }

                    sum.Width += node.Width;
                    sum.Stretch += ((GlueNode) node).Stretch;
                    sum.Shrink += ((GlueNode) node).Shrink;
                }
                else if (node.Type == NodeType.Penalty && ((PenaltyNode) node).Penalty != Infinity)
                {
                    MainLoop(index);
                }
            }

            if (activeNodes.Count != 0)
            {
                LinkedListNode<BreakPoint> node = activeNodes.First;
                LinkedListNode<BreakPoint> tmp = null;

                while (node != null)
                {
                    if (tmp == null || node.Value.Demerits < tmp.Value.Demerits)
                    {
                        tmp = node;
                    }

                    node = node.Next;
                }

                List<Break> breaks = new();

                while (tmp != null)
                {
                    breaks.Add(new Break(tmp.Value.Position, tmp.Value.Ratio));
                    tmp = tmp.Value.Previous;
                }

                // breaks.Reverse();

                var lineStart = 0;
                var y = 0;
                var x = 0;
                StringBuilder str = new(capacity: 1000);
                List<TextBlock> textBlocks = new();

                for (int i = breaks.Count - 2; i >= 0; i--)
                {
                    int position = breaks[i].Position;

                    for (int j = lineStart; j < nodes.Count; j++)
                    {
                        if (nodes[j].Type == NodeType.Box || (nodes[j].Type == NodeType.Penalty &&
                                                                ((PenaltyNode) nodes[j]).Penalty == -Infinity))
                        {
                            lineStart = j;

                            break;
                        }
                    }

                    var height = 0;
                    var baseline = 0;

                    for (int nodeIndex = lineStart; nodeIndex <= position; nodeIndex++)
                    {
                        if (nodes[nodeIndex].Type == NodeType.Box)
                        {
                            height = Math.Max(height, ((BoxNode) nodes[nodeIndex]).Height);

                            baseline = Math.Max(
                                baseline,
                                (int) ((TextPart) ((BoxNode) nodes[nodeIndex]).Part).Font.FontMetrics.Baseline);
                        }
                    }

                    Part part = ((BoxNode) nodes[lineStart]).Part;
                    int blockStart = lineStart;

                    for (int nodeIndex = lineStart; nodeIndex <= position; nodeIndex++)
                    {
                        if ((nodes[nodeIndex].Type == NodeType.Box && ((BoxNode) nodes[nodeIndex]).Part != part) ||
                            nodeIndex == position)
                        {
                            TextBlock textBlock = new();
                            textBlock.Part = part;
                            str.Clear();

                            for (int k = blockStart; k < nodeIndex - 1; k++)
                            {
                                if (nodes[k].Type == NodeType.Glue)
                                {
                                    if (nodes[k].Width > 0)
                                    {
                                        str.Append(value: ' ');
                                    }
                                }
                                else if (nodes[k].Type == NodeType.Box)
                                {
                                    str.Append(((BoxNode) nodes[k]).Value);
                                }
                            }

                            textBlock.Position = new Point(
                                x,
                                y + baseline - (int) ((TextPart) part).Font.FontMetrics.Baseline);

                            textBlock.Text = str.ToString();

                            textBlock.Size = new Size(
                                currentFormatter.MeasureText(((TextPart) part).Font, textBlock.Text).Width,
                                height);

                            x += textBlock.Size.Width;

                            textBlocks.Add(textBlock);

                            if (nodes[nodeIndex].Type == NodeType.Box)
                            {
                                part = ((BoxNode) nodes[nodeIndex]).Part;
                            }

                            blockStart = nodeIndex;
                        }
                    }

                    x = 0;
                    y += height;

                    lineStart = position;
                }

                return textBlocks;
            }

            return null;
        }

        private struct Candidate
        {
            public LinkedListNode<BreakPoint> Active { get; set; }
            public int Demerits { get; set; }
            public float Ratio { get; set; }
            
            public override string ToString()
            {
                return $"Candidate: Demerits = {Demerits} Ratio = {Ratio} Active = {Active.Value.ToString()}";
            }
        }

        private struct Break
        {
            public int Position { get; }
            public float Ratio { get; }

            public Break(int position, float ratio)
            {
                Position = position;
                Ratio = ratio;
            }
        }

        private struct Sum
        {
            public int Width { get; set; }
            public int Stretch { get; set; }
            public int Shrink { get; set; }

            public Sum(int width, int stretch, int shrink)
            {
                Width = width;
                Stretch = stretch;
                Shrink = shrink;
            }

#if DEBUG
            public override string ToString()
            {
                return $"Sum: Width = {Width} Stretch = {Stretch} Shrink = {Shrink}";
            }
#endif
        }

        private struct BreakPoint
        {
            public int Position { get; }
            public int Demerits { get; }
            public float Ratio { get; }
            public int Line { get; }
            public int FitnessClass { get; }
            public Sum Totals { get; set; }
            public LinkedListNode<BreakPoint> Previous { get; }

            public BreakPoint(int position, int demerits, float ratio, int line, int fitnessClass, Sum totals,
                LinkedListNode<BreakPoint> previous)
            {
                Position = position;
                Demerits = demerits;
                Ratio = ratio;
                Line = line;
                FitnessClass = fitnessClass;
                Totals = totals;
                Previous = previous;
            }

#if DEBUG
            public override string ToString()
            {
                return $"BreakPoint: Position = {Position} Demerits = {Demerits} Ratio = {Ratio} Line = {Line} FitnessClass = {FitnessClass} Totals = {{{Totals.ToString()}}} Previous = {{{(Previous != null ? Previous.Value.ToString() : "Null")}}}";
            }
#endif
        }
    }
}
