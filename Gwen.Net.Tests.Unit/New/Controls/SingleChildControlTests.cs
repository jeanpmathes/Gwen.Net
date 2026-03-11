using Gwen.Net.New.Controls.Internals;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.Tests.Unit.New.Visuals;

namespace Gwen.Net.Tests.Unit.New.Controls;

public class SingleChildControlTests
{
    [Fact]
    public void Child_IsNull_ByDefault()
    {
        MockControl control = new();

        Assert.Null(control.Child);
        Assert.Empty(control.Children);
    }

    [Fact]
    public void Child_ReplacesPreviousChild()
    {
        MockControl control = new();
        MockControl firstChild = new();
        MockControl secondChild = new();

        control.Child = firstChild;
        Assert.Equal(firstChild, control.Child);
        Assert.Single(control.Children);

        control.Child = secondChild;
        Assert.Equal(secondChild, control.Child);
        Assert.Single(control.Children);
    }

    private class MockControl : SingleChildControl<MockControl>
    {
        protected override ControlTemplate<MockControl> CreateDefaultTemplate()
        {
            return ControlTemplate.Create<MockControl>(_ => new MockVisual());
        }
    }
}
