using System;

namespace Gwen.Net.New.Input;

/// <summary>
/// Modifiers that are applied to a <seealso cref="KeyEvent"/>.
/// </summary>
[Flags]
public enum ModifierKeys
{
    /// <summary>
    /// No modifiers are applied.
    /// </summary>
    None = 0,
    
    /// <summary>
    /// The Control key is pressed.
    /// </summary>
    Control = 1 << 0,
    
    /// <summary>
    /// The Alt key is pressed.
    /// </summary>
    Alt = 1 << 1,
    
    /// <summary>
    /// The Shift key is pressed.
    /// </summary>
    Shift = 1 << 2,
}
