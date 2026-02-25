using Gwen.Net.New.Texts;

namespace Gwen.Net.Tests.Unit.New.Texts;

public class WeightTests
{
    [Theory]
    [InlineData((short)0)]
    [InlineData((short)-1)]
    [InlineData((short)1000)]
    public void Constructor_ThrowsForOutOfRangeValues(short value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = new Weight(value));
    }

    [Theory]
    [InlineData((short)1)]
    [InlineData((short)400)]
    [InlineData((short)999)]
    public void Constructor_AllowsValuesInRange(short value)
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
