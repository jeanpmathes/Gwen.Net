using System.Drawing;
using Gwen.Net.New.Utilities;

namespace Gwen.Net.Tests.Unit.New.Visuals;

public class VisualTests
{
    #region LAYOUTING

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

    [Fact]
    public void Measure_ClampsToMinimumSize()
    {
        var visual = new MockVisual {MinimumWidth = {Value = 50f}, MinimumHeight = {Value = 30f}};

        visual.Measure(new SizeF(width: 200f, height: 200f));

        Assert.Equal(expected: 50f, visual.MeasuredSize.Width);
        Assert.Equal(expected: 30f, visual.MeasuredSize.Height);
    }

    [Fact]
    public void Measure_ClampsToMaximumSize()
    {
        var parent = new MockVisual {MaximumWidth = {Value = 50f}, MaximumHeight = {Value = 40f}};
        var child = new MockVisual {MinimumWidth = {Value = 100f}, MinimumHeight = {Value = 100f}};

        parent.SetChildVisual(child);

        parent.Measure(new SizeF(width: 200f, height: 200f));

        Assert.Equal(expected: 50f, parent.MeasuredSize.Width);
        Assert.Equal(expected: 40f, parent.MeasuredSize.Height);
    }

    [Fact]
    public void Arrange_BoundsRespectMaximumSize()
    {
        var parent = new MockVisual {MaximumWidth = {Value = 50f}, MaximumHeight = {Value = 40f}};
        var child = new MockVisual {MinimumWidth = {Value = 100f}, MinimumHeight = {Value = 100f}};

        parent.SetChildVisual(child);

        parent.Measure(new SizeF(width: 200f, height: 200f));
        parent.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 50f, parent.Bounds.Width);
        Assert.Equal(expected: 40f, parent.Bounds.Height);
    }

    [Fact]
    public void Measure_WithMargin_MeasuredSizeIncludesMargin()
    {
        var visual = new MockVisual {Margin = {Value = new ThicknessF(left: 5f, top: 10f, right: 15f, bottom: 20f)}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        
        Assert.Equal(expected: 21f, visual.MeasuredSize.Width);
        Assert.Equal(expected: 31f, visual.MeasuredSize.Height);
    }

    [Fact]
    public void Arrange_WithMargin_BoundsPositionIsOffsetByMargin()
    {
        var visual = new MockVisual {Margin = {Value = new ThicknessF(left: 5f, top: 10f, right: 15f, bottom: 20f)}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 5f, visual.Bounds.X);
        Assert.Equal(expected: 10f, visual.Bounds.Y);
    }

    [Fact]
    public void Arrange_WithMargin_BoundsSizeDoesNotIncludeMargin()
    {
        var visual = new MockVisual {Margin = {Value = new ThicknessF(left: 5f, top: 10f, right: 15f, bottom: 20f)}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 1f, visual.Bounds.Width);
        Assert.Equal(expected: 1f, visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_ChildWithMargin_ChildBoundsAreOffsetByItsMargin()
    {
        var parent = new MockVisual();
        var child = new MockVisual {Margin = {Value = new ThicknessF(left: 5f, top: 10f, right: 15f, bottom: 20f)}};

        parent.SetChildVisual(child);

        parent.Measure(new SizeF(width: 200f, height: 200f));
        parent.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 5f, child.Bounds.X);
        Assert.Equal(expected: 10f, child.Bounds.Y);
        Assert.Equal(expected: 1f, child.Bounds.Width);
        Assert.Equal(expected: 1f, child.Bounds.Height);
    }

    [Fact]
    public void Measure_WithPaddingAndChild_MeasuredSizeIncludesPadding()
    {
        var parent = new MockVisual {Padding = {Value = new ThicknessF(left: 5f, top: 10f, right: 15f, bottom: 20f)}};
        var child = new MockVisual();
        parent.SetChildVisual(child);

        parent.Measure(new SizeF(width: 200f, height: 200f));
        
        Assert.Equal(expected: 21f, parent.MeasuredSize.Width);
        Assert.Equal(expected: 31f, parent.MeasuredSize.Height);
    }

    [Fact]
    public void Arrange_WithPadding_ChildIsOffsetByPadding()
    {
        var parent = new MockVisual {Padding = {Value = new ThicknessF(left: 5f, top: 10f, right: 15f, bottom: 20f)}};
        var child = new MockVisual();
        
        parent.SetChildVisual(child);

        parent.Measure(new SizeF(width: 200f, height: 200f));
        parent.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 5f, child.Bounds.X);
        Assert.Equal(expected: 10f, child.Bounds.Y);
    }

    [Fact]
    public void Arrange_ParentPaddingAndChildMargin_ChildIsOffsetByBoth()
    {
        var parent = new MockVisual {Padding = {Value = new ThicknessF(left: 5f, top: 10f, right: 15f, bottom: 20f)}};
        var child = new MockVisual {Margin = {Value = new ThicknessF(left: 3f, top: 7f, right: 3f, bottom: 7f)}};

        parent.SetChildVisual(child);

        parent.Measure(new SizeF(width: 200f, height: 200f));
        parent.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));
        
        Assert.Equal(expected: 8f, child.Bounds.X);
        Assert.Equal(expected: 17f, child.Bounds.Y);
        Assert.Equal(expected: 1f, child.Bounds.Width);
        Assert.Equal(expected: 1f, child.Bounds.Height);
    }

    #endregion LAYOUTING
}
