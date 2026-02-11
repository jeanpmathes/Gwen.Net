using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Gwen.Net.New.Bindings;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Styles;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Controls;

/// <summary>
/// The base class of all GWEN controls, meaning logical controls.
/// </summary>
public abstract class Control 
{
    /// <summary>
    /// Creates a new instance of the <see cref="Control"/> class.
    /// </summary>
    protected Control()
    {
        Foreground = Property.Create(this, Brushes.Black);
        Background = Property.Create(this, Brushes.Transparent);
        
        MinimumWidth = Property.Create(this, defaultValue: 1f);
        MinimumHeight = Property.Create(this, defaultValue: 1f);
        
        MaximumWidth = Property.Create(this, defaultValue: Single.PositiveInfinity);
        MaximumHeight = Property.Create(this, defaultValue: Single.PositiveInfinity);
    }
    
    #region PROPERTIES
    
    /// <summary>
    /// The preferred foreground brush of the control.
    /// </summary>
    public Property<Brush> Foreground { get; }
    
    /// <summary>
    /// The preferred background brush of the control.
    /// </summary>
    public Property<Brush> Background { get; }
    
    /// <summary>
    /// The minimum width of this control. Might not be respected by all layout containers.
    /// </summary>
    public Property<Single> MinimumWidth { get; }
    
    /// <summary>
    /// The minimum height of this control. Might not be respected by all layout containers.
    /// </summary>
    public Property<Single> MinimumHeight { get; }
    
    /// <summary>
    /// The maximum width of this control. Might not be respected by all layout containers.
    /// </summary>
    public Property<Single> MaximumWidth { get; }
        
    /// <summary>
    /// The maximum height of this control. Might not be respected by all layout containers.
    /// </summary>
    public Property<Single> MaximumHeight { get; }
    
    #endregion PROPERTIES
    
    #region HIERARCHY
    
    /// <summary>
    /// Whether this control is attached to a tree with a root control.
    /// </summary>
    protected Boolean IsAttachedToRoot { get; private set; }

    /// <summary>
    /// Whether this control is the root control of the tree.
    /// </summary>
    private protected Boolean IsRoot { get; set; }
    
    /// <summary>
    /// Set this control as the root of a tree.
    /// May only be called by the canvas controls.
    /// </summary>
    private protected void SetAsRoot()
    {
        IsRoot = true;

        Attach();
    }

    private readonly List<Control> children = [];
    
    /// <summary>
    /// The parent of this control.
    /// </summary>
    public Control? Parent { get; private set; }

    /// <summary>
    /// The children of this control.
    /// </summary>
    public IReadOnlyList<Control> Children => children;

    /// <summary>
    /// Set the child of this control.
    /// Replaces any existing children.
    /// </summary>
    /// <param name="child">
    ///     The child to set. Will be removed from its previous parent if any.
    ///     If <c>null</c>, all existing children will be removed.
    /// </param>
    protected void SetChild(Control? child)
    {
        if (child?.Parent == this && children.Count == 1) return;

        child?.Parent?.RemoveChild(child, isReparenting: true);

        if (children.Count > 0)
        {
            List<Control> oldChildren = new(children);
            children.Clear();

            foreach (Control oldChild in oldChildren)
            {
                oldChild.Parent = null;

                OnChildRemoved();
                oldChild.Detach(isReparenting: false);
            }
        }

        if (child == null) return;

        children.Add(child);
        child.Parent = this;

        if (IsAttachedToRoot)
            child.Attach();

        OnChildAdded();
    }

    /// <summary>
    /// Add a  child to this control.
    /// </summary>
    /// <param name="child">The child to add. Will be removed from its previous parent if any.</param>
    protected void AddChild(Control child)
    {
        if (child.Parent == this) return;

        child.Parent?.RemoveChild(child, isReparenting: true);

        children.Add(child);
        child.Parent = this;

        if (IsAttachedToRoot)
            child.Attach();

        OnChildAdded();
    }

    /// <summary>
    /// Remove a child from this control.
    /// If the specified child is not a child of this control, nothing happens.
    /// </summary>
    /// <param name="child">The child to remove.</param>
    protected void RemoveChild(Control child)
    {
        RemoveChild(child, isReparenting: false);
    }

    private void RemoveChild(Control child, Boolean isReparenting)
    {
        if (child.Parent != this) return;

        if (!children.Remove(child)) return;

        child.Parent = null;

        OnChildRemoved();
        child.Detach(isReparenting);
    }

    private void OnChildAdded()
    {
        ChildAdded?.Invoke(this, EventArgs.Empty);
    }
    
    /// <summary>
    /// Invoked when a child is added to this control.
    /// </summary>
    public event EventHandler? ChildAdded;
    
    private void OnChildRemoved()
    {
        ChildRemoved?.Invoke(this, EventArgs.Empty);
    }
    
    /// <summary>
    /// Invoked when a child is removed from this control.
    /// </summary>
    public event EventHandler? ChildRemoved;

    private void Attach()
    {
        if (IsAttachedToRoot) return;
        IsAttachedToRoot = true;

        InvalidateContext();
        OnAttach();
        AttachedToRoot?.Invoke(this, EventArgs.Empty);

        foreach (Control child in children)
        {
            child.Attach();
        }
    }

    /// <summary>
    /// Called when the control is attached to a tree with a root control.
    /// Note that for example giving this control a parent does not necessarily
    /// attach it to a root control, as the parent itself may not be attached to a root.
    /// </summary>
    public virtual void OnAttach() {}

    /// <summary>
    /// Invoked when this control is attached to a tree with a root control.
    /// </summary>
    public event EventHandler? AttachedToRoot;

    private void Detach(Boolean isReparenting)
    {
        if (!IsAttachedToRoot) return;
        IsAttachedToRoot = IsRoot;
        if (IsAttachedToRoot) return;

        InvalidateContext();
        OnDetach(isReparenting);
        DetachedFromRoot?.Invoke(this, EventArgs.Empty);

        foreach (Control child in children)
        {
            child.Detach(isReparenting);
        }
    }

    /// <summary>
    /// Called when the control is detached from a tree with a root control.
    /// Note that being detached in most cases does not mean losing the parent,
    /// as it may simply be that the parent or one of its ancestors was detached.
    /// </summary>
    /// <remarks>
    /// Generally, disposable resources must be disposed when being detached,
    /// unless the control is being reparented.
    /// </remarks>
    /// <param name="isReparenting">
    ///     Indicates whether the control is being detached because it is being reparented.
    /// </param>
    public virtual void OnDetach(Boolean isReparenting) {}

    /// <summary>
    /// Invoked when this control is detached from a tree with a root control.
    /// </summary>
    public event EventHandler? DetachedFromRoot;

    #endregion HIERARCHY
    
    #region CONTEXT

    private readonly Context? localContext;
    private Context? cachedContext;
    
    /// <summary>
    /// Rebuild the cached context for this control and reapply styling.
    /// </summary>
    private void InvalidateContext()
    {
        UpdateCachedContext();
        
        InvalidateStyling();
        
        OnInvalidateContext();
    }
    
    /// <summary>
    /// Override to react to context changes.
    /// </summary>
    protected virtual void OnInvalidateContext()
    {
        
    }
    
    [MemberNotNull(nameof(cachedContext))]
    private void UpdateCachedContext()
    {
        Context parentContext = Parent?.Context ?? Context.Default;
        
        cachedContext = localContext == null
            ? parentContext
            : new Context(localContext, parentContext);
    }

    /// <summary>
    /// The context of this control, which is used for example to determine styling.
    /// </summary>
    public Context Context
    {
        get
        {
            if (cachedContext != null)
                return cachedContext;

            UpdateCachedContext();

            return cachedContext;
        }

        init
        {
            localContext = value;
            
            UpdateCachedContext();
        }
    }

    #endregion CONTEXT
    
    #region VISUALIZATION / TEMPLATING
    
    /// <summary>
    /// Build or refresh the visual tree that represents this control.
    /// </summary>
    /// <returns>The root visual used to render this control.</returns>
    internal abstract Visual Visualize();
    
    #endregion VISUALIZATION / TEMPLATING
    
    #region STYLE
    
    /// <summary>
    /// Invalidate styling of this control, causing it to be reapplied.
    /// </summary>
    protected abstract void InvalidateStyling();
    
    #endregion
}

/// <summary>
/// The generic base class of all GWEN controls, meaning logical controls.
/// The generic variant is needed for templating.
/// </summary>
/// <typeparam name="TSelf">The type of the control itself.</typeparam>
public abstract class Control<TSelf> : Control where TSelf : Control<TSelf>
{
    /// <summary>
    /// Creates a new instance of the <see cref="Control{TSelf}"/> class.
    /// </summary>
    protected Control()
    {
        Template = Property.Create(this, Binding.Computed(CreateDefaultTemplate));
        Template.ValueChanged += OnTemplateChanged;
        
        Style = Property.Create(this,(Style<TSelf>?)null);
        Style.ValueChanged += OnStyleChanged;
    }
    
    /// <summary>
    /// Get this control as its own type.
    /// </summary>
    protected TSelf Self => (TSelf) this;
    
    #region VISUALIZATION / TEMPLATING
    
    /// <summary>
    /// The template used to visualize this control.
    /// </summary>
    public Property<ControlTemplate<TSelf>> Template { get; }
    
    /// <summary>
    /// The current root visual that represents this control, if it has been visualized.
    /// </summary>
    internal Visual? Visualization { get; private set; }

    /// <summary>
    /// Build or refresh the visual tree for this control and apply current styling.
    /// </summary>
    /// <returns>The root visual used to render this control.</returns>
    internal override Visual Visualize()
    {
        UnanchorVisualization();
        
        ApplyStyling();
        
        Visualization = Template.GetValue().Apply(Self);
        
        AnchorVisualization();
        
        return Visualization;
    }
    
    private void AnchorVisualization()
    {
        Visualization?.SetAsAnchor(this);
        
        if (IsRoot)
            Visualization?.SetAsRoot();
    }

    private void UnanchorVisualization()
    {
        if (Visualization == null) return;

        if (IsRoot)
            Visualization.UnsetAsRoot();
        
        Visualization.UnsetAsAnchor();
        Visualization = null;
    }
    
    /// <summary>
    /// Create a default template for this control, which is used if no style or local template is set.
    /// </summary>
    /// <returns>The default control template.</returns>
    protected abstract ControlTemplate<TSelf> CreateDefaultTemplate();
    
    private void OnTemplateChanged(Object? sender, EventArgs e)
    {
        InvalidateVisualization();
    }
    
    private void InvalidateVisualization()
    {
        if (Visualization != null)
            Visualize();
    }
    
    #endregion VISUALIZATION / TEMPLATING
    
    #region STYLE

    private IReadOnlyList<IStyle<TSelf>>? usedOuterStyles;
    private Style<TSelf>? usedLocalStyle;
    
    private Boolean IsStyled => usedOuterStyles != null || usedLocalStyle != null;
    
    /// <summary>
    /// Set a specific style just for this control, which overrides any styling from the context.
    /// This style does not affect any other controls.
    /// </summary>
    public Property<Style<TSelf>?> Style { get; }
    
    private void OnStyleChanged(Object? sender, EventArgs e)
    {
        InvalidateStyling();
    }

    /// <inheritdoc/>
    protected sealed override void InvalidateStyling()
    {
        if (!IsStyled) return;
        
        ApplyStyling();
    }
    
    private void ApplyStyling()
    {
        ClearStyling();
        
        usedOuterStyles = Context.GetStyling<TSelf>();
        if (usedOuterStyles.Count > 0)
        {
            foreach (IStyle<TSelf> style in usedOuterStyles)
            {
                style.Apply(Self);
            }
        }

        usedLocalStyle = Style.GetValue();
        usedLocalStyle?.Apply(Self);
    }

    private void ClearStyling()
    {
        if (!IsStyled) return;

        usedLocalStyle?.Clear(Self);
        usedLocalStyle = null;

        if (usedOuterStyles == null) return;

        for (Int32 index = usedOuterStyles.Count - 1; index >= 0; index--)
        {
            usedOuterStyles[index].Clear(Self);
        }
        
        usedOuterStyles = null;
    }

    #endregion STYLE
}
