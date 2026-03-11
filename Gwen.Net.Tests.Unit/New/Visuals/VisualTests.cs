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
        MockVisual visual = new();

        visual.Measure(new SizeF(width: 100f, height: 100f));

        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 100f, height: 100f));
        visual.Arrange(new RectangleF(x: 20f, y: 30f, width: 40f, height: 50f));

        Assert.Equal(expected: 20f, visual.Bounds.X);
        Assert.Equal(expected: 30f, visual.Bounds.Y);
    }

    [Fact]
    public void Measure_ClampsToMinimumSize()
    {
        MockVisual visual = new()
            {MinimumWidth = {Value = 50f}, MinimumHeight = {Value = 30f}};

        visual.Measure(new SizeF(width: 200f, height: 200f));

        Assert.Equal(expected: 50f, visual.MeasuredSize.Width);
        Assert.Equal(expected: 30f, visual.MeasuredSize.Height);
    }

    [Fact]
    public void Measure_ClampsToMaximumSize()
    {
        MockVisual parent = new()
            {MaximumWidth = {Value = 50f}, MaximumHeight = {Value = 40f}};

        MockVisual child = new()
            {MinimumWidth = {Value = 100f}, MinimumHeight = {Value = 100f}};

        parent.SetChildVisual(child);

        parent.Measure(new SizeF(width: 200f, height: 200f));

        Assert.Equal(expected: 50f, parent.MeasuredSize.Width);
        Assert.Equal(expected: 40f, parent.MeasuredSize.Height);
    }

    [Fact]
    public void Arrange_BoundsRespectMaximumSize()
    {
        MockVisual parent = new()
            {MaximumWidth = {Value = 50f}, MaximumHeight = {Value = 40f}};

        MockVisual child = new()
            {MinimumWidth = {Value = 100f}, MinimumHeight = {Value = 100f}};

        parent.SetChildVisual(child);

        parent.Measure(new SizeF(width: 200f, height: 200f));
        parent.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 50f, parent.Bounds.Width);
        Assert.Equal(expected: 40f, parent.Bounds.Height);
    }

    [Fact]
    public void Measure_WithMargin_MeasuredSizeIncludesMargin()
    {
        MockVisual visual = new()
            {Margin = {Value = new ThicknessF(left: 5f, top: 10f, right: 15f, bottom: 20f)}};

        visual.Measure(new SizeF(width: 200f, height: 200f));

        Assert.Equal(expected: 21f, visual.MeasuredSize.Width);
        Assert.Equal(expected: 31f, visual.MeasuredSize.Height);
    }

    [Fact]
    public void Arrange_WithMargin_BoundsPositionIsOffsetByMargin()
    {
        MockVisual visual = new()
            {Margin = {Value = new ThicknessF(left: 5f, top: 10f, right: 15f, bottom: 20f)}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 5f, visual.Bounds.X);
        Assert.Equal(expected: 10f, visual.Bounds.Y);
    }

    [Fact]
    public void Arrange_WithMargin_BoundsSizeDoesNotIncludeMargin()
    {
        MockVisual visual = new()
            {Margin = {Value = new ThicknessF(left: 5f, top: 10f, right: 15f, bottom: 20f)}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 180f, visual.Bounds.Width);
        Assert.Equal(expected: 170f, visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_ChildWithMargin_ChildBoundsAreOffsetByItsMargin()
    {
        MockVisual parent = new();

        MockVisual child = new()
            {Margin = {Value = new ThicknessF(left: 5f, top: 10f, right: 15f, bottom: 20f)}};

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
        MockVisual parent = new()
            {Padding = {Value = new ThicknessF(left: 5f, top: 10f, right: 15f, bottom: 20f)}};

        MockVisual child = new();
        parent.SetChildVisual(child);

        parent.Measure(new SizeF(width: 200f, height: 200f));

        Assert.Equal(expected: 21f, parent.MeasuredSize.Width);
        Assert.Equal(expected: 31f, parent.MeasuredSize.Height);
    }

    [Fact]
    public void Arrange_WithPadding_ChildIsOffsetByPadding()
    {
        MockVisual parent = new()
            {Padding = {Value = new ThicknessF(left: 5f, top: 10f, right: 15f, bottom: 20f)}};

        MockVisual child = new();

        parent.SetChildVisual(child);

        parent.Measure(new SizeF(width: 200f, height: 200f));
        parent.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 5f, child.Bounds.X);
        Assert.Equal(expected: 10f, child.Bounds.Y);
    }

    [Fact]
    public void Arrange_ParentPaddingAndChildMargin_ChildIsOffsetByBoth()
    {
        MockVisual parent = new()
            {Padding = {Value = new ThicknessF(left: 5f, top: 10f, right: 15f, bottom: 20f)}};

        MockVisual child = new()
            {Margin = {Value = new ThicknessF(left: 3f, top: 7f, right: 3f, bottom: 7f)}};

        parent.SetChildVisual(child);

        parent.Measure(new SizeF(width: 200f, height: 200f));
        parent.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 8f, child.Bounds.X);
        Assert.Equal(expected: 17f, child.Bounds.Y);
        Assert.Equal(expected: 174f, child.Bounds.Width);
        Assert.Equal(expected: 156f, child.Bounds.Height);
    }

    [Fact]
    public void Measure_OfHiddenVisual_ReturnsNormalSize()
    {
        MockVisual visual = new()
            {Visibility = {Value = Visibility.Hidden}};

        visual.Measure(new SizeF(width: 200f, height: 200f));

        Assert.Equal(expected: 1f, visual.MeasuredSize.Width);
        Assert.Equal(expected: 1f, visual.MeasuredSize.Height);
    }

    [Fact]
    public void Measure_OfCollapsedVisual_ReturnsZeroSize()
    {
        MockVisual visual = new()
            {Visibility = {Value = Visibility.Collapsed}};

        visual.Measure(new SizeF(width: 200f, height: 200f));

        Assert.Equal(expected: 0f, visual.MeasuredSize.Width);
        Assert.Equal(expected: 0f, visual.MeasuredSize.Height);
    }

    [Fact]
    public void Arrange_OfCollapsedVisual_SetsZeroSize()
    {
        MockVisual visual = new()
            {Visibility = {Value = Visibility.Collapsed}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 0f, visual.Bounds.Width);
        Assert.Equal(expected: 0f, visual.Bounds.Height);
    }

    #endregion LAYOUTING

    #region ALIGNMENT

    [Fact]
    public void Arrange_HorizontalStretch_FillsAvailableWidth()
    {
        MockVisual visual = new()
            {HorizontalAlignment = {Value = HorizontalAlignment.Stretch}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 0f, visual.Bounds.X);
        Assert.Equal(expected: 200f, visual.Bounds.Width);
    }

    [Fact]
    public void Arrange_HorizontalLeft_AlignedToLeftEdge()
    {
        MockVisual visual = new()
            {HorizontalAlignment = {Value = HorizontalAlignment.Left}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 0f, visual.Bounds.X);
        Assert.Equal(expected: 1f, visual.Bounds.Width);
    }

    [Fact]
    public void Arrange_HorizontalCenter_CenteredInAvailableSpace()
    {
        MockVisual visual = new()
            {HorizontalAlignment = {Value = HorizontalAlignment.Center}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 99.5f, visual.Bounds.X);
        Assert.Equal(expected: 1f, visual.Bounds.Width);
    }

    [Fact]
    public void Arrange_HorizontalRight_AlignedToRightEdge()
    {
        MockVisual visual = new()
            {HorizontalAlignment = {Value = HorizontalAlignment.Right}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 199f, visual.Bounds.X);
        Assert.Equal(expected: 1f, visual.Bounds.Width);
    }

    [Fact]
    public void Arrange_HorizontalCenter_WithMargin_CenteredWithinMarginArea()
    {
        MockVisual visual = new()
        {
            HorizontalAlignment = {Value = HorizontalAlignment.Center},
            Margin = {Value = new ThicknessF(left: 10f, top: 0f, right: 20f, bottom: 0f)}
        };

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 94.5f, visual.Bounds.X);
        Assert.Equal(expected: 1f, visual.Bounds.Width);
    }

    [Fact]
    public void Arrange_HorizontalRight_WithMargin_AlignedToRightWithinMarginArea()
    {
        MockVisual visual = new()
        {
            HorizontalAlignment = {Value = HorizontalAlignment.Right},
            Margin = {Value = new ThicknessF(left: 10f, top: 0f, right: 20f, bottom: 0f)}
        };

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 179f, visual.Bounds.X);
        Assert.Equal(expected: 1f, visual.Bounds.Width);
    }

    [Fact]
    public void Arrange_HorizontalStretch_RespectsMaximumWidth()
    {
        MockVisual visual = new()
        {
            HorizontalAlignment = {Value = HorizontalAlignment.Stretch},
            MaximumWidth = {Value = 80f}
        };

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 80f, visual.Bounds.Width);
    }

    [Fact]
    public void Arrange_HorizontalLeft_WithLargerMinimumSize_UsesMinimumSize()
    {
        MockVisual visual = new()
        {
            HorizontalAlignment = {Value = HorizontalAlignment.Left},
            MinimumWidth = {Value = 50f}
        };

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 0f, visual.Bounds.X);
        Assert.Equal(expected: 50f, visual.Bounds.Width);
    }

    [Fact]
    public void Arrange_VerticalStretch_FillsAvailableHeight()
    {
        MockVisual visual = new()
            {VerticalAlignment = {Value = VerticalAlignment.Stretch}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 0f, visual.Bounds.Y);
        Assert.Equal(expected: 200f, visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_VerticalTop_AlignedToTopEdge()
    {
        MockVisual visual = new()
            {VerticalAlignment = {Value = VerticalAlignment.Top}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 0f, visual.Bounds.Y);
        Assert.Equal(expected: 1f, visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_VerticalCenter_CenteredInAvailableSpace()
    {
        MockVisual visual = new()
            {VerticalAlignment = {Value = VerticalAlignment.Center}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 99.5f, visual.Bounds.Y);
        Assert.Equal(expected: 1f, visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_VerticalBottom_AlignedToBottomEdge()
    {
        MockVisual visual = new()
            {VerticalAlignment = {Value = VerticalAlignment.Bottom}};

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 199f, visual.Bounds.Y);
        Assert.Equal(expected: 1f, visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_VerticalCenter_WithMargin_CenteredWithinMarginArea()
    {
        MockVisual visual = new()
        {
            VerticalAlignment = {Value = VerticalAlignment.Center},
            Margin = {Value = new ThicknessF(left: 0f, top: 10f, right: 0f, bottom: 20f)}
        };

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 94.5f, visual.Bounds.Y);
        Assert.Equal(expected: 1f, visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_VerticalBottom_WithMargin_AlignedToBottomWithinMarginArea()
    {
        MockVisual visual = new()
        {
            VerticalAlignment = {Value = VerticalAlignment.Bottom},
            Margin = {Value = new ThicknessF(left: 0f, top: 10f, right: 0f, bottom: 20f)}
        };

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 179f, visual.Bounds.Y);
        Assert.Equal(expected: 1f, visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_VerticalStretch_RespectsMaximumHeight()
    {
        MockVisual visual = new()
        {
            VerticalAlignment = {Value = VerticalAlignment.Stretch},
            MaximumHeight = {Value = 80f}
        };

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 80f, visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_VerticalTop_WithLargerMinimumSize_UsesMinimumSize()
    {
        MockVisual visual = new()
        {
            VerticalAlignment = {Value = VerticalAlignment.Top},
            MinimumHeight = {Value = 50f}
        };

        visual.Measure(new SizeF(width: 200f, height: 200f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 0f, visual.Bounds.Y);
        Assert.Equal(expected: 50f, visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_CenterCenter_CenteredBothAxes()
    {
        MockVisual visual = new()
        {
            HorizontalAlignment = {Value = HorizontalAlignment.Center},
            VerticalAlignment = {Value = VerticalAlignment.Center},
            MinimumWidth = {Value = 40f},
            MinimumHeight = {Value = 20f}
        };

        visual.Measure(new SizeF(width: 200f, height: 100f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 100f));

        Assert.Equal(expected: 80f, visual.Bounds.X);
        Assert.Equal(expected: 40f, visual.Bounds.Y);
        Assert.Equal(expected: 40f, visual.Bounds.Width);
        Assert.Equal(expected: 20f, visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_RightBottom_PositionedAtBottomRight()
    {
        MockVisual visual = new()
        {
            HorizontalAlignment = {Value = HorizontalAlignment.Right},
            VerticalAlignment = {Value = VerticalAlignment.Bottom},
            MinimumWidth = {Value = 40f},
            MinimumHeight = {Value = 20f}
        };

        visual.Measure(new SizeF(width: 200f, height: 100f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 100f));

        Assert.Equal(expected: 160f, visual.Bounds.X);
        Assert.Equal(expected: 80f, visual.Bounds.Y);
        Assert.Equal(expected: 40f, visual.Bounds.Width);
        Assert.Equal(expected: 20f, visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_LeftTop_PositionedAtTopLeft()
    {
        MockVisual visual = new()
        {
            HorizontalAlignment = {Value = HorizontalAlignment.Left},
            VerticalAlignment = {Value = VerticalAlignment.Top},
            MinimumWidth = {Value = 40f},
            MinimumHeight = {Value = 20f}
        };

        visual.Measure(new SizeF(width: 200f, height: 100f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 100f));

        Assert.Equal(expected: 0f, visual.Bounds.X);
        Assert.Equal(expected: 0f, visual.Bounds.Y);
        Assert.Equal(expected: 40f, visual.Bounds.Width);
        Assert.Equal(expected: 20f, visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_CenterCenter_WithMargin_CenteredWithinMarginArea()
    {
        MockVisual visual = new()
        {
            HorizontalAlignment = {Value = HorizontalAlignment.Center},
            VerticalAlignment = {Value = VerticalAlignment.Center},
            MinimumWidth = {Value = 40f},
            MinimumHeight = {Value = 20f},
            Margin = {Value = new ThicknessF(left: 10f, top: 5f, right: 20f, bottom: 15f)}
        };

        visual.Measure(new SizeF(width: 200f, height: 100f));
        visual.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 100f));

        Assert.Equal(expected: 75f, visual.Bounds.X);
        Assert.Equal(expected: 35f, visual.Bounds.Y);
        Assert.Equal(expected: 40f, visual.Bounds.Width);
        Assert.Equal(expected: 20f, visual.Bounds.Height);
    }

    [Fact]
    public void Arrange_ChildCenterCenter_ChildCenteredWithinParentContentArea()
    {
        MockVisual parent = new()
            {Padding = {Value = new ThicknessF(10f)}};

        MockVisual child = new()
        {
            HorizontalAlignment = {Value = HorizontalAlignment.Center},
            VerticalAlignment = {Value = VerticalAlignment.Center},
            MinimumWidth = {Value = 40f},
            MinimumHeight = {Value = 20f}
        };

        parent.SetChildVisual(child);

        parent.Measure(new SizeF(width: 200f, height: 100f));
        parent.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 100f));

        Assert.Equal(expected: 80f, child.Bounds.X);
        Assert.Equal(expected: 40f, child.Bounds.Y);
        Assert.Equal(expected: 40f, child.Bounds.Width);
        Assert.Equal(expected: 20f, child.Bounds.Height);
    }

    #endregion ALIGNMENT

    #region COORDINATE TRANSFORMS

    [Fact]
    public void LocalPointToRoot_NestedChild_AccumulatesParentOffsets()
    {
        MockVisual parent = new();
        MockVisual child = new();

        parent.SetChildVisual(child);

        parent.Measure(new SizeF(width: 200f, height: 200f));
        parent.Arrange(new RectangleF(x: 10f, y: 20f, width: 200f, height: 200f));

        PointF rootPoint = child.LocalPointToRoot(new PointF(x: 5f, y: 5f));

        Assert.Equal(expected: 15f, rootPoint.X);
        Assert.Equal(expected: 25f, rootPoint.Y);
    }

    [Fact]
    public void RootPointToLocal_NestedChild_SubtractsAccumulatedOffsets()
    {
        MockVisual parent = new();
        MockVisual child = new();

        parent.SetChildVisual(child);

        parent.Measure(new SizeF(width: 200f, height: 200f));
        parent.Arrange(new RectangleF(x: 10f, y: 20f, width: 200f, height: 200f));

        PointF localPoint = child.RootPointToLocal(new PointF(x: 15f, y: 25f));

        Assert.Equal(expected: 5f, localPoint.X);
        Assert.Equal(expected: 5f, localPoint.Y);
    }

    #endregion COORDINATE TRANSFORMS

    #region HIERARCHY

    [Fact]
    public void GetChildAfter_WithNonChild_ReturnsNull()
    {
        MockVisual parent = new();
        MockVisual other = new();

        Assert.Null(parent.GetChildAfter(other));
    }

    [Fact]
    public void GetChildAfter_WithOnlyChild_ReturnsNull()
    {
        MockVisual parent = new();
        MockVisual child = new();
        parent.AddChildVisual(child);

        Assert.Null(parent.GetChildAfter(child));
    }

    [Fact]
    public void GetChildAfter_WithFirstOfTwo_ReturnsSecond()
    {
        MockVisual parent = new();
        MockVisual first = new();
        MockVisual second = new();
        parent.AddChildVisual(first);
        parent.AddChildVisual(second);

        Assert.Same(second, parent.GetChildAfter(first));
    }

    [Fact]
    public void GetChildBefore_WithNonChild_ReturnsNull()
    {
        MockVisual parent = new();
        MockVisual other = new();

        Assert.Null(parent.GetChildBefore(other));
    }

    [Fact]
    public void GetChildBefore_WithOnlyChild_ReturnsNull()
    {
        MockVisual parent = new();
        MockVisual child = new();
        parent.AddChildVisual(child);

        Assert.Null(parent.GetChildBefore(child));
    }

    [Fact]
    public void GetChildBefore_WithSecondOfTwo_ReturnsFirst()
    {
        MockVisual parent = new();
        MockVisual first = new();
        MockVisual second = new();
        parent.AddChildVisual(first);
        parent.AddChildVisual(second);

        Assert.Same(first, parent.GetChildBefore(second));
    }

    #endregion HIERARCHY
}
