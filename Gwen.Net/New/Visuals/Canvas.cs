using System.Drawing;
using Gwen.Net.New.Rendering;

namespace Gwen.Net.New.Visuals;

/// <summary>
/// The visual root element for a GWEN user interface.
/// </summary>
/// <seealso cref="Controls.Canvas"/>
public class Canvas : Visual
{
    /// <summary>
    /// Gets or sets the single child element.
    /// </summary>
    public Visual? Child
    {
        get => Children.Count > 0 ? Children[0] : null;
        set => SetChild(value);
    } 
    
    /// <inheritdoc/>
    public override void OnBoundsChanged(RectangleF oldBounds, RectangleF newBounds)
    {
        InvalidateMeasure();
    }
    
    /// <inheritdoc/>
    public override void Render(IRenderer renderer)
    {
        renderer.Begin();
        
        renderer.PushOffset(Point.Empty);
        renderer.PushClip(Bounds);
        
        base.Render(renderer);
        
        renderer.EndClip();
        
        renderer.PopClip();
        renderer.PopOffset();
        
        renderer.End();
    }
    
    /// <inheritdoc/>
    protected override void OnRender(IRenderer renderer)
    {
        renderer.DrawFilledRectangle(RenderBounds, Background);
    }
}
