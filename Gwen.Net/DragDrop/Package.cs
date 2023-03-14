using Gwen.Net.Control;

namespace Gwen.Net.DragDrop
{
    public class Package
    {
        public ControlBase DrawControl { get; set; }
        public Point HoldOffset { get; set; }
        public bool IsDraggable { get; set; }
        public string Name { get; set; }
        public object UserData { get; set; }
    }
}
