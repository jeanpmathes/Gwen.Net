using System.Drawing;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Rendering;
using Gwen.Net.New.Texts;
using Gwen.Net.New.Utilities;

namespace Gwen.Net.Tests.Unit.New.Rendering;

public class MockRenderer : Renderer
{
    public override void Begin()
    {
        
    }
    
    public override void End()
    {
        
    }

    public override void PushOffset(PointF offset)
    {
        
    }
    
    public override void PopOffset()
    {
        
    }
    
    public override void PushClip(RectangleF rectangle)
    {
        
    }
    
    public override void PopClip()
    {
        
    }
    
    public override void BeginClip()
    {
        
    }
    
    public override void EndClip()
    {
        
    }
    
    public override Boolean IsClipEmpty()
    {
        return false;
    }

    public override IFormattedText CreateFormattedText(String text, Font font)
    {
        return new MockFormattedText();
    }

    public override void DrawFilledRectangle(RectangleF rectangle, Brush brush)
    {
        
    }

    public override void DrawLinedRectangle(RectangleF rectangle, ThicknessF thickness, Brush brush)
    {
        
    }

    public override void Resize(Size size)
    {
    }
}
