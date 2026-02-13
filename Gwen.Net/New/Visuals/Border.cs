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

        if (availableSize.IsEmpty)
            return SizeF.Empty + borderThickness;

        SizeF desiredSize = base.OnMeasure(availableSize);

        desiredSize += borderThickness;

        return desiredSize;
    }

    /// <inheritdoc/>
    public override RectangleF OnArrange(RectangleF finalRectangle)
    {
        if (Children.Count == 0)
            return Rectangles.ConstraintSize(finalRectangle, MeasuredSize - Margin.GetValue());

        ThicknessF borderThickness = BorderThickness.GetValue();
        RectangleF contentArea = new RectangleF(PointF.Empty, finalRectangle.Size) - borderThickness - Padding.GetValue();

        if (contentArea.IsEmpty)
            return finalRectangle;

        foreach (Visual child in Children)
            child.Arrange(Rectangles.ConstraintSize(contentArea, child.MeasuredSize));

        return finalRectangle;
    }

    /// <inheritdoc/>
    protected override void OnRender(IRenderer renderer)
    {
        renderer.DrawFilledRectangle(RenderBounds, Background.GetValue());
        renderer.DrawLinedRectangle(RenderBounds, BorderThickness.GetValue(), BorderBrush.GetValue());
    }
}
