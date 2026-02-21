using System.Drawing;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Texts;

namespace Gwen.Net.Tests.Unit.New.Rendering;

public sealed class MockFormattedText : IFormattedText
{
    public SizeF Measure(SizeF availableSize)
    {
        return new SizeF(width: 42, height: 24);
    }
    
    public void Draw(RectangleF rectangle, Brush brush)
    {
        
    }
    
    public void Dispose()
    {
        
    }
}
