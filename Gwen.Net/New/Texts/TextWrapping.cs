using System;

namespace Gwen.Net.New.Texts;

/// <summary>
/// Specifies how text wraps when it exceeds the width of its layout bounds.
/// </summary>
public enum TextWrapping : Byte
{
    /// <summary>
    /// Text does not wrap; it overflows beyond the layout bounds.
    /// </summary>
    NoWrap,

    /// <summary>
    /// Text wraps at word boundaries when it exceeds the layout width.
    /// </summary>
    Wrap
}
