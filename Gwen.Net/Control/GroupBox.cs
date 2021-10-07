using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Skin;
using Gwen.Net.Xml;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Group box (container).
    /// </summary>
    [XmlControl]
    public class GroupBox : ContentControl
    {
        private readonly Text m_Text;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GroupBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public GroupBox(ControlBase parent)
            : base(parent)
        {
            m_Text = new Text(this);

            m_InnerPanel = new InnerContentControl(this);
        }

        /// <summary>
        ///     Text.
        /// </summary>
        [XmlProperty] public virtual string Text
        {
            get => m_Text.String;
            set => m_Text.String = value;
        }

        [XmlProperty] public override Padding Padding
        {
            get => m_InnerPanel.Padding;
            set => m_InnerPanel.Padding = value;
        }

        protected override Size Measure(Size availableSize)
        {
            Size titleSize = m_Text.DoMeasure(availableSize);

            Size innerSize = Size.Zero;

            if (m_InnerPanel != null)
            {
                innerSize = m_InnerPanel.DoMeasure(
                    new Size(availableSize.Width - 5 - 5, availableSize.Height - titleSize.Height - 5));
            }

            return new Size(
                Math.Max(10 + titleSize.Width + 10, 5 + innerSize.Width + 5),
                titleSize.Height + innerSize.Height + 5);
        }

        protected override Size Arrange(Size finalSize)
        {
            Size size = finalSize;

            m_Text.DoArrange(new Rectangle(x: 10, y: 0, m_Text.MeasuredSize.Width, m_Text.MeasuredSize.Height));

            if (m_InnerPanel != null)
            {
                m_InnerPanel.DoArrange(
                    new Rectangle(
                        x: 5,
                        m_Text.MeasuredSize.Height,
                        finalSize.Width - 5 - 5,
                        finalSize.Height - m_Text.MeasuredSize.Height - 5));
            }

            return size;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin)
        {
            skin.DrawGroupBox(this, textStart: 10, m_Text.ActualHeight, m_Text.ActualWidth);
        }
    }
}