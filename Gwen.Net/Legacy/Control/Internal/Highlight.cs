using Gwen.Net.Legacy.Skin;

namespace Gwen.Net.Legacy.Control.Internal
{
    /// <summary>
    ///     Drag and drop highlight.
    /// </summary>
    public class Highlight : ControlBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Highlight" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Highlight(ControlBase parent) : base(parent) {}

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawHighlight(this);
        }
    }
}
