using System;
using System.Globalization;
using Gwen.Net.Control.Internal;
using Gwen.Net.Control.Layout;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Numeric up/down.
    /// </summary>
    public class NumericUpDown : TextBoxNumeric
    {
        private readonly UpDownButtonDownKind downButton;

        private readonly Splitter splitter;
        private readonly UpDownButtonUpKind upButton;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NumericUpDown" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public NumericUpDown(ControlBase parent)
            : base(parent)
        {
            splitter = new Splitter(this);
            splitter.Dock = Dock.Right;

            upButton = new UpDownButtonUpKind(splitter);
            upButton.Clicked += OnButtonUp;
            upButton.IsTabable = false;
            splitter.SetPanel(panelIndex: 0, upButton);

            downButton = new UpDownButtonDownKind(splitter);
            downButton.Clicked += OnButtonDown;
            downButton.IsTabable = false;
            downButton.Padding = new Padding(left: 0, top: 1, right: 1, bottom: 0);
            splitter.SetPanel(panelIndex: 1, downButton);

            Max = 100f;
            Min = 0f;
            value = 0f;
            Step = 1f;

            Text = "0";
        }

        /// <summary>
        ///     Minimum value.
        /// </summary>
        public Single Min { get; set; }

        /// <summary>
        ///     Maximum value.
        /// </summary>
        public Single Max { get; set; }

        private Single step;
        
        public Single Step
        {
            get => step;
            set
            {
                DecimalPlaces = DetermineDecimalPlaces(value);
                step = value;
            } 
        }

        /// <summary>
        ///     Numeric value of the control.
        /// </summary>
        public override Single Value
        {
            get => base.Value;
            set
            {
                if (value < Min)
                {
                    value = Min;
                }

                if (value > Max)
                {
                    value = Max;
                }

                if (value == this.value)
                {
                    return;
                }

                base.Value = value;
            }
        }

        /// <summary>
        ///     Invoked when the value has been changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> ValueChanged;

        /// <summary>
        ///     Handler for Up Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyUp(Boolean down)
        {
            if (down)
            {
                OnButtonUp(control: null, EventArgs.Empty);
            }

            return true;
        }

        /// <summary>
        ///     Handler for Down Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyDown(Boolean down)
        {
            if (down)
            {
                OnButtonDown(control: null, new ClickedEventArgs(x: 0, y: 0, down: true));
            }

            return true;
        }

        /// <summary>
        ///     Handler for the button up event.
        /// </summary>
        /// <param name="control">Event source.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnButtonUp(ControlBase control, EventArgs args)
        {
            Value = value + Step;
        }

        /// <summary>
        ///     Handler for the button down event.
        /// </summary>
        /// <param name="control">Event source.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnButtonDown(ControlBase control, ClickedEventArgs args)
        {
            Value = value - Step;
        }

        /// <summary>
        ///     Determines whether the text can be assigned to the control.
        /// </summary>
        /// <param name="str">Text to evaluate.</param>
        /// <returns>True if the text is allowed.</returns>
        protected override Boolean IsTextAllowed(String str)
        {
            Single d;

            if (!Single.TryParse(str, out d))
            {
                return false;
            }

            if (d < Min)
            {
                return false;
            }

            if (d > Max)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Handler for the text changed event.
        /// </summary>
        protected override void OnTextChanged()
        {
            base.OnTextChanged();

            if (ValueChanged != null)
            {
                ValueChanged.Invoke(this, EventArgs.Empty);
            }
        }

        public override void SetValue(Single newValue, Boolean doEvents = true)
        {
            if (newValue < Min)
            {
                newValue = Min;
            }

            if (newValue > Max)
            {
                newValue = Max;
            }

            if (newValue == value)
            {
                return;
            }

            base.SetValue(newValue, doEvents);
        }
    }
}
