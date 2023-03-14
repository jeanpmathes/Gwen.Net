using System.Globalization;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Numeric text box - accepts only float numbers.
    /// </summary>
    public class TextBoxNumeric : TextBox
    {
        /// <summary>
        ///     Current numeric value.
        /// </summary>
        protected float value;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextBoxNumeric" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public TextBoxNumeric(ControlBase parent) : base(parent)
        {
            SetText("0", doEvents: false);
        }
        
        /// <summary>
        /// Sets the number of decimal places to display.
        /// </summary>
        public int DecimalPlaces { get; set; }

        /// <summary>
        ///     Current numerical value.
        /// </summary>
        public virtual float Value
        {
            get => value;
            set
            {
                this.value = value;
                Text = FormattedText;
            }
        }
        
        private string FormattedText => value.ToString($"F{DecimalPlaces}");

        protected virtual bool IsTextAllowed(string str)
        {
            if (str == "" || str == "-")
            {
                return true; // annoying if single - is not allowed
            }

            return float.TryParse(str, out _);
        }

        /// <summary>
        ///     Determines whether the control can insert text at a given cursor position.
        /// </summary>
        /// <param name="textToCheck">Text to check.</param>
        /// <param name="position">Cursor position.</param>
        /// <returns>True if allowed.</returns>
        protected override bool IsTextAllowed(string textToCheck, int position)
        {
            string newText = Text.Insert(position, textToCheck);

            return IsTextAllowed(newText);
        }

        // text -> value
        /// <summary>
        ///     Handler for text changed event.
        /// </summary>
        protected override void OnTextChanged()
        {
            if (string.IsNullOrEmpty(Text) || Text == "-")
            {
                value = 0;
            }
            else
            {
                value = float.Parse(Text);
            }

            base.OnTextChanged();
        }

        /// <summary>
        ///     Sets the control text.
        /// </summary>
        /// <param name="str">Text to set.</param>
        /// <param name="doEvents">Determines whether to invoke "text changed" event.</param>
        public override void SetText(string str, bool doEvents = true)
        {
            if (IsTextAllowed(str))
            {
                base.SetText(str, doEvents);
            }
        }

        /// <summary>
        ///     Sets the control value.
        /// </summary>
        /// <param name="newValue">Value to set.</param>
        /// <param name="doEvents">Determines whether to invoke "text changed" event.</param>
        public virtual void SetValue(float newValue, bool doEvents = true)
        {
            value = newValue;
            base.SetText(FormattedText, doEvents);
        }
    }
}
