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
        private readonly Button button;
        private readonly DownArrow downArrow;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ComboBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ComboBox(ControlBase parent)
            : base(parent)
        {
            button = new Button(this);
            button.Alignment = Alignment.Left | Alignment.CenterV;
            button.Text = String.Empty;
            button.TextPadding = Padding.Three;
            button.Clicked += OnClicked;

            downArrow = new DownArrow(this);

            IsTabable = true;
            KeyboardInputEnabled = true;
        }

        internal Boolean IsDepressed => button.IsDepressed;
        public override Boolean IsHovered => button.IsHovered;

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
            button.Text = String.Empty;
            base.RemoveAll();
        }

        /// <summary>
        ///     Internal handler for item selected event.
        /// </summary>
        /// <param name="control">Event source.</param>
        /// <param name="args">Event arguments.</param>
        protected override void OnItemSelected(ControlBase control, ItemSelectedEventArgs args)
        {
            if (!IsDisabled)
            {
                var item = control as MenuItem;

                if (null == item)
                {
                    return;
                }

                button.Text = item.Text;
            }

            base.OnItemSelected(control, args);
        }

        protected override Size Measure(Size availableSize)
        {
            return Size.Max(button.DoMeasure(availableSize), downArrow.DoMeasure(availableSize));
        }

        protected override Size Arrange(Size finalSize)
        {
            button.DoArrange(new Rectangle(Point.Zero, finalSize));

            downArrow.DoArrange(
                new Rectangle(
                    finalSize.Width - button.TextPadding.Right - downArrow.MeasuredSize.Width,
                    (finalSize.Height - downArrow.MeasuredSize.Height) / 2,
                    downArrow.MeasuredSize.Width,
                    downArrow.MeasuredSize.Height));

            return finalSize;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawComboBox(this, button.IsDepressed, IsOpen);
        }

        /// <summary>
        ///     Renders the focus overlay.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void RenderFocus(SkinBase currentSkin) {}
    }
}
