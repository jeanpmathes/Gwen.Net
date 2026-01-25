using System;

namespace Gwen.Net.Legacy.Control.Internal
{
    /// <summary>
    ///     Grab point for resizing.
    /// </summary>
    public class Resizer : Dragger
    {
        private Dock resizeDir;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Resizer" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Resizer(ControlBase parent)
            : base(parent)
        {
            resizeDir = Dock.Left;
            MouseInputEnabled = true;
            Target = parent;
        }

        /// <summary>
        ///     Gets or sets the sizing direction.
        /// </summary>
        public Dock ResizeDir
        {
            set
            {
                resizeDir = value;

                if ((0 != (value & Dock.Left) && 0 != (value & Dock.Top)) ||
                    (0 != (value & Dock.Right) && 0 != (value & Dock.Bottom)))
                {
                    Cursor = Cursor.SizeNWSE;

                    return;
                }

                if ((0 != (value & Dock.Right) && 0 != (value & Dock.Top)) ||
                    (0 != (value & Dock.Left) && 0 != (value & Dock.Bottom)))
                {
                    Cursor = Cursor.SizeNESW;

                    return;
                }

                if (0 != (value & Dock.Right) || 0 != (value & Dock.Left))
                {
                    Cursor = Cursor.SizeWE;

                    return;
                }

                if (0 != (value & Dock.Top) || 0 != (value & Dock.Bottom))
                {
                    Cursor = Cursor.SizeNS;
                }
            }
        }

        /// <summary>
        ///     Invoked when the control has been resized.
        /// </summary>
        public event GwenEventHandler<EventArgs> Resized;

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

            Rectangle oldBounds = target.Bounds;
            Rectangle bounds = target.Bounds;
            
            // If the desired bounds are not set to the current bounds, setting the bounds below can cause one of the dimensions to be set to 0.
            // This will cause the control to jump around when resizing.
            // Maybe there is a better way to fix this, but this works for now.
            target.DesiredBounds = bounds;

            Size min = target.MinimumSize;

            Point delta = target.LocalPosToCanvas(holdPos);
            delta.X -= x;
            delta.Y -= y;

            if (0 != (resizeDir & Dock.Left))
            {
                bounds.X -= delta.X;
                bounds.Width += delta.X;

                if (bounds.X < 0)
                {
                    bounds.Width += bounds.X;
                    bounds.X = 0;
                }

                // Conform to minimum size here so we don't go all weird when we snap it in the base control.

                if (bounds.Width < min.Width)
                {
                    Int32 diff = min.Width - bounds.Width;
                    bounds.Width += diff;
                    bounds.X -= diff;
                }

                target.Left = bounds.Left;
                target.Width = bounds.Width;
            }

            if (0 != (resizeDir & Dock.Top))
            {
                bounds.Y -= delta.Y;
                bounds.Height += delta.Y;

                if (bounds.Y < 0)
                {
                    bounds.Height += bounds.Y;
                    bounds.Y = 0;
                }

                // Conform to minimum size here so we don't go all weird when we snap it in the base control.
                
                if (bounds.Height < min.Height)
                {
                    Int32 diff = min.Height - bounds.Height;
                    bounds.Height += diff;
                    bounds.Y -= diff;
                }

                target.Top = bounds.Top;
                target.Height = bounds.Height;
            }

            if (0 != (resizeDir & Dock.Right))
            {
                bounds.Width -= delta.X;

                if (bounds.Width < min.Width)
                {
                    bounds.Width = min.Width;
                }

                holdPos.X += bounds.Width - oldBounds.Width;

                target.Left = bounds.Left;
                target.Width = bounds.Width;
            }

            if (0 != (resizeDir & Dock.Bottom))
            {
                bounds.Height -= delta.Y;

                if (bounds.Height < min.Height)
                {
                    bounds.Height = min.Height;
                }

                holdPos.Y += bounds.Height - oldBounds.Height;

                target.Top = bounds.Top;
                target.Height = bounds.Height;
            }

            // Lets set quickly new bounds and let the layout measure and arrange child controls later.
            target.SetBounds(bounds);

            // Set bounds that are checked by SetBounds() implementations.
            if (!Util.IsIgnore(target.Width))
            {
                target.Width = target.Bounds.Width;
            }

            if (!Util.IsIgnore(target.Height))
            {
                target.Height = target.Bounds.Height;
            }

            target.Invalidate();

            if (Resized != null)
            {
                Resized.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
