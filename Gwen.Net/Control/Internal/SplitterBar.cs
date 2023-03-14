using Gwen.Net.Skin;

namespace Gwen.Net.Control.Internal
{
    /// <summary>
    ///     Splitter bar.
    /// </summary>
    public class SplitterBar : Dragger
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SplitterBar" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public SplitterBar(ControlBase parent)
            : base(parent)
        {
            Target = this;
            RestrictToParent = true;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            if (ShouldDrawBackground)
            {
                currentSkin.DrawButton(this, depressed: true, hovered: false, IsDisabled);
            }
        }
    }
}
