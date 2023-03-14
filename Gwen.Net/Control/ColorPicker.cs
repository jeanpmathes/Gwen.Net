using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Control.Layout;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     RGBA color picker.
    /// </summary>
    public class ColorPicker : ControlBase, IColorPicker
    {
        private Color color;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColorPicker" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ColorPicker(ControlBase parent)
            : base(parent)
        {
            MouseInputEnabled = true;

            CreateControls();
            SelectedColor = new Color(a: 255, r: 50, g: 60, b: 70);
        }

        /// <summary>
        ///     Red value of the selected color.
        /// </summary>
        public int R
        {
            get => color.R;
            set => color = new Color(color.A, value, color.G, color.B);
        }

        /// <summary>
        ///     Green value of the selected color.
        /// </summary>
        public int G
        {
            get => color.G;
            set => color = new Color(color.A, color.R, value, color.B);
        }

        /// <summary>
        ///     Blue value of the selected color.
        /// </summary>
        public int B
        {
            get => color.B;
            set => color = new Color(color.A, color.R, color.G, value);
        }

        /// <summary>
        ///     Alpha value of the selected color.
        /// </summary>
        public int A
        {
            get => color.A;
            set => color = new Color(value, color.R, color.G, color.B);
        }

        /// <summary>
        ///     Determines whether the Alpha control is visible.
        /// </summary>
        public bool AlphaVisible
        {
            get
            {
                var gb = FindChildByName("Alphagroupbox", recursive: true) as GroupBox;

                return !gb.IsHidden;
            }
            set
            {
                var gb = FindChildByName("Alphagroupbox", recursive: true) as GroupBox;
                gb.IsHidden = !value;
                //Invalidate();
            }
        }

        /// <summary>
        ///     Selected color.
        /// </summary>
        public Color SelectedColor
        {
            get => color;
            set
            {
                color = value;
                UpdateControls();
            }
        }

        /// <summary>
        ///     Invoked when the selected color has been changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> ColorChanged;

        private void CreateControls()
        {
            VerticalLayout colorControlLayout = new(this);
            colorControlLayout.Dock = Dock.Fill;

            CreateColorControl(colorControlLayout, "Red");
            CreateColorControl(colorControlLayout, "Green");
            CreateColorControl(colorControlLayout, "Blue");
            CreateColorControl(colorControlLayout, "Alpha");

            GroupBox finalGroup = new(this);
            finalGroup.Dock = Dock.Right;
            finalGroup.Text = "Result";
            finalGroup.Name = "ResultGroupBox";

            DockLayout finalLayout = new(finalGroup);

            ColorDisplay disp = new(finalLayout);
            disp.Dock = Dock.Fill;
            disp.Name = "Result";
            disp.Width = Util.Ignore;
            disp.Height = Util.Ignore;
        }

        private void CreateColorControl(ControlBase parent, string name)
        {
            GroupBox colorGroup = new(parent);
            colorGroup.Text = name;
            colorGroup.Name = name + "groupbox";

            DockLayout layout = new(colorGroup);

            ColorDisplay disp = new(layout);
            disp.Height = Util.Ignore;
            disp.Dock = Dock.Left;
            disp.Name = name;

            TextBoxNumeric numeric = new(layout);
            numeric.Dock = Dock.Right;
            numeric.FitToText = "000";
            numeric.Name = name + "Box";
            numeric.SelectAllOnFocus = true;
            numeric.TextChanged += NumericTyped;

            HorizontalSlider slider = new(layout);
            slider.Dock = Dock.Fill;
            slider.VerticalAlignment = VerticalAlignment.Center;
            slider.SetRange(newMin: 0, newMax: 255);
            slider.Name = name + "Slider";
            slider.ValueChanged += SlidersMoved;
        }

        private void NumericTyped(ControlBase control, EventArgs args)
        {
            var box = control as TextBoxNumeric;

            if (null == box)
            {
                return;
            }

            if (box.Text == string.Empty)
            {
                return;
            }

            var textValue = (int) box.Value;

            if (textValue < 0)
            {
                textValue = 0;
            }

            if (textValue > 255)
            {
                textValue = 255;
            }

            if (box.Name.Contains("Red"))
            {
                R = textValue;
            }

            if (box.Name.Contains("Green"))
            {
                G = textValue;
            }

            if (box.Name.Contains("Blue"))
            {
                B = textValue;
            }

            if (box.Name.Contains("Alpha"))
            {
                A = textValue;
            }

            UpdateControls();
        }

        private void UpdateColorControls(string name, Color col, int sliderVal)
        {
            var disp = FindChildByName(name, recursive: true) as ColorDisplay;
            disp.Color = col;

            var slider = FindChildByName(name + "Slider", recursive: true) as HorizontalSlider;
            slider.Value = sliderVal;

            var box = FindChildByName(name + "Box", recursive: true) as TextBoxNumeric;
            box.Value = sliderVal;
        }

        private void UpdateControls()
        {
            //This is a little weird, but whatever for now
            UpdateColorControls("Red", new Color(a: 255, SelectedColor.R, g: 0, b: 0), SelectedColor.R);
            UpdateColorControls("Green", new Color(a: 255, r: 0, SelectedColor.G, b: 0), SelectedColor.G);
            UpdateColorControls("Blue", new Color(a: 255, r: 0, g: 0, SelectedColor.B), SelectedColor.B);
            UpdateColorControls("Alpha", new Color(SelectedColor.A, r: 255, g: 255, b: 255), SelectedColor.A);

            var disp = FindChildByName("Result", recursive: true) as ColorDisplay;
            disp.Color = SelectedColor;

            if (ColorChanged != null)
            {
                ColorChanged.Invoke(this, EventArgs.Empty);
            }
        }

        private void SlidersMoved(ControlBase control, EventArgs args)
        {
            var slider = control as HorizontalSlider;

            if (slider != null)
            {
                SetColorByName(GetColorFromName(slider.Name), (int) slider.Value);
            }

            UpdateControls();
        }

        private static string GetColorFromName(string name)
        {
            if (name.Contains("Red"))
            {
                return "Red";
            }

            if (name.Contains("Green"))
            {
                return "Green";
            }

            if (name.Contains("Blue"))
            {
                return "Blue";
            }

            if (name.Contains("Alpha"))
            {
                return "Alpha";
            }

            return string.Empty;
        }

        private void SetColorByName(string colorName, int colorValue)
        {
            if (colorName == "Red")
            {
                R = colorValue;
            }
            else if (colorName == "Green")
            {
                G = colorValue;
            }
            else if (colorName == "Blue")
            {
                B = colorValue;
            }
            else if (colorName == "Alpha")
            {
                A = colorValue;
            }
        }
    }
}
