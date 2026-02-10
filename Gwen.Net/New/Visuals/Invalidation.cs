namespace Gwen.Net.New.Visuals;

/// <summary>
/// The types of invalidation that can occur on a visual.
/// </summary>
public enum Invalidation
{
    /// <summary>
    /// The measure (desired size) of the visual is invalidated.
    /// </summary>
    Measure,
    
    /// <summary>
    /// The arrangement (final size and position) of the visual is invalidated.
    /// </summary>
    Arrange,
    
    /// <summary>
    /// The rendering (appearance) of the visual is invalidated.
    /// </summary>
    Render,
    
    /// <summary>
    /// No invalidation occurs.
    /// </summary>
    None
}
