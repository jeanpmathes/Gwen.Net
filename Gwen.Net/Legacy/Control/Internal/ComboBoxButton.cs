using Gwen.Net.Legacy.Skin;

namespace Gwen.Net.Legacy.Control.Internal
{
    /// <summary>
    ///     Editable combobox button.
    /// </summary>
    internal class ComboBoxButton : ButtonBase
    {
        private readonly EditableComboBox comboBox;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ComboBoxButton" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        /// <param name="comboBox">Editable combobox that owns this button.</param>
        public ComboBoxButton(ControlBase parent, EditableComboBox comboBox)
            : base(parent)
        {
            Width = BaseUnit;

            this.comboBox = comboBox;
        }

        protected override void AdaptToScaleChange()
        {
            Width = BaseUnit;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawComboBoxArrow(this, IsHovered, IsDepressed, comboBox.IsOpen, comboBox.IsDisabled);
        }
    }
}
