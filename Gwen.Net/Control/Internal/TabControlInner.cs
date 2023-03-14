using Gwen.Net.Skin;

namespace Gwen.Net.Control.Internal
{
    /// <summary>
    ///     Inner panel of tab control.
    /// </summary>
    public class TabControlInner : ControlBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TabControlInner" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        internal TabControlInner(ControlBase parent) : base(parent) {}

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawTabControl(this);
        }
    }
}
