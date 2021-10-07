using System;

namespace Gwen.Net.Control
{
    public class ClickedEventArgs : EventArgs
    {
        internal ClickedEventArgs(int x, int y, bool down)
        {
            X = x;
            Y = y;
            MouseDown = down;
        }

        public int X { get; }
        public int Y { get; }
        public bool MouseDown { get; }
    }
}