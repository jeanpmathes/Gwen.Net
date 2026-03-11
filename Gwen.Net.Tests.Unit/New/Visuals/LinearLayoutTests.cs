using System.Drawing;
using Gwen.Net.New;
using Gwen.Net.New.Utilities;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.Tests.Unit.New.Visuals;

public class LinearLayoutTests() : VisualTestBase<LinearLayout>(() => new LinearLayout())
{
    private static MockLinearLayout CreateLinearLayout()
    {
        return new MockLinearLayout {Visibility = {Value = Visibility.Visible}};
    }

    [Fact]
    public void Measure_Horizontal_NoChildren_ReturnsMinimumSize()
    {
        MockLinearLayout layout = CreateLinearLayout();

        layout.Measure(new SizeF(width: 200f, height: 200f));

        Assert.Equal(expected: 1f, layout.MeasuredSize.Width);
        Assert.Equal(expected: 1f, layout.MeasuredSize.Height);
    }

    [Fact]
    public void Measure_Horizontal_SingleChild_ReturnsChildSize()
    {
        MockLinearLayout layout = CreateLinearLayout();
        MockVisual child = new() {MinimumWidth = {Value = 50f}, MinimumHeight = {Value = 30f}};
        layout.Add(child);

        layout.Measure(new SizeF(width: 200f, height: 200f));

        Assert.Equal(expected: 50f, layout.MeasuredSize.Width);
        Assert.Equal(expected: 30f, layout.MeasuredSize.Height);
    }

    [Fact]
    public void Measure_Horizontal_MultipleChildren_SumsWidthAndTakesMaxHeight()
    {
        MockLinearLayout layout = CreateLinearLayout();
        layout.Add(new MockVisual {MinimumWidth = {Value = 40f}, MinimumHeight = {Value = 20f}});
        layout.Add(new MockVisual {MinimumWidth = {Value = 60f}, MinimumHeight = {Value = 50f}});
        layout.Add(new MockVisual {MinimumWidth = {Value = 30f}, MinimumHeight = {Value = 10f}});

        layout.Measure(new SizeF(width: 500f, height: 500f));

        Assert.Equal(expected: 130f, layout.MeasuredSize.Width);
        Assert.Equal(expected: 50f, layout.MeasuredSize.Height);
    }

    [Fact]
    public void Measure_Horizontal_WithPadding_IncludesPaddingInResult()
    {
        MockLinearLayout layout = CreateLinearLayout();
        layout.Padding.Value = new ThicknessF(left: 10f, top: 5f, right: 10f, bottom: 5f);

        layout.Add(new MockVisual {MinimumWidth = {Value = 50f}, MinimumHeight = {Value = 30f}});

        layout.Measure(new SizeF(width: 200f, height: 200f));

        Assert.Equal(expected: 70f, layout.MeasuredSize.Width);
        Assert.Equal(expected: 40f, layout.MeasuredSize.Height);
    }

    [Fact]
    public void Measure_Vertical_NoChildren_ReturnsMinimumSize()
    {
        MockLinearLayout layout = CreateLinearLayout();
        layout.Orientation.Value = Orientation.Vertical;

        layout.Measure(new SizeF(width: 200f, height: 200f));

        Assert.Equal(expected: 1f, layout.MeasuredSize.Width);
        Assert.Equal(expected: 1f, layout.MeasuredSize.Height);
    }

    [Fact]
    public void Measure_Vertical_SingleChild_ReturnsChildSize()
    {
        MockLinearLayout layout = CreateLinearLayout();
        layout.Orientation.Value = Orientation.Vertical;

        layout.Add(new MockVisual {MinimumWidth = {Value = 50f}, MinimumHeight = {Value = 30f}});

        layout.Measure(new SizeF(width: 200f, height: 200f));

        Assert.Equal(expected: 50f, layout.MeasuredSize.Width);
        Assert.Equal(expected: 30f, layout.MeasuredSize.Height);
    }

    [Fact]
    public void Measure_Vertical_MultipleChildren_SumsHeightAndTakesMaxWidth()
    {
        MockLinearLayout layout = CreateLinearLayout();
        layout.Orientation.Value = Orientation.Vertical;

        layout.Add(new MockVisual {MinimumWidth = {Value = 40f}, MinimumHeight = {Value = 20f}});
        layout.Add(new MockVisual {MinimumWidth = {Value = 60f}, MinimumHeight = {Value = 50f}});
        layout.Add(new MockVisual {MinimumWidth = {Value = 30f}, MinimumHeight = {Value = 10f}});

        layout.Measure(new SizeF(width: 500f, height: 500f));

        Assert.Equal(expected: 60f, layout.MeasuredSize.Width);
        Assert.Equal(expected: 80f, layout.MeasuredSize.Height);
    }

    [Fact]
    public void Measure_Vertical_WithPadding_IncludesPaddingInResult()
    {
        MockLinearLayout layout = CreateLinearLayout();
        layout.Orientation.Value = Orientation.Vertical;
        layout.Padding.Value = new ThicknessF(left: 10f, top: 5f, right: 10f, bottom: 5f);

        layout.Add(new MockVisual {MinimumWidth = {Value = 50f}, MinimumHeight = {Value = 30f}});

        layout.Measure(new SizeF(width: 200f, height: 200f));

        Assert.Equal(expected: 70f, layout.MeasuredSize.Width);
        Assert.Equal(expected: 40f, layout.MeasuredSize.Height);
    }

    [Fact]
    public void Arrange_Horizontal_ChildrenAreLaidOutLeftToRight()
    {
        MockLinearLayout layout = CreateLinearLayout();

        MockVisual child1 = new() {MinimumWidth = {Value = 40f}, MinimumHeight = {Value = 20f}};
        MockVisual child2 = new() {MinimumWidth = {Value = 60f}, MinimumHeight = {Value = 30f}};

        layout.Add(child1);
        layout.Add(child2);

        layout.Measure(new SizeF(width: 200f, height: 100f));
        layout.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 100f));

        Assert.Equal(expected: 0f, child1.Bounds.X);
        Assert.Equal(expected: 40f, child2.Bounds.X);
    }

    [Fact]
    public void Arrange_Horizontal_ChildrenGetFullHeight()
    {
        MockLinearLayout layout = CreateLinearLayout();

        MockVisual child1 = new() {MinimumWidth = {Value = 40f}, MinimumHeight = {Value = 20f}};
        MockVisual child2 = new() {MinimumWidth = {Value = 60f}, MinimumHeight = {Value = 30f}};

        layout.Add(child1);
        layout.Add(child2);

        layout.Measure(new SizeF(width: 200f, height: 100f));
        layout.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 100f));

        Assert.Equal(expected: 100f, child1.Bounds.Height);
        Assert.Equal(expected: 100f, child2.Bounds.Height);
    }

    [Fact]
    public void Arrange_Horizontal_ChildWidthEqualsDesiredWidth()
    {
        MockLinearLayout layout = CreateLinearLayout();

        MockVisual child1 = new() {MinimumWidth = {Value = 40f}};
        MockVisual child2 = new() {MinimumWidth = {Value = 60f}};

        layout.Add(child1);
        layout.Add(child2);

        layout.Measure(new SizeF(width: 200f, height: 100f));
        layout.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 100f));

        Assert.Equal(expected: 40f, child1.Bounds.Width);
        Assert.Equal(expected: 60f, child2.Bounds.Width);
    }

    [Fact]
    public void Arrange_Horizontal_WithPadding_ChildrenAreOffsetByPadding()
    {
        MockLinearLayout layout = CreateLinearLayout();
        layout.Padding.Value = new ThicknessF(left: 10f, top: 5f, right: 10f, bottom: 5f);

        MockVisual child = new() {MinimumWidth = {Value = 40f}, MinimumHeight = {Value = 20f}};
        layout.Add(child);

        layout.Measure(new SizeF(width: 200f, height: 100f));
        layout.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 100f));

        Assert.Equal(expected: 10f, child.Bounds.X);
        Assert.Equal(expected: 5f, child.Bounds.Y);
    }

    [Fact]
    public void Arrange_Horizontal_ThreeChildren_CorrectPositions()
    {
        MockLinearLayout layout = CreateLinearLayout();

        MockVisual child1 = new() {MinimumWidth = {Value = 20f}};
        MockVisual child2 = new() {MinimumWidth = {Value = 30f}};
        MockVisual child3 = new() {MinimumWidth = {Value = 50f}};

        layout.Add(child1);
        layout.Add(child2);
        layout.Add(child3);

        layout.Measure(new SizeF(width: 200f, height: 100f));
        layout.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 100f));

        Assert.Equal(expected: 0f, child1.Bounds.X);
        Assert.Equal(expected: 20f, child2.Bounds.X);
        Assert.Equal(expected: 50f, child3.Bounds.X);
    }

    [Fact]
    public void Arrange_Vertical_ChildrenAreLaidOutTopToBottom()
    {
        MockLinearLayout layout = CreateLinearLayout();
        layout.Orientation.Value = Orientation.Vertical;

        MockVisual child1 = new() {MinimumWidth = {Value = 40f}, MinimumHeight = {Value = 20f}};
        MockVisual child2 = new() {MinimumWidth = {Value = 60f}, MinimumHeight = {Value = 30f}};

        layout.Add(child1);
        layout.Add(child2);

        layout.Measure(new SizeF(width: 200f, height: 200f));
        layout.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 0f, child1.Bounds.Y);
        Assert.Equal(expected: 20f, child2.Bounds.Y);
    }

    [Fact]
    public void Arrange_Vertical_ChildrenGetFullWidth()
    {
        MockLinearLayout layout = CreateLinearLayout();
        layout.Orientation.Value = Orientation.Vertical;

        MockVisual child1 = new() {MinimumWidth = {Value = 40f}, MinimumHeight = {Value = 20f}};
        MockVisual child2 = new() {MinimumWidth = {Value = 60f}, MinimumHeight = {Value = 30f}};

        layout.Add(child1);
        layout.Add(child2);

        layout.Measure(new SizeF(width: 200f, height: 200f));
        layout.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 200f, child1.Bounds.Width);
        Assert.Equal(expected: 200f, child2.Bounds.Width);
    }

    [Fact]
    public void Arrange_Vertical_ChildHeightEqualsDesiredHeight()
    {
        MockLinearLayout layout = CreateLinearLayout();
        layout.Orientation.Value = Orientation.Vertical;

        MockVisual child1 = new() {MinimumHeight = {Value = 20f}};
        MockVisual child2 = new() {MinimumHeight = {Value = 30f}};

        layout.Add(child1);
        layout.Add(child2);

        layout.Measure(new SizeF(width: 200f, height: 200f));
        layout.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 200f));

        Assert.Equal(expected: 20f, child1.Bounds.Height);
        Assert.Equal(expected: 30f, child2.Bounds.Height);
    }

    [Fact]
    public void Arrange_Vertical_WithPadding_ChildrenAreOffsetByPadding()
    {
        MockLinearLayout layout = CreateLinearLayout();
        layout.Orientation.Value = Orientation.Vertical;
        layout.Padding.Value = new ThicknessF(left: 10f, top: 5f, right: 10f, bottom: 5f);

        MockVisual child = new() {MinimumWidth = {Value = 40f}, MinimumHeight = {Value = 20f}};
        layout.Add(child);

        layout.Measure(new SizeF(width: 200f, height: 100f));
        layout.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 100f));

        Assert.Equal(expected: 10f, child.Bounds.X);
        Assert.Equal(expected: 5f, child.Bounds.Y);
    }

    [Fact]
    public void Arrange_Vertical_ThreeChildren_CorrectPositions()
    {
        MockLinearLayout layout = CreateLinearLayout();
        layout.Orientation.Value = Orientation.Vertical;

        MockVisual child1 = new() {MinimumHeight = {Value = 20f}};
        MockVisual child2 = new() {MinimumHeight = {Value = 30f}};
        MockVisual child3 = new() {MinimumHeight = {Value = 50f}};

        layout.Add(child1);
        layout.Add(child2);
        layout.Add(child3);

        layout.Measure(new SizeF(width: 200f, height: 300f));
        layout.Arrange(new RectangleF(x: 0f, y: 0f, width: 200f, height: 300f));

        Assert.Equal(expected: 0f, child1.Bounds.Y);
        Assert.Equal(expected: 20f, child2.Bounds.Y);
        Assert.Equal(expected: 50f, child3.Bounds.Y);
    }

    private class MockLinearLayout : LinearLayout
    {
        public void Add(Visual child)
        {
            AddChild(child);
        }
    }
}
