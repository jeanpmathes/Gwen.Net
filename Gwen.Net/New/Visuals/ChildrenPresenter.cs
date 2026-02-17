using System;
using System.Collections.Generic;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Controls.Internals;

namespace Gwen.Net.New.Visuals;

/// <summary>
/// Presents the children of the template owner, or nothing if the owner has no children.
/// This should be in templates of <see cref="MultiChildControl{TControl}"/> controls, and will visualize the child controls of the owner.
/// As this does not define any layout behavior, using this class on its own is not sensible, instead a subclass should be used.
/// </summary>
public abstract class ChildrenPresenter : Visual
{
    private readonly Dictionary<Control, Visual> visualizedChildren = [];

    /// <inheritdoc/>
    public override void OnAttach()
    {
        Control? templateOwner = TemplateOwner.GetValue();
        if (templateOwner == null) return;
        
        templateOwner.ChildAdded += OnTemplateOwnerChildAdded;
        templateOwner.ChildRemoved += OnTemplateOwnerChildRemoved;
        
        Boolean isReparenting = visualizedChildren.Count > 0;
        if (isReparenting) return;
        
        foreach (Control child in templateOwner.Children)
        {
            AddVisualization(child);
        }
    }
    
    /// <inheritdoc/>
    public override void OnDetach(Boolean isReparenting)
    {
        Control? templateOwner = TemplateOwner.GetValue();
        if (templateOwner == null) return;
        
        templateOwner.ChildAdded -= OnTemplateOwnerChildAdded;
        templateOwner.ChildRemoved -= OnTemplateOwnerChildRemoved;

        if (isReparenting) return;
        
        foreach (Visual childVisualization in visualizedChildren.Values)
        {
            RemoveChild(childVisualization);
        }
        
        visualizedChildren.Clear();
    }
    
    private void OnTemplateOwnerChildAdded(Object? sender, ChildAddedEventArgs e)
    {
        AddVisualization(e.Child);
    }
    
    private void OnTemplateOwnerChildRemoved(Object? sender, ChildRemovedEventArgs e)
    {
        RemoveVisualization(e.Child);
    }
    
    private void AddVisualization(Control child)
    {
        if (visualizedChildren.ContainsKey(child)) return;
        
        Visual childVisualization = child.Visualize();
        visualizedChildren[child] = childVisualization;
        AddChild(childVisualization);
    }
    
    private void RemoveVisualization(Control child)
    {
        if (visualizedChildren.Remove(child, out Visual? childVisualization))
        {
            RemoveChild(childVisualization);
        }
    }
}
