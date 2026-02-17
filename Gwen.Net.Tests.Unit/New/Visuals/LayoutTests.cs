using Gwen.Net.New.Visuals;

namespace Gwen.Net.Tests.Unit.New.Visuals;

public class LayoutTests() : VisualTestBase<Layout>(() => new MockVisual())
{
    private class MockVisual : Layout;
}
