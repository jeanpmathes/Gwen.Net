using System;
using Gwen.Net.Input;
using Gwen.Net.Skin;

namespace Gwen.Net.Control.Internal
{
    /// <summary>
    ///     Base slider.
    /// </summary>
    public class Slider : ControlBase
    {
        protected readonly SliderBar sliderBar;
        protected Single max;
        protected Single min;
        protected Int32 notchCount;
        protected Boolean snapToNotches;
        protected Single value;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Slider" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        protected Slider(ControlBase parent)
            : base(parent)
        {
            sliderBar = new SliderBar(this);
            sliderBar.Dragged += OnMoved;

            min = 0.0f;
            max = 1.0f;

            snapToNotches = false;
            notchCount = 5;
            value = 0.0f;

            KeyboardInputEnabled = true;
            IsTabable = true;
        }

        /// <summary>
        ///     Number of notches on the slider axis.
        /// </summary>
        public Int32 NotchCount
        {
            get => notchCount;
            set => notchCount = value;
        }

        /// <summary>
        ///     Determines whether the slider should snap to notches.
        /// </summary>
        public Boolean SnapToNotches
        {
            get => snapToNotches;
            set => snapToNotches = value;
        }

        /// <summary>
        ///     Minimum value.
        /// </summary>
        public Single Min
        {
            get => min;
            set => SetRange(value, max);
        }

        /// <summary>
        ///     Maximum value.
        /// </summary>
        public Single Max
        {
            get => max;
            set => SetRange(min, value);
        }

        /// <summary>
        ///     Current value.
        /// </summary>
        public Single Value
        {
            get => min + (value * (max - min));
            set
            {
                if (value < min)
                {
                    value = min;
                }

                if (value > max)
                {
                    value = max;
                }

                // Normalize Value
                value = (value - min) / (max - min);
                SetValueInternal(value);
                Redraw();
            }
        }

        /// <summary>
        ///     Invoked when the value has been changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> ValueChanged;

        /// <summary>
        ///     Handler for Right Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyRight(Boolean down)
        {
            if (down)
            {
                Value = Value + 1;
            }

            return true;
        }

        /// <summary>
        ///     Handler for Up Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyUp(Boolean down)
        {
            if (down)
            {
                Value = Value + 1;
            }

            return true;
        }

        /// <summary>
        ///     Handler for Left Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyLeft(Boolean down)
        {
            if (down)
            {
                Value = Value - 1;
            }

            return true;
        }

        /// <summary>
        ///     Handler for Down Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyDown(Boolean down)
        {
            if (down)
            {
                Value = Value - 1;
            }

            return true;
        }

        /// <summary>
        ///     Handler for Home keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyHome(Boolean down)
        {
            if (down)
            {
                Value = min;
            }

            return true;
        }

        /// <summary>
        ///     Handler for End keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyEnd(Boolean down)
        {
            if (down)
            {
                Value = max;
            }

            return true;
        }

        /// <summary>
        ///     Handler invoked on mouse click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="down">If set to <c>true</c> mouse button is down.</param>
        protected override void OnMouseClickedLeft(Int32 x, Int32 y, Boolean down) {}

        protected virtual void OnMoved(ControlBase control, EventArgs args)
        {
            SetValueInternal(CalculateValue());
        }

        protected virtual Single CalculateValue()
        {
            return 0;
        }

        protected virtual void UpdateBarFromValue() {}

        protected virtual void SetValueInternal(Single val)
        {
            if (snapToNotches)
            {
                val = (Single)Math.Floor((val * notchCount) + 0.5f);
                val /= notchCount;
            }

            if (value != val)
            {
                value = val;

                if (ValueChanged != null)
                {
                    ValueChanged.Invoke(this, EventArgs.Empty);
                }
            }

            UpdateBarFromValue();
        }

        /// <summary>
        ///     Sets the value range.
        /// </summary>
        /// <param name="newMin">Minimum value.</param>
        /// <param name="newMax">Maximum value.</param>
        public void SetRange(Single newMin, Single newMax)
        {
            min = newMin;
            max = newMax;
        }

        /// <summary>
        ///     Renders the focus overlay.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void RenderFocus(SkinBase currentSkin)
        {
            if (InputHandler.KeyboardFocus != this)
            {
                return;
            }

            if (!IsTabable)
            {
                return;
            }

            currentSkin.DrawKeyboardHighlight(this, RenderBounds, offset: 0);
        }

        protected override Size Measure(Size availableSize)
        {
            sliderBar.DoMeasure(availableSize);

            return sliderBar.MeasuredSize;
        }

        protected override Size Arrange(Size finalSize)
        {
            sliderBar.DoArrange(new Rectangle(Point.Zero, sliderBar.MeasuredSize));

            UpdateBarFromValue();

            return finalSize;
        }

        protected override void OnBoundsChanged(Rectangle oldBounds)
        {
            base.OnBoundsChanged(oldBounds);

            // We need to know if bounds are changed to update the bar.
            // In Arrange() we don't know yet new bounds.
            UpdateBarFromValue();
        }
    }
}
