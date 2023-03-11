using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     ComboBox control.
    /// </summary>
    public class ComboBox : ComboBoxBase
    {
        private readonly Button m_Button;
        private readonly DownArrow m_DownArrow;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ComboBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ComboBox(ControlBase parent)
            : base(parent)
        {
            m_Button = new Button(this);
            m_Button.Alignment = Alignment.Left | Alignment.CenterV;
            m_Button.Text = string.Empty;
            m_Button.TextPadding = Padding.Three;
            m_Button.Clicked += OnClicked;

            m_DownArrow = new DownArrow(this);

            IsTabable = true;
            KeyboardInputEnabled = true;
        }

        internal bool IsDepressed => m_Button.IsDepressed;
        public override bool IsHovered => m_Button.IsHovered;

        /// <summary>
        ///     Internal Pressed implementation.
        /// </summary>
        private void OnClicked(ControlBase sender, ClickedEventArgs args)
        {
            if (IsOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        /// <summary>
        ///     Removes all items.
        /// </summary>
        public override void RemoveAll()
        {
            m_Button.Text = string.Empty;
            base.RemoveAll();
        }

        /// <summary>
        ///     Internal handler for item selected event.
        /// </summary>
        /// <param name="control">Event source.</param>
        protected override void OnItemSelected(ControlBase control, ItemSelectedEventArgs args)
        {
            if (!IsDisabled)
            {
                var item = control as MenuItem;

                if (null == item)
                {
                    return;
                }

                m_Button.Text = item.Text;
            }

            base.OnItemSelected(control, args);
        }

        protected override Size Measure(Size availableSize)
        {
            return Size.Max(m_Button.DoMeasure(availableSize), m_DownArrow.DoMeasure(availableSize));
        }

        protected override Size Arrange(Size finalSize)
        {
            m_Button.DoArrange(new Rectangle(Point.Zero, finalSize));

            m_DownArrow.DoArrange(
                new Rectangle(
                    finalSize.Width - m_Button.TextPadding.Right - m_DownArrow.MeasuredSize.Width,
                    (finalSize.Height - m_DownArrow.MeasuredSize.Height) / 2,
                    m_DownArrow.MeasuredSize.Width,
                    m_DownArrow.MeasuredSize.Height));

            return finalSize;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin)
        {
            skin.DrawComboBox(this, m_Button.IsDepressed, IsOpen);
        }

        /// <summary>
        ///     Renders the focus overlay.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void RenderFocus(SkinBase skin) {}
    }
}
