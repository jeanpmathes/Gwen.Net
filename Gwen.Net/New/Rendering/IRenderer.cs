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
    
    // todo: offset and clipping should use a stack with push/pop operations, no get/set available
    
    /// <summary>
    /// Get the current offset applied to all operations.
    /// </summary>
    public Size Offset { get; set; }
    
    /// <summary>
    /// Add an offset that will be added to all operations.
    /// It will be added to the current offset.
    /// </summary>
    /// <param name="offset">The offset to set.</param>
    /// <returns>The previous offset.</returns>
    public Size AddOffset(Point offset);
    
    /// <summary>
    /// Constrain the clipping region to the specified rectangle.
    /// If it extended beyond the current clipping region, it will be reduced accordingly.
    /// </summary>
    /// <param name="rectangle">The clipping rectangle to constrain to.</param>
    /// <returns>The previous clipping rectangle.</returns>
    public Rectangle ConstrainClipRegion(Rectangle rectangle);
    
    /// <summary>
    /// Get and set the current clipping rectangle.
    /// Note that the clipping rectangle is stored with the offset applied.
    /// </summary>
    public Rectangle Clip { get; set; }
    
    /// <summary>
    /// Get and set whether clipping is enabled.
    /// </summary>
    public Boolean IsClippingEnabled { get; set; }
    
    /// <summary>
    /// Draw a filled rectangle.
    /// </summary>
    /// <param name="rectangle">The rectangle to draw.</param>
    /// <param name="color">The color to use.</param>
    public void DrawFilledRectangle(Rectangle rectangle, Color color);

    /// <summary>
    /// Draw a rectangle outline.
    /// </summary>
    /// <param name="rectangle">The rectangle to draw.</param>
    /// <param name="color">The color to use.</param>
    public void DrawLinedRectangle(Rectangle rectangle, Color color)
    {
        DrawHorizontalLine(new Point(rectangle.Left, rectangle.Top), rectangle.Width, color);
        DrawHorizontalLine(new Point(rectangle.Left, rectangle.Bottom - 1), rectangle.Width, color);
        DrawVerticalLine(new Point(rectangle.Left, rectangle.Top), rectangle.Height, color);
        DrawVerticalLine(new Point(rectangle.Right - 1, rectangle.Top), rectangle.Height, color);
    }

    /// <summary>
    /// Draw a vertical line starting at the specified point with the specified length and color.
    /// </summary>
    /// <param name="start">The start point.</param>
    /// <param name="length">The length of the line.</param>
    /// <param name="color">The color to use.</param>
    public void DrawVerticalLine(Point start, Int32 length, Color color)
    {
        DrawFilledRectangle(new Rectangle(start.X, start.Y, width: 1, length), color);
    }

    /// <summary>
    /// Draw a horizontal line starting at the specified point with the specified length and color.
    /// </summary>
    /// <param name="start">The start point.</param>
    /// <param name="length">The length of the line.</param>
    /// <param name="color">The color to use.</param>
    public void DrawHorizontalLine(Point start, Int32 length, Color color)
    {
        DrawFilledRectangle(new Rectangle(start.X, start.Y, length, height: 1), color);
    }
    
    /// <summary>
    /// Resize the renderer's internal buffers to the specified size.
    /// </summary>
    /// <param name="size">The new size.</param>
    public void Resize(Size size);
}
