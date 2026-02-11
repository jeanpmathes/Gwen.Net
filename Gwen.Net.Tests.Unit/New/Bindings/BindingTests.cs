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
}
