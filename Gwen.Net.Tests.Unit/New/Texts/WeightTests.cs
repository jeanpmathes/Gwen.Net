using Gwen.Net.New.Texts;

namespace Gwen.Net.Tests.Unit.New.Texts;

public class WeightTests
{
    [Fact]
    public void GreaterThan_ReturnsTrueWhenLeftIsHeavier()
    {
        Assert.True(Weight.Bold > Weight.Normal);
    }

    [Fact]
    public void GreaterThan_ReturnsFalseWhenLeftIsLighter()
    {
        Assert.False(Weight.Normal > Weight.Bold);
    }

    [Fact]
    public void LessThan_ReturnsTrueWhenLeftIsLighter()
    {
        Assert.True(Weight.Normal < Weight.Bold);
    }

    [Fact]
    public void LessThan_ReturnsFalseWhenLeftIsHeavier()
    {
        Assert.False(Weight.Bold < Weight.Normal);
    }

    [Fact]
    public void GreaterThanOrEqual_ReturnsTrueWhenHeavier()
    {
        Assert.True(Weight.Bold >= Weight.Normal);
    }

    [Fact]
    public void GreaterThanOrEqual_ReturnsFalseWhenLighter()
    {
        Assert.False(Weight.Normal >= Weight.Bold);
    }

    [Fact]
    public void LessThanOrEqual_ReturnsTrueWhenLighter()
    {
        Assert.True(Weight.Normal <= Weight.Bold);
    }

    [Fact]
    public void LessThanOrEqual_ReturnsFalseWhenHeavier()
    {
        Assert.False(Weight.Bold <= Weight.Normal);
    }
}
