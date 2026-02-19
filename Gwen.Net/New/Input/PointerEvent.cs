using System;
using System.Drawing;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Input;

/// <summary>
/// Event representing pointer input.
/// </summary>
public abstract class PointerEvent : InputEvent
{
    /// <summary>
    /// The position in root canvas coordinates where the pointer event occurred.
    /// </summary>
    public PointF RootPosition { get; }

    /// <summary>
    /// The position in the local coordinate space of the current target visual.
    /// </summary>
    public PointF LocalPosition => Target.RootPointToLocal(RootPosition);
    
    /// <summary>
    /// Creates a new <seealso cref="PointerEvent"/>.
    /// </summary>
    protected PointerEvent(Visual source, PointF position) : base(source)
    {
        RootPosition = position;
    }
}
