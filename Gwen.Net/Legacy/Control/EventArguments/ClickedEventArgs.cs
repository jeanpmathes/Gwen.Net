using System;

namespace Gwen.Net.Legacy.Control
{
    public class ClickedEventArgs : EventArgs
    {
        internal ClickedEventArgs(Int32 x, Int32 y, Boolean down)
        {
            X = x;
            Y = y;
            MouseDown = down;
        }

        public Int32 X { get; }
        public Int32 Y { get; }
        public Boolean MouseDown { get; }
    }
}