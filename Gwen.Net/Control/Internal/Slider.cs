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
        protected readonly SliderBar m_SliderBar;
        protected float m_Max;
        protected float m_Min;
        protected int m_NotchCount;
        protected bool m_SnapToNotches;
        protected float m_Value;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Slider" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        protected Slider(ControlBase parent)
            : base(parent)
        {
            m_SliderBar = new SliderBar(this);
            m_SliderBar.Dragged += OnMoved;

            m_Min = 0.0f;
            m_Max = 1.0f;

            m_SnapToNotches = false;
            m_NotchCount = 5;
            m_Value = 0.0f;

            KeyboardInputEnabled = true;
            IsTabable = true;
        }

        /// <summary>
        ///     Number of notches on the slider axis.
        /// </summary>
        public int NotchCount
        {
            get => m_NotchCount;
            set => m_NotchCount = value;
        }

        /// <summary>
        ///     Determines whether the slider should snap to notches.
        /// </summary>
        public bool SnapToNotches
        {
            get => m_SnapToNotches;
            set => m_SnapToNotches = value;
        }

        /// <summary>
        ///     Minimum value.
        /// </summary>
        public float Min
        {
            get => m_Min;
            set => SetRange(value, m_Max);
        }

        /// <summary>
        ///     Maximum value.
        /// </summary>
        public float Max
        {
            get => m_Max;
            set => SetRange(m_Min, value);
        }

        /// <summary>
        ///     Current value.
        /// </summary>
        public float Value
        {
            get => m_Min + (m_Value * (m_Max - m_Min));
            set
            {
                if (value < m_Min)
                {
                    value = m_Min;
                }

                if (value > m_Max)
                {
                    value = m_Max;
                }

                // Normalize Value
                value = (value - m_Min) / (m_Max - m_Min);
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
        protected override bool OnKeyRight(bool down)
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
        protected override bool OnKeyUp(bool down)
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
        protected override bool OnKeyLeft(bool down)
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
        protected override bool OnKeyDown(bool down)
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
        protected override bool OnKeyHome(bool down)
        {
            if (down)
            {
                Value = m_Min;
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
        protected override bool OnKeyEnd(bool down)
        {
            if (down)
            {
                Value = m_Max;
            }

            return true;
        }

        /// <summary>
        ///     Handler invoked on mouse click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="down">If set to <c>true</c> mouse button is down.</param>
        protected override void OnMouseClickedLeft(int x, int y, bool down) {}

        protected virtual void OnMoved(ControlBase control, EventArgs args)
        {
            SetValueInternal(CalculateValue());
        }

        protected virtual float CalculateValue()
        {
            return 0;
        }

        protected virtual void UpdateBarFromValue() {}

        protected virtual void SetValueInternal(float val)
        {
            if (m_SnapToNotches)
            {
                val = (float)Math.Floor((val * m_NotchCount) + 0.5f);
                val /= m_NotchCount;
            }

            if (m_Value != val)
            {
                m_Value = val;

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
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public void SetRange(float min, float max)
        {
            m_Min = min;
            m_Max = max;
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

        protected override Size Measure(Size availableSize)
        {
            m_SliderBar.DoMeasure(availableSize);

            return m_SliderBar.MeasuredSize;
        }

        protected override Size Arrange(Size finalSize)
        {
            m_SliderBar.DoArrange(new Rectangle(Point.Zero, m_SliderBar.MeasuredSize));

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
