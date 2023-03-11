using System;
using Gwen.Net.Input;

namespace Gwen.Net.Control.Internal
{
    public abstract class ButtonBase : ControlBase
    {
        private bool depressed;
        private bool toggleStatus;

        public ButtonBase(ControlBase parent)
            : base(parent)
        {
            MouseInputEnabled = true;
        }

        /// <summary>
        ///     Indicates whether the button is depressed.
        /// </summary>
        public bool IsDepressed
        {
            get => depressed;
            set
            {
                if (depressed == value)
                {
                    return;
                }

                depressed = value;
                Redraw();
            }
        }

        /// <summary>
        ///     Indicates whether the button is toggleable.
        /// </summary>
        public bool IsToggle { get; set; }

        /// <summary>
        ///     Determines the button's toggle state.
        /// </summary>
        public bool ToggleState
        {
            get => toggleStatus;
            set
            {
                if (!IsToggle)
                {
                    return;
                }

                if (toggleStatus == value)
                {
                    return;
                }

                toggleStatus = value;

                if (Toggled != null && !IsDisabled)
                {
                    Toggled.Invoke(this, EventArgs.Empty);
                }

                if (toggleStatus)
                {
                    if (ToggledOn != null && !IsDisabled)
                    {
                        ToggledOn.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    if (ToggledOff != null && !IsDisabled)
                    {
                        ToggledOff.Invoke(this, EventArgs.Empty);
                    }
                }

                Redraw();
            }
        }

        /// <summary>
        ///     Invoked when the button is pressed. Will not be invoked if the button is disabled.
        /// </summary>
        public event GwenEventHandler<EventArgs> Pressed;

        /// <summary>
        ///     Invoked when the button is released. Will not be invoked if the button is disabled.
        /// </summary>
        public event GwenEventHandler<EventArgs> Released;

        /// <summary>
        ///     Invoked when the button's toggle state has changed. Will not be invoked if the button is disabled.
        /// </summary>
        public event GwenEventHandler<EventArgs> Toggled;

        /// <summary>
        ///     Invoked when the button's toggle state has changed to On. Will not be invoked if the button is disabled.
        /// </summary>
        public event GwenEventHandler<EventArgs> ToggledOn;

        /// <summary>
        ///     Invoked when the button's toggle state has changed to Off. Will not be invoked if the button is disabled.
        /// </summary>
        public event GwenEventHandler<EventArgs> ToggledOff;

        /// <summary>
        ///     Toggles the button.
        /// </summary>
        public virtual void Toggle()
        {
            ToggleState = !ToggleState;
        }

        /// <summary>
        ///     "Clicks" the button.
        /// </summary>
        public virtual void Press(ControlBase control = null)
        {
            OnClicked(x: 0, y: 0);
        }

        /// <summary>
        ///     Handler invoked on mouse click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="down">If set to <c>true</c> mouse button is down.</param>
        protected override void OnMouseClickedLeft(int x, int y, bool down)
        {
            if (down)
            {
                IsDepressed = true;
                InputHandler.MouseFocus = this;

                if (Pressed != null && !IsDisabled)
                {
                    Pressed.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                if (IsHovered && depressed)
                {
                    OnClicked(x, y);
                }

                IsDepressed = false;
                InputHandler.MouseFocus = null;
                
                bool IsUnderMouse(Point localMouse) => localMouse is {X: >= 0, Y: >= 0} && localMouse.X < ActualWidth && localMouse.Y < ActualHeight;
                
                if (Released != null && !IsDisabled && IsUnderMouse(CanvasPosToLocal(new Point(x, y))))
                {
                    Released.Invoke(this, EventArgs.Empty);
                }
            }

            Redraw();
        }

        /// <summary>
        ///     Internal OnPressed implementation.
        /// </summary>
        protected virtual void OnClicked(int x, int y)
        {
            if (IsToggle)
            {
                Toggle();
            }

            base.OnMouseClickedLeft(x, y, down: true);
        }

        /// <summary>
        ///     Default accelerator handler.
        /// </summary>
        protected override void OnAccelerator()
        {
            OnClicked(x: 0, y: 0);
        }

        /// <summary>
        ///     Handler invoked on mouse double click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        protected override void OnMouseDoubleClickedLeft(int x, int y)
        {
            base.OnMouseDoubleClickedLeft(x, y);
            OnMouseClickedLeft(x, y, down: true);
        }

        protected override Size Measure(Size availableSize)
        {
            return Size.Zero;
        }

        protected override Size Arrange(Size finalSize)
        {
            return finalSize;
        }
    }
}
