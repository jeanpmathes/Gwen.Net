using Gwen.Net.New.Utilities;

namespace Gwen.Net.Tests.Unit.New.Utilities;

public class RadiusFTests
{
    [Fact]
    public void UniformConstructor_SetsXAndYToSameValue()
    {
        RadiusF radius = new(5f);

        Assert.Equal(expected: 5f, radius.X);
        Assert.Equal(expected: 5f, radius.Y);
    }

    [Fact]
    public void ToString_ZeroAndNonZero_ReturnExpectedFormat()
    {
        RadiusF zero = RadiusF.Zero;
        RadiusF nonZero = new(x: 2f, y: 3f);

        Assert.Equal(expected: "RadiusF.Zero", zero.ToString());
        Assert.Equal(expected: "RadiusF(X: 2, Y: 3)", nonZero.ToString());
    }
}
