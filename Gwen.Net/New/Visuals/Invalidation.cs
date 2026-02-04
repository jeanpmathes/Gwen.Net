namespace Gwen.Net.New.Visuals;

/// <summary>
/// The types of invalidation that can occur on an element.
/// </summary>
public enum Invalidation
{
    /// <summary>
    /// The visualization (template) is invalidated.
    /// </summary>
    Visualization,
    
    /// <summary>
    /// The measure (desired size) of the element is invalidated.
    /// </summary>
    Measure,
    
    /// <summary>
    /// The arrangement (final size and position) of the element is invalidated.
    /// </summary>
    Arrange,
    
    /// <summary>
    /// The rendering (appearance) of the element is invalidated.
    /// </summary>
    Render,
    
    /// <summary>
    /// No invalidation occurs.
    /// </summary>
    None
}
