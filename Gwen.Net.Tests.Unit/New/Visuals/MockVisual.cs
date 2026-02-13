using Gwen.Net.New.Visuals;

namespace Gwen.Net.Tests.Unit.New.Visuals;

public class MockVisual : Visual
{
    public void SetChildVisual(Visual? child) => SetChild(child);
}
