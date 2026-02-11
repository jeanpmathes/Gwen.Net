using Gwen.Net.New.Controls;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.Tests.Unit.New.Visuals;

namespace Gwen.Net.Tests.Unit.New.Controls;

public sealed class MockControl : Control<MockControl>
{
    protected override ControlTemplate<MockControl> CreateDefaultTemplate()
    {
        return ControlTemplate.Create<MockControl>(_ => new MockVisual());
    }
}
