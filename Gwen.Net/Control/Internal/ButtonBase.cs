using System;
using Gwen.Net.Input;
using Gwen.Net.Xml;

namespace Gwen.Net.Control.Internal
{
    public abstract class ButtonBase : ControlBase
    {
        private bool m_Depressed;
        private bool m_ToggleStatus;

        public ButtonBase(ControlBase parent)
            : base(parent)
        {
            MouseInputEnabled = true;
        }

        /// <summary>
        ///     Indicates whether the button is depressed.
        /// </summary>
        [XmlProperty] public bool IsDepressed
        {
            get => m_Depressed;
            set
            {
                if (m_Depressed == value)
                {
                    return;
                }

                m_Depressed = value;
                Redraw();
            }
        }

        /// <summary>
        ///     Indicates whether the button is toggleable.
        /// </summary>
        [XmlProperty] public bool IsToggle { get; set; }

        /// <summary>
        ///     Determines the button's toggle state.
        /// </summary>
        [XmlProperty] public bool ToggleState
        {
            get => m_ToggleStatus;
            set
            {
                if (!IsToggle)
                {
                    return;
                }

                if (m_ToggleStatus == value)
                {
                    return;
                }

                m_ToggleStatus = value;

                if (Toggled != null)
                {
                    Toggled.Invoke(this, EventArgs.Empty);
                }

                if (m_ToggleStatus)
                {
                    if (ToggledOn != null)
                    {
                        ToggledOn.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    if (ToggledOff != null)
                    {
                        ToggledOff.Invoke(this, EventArgs.Empty);
                    }
                }

                Redraw();
            }
        }

        /// <summary>
        ///     Invoked when the button is pressed.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> Pressed;

        /// <summary>
        ///     Invoked when the button is released.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> Released;

        /// <summary>
        ///     Invoked when the button's toggle state has changed.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> Toggled;

        /// <summary>
        ///     Invoked when the button's toggle state has changed to On.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> ToggledOn;

        /// <summary>
        ///     Invoked when the button's toggle state has changed to Off.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> ToggledOff;

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
            //base.OnMouseClickedLeft(x, y, down);
            if (down)
            {
                IsDepressed = true;
                InputHandler.MouseFocus = this;

                if (Pressed != null)
                {
                    Pressed.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                if (IsHovered && m_Depressed)
                {
                    OnClicked(x, y);
                }

                IsDepressed = false;
                InputHandler.MouseFocus = null;

                if (Released != null)
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