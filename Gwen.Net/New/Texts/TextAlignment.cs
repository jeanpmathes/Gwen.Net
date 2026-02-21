using System;

namespace Gwen.Net.New.Texts;

/// <summary>
/// Specifies the horizontal alignment of text within its layout bounds.
/// </summary>
public enum TextAlignment : Byte
{
    /// <summary>
    /// Text is aligned to the leading edge (left in left-to-right layouts).
    /// </summary>
    Leading,

    /// <summary>
    /// Text is centered within the layout bounds.
    /// </summary>
    Center,

    /// <summary>
    /// Text is aligned to the trailing edge (right in left-to-right layouts).
    /// </summary>
    Trailing,

    /// <summary>
    /// Text is justified to fill the full width of the layout bounds.
    /// </summary>
    Justify
}
