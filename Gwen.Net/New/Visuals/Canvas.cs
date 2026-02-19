using System.Drawing;
using Gwen.Net.New.Input;
using Gwen.Net.New.Rendering;

namespace Gwen.Net.New.Visuals;

/// <summary>
/// The root visual for a GWEN user interface.
/// </summary>
/// <seealso cref="Controls.Canvas"/>
public class Canvas : Visual
{
    /// <summary>
    /// The input handler for this canvas, responsible for processing input events and determining which visual should receive them.
    /// </summary>
    public InputHandler Input { get; }

    /// <summary>
    /// Creates a new <see cref="Canvas"/> instance.
    /// </summary>
    public Canvas()
    {
        Input = new InputHandler(this);
    }
    
    /// <summary>
    /// Gets or sets the single child visual.
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
}
