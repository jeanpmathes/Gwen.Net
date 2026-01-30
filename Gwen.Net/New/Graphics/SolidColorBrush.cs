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
}
