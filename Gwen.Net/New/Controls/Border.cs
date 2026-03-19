using Gwen.Net.New.Bindings;
using Gwen.Net.New.Controls.Bases;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Utilities;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Controls;

/// <summary>
///     A <see cref="Border" /> draws a border and background around its child control.
/// </summary>
/// <seealso cref="Visuals.Border" />
public class Border : BorderBase<Border>
{
    /// <summary>
    /// Create a new instance of the <see cref="Border"/> class.
    /// </summary>
    public Border()
    {
        BorderWidth = Property.Create(this, WidthF.One);
        BorderRadius = Property.Create(this, RadiusF.Zero);
        BorderStrokeStyle = Property.Create(this, StrokeStyle.Solid);
    }

    /// <inheritdoc />
    protected override ControlTemplate<Border> CreateDefaultTemplate()
    {
        return ControlTemplate.Create<Border>(control => new Visuals.Border
        {
            BorderWidth = {Binding = Binding.To(control.BorderWidth)},
            BorderRadius = {Binding = Binding.To(control.BorderRadius)},
            BorderStrokeStyle = {Binding = Binding.To(control.BorderStrokeStyle)},

            Child = new ChildPresenter()
        });
    }

    #region PROPERTIES

    /// <summary>
    ///     The width of the border drawn around the child control.
    /// </summary>
    public Property<WidthF> BorderWidth { get; }

    /// <summary>
    ///     The radius of the corners of the drawn border.
    /// </summary>
    public Property<RadiusF> BorderRadius { get; }

    /// <summary>
    ///     The stroke style of the border drawn around the child control.
    /// </summary>
    public Property<StrokeStyle> BorderStrokeStyle { get; }

    #endregion PROPERTIES
}
