using System;

namespace Gwen.Net.New.Texts;

/// <summary>
/// Specifies how text is trimmed when it overflows its layout bounds.
/// </summary>
public enum TextTrimming : Byte
{
    /// <summary>
    /// Text is not trimmed; it may overflow the layout bounds.
    /// </summary>
    None,

    /// <summary>
    /// Text is clipped at a character boundary with no ellipsis.
    /// </summary>
    Character,

    /// <summary>
    /// Text is clipped at a word boundary with no ellipsis.
    /// </summary>
    Word,

    /// <summary>
    /// Text is trimmed at a character boundary and an ellipsis is appended.
    /// </summary>
    CharacterEllipsis,

    /// <summary>
    /// Text is trimmed at a word boundary and an ellipsis is appended.
    /// </summary>
    WordEllipsis,

    /// <summary>
    /// Text is trimmed in the middle and an ellipsis is inserted, preserving the end of the string.
    /// Useful for displaying file paths or similar strings where the tail is important.
    /// </summary>
    PathEllipsis
}
