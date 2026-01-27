using System;
using System.Drawing;

namespace Gwen.Net.New.Rendering;

/// <summary>
/// The interface expected from a renderer for the GWEN GUI.
/// </summary>
public interface IRenderer
{
    /// <summary>
    /// Begin a rendering pass. All rendering operations must be performed between <see cref="Begin"/> and <see cref="End"/>.
    /// </summary>
    public void Begin();
    
    /// <summary>
    /// End a rendering pass. All rendering operations must be performed between <see cref="Begin"/> and <see cref="End"/>.
    /// </summary>
    public void End();
    
    /// <summary>
    /// Push an offset that will be applied to all operations.
    /// The offset is additive, meaning the previous offset will be considered.
    /// </summary>
    /// <param name="offset">The offset to push.</param>
    public void PushOffset(PointF offset);
    
    /// <summary>
    /// Pop the last pushed offset. Performs no operation if no offset was previously pushed.
    /// </summary>
    public void PopOffset();
    
    /// <summary>
    /// Push a clipping rectangle that will be applied to all operations.
    /// The clipping rectangle is intersected with the previous clipping rectangle.
    /// Note that clipping must be enabled via <see cref="BeginClip"/> for the clipping rectangle to take effect.
    /// </summary>
    /// <param name="rectangle">The clipping rectangle to push.</param>
    public void PushClip(RectangleF rectangle);
    
    /// <summary>
    /// Pop the last pushed clipping rectangle. Performs no operation if no clipping rectangle was previously pushed.
    /// </summary>
    public void PopClip();
    
    /// <summary>
    /// Begin clipping. All rendering operations after this call will be clipped to the current clipping rectangle if clipping is enabled.
    /// </summary>
    public void BeginClip();
    
    /// <summary>
    /// End clipping. All rendering operations after this call will not be clipped.
    /// </summary>
    public void EndClip();
    
    /// <summary>
    /// Check if the current clipping rectangle is empty, meaning nothing would pass.
    /// </summary>
    /// <returns>>True if the clipping rectangle is empty, false otherwise.</returns>
    public Boolean IsClipEmpty();
    
    /// <summary>
    /// Draw a filled rectangle.
    /// </summary>
    /// <param name="rectangle">The rectangle to draw.</param>
    /// <param name="color">The color to use.</param>
    public void DrawFilledRectangle(RectangleF rectangle, Color color);

    /// <summary>
    /// Draw a rectangle outline.
    /// </summary>
    /// <param name="rectangle">The rectangle to draw.</param>
    /// <param name="color">The color to use.</param>
    public void DrawLinedRectangle(RectangleF rectangle, Color color)
    {
        DrawHorizontalLine(new PointF(rectangle.Left, rectangle.Top), rectangle.Width, color);
        DrawHorizontalLine(new PointF(rectangle.Left, rectangle.Bottom - 1), rectangle.Width, color);
        DrawVerticalLine(new PointF(rectangle.Left, rectangle.Top), rectangle.Height, color);
        DrawVerticalLine(new PointF(rectangle.Right - 1, rectangle.Top), rectangle.Height, color);
    }

    /// <summary>
    /// Draw a vertical line starting at the specified point with the specified length and color.
    /// </summary>
    /// <param name="start">The start point.</param>
    /// <param name="length">The length of the line.</param>
    /// <param name="color">The color to use.</param>
    public void DrawVerticalLine(PointF start, Single length, Color color)
    {
        DrawFilledRectangle(new RectangleF(start.X, start.Y, width: 1, length), color);
    }

    /// <summary>
    /// Draw a horizontal line starting at the specified point with the specified length and color.
    /// </summary>
    /// <param name="start">The start point.</param>
    /// <param name="length">The length of the line.</param>
    /// <param name="color">The color to use.</param>
    public void DrawHorizontalLine(PointF start, Single length, Color color)
    {
        DrawFilledRectangle(new RectangleF(start.X, start.Y, length, height: 1), color);
    }
    
    /// <summary>
    /// Resize the renderer's internal buffers to the specified size.
    /// </summary>
    /// <param name="size">The new size.</param>
    public void Resize(Size size);
}
