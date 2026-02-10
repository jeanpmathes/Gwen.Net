using System.Drawing;

namespace Gwen.Net.New.Utilities;

/// <summary>
/// Utility class for rectangle operations.
/// </summary>
public static class Rectangles
{
    /// <summary>
    /// Limit the size of the rectangle to the specified size.
    /// </summary>
    /// <param name="rectangle">The rectangle to constrain.</param>
    /// <param name="size">The maximum size.</param>
    /// <returns>The constrained rectangle.</returns>
    public static RectangleF ConstraintSize(RectangleF rectangle, SizeF size)
    {
        if (rectangle.Width > size.Width)
            rectangle.Width = size.Width;
        
        if (rectangle.Height > size.Height)
            rectangle.Height = size.Height;
        
        return rectangle;
    }
}
