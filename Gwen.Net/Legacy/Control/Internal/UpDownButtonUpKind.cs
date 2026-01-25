using Gwen.Net.Legacy.Skin;

namespace Gwen.Net.Legacy.Control.Internal
{
    /// <summary>
    ///     Numeric up arrow.
    /// </summary>
    public class UpDownButtonUpKind : ButtonBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UpDownButtonUpKind" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public UpDownButtonUpKind(ControlBase parent)
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
            currentSkin.DrawNumericUpDownButton(this, IsDepressed, up: true);
        }
    }
}
