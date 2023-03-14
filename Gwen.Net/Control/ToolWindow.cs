using Gwen.Net.Control.Internal;
using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    public class ToolWindow : WindowBase
    {
        private bool vertical;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ToolWindow" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ToolWindow(ControlBase parent)
            : base(parent)
        {
            dragBar = new Dragger(this);
            dragBar.Target = this;
            dragBar.SendToBack();

            Vertical = false;

            innerPanel = new InnerContentControl(this);
            innerPanel.SendToBack();
        }

        public bool Vertical
        {
            get => vertical;
            set
            {
                vertical = value;

                if (vertical)
                {
                    dragBar.Height = BaseUnit + 2;
                    dragBar.Width = Util.Ignore;
                }
                else
                {
                    dragBar.Width = BaseUnit + 2;
                    dragBar.Height = Util.Ignore;
                }

                EnableResizing();
                Invalidate();
            }
        }

        protected override void AdaptToScaleChange()
        {
            if (vertical) dragBar.Height = BaseUnit + 2;
            else dragBar.Width = BaseUnit + 2;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawToolWindow(this, vertical, vertical ? dragBar.ActualHeight : dragBar.ActualWidth);
        }

        /// <summary>
        ///     Renders under the actual control (shadows etc).
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void RenderUnder(SkinBase currentSkin)
        {
            base.RenderUnder(currentSkin);
            currentSkin.DrawShadow(this);
        }

        /// <summary>
        ///     Renders the focus overlay.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void RenderFocus(SkinBase currentSkin) {}

        protected override Size Measure(Size availableSize)
        {
            Size titleBarSize = dragBar.DoMeasure(new Size(availableSize.Width, availableSize.Height));

            if (innerPanel != null)
            {
                if (vertical)
                {
                    innerPanel.DoMeasure(new Size(availableSize.Width, availableSize.Height - titleBarSize.Height));
                }
                else
                {
                    innerPanel.DoMeasure(new Size(availableSize.Width - titleBarSize.Width, availableSize.Height));
                }
            }

            if (vertical)
            {
                return base.Measure(
                    new Size(innerPanel.MeasuredSize.Width, innerPanel.MeasuredSize.Height + titleBarSize.Height));
            }

            return base.Measure(
                new Size(innerPanel.MeasuredSize.Width + titleBarSize.Width, innerPanel.MeasuredSize.Height));
        }

        protected override Size Arrange(Size finalSize)
        {
            if (vertical)
            {
                dragBar.DoArrange(new Rectangle(x: 0, y: 0, finalSize.Width, dragBar.MeasuredSize.Height));
            }
            else
            {
                dragBar.DoArrange(new Rectangle(x: 0, y: 0, dragBar.MeasuredSize.Width, finalSize.Height));
            }

            if (innerPanel != null)
            {
                if (vertical)
                {
                    innerPanel.DoArrange(
                        new Rectangle(
                            x: 0,
                            dragBar.MeasuredSize.Height,
                            finalSize.Width,
                            finalSize.Height - dragBar.MeasuredSize.Height));
                }
                else
                {
                    innerPanel.DoArrange(
                        new Rectangle(
                            dragBar.MeasuredSize.Width,
                            y: 0,
                            finalSize.Width - dragBar.MeasuredSize.Width,
                            finalSize.Height));
                }
            }

            return base.Arrange(finalSize);
        }

        public override void EnableResizing(bool left = true, bool top = true, bool right = true, bool bottom = true)
        {
            base.EnableResizing(!vertical ? false : left, vertical ? false : top, right, bottom);
        }
    }
}
