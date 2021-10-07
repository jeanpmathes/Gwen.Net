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
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin)
        {
            if (ShouldDrawBackground)
            {
                skin.DrawButton(this, depressed: true, hovered: false, IsDisabled);
            }
        }
    }
}