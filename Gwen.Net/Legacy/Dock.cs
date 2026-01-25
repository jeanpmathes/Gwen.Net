using Gwen.Net.Legacy.Control;

namespace Gwen.Net.Legacy
{
    public enum Dock
    {
        None = ControlBase.InternalFlags.DockNone,
        Left = ControlBase.InternalFlags.DockLeft,
        Top = ControlBase.InternalFlags.DockTop,
        Right = ControlBase.InternalFlags.DockRight,
        Bottom = ControlBase.InternalFlags.DockBottom,
        Fill = ControlBase.InternalFlags.DockFill
    }
}