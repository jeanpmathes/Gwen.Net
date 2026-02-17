using System.Drawing;
using Gwen.Net.New.Utilities;

namespace Gwen.Net.Tests.Unit.New.Utilities;

public class GeometryUtilitiesTests
{
    [Fact]
    public void SizesClamp_ConstrainsBothDimensions()
    {
        SizeF result = Sizes.Clamp(new SizeF(width: 50, height: 2), new SizeF(width: 10, height: 5), new SizeF(width: 30, height: 40));

        Assert.Equal(new SizeF(width: 30, height: 5), result);
    }
    
    [Fact]
    public void SizesClamp_ClampsWidthAndHeightIndependently()
    {
        SizeF result = Sizes.Clamp(new SizeF(width: 5, height: 50), new SizeF(width: 10, height: 5), new SizeF(width: 30, height: 40));

        Assert.Equal(new SizeF(width: 10, height: 40), result);
    }
    
    [Fact]
    public void SizesMax_ReturnsComponentWiseMaximum()
    {
        SizeF result = Sizes.Max(new SizeF(width: 10, height: 50), new SizeF(width: 20, height: 40));

        Assert.Equal(new SizeF(width: 20, height: 50), result);
    }

    [Fact]
    public void RectanglesClamp_ClampsSizeOfRectangle()
    {
        RectangleF result = Rectangles.ClampSize(new RectangleF(location: PointF.Empty, new SizeF(width: 50, height: 2)), new SizeF(width: 10, height: 5), new SizeF(width: 30, height: 40));

        Assert.Equal(new RectangleF(location: PointF.Empty, new SizeF(width: 30, height: 5)), result);
    }
    
    [Fact]
    public void RectanglesClamp_PreservesLocation()
    {
        RectangleF result = Rectangles.ClampSize(new RectangleF(new PointF(x: 5, y: 10), new SizeF(width: 50, height: 2)), new SizeF(width: 10, height: 5), new SizeF(width: 30, height: 40));

        Assert.Equal(new RectangleF(new PointF(x: 5, y: 10), new SizeF(width: 30, height: 5)), result);
    }
}
