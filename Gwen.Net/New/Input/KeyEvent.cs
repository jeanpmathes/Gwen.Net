using System;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Input;

/// <summary>
/// A (special) key input event.
/// </summary>
public sealed class KeyEvent : InputEvent
{
    /// <summary>
    /// The key that is the source of this event.
    /// </summary>
    public Key Key { get; }
    
    /// <summary>
    /// Whether the key is being pressed (true) or released (false).
    /// </summary>
    public Boolean IsDown { get; }
    
    /// <summary>
    /// Whether this event is a repeat event (i.e. the key is being held down and this event is being fired repeatedly). This is only true for key down events, and is always false for key up events.
    /// </summary>
    public Boolean IsRepeat { get; }
    
    /// <summary>
    /// Any modifier keys that are currently pressed (e.g. Shift, Control, Alt) when this event is fired.
    /// </summary>
    public ModifierKeys Modifiers { get; }
    
    /// <summary>
    /// Creates a new <seealso cref="KeyEvent"/>.
    /// </summary>
    public KeyEvent(Visual source, Key key, Boolean isDown, Boolean isRepeat, ModifierKeys modifiers) : base(source)
    {
        Key = key;
        IsDown = isDown;
        IsRepeat = isRepeat;
        Modifiers = modifiers;
    }
}
