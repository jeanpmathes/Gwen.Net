using System;

namespace Gwen.Net.New.Graphics;

/// <summary>
/// A brush that is invisible (does not render).
/// </summary>
public class TransparentBrush : Brush
{
    /// <inheritdoc />
    public override Boolean Equals(Object? obj)
    {
        return obj is TransparentBrush;
    }

    /// <inheritdoc />
    public override Int32 GetHashCode()
    {
        return typeof(TransparentBrush).GetHashCode();
    }
}
