using Gwen.Net.Control.Internal;
using Gwen.Net.Skin;
using Gwen.Net.Xml;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Static text label.
    /// </summary>
    [XmlControl]
    public class Label : ControlBase
    {
        protected readonly Text m_Text;
        private Alignment m_Align;
        private bool m_AutoSizeToContent;
        private Padding m_TextPadding;


        /// <summary>
        ///     Initializes a new instance of the <see cref="Label" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Label(ControlBase parent) : base(parent)
        {
            m_Text = new Text(this);
            //m_Text.Font = Skin.DefaultFont;

            m_AutoSizeToContent = true;

            MouseInputEnabled = false;
            Alignment = Alignment.Left | Alignment.Top;
        }

        /// <summary>
        ///     Text alignment.
        /// </summary>
        [XmlProperty] public Alignment Alignment
        {
            get => m_Align;
            set
            {
                m_Align = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Text.
        /// </summary>
        [XmlProperty] public virtual string Text
        {
            get => m_Text.String;
            set => m_Text.String = value;
        }

        /// <summary>
        ///     Font.
        /// </summary>
        [XmlProperty] public Font Font
        {
            get => m_Text.Font;
            set
            {
                m_Text.Font = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Text color.
        /// </summary>
        [XmlProperty] public Color TextColor
        {
            get => m_Text.TextColor;
            set => m_Text.TextColor = value;
        }

        /// <summary>
        ///     Override text color (used by tooltips).
        /// </summary>
        [XmlProperty] public Color TextColorOverride
        {
            get => m_Text.TextColorOverride;
            set => m_Text.TextColorOverride = value;
        }

        /// <summary>
        ///     Text override - used to display different string.
        /// </summary>
        [XmlProperty] public string TextOverride
        {
            get => m_Text.TextOverride;
            set => m_Text.TextOverride = value;
        }

        /// <summary>
        ///     Determines if the control should autosize to its text.
        /// </summary>
        [XmlProperty] public bool AutoSizeToContents
        {
            get => m_AutoSizeToContent;
            set
            {
                m_AutoSizeToContent = value;
                IsVirtualControl = !value;

                if (value)
                {
                    Invalidate();
                }
            }
        }

        /// <summary>
        ///     Text padding.
        /// </summary>
        [XmlProperty] public Padding TextPadding
        {
            get => m_TextPadding;
            set
            {
                m_TextPadding = value;
                Invalidate();
            }
        }

        [XmlEvent] public override event GwenEventHandler<ClickedEventArgs> Clicked
        {
            add
            {
                base.Clicked += value;
                MouseInputEnabled = ClickEventAssigned;
            }
            remove
            {
                base.Clicked -= value;
                MouseInputEnabled = ClickEventAssigned;
            }
        }

        [XmlEvent] public override event GwenEventHandler<ClickedEventArgs> DoubleClicked
        {
            add
            {
                base.DoubleClicked += value;
                MouseInputEnabled = ClickEventAssigned;
            }
            remove
            {
                base.DoubleClicked -= value;
                MouseInputEnabled = ClickEventAssigned;
            }
        }

        [XmlEvent] public override event GwenEventHandler<ClickedEventArgs> RightClicked
        {
            add
            {
                base.RightClicked += value;
                MouseInputEnabled = ClickEventAssigned;
            }
            remove
            {
                base.RightClicked -= value;
                MouseInputEnabled = ClickEventAssigned;
            }
        }

        [XmlEvent] public override event GwenEventHandler<ClickedEventArgs> DoubleRightClicked
        {
            add
            {
                base.DoubleRightClicked += value;
                MouseInputEnabled = ClickEventAssigned;
            }
            remove
            {
                base.DoubleRightClicked -= value;
                MouseInputEnabled = ClickEventAssigned;
            }
        }

        /// <summary>
        ///     Returns index of the character closest to specified point (in canvas coordinates).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual Point GetClosestCharacter(int x, int y)
        {
            return new(m_Text.GetClosestCharacter(m_Text.CanvasPosToLocal(new Point(x, y))), y: 0);
        }

        /// <summary>
        ///     Handler for text changed event.
        /// </summary>
        protected virtual void OnTextChanged() {}

        protected override Size Measure(Size availableSize)
        {
            return m_Text.DoMeasure(availableSize) + m_TextPadding + Padding;
        }

        protected override Size Arrange(Size finalSize)
        {
            Size innerSize = finalSize - m_TextPadding - Padding;
            Rectangle rect = new(Point.Zero, Size.Min(m_Text.MeasuredSize, innerSize));

            if ((m_Align & Alignment.CenterH) != 0)
            {
                rect.X = (innerSize.Width - m_Text.MeasuredSize.Width) / 2;
            }
            else if ((m_Align & Alignment.Right) != 0)
            {
                rect.X = innerSize.Width - m_Text.MeasuredSize.Width;
            }

            if ((m_Align & Alignment.CenterV) != 0)
            {
                rect.Y = (innerSize.Height - m_Text.MeasuredSize.Height) / 2;
            }
            else if ((m_Align & Alignment.Bottom) != 0)
            {
                rect.Y = innerSize.Height - m_Text.MeasuredSize.Height;
            }

            rect.Offset(m_TextPadding + Padding);

            m_Text.DoArrange(rect);

            return finalSize;
        }

        /// <summary>
        ///     Gets the coordinates of specified character.
        /// </summary>
        /// <param name="index">Character index.</param>
        /// <returns>Character coordinates (local).</returns>
        public virtual Point GetCharacterPosition(int index)
        {
            Point p = m_Text.GetCharacterPosition(index);

            return new Point(p.X + m_Text.ActualLeft, p.Y + m_Text.ActualTop);
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin) {}
    }
}