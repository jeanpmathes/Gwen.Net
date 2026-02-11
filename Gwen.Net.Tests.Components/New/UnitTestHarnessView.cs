using Gwen.Net.New.Controls;

namespace Gwen.Net.Tests.Components.New;

public static class UnitTestHarnessView
{
    public static Control Create(UnitTestHarness harness)
    {
        return new Border
        {
            MinimumWidth = { Value = 500f },
            MinimumHeight = { Value = 500f },

            Child = new Border
            {
                MinimumWidth = { Value = 500f },
                MinimumHeight = { Value = 250f },
            }
        };
    }
}
