using System;

namespace Gwen.Net.New.Utilities;

/// <summary>
/// A radius used to draw rounded corners for rectangles.
/// </summary>
public readonly record struct RadiusF
{
    /// <summary>
    /// Create a new radius with the specified X and Y values.
    /// </summary>
    /// <param name="x">The x value to use.</param>
    /// <param name="y">The y value to use.</param>
    public RadiusF(Single x, Single y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Create a new radius with uniform X and Y values.
    /// </summary>
    /// <param name="uniform">The uniform value to use for both X and Y.</param>
    public RadiusF(Single uniform) : this(uniform, uniform) {}

    /// <summary>
    /// Get a uniformly zero radius, which corresponds to non-rounded corners.
    /// </summary>
    public static RadiusF Zero { get; } = new(0);

    /// <summary>
    /// The radius of the corners on the X axis.
    /// </summary>
    public Single X { get; }

    /// <summary>
    /// The radius of the corner on the Y axis.
    /// </summary>
    public Single Y { get; }

    /// <inheritdoc />
    public override String ToString()
    {
        return this == Zero
            ? "RadiusF.Zero"
            : $"RadiusF(X: {X}, Y: {Y})";
    }
}
