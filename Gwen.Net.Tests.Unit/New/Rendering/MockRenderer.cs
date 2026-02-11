using System.Drawing;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Rendering;

namespace Gwen.Net.Tests.Unit.New.Rendering;

public class MockRenderer : Renderer
{
    public RectangleF LastDrawnRectangle { get; private set; }
    public Brush LastUsedBrush { get; private set; } = null!;
    
    public override void DrawFilledRectangle(RectangleF rectangle, Brush brush)
    {
        LastDrawnRectangle = Transform(rectangle);
        LastUsedBrush = brush;
    }
    
    public RectangleF ClipRectangle(RectangleF rectangle)
    {
        (Single x, Single y) uv0 = (0, 0);
        (Single x, Single y) uv1 = (1, 1);
        
        base.ClipRectangle(ref rectangle, ref uv0, ref uv1);
        
        return rectangle;
    }
}
