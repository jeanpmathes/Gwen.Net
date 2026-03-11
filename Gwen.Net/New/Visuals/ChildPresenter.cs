using System;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Controls.Internals;

namespace Gwen.Net.New.Visuals;

/// <summary>
///     Presents a single child of the template owner, or nothing if the owner has no child.
///     This should be in templates of <see cref="SingleChildControl{TControl}" /> controls, and will visualize the child
///     control of the owner if it exists.
/// </summary>
public class ChildPresenter : Visual
{
    private Control? visualizedChild;
    private Visual? childVisualization;

    /// <summary>
    ///     Constructs a new instance of the <see cref="ChildPresenter" /> class.
    /// </summary>
    public ChildPresenter()
    {
        Visibility.Set(New.Visibility.Collapsed);
    }

    /// <inheritdoc />
    public override void OnAttach()
    {
        Control? templateOwner = TemplateOwner.GetValue();
        if (templateOwner == null) return;

        templateOwner.ChildAdded += OnTemplateOwnerChildAdded;
        templateOwner.ChildRemoved += OnTemplateOwnerChildRemoved;

        Boolean isReparenting = childVisualization != null;
        if (isReparenting) return;

        if (templateOwner.Children.Count.GetValue() == 0) return;

        Control child = templateOwner.Children[0];
        UpdateVisualization(child);
    }

    /// <inheritdoc />
    public override void OnDetach(Boolean isReparenting)
    {
        Control? templateOwner = TemplateOwner.GetValue();
        if (templateOwner == null) return;

        templateOwner.ChildAdded -= OnTemplateOwnerChildAdded;
        templateOwner.ChildRemoved -= OnTemplateOwnerChildRemoved;

        if (isReparenting) return;

        RemoveVisualization();
    }

    private void OnTemplateOwnerChildAdded(Object? sender, ChildAddedEventArgs e)
    {
        if (sender is not Control templateOwner) return;
        if (templateOwner.Children.Count.GetValue() == 0) return;

        Control child = templateOwner.Children[0];
        UpdateVisualization(child);
    }

    private void OnTemplateOwnerChildRemoved(Object? sender, ChildRemovedEventArgs e)
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

        RemoveVisualization();

        if (child == null) return;

        AddVisualization(child);
    }

    private void AddVisualization(Control child)
    {
        Visibility.Clear();

        visualizedChild = child;
        childVisualization = child.Visualize();

        AddChild(childVisualization);
    }

    private void RemoveVisualization()
    {
        Visibility.Set(New.Visibility.Collapsed);

        // todo: these set / clear methods are only used here and in ChildrenPresenter
        // todo: maybe that system can be completely removed from visual properties??
        // todo: this would make VisualProperty simpler, at the cost of having children presenters be visible when empty
        // todo: and the only problem with that would be that they render their boundaries in debug mode

        visualizedChild = null;
        if (childVisualization == null) return;

        RemoveChild(childVisualization);
        childVisualization = null;
    }
}
