using System;
using System.Drawing;
using Gwen.Net.New.Graphics;

namespace Gwen.Net.New.Rendering;

/// <summary>
/// Abstract base class that helps implement renderers.
/// </summary>
public abstract class Renderer : IRenderer
{
    private Single scale = 1.0f;

    /// <inheritdoc/>
    public abstract void Begin();

    /// <inheritdoc/>
    public abstract void End();

    /// <inheritdoc/>
    public abstract void PushOffset(PointF offset);

    /// <inheritdoc/>
    public abstract void PopOffset();

    /// <inheritdoc/>
    public abstract void PushClip(RectangleF rectangle);

    /// <inheritdoc/>
    public abstract void PopClip();

    /// <inheritdoc/>
    public abstract void BeginClip();

    /// <inheritdoc/>
    public abstract void EndClip();

    /// <inheritdoc/>
    public abstract Boolean IsClipEmpty();
    
    /// <inheritdoc/>
    public abstract void DrawFilledRectangle(RectangleF rectangle, Brush brush);

    /// <inheritdoc/>
    public abstract void Resize(Size size);

    /// <inheritdoc/>
    public virtual void Scale(Single newScale)
    {
        scale = newScale;
    }
    
    /// <summary>
    /// Scale a point by applying the scale factor.
    /// </summary>
    /// <param name="point">The point to scale.</param>
    /// <returns>>The scaled point.</returns>
    protected PointF Scale(PointF point)
    {
        point.X *= scale;
        point.Y *= scale;

        return point;
    }
    
    /// <summary>
    /// Scale a rectangle by applying the scale factor.
    /// </summary>
    /// <param name="rectangle">The rectangle to scale.</param>
    /// <returns>The scaled rectangle.</returns>
    protected RectangleF Scale(RectangleF rectangle)
    {
        rectangle.X *= scale;
        rectangle.Y *= scale;
        rectangle.Width *= scale;
        rectangle.Height *= scale;

        return rectangle;
    }
}
