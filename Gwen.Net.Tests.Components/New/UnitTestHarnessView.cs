using Gwen.Net.New;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Utilities;

namespace Gwen.Net.Tests.Components.New;

public static class UnitTestHarnessView
{
    public static Control Create(UnitTestHarness harness)
    {
        return new Border
        {
            HorizontalAlignment = { Value = HorizontalAlignment.Stretch },
            VerticalAlignment = { Value = VerticalAlignment.Stretch },
            Margin = { Value = new ThicknessF(5)},
            Padding = {  Value = new ThicknessF(5) },

            Child = new Border
            {
                BorderThickness = { Value = new ThicknessF(3)},
                MinimumWidth = { Value = 500f },
                MinimumHeight = { Value = 250f },
                HorizontalAlignment = { Value = HorizontalAlignment.Center },
                VerticalAlignment = { Value = VerticalAlignment.Center },
            }
        };
    }
}
