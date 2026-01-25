using System;
using Gwen.Net.Legacy.Input;
using Gwen.Net.Legacy.Skin;

namespace Gwen.Net.Legacy.Control
{
    /// <summary>
    ///     RadioButton with label.
    /// </summary>
    public class LabeledRadioButton : ControlBase
    {
        private readonly Label label;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LabeledRadioButton" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public LabeledRadioButton(ControlBase parent)
            : base(parent)
        {
            MouseInputEnabled = true;

            RadioButton = new RadioButton(this);
            RadioButton.IsTabable = false;
            RadioButton.KeyboardInputEnabled = false;

            label = new Label(this);
            label.Alignment = Alignment.CenterV | Alignment.Left;
            label.Text = "Radio Button";
            label.Clicked += (_, _) => RadioButton.Press();
            label.IsTabable = false;
            label.KeyboardInputEnabled = false;
        }

        /// <summary>
        ///     Label text.
        /// </summary>
        public String Text
        {
            get => label.Text;
            set => label.Text = value;
        }

        // todo: would be nice to remove that
        internal RadioButton RadioButton { get; }

        /// <summary>
        ///     Invoked when the radiobutton has been checked.
        /// </summary>
        public event GwenEventHandler<EventArgs> Checked
        {
            add => RadioButton.Checked += value;
            remove => RadioButton.Checked -= value;
        }

        protected override Size Measure(Size availableSize)
        {
            Size labelSize = label.DoMeasure(availableSize);
            Size radioButtonSize = RadioButton.DoMeasure(availableSize);

            return new Size(
                labelSize.Width + 4 + radioButtonSize.Width,
                Math.Max(labelSize.Height, radioButtonSize.Height));
        }

        protected override Size Arrange(Size finalSize)
        {
            if (RadioButton.MeasuredSize.Height > label.MeasuredSize.Height)
            {
                RadioButton.DoArrange(
                    new Rectangle(x: 0, y: 0, RadioButton.MeasuredSize.Width, RadioButton.MeasuredSize.Height));

                label.DoArrange(
                    new Rectangle(
                        RadioButton.MeasuredSize.Width + 4,
                        (RadioButton.MeasuredSize.Height - label.MeasuredSize.Height) / 2,
                        label.MeasuredSize.Width,
                        label.MeasuredSize.Height));
            }
            else
            {
                RadioButton.DoArrange(
                    new Rectangle(
                        x: 0,
                        (label.MeasuredSize.Height - RadioButton.MeasuredSize.Height) / 2,
                        RadioButton.MeasuredSize.Width,
                        RadioButton.MeasuredSize.Height));

                label.DoArrange(
                    new Rectangle(
                        RadioButton.MeasuredSize.Width + 4,
                        y: 0,
                        label.MeasuredSize.Width,
                        label.MeasuredSize.Height));
            }

            return finalSize;
        }

        /// <summary>
        ///     Renders the focus overlay.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void RenderFocus(SkinBase currentSkin)
        {
            if (InputHandler.KeyboardFocus != this)
            {
                return;
            }

            if (!IsTabable)
            {
                return;
            }

            currentSkin.DrawKeyboardHighlight(this, RenderBounds, offset: 0);
        }

        /// <summary>
        ///     Handler for Space keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeySpace(Boolean down)
        {
            if (down)
            {
                RadioButton.IsChecked = !RadioButton.IsChecked;
            }

            return true;
        }

        /// <summary>
        ///     Selects the radio button.
        /// </summary>
        public virtual void Select()
        {
            RadioButton.IsChecked = true;
        }
    }
}
