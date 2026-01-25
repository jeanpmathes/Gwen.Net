using System;
using System.Collections.Generic;
using System.Text;
using Gwen.Net.Legacy.Renderer;

namespace Gwen.Net.Legacy.RichText.KnuthPlass
{
    // Knuth and Plass line breaking algorithm
    //
    // Original JavaScript implementation by Bram Stein
    // from https://github.com/bramstein/typeset
    // licensed under the new BSD License.
    internal class LineBreaker : RichText.LineBreaker
    {
        public const Int32 Infinity = 10000;

        public const Int32 DemeritsLine = 10;
        public const Int32 DemeritsFlagged = 100;
        public const Int32 DemeritsFitness = 3000;

        private readonly LinkedList<BreakPoint> activeNodes = new();

        private readonly Formatter formatter;

        private List<Node> nodes;

        private Paragraph paragraph;

        private Sum sum = new(width: 0, stretch: 0, shrink: 0);
        private Int32 tolerance;
        private Int32 totalWidth;

        public LineBreaker(RendererBase renderer, Font defaultFont)
            : base(renderer, defaultFont)
        {
            formatter = new LeftFormatter(renderer, defaultFont);
        }

        public override List<TextBlock> LineBreak(Paragraph currentParagraph, Int32 width)
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

        private Int32 GetLineLength(Int32 currentLine)
        {
            return totalWidth - paragraph.Margin.Left - paragraph.Margin.Right -
                   (currentLine == 1 ? paragraph.FirstIndent : paragraph.RemainigIndent);
        }

        private Single ComputeCost(Int32 end, Sum activeTotals, Int32 currentLine)
        {
            Int32 width = sum.Width - activeTotals.Width;

            Int32 lineLength = GetLineLength(currentLine);

            if (nodes[end].Type == NodeType.Penalty)
            {
                width += nodes[end].Width;
            }

            if (width < lineLength)
            {
                Int32 stretch = sum.Stretch - activeTotals.Stretch;

                if (stretch > 0)
                {
                    return (Single) (lineLength - width) / stretch;
                }

                return Infinity;
            }

            if (width > lineLength)
            {
                Int32 shrink = sum.Shrink - activeTotals.Shrink;

                if (shrink > 0)
                {
                    return (Single) (lineLength - width) / shrink;
                }

                return Infinity;
            }

            return 0.0f;
        }

        private Sum ComputeSum(Int32 breakPointIndex)
        {
            Sum result = new(sum.Width, sum.Stretch, sum.Shrink);

            for (Int32 i = breakPointIndex; i < nodes.Count; i++)
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

        private void MainLoop(Int32 index)
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
                    Int32 currentLine = active.Value.Line + 1;
                    Single ratio = ComputeCost(index, active.Value.Totals, currentLine);

                    if (ratio < -1 || (node.Type == NodeType.Penalty && ((PenaltyNode) node).Penalty == -Infinity))
                    {
                        activeNodes.Remove(active);
                    }

                    if (-1 <= ratio && ratio <= tolerance)
                    {
                        var badness = (Int32) (100.0f * Math.Pow(Math.Abs(ratio), y: 3));

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

                Int32 fitnessClass;

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

        private List<TextBlock> DoLineBreak(Paragraph currentParagraph, Formatter currentFormatter, Int32 width, Int32 currentTolerance)
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

                for (Int32 i = breaks.Count - 2; i >= 0; i--)
                {
                    Int32 position = breaks[i].Position;

                    for (Int32 j = lineStart; j < nodes.Count; j++)
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

                    for (Int32 nodeIndex = lineStart; nodeIndex <= position; nodeIndex++)
                    {
                        if (nodes[nodeIndex].Type == NodeType.Box)
                        {
                            height = Math.Max(height, ((BoxNode) nodes[nodeIndex]).Height);

                            baseline = Math.Max(
                                baseline,
                                (Int32) ((TextPart) ((BoxNode) nodes[nodeIndex]).Part).Font.FontMetrics.Baseline);
                        }
                    }

                    Part part = ((BoxNode) nodes[lineStart]).Part;
                    Int32 blockStart = lineStart;

                    for (Int32 nodeIndex = lineStart; nodeIndex <= position; nodeIndex++)
                    {
                        if ((nodes[nodeIndex].Type == NodeType.Box && ((BoxNode) nodes[nodeIndex]).Part != part) ||
                            nodeIndex == position)
                        {
                            TextBlock textBlock = new();
                            textBlock.Part = part;
                            str.Clear();

                            for (Int32 k = blockStart; k < nodeIndex - 1; k++)
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
                                y + baseline - (Int32) ((TextPart) part).Font.FontMetrics.Baseline);

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
            public Int32 Demerits { get; set; }
            public Single Ratio { get; set; }
            
            public override String ToString()
            {
                return $"Candidate: Demerits = {Demerits} Ratio = {Ratio} Active = {Active.Value.ToString()}";
            }
        }

        private struct Break
        {
            public Int32 Position { get; }
            public Single Ratio { get; }

            public Break(Int32 position, Single ratio)
            {
                Position = position;
                Ratio = ratio;
            }
        }

        private struct Sum
        {
            public Int32 Width { get; set; }
            public Int32 Stretch { get; set; }
            public Int32 Shrink { get; set; }

            public Sum(Int32 width, Int32 stretch, Int32 shrink)
            {
                Width = width;
                Stretch = stretch;
                Shrink = shrink;
            }
        }

        private struct BreakPoint
        {
            public Int32 Position { get; }
            public Int32 Demerits { get; }
            public Single Ratio { get; }
            public Int32 Line { get; }
            public Int32 FitnessClass { get; }
            public Sum Totals { get; set; }
            public LinkedListNode<BreakPoint> Previous { get; }

            public BreakPoint(Int32 position, Int32 demerits, Single ratio, Int32 line, Int32 fitnessClass, Sum totals,
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
        }
    }
}
