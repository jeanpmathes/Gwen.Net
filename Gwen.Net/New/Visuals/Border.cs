using System.Drawing;
using Gwen.Net.New.Rendering;

namespace Gwen.Net.New.Visuals;

/// <summary>
/// A border draws background and border around its child element.
/// </summary>
public class Border : VisualElement
{
    /// <inheritdoc/>
    public override void OnRender(IRenderer renderer)
    {
        renderer.DrawFilledRectangle(RenderBounds, Color.White);
        renderer.DrawLinedRectangle(RenderBounds, Color.Black);
    }
}
