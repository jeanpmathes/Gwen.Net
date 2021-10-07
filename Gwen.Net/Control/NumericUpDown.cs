using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Control.Layout;
using Gwen.Net.Xml;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Numeric up/down.
    /// </summary>
    [XmlControl]
    public class NumericUpDown : TextBoxNumeric
    {
        private readonly UpDownButton_Down m_Down;

        private readonly Splitter m_Splitter;
        private readonly UpDownButton_Up m_Up;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NumericUpDown" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public NumericUpDown(ControlBase parent)
            : base(parent)
        {
            m_Splitter = new Splitter(this);
            m_Splitter.Dock = Dock.Right;

            m_Up = new UpDownButton_Up(m_Splitter);
            m_Up.Clicked += OnButtonUp;
            m_Up.IsTabable = false;
            m_Splitter.SetPanel(panelIndex: 0, m_Up);

            m_Down = new UpDownButton_Down(m_Splitter);
            m_Down.Clicked += OnButtonDown;
            m_Down.IsTabable = false;
            m_Down.Padding = new Padding(left: 0, top: 1, right: 1, bottom: 0);
            m_Splitter.SetPanel(panelIndex: 1, m_Down);

            Max = 100f;
            Min = 0f;
            m_Value = 0f;
            Step = 1f;

            Text = "0";
        }

        /// <summary>
        ///     Minimum value.
        /// </summary>
        [XmlProperty] public float Min { get; set; }

        /// <summary>
        ///     Maximum value.
        /// </summary>
        [XmlProperty] public float Max { get; set; }

        [XmlProperty] public float Step { get; set; }

        /// <summary>
        ///     Numeric value of the control.
        /// </summary>
        [XmlProperty] public override float Value
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

                if (value == m_Value)
                {
                    return;
                }

                base.Value = value;
            }
        }

        /// <summary>
        ///     Invoked when the value has been changed.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> ValueChanged;

        /// <summary>
        ///     Handler for Up Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnKeyUp(bool down)
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
        protected override bool OnKeyDown(bool down)
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
        protected virtual void OnButtonUp(ControlBase control, EventArgs args)
        {
            Value = m_Value + Step;
        }

        /// <summary>
        ///     Handler for the button down event.
        /// </summary>
        /// <param name="control">Event source.</param>
        protected virtual void OnButtonDown(ControlBase control, ClickedEventArgs args)
        {
            Value = m_Value - Step;
        }

        /// <summary>
        ///     Determines whether the text can be assighed to the control.
        /// </summary>
        /// <param name="str">Text to evaluate.</param>
        /// <returns>True if the text is allowed.</returns>
        protected override bool IsTextAllowed(string str)
        {
            float d;

            if (!float.TryParse(str, out d))
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

        public override void SetValue(float value, bool doEvents = true)
        {
            if (value < Min)
            {
                value = Min;
            }

            if (value > Max)
            {
                value = Max;
            }

            if (value == m_Value)
            {
                return;
            }

            base.SetValue(value, doEvents);
        }
    }
}