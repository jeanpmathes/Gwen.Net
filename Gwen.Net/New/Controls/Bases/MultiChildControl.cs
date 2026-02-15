using System;
using System.Collections.Generic;
using Gwen.Net.New.Bindings;

namespace Gwen.Net.New.Controls.Bases;

/// <summary>
/// A control which can have multiple child controls.
/// </summary>
/// <typeparam name="TControl">The concrete type of the control.</typeparam>
public abstract class MultiChildControl<TControl> : Control<TControl> where TControl : MultiChildControl<TControl>
{
    private ListSlot<Control> localChildren = [];
    
    private Boolean isUpdatingChildren;

    /// <summary>
    /// Creates a new instance of the <see cref="MultiChildControl{TControl}"/> class.
    /// </summary>
    protected MultiChildControl()
    {
        ChildAdded += OnChildAdded;
        ChildRemoved += OnChildRemoved;
        
        localChildren.CollectionChanged += OnLocalChildrenChanged;
    }

    private void OnChildAdded(Object? sender, ChildAddedEventArgs e)
    {
        if (isUpdatingChildren) return;

        try
        {
            isUpdatingChildren = true;

            localChildren.Add(e.Child);
        }
        finally
        {
            isUpdatingChildren = false;
        }
    }
    
    private void OnChildRemoved(Object? sender, ChildRemovedEventArgs e)
    {
        if (isUpdatingChildren) return;

        try
        {
            isUpdatingChildren = true;

            localChildren.Remove(e.Child);
        }
        finally
        {
            isUpdatingChildren = false;
        }
    }

    private void OnLocalChildrenChanged(Object? sender, CollectionChangedEventArgs<Control> e)
    {
        if (isUpdatingChildren) return;

        try
        {
            isUpdatingChildren = true;
        
            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    foreach (Control child in e.NewItems)
                    {
                        if (child.Parent == this)
                        {
                            localChildren.Remove(child);
                            continue;
                        }

                        AddChild(child);
                    }
                    break;
            
                case CollectionChangeAction.Remove:
                    foreach (Control child in e.OldItems)
                    {
                        RemoveChild(child);
                    }
                    break;
            
                case CollectionChangeAction.Replace:
                case CollectionChangeAction.Move:
                case CollectionChangeAction.Reorder:
                default:
                    throw new NotSupportedException();
            }
        }
        finally
        {
            isUpdatingChildren = false;
        }
    }

    /// <summary>
    /// Get or sets the child controls of this control. Setting this property will replace all existing child controls with the new collection of child controls.
    /// It is possible to pass a <see cref="ListSlot{Control}"/> to this property, in which case the <see cref="Children"/> property will be directly bound to the provided <see cref="ListSlot{Control}"/> and any changes to the collection will automatically update the child controls of this control.
    /// However, not all operations on the <see cref="ListSlot{Control}"/> are supported. Only adding and removing items from the collection is supported. Replacing, moving, or reordering items in the collection is not supported and will throw a <see cref="NotSupportedException"/>.
    /// </summary>
    public new IList<Control> Children
    {
        get => localChildren;
        
        set
        {
            if (ReferenceEquals(localChildren, value)) return;

            localChildren.Clear();

            if (value is ListSlot<Control> newChildren)
            {
                localChildren.CollectionChanged -= OnLocalChildrenChanged;

                localChildren = newChildren;
                localChildren.CollectionChanged += OnLocalChildrenChanged;

                try
                {
                    isUpdatingChildren = true;

                    foreach (Control child in localChildren)
                    {
                        AddChild(child);
                    }
                }
                finally
                {
                    isUpdatingChildren = false;
                }
            }
            else
            {
                foreach (Control child in value)
                {
                    AddChild(child);
                }
            }
        }
    }
}
