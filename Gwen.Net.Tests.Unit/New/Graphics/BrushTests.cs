using System.Drawing;
using Gwen.Net.New.Graphics;

namespace Gwen.Net.Tests.Unit.New.Graphics;

public class BrushTests
{
    [Fact]
    public void TransparentBrush_IsEqualToAllOtherInstances()
    {
        TransparentBrush brush1 = new TransparentBrush();
        TransparentBrush brush2 = new TransparentBrush();

        Assert.True(brush1.Equals(brush2));
    }
    
    [Fact]
    public void TransparentBrush_GetHashCode_ReturnsSameValueForAllInstances()
    {
        TransparentBrush brush1 = new TransparentBrush();
        TransparentBrush brush2 = new TransparentBrush();

        Assert.Equal(brush1.GetHashCode(), brush2.GetHashCode());
    }

    [Fact]
    public void SolidColorBrush_TwoIndependentInstancesWithSameColor_AreEqual()
    {
        SolidColorBrush first = new SolidColorBrush(Color.Red);
        SolidColorBrush second = new SolidColorBrush(Color.Red);

        Assert.True(first.Equals(second));
    }
}
