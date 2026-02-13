using Gwen.Net.New.Controls;
using Gwen.Net.New.Utilities;

namespace Gwen.Net.Tests.Components.New;

public static class UnitTestHarnessView
{
    public static Control Create(UnitTestHarness harness)
    {
        return new Border
        {
            MinimumWidth = { Value = 500f },
            MinimumHeight = { Value = 500f },
            Margin = { Value = new ThicknessF(5)},
            Padding = {  Value = new ThicknessF(5) },

            Child = new Border
            {
                MinimumWidth = { Value = 500f },
                MinimumHeight = { Value = 250f },
            }
        };
    }
}
