using Gwen.Net.Control.Internal;
using Gwen.Net.Skin;
using Gwen.Net.Xml;

namespace Gwen.Net.Control
{
    [XmlControl]
    public class ToolWindow : WindowBase
    {
        private bool m_vertical;

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

            m_InnerPanel = new InnerContentControl(this);
            m_InnerPanel.SendToBack();
        }

        [XmlProperty] public bool Vertical
        {
            get => m_vertical;
            set
            {
                m_vertical = value;

                if (m_vertical)
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
            if (m_vertical) dragBar.Height = BaseUnit + 2;
            else dragBar.Width = BaseUnit + 2;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin)
        {
            bool hasFocus = IsOnTop;

            skin.DrawToolWindow(this, m_vertical, m_vertical ? dragBar.ActualHeight : dragBar.ActualWidth);
        }

        /// <summary>
        ///     Renders under the actual control (shadows etc).
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void RenderUnder(SkinBase skin)
        {
            base.RenderUnder(skin);
            skin.DrawShadow(this);
        }

        /// <summary>
        ///     Renders the focus overlay.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void RenderFocus(SkinBase skin) {}

        protected override Size Measure(Size availableSize)
        {
            Size titleBarSize = dragBar.DoMeasure(new Size(availableSize.Width, availableSize.Height));

            if (m_InnerPanel != null)
            {
                if (m_vertical)
                {
                    m_InnerPanel.DoMeasure(new Size(availableSize.Width, availableSize.Height - titleBarSize.Height));
                }
                else
                {
                    m_InnerPanel.DoMeasure(new Size(availableSize.Width - titleBarSize.Width, availableSize.Height));
                }
            }

            if (m_vertical)
            {
                return base.Measure(
                    new Size(m_InnerPanel.MeasuredSize.Width, m_InnerPanel.MeasuredSize.Height + titleBarSize.Height));
            }

            return base.Measure(
                new Size(m_InnerPanel.MeasuredSize.Width + titleBarSize.Width, m_InnerPanel.MeasuredSize.Height));
        }

        protected override Size Arrange(Size finalSize)
        {
            if (m_vertical)
            {
                dragBar.DoArrange(new Rectangle(x: 0, y: 0, finalSize.Width, dragBar.MeasuredSize.Height));
            }
            else
            {
                dragBar.DoArrange(new Rectangle(x: 0, y: 0, dragBar.MeasuredSize.Width, finalSize.Height));
            }

            if (m_InnerPanel != null)
            {
                if (m_vertical)
                {
                    m_InnerPanel.DoArrange(
                        new Rectangle(
                            x: 0,
                            dragBar.MeasuredSize.Height,
                            finalSize.Width,
                            finalSize.Height - dragBar.MeasuredSize.Height));
                }
                else
                {
                    m_InnerPanel.DoArrange(
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
            base.EnableResizing(!m_vertical ? false : left, m_vertical ? false : top, right, bottom);
        }
    }
}