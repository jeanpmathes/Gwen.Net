using System;
using System.Drawing;

namespace Gwen.Net.New.Utilities;

/// <summary>
/// Helps to work with sizes.
/// </summary>
public static class Sizes
{
    /// <summary>
    /// Get the component-wise maximum of two sizes.
    /// </summary>
    /// <param name="size1">The first size.</param>
    /// <param name="size2">The second size.</param>
    /// <returns>>The component-wise maximum of the two sizes.</returns>
    public static SizeF Max(SizeF size1, SizeF size2)
    {
        return new SizeF(Math.Max(size1.Width, size2.Width), Math.Max(size1.Height, size2.Height));
    }
    
    /// <summary>
    /// Clamp size between min and max sizes, performing a component-wise clamp.
    /// </summary>
    /// <param name="size">The size to clamp.</param>
    /// <param name="minSize">The minimum size.</param>
    /// <param name="maxSize">The maximum size.</param>
    /// <returns>The clamped size.</returns>
    public static SizeF Clamp(SizeF size, SizeF minSize, SizeF maxSize)
    {
        return new SizeF(Math.Clamp(size.Width, minSize.Width, maxSize.Width), Math.Clamp(size.Height, minSize.Height, maxSize.Height));
    }
}
