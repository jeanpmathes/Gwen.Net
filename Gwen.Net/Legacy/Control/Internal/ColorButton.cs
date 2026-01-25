using Gwen.Net.Legacy.Skin;

namespace Gwen.Net.Legacy.Control.Internal
{
    /// <summary>
    ///     Property button.
    /// </summary>
    public class ColorButton : ButtonBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ColorButton" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ColorButton(ControlBase parent) : base(parent)
        {
            Color = Color.Black;
        }

        /// <summary>
        ///     Current color value.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.Renderer.DrawColor = Color;
            currentSkin.Renderer.DrawFilledRect(RenderBounds);
        }
    }
}
