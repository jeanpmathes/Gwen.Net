using System.Drawing;
using Gwen.Net.New.Utilities;

namespace Gwen.Net.Tests.Unit.New.Utilities;

public class ThicknessFTests
{
    [Fact]
    public void UniformConstructor_SetsAllSidesToSameValue()
    {
        ThicknessF thickness = new(5f);

        Assert.Equal(expected: 5f, thickness.Left);
        Assert.Equal(expected: 5f, thickness.Top);
        Assert.Equal(expected: 5f, thickness.Right);
        Assert.Equal(expected: 5f, thickness.Bottom);
    }

    [Fact]
    public void FourSideConstructor_SetsEachSideIndependently()
    {
        ThicknessF thickness = new(left: 1f, top: 2f, right: 3f, bottom: 4f);

        Assert.Equal(expected: 1f, thickness.Left);
        Assert.Equal(expected: 2f, thickness.Top);
        Assert.Equal(expected: 3f, thickness.Right);
        Assert.Equal(expected: 4f, thickness.Bottom);
    }

    [Fact]
    public void Default_HasAllSidesZero()
    {
        ThicknessF thickness = default;

        Assert.Equal(expected: 0f, thickness.Left);
        Assert.Equal(expected: 0f, thickness.Top);
        Assert.Equal(expected: 0f, thickness.Right);
        Assert.Equal(expected: 0f, thickness.Bottom);
    }

    [Fact]
    public void SizePlusThickness_IncreasesWidthAndHeight()
    {
        SizeF size = new(width: 100f, height: 50f);
        ThicknessF thickness = new(left: 1f, top: 2f, right: 3f, bottom: 4f);

        SizeF result = size + thickness;

        Assert.Equal(new SizeF(width: 104f, height: 56f), result);
    }

    [Fact]
    public void SizeMinusThickness_DecreasesWidthAndHeight()
    {
        SizeF size = new(width: 100f, height: 50f);
        ThicknessF thickness = new(left: 1f, top: 2f, right: 3f, bottom: 4f);

        SizeF result = size - thickness;

        Assert.Equal(new SizeF(width: 96f, height: 44f), result);
    }

    [Fact]
    public void RectanglePlusThickness_ExpandsRectangleOutward()
    {
        RectangleF rect = new(x: 10f, y: 20f, width: 100f, height: 50f);
        ThicknessF thickness = new(left: 1f, top: 2f, right: 3f, bottom: 4f);

        RectangleF result = rect + thickness;

        Assert.Equal(expected: 9f, result.X);
        Assert.Equal(expected: 18f, result.Y);
        Assert.Equal(expected: 104f, result.Width);
        Assert.Equal(expected: 56f, result.Height);
    }

    [Fact]
    public void RectangleMinusThickness_ShrinksRectangleInward()
    {
        RectangleF rect = new(x: 10f, y: 20f, width: 100f, height: 50f);
        ThicknessF thickness = new(left: 1f, top: 2f, right: 3f, bottom: 4f);

        RectangleF result = rect - thickness;

        Assert.Equal(expected: 11f, result.X);
        Assert.Equal(expected: 22f, result.Y);
        Assert.Equal(expected: 96f, result.Width);
        Assert.Equal(expected: 44f, result.Height);
    }

    [Fact]
    public void ToString_Zero_ReturnsExpectedFormat()
    {
        Assert.Equal(expected: "ThicknessF.Zero", ThicknessF.Zero.ToString());
    }

    [Fact]
    public void ToString_NonZero_ReturnsExpectedFormat()
    {
        Assert.Equal(expected: "ThicknessF(Left: 1, Top: 2, Right: 3, Bottom: 4)", new ThicknessF(left: 1f, top: 2f, right: 3f, bottom: 4f).ToString());
    }
}
