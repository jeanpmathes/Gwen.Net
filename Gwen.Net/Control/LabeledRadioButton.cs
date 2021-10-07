using System;
using Gwen.Net.Input;
using Gwen.Net.Skin;
using Gwen.Net.Xml;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     RadioButton with label.
    /// </summary>
    [XmlControl]
    public class LabeledRadioButton : ControlBase
    {
        private readonly Label m_Label;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LabeledRadioButton" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public LabeledRadioButton(ControlBase parent)
            : base(parent)
        {
            MouseInputEnabled = true;

            RadioButton = new RadioButton(this);
            RadioButton.IsTabable = false;
            RadioButton.KeyboardInputEnabled = false;

            m_Label = new Label(this);
            m_Label.Alignment = Alignment.CenterV | Alignment.Left;
            m_Label.Text = "Radio Button";
            m_Label.Clicked += delegate(ControlBase control, ClickedEventArgs args) { RadioButton.Press(control); };
            m_Label.IsTabable = false;
            m_Label.KeyboardInputEnabled = false;
        }

        /// <summary>
        ///     Label text.
        /// </summary>
        [XmlProperty] public string Text
        {
            get => m_Label.Text;
            set => m_Label.Text = value;
        }

        // todo: would be nice to remove that
        internal RadioButton RadioButton { get; }

        /// <summary>
        ///     Invoked when the radiobutton has been checked.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> Checked
        {
            add => RadioButton.Checked += value;
            remove => RadioButton.Checked -= value;
        }

        protected override Size Measure(Size availableSize)
        {
            Size labelSize = m_Label.DoMeasure(availableSize);
            Size radioButtonSize = RadioButton.DoMeasure(availableSize);

            return new Size(
                labelSize.Width + 4 + radioButtonSize.Width,
                Math.Max(labelSize.Height, radioButtonSize.Height));
        }

        protected override Size Arrange(Size finalSize)
        {
            if (RadioButton.MeasuredSize.Height > m_Label.MeasuredSize.Height)
            {
                RadioButton.DoArrange(
                    new Rectangle(x: 0, y: 0, RadioButton.MeasuredSize.Width, RadioButton.MeasuredSize.Height));

                m_Label.DoArrange(
                    new Rectangle(
                        RadioButton.MeasuredSize.Width + 4,
                        (RadioButton.MeasuredSize.Height - m_Label.MeasuredSize.Height) / 2,
                        m_Label.MeasuredSize.Width,
                        m_Label.MeasuredSize.Height));
            }
            else
            {
                RadioButton.DoArrange(
                    new Rectangle(
                        x: 0,
                        (m_Label.MeasuredSize.Height - RadioButton.MeasuredSize.Height) / 2,
                        RadioButton.MeasuredSize.Width,
                        RadioButton.MeasuredSize.Height));

                m_Label.DoArrange(
                    new Rectangle(
                        RadioButton.MeasuredSize.Width + 4,
                        y: 0,
                        m_Label.MeasuredSize.Width,
                        m_Label.MeasuredSize.Height));
            }

            return finalSize;
        }

        /// <summary>
        ///     Renders the focus overlay.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void RenderFocus(SkinBase skin)
        {
            if (InputHandler.KeyboardFocus != this)
            {
                return;
            }

            if (!IsTabable)
            {
                return;
            }

            skin.DrawKeyboardHighlight(this, RenderBounds, offset: 0);
        }

        /// <summary>
        ///     Handler for Space keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnKeySpace(bool down)
        {
            if (down)
            {
                RadioButton.IsChecked = !RadioButton.IsChecked;
            }

            return true;
        }

        /// <summary>
        ///     Selects the radio button.
        /// </summary>
        public virtual void Select()
        {
            RadioButton.IsChecked = true;
        }
    }
}