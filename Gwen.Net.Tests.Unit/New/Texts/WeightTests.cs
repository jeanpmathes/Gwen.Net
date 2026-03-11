using Gwen.Net.New.Texts;

namespace Gwen.Net.Tests.Unit.New.Texts;

public class WeightTests
{
    [Theory]
    [InlineData((Int16) 0)]
    [InlineData((Int16) (-1))]
    [InlineData((Int16) 1000)]
    public void Constructor_ThrowsForOutOfRangeValues(Int16 value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = new Weight(value));
    }

    [Theory]
    [InlineData((Int16) 1)]
    [InlineData((Int16) 400)]
    [InlineData((Int16) 999)]
    public void Constructor_AllowsValuesInRange(Int16 value)
    {
        Weight weight = new(value);

        Assert.Equal(value, weight.Value);
    }

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
