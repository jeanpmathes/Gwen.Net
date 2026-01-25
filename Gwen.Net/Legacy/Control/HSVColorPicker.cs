using System;
using Gwen.Net.Legacy.Control.Internal;
using Gwen.Net.Legacy.Control.Layout;

namespace Gwen.Net.Legacy.Control
{
    /// <summary>
    ///     HSV color picker with "before" and "after" color boxes.
    /// </summary>
    public class HSVColorPicker : ControlBase, IColorPicker
    {
        private readonly ColorDisplay after;
        private readonly ColorDisplay before;
        private readonly NumericUpDown blue;
        private readonly ColorSlider colorSlider;
        private readonly NumericUpDown green;
        private readonly ColorLerpBox lerpBox;
        private readonly NumericUpDown red;

        private Boolean enableDefaultColor;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HSVColorPicker" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public HSVColorPicker(ControlBase parent)
            : base(parent)
        {
            MouseInputEnabled = true;

            Int32 baseSize = BaseUnit;

            lerpBox = new ColorLerpBox(this);
            lerpBox.Margin = Margin.Two;
            lerpBox.ColorChanged += ColorBoxChanged;
            lerpBox.Dock = Dock.Fill;

            ControlBase values = new VerticalLayout(this);
            values.Dock = Dock.Right;

            {
                after = new ColorDisplay(values);
                after.Size = new Size(baseSize * 5, baseSize * 2);

                before = new ColorDisplay(values);
                before.Margin = new Margin(left: 2, top: 0, right: 2, bottom: 2);
                before.Size = new Size(baseSize * 5, baseSize * 2);

                GridLayout grid = new(values);
                grid.Margin = new Margin(left: 2, top: 0, right: 2, bottom: 2);
                grid.SetColumnWidths(GridLayout.AutoSize, GridLayout.Fill);

                {
                    {
                        Label label = new(grid);
                        label.Text = "R: ";
                        label.Alignment = Alignment.Left | Alignment.CenterV;

                        red = new NumericUpDown(grid);
                        red.Min = 0;
                        red.Max = 255;
                        red.SelectAllOnFocus = true;
                        red.ValueChanged += NumericTyped;
                    }

                    {
                        Label label = new(grid);
                        label.Text = "G: ";
                        label.Alignment = Alignment.Left | Alignment.CenterV;

                        green = new NumericUpDown(grid);
                        green.Min = 0;
                        green.Max = 255;
                        green.SelectAllOnFocus = true;
                        green.ValueChanged += NumericTyped;
                    }

                    {
                        Label label = new(grid);
                        label.Text = "B: ";
                        label.Alignment = Alignment.Left | Alignment.CenterV;

                        blue = new NumericUpDown(grid);
                        blue.Min = 0;
                        blue.Max = 255;
                        blue.SelectAllOnFocus = true;
                        blue.ValueChanged += NumericTyped;
                    }
                }
            }

            colorSlider = new ColorSlider(this);
            colorSlider.Margin = Margin.Two;
            colorSlider.ColorChanged += ColorSliderChanged;
            colorSlider.Dock = Dock.Right;

            EnableDefaultColor = false;

            SetColor(DefaultColor);
        }

        /// <summary>
        ///     The "before" color.
        /// </summary>
        public Color DefaultColor
        {
            get => before.Color;
            set => before.Color = value;
        }

        /// <summary>
        ///     Show / hide default color box
        /// </summary>
        public Boolean EnableDefaultColor
        {
            get => enableDefaultColor;
            set
            {
                enableDefaultColor = value;
                UpdateChildControlVisibility();
            }
        }

        /// <summary>
        ///     Selected color.
        /// </summary>
        public Color SelectedColor => lerpBox.SelectedColor;

        protected override void AdaptToScaleChange()
        {
            Int32 baseSize = BaseUnit;

            after.Size = new Size(baseSize * 5, baseSize * 2);
            before.Size = new Size(baseSize * 5, baseSize * 2);
        }

        /// <summary>
        ///     Invoked when the selected color has changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> ColorChanged;

        private void NumericTyped(ControlBase control, EventArgs args)
        {
            var box = control as NumericUpDown;

            if (box == null)
            {
                return;
            }

            var value = (Int32) box.Value;

            if (value < 0)
            {
                value = 0;
            }

            if (value > 255)
            {
                value = 255;
            }

            Color newColor = SelectedColor;

            if (box == red)
            {
                newColor = new Color(SelectedColor.A, value, SelectedColor.G, SelectedColor.B);
            }
            else if (box == green)
            {
                newColor = new Color(SelectedColor.A, SelectedColor.R, value, SelectedColor.B);
            }
            else if (box == blue)
            {
                newColor = new Color(SelectedColor.A, SelectedColor.R, SelectedColor.G, value);
            }
            //else if (box.Name.Contains("Alpha"))
            //    newColor = Color.FromArgb(textValue, SelectedColor.R, SelectedColor.G, SelectedColor.B);

            colorSlider.SetColor(newColor, doEvents: false);
            lerpBox.SetColor(newColor, onlyHue: false, doEvents: false);
            after.Color = newColor;

            if (ColorChanged != null)
            {
                ColorChanged.Invoke(this, EventArgs.Empty);
            }
        }

        private void UpdateControls(Color color)
        {
            red.SetValue(color.R, doEvents: false);
            green.SetValue(color.G, doEvents: false);
            blue.SetValue(color.B, doEvents: false);
            after.Color = color;
        }

        /// <summary>
        ///     Sets the selected color.
        /// </summary>
        /// <param name="color">Color to set.</param>
        /// <param name="onlyHue">Determines whether only the hue should be set.</param>
        /// <param name="reset">Determines whether the "before" color should be set as well.</param>
        public void SetColor(Color color, Boolean onlyHue = false, Boolean reset = false)
        {
            UpdateControls(color);

            if (reset)
            {
                before.Color = color;
            }

            colorSlider.SetColor(color, doEvents: false);
            lerpBox.SetColor(color, onlyHue, doEvents: false);
            after.Color = color;

            if (ColorChanged != null)
            {
                ColorChanged.Invoke(this, EventArgs.Empty);
            }
        }

        private void ColorBoxChanged(ControlBase control, EventArgs args)
        {
            UpdateControls(SelectedColor);
            //Invalidate();

            if (ColorChanged != null)
            {
                ColorChanged.Invoke(this, EventArgs.Empty);
            }
        }

        private void ColorSliderChanged(ControlBase control, EventArgs args)
        {
            lerpBox.SetColor(colorSlider.SelectedColor, onlyHue: true, doEvents: false);
            UpdateControls(SelectedColor);
            //Invalidate();

            if (ColorChanged != null)
            {
                ColorChanged.Invoke(this, EventArgs.Empty);
            }
        }

        private void UpdateChildControlVisibility()
        {
            if (enableDefaultColor)
            {
                after.Margin = new Margin(left: 2, top: 2, right: 2, bottom: 0);
                before.Margin = new Margin(left: 2, top: 0, right: 2, bottom: 2);
                after.Height = BaseUnit * 2;
                before.Show();
            }
            else
            {
                after.Margin = Margin.Two;
                before.Collapse();
                after.Height = BaseUnit * 4;
            }
        }
    }
}
