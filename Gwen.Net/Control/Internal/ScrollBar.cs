using System;
using Gwen.Net.Skin;

namespace Gwen.Net.Control.Internal
{
    /// <summary>
    ///     Base class for scrollbars.
    /// </summary>
    public class ScrollBar : ControlBase
    {
        protected readonly ScrollBarBar bar;
        protected readonly ScrollBarButton[] scrollButton;
        protected Single contentSize;

        protected Boolean depressed;
        protected Single nudgeAmount;
        protected Single scrollAmount;
        protected Single viewableContentSize;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ScrollBar" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        protected ScrollBar(ControlBase parent) : base(parent)
        {
            scrollButton = new ScrollBarButton[2];
            scrollButton[0] = new ScrollBarButton(this);
            scrollButton[1] = new ScrollBarButton(this);

            bar = new ScrollBarBar(this);

            depressed = false;

            scrollAmount = 0;
            contentSize = 0;
            viewableContentSize = 0;

            NudgeAmount = 20;
        }

        /// <summary>
        ///     Bar size (in pixels).
        /// </summary>
        public virtual Int32 BarSize { get; set; }

        /// <summary>
        ///     Bar position (in pixels).
        /// </summary>
        public virtual Int32 BarPos => 0;

        /// <summary>
        ///     Button size (in pixels).
        /// </summary>
        public virtual Int32 ButtonSize => 0;

        public virtual Single NudgeAmount
        {
            get => nudgeAmount / contentSize;
            set => nudgeAmount = value;
        }

        public Single ScrollAmount => scrollAmount;
        public Single ContentSize => contentSize;
        public Single ViewableContentSize => viewableContentSize;

        /// <summary>
        ///     Indicates whether the bar is horizontal.
        /// </summary>
        public virtual Boolean IsHorizontal => false;

        /// <summary>
        ///     Invoked when the bar is moved.
        /// </summary>
        public event GwenEventHandler<EventArgs> BarMoved;
        
        /// <summary>
        /// Set whether to ignore a mouse hold. Necessary during arranging.
        /// </summary>
        /// <param name="ignore">Whether to ignore a mouse hold</param>
        internal void SetHoldIgnore(Boolean ignore) => bar.SetHoldIgnore(ignore);

        /// <summary>
        ///     Sets the scroll amount (0-1).
        /// </summary>
        /// <param name="value">Scroll amount.</param>
        /// <param name="forceUpdate">Determines whether the control should be updated.</param>
        /// <returns>True if control state changed.</returns>
        public virtual Boolean SetScrollAmount(Single value, Boolean forceUpdate = false)
        {
            if (Math.Abs(scrollAmount - value) < 0.0001f && !forceUpdate)
            {
                return false;
            }

            scrollAmount = value;
            OnBarMoved(this, EventArgs.Empty);

            return true;
        }

        public void SetContentSize(Single newContentSize, Single newViewableContentSize)
        {
            this.contentSize = newContentSize;
            this.viewableContentSize = newViewableContentSize;

            UpdateBarSize();
        }

        protected virtual void UpdateBarSize() {}

        /// <summary>
        ///     Handler invoked on mouse click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="down">If set to <c>true</c> mouse button is down.</param>
        protected override void OnMouseClickedLeft(Int32 x, Int32 y, Boolean down)
        {
            // Intentionally left empty.
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawScrollBar(this, IsHorizontal, depressed);
        }

        /// <summary>
        ///     Handler for the BarMoved event.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnBarMoved(ControlBase control, EventArgs args)
        {
            if (BarMoved != null)
            {
                BarMoved.Invoke(this, EventArgs.Empty);
            }
        }

        protected virtual Single CalculateScrolledAmount()
        {
            return 0;
        }

        protected virtual Int32 CalculateBarSize()
        {
            return 0;
        }

        public virtual void ScrollToLeft() {}
        public virtual void ScrollToRight() {}
        public virtual void ScrollToTop() {}
        public virtual void ScrollToBottom() {}
    }
}
