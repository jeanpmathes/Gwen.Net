using System;
using System.Drawing;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Input;

/// <summary>
/// A scroll input event.
/// </summary>
public sealed class ScrollEvent : PointerEvent
{
    /// <summary>
    /// The scroll delta along the horizontal axis.
    /// </summary>
    public Single DeltaX { get; }
    
    /// <summary>
    /// The scroll delta along the vertical axis.
    /// </summary>
    public Single DeltaY { get; }
    
    /// <summary>
    /// Creates a new <seealso cref="ScrollEvent"/>.
    /// </summary>
    public ScrollEvent(Visual source, PointF position, Single deltaX, Single deltaY) : base(source, position)
    {
        DeltaX = deltaX;
        DeltaY = deltaY;
    }
}
