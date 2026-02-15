using System;
using Gwen.Net.New.Controls;

namespace Gwen.Net.New.Visuals;

/// <summary>
/// Presents a single child of the template owner, or nothing if the owner has no child.
/// </summary>
public class ChildPresenter : Visual
{
    private Control? visualizedChild;
    private Visual? childVisualization;
    
    /// <inheritdoc/>
    public override void OnAttach()
    {
        Control? templateOwner = TemplateOwner.GetValue();
        if (templateOwner == null) return;
        
        templateOwner.ChildAdded += OnTemplateOwnerChildAdded;
        templateOwner.ChildRemoved += OnTemplateOwnerChildRemoved;
        
        if (templateOwner.Children.Count.GetValue() == 0) return;
        
        Control child = templateOwner.Children[0];
        UpdateVisualization(child);
    }
    
    /// <inheritdoc/>
    public override void OnDetach(Boolean isReparenting)
    {
        Control? templateOwner = TemplateOwner.GetValue();
        if (templateOwner == null) return;
        
        templateOwner.ChildAdded -= OnTemplateOwnerChildAdded;
        templateOwner.ChildRemoved -= OnTemplateOwnerChildRemoved;
    }

    private void OnTemplateOwnerChildAdded(Object? sender, EventArgs e)
    {
        if (sender is not Control templateOwner) return;
        if (templateOwner.Children.Count.GetValue() == 0) return;
        
        Control child = templateOwner.Children[0];
        UpdateVisualization(child);
    }
    
    private void OnTemplateOwnerChildRemoved(Object? sender, EventArgs e)
    {
        if (sender is not Control templateOwner) return;

        if (templateOwner.Children.Count.GetValue() > 0)
        {
            Control child = templateOwner.Children[0];
            UpdateVisualization(child);
        }
        else
        {
            UpdateVisualization(null);
        }
    }
    
    private void UpdateVisualization(Control? child)
    {
        if (child == visualizedChild) return;
        
        DetachVisualization();
        
        if (child == null) return;
        
        AttachVisualization(child);
    }
    
    private void AttachVisualization(Control child)
    {
        visualizedChild = child;
        childVisualization = child.Visualize();
        
        AddChild(childVisualization);
    }
    
    private void DetachVisualization()
    {
        visualizedChild = null;
        if (childVisualization == null) return;
        
        RemoveChild(childVisualization);
        childVisualization = null;
    }
}
