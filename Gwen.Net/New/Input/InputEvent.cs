using System;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Input;

/// <summary>
/// Base class for all input events.
/// </summary>
public abstract class InputEvent
{
    /// <summary>
    /// The visual that is the source of this event.
    /// </summary>
    public Visual Source { get; private set; }
    
    /// <summary>
    /// The visual that is currently targeted by this event.
    /// </summary>
    public Visual Target { get; private set; }
    
    /// <summary>
    /// Whether this event has been handled. If true, the event will not be propagated to other visuals.
    /// </summary>
    public Boolean Handled { get; set; }
    
    /// <summary>
    /// Creates a new input event with the specified source visual. The current target is initially set to the source visual.
    /// </summary>
    /// <param name="source">The visual that is the source of this event.</param>
    public InputEvent(Visual source)
    {
        Source = source;
        Target = source;
    }
    
    internal void SetTarget(Visual target)
    {
        Target = target;
    }
}
