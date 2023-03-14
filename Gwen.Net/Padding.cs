using System;

namespace Gwen.Net
{
    /// <summary>
    ///     Represents inner spacing.
    /// </summary>
    public struct Padding : IEquatable<Padding>
    {
        public int Top { get; }
        public int Bottom { get; }
        public int Left{ get; }
        public int Right { get; }

        // common values
        public static Padding Zero { get; } = new(padding: 0);
        public static Padding One { get; } = new(padding: 1);
        public static Padding Two { get; } = new(padding: 2);
        public static Padding Three { get; } = new(padding: 3);
        public static Padding Four { get; } = new(padding: 4);
        public static Padding Five { get; } = new(padding: 5);

        public Padding(int left, int top, int right, int bottom)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        public Padding(int horizontal, int vertical)
        {
            Top = vertical;
            Bottom = vertical;
            Left = horizontal;
            Right = horizontal;
        }

        public Padding(int padding)
        {
            Top = padding;
            Bottom = padding;
            Left = padding;
            Right = padding;
        }

        public bool Equals(Padding other)
        {
            return other.Top == Top && other.Bottom == Bottom && other.Left == Left && other.Right == Right;
        }

        public static bool operator ==(Padding lhs, Padding rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Padding lhs, Padding rhs)
        {
            return !lhs.Equals(rhs);
        }

        public static Padding operator +(Padding lhs, Padding rhs)
        {
            return new(lhs.Left + rhs.Left, lhs.Top + rhs.Top, lhs.Right + rhs.Right, lhs.Bottom + rhs.Bottom);
        }

        public static Padding operator -(Padding lhs, Padding rhs)
        {
            return new(lhs.Left - rhs.Left, lhs.Top - rhs.Top, lhs.Right - rhs.Right, lhs.Bottom - rhs.Bottom);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(objA: null, obj))
            {
                return false;
            }

            if (obj.GetType() != typeof(Padding))
            {
                return false;
            }

            return Equals((Padding)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = Top;
                result = (result * 397) ^ Bottom;
                result = (result * 397) ^ Left;
                result = (result * 397) ^ Right;

                return result;
            }
        }

        public static explicit operator Padding(Margin margin)
        {
            return new(margin.Left, margin.Top, margin.Right, margin.Bottom);
        }

        public override string ToString()
        {
            return $"Left = {Left} Top = {Top} Right = {Right} Bottom = {Bottom}";
        }
    }
}
