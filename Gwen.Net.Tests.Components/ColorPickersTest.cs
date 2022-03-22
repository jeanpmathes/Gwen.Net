using System;
using Gwen.Net.Control;
using Gwen.Net.Control.Layout;

namespace Gwen.Net.Tests.Components
{
    [UnitTest(Category = "Non-standard", Order = 501)]
    public class ColorPickersTest : GUnit
    {
        public ColorPickersTest(ControlBase parent)
            : base(parent)
        {
            /* RGB Picker */
            {
                ColorPicker rgbPicker = new(this);
                rgbPicker.Dock = Dock.Top;
                rgbPicker.ColorChanged += ColorChanged;
            }

            /* HSVColorPicker */
            {
                HSVColorPicker hsvPicker = new(this);
                hsvPicker.Dock = Dock.Fill;
                hsvPicker.HorizontalAlignment = HorizontalAlignment.Left;
                hsvPicker.VerticalAlignment = VerticalAlignment.Top;
                hsvPicker.ColorChanged += ColorChanged;
            }

            /* HSVColorPicker in Window */
            {
                WindowTest window = new(base.GetCanvas());
                window.Size = new Size(width: 300, height: 200);
                window.Collapse();
                DockLayout layout = new(window);

                HSVColorPicker hsvPicker = new(layout);
                hsvPicker.Margin = Margin.Two;
                hsvPicker.Dock = Dock.Fill;
                hsvPicker.ColorChanged += ColorChanged;

                Button OpenWindow = new(this);
                OpenWindow.Dock = Dock.Bottom;
                OpenWindow.HorizontalAlignment = HorizontalAlignment.Left;
                OpenWindow.Text = "Open Window";

                OpenWindow.Clicked += delegate { window.Show(); };
            }
        }

        private void ColorChanged(ControlBase control, EventArgs args)
        {
            var picker = control as IColorPicker;
            Color c = picker.SelectedColor;
            var hsv = c.ToHSV();

            string text = string.Format(
                "Color changed: RGB: {0:X2}{1:X2}{2:X2} HSV: {3:F1} {4:F2} {5:F2}",
                c.R,
                c.G,
                c.B,
                hsv.H,
                hsv.S,
                hsv.V);

            UnitPrint(text);
        }
    }
}
