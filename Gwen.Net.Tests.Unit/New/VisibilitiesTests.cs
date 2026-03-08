using Gwen.Net.New;

namespace Gwen.Net.Tests.Unit.New;

public sealed class VisibilitiesTests
{
    [Fact]
    public void IsVisible_Visible_ReturnsTrue()
    {
        Assert.True(Visibility.Visible.IsVisible);
    }

    [Fact]
    public void IsVisible_Hidden_ReturnsFalse()
    {
        Assert.False(Visibility.Hidden.IsVisible);
    }

    [Fact]
    public void IsVisible_Collapsed_ReturnsFalse()
    {
        Assert.False(Visibility.Collapsed.IsVisible);
    }

    [Fact]
    public void IsLayouted_Hidden_ReturnsTrue()
    {
        Assert.True(Visibility.Hidden.IsLayouted);
    }

    [Fact]
    public void IsLayouted_Collapsed_ReturnsFalse()
    {
        Assert.False(Visibility.Collapsed.IsLayouted);
    }
}
