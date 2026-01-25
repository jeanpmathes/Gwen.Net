using System;

namespace Gwen.Net.Legacy.Control
{
    /// <summary>
    ///     Numeric text box - accepts only float numbers.
    /// </summary>
    public class TextBoxNumeric : TextBox
    {
        /// <summary>
        ///     Current numeric value.
        /// </summary>
        protected Single value;

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
        public Int32 DecimalPlaces { get; set; }

        public static Int32 DetermineDecimalPlaces(Single value)
        {
            var decimalPlaces = 0;
            while (Math.Abs(value - Math.Round(value, decimalPlaces)) > 0.00001f)
            {
                decimalPlaces++;
            }
                
            return decimalPlaces;
        }

        /// <summary>
        ///     Current numerical value.
        /// </summary>
        public virtual Single Value
        {
            get => value;
            set
            {
                this.value = value;
                DecimalPlaces = DetermineDecimalPlaces(value);
                Text = FormattedText;
            }
        }
        
        private String FormattedText => value.ToString($"F{DecimalPlaces}");

        protected virtual Boolean IsTextAllowed(String str)
        {
            if (str == "" || str == "-")
            {
                return true; // annoying if single - is not allowed
            }

            return Single.TryParse(str, out _);
        }

        /// <summary>
        ///     Determines whether the control can insert text at a given cursor position.
        /// </summary>
        /// <param name="textToCheck">Text to check.</param>
        /// <param name="position">Cursor position.</param>
        /// <returns>True if allowed.</returns>
        protected override Boolean IsTextAllowed(String textToCheck, Int32 position)
        {
            String newText = Text.Insert(position, textToCheck);

            return IsTextAllowed(newText);
        }

        // text -> value
        /// <summary>
        ///     Handler for text changed event.
        /// </summary>
        protected override void OnTextChanged()
        {
            if (String.IsNullOrEmpty(Text) || Text == "-")
            {
                value = 0;
            }
            else
            {
                value = Single.Parse(Text);
            }

            base.OnTextChanged();
        }

        /// <summary>
        ///     Sets the control text.
        /// </summary>
        /// <param name="str">Text to set.</param>
        /// <param name="doEvents">Determines whether to invoke "text changed" event.</param>
        public override void SetText(String str, Boolean doEvents = true)
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
        public virtual void SetValue(Single newValue, Boolean doEvents = true)
        {
            value = newValue;
            base.SetText(FormattedText, doEvents);
        }
    }
}
