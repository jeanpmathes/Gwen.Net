using System.Drawing;
using Gwen.Net.New.Bindings;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Resources;
using Gwen.Net.New.Styles;
using Gwen.Net.New.Utilities;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Gwen.Net.New.Themes;

/// <summary>
///     The original GWEN theme, formerly known as "DefaultSkin". It is a light theme with rounded corners.
/// </summary>
public class ClassicLight(ResourceRegistry registry) : IResourceBundle<ClassicLight>
{
    // todo: ClassicDark and ClassicLight will probably have very similar code, so creating a sort of shared theme constructor could be a good idea

    private static readonly Brush basicBackgroundBrush = new SolidColorBrush(Color.FromArgb(red: 253, green: 253, blue: 253));
    private static readonly Brush basicForegroundBrush = new SolidColorBrush(Color.FromArgb(red: 73, green: 73, blue: 73));

    private static readonly Brush buttonBorderBrush = new SolidColorBrush(Color.FromArgb(red: 82, green: 82, blue: 82));
    private static readonly Brush buttonBackgroundBrush = basicBackgroundBrush; // todo: make gradient

    private static readonly Brush buttonHoveredBackgroundBrush = new SolidColorBrush(Color.FromArgb(red: 241, green: 241, blue: 241)); // todo: maybe make gradient
    private static readonly Brush buttonPressedBackgroundBrush = new SolidColorBrush(Color.FromArgb(red: 87, green: 180, blue: 245)); // todo: make gradient

    private static readonly Brush buttonDisabledForegroundBrush = new SolidColorBrush(Color.FromArgb(red: 115, green: 115, blue: 115));
    private static readonly Brush buttonDisabledBorderBrush = new SolidColorBrush(Color.FromArgb(red: 154, green: 154, blue: 154));
    private static readonly Brush buttonDisabledBackgroundBrush = new SolidColorBrush(Color.FromArgb(red: 244, green: 244, blue: 244));

    public Style<Canvas> CanvasStyle { get; } = registry.AddStyle<Canvas>(b => b
        .Set(x => x.Foreground, basicForegroundBrush)
        .Set(x => x.Background, basicBackgroundBrush)
    );

    public Style<IButton> ButtonStyle { get; } = registry.AddStyle<IButton>(b => b
        .Set(x => x.Opacity, value: 1.0f)
        .Set(x => x.BorderBrush, buttonBorderBrush)
        .Set(x => x.BorderWidth, new WidthF(2.0f))
        .Set(x => x.BorderRadius, new RadiusF(5.0f))
        .Set(x => x.Background, buttonBackgroundBrush)
        .Trigger(x => x.IsHovered, x => x.Background, buttonHoveredBackgroundBrush)
        .Trigger(x => x.IsPressed, x => x.Background, buttonPressedBackgroundBrush)
        .Trigger(x => Binding.To(x.Enablement).Compute(v => v.IsDisabled), x => x.Background, buttonDisabledBackgroundBrush)
        .Trigger(x => Binding.To(x.Enablement).Compute(v => v.IsDisabled), x => x.BorderBrush, buttonDisabledBorderBrush)
        .Trigger(x => Binding.To(x.Enablement).Compute(v => v.IsDisabled), x => x.Foreground, buttonDisabledForegroundBrush)
    );


    /// <inheritdoc />
    public static ClassicLight Load(ResourceRegistry registry)
    {
        return new ClassicLight(registry);
    }
}
