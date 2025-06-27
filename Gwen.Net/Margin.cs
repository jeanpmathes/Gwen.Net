using System;

namespace Gwen.Net
{
    /// <summary>
    ///     Represents outer spacing.
    /// </summary>
    public struct Margin : IEquatable<Margin>
    {
        public Int32 Top { get; set; }
        public Int32 Bottom { get; set; }
        public Int32 Left { get; set; }
        public Int32 Right { get; set; }

        // common values
        public static Margin Zero { get; } = new(margin: 0);
        public static Margin One { get; } = new(margin: 1);
        public static Margin Two { get; } = new(margin: 2);
        public static Margin Three { get; } = new(margin: 3);
        public static Margin Four { get; } = new(margin: 4);
        public static Margin Five { get; } = new(margin: 5);
        public static Margin Six { get; } = new(margin: 6);
        public static Margin Seven { get; } = new(margin: 7);
        public static Margin Eight { get; } = new(margin: 8);
        public static Margin Nine { get; } = new(margin: 9);
        public static Margin Ten { get; } = new(margin: 10);

        public Margin(Int32 left, Int32 top, Int32 right, Int32 bottom)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        public Margin(Int32 horizontal, Int32 vertical)
        {
            Top = vertical;
            Bottom = vertical;
            Left = horizontal;
            Right = horizontal;
        }

        public Margin(Int32 margin)
        {
            Top = margin;
            Bottom = margin;
            Left = margin;
            Right = margin;
        }

        public Boolean Equals(Margin other)
        {
            return other.Top == Top && other.Bottom == Bottom && other.Left == Left && other.Right == Right;
        }

        public static Boolean operator ==(Margin lhs, Margin rhs)
        {
            return lhs.Equals(rhs);
        }

        public static Boolean operator !=(Margin lhs, Margin rhs)
        {
            return !lhs.Equals(rhs);
        }

        public static Margin operator +(Margin lhs, Margin rhs)
        {
            return new(lhs.Left + rhs.Left, lhs.Top + rhs.Top, lhs.Right + rhs.Right, lhs.Bottom + rhs.Bottom);
        }

        public static Margin operator -(Margin lhs, Margin rhs)
        {
            return new(lhs.Left - rhs.Left, lhs.Top - rhs.Top, lhs.Right - rhs.Right, lhs.Bottom - rhs.Bottom);
        }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(objA: null, obj))
            {
                return false;
            }

            if (obj.GetType() != typeof(Margin))
            {
                return false;
            }

            return Equals((Margin)obj);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                Int32 result = Top;
                result = (result * 397) ^ Bottom;
                result = (result * 397) ^ Left;
                result = (result * 397) ^ Right;

                return result;
            }
        }

        public static explicit operator Margin(Padding padding)
        {
            return new(padding.Left, padding.Top, padding.Right, padding.Bottom);
        }

        public override String ToString()
        {
            return $"Left = {Left} Top = {Top} Right = {Right} Bottom = {Bottom}";
        }
    }
}
