using System.Drawing;
using Gwen.Net.New.Graphics;

namespace Gwen.Net.Tests.Unit.New.Rendering;

public class RendererTests 
{
    [Fact]
    public void PushOffset_WithMultipleOffsets_CalculatesCumulativeOffset()
    {
        var renderer = new MockRenderer();
        
        renderer.PushOffset(new PointF(x: 10, y: 20));
        renderer.PushOffset(new PointF(x: 5, y: 5));
        
        renderer.DrawFilledRectangle(new RectangleF(x: 0, y: 0, width: 100, height: 50), Brushes.Red);
        
        Assert.Equal(new RectangleF(x: 15, y: 25, width: 100, height: 50), renderer.LastDrawnRectangle);
    }
    
    [Fact]
    public void PopOffset_WithMultipleOffsets_RestoresPreviousOffsets()
    {
        var renderer = new MockRenderer();
        
        renderer.PushOffset(new PointF(x: 10, y: 20));
        renderer.PushOffset(new PointF(x: 5, y: 5));
        
        renderer.PopOffset();
        renderer.DrawFilledRectangle(new RectangleF(x: 0, y: 0, width: 100, height: 50), Brushes.Red);
        
        Assert.Equal(new RectangleF(x: 10, y: 20, width: 100, height: 50), renderer.LastDrawnRectangle);
        
        renderer.PopOffset();
        renderer.DrawFilledRectangle(new RectangleF(x: 0, y: 0, width: 100, height: 50), Brushes.Red);
        
        Assert.Equal(new RectangleF(x: 0, y: 0, width: 100, height: 50), renderer.LastDrawnRectangle);
    }
    
    [Fact]
    public void ClipRectangle_DoesNotClipContainedRectangles()
    {
        var renderer = new MockRenderer();
        renderer.BeginClip();
        
        renderer.PushClip(new RectangleF(x: 0, y: 0, width: 100, height: 100));
        
        Assert.Equal(new RectangleF(x: 10, y: 10, width: 50, height: 50), renderer.ClipRectangle(new RectangleF(x: 10, y: 10, width: 50, height: 50)));
    }
    
    [Fact]
    public void ClipRectangle_DoesNotClipWithoutActiveClip()
    {
        var renderer = new MockRenderer();
        
        Assert.Equal(new RectangleF(x: 10, y: 10, width: 50, height: 50), renderer.ClipRectangle(new RectangleF(x: 10, y: 10, width: 50, height: 50)));
    }

    [Fact]
    public void ClipRectangle_ClipsPartiallyOverlappingRectangles()
    {
        var renderer = new MockRenderer();
        renderer.BeginClip();

        renderer.PushClip(new RectangleF(x: 0, y: 0, width: 100, height: 100));

        Assert.Equal(new RectangleF(x: 50, y: 50, width: 50, height: 50), renderer.ClipRectangle(new RectangleF(x: 50, y: 50, width: 100, height: 100)));
        Assert.Equal(new RectangleF(x: 0, y: 0, width: 50, height: 50), renderer.ClipRectangle(new RectangleF(x: -50, y: -50, width: 100, height: 100)));
    }

    [Fact]
    public void ClipRectangle_ClipsNonOverlappingRectanglesToEmpty()
    {
        var renderer = new MockRenderer();
        renderer.BeginClip();
        
        renderer.PushClip(new RectangleF(x: 10, y: 10, width: 100, height: 100));
        
        Assert.True(renderer.ClipRectangle(new RectangleF(x: 0, y: 0, width: 5, height: 5)).IsEmpty);
    }
    
    [Fact]
    public void ClipRectangle_WithNestedClips_CalculatesCumulativeClip()
    {
        var renderer = new MockRenderer();
        renderer.BeginClip();
        
        renderer.PushClip(new RectangleF(x: 0, y: 0, width: 100, height: 100));
        renderer.PushClip(new RectangleF(x: 10, y: 10, width: 50, height: 50));
        
        Assert.Equal(new RectangleF(x: 10, y: 10, width: 50, height: 50), renderer.ClipRectangle(new RectangleF(x: 0, y: 0, width: 100, height: 100)));
    }

    [Fact]
    public void ClipRectangle_IsAffectedByCurrentOffset()
    {
        var renderer = new MockRenderer();
        renderer.BeginClip();
        
        renderer.PushOffset(new PointF(x: 10, y: 20));
        renderer.PushClip(new RectangleF(x: 0, y: 0, width: 100, height: 100));
        
        Assert.Equal(new RectangleF(x: 10, y: 20, width: 90, height: 80), renderer.ClipRectangle(new RectangleF(x: 0, y: 0, width: 100, height: 100)));
    }
    
    [Fact]
    public void PopClip_WithNestedClips_RestoresPreviousClips()
    {
        var renderer = new MockRenderer();
        renderer.BeginClip();
        
        renderer.PushClip(new RectangleF(x: 0, y: 0, width: 100, height: 100));
        renderer.PushClip(new RectangleF(x: 10, y: 10, width: 50, height: 50));
        
        renderer.PopClip();
        Assert.Equal(new RectangleF(x: 0, y: 0, width: 100, height: 100), renderer.ClipRectangle(new RectangleF(x: 0, y: 0, width: 100, height: 100)));
        
        renderer.PopClip();
        Assert.Equal(new RectangleF(x: 0, y: 0, width: 100, height: 100), renderer.ClipRectangle(new RectangleF(x: 0, y: 0, width: 100, height: 100)));
    }
    
    [Fact]
    public void SetScale_AffectsDrawnRectangles()
    {
        var renderer = new MockRenderer();
        
        renderer.Scale(2.0f);
        renderer.DrawFilledRectangle(new RectangleF(x: 10, y: 20, width: 30, height: 40), Brushes.Red);
        
        Assert.Equal(new RectangleF(x: 20, y: 40, width: 60, height: 80), renderer.LastDrawnRectangle);
    }
}
