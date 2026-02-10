using System;
using System.Drawing;

namespace Gwen.Net.New.Utilities;

/// <summary>
/// Helps to work with sizes.
/// </summary>
public static class Sizes
{
    /// <summary>
    /// Clamp size between min and max sizes.
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
