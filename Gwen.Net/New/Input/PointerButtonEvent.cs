using System;
using System.Drawing;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Input;

/// <summary>
/// Event representing a pointer button press or release.
/// </summary>
public sealed class PointerButtonEvent : PointerEvent
{
    /// <summary>
    /// The pointer button associated with this event.
    /// </summary>
    public PointerButton Button { get; }
    
    /// <summary>
    /// Whether the pointer button associated with this event is being pressed (true) or released (false).
    /// </summary>
    public Boolean IsDown { get; }
    
    /// <summary>
    /// The modifier keys that are currently pressed (e.g. Shift, Control, Alt) when this event is fired.
    /// </summary>
    public ModifierKeys Modifiers { get; }
    
    /// <summary>
    /// Creates a new <seealso cref="PointerButtonEvent"/>.
    /// </summary>
    public PointerButtonEvent(Visual source, PointF position, PointerButton button, Boolean isDown, ModifierKeys modifiers) : base(source, position)
    {
        Button = button;
        IsDown = isDown;
        Modifiers = modifiers;
    }
}
