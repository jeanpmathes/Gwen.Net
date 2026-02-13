using System.Drawing;

namespace Gwen.Net.Tests.Unit.New.Visuals;

public class VisualTests
{
    [Fact]
    public void Arrange_WhenFinalRectangleChanges_UpdatesBounds()
    {
        var visual = new MockVisual();

        visual.Measure(new SizeF(width: 100f, height: 100f));

        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 100f, height: 100f));
        visual.Arrange(new RectangleF(x: 20f, y: 30f, width: 40f, height: 50f));

        Assert.Equal(expected: 20f, actual: visual.Bounds.X);
        Assert.Equal(expected: 30f, actual: visual.Bounds.Y);
    }
}
