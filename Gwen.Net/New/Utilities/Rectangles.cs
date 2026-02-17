using System.Drawing;

namespace Gwen.Net.New.Utilities;

/// <summary>
/// Utility class for rectangle operations.
/// </summary>
public static class Rectangles
{
    /// <summary>
    /// Clamp the size of the rectangle to the specified minimum and maximum sizes.
    /// </summary>
    /// <param name="rectangle">The rectangle to clamp.</param>
    /// <param name="minSize">The minimum size.</param>
    /// <param name="maxSize">The maximum size.</param>
    /// <returns>The clamped rectangle.</returns>
    public static RectangleF ClampSize(RectangleF rectangle, SizeF minSize, SizeF maxSize)
    {
        rectangle.Size = Sizes.Clamp(rectangle.Size, minSize, maxSize);
        return rectangle;
    }
}
