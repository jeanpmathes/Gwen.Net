using System;

namespace Gwen.Net
{
    public struct Size
    {
        public Int32 Width { get; set; }
        public Int32 Height { get; set; }

        public Size(Int32 width, Int32 height)
        {
            Width = width;
            Height = height;
        }

        public Size(Int32 size)
        {
            Width = size;
            Height = size;
        }

        public static Size Min(Size sz1, Size sz2)
        {
            return new(Math.Min(sz1.Width, sz2.Width), Math.Min(sz1.Height, sz2.Height));
        }

        public static Size Max(Size sz1, Size sz2)
        {
            return new(Math.Max(sz1.Width, sz2.Width), Math.Max(sz1.Height, sz2.Height));
        }

        public static explicit operator Point(Size size)
        {
            return new(size.Width, size.Height);
        }

        public static Size operator +(Size sz1, Size sz2)
        {
            return new(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }

        public static Size operator -(Size sz1, Size sz2)
        {
            return new(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
        }

        public static Boolean operator ==(Size sz1, Size sz2)
        {
            return sz1.Width == sz2.Width && sz1.Height == sz2.Height;
        }

        public static Boolean operator !=(Size sz1, Size sz2)
        {
            return sz1.Width != sz2.Width || sz1.Height != sz2.Height;
        }

        public static Size operator -(Size size, Margin margin)
        {
            return new(size.Width - margin.Left - margin.Right, size.Height - margin.Top - margin.Bottom);
        }

        public static Size operator +(Size size, Margin margin)
        {
            return new(size.Width + margin.Left + margin.Right, size.Height + margin.Top + margin.Bottom);
        }

        public static Size operator +(Size size, Padding padding)
        {
            return new(size.Width + padding.Left + padding.Right, size.Height + padding.Top + padding.Bottom);
        }

        public static Size operator -(Size size, Padding padding)
        {
            return new(size.Width - padding.Left - padding.Right, size.Height - padding.Top - padding.Bottom);
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is Size)
            {
                return (Size)obj == this;
            }

            return false;
        }

        public override Int32 GetHashCode()
        {
            return Width | (Height << 16);
        }

        public override String ToString()
        {
            return $"Width = {Width} Height = {Height}";
        }

        public static readonly Size Zero = new(width: 0, height: 0);
        public static readonly Size One = new(width: 1, height: 1);
        public static readonly Size Infinity = new(Util.Infinity, Util.Infinity);
    }
}
