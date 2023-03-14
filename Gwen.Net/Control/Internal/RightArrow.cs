using Gwen.Net.Skin;

namespace Gwen.Net.Control.Internal
{
    /// <summary>
    ///     Submenu indicator.
    /// </summary>
    public class RightArrow : ControlBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RightArrow" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public RightArrow(ControlBase parent)
            : base(parent)
        {
            MouseInputEnabled = false;

            Size = new Size(BaseUnit);
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
            currentSkin.DrawMenuRightArrow(this);
        }
    }
}
