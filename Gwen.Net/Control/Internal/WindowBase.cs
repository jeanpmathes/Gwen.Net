using System;
using System.Linq;

namespace Gwen.Net.Control
{
    public enum StartPosition
    {
        CenterParent,
        CenterCanvas,
        Manual
    }
}

namespace Gwen.Net.Control.Internal
{
    public abstract class WindowBase : ResizableControl
    {
        private readonly ControlBase realParent;
        protected Dragger dragBar;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WindowBase" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        protected WindowBase(ControlBase parent)
            : base(parent.GetCanvas())
        {
            realParent = parent;

            EnableResizing();
            BringToFront();
            IsTabable = false;
            Focus();
            MinimumSize = new Size(width: 100, height: 40);
            ClampMovement = true;
            KeyboardInputEnabled = false;
            MouseInputEnabled = true;
        }

        /// <summary>
        ///     Is window draggable.
        /// </summary>
        public Boolean IsDraggingEnabled
        {
            get => dragBar.Target != null;
            set => dragBar.Target = value ? this : null;
        }

        /// <summary>
        ///     Determines whether the control should be disposed on close.
        /// </summary>
        public Boolean DeleteOnClose { get; set; }

        public override Padding Padding
        {
            get => innerPanel.Padding;
            set => innerPanel.Padding = value;
        }

        /// <summary>
        ///     Starting position of the window.
        /// </summary>
        public StartPosition StartPosition { get; set; } = StartPosition.Manual;

        /// <summary>
        ///     Indicates whether the control is on top of its parent's children.
        /// </summary>
        public override Boolean IsOnTop => Parent.Children.LastOrDefault(x => x is Window) == this;

        public event GwenEventHandler<EventArgs> Closed;

        public override void Show()
        {
            BringToFront();
            base.Show();
        }

        public virtual void Close()
        {
            IsCollapsed = true;

            if (DeleteOnClose)
            {
                Parent.RemoveChild(this, dispose: true);
            }

            if (Closed != null)
            {
                Closed(this, EventArgs.Empty);
            }
        }

        public override void Touch()
        {
            base.Touch();
            BringToFront();
        }

        protected virtual void OnDragged(ControlBase control, EventArgs args)
        {
            SetDragAndResizeCompatibleProperties();
        }

        protected override void OnResized(ControlBase control, EventArgs args)
        {
            SetDragAndResizeCompatibleProperties();
            base.OnResized(control, args);
        }
        
        private void SetDragAndResizeCompatibleProperties()
        {
            StartPosition = StartPosition.Manual;

            if (HorizontalAlignment == HorizontalAlignment.Center)
            {
                HorizontalAlignment = HorizontalAlignment.Left;
            }
            
            if (VerticalAlignment == VerticalAlignment.Center)
            {
                VerticalAlignment = VerticalAlignment.Top;
            }
        }

        public override Boolean SetBounds(Int32 x, Int32 y, Int32 width, Int32 height)
        {
            if (StartPosition == StartPosition.CenterCanvas)
            {
                ControlBase canvas = GetCanvas();
                x = (canvas.ActualWidth - width) / 2;
                y = (canvas.ActualHeight - height) / 2;
            }
            else if (StartPosition == StartPosition.CenterParent)
            {
                Point pt = realParent.LocalPosToCanvas(
                    new Point(realParent.ActualWidth / 2, realParent.ActualHeight / 2));

                x = pt.X - (width / 2);
                y = pt.Y - (height / 2);
            }

            return base.SetBounds(x, y, width, height);
        }
    }
}
