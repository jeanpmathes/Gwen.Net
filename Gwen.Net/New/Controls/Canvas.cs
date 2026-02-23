using System;
using System.Drawing;
using Gwen.Net.New.Bindings;
using Gwen.Net.New.Controls.Internals;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Input;
using Gwen.Net.New.Rendering;
using Gwen.Net.New.Resources;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Controls;

/// <summary>
/// The root control for a GWEN user interface.
/// </summary>
/// <seealso cref="Visuals.Canvas"/>
public sealed class Canvas : SingleChildControl<Canvas>, IDisposable
{
    private readonly IRenderer onlyRenderer;

    private readonly Binding<InputHandler?> inputBinding;

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
        Canvas canvas = new(renderer, registry);
        
        canvas.SetAsRoot(renderer);
        canvas.Visualize();
        
        return canvas;
    }
    
    private Canvas(IRenderer renderer, ResourceRegistry registry)
    {
        onlyRenderer = renderer;

        inputBinding = Binding.Transform(Visualization,
            visualization =>
            {
                if (visualization is Visuals.Canvas canvas)
                    return canvas.Input;
                
                return null;
            });
        
        Context = new Context(registry, this);
    }

    /// <summary>
    /// Get the input handler for this canvas, or null if the canvas is not visualized.
    /// </summary>
    public InputHandler? Input => inputBinding.GetValue();

    /// <summary>
    /// Get the current scale of the canvas.
    /// Use <see cref="SetScale"/> to change the scale.
    /// </summary>
    public Single Scale { get; private set; } = 1.0f;

    /// <summary>
    /// Set the scale of the canvas.
    /// </summary>
    /// <param name="newScale">The scale factor, must be greater than zero.</param>
    public void SetScale(Single newScale)
    {
        if (newScale <= 0)
            throw new ArgumentOutOfRangeException(nameof(newScale), newScale, "Scale must be greater than zero.");
        
        Scale = newScale;
        
        onlyRenderer.Scale(newScale);
        
        UpdateSize();
        
        Visualization.GetValue()?.InvalidateRender();
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
    
    /// <summary>
    /// Whether to draw debug outlines for controls.
    /// </summary>
    /// <param name="enabled">True to draw debug outlines, false to disable them.</param>
    public void SetDebugOutlines(Boolean enabled)
    {
        Visualization.GetValue()?.DrawDebugOutlines = enabled;
    }

    private void UpdateSize()
    {
        Visualization.GetValue()?.SetSize(viewportSize / Scale);
    }

    /// <summary>
    /// Render the canvas.
    /// </summary>
    public void Render()
    {
        Visualization.GetValue()?.Render();
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
