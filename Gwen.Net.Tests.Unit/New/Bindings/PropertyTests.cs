using Gwen.Net.New.Bindings;
using Gwen.Net.Tests.Unit.New.Controls;

namespace Gwen.Net.Tests.Unit.New.Bindings;

public class PropertyTests
{
    [Fact]
    public void GetValue_WhenInactive_ReflectsCurrentBindingValue()
    {
        var owner = new MockControl();
        var source = new Slot<Int32>(10);
        Property<Int32> property = Property.Create(owner, defaultValue: 0);
        property.Binding = Binding.To(source);

        source.SetValue(11);

        Assert.Equal(expected: 11, property.GetValue());
    }

    [Fact]
    public void ActiveProperty_RaisesValueChangedOnlyForActualValueChange()
    {
        var owner = new MockControl();
        var source = new Slot<Int32>(2);
        Property<Int32> property = Property.Create(owner, defaultValue: 0);
        property.Binding = Binding.To(source);

        var events = 0;
        property.ValueChanged += (_, _) => events++;

        property.Activate();
        source.SetValue(2); // Does not cause an event as it matches the current value.
        source.SetValue(9);

        Assert.Equal(expected: 2, events);
        Assert.Equal(expected: 9, property.GetValue());
    }

    [Fact]
    public void Deactivate_UnsubscribesFromBindingNotifications()
    {
        var owner = new MockControl();
        var source = new Slot<Int32>(1);
        Property<Int32> property = Property.Create(owner, defaultValue: 0);
        property.Binding = Binding.To(source);

        var events = 0;
        property.ValueChanged += (_, _) => events++;

        property.Activate(); // Only this causes an event.
        property.Deactivate();
        source.SetValue(6);

        Assert.Equal(expected: 1, events);
        Assert.Equal(expected: 6, property.GetValue());
    }

    [Fact]
    public void LocalValue_TakesPrecedenceOverStyle()
    {
        var owner = new MockControl();
        Property<Int32> property = Property.Create(owner, defaultValue: 1);

        property.Style(5);
        property.Value = 8;

        Assert.Equal(expected: 8, property.GetValue());
    }
}
