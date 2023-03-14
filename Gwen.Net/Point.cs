﻿namespace Gwen.Net
{
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static explicit operator Size(Point point)
        {
            return new(point.X, point.Y);
        }

        public static Point operator +(Point pt1, Point pt2)
        {
            return new(pt1.X + pt2.X, pt1.Y + pt2.Y);
        }

        public static Point operator -(Point pt1, Point pt2)
        {
            return new(pt1.X - pt2.X, pt1.Y - pt2.Y);
        }

        public static bool operator ==(Point pt1, Point pt2)
        {
            return pt1.X == pt2.X && pt1.Y == pt2.Y;
        }

        public static bool operator !=(Point pt1, Point pt2)
        {
            return pt1.X != pt2.X || pt1.Y != pt2.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Point)
            {
                return (Point)obj == this;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return X | (Y << 16);
        }

        public override string ToString()
        {
            return $"X = {X} Y = {Y}";
        }

        public static readonly Point Zero = new(x: 0, y: 0);
    }
}
