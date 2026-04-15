using System;
using Gwen.Net.Control.Internal;

namespace Gwen.Net.Control
{
    public enum Resizing
    {
        None,
        Width,
        Height,
        Both
    }

    /// <summary>
    ///     Base resizable control.
    /// </summary>
    public class ResizableControl : ContentControl
    {
        private const Int32 ResizerThickness = 6;
        private readonly Resizer[] resizerGroup;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResizableControl" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ResizableControl(ControlBase parent)
            : base(parent)
        {
            this.resizerGroup = new Resizer[8];
            MinimumSize = new Size(width: 5, height: 5);
            ClampMovement = false;

            Resizer resizer;

            resizer = this.resizerGroup[(Int32) ResizerPos.Bottom] = new Resizer(this);
            resizer.ResizeDir = Dock.Bottom;
            resizer.Resized += OnResized;
            resizer.Target = this;

            resizer = this.resizerGroup[(Int32) ResizerPos.LeftBottom] = new Resizer(this);
            resizer.ResizeDir = Dock.Bottom | Dock.Left;
            resizer.Resized += OnResized;
            resizer.Target = this;

            resizer = this.resizerGroup[(Int32) ResizerPos.RightBottom] = new Resizer(this);
            resizer.ResizeDir = Dock.Bottom | Dock.Right;
            resizer.Resized += OnResized;
            resizer.Target = this;

            resizer = this.resizerGroup[(Int32) ResizerPos.Top] = new Resizer(this);
            resizer.ResizeDir = Dock.Top;
            resizer.Resized += OnResized;
            resizer.Target = this;

            resizer = this.resizerGroup[(Int32) ResizerPos.LeftTop] = new Resizer(this);
            resizer.ResizeDir = Dock.Left | Dock.Top;
            resizer.Resized += OnResized;
            resizer.Target = this;

            resizer = this.resizerGroup[(Int32) ResizerPos.RightTop] = new Resizer(this);
            resizer.ResizeDir = Dock.Right | Dock.Top;
            resizer.Resized += OnResized;
            resizer.Target = this;

            resizer = this.resizerGroup[(Int32) ResizerPos.Left] = new Resizer(this);
            resizer.ResizeDir = Dock.Left;
            resizer.Resized += OnResized;
            resizer.Target = this;

            resizer = this.resizerGroup[(Int32) ResizerPos.Right] = new Resizer(this);
            resizer.ResizeDir = Dock.Right;
            resizer.Resized += OnResized;
            resizer.Target = this;
        }

        /// <summary>
        ///     Enable or disable resizing.
        /// </summary>
        public Resizing Resizing
        {
            get
            {
                if (GetResizer(ResizerPos.Right).IsCollapsed)
                {
                    if (GetResizer(ResizerPos.Bottom).IsCollapsed)
                    {
                        return Resizing.None;
                    }

                    return Resizing.Height;
                }

                if (GetResizer(ResizerPos.Bottom).IsCollapsed)
                {
                    return Resizing.Width;
                }

                return Resizing.Both;
            }
            set
            {
                switch (value)
                {
                    case Resizing.None:
                        EnableResizing(left: false, top: false, right: false, bottom: false);

                        break;
                    case Resizing.Width:
                        EnableResizing(left: true, top: false, right: true, bottom: false);

                        break;
                    case Resizing.Height:
                        EnableResizing(left: false, top: true, right: false);

                        break;
                    case Resizing.Both:
                        EnableResizing();

                        break;
                }
            }
        }

        /// <summary>
        ///     Determines whether control's position should be restricted to its parent bounds.
        /// </summary>
        public Boolean ClampMovement { get; set; }

        /// <summary>
        ///     Invoked when the control has been resized.
        /// </summary>
        public event GwenEventHandler<EventArgs> Resized;

        /// <summary>
        ///     Handler for the resized event.
        /// </summary>
        /// <param name="control">Event source.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnResized(ControlBase control, EventArgs args)
        {
            if (Resized != null)
            {
                Resized.Invoke(this, EventArgs.Empty);
            }
        }

        protected Resizer GetResizer(ResizerPos resizerPos)
        {
            return resizerGroup[(Int32) resizerPos];
        }

        /// <summary>
        ///     Enable or disable resizing.
        /// </summary>
        /// <param name="left">Is resizing left edge enabled.</param>
        /// <param name="top">Is resizing top edge enabled.</param>
        /// <param name="right">Is resizing right edge enabled.</param>
        /// <param name="bottom">Is resizing bottom edge enabled.</param>
        public virtual void EnableResizing(Boolean left = true, Boolean top = true, Boolean right = true, Boolean bottom = true)
        {
            var d = new Boolean[8];

            if (!left)
            {
                d[(Int32) ResizerPos.Left] = d[(Int32) ResizerPos.LeftTop] = d[(Int32) ResizerPos.LeftBottom] = true;
            }

            if (!top)
            {
                d[(Int32) ResizerPos.Top] = d[(Int32) ResizerPos.LeftTop] = d[(Int32) ResizerPos.RightTop] = true;
            }

            if (!right)
            {
                d[(Int32) ResizerPos.Right] = d[(Int32) ResizerPos.RightTop] = d[(Int32) ResizerPos.RightBottom] = true;
            }

            if (!bottom)
            {
                d[(Int32) ResizerPos.Bottom] = d[(Int32) ResizerPos.LeftBottom] = d[(Int32) ResizerPos.RightBottom] = true;
            }

            for (var i = 0; i < 8; i++)
            {
                if (d[i])
                {
                    resizerGroup[i].MouseInputEnabled = false;
                    resizerGroup[i].Collapse(collapsed: true, measure: false);
                }
                else
                {
                    resizerGroup[i].MouseInputEnabled = true;
                    resizerGroup[i].Collapse(collapsed: false, measure: false);
                }
            }

            Invalidate();
        }

        /// <summary>
        ///     Sets the control bounds.
        /// </summary>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <returns>
        ///     True if bounds changed.
        /// </returns>
        public override Boolean SetBounds(Int32 x, Int32 y, Int32 width, Int32 height)
        {
            width = Util.Clamp(width, MinimumSize.Width, MaximumSize.Width);
            height = Util.Clamp(height, MinimumSize.Height, MaximumSize.Height);

            // Clamp to parent's window
            ControlBase parent = Parent;

            if (parent != null && ClampMovement)
            {
                if (x + width > parent.ActualWidth)
                {
                    x = parent.ActualWidth - width;
                }

                if (x < 0)
                {
                    x = 0;
                }

                if (y + height > parent.ActualHeight)
                {
                    y = parent.ActualHeight - height;
                }

                if (y < 0)
                {
                    y = 0;
                }
            }

            return base.SetBounds(x, y, width, height);
        }

        /// <summary>
        ///     Sets the control size.
        /// </summary>
        /// <param name="width">New width.</param>
        /// <param name="height">New height.</param>
        /// <returns>True if bounds changed.</returns>
        public override Boolean SetSize(Int32 width, Int32 height)
        {
            Boolean changed = base.SetSize(width, height);

            if (changed)
            {
                OnResized(this, EventArgs.Empty);
            }

            return changed;
        }

        protected override Size Measure(Size availableSize)
        {
            resizerGroup[(Int32) ResizerPos.Left]
                .DoMeasure(new Size(ResizerThickness, availableSize.Height - (2 * ResizerThickness)));

            resizerGroup[(Int32) ResizerPos.LeftTop].DoMeasure(new Size(ResizerThickness, ResizerThickness));

            resizerGroup[(Int32) ResizerPos.Top]
                .DoMeasure(new Size(availableSize.Width - (2 * ResizerThickness), ResizerThickness));

            resizerGroup[(Int32) ResizerPos.RightTop].DoMeasure(new Size(ResizerThickness, ResizerThickness));

            resizerGroup[(Int32) ResizerPos.Right]
                .DoMeasure(new Size(ResizerThickness, availableSize.Height - (2 * ResizerThickness)));

            resizerGroup[(Int32) ResizerPos.RightBottom].DoMeasure(new Size(ResizerThickness, ResizerThickness));

            resizerGroup[(Int32) ResizerPos.Bottom]
                .DoMeasure(new Size(availableSize.Width - (2 * ResizerThickness), ResizerThickness));

            resizerGroup[(Int32) ResizerPos.LeftBottom].DoMeasure(new Size(ResizerThickness, ResizerThickness));

            return availableSize;
        }

        protected override Size Arrange(Size finalSize)
        {
            resizerGroup[(Int32) ResizerPos.Left].DoArrange(
                new Rectangle(x: 0, ResizerThickness, ResizerThickness, finalSize.Height - (2 * ResizerThickness)));

            resizerGroup[(Int32) ResizerPos.LeftTop]
                .DoArrange(new Rectangle(x: 0, y: 0, ResizerThickness, ResizerThickness));

            resizerGroup[(Int32) ResizerPos.Top].DoArrange(
                new Rectangle(ResizerThickness, y: 0, finalSize.Width - (2 * ResizerThickness), ResizerThickness));

            resizerGroup[(Int32) ResizerPos.RightTop].DoArrange(
                new Rectangle(finalSize.Width - ResizerThickness, y: 0, ResizerThickness, ResizerThickness));

            resizerGroup[(Int32) ResizerPos.Right].DoArrange(
                new Rectangle(
                    finalSize.Width - ResizerThickness,
                    ResizerThickness,
                    ResizerThickness,
                    finalSize.Height - (2 * ResizerThickness)));

            resizerGroup[(Int32) ResizerPos.RightBottom].DoArrange(
                new Rectangle(
                    finalSize.Width - ResizerThickness,
                    finalSize.Height - ResizerThickness,
                    ResizerThickness,
                    ResizerThickness));

            resizerGroup[(Int32) ResizerPos.Bottom].DoArrange(
                new Rectangle(
                    ResizerThickness,
                    finalSize.Height - ResizerThickness,
                    finalSize.Width - (2 * ResizerThickness),
                    ResizerThickness));

            resizerGroup[(Int32) ResizerPos.LeftBottom].DoArrange(
                new Rectangle(x: 0, finalSize.Height - ResizerThickness, ResizerThickness, ResizerThickness));

            return finalSize;
        }

        protected enum ResizerPos
        {
            Left,
            LeftTop,
            Top,
            RightTop,
            Right,
            RightBottom,
            Bottom,
            LeftBottom
        }
    }
}
