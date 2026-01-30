using System;
using System.Collections.Generic;
using System.Drawing;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Rendering;
using Gwen.Net.New.Utilities;

namespace Gwen.Net.New.Visuals;

/// <summary>
/// The base class of all visual elements.
/// Visual elements can be measured, arranged and rendered.
/// </summary>
public abstract class VisualElement : Element
{
    /// <summary>
    /// Creates a new instance of the <see cref="VisualElement"/> class.
    /// </summary>
    protected VisualElement()
    {
        InvalidateMeasure();
    }

    #region LOOK

    /// <summary>
    /// Gets or sets the background brush of this element.
    /// </summary>
    public Brush Background
    {
        get;

        set
        {
            field = value;
            InvalidateRender();
        }
    }

    #endregion LOOK
    
    #region HIERARCHY
    
    private readonly List<VisualElement> visualChildren = [];

    /// <summary>
    /// The visual parent of this element, or <c>null</c> if it has no visual parent.
    /// </summary>
    public VisualElement? VisualParent { get; private set; }

    /// <summary>
    /// The visual children of this element.
    /// </summary>
    public IReadOnlyList<VisualElement> VisualChildren => visualChildren;
    
    /// <summary>
    /// Set the visual child of this element.
    /// Replaces any existing visual children.
    /// </summary>
    /// <param name="child">
    ///     The child to set. Will be removed from its previous parent if any.
    ///     If <c>null</c>, all existing visual children will be removed.
    /// </param>
    protected void SetVisualChild(VisualElement? child)
    {
        if (child?.VisualParent == this && visualChildren.Count == 1) return;

        child?.VisualParent?.RemoveVisualChild(child, isReparenting: true);

        if (visualChildren.Count > 0)
        {
            List<VisualElement> oldChildren = new(visualChildren);
            visualChildren.Clear();

            foreach (VisualElement oldChild in oldChildren)
            {
                oldChild.VisualParent = null;

                OnVisualChildRemoved(oldChild);
                oldChild.Detach(isReparenting: false);
            }
        }

        if (child == null) return;

        visualChildren.Add(child);
        child.VisualParent = this;

        OnVisualChildAdded(child);

        if (IsAttachedToRoot) child.Attach();
    }

    /// <summary>
    /// Remove all visual children from this element.
    /// </summary>
    protected void RemoveAllVisualChildren()
    {
        SetVisualChild(child: null);
    }

    /// <summary>
    /// Add a visual child to this element.
    /// </summary>
    /// <param name="child">The child to add. Will be removed from its previous parent if any.</param>
    protected void AddVisualChild(VisualElement child)
    {
        if (child.VisualParent == this) return;

        child.VisualParent?.RemoveVisualChild(child);

        visualChildren.Add(child);
        child.VisualParent = this;

        OnVisualChildAdded(child);

        if (IsAttachedToRoot) child.Attach();
    }

    /// <summary>
    /// Remove a visual child from this element.
    /// If the specified child is not a visual child of this element, nothing happens.
    /// </summary>
    /// <param name="child">The child to remove.</param>
    protected void RemoveVisualChild(VisualElement child)
    {
        RemoveVisualChild(child, isReparenting: false);
    }

    private void RemoveVisualChild(VisualElement child, Boolean isReparenting)
    {
        if (child.VisualParent != this) return;

        if (!visualChildren.Remove(child)) return;

        child.VisualParent = null;

        OnVisualChildRemoved(child);
        child.Detach(isReparenting);
    }

    private protected virtual void OnVisualChildAdded(VisualElement child) {}
    private protected virtual void OnVisualChildRemoved(VisualElement child) {}

    private protected sealed override void OnAttachInternal()
    {
        foreach (VisualElement child in visualChildren)
            child.Attach();
    }

    private protected sealed override void OnDetachInternal(Boolean isReparenting)
    {
        foreach (VisualElement child in visualChildren)
            child.Detach(isReparenting);
    }

    #endregion HIERARCHY
    
    #region POSITIONING
    
    /// <summary>
    /// The bounds of this element.
    /// </summary>
    public RectangleF Bounds { get; private set; }
    
    /// <summary>
    /// The bounds with negative offset applied, zeroing them to (0,0).
    /// </summary>
    protected RectangleF RenderBounds => new(PointF.Empty, Bounds.Size);
    
    /// <summary>
    ///     Location of the element. Valid after arranging.
    /// </summary>
    public Single ActualLeft => Bounds.X;

    /// <summary>
    ///     Location of the element. Valid after arranging.
    /// </summary>
    public Single ActualTop => Bounds.Y;
    
    /// <summary>
    ///     The actual location of the element. Valid after arranging.
    /// </summary>
    public PointF ActualLocation => Bounds.Location;

    /// <summary>
    ///     The actual width of the element. Valid after arranging.
    /// </summary>
    public Single ActualWidth => Bounds.Width;

    /// <summary>
    ///     The actual height of the element. Valid after arranging.
    /// </summary>
    public Single ActualHeight => Bounds.Height;
    
    /// <summary>
    ///     The actual size of the element. Valid after arranging.
    /// </summary>
    public SizeF ActualSize => Bounds.Size;
    
    /// <summary>
    ///     Sets the size of the element.
    /// </summary>
    /// <param name="size">New size.</param>
    /// <returns>True if bounds changed.</returns>
    /// <remarks>Bounds are reset after the next layout pass.</remarks>
    public virtual Boolean SetSize(SizeF size)
    {
        return SetBounds(Bounds with { Size = size });
    }

    /// <summary>
    ///     Sets the control bounds.
    /// </summary>
    /// <param name="newBounds">New bounds.</param>
    /// <returns>True if bounds changed.</returns>
    /// <remarks>Bounds are reset after the next layout pass.</remarks>
    public virtual Boolean SetBounds(RectangleF newBounds)
    {
        if (Bounds == newBounds) return false;
        
        RectangleF oldBounds = Bounds;
        Bounds = newBounds;
        
        OnBoundsChanged(oldBounds, newBounds);
        
        InvalidateRender();
        
        return true;
    }

    /// <summary>
    /// Called when the bounds of this element have changed.
    /// </summary>
    /// <param name="oldBounds">The old bounds.</param>
    /// <param name="newBounds">The new bounds.</param>
    public virtual void OnBoundsChanged(RectangleF oldBounds, RectangleF newBounds)
    {
        
    }
    
    #endregion POSITIONING
    
    #region LAYOUTING

    /// <summary>
    /// The minimum size of this element. Might not be respected by all layout containers.
    /// </summary>
    public SizeF MinimumSize { get; set; } = Sizes.One;
    
    /// <summary>
    /// The maximum size of this element. Might not be respected by all layout containers.
    /// </summary>
    public SizeF MaximumSize { get; set; } = Sizes.Infinity;
    
    private Boolean isMeasureValid;
    private Boolean isArrangeValid;

    /// <summary>
    /// The size this element measured itself to be during the last measure pass.
    /// This is effectively the minimum size this element requires to render itself properly.
    /// </summary>
    public SizeF MeasuredSize { get; private set; } = SizeF.Empty;
    
    private SizeF lastAvailableSize = SizeF.Empty;

    /// <summary>
    /// Measure the desired size of this element given the available size.
    /// </summary>
    /// <param name="availableSize">The available size. The element does not have to use all of this size.</param>
    /// <returns>The desired size required by this element, might be larger than the available size.</returns>
    public SizeF Measure(SizeF availableSize)
    {
        if (isMeasureValid && lastAvailableSize == availableSize)
            return MeasuredSize;
        
        lastAvailableSize = availableSize;
        
        availableSize = Sizes.Clamp(availableSize, MinimumSize, MaximumSize);
        
        MeasuredSize = OnMeasure(availableSize);
        
        MeasuredSize = Sizes.Clamp(MeasuredSize, MinimumSize, MaximumSize);
        
        isMeasureValid = true;
        isArrangeValid = false;

        return MeasuredSize;
    }
    
    /// <summary>
    /// Override to define custom measuring logic. See <see cref="Measure(SizeF)"/>.
    /// If you override this, you will likely need to override <see cref="OnArrange(RectangleF)"/> as well and vice versa.
    /// </summary>
    /// <param name="availableSize">The available size. The element does not have to use all of this size.</param>
    /// <returns>The desired size required by this element, might be larger than the available size.</returns>
    public virtual SizeF OnMeasure(SizeF availableSize)
    {
        if (visualChildren.Count == 0)
            return SizeF.Empty;
        
        var desiredSize = SizeF.Empty;
        
        foreach (VisualElement child in visualChildren)
        {
            SizeF childDesiredSize = child.Measure(availableSize);
            
            desiredSize.Width = Math.Max(desiredSize.Width, childDesiredSize.Width);
            desiredSize.Height = Math.Max(desiredSize.Height, childDesiredSize.Height);
        }
        
        return desiredSize;
    }

    /// <summary>
    /// Arrange this element within the given final rectangle.
    /// This will set the <see cref="Bounds"/> of this element.
    /// </summary>
    /// <param name="finalRectangle">The final rectangle to arrange this element in.</param>
    public void Arrange(RectangleF finalRectangle)
    {
        if (isArrangeValid) return;
        
        if (!isMeasureValid)
            Measure(finalRectangle.Size);
        
        RectangleF allowedRectangle = Rectangles.ConstraintSize(finalRectangle, MaximumSize);
        
        RectangleF arrangedRectangle = OnArrange(allowedRectangle);
        
        arrangedRectangle = Rectangles.ConstraintSize(arrangedRectangle, allowedRectangle.Size);
        
        SetBounds(arrangedRectangle);
        
        // SetBounds might invalidate something, so we set everything to valid again.
        
        isArrangeValid = true;
        isMeasureValid = true;
    }
    
    /// <summary>
    /// Override to define custom arranging logic. See <see cref="Arrange(RectangleF)"/>.
    /// If you have overriden <see cref="OnMeasure(SizeF)"/>, you will likely need to override this as well and vice versa.
    /// </summary>
    /// <param name="finalRectangle">The final rectangle to arrange this element in.</param>
    /// <returns>The actual rectangle used by this element after arranging.</returns>
    public virtual RectangleF OnArrange(RectangleF finalRectangle)
    {
        if (visualChildren.Count == 0)
            return Rectangles.ConstraintSize(finalRectangle, MeasuredSize);
        
        foreach (VisualElement child in visualChildren)
            child.Arrange(Rectangles.ConstraintSize(finalRectangle, child.MeasuredSize));

        return finalRectangle;
    }

    /// <summary>
    /// Invalidate the measurement of this element, causing a re-measure and re-arrange.
    /// </summary>
    public void InvalidateMeasure()
    {
        if (!isMeasureValid) return;
        
        isMeasureValid = false;
        isArrangeValid = false;
        
        VisualParent?.InvalidateMeasure();
    }

    /// <summary>
    /// Invalidate the arrangement of this element, causing a re-arrange.
    /// </summary>
    public void InvalidateArrange()
    {
        if (!isArrangeValid) return;
        
        isArrangeValid = false;
        
        VisualParent?.InvalidateArrange();
    }
    
    #endregion LAYOUTING

    #region RENDERING

    /// <summary>
    /// Invalidate the rendering of this element, causing a re-render.
    /// </summary>
    public void InvalidateRender()
    {
        VisualParent?.InvalidateRender();
    }

    /// <summary>
    ///     Determines whether the control should be clipped to its bounds while rendering.
    /// </summary>
    protected virtual Boolean ShouldClip => true;
    
    /// <summary>
    /// Render this element using the specified renderer.
    /// </summary>
    /// <param name="renderer">The renderer to use.</param>
    public virtual void Render(IRenderer renderer)
    {
        PrepareRender();
        
        renderer.PushOffset(Bounds.Location);
        
        DoRender();

        if (ShouldClip)
            renderer.PopClip();
        
        renderer.PopOffset();

        // todo: debug outlines, maybe through context

        void DoRender()
        {
            if (ShouldClip)
            {
                renderer.PushClip(Bounds);
            
                if (renderer.IsClipEmpty()) return;
                
                renderer.BeginClip();
            }
        
            OnRender(renderer);

            foreach (VisualElement child in visualChildren)
            {
                child.Render(renderer);
            }
        }
    }

    private void PrepareRender()
    {
        // Generally, measure and arrange bubble up to the root, which means the measure and arrange
        // is performed at most once for the entire visual tree.
        // However, layout boundaries would prevent bubbling up beyond them, so they must also ensure
        // that measure and arrange are valid before rendering.
        
        if (!isMeasureValid)
            Measure(Bounds.Size);
        
        if (!isArrangeValid)
            Arrange(Bounds);
    }
    
    /// <summary>
    /// Called when this element should render itself.
    /// Override this to implement custom rendering logic.
    /// </summary>
    /// <param name="renderer">The renderer to use.</param>
    public virtual void OnRender(IRenderer renderer) {}

    #endregion RENDERING
    
    #region TEMPLATING

    /// <summary>
    /// The control that owns the template this element is the root of, or <c>null</c> if this element is not part of a control template or is not the root of such a template.
    /// </summary>
    public Control? TemplateOwner { get; internal set; }
    
    private protected override VisualElement GetOrCreateVisualization() => this;

    #endregion TEMPLATING
}
