using Gwen.Net.Skin;

namespace Gwen.Net.Control.Internal
{
    public enum ScrollBarButtonDirection
    {
        Left, Top, Right, Bottom
    }

    /// <summary>
    ///     Scrollbar button.
    /// </summary>
    public class ScrollBarButton : ButtonBase
    {
        private ScrollBarButtonDirection direction;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ScrollBarButton" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ScrollBarButton(ControlBase parent)
            : base(parent)
        {
            SetDirectionUp();
        }

        public virtual void SetDirectionUp()
        {
            direction = ScrollBarButtonDirection.Top;
        }

        public virtual void SetDirectionDown()
        {
            direction = ScrollBarButtonDirection.Bottom;
        }

        public virtual void SetDirectionLeft()
        {
            direction = ScrollBarButtonDirection.Left;
        }

        public virtual void SetDirectionRight()
        {
            direction = ScrollBarButtonDirection.Right;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawScrollButton(this, direction, IsDepressed, IsHovered, IsDisabled);
        }
    }
}
