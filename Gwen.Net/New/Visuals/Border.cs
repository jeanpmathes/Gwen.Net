using System.Drawing;
using Gwen.Net.New.Bindings;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Utilities;

namespace Gwen.Net.New.Visuals;

/// <summary>
///     Draws a border and background around its child visual.
/// </summary>
/// <see cref="Controls.Border" />
public class Border : Visual
{
    /// <summary>
    ///     Creates a new border visual.
    /// </summary>
    public Border()
    {
        BorderBrush = VisualProperty.Create(this, BindToOwnerForeground(), Invalidation.Render);
        BorderWidth = VisualProperty.Create(this, WidthF.One, Invalidation.Measure);
        BorderRadius = VisualProperty.Create(this, RadiusF.Zero, Invalidation.Render);
        BorderStrokeStyle = VisualProperty.Create(this, StrokeStyle.Solid, Invalidation.Render);
    }

    /// <summary>
    ///     Gets or sets the single child visual.
    /// </summary>
    public Visual? Child
    {
        get => Children.Count > 0 ? Children[0] : null;
        init => SetChild(value);
    }

    /// <inheritdoc />
    public override SizeF OnMeasure(SizeF availableSize)
    {
        ThicknessF borderThickness = BorderWidth.GetValue().ToThicknessF();

        availableSize -= borderThickness;

        SizeF desiredSize = base.OnMeasure(availableSize);

        desiredSize += borderThickness;

        return desiredSize;
    }

    /// <inheritdoc />
    public override void OnArrange(RectangleF finalRectangle)
    {
        finalRectangle -= BorderWidth.GetValue().ToThicknessF();
        finalRectangle -= Padding.GetValue();

        if (finalRectangle.IsEmpty)
            return;

        foreach (Visual child in Children)
            child.Arrange(finalRectangle);
    }

    /// <inheritdoc />
    protected override void OnRender()
    {
        Renderer.DrawFilledRectangle(RenderBounds, BorderRadius.GetValue(), Background.GetValue());
        Renderer.DrawLinedRectangle(RenderBounds, BorderWidth.GetValue(), BorderRadius.GetValue(), BorderStrokeStyle.GetValue(), BorderBrush.GetValue());
    }

    #region PROPERTIES

    /// <summary>
    ///     The brush used to draw the border.
    /// </summary>
    public VisualProperty<Brush> BorderBrush { get; }

    /// <summary>
    ///     The thickness of the border.
    /// </summary>
    public VisualProperty<WidthF> BorderWidth { get; }

    /// <summary>
    ///     The radius of the corners. This affects both the background and the border.
    /// </summary>
    public VisualProperty<RadiusF> BorderRadius { get; }

    /// <summary>
    ///     The style of the border stroke.
    /// </summary>
    public VisualProperty<StrokeStyle> BorderStrokeStyle { get; }

    #endregion PROPERTIES
}
