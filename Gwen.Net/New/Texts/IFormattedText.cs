using System;
using System.Drawing;
using Gwen.Net.New.Graphics;

namespace Gwen.Net.New.Texts;

/// <summary>
/// Combines text content with formatting information.
/// </summary>
public interface IFormattedText : IDisposable
{
    /// <summary>
    /// Measures the size of the formatted text given the available size constraints.
    /// </summary>
    /// <param name="availableSize">The available size for the formatted text.</param>
    /// <returns>The size required to render the formatted text within the given constraints.</returns>
    public SizeF Measure(SizeF availableSize);

    /// <summary>
    /// Draws the formatted text in a given rectangle using the specified brush.
    /// Note that this method is affected by the current state of the renderer, e.g. offset and clipping.
    /// </summary>
    /// <param name="rectangle">The rectangle in which to draw the text.</param>
    /// <param name="brush">The brush to use for drawing the text.</param>
    public void Draw(RectangleF rectangle, Brush brush);
}
