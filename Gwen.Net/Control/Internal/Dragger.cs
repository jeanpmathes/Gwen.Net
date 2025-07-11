﻿using System;
using Gwen.Net.Input;
using Gwen.Net.Skin;

namespace Gwen.Net.Control.Internal
{
    /// <summary>
    ///     Base for controls that can be dragged by mouse.
    /// </summary>
    public class Dragger : ControlBase
    {
        private Boolean held;
        
        protected Point holdPos;
        protected ControlBase target;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Dragger" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Dragger(ControlBase parent) : base(parent)
        {
            MouseInputEnabled = true;
            held = false;
        }

        internal ControlBase Target
        {
            get => target;
            set => target = value;
        }

        /// <summary>
        ///     Indicates if the control is being dragged.
        /// </summary>
        public Boolean IsHeld => held && !holdIgnore;
        
        private Boolean holdIgnore;
        
        /// <summary>
        /// Set whether the control should ignore the hold state.
        /// </summary>
        /// <param name="ignore">Whether to ignore the hold state.</param>
        internal void SetHoldIgnore(Boolean ignore)
        {
            holdIgnore = ignore;
        }

        /// <summary>
        ///     Event invoked when the control position has been changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> Dragged;

        /// <summary>
        ///     Handler invoked on mouse click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="down">If set to <c>true</c> mouse button is down.</param>
        protected override void OnMouseClickedLeft(Int32 x, Int32 y, Boolean down)
        {
            if (null == target)
            {
                return;
            }

            if (down)
            {
                held = true;
                holdPos = target.CanvasPosToLocal(new Point(x, y));
                InputHandler.MouseFocus = this;
            }
            else
            {
                held = false;

                InputHandler.MouseFocus = null;
            }
        }

        /// <summary>
        ///     Handler invoked on mouse moved event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="dx">X change.</param>
        /// <param name="dy">Y change.</param>
        protected override void OnMouseMoved(Int32 x, Int32 y, Int32 dx, Int32 dy)
        {
            if (null == target)
            {
                return;
            }

            if (!IsHeld)
            {
                return;
            }

            Point p = new(x - holdPos.X, y - holdPos.Y);

            // Translate to parent
            if (target.Parent != null)
            {
                p = target.Parent.CanvasPosToLocal(p);
            }
            
            target.MoveTo(p.X, p.Y);

            if (Dragged != null)
            {
                Dragged.Invoke(this, EventArgs.Empty);
            }
        }

        protected override Size Measure(Size availableSize)
        {
            return availableSize;
        }

        protected override Size Arrange(Size finalSize)
        {
            return finalSize;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            // Intentionally left empty.
        }
    }
}
