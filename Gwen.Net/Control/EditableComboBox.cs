using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Skin;
using Gwen.Net.Xml;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Editable ComboBox control.
    /// </summary>
    [XmlControl(CustomHandler = "XmlElementHandler")]
    public class EditableComboBox : ComboBoxBase
    {
        private readonly ComboBoxButton m_Button;
        private readonly TextBox m_TextBox;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EditableComboBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public EditableComboBox(ControlBase parent)
            : base(parent)
        {
            m_TextBox = new TextBox(this);

            m_Button = new ComboBoxButton(m_TextBox, this);
            m_Button.Dock = Dock.Right;
            m_Button.Clicked += OnClicked;

            IsTabable = true;
            KeyboardInputEnabled = true;
        }

        /// <summary>
        ///     Text.
        /// </summary>
        [XmlProperty] public virtual string Text
        {
            get => m_TextBox.Text;
            set => m_TextBox.SetText(value);
        }

        /// <summary>
        ///     Text color.
        /// </summary>
        [XmlProperty] public Color TextColor
        {
            get => m_TextBox.TextColor;
            set => m_TextBox.TextColor = value;
        }

        /// <summary>
        ///     Font.
        /// </summary>
        [XmlProperty] public Font Font
        {
            get => m_TextBox.Font;
            set => m_TextBox.Font = value;
        }

        internal bool IsDepressed => m_Button.IsDepressed;

        /// <summary>
        ///     Invoked when the text has changed.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> TextChanged
        {
            add => m_TextBox.TextChanged += value;
            remove => m_TextBox.TextChanged -= value;
        }

        /// <summary>
        ///     Invoked when the submit key has been pressed.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> SubmitPressed
        {
            add => m_TextBox.SubmitPressed += value;
            remove => m_TextBox.SubmitPressed -= value;
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
        protected override void OnItemSelected(ControlBase control, ItemSelectedEventArgs args)
        {
            if (!IsDisabled)
            {
                MenuItem item = control as MenuItem;

                if (null == item)
                {
                    return;
                }

                m_TextBox.Text = item.Text;
            }

            base.OnItemSelected(control, args);
        }

        protected override Size Measure(Size availableSize)
        {
            return m_TextBox.DoMeasure(availableSize);
        }

        protected override Size Arrange(Size finalSize)
        {
            m_TextBox.DoArrange(new Rectangle(Point.Zero, finalSize));

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

        internal static ControlBase XmlElementHandler(Parser parser, Type type, ControlBase parent)
        {
            EditableComboBox element = new(parent);
            parser.ParseAttributes(element);

            if (parser.MoveToContent())
            {
                foreach (string elementName in parser.NextElement())
                {
                    if (elementName == "Option")
                    {
                        element.AddItem(parser.ParseElement<MenuItem>(element));
                    }
                }
            }

            return element;
        }
    }
}