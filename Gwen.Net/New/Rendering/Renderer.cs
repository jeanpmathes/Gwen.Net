using System.Drawing;
using Gwen.Net.New.Graphics;

namespace Gwen.Net.New.Rendering;

/// <summary>
/// Abstract base class that helps implement renderers.
/// </summary>
public abstract class Renderer : IRenderer
{
    /// <inheritdoc/>
    public abstract void Begin();
    
    /// <inheritdoc/>
    public abstract void End();
    
    /// <inheritdoc/>
    public abstract void PushOffset(PointF offset);
    
    /// <inheritdoc/>
    public abstract void PopOffset();
    
    /// <inheritdoc/>
    public abstract void PushClip(RectangleF rectangle);
    
    /// <inheritdoc/>
    public abstract void PopClip();
    
    /// <inheritdoc/>
    public abstract void BeginClip();
    
    /// <inheritdoc/>
    public abstract void EndClip();
    
    /// <inheritdoc/>
    public abstract bool IsClipEmpty();
    
    /// <inheritdoc/>
    public abstract void DrawFilledRectangle(RectangleF rectangle, Brush brush);
    
    /// <inheritdoc/>
    public abstract void Resize(Size size);
    
    /// <inheritdoc/>
    public abstract void Scale(float newScale);
}