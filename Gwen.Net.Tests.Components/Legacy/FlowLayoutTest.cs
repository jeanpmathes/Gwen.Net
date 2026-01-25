using System;
using Gwen.Net.Legacy;
using Gwen.Net.Legacy.Control;
using Gwen.Net.Legacy.Control.Layout;

namespace Gwen.Net.Tests.Components.Legacy
{
    [UnitTest(Category = "Layout", Order = 401)]
    public class FlowLayoutTest : GUnit
    {
        public FlowLayoutTest(ControlBase parent)
            : base(parent)
        {
            ControlBase layout = new DockLayout(this);

            FlowLayout flowLayout = new(layout);
            flowLayout.Width = 200;
            flowLayout.Padding = Padding.Five;
            flowLayout.Dock = Dock.Fill;
            flowLayout.DrawDebugOutlines = true;

            {
                Button button;
                var buttonNum = 1;
                const Int32 buttonCount = 10;

                for (var n = 0; n < buttonCount; n++)
                {
                    button = new Button(flowLayout);
                    button.VerticalAlignment = VerticalAlignment.Top;
                    button.HorizontalAlignment = HorizontalAlignment.Left;
                    button.Margin = Margin.Five;
                    button.Padding = Padding.Five;
                    button.ShouldDrawBackground = false;
                    button.Text = $"Button {buttonNum++}";
                    button.SetImage("Legacy/test16.png", ImageAlign.Above);
                }
            }

            HorizontalSlider flowLayoutWidth = new(layout);
            flowLayoutWidth.Margin = Margin.Five;
            flowLayoutWidth.Width = 500;
            flowLayoutWidth.Dock = Dock.Top;
            flowLayoutWidth.Min = 50;
            flowLayoutWidth.Max = 500;
            flowLayoutWidth.Value = flowLayout.Width;
            flowLayoutWidth.ValueChanged += (_, _) => { flowLayout.Width = (Int32) flowLayoutWidth.Value; };
        }
    }
}
