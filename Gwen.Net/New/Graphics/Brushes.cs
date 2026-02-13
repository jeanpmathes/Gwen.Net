namespace Gwen.Net.New.Graphics;

/// <summary>
/// A utility to access useful predefined brushes.
/// </summary>
public static class Brushes
{
    /// <summary>
    /// Gets a brush that is completely transparent.
    /// </summary>
    public static Brush Transparent { get; } = new TransparentBrush();
    
    /// <summary>
    /// Gets a brush that is solid white.
    /// </summary>
    public static Brush White { get; } = new SolidColorBrush(System.Drawing.Color.Transparent);
    
    /// <summary>
    /// Gets a brush that is solid black.
    /// </summary>
    public static Brush Black { get; } = new SolidColorBrush(System.Drawing.Color.Black);
    
    /// <summary>
    /// Gets a brush that is solid red.
    /// </summary>
    public static Brush Red { get; } = new SolidColorBrush(System.Drawing.Color.Red);
    
    /// <summary>
    /// Gets a brush that is solid green.
    /// </summary>
    public static Brush Green { get; } = new SolidColorBrush(System.Drawing.Color.Green);
    
    /// <summary>
    /// Gets a brush that is solid blue.
    /// </summary>
    public static Brush Blue { get; } = new SolidColorBrush(System.Drawing.Color.Blue);

    /// <summary>
    /// Get a brush to draw debug bounds with.
    /// </summary>
    public static Brush DebugBounds => Red;

    /// <summary>
    /// Get a brush to draw debug margin outlines with.
    /// </summary>
    public static Brush DebugMargin => Blue;

    /// <summary>
    /// Get a brush to draw debug padding outlines with.
    /// </summary>
    public static Brush DebugPadding => Green;
}
