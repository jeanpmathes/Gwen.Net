using System.Collections.Generic;

namespace Gwen.Net.New.Visuals;

/// <summary>
/// Base class of all visual hosts, which are visual elements which host logical elements and their visualizations.
/// If a visual element takes logical children, it should derive from this class.
/// If a visual element does not derive from this class, it should take visual children only.
/// </summary>
public abstract class VisualHost : VisualElement
{
    private readonly Dictionary<Element, VisualElement> visualizations = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="VisualHost"/> class.
    /// Children can be passed as parameters, but can also be added later.
    /// </summary>
    protected VisualHost(params IEnumerable<Element> children)
    {
        foreach (Element child in children)
        {
            AddLogicalChild(child);
        }
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="VisualHost"/> class.
    /// A single child can be passed as parameter, but more can also be added later.
    /// </summary>
    protected VisualHost(Element? child)
    {
        if (child != null)
        {
            AddLogicalChild(child);
        }
    }

    private protected override void OnLogicalChildAdded(Element child)
    {
        if (IsAttachedToRoot && child.Visualization is {} visualization)
        {
            AddVisualChild(visualization);
            visualizations[child] = visualization;
        }
    }

    private protected override void OnLogicalChildRemoved(Element child)
    {
        if (IsAttachedToRoot && visualizations.TryGetValue(child, out VisualElement? visualization))
        {
            RemoveVisualChild(visualization);
            visualizations.Remove(child);
        }
    }

    /// <inheritdoc/>
    public sealed override void OnAttach()
    {
        RemoveAllVisualChildren();
        visualizations.Clear();
        
        foreach (Element child in LogicalChildren)
        {
            if (child.Visualization is {} visualization)
            {
                AddVisualChild(visualization);
                visualizations[child] = visualization;
            }
        }
    }
    
    /// <inheritdoc/>
    protected override void OnVisualizationInvalidated(Element child)
    {
        if (IsAttachedToRoot && visualizations.TryGetValue(child, out VisualElement? oldVisualization))
        {
            RemoveVisualChild(oldVisualization);
            visualizations.Remove(child);
            
            if (child.Visualization is {} newVisualization)
            {
                AddVisualChild(newVisualization);
                visualizations[child] = newVisualization;
            }
        }
        else base.OnVisualizationInvalidated(child);
    }
}
