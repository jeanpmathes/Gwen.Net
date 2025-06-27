using System;
using Gwen.Net.Control;

namespace Gwen.Net.DragDrop
{
    public class Package
    {
        public ControlBase DrawControl { get; set; }
        public Point HoldOffset { get; set; }
        public Boolean IsDraggable { get; set; }
        public String Name { get; set; }
        public Object UserData { get; set; }
    }
}
