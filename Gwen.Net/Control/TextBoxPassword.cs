using Gwen.Net.Xml;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Text box with masked text.
    /// </summary>
    /// <remarks>
    ///     This class doesn't prevent programatic access to the text in any way.
    /// </remarks>
    [XmlControl]
    public class TextBoxPassword : TextBox
    {
        private string m_Mask;

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
        [XmlProperty] public char MaskCharacter { get; set; }

        /// <summary>
        ///     Handler for text changed event.
        /// </summary>
        protected override void OnTextChanged()
        {
            m_Mask = new string(MaskCharacter, Text.Length);
            TextOverride = m_Mask;
            base.OnTextChanged();
        }
    }
}
