using System.Drawing;
using Gwen.Net.New.Utilities;

namespace Gwen.Net.Tests.Unit.New.Rendering;

public class RendererTests
{
    private sealed class MockRenderer : Rendering.MockRenderer
    {
        public new PointF ApplyScale(PointF point) => base.ApplyScale(point);
        public new SizeF ApplyScale(SizeF size) => base.ApplyScale(size);
        public new RectangleF ApplyScale(RectangleF rectangle) => base.ApplyScale(rectangle);
        public new ThicknessF ApplyScale(ThicknessF thickness) => base.ApplyScale(thickness);

        public new PointF ApplyInverseScale(PointF point) => base.ApplyInverseScale(point);
        public new SizeF ApplyInverseScale(SizeF size) => base.ApplyInverseScale(size);
        public new RectangleF ApplyInverseScale(RectangleF rectangle) => base.ApplyInverseScale(rectangle);
        public new ThicknessF ApplyInverseScale(ThicknessF thickness) => base.ApplyInverseScale(thickness);
    }

    [Fact]
    public void Scale_ApplyScale_ScalesPoint()
    {
        var renderer = new MockRenderer();
        renderer.Scale(2f);

        PointF result = renderer.ApplyScale(new PointF(x: 3f, y: 5f));

        Assert.Equal(expected: 6f, result.X);
        Assert.Equal(expected: 10f, result.Y);
    }

    [Fact]
    public void Scale_ApplyScale_ScalesSize()
    {
        var renderer = new MockRenderer();
        renderer.Scale(2f);

        SizeF result = renderer.ApplyScale(new SizeF(width: 3f, height: 5f));

        Assert.Equal(expected: 6f, result.Width);
        Assert.Equal(expected: 10f, result.Height);
    }

    [Fact]
    public void Scale_ApplyScale_ScalesRectangle()
    {
        var renderer = new MockRenderer();
        renderer.Scale(2f);

        RectangleF result = renderer.ApplyScale(new RectangleF(x: 3f, y: 5f, width: 10f, height: 20f));

        Assert.Equal(expected: 6f, result.X);
        Assert.Equal(expected: 10f, result.Y);
        Assert.Equal(expected: 20f, result.Width);
        Assert.Equal(expected: 40f, result.Height);
    }

    [Fact]
    public void Scale_ApplyScale_ScalesThickness()
    {
        var renderer = new MockRenderer();
        renderer.Scale(2f);

        ThicknessF result = renderer.ApplyScale(new ThicknessF(left: 1f, top: 2f, right: 3f, bottom: 4f));

        Assert.Equal(expected: 2f, result.Left);
        Assert.Equal(expected: 4f, result.Top);
        Assert.Equal(expected: 6f, result.Right);
        Assert.Equal(expected: 8f, result.Bottom);
    }

    [Fact]
    public void Scale_ApplyInverseScale_UnscalesPoint()
    {
        var renderer = new MockRenderer();
        renderer.Scale(2f);

        PointF result = renderer.ApplyInverseScale(new PointF(x: 6f, y: 10f));

        Assert.Equal(expected: 3f, result.X);
        Assert.Equal(expected: 5f, result.Y);
    }

    [Fact]
    public void Scale_ApplyInverseScale_UnscalesSize()
    {
        var renderer = new MockRenderer();
        renderer.Scale(2f);

        SizeF result = renderer.ApplyInverseScale(new SizeF(width: 6f, height: 10f));

        Assert.Equal(expected: 3f, result.Width);
        Assert.Equal(expected: 5f, result.Height);
    }

    [Fact]
    public void Scale_ApplyInverseScale_UnscalesRectangle()
    {
        var renderer = new MockRenderer();
        renderer.Scale(2f);

        RectangleF result = renderer.ApplyInverseScale(new RectangleF(x: 6f, y: 10f, width: 20f, height: 40f));

        Assert.Equal(expected: 3f, result.X);
        Assert.Equal(expected: 5f, result.Y);
        Assert.Equal(expected: 10f, result.Width);
        Assert.Equal(expected: 20f, result.Height);
    }

    [Fact]
    public void Scale_ApplyInverseScale_UnscalesThickness()
    {
        var renderer = new MockRenderer();
        renderer.Scale(2f);

        ThicknessF result = renderer.ApplyInverseScale(new ThicknessF(left: 2f, top: 4f, right: 6f, bottom: 8f));

        Assert.Equal(expected: 1f, result.Left);
        Assert.Equal(expected: 2f, result.Top);
        Assert.Equal(expected: 3f, result.Right);
        Assert.Equal(expected: 4f, result.Bottom);
    }
}
