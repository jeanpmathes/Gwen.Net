using Gwen.Net.New.Bindings;

namespace Gwen.Net.Tests.Unit.New.Bindings;

public class BindingTests
{
    [Fact]
    public void Constant_ReturnsConfiguredValue()
    {
        Binding<Int32> binding = Binding.Constant(42);

        Assert.Equal(expected: 42, binding.GetValue());
    }

    [Fact]
    public void Transform_WithSingleSource_EqualDependencyValue_DoesNotRaiseValueChanged()
    {
        var source = new Slot<Int32>(3);
        Binding<Int32> binding = Binding.Transform(source, value => value * 2);
        var events = 0;
        binding.ValueChanged += (_, _) => events++;

        source.SetValue(3);

        Assert.Equal(expected: 6, binding.GetValue());
        Assert.Equal(expected: 0, events);
    }
    
    [Fact]
    public void Transform_WithSingleSource_ReactsToDependencyChanges()
    {
        var source = new Slot<Int32>(3);
        Binding<Int32> binding = Binding.Transform(source, value => value * 2);
        var events = 0;
        binding.ValueChanged += (_, _) => events++;

        source.SetValue(5);

        Assert.Equal(expected: 10, binding.GetValue());
        Assert.Equal(expected: 1, events);
    }

    [Fact]
    public void Transform_WithTwoSources_ReactsToDependencyChanges()
    {
        var source1 = new Slot<Int32>(3);
        var source2 = new Slot<Int32>(4);
        Binding<Int32> binding = Binding.Transform(source1, source2, (value1, value2) => value1 * value2);
        var events = 0;
        binding.ValueChanged += (_, _) => events++;

        source1.SetValue(5);

        Assert.Equal(expected: 20, binding.GetValue());
        Assert.Equal(expected: 1, events);

        source2.SetValue(6);

        Assert.Equal(expected: 30, binding.GetValue());
        Assert.Equal(expected: 2, events);
    }
    
    [Fact]
    public void Transform_WithThreeSources_ReactsToDependencyChanges()
    {
        var source1 = new Slot<Int32>(2);
        var source2 = new Slot<Int32>(3);
        var source3 = new Slot<Int32>(4);
        Binding<Int32> binding = Binding.Transform(source1, source2, source3, (value1, value2, value3) => value1 * value2 * value3);
        var events = 0;
        binding.ValueChanged += (_, _) => events++;

        source1.SetValue(5);

        Assert.Equal(expected: 60, binding.GetValue());
        Assert.Equal(expected: 1, events);

        source2.SetValue(6);

        Assert.Equal(expected: 120, binding.GetValue());
        Assert.Equal(expected: 2, events);

        source3.SetValue(7);

        Assert.Equal(expected: 210, binding.GetValue());
        Assert.Equal(expected: 3, events);
    }
    
    [Fact]
    public void FlatTransform_ReturnsValueFromInitialInnerSource()
    {
        Slot<Int32> outer = new(0);
        Slot<Int32> inner = new(42);

        Binding<Int32> binding = Binding.FlatTransform(outer, _ => inner);

        Assert.Equal(expected: 42, binding.GetValue());
    }

    [Fact]
    public void FlatTransform_WhenOuterChanges_SwitchesToNewInnerSource()
    {
        Slot<Int32> outer = new(0);
        Slot<Int32> inner1 = new(10);
        Slot<Int32> inner2 = new(20);

        Binding<Int32> binding = Binding.FlatTransform(outer, v => v == 0 ? inner1 : inner2);

        outer.SetValue(1);

        Assert.Equal(expected: 20, binding.GetValue());
    }

    [Fact]
    public void FlatTransform_WhenCurrentInnerChanges_RaisesValueChanged()
    {
        Slot<Int32> outer = new(0);
        Slot<Int32> inner = new(0);
        var events = 0;

        Binding<Int32> binding = Binding.FlatTransform(outer, _ => inner);
        binding.ValueChanged += (_, _) => events++;

        inner.SetValue(99);

        Assert.Equal(expected: 1, events);
        Assert.Equal(expected: 99, binding.GetValue());
    }

    [Fact]
    public void FlatTransform_WhenOuterChanges_RaisesValueChanged()
    {
        Slot<Int32> outer = new(0);
        Slot<Int32> inner1 = new(10);
        Slot<Int32> inner2 = new(20);
        var events = 0;

        Binding<Int32> binding = Binding.FlatTransform(outer, v => v == 0 ? inner1 : inner2);
        binding.ValueChanged += (_, _) => events++;

        outer.SetValue(1);

        Assert.Equal(expected: 1, events);
    }

    [Fact]
    public void FlatTransform_AfterOuterChanges_NoLongerReactsToOldInner()
    {
        Slot<Int32> outer = new(0);
        Slot<Int32> inner1 = new(10);
        Slot<Int32> inner2 = new(20);
        var events = 0;

        Binding<Int32> binding = Binding.FlatTransform(outer, v => v == 0 ? inner1 : inner2);

        outer.SetValue(1);

        binding.ValueChanged += (_, _) => events++;
        inner1.SetValue(99);

        Assert.Equal(expected: 0, events);
        Assert.Equal(expected: 20, binding.GetValue());
    }

    [Fact]
    public void FlatTransform_AfterOuterChanges_ReactsToNewInner()
    {
        Slot<Int32> outer = new(0);
        Slot<Int32> inner1 = new(10);
        Slot<Int32> inner2 = new(20);

        Binding<Int32> binding = Binding.FlatTransform(outer, v => v == 0 ? inner1 : inner2);

        outer.SetValue(1);
        inner2.SetValue(30);

        Assert.Equal(expected: 30, binding.GetValue());
    }

    [Fact]
    public void FlatTransform_Nullable_ReturnsDefaultWhenSelectorReturnsNull()
    {
        Slot<Int32> outer = new(0);

        Binding<String> binding = Binding.FlatTransform<Int32, String>(outer, _ => null, defaultValue: "default");

        Assert.Equal(expected: "default", binding.GetValue());
    }

    [Fact]
    public void FlatTransform_Nullable_WhenSelectorReturnsSource_ReturnsItsValue()
    {
        Slot<Int32> outer = new(0);
        Slot<String> inner = new("hello");

        Binding<String> binding = Binding.FlatTransform<Int32, String>(outer, _ => inner, defaultValue: "default");

        Assert.Equal(expected: "hello", binding.GetValue());
    }

    [Fact]
    public void FlatTransform_Nullable_WhenSelectorBecomesNull_ReturnsDefault()
    {
        Slot<Boolean> outer = new(true);
        Slot<String> inner = new("hello");

        Binding<String> binding = Binding.FlatTransform<Boolean, String>(outer, v => v ? inner : null, defaultValue: "default");

        outer.SetValue(false);

        Assert.Equal(expected: "default", binding.GetValue());
    }
}
