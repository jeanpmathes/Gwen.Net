﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Gwen.Net.DragDrop;
using Gwen.Net.Input;
using Gwen.Net.Renderer;
using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Canvas control. It should be the root parent for all other controls.
    /// </summary>
    public class Canvas : ControlBase
    {
        private readonly List<IDisposable> disposeQueue; // dictionary for faster access?

        private readonly HashSet<ControlBase> measureQueue = new();

        // [omeg] these are not created by us, so no disposing
        internal ControlBase firstTab;

        private Single scale;
        internal ControlBase nextTab;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Canvas" /> class.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        public Canvas(SkinBase skin) : base(parent: null) // Is OK because skin is set.
        {
            Dock = Dock.Fill;
            SetBounds(x: 0, y: 0, width: 10000, height: 10000);
            SetSkin(skin);
            Scale = 1.0f;
            BackgroundColor = Color.White;
            ShouldDrawBackground = false;

            disposeQueue = new List<IDisposable>();
        }

        /// <summary>
        ///     Scale for rendering.
        /// </summary>
        public override Single Scale
        {
            get => scale;
            set
            {
                if (scale == value)
                {
                    return;
                }

                scale = value;

                if (Skin != null && Skin.Renderer != null)
                {
                    Skin.Renderer.Scale = scale;
                }

                OnScaleChanged();
                Redraw();
                Invalidate();
            }
        }

        /// <summary>
        ///     Background color.
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        ///     In most situations you will be rendering the canvas every frame.
        ///     But in some situations you will only want to render when there have been changes.
        ///     You can do this by checking NeedsRedraw.
        /// </summary>
        public Boolean NeedsRedraw { get; set; }

        public override void Dispose()
        {
            ProcessDelayedDeletes();

            // Dispose all cached fonts.
            FontCache.FreeCache();

            base.Dispose();
        }

        /// <summary>
        ///     Re-renders the control, invalidates cached texture.
        /// </summary>
        public override void Redraw()
        {
            NeedsRedraw = true;
            base.Redraw();
        }

        // Children call parent.GetCanvas() until they get to 
        // this top level function.
        public override Canvas GetCanvas()
        {
            return this;
        }

        /// <summary>
        ///     Renders the canvas. Call in your rendering loop.
        /// </summary>
        public void RenderCanvas()
        {
            DoThink();

            SkinBase skin = Skin;
            RendererBase render = skin.Renderer;

            render.Begin();

            render.ClipRegion = Bounds;
            render.RenderOffset = Point.Zero;

            if (ShouldDrawBackground)
            {
                render.DrawColor = BackgroundColor;
                render.DrawFilledRect(RenderBounds);
            }

            DoRender(skin);

            DragAndDrop.RenderOverlay(this, skin);

            Net.ToolTip.RenderToolTip(skin);

            render.EndClip();

            render.End();
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            base.Render(currentSkin);
            NeedsRedraw = false;
        }

        /// <summary>
        ///     Handler invoked when control's bounds change.
        /// </summary>
        /// <param name="oldBounds">Old bounds.</param>
        protected override void OnBoundsChanged(Rectangle oldBounds)
        {
            base.OnBoundsChanged(oldBounds);
            Invalidate();
        }

        /// <summary>
        ///     Processes input and layout. Also purges delayed delete queue.
        /// </summary>
        private void DoThink()
        {
            if (IsHidden || IsCollapsed)
            {
                return;
            }

            // Reset tabbing
            nextTab = null;
            firstTab = null;

            ProcessDelayedDeletes();

            // Check has focus etc..
            RecurseControls();

            // If we didn't have a next tab, cycle to the start.
            if (nextTab == null)
            {
                nextTab = firstTab;
            }

            InputHandler.OnCanvasThink(this);

            // Update timers
            Timer.Tick();

            // Is total layout needed
            if (NeedsLayout)
            {
                DoLayout();
            }
            
            ISet<ControlBase> measured = new HashSet<ControlBase>();

            // Check if individual controls need layout
            while (measureQueue.Count > 0)
            {
                ControlBase element = measureQueue.First();
                measureQueue.Remove(element);
                Boolean added = measured.Add(element);
                
                if (added) element.DoLayout();
            }
        }

        /// <summary>
        ///     Adds given control to the delete queue and detaches it from canvas. Don't call from Dispose, it modifies child
        ///     list.
        /// </summary>
        /// <param name="control">Control to delete.</param>
        public void AddDelayedDelete(ControlBase control)
        {
            if (!disposeQueue.Contains(control))
            {
                disposeQueue.Add(control);
                RemoveChild(control, dispose: false);
            }
            else
            {
                Debug.Fail("Control deleted twice!");
            }
        }

        private void ProcessDelayedDeletes()
        {
            //if (disposeQueue.Count > 0)
            //    System.Diagnostics.Debug.Print("Canvas.ProcessDelayedDeletes: {0} items", disposeQueue.Count);
            foreach (IDisposable control in disposeQueue)
            {
                control.Dispose();
            }

            disposeQueue.Clear();
        }

        public void AddToMeasure(ControlBase element)
        {
            measureQueue.Add(element);
        }

        /// <summary>
        ///     Handles mouse movement events. Called from Input subsystems.
        /// </summary>
        /// <returns>True if handled.</returns>
        public Boolean Input_MouseMoved(Int32 x, Int32 y, Int32 dx, Int32 dy)
        {
            if (IsHidden || IsCollapsed)
            {
                return false;
            }

            // Todo: Handle scaling here..
            //float fScale = 1.0f / Scale();

            return InputHandler.OnMouseMoved(this, x, y, dx, dy);
        }

        /// <summary>
        ///     Handles mouse button events. Called from Input subsystems.
        /// </summary>
        /// <returns>True if handled.</returns>
        public Boolean Input_MouseButton(Int32 button, Boolean down)
        {
            if (IsHidden || IsCollapsed)
            {
                return false;
            }

            return InputHandler.OnMouseClicked(this, button, down);
        }

        /// <summary>
        ///     Handles keyboard events. Called from Input subsystems.
        /// </summary>
        /// <returns>True if handled.</returns>
        public Boolean Input_Key(GwenMappedKey key, Boolean down)
        {
            if (IsHidden || IsCollapsed)
            {
                return false;
            }

            if (key <= GwenMappedKey.Invalid)
            {
                return false;
            }

            if (key >= GwenMappedKey.Count)
            {
                return false;
            }

            return InputHandler.OnKeyEvent(this, key, down);
        }

        /// <summary>
        ///     Handles keyboard events. Called from Input subsystems.
        /// </summary>
        /// <returns>True if handled.</returns>
        public Boolean Input_Character(Char chr)
        {
            if (IsHidden || IsCollapsed)
            {
                return false;
            }

            if (Char.IsControl(chr))
            {
                return false;
            }

            //Handle Accelerators
            if (InputHandler.HandleAccelerator(this, chr))
            {
                return true;
            }

            //Handle characters
            if (InputHandler.KeyboardFocus == null)
            {
                return false;
            }

            if (InputHandler.KeyboardFocus.GetCanvas() != this)
            {
                return false;
            }

            if (!InputHandler.KeyboardFocus.IsVisible)
            {
                return false;
            }

            if (InputHandler.IsControlDown)
            {
                return false;
            }

            return InputHandler.KeyboardFocus.InputChar(chr);
        }

        /// <summary>
        ///     Handles the mouse wheel events. Called from Input subsystems.
        /// </summary>
        /// <returns>True if handled.</returns>
        public Boolean Input_MouseWheel(Int32 val)
        {
            if (IsHidden || IsCollapsed)
            {
                return false;
            }

            if (InputHandler.HoveredControl == null)
            {
                return false;
            }

            if (InputHandler.HoveredControl == this)
            {
                return false;
            }

            if (InputHandler.HoveredControl.GetCanvas() != this)
            {
                return false;
            }

            return InputHandler.HoveredControl.InputMouseWheeled(val);
        }
    }
}
