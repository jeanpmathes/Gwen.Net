using Gwen.Net.Legacy.Skin;

namespace Gwen.Net.Legacy.Control.Internal
{
    /// <summary>
    ///     Divider menu item.
    /// </summary>
    public class MenuDivider : ControlBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuDivider" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public MenuDivider(ControlBase parent)
            : base(parent) {}

        protected override Size Measure(Size availableSize)
        {
            return new(width: 10, height: 1);
        }

        protected override Size Arrange(Size finalSize)
        {
            return new(finalSize.Width, height: 1);
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawMenuDivider(this);
        }
    }
}
