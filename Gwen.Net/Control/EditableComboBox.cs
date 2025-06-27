using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Editable ComboBox control.
    /// </summary>
    public class EditableComboBox : ComboBoxBase
    {
        private readonly ComboBoxButton button;
        private readonly TextBox textBox;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EditableComboBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public EditableComboBox(ControlBase parent)
            : base(parent)
        {
            textBox = new TextBox(this);

            button = new ComboBoxButton(textBox, this);
            button.Dock = Dock.Right;
            button.Clicked += OnClicked;

            IsTabable = true;
            KeyboardInputEnabled = true;
        }

        /// <summary>
        ///     Text.
        /// </summary>
        public virtual String Text
        {
            get => textBox.Text;
            set => textBox.SetText(value);
        }

        /// <summary>
        ///     Text color.
        /// </summary>
        public Color TextColor
        {
            get => textBox.TextColor;
            set => textBox.TextColor = value;
        }

        /// <summary>
        ///     Font.
        /// </summary>
        public Font Font
        {
            get => textBox.Font;
            set => textBox.Font = value;
        }

        internal Boolean IsDepressed => button.IsDepressed;

        /// <summary>
        ///     Invoked when the text has changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> TextChanged
        {
            add => textBox.TextChanged += value;
            remove => textBox.TextChanged -= value;
        }

        /// <summary>
        ///     Invoked when the submit key has been pressed.
        /// </summary>
        public event GwenEventHandler<EventArgs> SubmitPressed
        {
            add => textBox.SubmitPressed += value;
            remove => textBox.SubmitPressed -= value;
        }

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

                textBox.Text = item.Text;
            }

            base.OnItemSelected(control, args);
        }

        protected override Size Measure(Size availableSize)
        {
            return textBox.DoMeasure(availableSize);
        }

        protected override Size Arrange(Size finalSize)
        {
            textBox.DoArrange(new Rectangle(Point.Zero, finalSize));

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
