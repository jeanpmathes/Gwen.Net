using Gwen.Net.New;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Texts;
using Gwen.Net.New.Utilities;

namespace Gwen.Net.Tests.Components.New;

public static class UnitTestHarnessView
{
    public static Control Create(UnitTestHarness harness)
    {
        return new Border
        {
            HorizontalAlignment = {Value = HorizontalAlignment.Stretch},
            VerticalAlignment = {Value = VerticalAlignment.Stretch},
            Margin = {Value = new ThicknessF(5)},
            Padding = {Value = new ThicknessF(5)},

            Child = new LinearLayout
            {
                Children =
                {
                    new Border
                    {
                        BorderThickness = {Value = new ThicknessF(3)},
                        MinimumWidth = {Value = 500f},
                        MinimumHeight = {Value = 250f},
                        HorizontalAlignment = {Value = HorizontalAlignment.Center},
                        VerticalAlignment = {Value = VerticalAlignment.Center},
                    },
                    new Text
                    {
                        Content = {Value = "Hello, World!"}
                    },
                    new Text
                    {
                        MaximumWidth = { Value = 100f},
                        TextTrimming = { Value = TextTrimming.CharacterEllipsis},
                        
                        Content = {Value = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec a diam lectus. Sed sit amet ipsum mauris. Maecenas congue ligula ac quam viverra nec consectetur ante hendrerit. Donec et mollis dolor. Praesent et diam eget libero egestas mattis sit amet vitae augue. Nam tincidunt congue enim, ut porta lorem lacinia consectetur. Donec ut libero sed arcu vehicula ultricies a non tortor. Lorem ipsum dolor sit amet, consectetur adipiscing elit."}
                    }
                }
            }
        };
    }
}
