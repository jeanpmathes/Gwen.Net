using System;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Input;

/// <summary>
/// A text input event.
/// </summary>
public sealed class TextEvent : InputEvent
{
    /// <summary>
    /// The text associated with this event.
    /// </summary>
    public String Text { get; }
    
    /// <summary>
    /// Creates a new <seealso cref="TextEvent"/>.
    /// </summary>
    public TextEvent(Visual source, String text) : base(source)
    {
        Text = text;
    }
}
