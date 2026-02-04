namespace Gwen.Net.New.Visuals;

/// <summary>
/// The types of invalidation that can occur on an element.
/// </summary>
public enum Invalidation
{
    /// <summary>
    /// The visualization (template) is invalidated.
    /// </summary>
    InvalidateVisualization,
    
    /// <summary>
    /// The measure (desired size) of the element is invalidated.
    /// </summary>
    InvalidateMeasure,
    
    /// <summary>
    /// The arrangement (final size and position) of the element is invalidated.
    /// </summary>
    InvalidateArrange,
    
    /// <summary>
    /// The rendering (appearance) of the element is invalidated.
    /// </summary>
    InvalidateRender
}
