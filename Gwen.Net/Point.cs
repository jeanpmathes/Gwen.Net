using System;

namespace Gwen.Net
{
    public struct Point
    {
        public Int32 X { get; set; }
        public Int32 Y { get; set; }

        public Point(Int32 x, Int32 y)
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

        public static Boolean operator ==(Point pt1, Point pt2)
        {
            return pt1.X == pt2.X && pt1.Y == pt2.Y;
        }

        public static Boolean operator !=(Point pt1, Point pt2)
        {
            return pt1.X != pt2.X || pt1.Y != pt2.Y;
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is Point)
            {
                return (Point)obj == this;
            }

            return false;
        }

        public override Int32 GetHashCode()
        {
            return X | (Y << 16);
        }

        public override String ToString()
        {
            return $"X = {X} Y = {Y}";
        }

        public static readonly Point Zero = new(x: 0, y: 0);
    }
}
