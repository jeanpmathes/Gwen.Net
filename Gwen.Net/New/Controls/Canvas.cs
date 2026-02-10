using System;
using System.Drawing;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Rendering;
using Gwen.Net.New.Resources;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Controls;

/// <summary>
/// The root control for a GWEN user interface.
/// </summary>
/// <seealso cref="Visuals.Canvas"/>
public sealed class Canvas : Control<Canvas>, IDisposable
{
    private readonly IRenderer onlyRenderer;
    
    private Single scale = 1.0f;
    private SizeF viewportSize = SizeF.Empty;
    
    /// <summary>
    /// Create a new canvas with the given renderer and registry.
    /// </summary>
    /// <param name="renderer">
    ///     The only renderer which will be used for rendering this canvas.
    ///     Will not be disposed by the canvas.
    /// </param>
    /// <param name="registry">
    ///     The registry to create the canvas context from.
    ///     If a UI theme is used, the registry should contain the theme's styles.
    ///     Will not be disposed by the canvas.
    /// </param>
    /// <returns>A new canvas instance.</returns>
    public static Canvas Create(IRenderer renderer, ResourceRegistry registry)
    {
        Canvas canvas = new(renderer)
        {
            Context = new Context(registry)
        };
        
        canvas.SetAsRoot();
        canvas.Visualize();
        
        return canvas;
    }
    
    private Canvas(IRenderer renderer)
    {
        onlyRenderer = renderer;
    }

    /// <summary>
    /// Gets or sets the single child control.
    /// </summary>
    public Control? Child
    {
        get => Children.Count > 0 ? Children[0] : null;
        set => SetChild(value);
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
        
        Visualization?.InvalidateRender();
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
        Visualization?.SetSize(viewportSize / scale);
    }

    /// <summary>
    /// Render the canvas.
    /// </summary>
    public void Render()
    {
        Visualization?.Render(onlyRenderer);
    }
    
    /// <inheritdoc/>
    public void Dispose()
    {
        if (Child != null) 
            RemoveChild(Child);
    }

    /// <inheritdoc />
    protected override ControlTemplate<Canvas> CreateDefaultTemplate()
    {
        return ControlTemplate.Create<Canvas>(_ => new Visuals.Canvas
        {
            Child = new ChildPresenter()
        });
    }
}
