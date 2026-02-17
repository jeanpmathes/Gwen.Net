using System.Drawing;
using Gwen.Net.New.Bindings;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Rendering;
using Gwen.Net.New.Utilities;

namespace Gwen.Net.New.Visuals;

/// <summary>
/// Draws a border and background around its child visual.
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
        BorderThickness = VisualProperty.Create(this, ThicknessF.One, Invalidation.Measure);
    }

    #region PROPERTIES
    
    /// <summary>
    /// The brush used to draw the border.
    /// </summary>
    public VisualProperty<Brush> BorderBrush { get; }

    /// <summary>
    /// The thickness of the border.
    /// </summary>
    public VisualProperty<ThicknessF> BorderThickness { get; }
    
    #endregion PROPERTIES
    
    /// <summary>
    /// Gets or sets the single child visual.
    /// </summary>
    public Visual? Child
    {
        get => Children.Count > 0 ? Children[0] : null;
        set => SetChild(value);
    }

    /// <inheritdoc/>
    public override SizeF OnMeasure(SizeF availableSize)
    {
        ThicknessF borderThickness = BorderThickness.GetValue();

        availableSize -= borderThickness;
        
        SizeF desiredSize = base.OnMeasure(availableSize);

        desiredSize += borderThickness;

        return desiredSize;
    }

    /// <inheritdoc/>
    public override void OnArrange(RectangleF finalRectangle)
    {
        finalRectangle -= BorderThickness.GetValue();
        finalRectangle -= Padding.GetValue();
        
        if (finalRectangle.IsEmpty)
            return;
        
        foreach (Visual child in Children)
            child.Arrange(finalRectangle);
    }

    /// <inheritdoc/>
    protected override void OnRender(IRenderer renderer)
    {
        renderer.DrawFilledRectangle(RenderBounds, Background.GetValue());
        renderer.DrawLinedRectangle(RenderBounds, BorderThickness.GetValue(), BorderBrush.GetValue());
    }
}
