using Gwen.Net.New.Bindings;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Rendering;

namespace Gwen.Net.New.Visuals;

/// <summary>
/// A border draws background and border around its child element.
/// </summary>
public class Border : VisualHost
{
    #region PROPERTIES

    /// <summary>
    /// Creates a new instance of the <see cref="Border"/> class.
    /// </summary>
    public Border()
    {
        BorderBrush = Property.Create(this, BindToTemplateOwnerForeground(), Invalidation.Render);
    }
    
    /// <summary>
    /// The brush used to draw the border. If <c>null</c>, no border will be drawn.
    /// </summary>
    public Property<Brush> BorderBrush { get; }
    
    #endregion PROPERTIES
    
    /// <summary>
    /// Gets or sets the single child element.
    /// </summary>
    public Element? Child
    {
        get => LogicalChildren.Count > 0 ? LogicalChildren[0] : null;
        set => SetLogicalChild(value);
    } 
    
    /// <inheritdoc/>
    public override void OnRender(IRenderer renderer)
    {
        base.OnRender(renderer);
        
        renderer.DrawLinedRectangle(RenderBounds, BorderBrush);
    }
}
