using System.Drawing;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Resources;
using Gwen.Net.New.Styles;

namespace Gwen.Net.New.Themes;

/// <summary>
/// The original GWEN theme, formerly known as "DefaultSkin". It is a light theme with rounded corners.
/// </summary>
public class ClassicLight(ResourceRegistry registry) : IResourceBundle<ClassicLight>
{
    // todo: ClassicDark and ClassicLight will probably have very similar code, so creating a sort of shared theme constructor could be a good idea

    private static readonly Brush basicBackgroundBrush = new SolidColorBrush(Color.FromArgb(red: 253, green: 253, blue: 253));
    private static readonly Brush basicForegroundBrush = new SolidColorBrush(Color.FromArgb(red: 73, green: 73, blue: 73));
    
    /// <inheritdoc/>
    public static ClassicLight Load(ResourceRegistry registry) => new(registry);

    internal Style<Control> ControlStyle { get; } = registry.AddStyle<Control>(b => b
        .Set(x => x.Foreground, basicForegroundBrush)
    );
    
    internal Style<Canvas> CanvasStyle { get; } = registry.AddStyle<Canvas>(b => b
        .Set(x => x.Background, basicBackgroundBrush)
    );
}
