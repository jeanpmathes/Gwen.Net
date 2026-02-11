using System.Drawing;
using Gwen.Net.New.Bindings;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.Tests.Unit.New.Bindings;

public class VisualPropertyTests
{
    [Fact]
    public void InvalidationMeasure_TriggersNewMeasurePass()
    {
        var visual = new CountingVisual();
        VisualProperty<Single> property = VisualProperty.Create(visual, defaultValue: 1f, Invalidation.Measure);

        property.Activate();
        visual.Measure(new SizeF(width: 100, height: 100));
        Int32 firstCount = visual.MeasureCalls;

        property.Value = 2f;
        visual.Measure(new SizeF(width: 100, height: 100));

        Assert.Equal(firstCount + 1, visual.MeasureCalls);
    }

    [Fact]
    public void InvalidationArrange_TriggersNewArrangePass()
    {
        var visual = new CountingVisual();
        VisualProperty<Single> property = VisualProperty.Create(visual, defaultValue: 1f, Invalidation.Arrange);

        property.Activate();
        visual.Measure(new SizeF(width: 100, height: 100));
        visual.Arrange(new RectangleF(x: 0, y: 0, width: 100, height: 100));
        Int32 firstCount = visual.ArrangeCalls;

        property.Value = 2f;
        visual.Arrange(new RectangleF(x: 0, y: 0, width: 100, height: 100));

        Assert.Equal(firstCount + 1, visual.ArrangeCalls);
    }

    [Fact]
    public void ActiveProperty_CachesValueAndEmitsChangeOnlyWhenValueChanges()
    {
        var visual = new CountingVisual();
        var source = new Slot<Int32>(3);
        VisualProperty<Int32> property = VisualProperty.Create(visual, Binding.Transform(source, x => x));

        var events = 0;
        property.ValueChanged += (_, _) => events++;

        property.Activate();
        source.SetValue(3);
        source.SetValue(10);

        Assert.Equal(expected: 2, events); // activation + change to 10
        Assert.Equal(expected: 10, property.GetValue());
    }

    private sealed class CountingVisual : Visual
    {
        public Int32 MeasureCalls { get; private set; }
        public Int32 ArrangeCalls { get; private set; }

        public override SizeF OnMeasure(SizeF availableSize)
        {
            MeasureCalls++;
            return new SizeF(width: 5, height: 5);
        }

        public override RectangleF OnArrange(RectangleF finalRectangle)
        {
            ArrangeCalls++;
            return finalRectangle;
        }
    }
}
