using System;

namespace Gwen.Net
{
    public struct Anchor : IEquatable<Anchor>
    {
        public readonly byte Top;
        public readonly byte Bottom;
        public readonly byte Left;
        public readonly byte Right;

        public static Anchor LeftTop = new(left: 0, top: 0, right: 0, bottom: 0);
        public static Anchor RightTop = new(left: 100, top: 0, right: 100, bottom: 0);
        public static Anchor LeftBottom = new(left: 0, top: 100, right: 0, bottom: 100);
        public static Anchor RightBottom = new(left: 100, top: 100, right: 100, bottom: 100);

        public Anchor(byte left, byte top, byte right, byte bottom)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        public bool Equals(Anchor other)
        {
            return other.Top == Top && other.Bottom == Bottom && other.Left == Left && other.Right == Right;
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
            int result = Top;
            result |= Bottom << 8;
            result |= Left << 16;
            result |= Right << 24;

            return result;

        }
    }
}