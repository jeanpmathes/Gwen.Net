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
    /// <summary>
    /// Initializes a new instance of the <see cref="Canvas"/> class.
    /// </summary>
    public Canvas()
    {
        SetAsRoot();
    }

    /// <summary>
    /// Gets or sets the content of the canvas.
    /// </summary>
    public Element? Content
    {
        get;

        set
        {
            SetLogicalChild(value);
            field = value;
        }
    }

    /// <inheritdoc/>
    public override void OnBoundsChanged(RectangleF oldBounds, RectangleF newBounds)
    {
        InvalidateMeasure();
    }

    /// <inheritdoc/>
    public override void Render(IRenderer renderer)
    {
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
        if (Content != null) 
            RemoveLogicalChild(Content);
    }
}
