using System.Drawing;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Resources;
using Gwen.Net.New.Styles;

namespace Gwen.Net.New.Themes;

/// <summary>
/// The original dark GWEN theme, formerly known as "DefaultSkin2". It is a dark theme, with a more modern look.
/// </summary>
public class ClassicDark(ResourceRegistry registry) : IResourceBundle<ClassicDark>
{
    // todo: ClassicDark and ClassicLight will probably have very similar code, so creating a sort of shared theme constructor could be a good idea
    
    private static readonly Brush basicBackgroundBrush = new SolidColorBrush(Color.FromArgb(red: 45, green: 45, blue: 48));
    private static readonly Brush basicForegroundBrush = new SolidColorBrush(Color.FromArgb(red: 255, green: 255, blue: 255));
    
    /// <inheritdoc/>
    public static ClassicDark Load(ResourceRegistry registry) => new(registry);
    
    internal Style<Control> ControlStyle { get; } = registry.AddStyle<Control>(b => b
        .Set(x => x.Foreground, basicForegroundBrush)
    );
    
    internal Style<Canvas> CanvasStyle { get; } = registry.AddStyle<Canvas>(b => b
        .Set(x => x.Background, basicBackgroundBrush)
    );
}
