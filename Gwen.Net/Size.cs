using System;

namespace Gwen.Net
{
    public struct Size
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public Size(int size)
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

        public static bool operator ==(Size sz1, Size sz2)
        {
            return sz1.Width == sz2.Width && sz1.Height == sz2.Height;
        }

        public static bool operator !=(Size sz1, Size sz2)
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

        public override bool Equals(object obj)
        {
            if (obj is Size)
            {
                return (Size)obj == this;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Width | (Height << 16);
        }

        public override string ToString()
        {
            return $"Width = {Width} Height = {Height}";
        }

        public static readonly Size Zero = new(width: 0, height: 0);
        public static readonly Size One = new(width: 1, height: 1);
        public static readonly Size Infinity = new(Util.Infinity, Util.Infinity);
    }
}
