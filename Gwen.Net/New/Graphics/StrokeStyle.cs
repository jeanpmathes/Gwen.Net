namespace Gwen.Net.New.Graphics;

/// <summary>
/// The style of a stroke, for example the line around a border.
/// </summary>
public enum StrokeStyle
{
    /// <summary>
    /// A solid stroke. The default stroke.
    /// </summary>
    Solid,

    /// <summary>
    /// A dashed stroke.
    /// </summary>
    Dashes,

    /// <summary>
    /// A squared stroke, consisting of many squares.
    /// Essentially like <see cref="Dotted"/>, but not round.
    /// </summary>
    Squared,

    /// <summary>
    /// A dotted stroke, using round markers.
    /// </summary>
    Dotted
}
