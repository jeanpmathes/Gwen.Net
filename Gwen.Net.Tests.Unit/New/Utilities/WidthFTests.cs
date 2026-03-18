using Gwen.Net.New.Utilities;

namespace Gwen.Net.Tests.Unit.New.Utilities;

public class WidthFTests
{
    [Fact]
    public void UniformConstructor_SetsXAndYToSameValue()
    {
        RadiusF radius = new(5f);

        Assert.Equal(expected: 5f, radius.X);
        Assert.Equal(expected: 5f, radius.Y);
    }

    [Fact]
    public void ToString_Zero_ReturnsExpectedFormat()
    {
        Assert.Equal(expected: "WidthF.Zero", WidthF.Zero.ToString());
    }

    [Fact]
    public void ToString_NonZero_ReturnsExpectedFormat()
    {
        Assert.Equal(expected: "WidthF(Value: 5)", new WidthF(5.0f).ToString());
    }
}
