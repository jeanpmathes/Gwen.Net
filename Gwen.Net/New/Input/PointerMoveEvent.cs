using System;
using System.Drawing;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Input;

/// <summary>
/// Represents a pointer move event, which occurs when the pointer moves across the canvas.
/// </summary>
public sealed class PointerMoveEvent : PointerEvent
{
    /// <summary>
    /// The change in the pointer's X coordinate since the last pointer move event, in root canvas coordinates.
    /// </summary>
    public Single DeltaX { get; }
    
    /// <summary>
    /// The change in the pointer's Y coordinate since the last pointer move event, in root canvas coordinates.
    /// </summary>
    public Single DeltaY { get; }
    
    /// <summary>
    /// Creates a new <seealso cref="PointerMoveEvent"/> with the specified source visual, pointer position, and change in pointer coordinates since the last pointer move event.
    /// </summary>
    public PointerMoveEvent(Visual source, PointF position, Single deltaX, Single deltaY) : base(source, position)
    {
        DeltaX = deltaX;
        DeltaY = deltaY;
    }
}
