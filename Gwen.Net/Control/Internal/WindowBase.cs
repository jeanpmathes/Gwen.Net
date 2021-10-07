using System;
using System.Linq;
using Gwen.Net.Xml;

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
        private readonly ControlBase m_RealParent;
        protected Dragger m_DragBar;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WindowBase" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public WindowBase(ControlBase parent)
            : base(parent.GetCanvas())
        {
            m_RealParent = parent;

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
        [XmlProperty] public bool IsDraggingEnabled
        {
            get => m_DragBar.Target != null;
            set => m_DragBar.Target = value ? this : null;
        }

        /// <summary>
        ///     Determines whether the control should be disposed on close.
        /// </summary>
        [XmlProperty] public bool DeleteOnClose { get; set; }

        [XmlProperty] public override Padding Padding
        {
            get => m_InnerPanel.Padding;
            set => m_InnerPanel.Padding = value;
        }

        /// <summary>
        ///     Starting position of the window.
        /// </summary>
        [XmlProperty] public StartPosition StartPosition { get; set; } = StartPosition.Manual;

        /// <summary>
        ///     Indicates whether the control is on top of its parent's children.
        /// </summary>
        public override bool IsOnTop => Parent.Children.Where(x => x is Window).Last() == this;

        [XmlEvent] public event GwenEventHandler<EventArgs> Closed;

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
            StartPosition = StartPosition.Manual;
        }

        protected override void OnResized(ControlBase control, EventArgs args)
        {
            StartPosition = StartPosition.Manual;

            base.OnResized(control, args);
        }

        public override bool SetBounds(int x, int y, int width, int height)
        {
            if (StartPosition == StartPosition.CenterCanvas)
            {
                ControlBase canvas = GetCanvas();
                x = (canvas.ActualWidth - width) / 2;
                y = (canvas.ActualHeight - height) / 2;
            }
            else if (StartPosition == StartPosition.CenterParent)
            {
                Point pt = m_RealParent.LocalPosToCanvas(
                    new Point(m_RealParent.ActualWidth / 2, m_RealParent.ActualHeight / 2));

                x = pt.X - (width / 2);
                y = pt.Y - (height / 2);
            }

            return base.SetBounds(x, y, width, height);
        }
    }
}