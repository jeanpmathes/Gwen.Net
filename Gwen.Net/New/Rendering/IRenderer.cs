using System;
using System.Drawing;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Utilities;

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
    /// <returns>True if the clipping rectangle is empty, false otherwise.</returns>
    public Boolean IsClipEmpty();
    
    /// <summary>
    /// Draw a filled rectangle.
    /// </summary>
    /// <param name="rectangle">The rectangle to draw.</param>
    /// <param name="brush">The brush to use.</param>
    public void DrawFilledRectangle(RectangleF rectangle, Brush brush);

    /// <summary>
    /// Draw a rectangle outline with a 1-unit wide border.
    /// </summary>
    /// <param name="rectangle">The rectangle to draw.</param>
    /// <param name="brush">The brush to use.</param>
    public void DrawLinedRectangle(RectangleF rectangle, Brush brush)
    {
        DrawLinedRectangle(rectangle, ThicknessF.One, brush);
    }

    /// <summary>
    /// Draw a rectangle outline with the specified border thickness.
    /// </summary>
    /// <param name="rectangle">The rectangle to draw.</param>
    /// <param name="thickness">The thickness of each edge.</param>
    /// <param name="brush">The brush to use.</param>
    public void DrawLinedRectangle(RectangleF rectangle, ThicknessF thickness, Brush brush)
    {
        DrawFilledRectangle(new RectangleF(rectangle.Left, rectangle.Top, rectangle.Width, thickness.Top), brush);
        DrawFilledRectangle(new RectangleF(rectangle.Left, rectangle.Bottom - thickness.Bottom, rectangle.Width, thickness.Bottom), brush);
        DrawFilledRectangle(new RectangleF(rectangle.Left, rectangle.Top + thickness.Top, thickness.Left, rectangle.Height - thickness.Top - thickness.Bottom), brush);
        DrawFilledRectangle(new RectangleF(rectangle.Right - thickness.Right, rectangle.Top + thickness.Top, thickness.Right, rectangle.Height - thickness.Top - thickness.Bottom), brush);
    }

    /// <summary>
    /// Draw a vertical line starting at the specified point with the specified length and color.
    /// </summary>
    /// <param name="start">The start point.</param>
    /// <param name="length">The length of the line.</param>
    /// <param name="brush">The brush to use.</param>
    public void DrawVerticalLine(PointF start, Single length, Brush brush)
    {
        DrawVerticalLine(start, length, width: 1, brush);
    }

    /// <summary>
    /// Draw a vertical line starting at the specified point with the specified length, width, and color.
    /// </summary>
    /// <param name="start">The start point.</param>
    /// <param name="length">The length of the line.</param>
    /// <param name="width">The width of the line.</param>
    /// <param name="brush">The brush to use.</param>
    public void DrawVerticalLine(PointF start, Single length, Single width, Brush brush)
    {
        DrawFilledRectangle(new RectangleF(start.X, start.Y, width, length), brush);
    }

    /// <summary>
    /// Draw a horizontal line starting at the specified point with the specified length and color.
    /// </summary>
    /// <param name="start">The start point.</param>
    /// <param name="length">The length of the line.</param>
    /// <param name="brush">The brush to use.</param>
    public void DrawHorizontalLine(PointF start, Single length, Brush brush)
    {
        DrawHorizontalLine(start, length, width: 1, brush);
    }

    /// <summary>
    /// Draw a horizontal line starting at the specified point with the specified length, width, and color.
    /// </summary>
    /// <param name="start">The start point.</param>
    /// <param name="length">The length of the line.</param>
    /// <param name="width">The width (height) of the line.</param>
    /// <param name="brush">The brush to use.</param>
    public void DrawHorizontalLine(PointF start, Single length, Single width, Brush brush)
    {
        DrawFilledRectangle(new RectangleF(start.X, start.Y, length, width), brush);
    }
    
    /// <summary>
    /// Resize the renderer's internal buffers to the specified size.
    /// </summary>
    /// <param name="size">The new size.</param>
    public void Resize(Size size);

    /// <summary>
    /// Scale the rendered results by the specified factor.
    /// This affects all subsequent rendering operations and text measurements.
    /// </summary>
    /// <param name="newScale">The new scale factor.</param>
    public void Scale(Single newScale);
}
