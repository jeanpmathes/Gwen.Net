using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Input;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Vertical scrollbar.
    /// </summary>
    public class VerticalScrollBar : ScrollBar
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VerticalScrollBar" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public VerticalScrollBar(ControlBase parent)
            : base(parent)
        {
            Width = BaseUnit;

            m_Bar.IsVertical = true;

            m_ScrollButton[0].Dock = Dock.Top;
            m_ScrollButton[0].SetDirectionUp();
            m_ScrollButton[0].Clicked += NudgeUp;

            m_ScrollButton[1].Dock = Dock.Bottom;
            m_ScrollButton[1].SetDirectionDown();
            m_ScrollButton[1].Clicked += NudgeDown;

            m_Bar.Dock = Dock.Fill;
            m_Bar.Dragged += OnBarMoved;
        }

        /// <summary>
        ///     Bar size (in pixels).
        /// </summary>
        public override int BarSize => m_Bar.ActualHeight;

        /// <summary>
        ///     Bar position (in pixels).
        /// </summary>
        public override int BarPos => m_Bar.ActualTop - ActualWidth;

        /// <summary>
        ///     Button size (in pixels).
        /// </summary>
        public override int ButtonSize => ActualWidth;

        public override int Width
        {
            get => base.Width;

            set
            {
                base.Width = value;

                m_ScrollButton[0].Height = Width;
                m_ScrollButton[1].Height = Width;
            }
        }

        public override float NudgeAmount
        {
            get
            {
                if (m_Depressed)
                {
                    return m_ViewableContentSize / m_ContentSize;
                }

                return base.NudgeAmount;
            }
            set => base.NudgeAmount = value;
        }

        protected override Size Arrange(Size finalSize)
        {
            Size size = base.Arrange(finalSize);

            SetScrollAmount(ScrollAmount, forceUpdate: true);

            return size;
        }

        protected override void UpdateBarSize()
        {
            float barHeight = 0.0f;

            if (m_ContentSize > 0.0f)
            {
                barHeight = m_ViewableContentSize / m_ContentSize * (ActualHeight - (ButtonSize * 2));
            }

            if (barHeight < ButtonSize * 0.5f)
            {
                barHeight = (int)(ButtonSize * 0.5f);
            }

            m_Bar.SetSize(m_Bar.ActualWidth, (int)barHeight);
            m_Bar.IsHidden = ActualHeight - (ButtonSize * 2) <= barHeight;

            //Based on our last scroll amount, produce a position for the bar
            if (!m_Bar.IsHeld)
            {
                SetScrollAmount(ScrollAmount, forceUpdate: true);
            }
        }

        public virtual void NudgeUp(ControlBase control, EventArgs args)
        {
            if (!IsDisabled)
            {
                SetScrollAmount(ScrollAmount - NudgeAmount, forceUpdate: true);
            }
        }

        public virtual void NudgeDown(ControlBase control, EventArgs args)
        {
            if (!IsDisabled)
            {
                SetScrollAmount(ScrollAmount + NudgeAmount, forceUpdate: true);
            }
        }

        public override void ScrollToTop()
        {
            SetScrollAmount(value: 0, forceUpdate: true);
        }

        public override void ScrollToBottom()
        {
            SetScrollAmount(value: 1, forceUpdate: true);
        }

        /// <summary>
        ///     Handler invoked on mouse click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="down">If set to <c>true</c> mouse button is down.</param>
        protected override void OnMouseClickedLeft(int x, int y, bool down)
        {
            base.OnMouseClickedLeft(x, y, down);

            if (down)
            {
                m_Depressed = true;
                InputHandler.MouseFocus = this;
            }
            else
            {
                Point clickPos = CanvasPosToLocal(new Point(x, y));

                if (clickPos.Y < m_Bar.ActualTop)
                {
                    NudgeUp(this, EventArgs.Empty);
                }
                else if (clickPos.Y > m_Bar.ActualTop + m_Bar.ActualHeight)
                {
                    NudgeDown(this, EventArgs.Empty);
                }

                m_Depressed = false;
                InputHandler.MouseFocus = null;
            }
        }

        protected override float CalculateScrolledAmount()
        {
            float value = (float)(m_Bar.ActualTop - ButtonSize) /
                          (ActualHeight - m_Bar.ActualHeight - (ButtonSize * 2));

            if (Single.IsNaN(value))
            {
                value = 0.0f;
            }

            return value;
        }

        /// <summary>
        ///     Sets the scroll amount (0-1).
        /// </summary>
        /// <param name="value">Scroll amount.</param>
        /// <param name="forceUpdate">Determines whether the control should be updated.</param>
        /// <returns>True if control state changed.</returns>
        public override bool SetScrollAmount(float value, bool forceUpdate = false)
        {
            value = Util.Clamp(value, min: 0, max: 1);

            if (!base.SetScrollAmount(value, forceUpdate))
            {
                return false;
            }

            if (forceUpdate)
            {
                int newY = (int)(ButtonSize + (value * (ActualHeight - m_Bar.ActualHeight - (ButtonSize * 2))));
                m_Bar.MoveTo(m_Bar.ActualLeft, newY);
            }

            return true;
        }

        /// <summary>
        ///     Handler for the BarMoved event.
        /// </summary>
        /// <param name="control">The control.</param>
        protected override void OnBarMoved(ControlBase control, EventArgs args)
        {
            if (m_Bar.IsHeld)
            {
                SetScrollAmount(CalculateScrolledAmount());
            }

            base.OnBarMoved(control, EventArgs.Empty);
        }
    }
}