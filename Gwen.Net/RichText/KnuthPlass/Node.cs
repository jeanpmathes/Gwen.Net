using System;

namespace Gwen.Net.RichText.KnuthPlass
{
    internal enum NodeType { Box, Glue, Penalty }

    internal abstract class Node
    {
        public Node(NodeType type, int width)
        {
            Type = type;
            Width = width;
        }

        public NodeType Type { get; }

        public int Width { get; }
    }

    internal class BoxNode : Node
    {
        public BoxNode(int width, string value, Part part, int height)
            : base(NodeType.Box, width)
        {
            Value = value;
            Height = height;
            Part = part;
        }

        public string Value { get; }

        public int Height { get; }

        public Part Part { get; }

#if DEBUG
        public override string ToString()
        {
            return String.Format("Box: Width = {0} Value = {1}", Width, Value);
        }
#endif
    }

    internal class GlueNode : Node
    {
        public GlueNode(int width, int stretch, int shrink)
            : base(NodeType.Glue, width)
        {
            Stretch = stretch;
            Shrink = shrink;
        }

        public int Stretch { get; }

        public int Shrink { get; }

#if DEBUG
        public override string ToString()
        {
            return String.Format("Glue: Width = {0} Stretch = {1} Shrink = {2}", Width, Stretch, Shrink);
        }
#endif
    }

    internal class PenaltyNode : Node
    {
        public PenaltyNode(int width, int penalty, int flagged)
            : base(NodeType.Penalty, width)
        {
            Penalty = penalty;
            Flagged = flagged;
        }

        public int Penalty { get; }

        public int Flagged { get; }

#if DEBUG
        public override string ToString()
        {
            return String.Format("Penalty: Width = {0} Penalty = {1} Flagged = {2}", Width, Penalty, Flagged);
        }
#endif
    }
}