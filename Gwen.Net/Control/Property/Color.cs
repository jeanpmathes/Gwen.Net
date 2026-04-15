using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Input;

namespace Gwen.Net.Control.Property
{
    /// <summary>
    ///     Color property.
    /// </summary>
    public class ColorProperty : Text
    {
        protected readonly ColorButton button;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColorProperty" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ColorProperty(ControlBase parent) : base(parent)
        {
            button = new ColorButton(textBox);
            button.Dock = Dock.Right;
            button.Width = 20;
            button.Margin = new Margin(left: 1, top: 1, right: 1, bottom: 2);
            button.Clicked += OnButtonPressed;
        }

        /// <summary>
        ///     Property value.
        /// </summary>
        public override String Value
        {
            get => textBox.Text;
            set => base.Value = value;
        }

        /// <summary>
        ///     Indicates whether the property value is being edited.
        /// </summary>
        public override Boolean IsEditing => textBox == InputHandler.KeyboardFocus;

        /// <summary>
        ///     Color-select button press handler.
        /// </summary>
        /// <param name="control">Event source.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnButtonPressed(ControlBase control, EventArgs args)
        {
            Canvas canvas = GetCanvas();

            canvas.CloseMenus();

            Popup popup = new(canvas);
            popup.DeleteOnClose = true;
            popup.IsHidden = false;
            popup.BringToFront();

            HSVColorPicker picker = new(popup);
            picker.SetColor(GetColorFromText(), onlyHue: false, reset: true);
            picker.ColorChanged += OnColorChanged;

            Point p = button.LocalPosToCanvas(Point.Zero);

            popup.DoMeasure(canvas.ActualSize);

            popup.DoArrange(
                new Rectangle(
                    p.X + button.ActualWidth - popup.MeasuredSize.Width,
                    p.Y + ActualHeight,
                    popup.MeasuredSize.Width,
                    popup.MeasuredSize.Height));

            popup.Open(new Point(p.X + button.ActualWidth - popup.MeasuredSize.Width, p.Y + ActualHeight));
        }

        /// <summary>
        ///     Color changed handler.
        /// </summary>
        /// <param name="control">Event source.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnColorChanged(ControlBase control, EventArgs args)
        {
            var picker = control as HSVColorPicker;
            SetTextFromColor(picker.SelectedColor);
            DoChanged();
        }

        /// <summary>
        ///     Sets the property value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <param name="fireEvents">Determines whether to fire "value changed" event.</param>
        public override void SetValue(String value, Boolean fireEvents = false)
        {
            textBox.SetText(value, fireEvents);
        }

        private void SetTextFromColor(Color color)
        {
            textBox.Text = $"{color.R} {color.G} {color.B}";
        }

        private Color GetColorFromText()
        {
            String[] split = textBox.Text.Split(separator: ' ');

            Byte red = 0;
            Byte green = 0;
            Byte blue = 0;
            Byte alpha = 255;

            if (split.Length > 0 && split[0].Length > 0)
            {
                Byte.TryParse(split[0], out red);
            }

            if (split.Length > 1 && split[1].Length > 0)
            {
                Byte.TryParse(split[1], out green);
            }

            if (split.Length > 2 && split[2].Length > 0)
            {
                Byte.TryParse(split[2], out blue);
            }

            return new Color(alpha, red, green, blue);
        }

        protected override void DoChanged()
        {
            base.DoChanged();
            button.Color = GetColorFromText();
        }
    }
}
