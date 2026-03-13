using Gwen.Net.New;

namespace Gwen.Net.Tests.Unit.New;

public class EnablementsTests
{
    [Fact]
    public void IsEnabled_Enabled_ReturnsTrue()
    {
        Assert.True(Enablement.Enabled.IsEnabled);
    }

    [Fact]
    public void IsEnabled_ReadOnly_ReturnsFalse()
    {
        Assert.False(Enablement.ReadOnly.IsEnabled);
    }

    [Fact]
    public void IsEnabled_Disabled_ReturnsFalse()
    {
        Assert.False(Enablement.Disabled.IsEnabled);
    }

    [Fact]
    public void IsFocusable_Enabled_ReturnsTrue()
    {
        Assert.True(Enablement.Enabled.IsFocusable);
    }

    [Fact]
    public void IsFocusable_ReadOnly_ReturnsTrue()
    {
        Assert.True(Enablement.ReadOnly.IsFocusable);
    }

    [Fact]
    public void IsFocusable_Disabled_ReturnsFalse()
    {
        Assert.False(Enablement.Disabled.IsFocusable);
    }

    [Fact]
    public void CanReceiveInput_Enabled_ReturnsTrue()
    {
        Assert.True(Enablement.Enabled.CanReceiveInput);
    }

    [Fact]
    public void CanReceiveInput_ReadOnly_ReturnsTrue()
    {
        Assert.True(Enablement.ReadOnly.CanReceiveInput);
    }

    [Fact]
    public void CanReceiveInput_Disabled_ReturnsFalse()
    {
        Assert.False(Enablement.Disabled.CanReceiveInput);
    }

    [Fact]
    public void FromBoolean_True_ReturnsEnabled()
    {
        Assert.Equal(Enablement.Enabled, Enablements.FromBoolean(true));
    }

    [Fact]
    public void FromBoolean_False_ReturnsDisabled()
    {
        Assert.Equal(Enablement.Disabled, Enablements.FromBoolean(false));
    }

    [Fact]
    public void IsDisabled_Enabled_ReturnsFalse()
    {
        Assert.False(Enablement.Enabled.IsDisabled);
    }

    [Fact]
    public void IsDisabled_ReadOnly_ReturnsFalse()
    {
        Assert.False(Enablement.ReadOnly.IsDisabled);
    }

    [Fact]
    public void IsDisabled_Disabled_ReturnsTrue()
    {
        Assert.True(Enablement.Disabled.IsDisabled);
    }
}
