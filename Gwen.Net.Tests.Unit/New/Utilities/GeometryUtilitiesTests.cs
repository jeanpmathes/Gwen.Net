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
    public void ConstraintSize_OnlyShrinksDimensionsThatExceedLimit()
    {
        var rectangle = new RectangleF(x: 3, y: 4, width: 100, height: 20);

        RectangleF result = Rectangles.ConstraintSize(rectangle, new SizeF(width: 80, height: 25));

        Assert.Equal(new RectangleF(x: 3, y: 4, width: 80, height: 20), result);
    }
}
