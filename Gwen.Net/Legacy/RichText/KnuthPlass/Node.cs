using System;

namespace Gwen.Net.Legacy.RichText.KnuthPlass
{
    internal enum NodeType { Box, Glue, Penalty }

    internal abstract class Node
    {
        public Node(NodeType type, Int32 width)
        {
            Type = type;
            Width = width;
        }

        public NodeType Type { get; }

        public Int32 Width { get; }
    }

    internal class BoxNode : Node
    {
        public BoxNode(Int32 width, String value, Part part, Int32 height)
            : base(NodeType.Box, width)
        {
            Value = value;
            Height = height;
            Part = part;
        }

        public String Value { get; }

        public Int32 Height { get; }

        public Part Part { get; }
    }

    internal class GlueNode : Node
    {
        public GlueNode(Int32 width, Int32 stretch, Int32 shrink)
            : base(NodeType.Glue, width)
        {
            Stretch = stretch;
            Shrink = shrink;
        }

        public Int32 Stretch { get; }

        public Int32 Shrink { get; }
    }

    internal class PenaltyNode : Node
    {
        public PenaltyNode(Int32 width, Int32 penalty, Int32 flagged)
            : base(NodeType.Penalty, width)
        {
            Penalty = penalty;
            Flagged = flagged;
        }

        public Int32 Penalty { get; }

        public Int32 Flagged { get; }
    }
}
