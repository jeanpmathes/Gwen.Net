using Gwen.Net.New.Graphics;
using Gwen.Net.New.Rendering;

namespace Gwen.Net.New.Visuals;

/// <summary>
/// A border draws background and border around its child element.
/// </summary>
public class Border : VisualHost
{
    /// <summary>
    /// Creates a new border element.
    /// </summary>
    public Border(Element? child = null) : base(child)
    {
        
    }
    
    /// <summary>
    /// Gets or sets the brush used to draw the border.
    /// </summary>
    public Brush BorderBrush
    {
        get;

        set
        {
            field = value;
            InvalidateRender();
        }
    } = Brushes.Black;
    
    /// <inheritdoc/>
    public override void OnRender(IRenderer renderer)
    {
        renderer.DrawFilledRectangle(RenderBounds, Background);
        renderer.DrawLinedRectangle(RenderBounds, BorderBrush);
    }
}
