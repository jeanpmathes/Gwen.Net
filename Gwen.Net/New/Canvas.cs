using System;
using System.Drawing;
using Gwen.Net.New.Rendering;
using Gwen.Net.New.Visuals.Layout;

namespace Gwen.Net.New;

/// <summary>
/// The root element for a GWEN user interface.
/// </summary>
public sealed class Canvas : Panel, IDisposable
{
    private readonly IRenderer onlyRenderer;
    
    private Single scale = 1.0f;
    private SizeF viewportSize = SizeF.Empty;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Canvas"/> class.
    /// </summary>
    /// <param name="renderer">
    ///     The only renderer which will be used for rendering this canvas.
    ///     Will not be disposed by the canvas.
    /// </param>
    public Canvas(IRenderer renderer)
    {
        onlyRenderer = renderer;
        
        SetAsRoot();
    }

    /// <summary>
    /// Gets or sets the single child element.
    /// </summary>
    public Element? Child
    {
        get => LogicalChildren.Count > 0 ? LogicalChildren[0] : null;
        set => SetLogicalChild(value);
    } 
    
    /// <inheritdoc/>
    public override void OnBoundsChanged(RectangleF oldBounds, RectangleF newBounds)
    {
        InvalidateMeasure();
    }

    /// <summary>
    /// Set the scale of the canvas.
    /// </summary>
    /// <param name="newScale">The scale factor.</param>
    public void SetScale(Single newScale)
    {
        scale = newScale;
        
        onlyRenderer.Scale(newScale);
        
        UpdateSize();
        
        InvalidateRender();
    }
    
    /// <summary>
    /// Set the size of the rendering viewport.
    /// </summary>
    /// <param name="newSize">The size of the viewport.</param>
    public void SetRenderingSize(Size newSize)
    {
        viewportSize = newSize;
        
        onlyRenderer.Resize(newSize);
        
        UpdateSize();
    }

    private void UpdateSize()
    {
        SetSize(viewportSize / scale);
    }

    /// <summary>
    /// Render the canvas.
    /// </summary>
    public void Render()
    {
        Render(onlyRenderer);
    }
    
    /// <inheritdoc/>
    public override void Render(IRenderer renderer)
    {
        if (renderer != onlyRenderer)
            throw new InvalidOperationException("Canvas can only be rendered with the initial renderer it was created with.");
        
        renderer.Begin();
        
        renderer.PushOffset(Point.Empty);
        renderer.PushClip(Bounds);
        
        base.Render(renderer);
        
        renderer.EndClip();
        
        renderer.PopClip();
        renderer.PopOffset();
        
        renderer.End();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (Child != null) 
            RemoveLogicalChild(Child);
    }
}
