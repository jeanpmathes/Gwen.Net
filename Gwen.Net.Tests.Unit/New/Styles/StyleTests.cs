using Gwen.Net.New.Styles;
using Gwen.Net.Tests.Unit.New.Controls;

namespace Gwen.Net.Tests.Unit.New.Styles;

public class StyleTests
{
    [Fact]
    public void StyleClear_RevertsStyledPropertyToDefault()
    {
        Style<MockControl> style = Styling.Create<MockControl>(builder => builder.Set(c => c.MinimumHeight, value: 12f));
        var element = new MockControl();

        style.Apply(element);
        Assert.Equal(expected: 12f, element.MinimumHeight.GetValue());

        style.Clear(element);
        Assert.Equal(expected: 1f, element.MinimumHeight.GetValue());
    }
}
