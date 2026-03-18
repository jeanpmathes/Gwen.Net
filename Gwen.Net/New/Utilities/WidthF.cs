using System;

namespace Gwen.Net.New.Utilities;

/// <summary>
/// A width of lines.
/// </summary>
public readonly record struct WidthF
{
    /// <summary>
    /// Creates a new width with the specified width.
    /// </summary>
    /// <param name="value">The width of the lines.</param>
    public WidthF(Single value)
    {
        Value = value;
    }

    /// <summary>
    /// A width of size zero.
    /// </summary>
    public static WidthF Zero { get; } = new(0);

    /// <summary>
    /// A width of size one.
    /// </summary>
    public static WidthF One { get; } = new(1);

    /// <summary>
    /// The width value.
    /// </summary>
    public Single Value { get; init; }

    /// <summary>
    /// Create a <see cref="ThicknessF"/> from this width.
    /// </summary>
    /// <returns>A <see cref="ThicknessF"/> with the same width for all sides.</returns>
    public ThicknessF ToThicknessF() => new(Value);

    /// <inheritdoc/>
    public override String ToString()
    {
        return this == Zero
            ? "WidthF.Zero"
            : $"WidthF(Value: {Value})";
    }
}
