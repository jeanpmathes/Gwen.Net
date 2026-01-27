using System;
using System.Collections.Generic;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New;

/// <summary>
/// The base class of all GWEN elements, both visual elements and logical controls.
/// It defines the parent/child relationship of the visual and logical tree.
/// </summary>
public abstract class Element
{
    #region HIERARCHY
    
    private readonly List<Element> logicalChildren = [];

    private Boolean isAttachedToRoot;

    private protected Boolean IsAttachedToRoot => isAttachedToRoot;

    /// <summary>
    /// Whether this element is the root element of the tree.
    /// </summary>
    internal Boolean IsRoot { get; private set; }

    /// <summary>
    /// The logical parent of this element, or <c>null</c> if it has no logical parent.
    /// </summary>
    public Element? LogicalParent { get; private set; }

    /// <summary>
    /// The logical children of this element.
    /// </summary>
    public IReadOnlyList<Element> LogicalChildren => logicalChildren;
    
    /// <summary>
    /// Set the logical child of this element.
    /// Replaces any existing logical children.
    /// </summary>
    /// <param name="child">
    ///     The child to set. Will be removed from its previous parent if any.
    ///     If <c>null</c>, all existing logical children will be removed.
    /// </param>
    protected void SetLogicalChild(Element? child)
    {
        if (child?.LogicalParent == this && logicalChildren.Count == 1) return;

        child?.LogicalParent?.RemoveLogicalChild(child, isReparenting: true);

        if (logicalChildren.Count > 0)
        {
            List<Element> oldChildren = new(logicalChildren);
            logicalChildren.Clear();

            foreach (Element oldChild in oldChildren)
            {
                oldChild.LogicalParent = null;

                OnLogicalChildRemoved(oldChild);
                oldChild.Detach(isReparenting: false);
            }
        }

        if (child == null) return;

        logicalChildren.Add(child);
        child.LogicalParent = this;

        OnLogicalChildAdded(child);

        if (isAttachedToRoot) child.Attach();
    }

    /// <summary>
    /// Add a logical child to this element.
    /// </summary>
    /// <param name="child">The child to add. Will be removed from its previous parent if any.</param>
    protected void AddLogicalChild(Element child)
    {
        if (child.LogicalParent == this) return;

        child.LogicalParent?.RemoveLogicalChild(child, isReparenting: true);

        logicalChildren.Add(child);
        child.LogicalParent = this;

        OnLogicalChildAdded(child);

        if (isAttachedToRoot) child.Attach();
    }

    /// <summary>
    /// Remove a logical child from this element.
    /// If the specified child is not a logical child of this element, nothing happens.
    /// </summary>
    /// <param name="child">The child to remove.</param>
    protected void RemoveLogicalChild(Element child)
    {
        RemoveLogicalChild(child, isReparenting: false);
    }

    private void RemoveLogicalChild(Element child, Boolean isReparenting)
    {
        if (child.LogicalParent != this) return;

        if (!logicalChildren.Remove(child)) return;

        child.LogicalParent = null;

        OnLogicalChildRemoved(child);
        child.Detach(isReparenting);
    }

    private protected virtual void OnLogicalChildAdded(Element child) {}
    private protected virtual void OnLogicalChildRemoved(Element child) {}
    
    /// <summary>
    /// Set this element as root element.
    /// May only be called by <see cref="Canvas"/>.
    /// </summary>
    private protected void SetAsRoot()
    {
        IsRoot = true;

        Attach();
    }

    private protected void Attach()
    {
        if (isAttachedToRoot) return;
        isAttachedToRoot = true;

        OnAttach();

        foreach (Element child in logicalChildren)
        {
            child.Attach();
        }

        OnAttachInternal();
    }

    private protected virtual void OnAttachInternal() {}

    /// <summary>
    /// Called when the element is attached to a tree with a root element.
    /// Note that for example giving this element a parent does not necessarily
    /// attach it to a root element, as the parent itself may not be attached to a root.
    /// </summary>
    public virtual void OnAttach() {}

    private protected void Detach(Boolean isReparenting)
    {
        if (!isAttachedToRoot) return;
        isAttachedToRoot = IsRoot;
        if (isAttachedToRoot) return;

        OnDetach(isReparenting);

        foreach (Element child in logicalChildren)
        {
            child.Detach(isReparenting);
        }

        OnDetachInternal(isReparenting);
    }

    private protected virtual void OnDetachInternal(Boolean isReparenting) {}

    /// <summary>
    /// Called when the element is detached from a tree with a root element.
    /// Note that being detached in most cases does not mean losing the parent,
    /// as it may simply be that the parent or one of its ancestors was detached.
    /// </summary>
    /// <remarks>
    /// Generally, disposable resources must be disposed when being detached,
    /// unless the element is being reparented.
    /// </remarks>
    /// <param name="isReparenting">
    ///     Indicates whether the element is being detached because it is being reparented.
    /// </param>
    public virtual void OnDetach(Boolean isReparenting) {}
    
    #endregion HIERARCHY
    
    #region VISUALIZATION
    
    /// <summary>
    /// The visual representation of this element, or <c>null</c> if it has no visual representation.
    /// For controls, this property returns the root visual element of the control's template.
    /// For visual elements, this property returns the element itself.
    /// </summary>
    public VisualElement? Visualization
    {
        get
        {
            if (field == null)
                Visualization = GetOrCreateVisualization();

            // The visualization will be parented by the visual host of the logical parent, if any.

            return field;
        }

        private set;
    }

    private protected abstract VisualElement GetOrCreateVisualization();
    
    #endregion VISUALIZATION
}
