using System;

namespace Gwen.Net
{
    public readonly struct Anchor : IEquatable<Anchor>
    {
        public readonly byte top;
        public readonly byte bottom;
        public readonly byte left;
        public readonly byte right;

        public static Anchor LeftTop { get; } = new(left: 0, top: 0, right: 0, bottom: 0);
        public static Anchor RightTop { get; } = new(left: 100, top: 0, right: 100, bottom: 0);
        public static Anchor LeftBottom { get; } = new(left: 0, top: 100, right: 0, bottom: 100);
        public static Anchor RightBottom { get; } = new(left: 100, top: 100, right: 100, bottom: 100);

        public Anchor(byte left, byte top, byte right, byte bottom)
        {
            this.top = top;
            this.bottom = bottom;
            this.left = left;
            this.right = right;
        }

        public bool Equals(Anchor other)
        {
            return other.top == top && other.bottom == bottom && other.left == left && other.right == right;
        }

        public static bool operator ==(Anchor lhs, Anchor rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Anchor lhs, Anchor rhs)
        {
            return !lhs.Equals(rhs);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(objA: null, obj))
            {
                return false;
            }

            if (obj.GetType() != typeof(Anchor))
            {
                return false;
            }

            return Equals((Anchor)obj);
        }

        public override int GetHashCode()
        {
            int result = top;
            result |= bottom << 8;
            result |= left << 16;
            result |= right << 24;

            return result;

        }
    }
}
