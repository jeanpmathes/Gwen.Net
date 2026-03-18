using System;
using System.Drawing;
using Gwen.Net.New.Bindings;
using Gwen.Net.New.Controls.Bases;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Utilities;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Controls;

/// <summary>
///     Non-generic contract for buttons.
/// </summary>
public interface IButton : IContentControl
{
    /// <inheritdoc cref="Button{TContent}.BorderBrush" />
    public Property<Brush> BorderBrush { get; }

    /// <inheritdoc cref="Button{TContent}.BorderWidth" />
    public Property<WidthF> BorderWidth { get; }

    /// <inheritdoc cref="Button{TContent}.BorderRadius" />
    public Property<RadiusF> BorderRadius { get; }

    /// <inheritdoc cref="ButtonBase{TContent,TControl}.IsPressed" />
    public ReadOnlySlot<Boolean> IsPressed { get; }
}

internal static class ButtonDefaults
{
    public static readonly Brush DisabledForeground = new SolidColorBrush(Color.Gray);

    public static readonly Brush FocusedBorderBrush = new SolidColorBrush(Color.Blue);

    public static readonly Brush HoveredBackground = new SolidColorBrush(Color.LightGray);
    public static readonly Brush PressedBackground = new SolidColorBrush(Color.Gray);
}

/// <summary>
///     A content control that can be clicked to perform an action.
/// </summary>
public class Button<TContent> : ButtonBase<TContent, Button<TContent>>, IButton where TContent : class
{
    /// <summary>
    ///     Creates a new instance of the <see cref="Button{TContent}" /> class.
    /// </summary>
    public Button()
    {
        BorderBrush = Property.Create(this, Binding.To(Foreground).Combine(IsKeyboardFocused).Compute((foreground, isFocused) => isFocused ? ButtonDefaults.FocusedBorderBrush : foreground));
        BorderWidth = Property.Create(this, new WidthF(1.0f));
        BorderRadius = Property.Create(this, RadiusF.Zero);

        Foreground.OverrideDefault(old => old
            .Combine(Enablement)
            .Compute((foreground, enablement) => enablement.IsEnabled ? foreground : ButtonDefaults.DisabledForeground));

        Background.OverrideDefault(old => old
            .Combine(IsPressed, IsHovered)
            .Compute((background, isPressed, isHovered) => isPressed
                ? ButtonDefaults.PressedBackground
                : isHovered
                    ? ButtonDefaults.HoveredBackground
                    : background));

        Opacity.OverrideDefault(old => old
            .Combine(Enablement)
            .Compute((opacity, enablement) => enablement.IsEnabled ? opacity : 0.8f));

        IsNavigable.OverrideDefault(defaultValue: true);
    }

    /// <inheritdoc />
    protected override ControlTemplate<Button<TContent>> CreateDefaultTemplate()
    {
        return ControlTemplate.Create<Button<TContent>>(static control => new Visuals.Border
        {
            BorderBrush = {Binding = Binding.To(control.BorderBrush)},
            BorderWidth = {Binding = Binding.To(control.BorderWidth)},
            BorderRadius = {Binding = Binding.To(control.BorderRadius)},

            Child = new ChildPresenter()
        });
    }

    #region PROPERTIES

    /// <summary>
    ///     The brush used to draw the border of the button.
    /// </summary>
    public Property<Brush> BorderBrush { get; }

    /// <summary>
    ///     The width of the button's border.
    /// </summary>
    public Property<WidthF> BorderWidth { get; }

    /// <summary>
    ///     The radius of the corners of the button's border.
    /// </summary>
    public Property<RadiusF> BorderRadius { get; }

    #endregion PROPERTIES
}
