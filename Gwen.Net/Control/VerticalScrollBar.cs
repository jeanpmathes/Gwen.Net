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

            bar.IsVertical = true;

            scrollButton[0].Dock = Dock.Top;
            scrollButton[0].SetDirectionUp();
            scrollButton[0].Clicked += NudgeUp;

            scrollButton[1].Dock = Dock.Bottom;
            scrollButton[1].SetDirectionDown();
            scrollButton[1].Clicked += NudgeDown;

            bar.Dock = Dock.Fill;
            bar.Dragged += OnBarMoved;
        }

        /// <summary>
        ///     Bar size (in pixels).
        /// </summary>
        public override int BarSize => bar.ActualHeight;

        /// <summary>
        ///     Bar position (in pixels).
        /// </summary>
        public override int BarPos => bar.ActualTop - ActualWidth;

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

                scrollButton[0].Height = Width;
                scrollButton[1].Height = Width;
            }
        }

        public override float NudgeAmount
        {
            get
            {
                if (depressed)
                {
                    return viewableContentSize / contentSize;
                }

                return base.NudgeAmount;
            }
            set => base.NudgeAmount = value;
        }

        protected override void AdaptToScaleChange()
        {
            Width = BaseUnit;
        }

        protected override Size Arrange(Size finalSize)
        {
            Size size = base.Arrange(finalSize);

            SetScrollAmount(ScrollAmount, forceUpdate: true);

            return size;
        }

        protected override void UpdateBarSize()
        {
            var barHeight = 0.0f;

            if (contentSize > 0.0f)
            {
                barHeight = viewableContentSize / contentSize * (ActualHeight - (ButtonSize * 2));
            }

            if (barHeight < ButtonSize * 0.5f)
            {
                barHeight = (int) (ButtonSize * 0.5f);
            }

            bar.SetSize(bar.ActualWidth, (int) barHeight);
            bar.IsHidden = ActualHeight - (ButtonSize * 2) <= barHeight;

            //Based on our last scroll amount, produce a position for the bar
            if (!bar.IsHeld)
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
                depressed = true;
                InputHandler.MouseFocus = this;
            }
            else
            {
                Point clickPos = CanvasPosToLocal(new Point(x, y));

                if (clickPos.Y < bar.ActualTop)
                {
                    NudgeUp(this, EventArgs.Empty);
                }
                else if (clickPos.Y > bar.ActualTop + bar.ActualHeight)
                {
                    NudgeDown(this, EventArgs.Empty);
                }

                depressed = false;
                InputHandler.MouseFocus = null;
            }
        }

        protected override float CalculateScrolledAmount()
        {
            float value = (float) (bar.ActualTop - ButtonSize) /
                          (ActualHeight - bar.ActualHeight - (ButtonSize * 2));

            if (float.IsNaN(value))
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
            
            if (forceUpdate)
            {
                var newY = (int) (ButtonSize + value * (ActualHeight - bar.ActualHeight - ButtonSize * 2));
                bar.MoveTo(bar.ActualLeft, newY);
            }

            bool changed = base.SetScrollAmount(value, forceUpdate);
            
            return changed || forceUpdate;
        }

        /// <summary>
        ///     Handler for the BarMoved event.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected override void OnBarMoved(ControlBase control, EventArgs args)
        {
            if (bar.IsHeld)
            {
                SetScrollAmount(CalculateScrolledAmount());
            }

            base.OnBarMoved(control, EventArgs.Empty);
        }
    }
}
