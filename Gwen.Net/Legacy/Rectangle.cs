using System;

namespace Gwen.Net.Legacy
{
    public struct Rectangle
    {
        public Int32 X { get; set; }
        public Int32 Y { get; set; }
        public Int32 Width { get; set; }
        public Int32 Height { get; set; }

        public Int32 Left => X;
        public Int32 Top => Y;
        public Int32 Right => X + Width - 1;
        public Int32 Bottom => Y + Height - 1;

        public Point Location
        {
            get => new(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public Size Size
        {
            get => new(Width, Height);
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        public Rectangle(Int32 x, Int32 y, Int32 width, Int32 height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rectangle(Point point, Size size)
        {
            X = point.X;
            Y = point.Y;
            Width = size.Width;
            Height = size.Height;
        }

        public Rectangle(Int32 x, Int32 y, Size size)
        {
            X = x;
            Y = y;
            Width = size.Width;
            Height = size.Height;
        }

        public Rectangle(Point point, Int32 width, Int32 height)
        {
            X = point.X;
            Y = point.Y;
            Width = width;
            Height = height;
        }

        public void Inflate(Margin margin)
        {
            X -= margin.Left;
            Y -= margin.Top;
            Width += margin.Left + margin.Right;
            Height += margin.Top + margin.Bottom;
        }

        public void Inflate(Padding padding)
        {
            X -= padding.Left;
            Y -= padding.Top;
            Width += padding.Left + padding.Right;
            Height += padding.Top + padding.Bottom;
        }

        public void Deflate(Margin margin)
        {
            X += margin.Left;
            Y += margin.Top;
            Width -= margin.Left + margin.Right;
            Height -= margin.Top + margin.Bottom;
        }

        public void Deflate(Padding padding)
        {
            X += padding.Left;
            Y += padding.Top;
            Width -= padding.Left + padding.Right;
            Height -= padding.Top + padding.Bottom;
        }

        public void Offset(Margin margin)
        {
            X += margin.Left;
            Y += margin.Top;
        }

        public void Offset(Padding padding)
        {
            X += padding.Left;
            Y += padding.Top;
        }

        public static Boolean operator ==(Rectangle rect1, Rectangle rect2)
        {
            return rect1.X == rect2.X && rect1.Y == rect2.Y && rect1.Width == rect2.Width &&
                   rect1.Height == rect2.Height;
        }

        public static Boolean operator !=(Rectangle rect1, Rectangle rect2)
        {
            return rect1.X != rect2.X || rect1.Y != rect2.Y || rect1.Width != rect2.Width ||
                   rect1.Height != rect2.Height;
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is Rectangle)
            {
                return (Rectangle)obj == this;
            }

            return false;
        }

        public override Int32 GetHashCode()
        {
            return (X | (Y << 16)) ^ (Width | (Height << 16));
        }

        public override String ToString()
        {
            return $"X = {X} Y = {Y} Width = {Width} Height = {Height}";
        }

        public static readonly Rectangle Empty = new(x: 0, y: 0, width: 0, height: 0);
    }
}
