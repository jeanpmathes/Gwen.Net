using System;

namespace Gwen.Net
{
    public struct Rectangle
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int Left => X;
        public int Top => Y;
        public int Right => X + Width - 1;
        public int Bottom => Y + Height - 1;

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

        public Rectangle(int x, int y, int width, int height)
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

        public Rectangle(int x, int y, Size size)
        {
            X = x;
            Y = y;
            Width = size.Width;
            Height = size.Height;
        }

        public Rectangle(Point point, int width, int height)
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

        public static bool operator ==(Rectangle rect1, Rectangle rect2)
        {
            return rect1.X == rect2.X && rect1.Y == rect2.Y && rect1.Width == rect2.Width &&
                   rect1.Height == rect2.Height;
        }

        public static bool operator !=(Rectangle rect1, Rectangle rect2)
        {
            return rect1.X != rect2.X || rect1.Y != rect2.Y || rect1.Width != rect2.Width ||
                   rect1.Height != rect2.Height;
        }

        public override bool Equals(object obj)
        {
            if (obj is Rectangle)
            {
                return (Rectangle)obj == this;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (X | (Y << 16)) ^ (Width | (Height << 16));
        }

        public override string ToString()
        {
            return String.Format("X = {0} Y = {1} Width = {2} Height = {3}", X, Y, Width, Height);
        }

        public static readonly Rectangle Empty = new(x: 0, y: 0, width: 0, height: 0);
    }
}