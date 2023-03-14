using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Input;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Horizontal scrollbar.
    /// </summary>
    public class HorizontalScrollBar : ScrollBar
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="HorizontalScrollBar" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public HorizontalScrollBar(ControlBase parent)
            : base(parent)
        {
            Height = BaseUnit;

            bar.IsHorizontal = true;

            scrollButton[0].Dock = Dock.Left;
            scrollButton[0].SetDirectionLeft();
            scrollButton[0].Clicked += NudgeLeft;

            scrollButton[1].Dock = Dock.Right;
            scrollButton[1].SetDirectionRight();
            scrollButton[1].Clicked += NudgeRight;

            bar.Dock = Dock.Fill;
            bar.Dragged += OnBarMoved;
        }

        /// <summary>
        ///     Bar size (in pixels).
        /// </summary>
        public override int BarSize => bar.ActualWidth;

        /// <summary>
        ///     Bar position (in pixels).
        /// </summary>
        public override int BarPos => bar.ActualLeft - ActualHeight;

        /// <summary>
        ///     Indicates whether the bar is horizontal.
        /// </summary>
        public override bool IsHorizontal => true;

        /// <summary>
        ///     Button size (in pixels).
        /// </summary>
        public override int ButtonSize => ActualHeight;

        public override int Height
        {
            get => base.Height;

            set
            {
                base.Height = value;

                scrollButton[0].Width = Height;
                scrollButton[1].Width = Height;
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
            Height = BaseUnit;
        }

        protected override Size Arrange(Size finalSize)
        {
            Size size = base.Arrange(finalSize);

            SetScrollAmount(ScrollAmount, forceUpdate: true);

            return size;
        }

        protected override void UpdateBarSize()
        {
            var barWidth = 0.0f;

            if (contentSize > 0.0f)
            {
                barWidth = viewableContentSize / contentSize * (ActualWidth - (ButtonSize * 2));
            }

            if (barWidth < ButtonSize * 0.5f)
            {
                barWidth = (int) (ButtonSize * 0.5f);
            }

            bar.SetSize((int) barWidth, bar.ActualHeight);
            bar.IsHidden = ActualWidth - (ButtonSize * 2) <= barWidth;

            //Based on our last scroll amount, produce a position for the bar
            if (!bar.IsHeld)
            {
                SetScrollAmount(ScrollAmount, forceUpdate: true);
            }
        }

        public void NudgeLeft(ControlBase control, EventArgs args)
        {
            if (!IsDisabled)
            {
                SetScrollAmount(ScrollAmount - NudgeAmount, forceUpdate: true);
            }
        }

        public void NudgeRight(ControlBase control, EventArgs args)
        {
            if (!IsDisabled)
            {
                SetScrollAmount(ScrollAmount + NudgeAmount, forceUpdate: true);
            }
        }

        public override void ScrollToLeft()
        {
            SetScrollAmount(value: 0, forceUpdate: true);
        }

        public override void ScrollToRight()
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

                if (clickPos.X < bar.ActualLeft)
                {
                    NudgeLeft(this, EventArgs.Empty);
                }
                else if (clickPos.X > bar.ActualLeft + bar.ActualWidth)
                {
                    NudgeRight(this, EventArgs.Empty);
                }

                depressed = false;
                InputHandler.MouseFocus = null;
            }
        }

        protected override float CalculateScrolledAmount()
        {
            float value = (float) (bar.ActualLeft - ButtonSize) /
                          (ActualWidth - bar.ActualWidth - (ButtonSize * 2));

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
        /// <returns>
        ///     True if control state changed.
        /// </returns>
        public override bool SetScrollAmount(float value, bool forceUpdate = false)
        {
            value = Util.Clamp(value, min: 0, max: 1);

            if (!base.SetScrollAmount(value, forceUpdate))
            {
                return false;
            }

            if (forceUpdate)
            {
                var newX = (int) (ButtonSize + value * (ActualWidth - bar.ActualWidth - ButtonSize * 2));
                bar.MoveTo(newX, bar.ActualTop);
            }

            return true;
        }

        /// <summary>
        ///     Handler for the BarMoved event.
        /// </summary>
        /// <param name="control">Event source.</param>
        /// <param name="args">Event arguments.</param>
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
