using Gwen.Net.New.Bindings;

namespace Gwen.Net.Tests.Unit.New.Bindings;

public class SlotTests
{
    [Fact]
    public void SetValue_WithDifferentValue_UpdatesValueAndRaisesValueChanged()
    {
        var slot = new Slot<Int32>(5);
        var events = 0;
        slot.ValueChanged += (_, _) => events++;

        slot.SetValue(9);

        Assert.Equal(expected: 9, slot.GetValue());
        Assert.Equal(expected: 1, events);
    }

    [Fact]
    public void SetValue_WithEqualValue_DoesNotRaiseValueChanged()
    {
        var slot = new Slot<String>("abc");
        var events = 0;
        slot.ValueChanged += (_, _) => events++;

        slot.SetValue("abc");

        Assert.Equal("abc", slot.GetValue());
        Assert.Equal(expected: 0, events);
    }
}
