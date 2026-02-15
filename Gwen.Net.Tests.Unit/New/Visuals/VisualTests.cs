using System.Drawing;
using Gwen.Net.New;
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

        Assert.Equal(expected: 180f, visual.Bounds.Width);
        Assert.Equal(expected: 170f, visual.Bounds.Height);
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
        Assert.Equal(expected: 180f, child.Bounds.Width);
        Assert.Equal(expected: 170f, child.Bounds.Height);
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
        Assert.Equal(expected: 174f, child.Bounds.Width);
        Assert.Equal(expected: 156f, child.Bounds.Height);
    }

    #endregion LAYOUTING

    #region ALIGNMENT

    [Fact]
    public void Arrange_HorizontalStretch_FillsAvailableWidth()
    {
        var visual = new MockVisual {HorizontalAlignment = {Value = HorizontalAlignment.Stretch}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 0f, actual: visual.Bounds.X);
        Assert.Equal(expected: 200f, actual: visual.Bounds.Width);
    }

    [Fact]
    public void Arrange_HorizontalLeft_AlignedToLeftEdge()
    {
        var visual = new MockVisual {HorizontalAlignment = {Value = HorizontalAlignment.Left}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 0f, actual: visual.Bounds.X);
        Assert.Equal(expected: 1f, actual: visual.Bounds.Width);
    }

    [Fact]
    public void Arrange_HorizontalCenter_CenteredInAvailableSpace()
    {
        var visual = new MockVisual {HorizontalAlignment = {Value = HorizontalAlignment.Center}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 99.5f, actual: visual.Bounds.X);
        Assert.Equal(expected: 1f, actual: visual.Bounds.Width);
    }

    [Fact]
    public void Arrange_HorizontalRight_AlignedToRightEdge()
    {
        var visual = new MockVisual {HorizontalAlignment = {Value = HorizontalAlignment.Right}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 199f, actual: visual.Bounds.X);
        Assert.Equal(expected: 1f, actual: visual.Bounds.Width);
    }

    [Fact]
    public void Arrange_HorizontalCenter_WithMargin_CenteredWithinMarginArea()
    {
        var visual = new MockVisual
        {
            HorizontalAlignment = {Value = HorizontalAlignment.Center},
            Margin = {Value = new ThicknessF(left: 10f, top: 0f, right: 20f, bottom: 0f)}
        };

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));
        
        Assert.Equal(expected: 94.5f, actual: visual.Bounds.X);
        Assert.Equal(expected: 1f, actual: visual.Bounds.Width);
    }

    [Fact]
    public void Arrange_HorizontalRight_WithMargin_AlignedToRightWithinMarginArea()
    {
        var visual = new MockVisual
        {
            HorizontalAlignment = {Value = HorizontalAlignment.Right},
            Margin = {Value = new ThicknessF(left: 10f, top: 0f, right: 20f, bottom: 0f)}
        };

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));
        
        Assert.Equal(expected: 179f, actual: visual.Bounds.X);
        Assert.Equal(expected: 1f, actual: visual.Bounds.Width);
    }

    [Fact]
    public void Arrange_HorizontalStretch_RespectsMaximumWidth()
    {
        var visual = new MockVisual
        {
            HorizontalAlignment = {Value = HorizontalAlignment.Stretch},
            MaximumWidth = {Value = 80f}
        };

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 80f, actual: visual.Bounds.Width);
    }

    [Fact]
    public void Arrange_HorizontalLeft_WithLargerMinimumSize_UsesMinimumSize()
    {
        var visual = new MockVisual
        {
            HorizontalAlignment = {Value = HorizontalAlignment.Left},
            MinimumWidth = {Value = 50f}
        };

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 0f, actual: visual.Bounds.X);
        Assert.Equal(expected: 50f, actual: visual.Bounds.Width);
    }

    [Fact]
    public void Arrange_VerticalStretch_FillsAvailableHeight()
    {
        var visual = new MockVisual {VerticalAlignment = {Value = VerticalAlignment.Stretch}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 0f, actual: visual.Bounds.Y);
        Assert.Equal(expected: 200f, actual: visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_VerticalTop_AlignedToTopEdge()
    {
        var visual = new MockVisual {VerticalAlignment = {Value = VerticalAlignment.Top}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 0f, actual: visual.Bounds.Y);
        Assert.Equal(expected: 1f, actual: visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_VerticalCenter_CenteredInAvailableSpace()
    {
        var visual = new MockVisual {VerticalAlignment = {Value = VerticalAlignment.Center}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 99.5f, actual: visual.Bounds.Y);
        Assert.Equal(expected: 1f, actual: visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_VerticalBottom_AlignedToBottomEdge()
    {
        var visual = new MockVisual {VerticalAlignment = {Value = VerticalAlignment.Bottom}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 199f, actual: visual.Bounds.Y);
        Assert.Equal(expected: 1f, actual: visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_VerticalCenter_WithMargin_CenteredWithinMarginArea()
    {
        var visual = new MockVisual
        {
            VerticalAlignment = {Value = VerticalAlignment.Center},
            Margin = {Value = new ThicknessF(left: 0f, top: 10f, right: 0f, bottom: 20f)}
        };

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));
        
        Assert.Equal(expected: 94.5f, actual: visual.Bounds.Y);
        Assert.Equal(expected: 1f, actual: visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_VerticalBottom_WithMargin_AlignedToBottomWithinMarginArea()
    {
        var visual = new MockVisual
        {
            VerticalAlignment = {Value = VerticalAlignment.Bottom},
            Margin = {Value = new ThicknessF(left: 0f, top: 10f, right: 0f, bottom: 20f)}
        };

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));
        
        Assert.Equal(expected: 179f, actual: visual.Bounds.Y);
        Assert.Equal(expected: 1f, actual: visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_VerticalStretch_RespectsMaximumHeight()
    {
        var visual = new MockVisual
        {
            VerticalAlignment = {Value = VerticalAlignment.Stretch},
            MaximumHeight = {Value = 80f}
        };

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 80f, actual: visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_VerticalTop_WithLargerMinimumSize_UsesMinimumSize()
    {
        var visual = new MockVisual
        {
            VerticalAlignment = {Value = VerticalAlignment.Top},
            MinimumHeight = {Value = 50f}
        };

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 0f, actual: visual.Bounds.Y);
        Assert.Equal(expected: 50f, actual: visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_CenterCenter_CenteredBothAxes()
    {
        var visual = new MockVisual
        {
            HorizontalAlignment = {Value = HorizontalAlignment.Center},
            VerticalAlignment = {Value = VerticalAlignment.Center},
            MinimumWidth = {Value = 40f},
            MinimumHeight = {Value = 20f}
        };

        visual.Measure(new SizeF(width: 200f, height: 100f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 100f));

        Assert.Equal(expected: 80f, actual: visual.Bounds.X);
        Assert.Equal(expected: 40f, actual: visual.Bounds.Y);
        Assert.Equal(expected: 40f, actual: visual.Bounds.Width);
        Assert.Equal(expected: 20f, actual: visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_RightBottom_PositionedAtBottomRight()
    {
        var visual = new MockVisual
        {
            HorizontalAlignment = {Value = HorizontalAlignment.Right},
            VerticalAlignment = {Value = VerticalAlignment.Bottom},
            MinimumWidth = {Value = 40f},
            MinimumHeight = {Value = 20f}
        };

        visual.Measure(new SizeF(width: 200f, height: 100f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 100f));

        Assert.Equal(expected: 160f, actual: visual.Bounds.X);
        Assert.Equal(expected: 80f, actual: visual.Bounds.Y);
        Assert.Equal(expected: 40f, actual: visual.Bounds.Width);
        Assert.Equal(expected: 20f, actual: visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_LeftTop_PositionedAtTopLeft()
    {
        var visual = new MockVisual
        {
            HorizontalAlignment = {Value = HorizontalAlignment.Left},
            VerticalAlignment = {Value = VerticalAlignment.Top},
            MinimumWidth = {Value = 40f},
            MinimumHeight = {Value = 20f}
        };

        visual.Measure(new SizeF(width: 200f, height: 100f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 100f));

        Assert.Equal(expected: 0f, actual: visual.Bounds.X);
        Assert.Equal(expected: 0f, actual: visual.Bounds.Y);
        Assert.Equal(expected: 40f, actual: visual.Bounds.Width);
        Assert.Equal(expected: 20f, actual: visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_CenterCenter_WithMargin_CenteredWithinMarginArea()
    {
        var visual = new MockVisual
        {
            HorizontalAlignment = {Value = HorizontalAlignment.Center},
            VerticalAlignment = {Value = VerticalAlignment.Center},
            MinimumWidth = {Value = 40f},
            MinimumHeight = {Value = 20f},
            Margin = {Value = new ThicknessF(left: 10f, top: 5f, right: 20f, bottom: 15f)}
        };

        visual.Measure(new SizeF(width: 200f, height: 100f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 100f));
        
        Assert.Equal(expected: 75f, actual: visual.Bounds.X);
        Assert.Equal(expected: 35f, actual: visual.Bounds.Y);
        Assert.Equal(expected: 40f, actual: visual.Bounds.Width);
        Assert.Equal(expected: 20f, actual: visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_ChildCenterCenter_ChildCenteredWithinParentContentArea()
    {
        var parent = new MockVisual {Padding = {Value = new ThicknessF(10f)}};
        var child = new MockVisual
        {
            HorizontalAlignment = {Value = HorizontalAlignment.Center},
            VerticalAlignment = {Value = VerticalAlignment.Center},
            MinimumWidth = {Value = 40f},
            MinimumHeight = {Value = 20f}
        };

        parent.SetChildVisual(child);

        parent.Measure(new SizeF(width: 200f, height: 100f));
        parent.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 100f));
        
        Assert.Equal(expected: 80f, actual: child.Bounds.X);
        Assert.Equal(expected: 40f, actual: child.Bounds.Y);
        Assert.Equal(expected: 40f, actual: child.Bounds.Width);
        Assert.Equal(expected: 20f, actual: child.Bounds.Height);
    }

    #endregion ALIGNMENT
}
