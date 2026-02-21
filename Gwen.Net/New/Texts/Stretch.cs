using System;

namespace Gwen.Net.New.Texts;

/// <summary>
/// The stretch degree of a font.
/// </summary>
public enum Stretch : Byte
{
    /// <summary>
    /// The font is ultra-condensed.
    /// </summary>
    UltraCondensed,
    
    /// <summary>
    /// The font is extra-condensed.
    /// </summary>
    ExtraCondensed,
    
    /// <summary>
    /// The font is condensed.
    /// </summary>
    Condensed,
    
    /// <summary>
    /// The font is semi-condensed.
    /// </summary>
    SemiCondensed,
    
    /// <summary>
    /// The font is normal (not condensed or expanded).
    /// </summary>
    Normal,
    
    /// <summary>
    /// The font is semi-expanded.
    /// </summary>
    SemiExpanded,
    
    /// <summary>
    /// The font is expanded.
    /// </summary>
    Expanded,
    
    /// <summary>
    /// The font is extra-expanded.
    /// </summary>
    ExtraExpanded,
    
    /// <summary>
    /// The font is ultra-expanded.
    /// </summary>
    UltraExpanded
}
