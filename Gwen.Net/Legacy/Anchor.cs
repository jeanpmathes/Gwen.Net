using System;

namespace Gwen.Net.Legacy
{
    public readonly struct Anchor : IEquatable<Anchor>
    {
        public readonly Byte top;
        public readonly Byte bottom;
        public readonly Byte left;
        public readonly Byte right;

        public static Anchor LeftTop { get; } = new(left: 0, top: 0, right: 0, bottom: 0);
        public static Anchor RightTop { get; } = new(left: 100, top: 0, right: 100, bottom: 0);
        public static Anchor LeftBottom { get; } = new(left: 0, top: 100, right: 0, bottom: 100);
        public static Anchor RightBottom { get; } = new(left: 100, top: 100, right: 100, bottom: 100);

        public Anchor(Byte left, Byte top, Byte right, Byte bottom)
        {
            this.top = top;
            this.bottom = bottom;
            this.left = left;
            this.right = right;
        }

        public static Boolean operator ==(Anchor lhs, Anchor rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Boolean operator !=(Anchor lhs, Anchor rhs)
        {
            return !lhs.Equals(rhs);
        }
        
        public Boolean Equals(Anchor other)
        {
            return other.top == top && other.bottom == bottom && other.left == left && other.right == right;
        }

        public override Boolean Equals(Object? obj)
        {
            if (ReferenceEquals(objA: null, obj))
            {
                return false;
            }

            return obj is Anchor anchor && Equals(anchor);
        }

        public override Int32 GetHashCode()
        {
            Int32 result = top;
            result |= bottom << 8;
            result |= left << 16;
            result |= right << 24;

            return result;

        }
    }
}
