using Gwen.Net.New.Bindings;
using Gwen.Net.Tests.Unit.New.Controls;

namespace Gwen.Net.Tests.Unit.New.Bindings;

public class PropertyTests
{
    [Fact]
    public void GetValue_WhenInactive_ReflectsCurrentBindingValue()
    {
        MockControl owner = new();
        Slot<Int32> source = new(10);
        Property<Int32> property = Property.Create(owner, defaultValue: 0);
        property.Binding = Binding.To(source);

        source.SetValue(11);

        Assert.Equal(expected: 11, property.GetValue());
    }

    [Fact]
    public void ActiveProperty_RaisesValueChangedOnlyForActualValueChange()
    {
        MockControl owner = new();
        Slot<Int32> source = new(2);
        Property<Int32> property = Property.Create(owner, defaultValue: 0);
        property.Binding = Binding.To(source);

        Int32 events = 0;
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
        MockControl owner = new();
        Slot<Int32> source = new(1);
        Property<Int32> property = Property.Create(owner, defaultValue: 0);
        property.Binding = Binding.To(source);

        Int32 events = 0;
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
        MockControl owner = new();
        Property<Int32> property = Property.Create(owner, defaultValue: 1);

        property.Style(5);
        property.Value = 8;

        Assert.Equal(expected: 8, property.GetValue());
    }

    [Fact]
    public void CoercionBinding_IsAppliedToDefaultValue()
    {
        MockControl owner = new();
        Binding<Int32, Int32> clamp = Binding.Computed<Int32, Int32>(input => Math.Clamp(input, min: 0, max: 3));
        Property<Int32> property = Property.Create(owner, defaultValue: 5, clamp);

        Assert.Equal(expected: 3, property.GetValue());
    }

    [Fact]
    public void CoercionBinding_IsAppliedToLocalValue()
    {
        MockControl owner = new();
        Binding<Int32, Int32> clamp = Binding.Computed<Int32, Int32>(input => Math.Clamp(input, min: 0, max: 3));
        Property<Int32> property = Property.Create(owner, defaultValue: 0, clamp);

        property.Value = 7;

        Assert.Equal(expected: 3, property.GetValue());
    }

    [Fact]
    public void CoercionBinding_TriggersValueChangedWhenCoercedResultChanges()
    {
        MockControl owner = new();
        Slot<Int32> maxSlot = new(10);
        Binding<Int32, Int32> clamp = Binding.To(maxSlot).Parametrize<Int32, Int32>((input, max) => Math.Clamp(input, min: 0, max));
        Property<Int32> property = Property.Create(owner, defaultValue: 15, clamp);

        property.Activate();

        Int32 events = 0;
        property.ValueChanged += (_, _) => events++;

        maxSlot.SetValue(5);

        Assert.Equal(expected: 1, events);
        Assert.Equal(expected: 5, property.GetValue());
    }

    [Fact]
    public void CoercionBinding_DoesNotTriggerValueChangedWhenCoercedResultIsUnchanged()
    {
        MockControl owner = new();
        Binding<Int32, Int32> clamp = Binding.Computed<Int32, Int32>(input => Math.Clamp(input, min: 0, max: 3));
        Property<Int32> property = Property.Create(owner, defaultValue: 5, clamp);

        property.Activate();

        Int32 events = 0;
        property.ValueChanged += (_, _) => events++;

        property.Value = 7; // Still clamps to 3, no actual change.

        Assert.Equal(expected: 0, events);
        Assert.Equal(expected: 3, property.GetValue());
    }
}
