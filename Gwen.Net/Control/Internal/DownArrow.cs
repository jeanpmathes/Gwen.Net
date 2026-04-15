using Gwen.Net.Skin;

namespace Gwen.Net.Control.Internal
{
    /// <summary>
    ///     ComboBox arrow.
    /// </summary>
    public class DownArrow : ControlBase
    {
        private readonly ComboBox comboBox;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DownArrow" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public DownArrow(ComboBox parent)
            : base(parent)
        {
            MouseInputEnabled = false;

            Size = new Size(BaseUnit);

            comboBox = parent;
        }

        protected override void AdaptToScaleChange()
        {
            Size = new Size(BaseUnit);
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawComboBoxArrow(
                this,
                comboBox.IsHovered,
                comboBox.IsDepressed,
                comboBox.IsOpen,
                comboBox.IsDisabled);
        }
    }
}
