using System;
using System.Drawing;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Texts;
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
    /// Create a formatted text object for the given text, font, and layout options.
    /// </summary>
    /// <param name="text">The text to format.</param>
    /// <param name="font">The font to use for formatting the text.</param>
    /// <param name="options">The layout options such as wrapping, alignment, trimming, and line height.</param>
    /// <returns>The formatted text object.</returns>
    IFormattedText CreateFormattedText(String text, Font font, TextOptions options);
    
    /// <summary>
    /// Draw a filled rectangle.
    /// </summary>
    /// <param name="rectangle">The rectangle to draw.</param>
    /// <param name="brush">The brush to use.</param>
    public void DrawFilledRectangle(RectangleF rectangle, Brush brush);

    /// <summary>
    /// Draw a rectangle outline with the specified border thickness.
    /// </summary>
    /// <param name="rectangle">The rectangle to draw.</param>
    /// <param name="thickness">The thickness of each edge.</param>
    /// <param name="brush">The brush to use.</param>
    public void DrawLinedRectangle(RectangleF rectangle, ThicknessF thickness, Brush brush);
    
    /// <summary>
    /// Draw a rectangle outline with a default border thickness of 1 unit.
    /// </summary>
    /// <param name="rectangle">The rectangle to draw.</param>
    /// <param name="brush">The brush to use.</param>
    public void DrawLinedRectangle(RectangleF rectangle, Brush brush) => DrawLinedRectangle(rectangle, ThicknessF.One, brush);
    
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
