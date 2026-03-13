using Gwen.Net.New.Bindings;
using Gwen.Net.New.Styles;
using Gwen.Net.Tests.Unit.New.Controls;

namespace Gwen.Net.Tests.Unit.New.Styles;

public class StyleTests
{
    [Fact]
    public void Clear_RevertsStyledPropertyToDefault()
    {
        Style<MockControl> style = Styling.Create<MockControl>(builder => builder.Set(c => c.MinimumHeight, value: 12f));
        MockControl element = new();

        style.Apply(element);
        Assert.Equal(expected: 12f, element.MinimumHeight.GetValue());

        style.Clear(element);
        Assert.Equal(expected: 1f, element.MinimumHeight.GetValue());
    }

    [Fact]
    public void Set_WithBinding_TracksSourceUpdates()
    {
        Slot<Single> minimumHeight = new(12f);

        Style<MockControl> style = Styling.Create<MockControl>(builder => builder.Set(c => c.MinimumHeight, Binding.To(minimumHeight)));
        MockControl element = new();

        style.Apply(element);
        Assert.Equal(expected: 12f, element.MinimumHeight.GetValue());

        minimumHeight.SetValue(24f);

        Assert.Equal(expected: 24f, element.MinimumHeight.GetValue());
    }

    [Fact]
    public void Trigger_IsAppliedAndRemovedDependingOnCondition()
    {
        Slot<Boolean> isTriggered = new(false);

        Style<MockControl> style = Styling.Create<MockControl>(builder => builder
            .Set(c => c.MinimumHeight, value: 10f)
            .Trigger(_ => Binding.To(isTriggered), c => c.MinimumHeight, value: 20f));

        MockControl element = new();
        style.Apply(element);

        Assert.Equal(expected: 10f, element.MinimumHeight.GetValue());

        isTriggered.SetValue(true);
        Assert.Equal(expected: 20f, element.MinimumHeight.GetValue());

        isTriggered.SetValue(false);
        Assert.Equal(expected: 10f, element.MinimumHeight.GetValue());
    }
}
