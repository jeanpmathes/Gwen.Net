using System.Drawing;
using Gwen.Net.New.Bindings;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Resources;
using Gwen.Net.New.Styles;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Gwen.Net.New.Themes;

/// <summary>
///     The original dark GWEN theme, formerly known as "DefaultSkin2". It is a dark theme, with a more modern look.
/// </summary>
public class ClassicDark(ResourceRegistry registry) : IResourceBundle<ClassicDark>
{
    private static readonly Brush basicBackgroundBrush = new SolidColorBrush(Color.FromArgb(red: 45, green: 45, blue: 48));
    private static readonly Brush basicForegroundBrush = new SolidColorBrush(Color.FromArgb(red: 255, green: 255, blue: 255));

    private static readonly Brush hoveredBackgroundBrush = new SolidColorBrush(Color.FromArgb(red: 62, green: 62, blue: 64));
    private static readonly Brush hoveredForegroundBrush = new SolidColorBrush(Color.FromArgb(red: 0, green: 122, blue: 204));

    private static readonly Brush pressedBackgroundBrush = new SolidColorBrush(Color.FromArgb(red: 0, green: 122, blue: 204));
    private static readonly Brush pressedForegroundBrush = new SolidColorBrush(Color.FromArgb(red: 255, green: 255, blue: 255));

    private static readonly Brush disabledForegroundBrush = new SolidColorBrush(Color.FromArgb(red: 62, green: 62, blue: 64));

    private static readonly Brush focusedOutlineBrush = new SolidColorBrush(Color.FromArgb(red: 0, green: 0, blue: 0));

    public Style<Canvas> CanvasStyle { get; } = registry.AddStyle<Canvas>(b => b
        .Set(x => x.Foreground, basicForegroundBrush)
        .Set(x => x.Background, basicBackgroundBrush)
    );

    public Style<IButton> ButtonStyle { get; } = registry.AddStyle<IButton>(b => b
        .Set(x => x.Opacity, value: 1.0f)
        .Set(x => x.BorderBrush, basicBackgroundBrush)
        .Trigger(x => x.IsHovered, x => x.Background, hoveredBackgroundBrush)
        .Trigger(x => x.IsPressed, x => x.Background, pressedBackgroundBrush)
        .Trigger(x => x.IsHovered, x => x.BorderBrush, hoveredBackgroundBrush)
        .Trigger(x => x.IsPressed, x => x.BorderBrush, pressedBackgroundBrush)
        .Trigger(x => x.IsKeyboardFocused, x => x.BorderBrush, focusedOutlineBrush)
        .Trigger(x => x.IsKeyboardFocused, x => x.BorderStrokeStyle, StrokeStyle.Squared)
        .Trigger(x => x.IsHovered, x => x.Foreground, hoveredForegroundBrush)
        .Trigger(x => x.IsPressed, x => x.Foreground, pressedForegroundBrush)
        .Trigger(x => Binding.To(x.Enablement).Compute(v => v.IsDisabled), x => x.Foreground, disabledForegroundBrush)
    );

    /// <inheritdoc />
    public static ClassicDark Load(ResourceRegistry registry)
    {
        return new ClassicDark(registry);
    }
}
