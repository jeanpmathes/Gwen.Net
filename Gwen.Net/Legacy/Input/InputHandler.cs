using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Gwen.Net.Legacy.Control;
using Gwen.Net.Legacy.DragDrop;
using Gwen.Net.Legacy.Platform;

namespace Gwen.Net.Legacy.Input
{
    /// <summary>
    ///     Input handling.
    /// </summary>
    public static class InputHandler
    {
        private static readonly KeyData keyData = new();
        private static readonly Single[] lastClickTime = new Single[MaxMouseButtons];
        private static Point lastClickPos;
        private static ControlBase hoveredControl;

        /// <summary>
        ///     Control that currently has keyboard focus.
        /// </summary>
        public static ControlBase KeyboardFocus { get; set; }

        /// <summary>
        ///     Control that currently has mouse focus.
        /// </summary>
        public static ControlBase? MouseFocus { get; set; }

        /// <summary>
        ///     Current mouse position.
        /// </summary>
        public static Point MousePosition { get; set; } // not property to allow modification of Point fields

        /// <summary>
        ///     Control currently hovered by mouse.
        /// </summary>
        public static ControlBase HoveredControl
        {
            get => hoveredControl;
            set
            {
                if (value is null)
                {
                    Debug.Write("Clearing Hover");
                }

                hoveredControl = value;
            }
        }

        /// <summary>
        ///     Maximum number of mouse buttons supported.
        /// </summary>
        public static Int32 MaxMouseButtons => 5;

        /// <summary>
        ///     Maximum time in seconds between mouse clicks to be recognized as double click.
        /// </summary>
        public static Single DoubleClickSpeed => 0.5f;

        /// <summary>
        ///     Time in seconds between autorepeating of keys.
        /// </summary>
        public static Single KeyRepeatRate => 0.03f;

        /// <summary>
        ///     Time in seconds before key starts to autorepeat.
        /// </summary>
        public static Single KeyRepeatDelay => 0.5f;

        /// <summary>
        ///     Indicates whether the left mouse button is down.
        /// </summary>
        public static Boolean IsLeftMouseDown => keyData.LeftMouseDown;

        /// <summary>
        ///     Indicates whether the right mouse button is down.
        /// </summary>
        public static Boolean IsRightMouseDown => keyData.RightMouseDown;

        /// <summary>
        ///     Indicates whether the shift key is down.
        /// </summary>
        public static Boolean IsShiftDown => IsKeyDown(GwenMappedKey.Shift);

        /// <summary>
        ///     Indicates whether the control key is down.
        /// </summary>
        public static Boolean IsControlDown => IsKeyDown(GwenMappedKey.Control);

        /// <summary>
        ///     Checks if the given key is pressed.
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <returns>True if the key is down.</returns>
        public static Boolean IsKeyDown(GwenMappedKey key)
        {
            return keyData.KeyState[(Int32) key];
        }

        /// <summary>
        ///     Handles copy, paste etc.
        /// </summary>
        /// <param name="canvas">Canvas.</param>
        /// <param name="chr">Input character.</param>
        /// <returns>True if the key was handled.</returns>
        public static Boolean DoSpecialKeys(ControlBase canvas, Char chr)
        {
            if (null == KeyboardFocus)
            {
                return false;
            }

            if (KeyboardFocus.GetCanvas() != canvas)
            {
                return false;
            }

            if (!KeyboardFocus.IsVisible)
            {
                return false;
            }

            if (!IsControlDown)
            {
                return false;
            }

            if (chr == 'C' || chr == 'c')
            {
                KeyboardFocus.InputCopy(from: null);

                return true;
            }

            if (chr == 'V' || chr == 'v')
            {
                KeyboardFocus.InputPaste(from: null);

                return true;
            }

            if (chr == 'X' || chr == 'x')
            {
                KeyboardFocus.InputCut(from: null);

                return true;
            }

            if (chr == 'A' || chr == 'a')
            {
                KeyboardFocus.InputSelectAll(from: null);

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Handles accelerator input.
        /// </summary>
        /// <param name="canvas">Canvas.</param>
        /// <param name="chr">Input character.</param>
        /// <returns>True if the key was handled.</returns>
        public static Boolean HandleAccelerator(ControlBase canvas, Char chr)
        {
            //Build the accelerator search string
            StringBuilder accelString = new();

            if (IsControlDown)
            {
                accelString.Append("CTRL+");
            }

            if (IsShiftDown)
            {
                accelString.Append("SHIFT+");
            }
            // [omeg] todo: alt?

            accelString.Append(chr);
            var acc = accelString.ToString();

            if (KeyboardFocus != null && KeyboardFocus.HandleAccelerator(acc))
            {
                return true;
            }

            if (MouseFocus != null && MouseFocus.HandleAccelerator(acc))
            {
                return true;
            }

            if (canvas.HandleAccelerator(acc))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Handles focus updating and key autorepeats.
        /// </summary>
        /// <param name="control">Unused.</param>
        public static void OnCanvasThink(ControlBase control)
        {
            if (MouseFocus != null && !MouseFocus.IsVisible)
            {
                MouseFocus = null;
            }

            if (KeyboardFocus != null && (!KeyboardFocus.IsVisible || !KeyboardFocus.KeyboardInputEnabled))
            {
                KeyboardFocus = null;
            }

            if (null == KeyboardFocus)
            {
                return;
            }

            if (KeyboardFocus.GetCanvas() != control)
            {
                return;
            }

            Single time = GwenPlatform.GetTimeInSeconds();

            //
            // Simulate Key-Repeats
            //
            for (var i = 0; i < (Int32) GwenMappedKey.Count; i++)
            {
                if (keyData.KeyState[i] && keyData.Target != KeyboardFocus)
                {
                    keyData.KeyState[i] = false;

                    continue;
                }

                if (keyData.KeyState[i] && time > keyData.NextRepeat[i])
                {
                    keyData.NextRepeat[i] = GwenPlatform.GetTimeInSeconds() + KeyRepeatRate;

                    if (KeyboardFocus != null)
                    {
                        KeyboardFocus.InputKeyPressed((GwenMappedKey) i);
                    }
                }
            }
        }

        /// <summary>
        ///     Mouse moved handler.
        /// </summary>
        /// <param name="canvas">Canvas.</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns>True if handled.</returns>
        public static Boolean OnMouseMoved(ControlBase canvas, Int32 x, Int32 y, Int32 dx, Int32 dy)
        {
            // Send input to canvas for study
            MousePosition = new Point(x, y);

            UpdateHoveredControl(canvas);

            if (HoveredControl == null)
            {
                return false;
            }

            if (HoveredControl == canvas)
            {
                return false;
            }

            if (HoveredControl.GetCanvas() != canvas)
            {
                return false;
            }

            HoveredControl.InputMouseMoved(x, y, dx, dy);
            HoveredControl.UpdateCursor();

            DragAndDrop.OnMouseMoved(HoveredControl, x, y);

            return true;
        }

        /// <summary>
        ///     Mouse click handler.
        /// </summary>
        /// <param name="canvas">Canvas.</param>
        /// <param name="mouseButton">Mouse button number.</param>
        /// <param name="down">Specifies if the button is down.</param>
        /// <returns>True if handled.</returns>
        public static Boolean OnMouseClicked(ControlBase canvas, Int32 mouseButton, Boolean down)
        {
            // If we click on a control that isn't a menu we want to close
            // all the open menus. Menus are children of the canvas.
            if (down && (null == HoveredControl || !HoveredControl.IsMenuComponent))
            {
                canvas.CloseMenus();
            }

            if (null == HoveredControl) return false;
            
            if (HoveredControl.GetCanvas() != canvas) return false;
            
            if (!HoveredControl.IsVisible) return false;
            
            if (HoveredControl == canvas) return false;
            
            if (mouseButton > MaxMouseButtons) return false;
       
            if (mouseButton == 0)
            {
                keyData.LeftMouseDown = down;
            }
            else if (mouseButton == 1)
            {
                keyData.RightMouseDown = down;
            }
            
            // todo: Shouldn't double click if mouse has moved significantly
            var isDoubleClick = false;

            if (down &&
                lastClickPos.X == MousePosition.X &&
                lastClickPos.Y == MousePosition.Y &&
                GwenPlatform.GetTimeInSeconds() - lastClickTime[mouseButton] < DoubleClickSpeed)
            {
                isDoubleClick = true;
            }

            if (down && !isDoubleClick)
            {
                lastClickTime[mouseButton] = GwenPlatform.GetTimeInSeconds();
                lastClickPos = MousePosition;
            }

            if (down)
            {
                FindKeyboardFocus(HoveredControl);
            }

            HoveredControl.UpdateCursor();

            // This tells the child it has been touched, which
            // in turn tells its parents, who tell their parents.
            // This is basically so that Windows can pop themselves
            // to the top when one of their children have been clicked.
            if (down)
            {
                HoveredControl.Touch();
            }

            switch (mouseButton)
            {
                case 0:
                {
                    if (DragAndDrop.OnMouseButton(HoveredControl, MousePosition.X, MousePosition.Y, down))
                    {
                        return true;
                    }

                    if (isDoubleClick)
                    {
                        HoveredControl.InputMouseDoubleClickedLeft(MousePosition.X, MousePosition.Y);
                    }
                    else
                    {
                        HoveredControl.InputMouseClickedLeft(MousePosition.X, MousePosition.Y, down);
                    }

                    return true;
                }

                case 1:
                {
                    if (isDoubleClick)
                    {
                        HoveredControl.InputMouseDoubleClickedRight(MousePosition.X, MousePosition.Y);
                    }
                    else
                    {
                        HoveredControl.InputMouseClickedRight(MousePosition.X, MousePosition.Y, down);
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Key handler.
        /// </summary>
        /// <param name="canvas">Canvas.</param>
        /// <param name="key">Key.</param>
        /// <param name="down">True if the key is down.</param>
        /// <returns>True if handled.</returns>
        public static Boolean OnKeyEvent(ControlBase canvas, GwenMappedKey key, Boolean down)
        {
            if (null == KeyboardFocus)
            {
                return false;
            }

            if (KeyboardFocus.GetCanvas() != canvas)
            {
                return false;
            }

            if (!KeyboardFocus.IsVisible)
            {
                return false;
            }

            var iKey = (Int32) key;

            if (down)
            {
                if (!keyData.KeyState[iKey])
                {
                    keyData.KeyState[iKey] = true;
                    keyData.NextRepeat[iKey] = GwenPlatform.GetTimeInSeconds() + KeyRepeatDelay;
                    keyData.Target = KeyboardFocus;

                    return KeyboardFocus.InputKeyPressed(key);
                }
            }
            else
            {
                if (keyData.KeyState[iKey])
                {
                    keyData.KeyState[iKey] = false;

                    // BUG BUG. This causes shift left arrow in textboxes
                    // to not work. What is disabling it here breaking?
                    //keyData.Target = NULL;

                    return KeyboardFocus.InputKeyPressed(key, down: false);
                }
            }

            return false;
        }

        private static void UpdateHoveredControl(ControlBase inCanvas)
        {
            ControlBase hovered = inCanvas.GetControlAt(MousePosition.X, MousePosition.Y);

            if (hovered != HoveredControl)
            {
                if (HoveredControl != null)
                {
                    ControlBase oldHover = HoveredControl;
                    HoveredControl = null;
                    oldHover.InputMouseLeft();
                }

                HoveredControl = hovered;

                if (HoveredControl != null)
                {
                    HoveredControl.InputMouseEntered();
                }
            }

            if (MouseFocus != null && MouseFocus.GetCanvas() == inCanvas)
            {
                if (HoveredControl != null)
                {
                    ControlBase oldHover = HoveredControl;
                    HoveredControl = null;
                    oldHover.Redraw();
                }

                HoveredControl = MouseFocus;
            }
        }

        private static void FindKeyboardFocus(ControlBase control)
        {
            if (null == control)
            {
                return;
            }

            if (control.KeyboardInputEnabled)
            {
                //Make sure none of our children have keyboard focus first - todo: recursive
                if (control.Children.Any(child => child == KeyboardFocus))
                {
                    return;
                }

                control.Focus();

                return;
            }

            FindKeyboardFocus(control.Parent);
        }
    }
}
