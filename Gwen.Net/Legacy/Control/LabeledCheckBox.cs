using System;

namespace Gwen.Net.Legacy.Control
{
    /// <summary>
    ///     CheckBox with label.
    /// </summary>
    public class LabeledCheckBox : ControlBase
    {
        private readonly CheckBox checkBox;
        private readonly Label label;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LabeledCheckBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public LabeledCheckBox(ControlBase parent)
            : base(parent)
        {
            checkBox = new CheckBox(this);
            checkBox.IsTabable = false;
            checkBox.CheckChanged += OnCheckChanged;

            label = new Label(this);
            label.Clicked += (_, _) => checkBox.Press();
            label.IsTabable = false;

            IsTabable = false;
        }

        /// <summary>
        ///     Indicates whether the control is checked.
        /// </summary>
        public Boolean IsChecked
        {
            get => checkBox.IsChecked;
            set => checkBox.IsChecked = value;
        }

        /// <summary>
        ///     Label text.
        /// </summary>
        public String Text
        {
            get => label.Text;
            set => label.Text = value;
        }

        /// <summary>
        ///     Invoked when the control has been checked.
        /// </summary>
        public event GwenEventHandler<EventArgs> Checked;

        /// <summary>
        ///     Invoked when the control has been unchecked.
        /// </summary>
        public event GwenEventHandler<EventArgs> UnChecked;

        /// <summary>
        ///     Invoked when the control's check has been changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> CheckChanged;

        protected override Size Measure(Size availableSize)
        {
            Size labelSize = label.DoMeasure(availableSize);
            Size radioButtonSize = checkBox.DoMeasure(availableSize);

            return new Size(
                labelSize.Width + 4 + radioButtonSize.Width,
                Math.Max(labelSize.Height, radioButtonSize.Height));
        }

        protected override Size Arrange(Size finalSize)
        {
            if (checkBox.MeasuredSize.Height > label.MeasuredSize.Height)
            {
                checkBox.DoArrange(
                    new Rectangle(x: 0, y: 0, checkBox.MeasuredSize.Width, checkBox.MeasuredSize.Height));

                label.DoArrange(
                    new Rectangle(
                        checkBox.MeasuredSize.Width + 4,
                        (checkBox.MeasuredSize.Height - label.MeasuredSize.Height) / 2,
                        label.MeasuredSize.Width,
                        label.MeasuredSize.Height));
            }
            else
            {
                checkBox.DoArrange(
                    new Rectangle(
                        x: 0,
                        (label.MeasuredSize.Height - checkBox.MeasuredSize.Height) / 2,
                        checkBox.MeasuredSize.Width,
                        checkBox.MeasuredSize.Height));

                label.DoArrange(
                    new Rectangle(
                        checkBox.MeasuredSize.Width + 4,
                        y: 0,
                        label.MeasuredSize.Width,
                        label.MeasuredSize.Height));
            }

            return MeasuredSize;
        }

        /// <summary>
        ///     Handler for CheckChanged event.
        /// </summary>
        protected virtual void OnCheckChanged(ControlBase control, EventArgs args)
        {
            if (checkBox.IsChecked)
            {
                if (Checked != null)
                {
                    Checked.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                if (UnChecked != null)
                {
                    UnChecked.Invoke(this, EventArgs.Empty);
                }
            }

            if (CheckChanged != null)
            {
                CheckChanged.Invoke(this, EventArgs.Empty);
            }
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
            base.OnKeySpace(down);

            if (!down)
            {
                checkBox.IsChecked = !checkBox.IsChecked;
            }

            return true;
        }
    }
}
