using Gwen.Net.New.Bindings;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Rendering;

namespace Gwen.Net.New.Visuals;

/// <summary>
/// Draws a border and background around its child element.
/// </summary>
/// <see cref="Controls.Border"/>
public class Border : Visual
{
    /// <summary>
    /// Creates a new border visual.
    /// </summary>
    public Border()
    {
        BorderBrush = VisualProperty.Create(this, BindToOwnerForeground(), Invalidation.Render);
    }
    
    /// <summary>
    /// The brush used to draw the border.
    /// </summary>
    public VisualProperty<Brush> BorderBrush { get; }
    
    /// <summary>
    /// Gets or sets the single child element.
    /// </summary>
    public Visual? Child
    {
        get => Children.Count > 0 ? Children[0] : null;
        set => SetChild(value);
    }

    /// <inheritdoc/>
    protected override void OnRender(IRenderer renderer)
    {
        renderer.DrawFilledRectangle(RenderBounds, Background);
        renderer.DrawLinedRectangle(RenderBounds, BorderBrush);
    }
}
