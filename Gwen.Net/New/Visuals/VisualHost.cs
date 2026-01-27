using System.Collections.Generic;

namespace Gwen.Net.New.Visuals;

/// <summary>
/// Base class of all visual hosts, which are visual elements which host logical elements and their visualizations.
/// </summary>
public abstract class VisualHost : VisualElement
{
    private readonly Dictionary<Element, VisualElement> visualizations = new();
    
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
}
