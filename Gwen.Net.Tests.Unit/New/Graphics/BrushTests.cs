using System.Drawing;
using Gwen.Net.New.Graphics;

namespace Gwen.Net.Tests.Unit.New.Graphics;

public class BrushTests
{
    [Fact]
    public void TransparentBrush_IsEqualToAllOtherInstances()
    {
        TransparentBrush brush1 = new();
        TransparentBrush brush2 = new();

        Assert.True(brush1.Equals(brush2));
    }

    [Fact]
    public void TransparentBrush_GetHashCode_ReturnsSameValueForAllInstances()
    {
        TransparentBrush brush1 = new();
        TransparentBrush brush2 = new();

        Assert.Equal(brush1.GetHashCode(), brush2.GetHashCode());
    }

    [Fact]
    public void SolidColorBrush_TwoIndependentInstancesWithSameColor_AreEqual()
    {
        SolidColorBrush first = new(Color.Red);
        SolidColorBrush second = new(Color.Red);

        Assert.True(first.Equals(second));
    }
}
