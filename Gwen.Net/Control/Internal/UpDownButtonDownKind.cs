using Gwen.Net.Skin;

namespace Gwen.Net.Control.Internal
{
    /// <summary>
    ///     Numeric down arrow.
    /// </summary>
    internal class UpDownButtonDownKind : ButtonBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UpDownButtonDownKind" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public UpDownButtonDownKind(ControlBase parent)
            : base(parent)
        {
            Width = BaseUnit / 2;
        }

        protected override void AdaptToScaleChange()
        {
            Width = BaseUnit / 2;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawNumericUpDownButton(this, IsDepressed, up: false);
        }
    }
}
