using Gwen.Net.Skin;

namespace Gwen.Net.Control.Internal
{
    /// <summary>
    ///     Window close button.
    /// </summary>
    public class CloseButton : ButtonBase
    {
        private readonly Window window;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CloseButton" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        /// <param name="owner">Window that owns this button.</param>
        public CloseButton(ControlBase parent, Window owner)
            : base(parent)
        {
            window = owner;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawWindowCloseButton(this, IsDepressed && IsHovered, IsHovered && ShouldDrawHover, !window.IsOnTop);
        }
    }
}
