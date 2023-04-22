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
    }
}
