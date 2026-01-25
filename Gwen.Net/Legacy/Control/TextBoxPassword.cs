using System;

namespace Gwen.Net.Legacy.Control
{
    /// <summary>
    ///     Text box with masked text.
    /// </summary>
    /// <remarks>
    ///     This class doesn't prevent programmatic access to the text in any way.
    /// </remarks>
    public class TextBoxPassword : TextBox
    {
        private String mask;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextBoxPassword" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public TextBoxPassword(ControlBase parent)
            : base(parent)
        {
            MaskCharacter = '*';
        }

        /// <summary>
        ///     Character used in place of actual characters for display.
        /// </summary>
        public Char MaskCharacter { get; set; }

        /// <summary>
        ///     Handler for text changed event.
        /// </summary>
        protected override void OnTextChanged()
        {
            mask = new String(MaskCharacter, Text.Length);
            TextOverride = mask;
            base.OnTextChanged();
        }
    }
}
