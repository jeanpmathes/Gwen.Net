using System;
using System.Drawing;
using Gwen.Net.New.Controls;
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
    /// Whether the pointer event hits the specified visual.
    /// </summary>
    /// <param name="visual">The visual to test for hit.</param>
    /// <returns>>True if the pointer event hits the visual; otherwise, false.</returns>
    public Boolean Hits(Visual visual) => visual.Bounds.Contains(visual.RootPointToLocal(RootPosition));
    
    /// <summary>
    /// Whether the pointer event hits the specified control. This checks whether the pointer event hits the visualization of the control.
    /// If the control does not have a visualization, this will return false.
    /// </summary>
    /// <param name="control">The control to test for hit.</param>
    /// <returns>True if the pointer event hits the control's visualization; otherwise, false.</returns>
    public Boolean Hits(Control control) => control.Visualization.GetValue() is {} visual && Hits(visual);
    
    /// <summary>
    /// Creates a new <seealso cref="PointerEvent"/>.
    /// </summary>
    protected PointerEvent(Visual source, PointF position) : base(source)
    {
        RootPosition = position;
    }
}
