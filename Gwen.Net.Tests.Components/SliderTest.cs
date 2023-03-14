using System;
using Gwen.Net.Control;
using Gwen.Net.Control.Internal;
using Gwen.Net.Control.Layout;

namespace Gwen.Net.Tests.Components
{
    [UnitTest(Category = "Standard", Order = 207)]
    public class SliderTest : GUnit
    {
        public SliderTest(ControlBase parent)
            : base(parent)
        {
            HorizontalLayout hlayout = new(this);

            VerticalLayout vlayout = new(hlayout);

            {
                HorizontalSlider slider = new(vlayout);
                slider.Margin = Margin.Ten;
                slider.Width = 150;
                slider.SetRange(newMin: 0, newMax: 100);
                slider.Value = 25;
                slider.ValueChanged += SliderMoved;
            }

            {
                HorizontalSlider slider = new(vlayout);
                slider.Margin = Margin.Ten;
                slider.Width = 150;
                slider.SetRange(newMin: 0, newMax: 100);
                slider.Value = 20;
                slider.NotchCount = 10;
                slider.SnapToNotches = true;
                slider.ValueChanged += SliderMoved;
            }

            {
                VerticalSlider slider = new(hlayout);
                slider.Margin = Margin.Ten;
                slider.Height = 200;
                slider.SetRange(newMin: 0, newMax: 100);
                slider.Value = 25;
                slider.ValueChanged += SliderMoved;
            }

            {
                VerticalSlider slider = new(hlayout);
                slider.Margin = Margin.Ten;
                slider.Height = 200;
                slider.SetRange(newMin: 0, newMax: 100);
                slider.Value = 20;
                slider.NotchCount = 10;
                slider.SnapToNotches = true;
                slider.ValueChanged += SliderMoved;
            }
        }

        private void SliderMoved(ControlBase control, EventArgs args)
        {
            var slider = control as Slider;
            UnitPrint($"Slider moved: ValueChanged: {slider.Value}");
        }
    }
}
