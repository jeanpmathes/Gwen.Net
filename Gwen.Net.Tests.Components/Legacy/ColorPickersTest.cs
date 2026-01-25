using System;
using Gwen.Net.Legacy;
using Gwen.Net.Legacy.Control;
using Gwen.Net.Legacy.Control.Layout;

namespace Gwen.Net.Tests.Components.Legacy
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

                Button openWindow = new(this);
                openWindow.Dock = Dock.Bottom;
                openWindow.HorizontalAlignment = HorizontalAlignment.Left;
                openWindow.Text = "Open Window";

                openWindow.Clicked += delegate { window.Show(); };
            }
        }

        private void ColorChanged(ControlBase control, EventArgs args)
        {
            var picker = (IColorPicker) control;
            Color c = picker.SelectedColor;
            var hsv = c.ToHSV();

            var text = $"Color changed: RGB: {c.R:X2}{c.G:X2}{c.B:X2} HSV: {hsv.H:F1} {hsv.S:F2} {hsv.V:F2}";

            UnitPrint(text);
        }
    }
}
