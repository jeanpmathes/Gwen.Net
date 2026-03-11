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
    public void Equals_ReturnsTrueForIdenticalThicknesses()
    {
        ThicknessF a = new(left: 1f, top: 2f, right: 3f, bottom: 4f);
        ThicknessF b = new(left: 1f, top: 2f, right: 3f, bottom: 4f);

        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Equals_ReturnsFalseWhenAnySideDiffers()
    {
        ThicknessF reference = new(left: 1f, top: 2f, right: 3f, bottom: 4f);

        Assert.False(reference.Equals(new ThicknessF(left: 0f, top: 2f, right: 3f, bottom: 4f)));
        Assert.False(reference.Equals(new ThicknessF(left: 1f, top: 0f, right: 3f, bottom: 4f)));
        Assert.False(reference.Equals(new ThicknessF(left: 1f, top: 2f, right: 0f, bottom: 4f)));
        Assert.False(reference.Equals(new ThicknessF(left: 1f, top: 2f, right: 3f, bottom: 0f)));
    }

    [Fact]
    public void Equals_AlignsWithEqualityOperator()
    {
        ThicknessF a = new(left: 1f, top: 2f, right: 3f, bottom: 4f);
        ThicknessF b = new(left: 1f, top: 2f, right: 3f, bottom: 4f);
        ThicknessF c = new(left: 0f, top: 0f, right: 0f, bottom: 0f);

        Assert.True(a == b);
        Assert.False(a == c);
    }

    [Fact]
    public void GetHashCode_ReturnsSameValueForEqualThicknesses()
    {
        ThicknessF a = new(left: 1f, top: 2f, right: 3f, bottom: 4f);
        ThicknessF b = new(left: 1f, top: 2f, right: 3f, bottom: 4f);

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }
}
