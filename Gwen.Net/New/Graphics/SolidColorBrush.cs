using System;
using System.Drawing;

namespace Gwen.Net.New.Graphics;

/// <summary>
/// A brush that fills with a solid color.
/// </summary>
public class SolidColorBrush(Color color) : Brush
{
    /// <summary>
    /// The color of the brush.
    /// </summary>
    public Color Color { get; } = color;

    /// <inheritdoc />
    public override Boolean Equals(Object? obj)
    {
        return obj is SolidColorBrush solidColorBrush && Color.Equals(solidColorBrush.Color);
    }

    /// <inheritdoc />
    public override Int32 GetHashCode()
    {
        return Color.GetHashCode();
    }
}
