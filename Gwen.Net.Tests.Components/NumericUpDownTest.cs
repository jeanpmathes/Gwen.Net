using System;
using Gwen.Net.Control;
using Gwen.Net.Control.Layout;

namespace Gwen.Net.Tests.Components
{
    [UnitTest(Category = "Standard", Order = 206)]
    public class NumericUpDownTest : GUnit
    {
        public NumericUpDownTest(ControlBase parent)
            : base(parent)
        {
            VerticalLayout layout = new(this);

            NumericUpDown ctrl = new(layout);
            ctrl.Margin = Margin.Five;
            ctrl.Width = 70;
            ctrl.Value = 50;
            ctrl.Max = 100;
            ctrl.Min = -100;
            ctrl.ValueChanged += OnValueChanged;

            ctrl = new NumericUpDown(layout);
            ctrl.Margin = Margin.Five;
            ctrl.Width = 70;
            ctrl.Value = 50;
            ctrl.Max = 100;
            ctrl.Min = -100;
            ctrl.Step = 5;
            ctrl.ValueChanged += OnValueChanged;

            ctrl = new NumericUpDown(layout);
            ctrl.Margin = Margin.Five;
            ctrl.Width = 70;
            ctrl.Value = 50;
            ctrl.Max = 100;
            ctrl.Min = -100;
            ctrl.Step = 0.1f;
            ctrl.ValueChanged += OnValueChanged;

            ctrl = new NumericUpDown(layout);
            ctrl.Margin = Margin.Five;
            ctrl.Width = 70;
            ctrl.Max = Single.MaxValue;
            ctrl.Min = 0;
            ctrl.Step = 1f;
            ctrl.ValueChanged += OnValueChanged;
        }

        private void OnValueChanged(ControlBase control, EventArgs args)
        {
            UnitPrint($"NumericUpDown: ValueChanged: {((NumericUpDown) control).Value}");
        }
    }
}
