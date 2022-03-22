using System;
using Gwen.Net.Control.Layout;
using Gwen.Net.DragDrop;

namespace Gwen.Net.Control.Internal
{
    /// <summary>
    ///     Tab strip - groups TabButtons and allows reordering.
    /// </summary>
    public class TabStrip : StackLayout
    {
        private int m_ScrollOffset;
        private ControlBase m_TabDragControl;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TabStrip" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public TabStrip(ControlBase parent)
            : base(parent)
        {
            AllowReorder = false;
            m_ScrollOffset = 0;
        }

        /// <summary>
        ///     Determines whether it is possible to reorder tabs by mouse dragging.
        /// </summary>
        public bool AllowReorder { get; set; }

        internal int ScrollOffset
        {
            get => m_ScrollOffset;
            set => SetScrollOffset(value);
        }

        internal Size TotalSize { get; private set; }

        /// <summary>
        ///     Determines whether the control should be clipped to its bounds while rendering.
        /// </summary>
        protected override bool ShouldClip => false;

        /// <summary>
        ///     Strip position (top/left/right/bottom).
        /// </summary>
        public Dock StripPosition
        {
            get => Dock;
            set
            {
                Dock = value;

                switch (value)
                {
                    case Dock.Top:
                        Padding = new Padding(left: 5, top: 0, right: 0, bottom: 0);
                        Horizontal = true;

                        break;
                    case Dock.Left:
                        Padding = new Padding(left: 0, top: 5, right: 0, bottom: 0);
                        Horizontal = false;

                        break;
                    case Dock.Bottom:
                        Padding = new Padding(left: 5, top: 0, right: 0, bottom: 0);
                        Horizontal = true;

                        break;
                    case Dock.Right:
                        Padding = new Padding(left: 0, top: 5, right: 0, bottom: 0);
                        Horizontal = false;

                        break;
                }
            }
        }

        private void SetScrollOffset(int value)
        {
            for (var i = 0; i < Children.Count; i++)
            {
                if (i < value && i < Children.Count - 1)
                {
                    Children[i].Collapse(collapsed: true, measure: false);
                }
                else
                {
                    Children[i].Collapse(collapsed: false, measure: false);
                }
            }

            m_ScrollOffset = value;
            m_ScrollOffset = Math.Min(m_ScrollOffset, Children.Count - 1);
            m_ScrollOffset = Math.Max(m_ScrollOffset, val2: 0);

            Invalidate();
        }

        protected override Size Measure(Size availableSize)
        {
            var num = 0;

            foreach (ControlBase child in Children)
            {
                var button = child as TabButton;

                if (null == button)
                {
                    continue;
                }

                Margin m = new();
                int notFirst = num > 0 ? -1 : 0;

                switch (StripPosition)
                {
                    case Dock.Top:
                    case Dock.Bottom:
                        m.Left = notFirst;

                        break;
                    case Dock.Left:
                    case Dock.Right:
                        m.Top = notFirst;

                        break;
                }

                button.Margin = m;
                num++;
            }

            TotalSize = base.Measure(Size.Infinity);

            return TotalSize;
        }

        public override void DragAndDrop_HoverEnter(Package p, int x, int y)
        {
            if (m_TabDragControl != null)
            {
                throw new InvalidOperationException("ERROR! TabStrip::DragAndDrop_HoverEnter");
            }

            m_TabDragControl = new Highlight(GetCanvas());
            m_TabDragControl.MouseInputEnabled = false;
            m_TabDragControl.Size = new Size(width: 3, ActualHeight);
            Invalidate();
        }

        public override void DragAndDrop_HoverLeave(Package p)
        {
            if (m_TabDragControl != null)
            {
                m_TabDragControl.Parent.RemoveChild(
                    m_TabDragControl,
                    dispose: false); // [omeg] need to do that explicitely

                m_TabDragControl.Dispose();
            }

            m_TabDragControl = null;
        }

        public override void DragAndDrop_Hover(Package p, int x, int y)
        {
            Point localPos = CanvasPosToLocal(new Point(x, y));

            ControlBase droppedOn = GetControlAt(localPos.X, localPos.Y);

            if (droppedOn != null && droppedOn != this)
            {
                Point dropPos = droppedOn.CanvasPosToLocal(new Point(x, y));
                m_TabDragControl.BringToFront();
                int pos = droppedOn.ActualLeft - 1;

                if (dropPos.X > droppedOn.ActualWidth / 2)
                {
                    pos += droppedOn.ActualWidth - 1;
                }

                Point canvasPos = LocalPosToCanvas(new Point(pos, y: 0));
                m_TabDragControl.MoveTo(canvasPos.X, canvasPos.Y);
            }
            else
            {
                m_TabDragControl.BringToFront();
            }
        }

        public override bool DragAndDrop_HandleDrop(Package p, int x, int y)
        {
            Point LocalPos = CanvasPosToLocal(new Point(x, y));

            var button = DragAndDrop.SourceControl as TabButton;
            var tabControl = Parent as TabControl;

            if (tabControl != null && button != null)
            {
                if (button.TabControl != tabControl)
                {
                    // We've moved tab controls!
                    tabControl.AddPage(button);
                }
            }

            ControlBase droppedOn = GetControlAt(LocalPos.X, LocalPos.Y);

            if (droppedOn != null && droppedOn != this)
            {
                Point dropPos = droppedOn.CanvasPosToLocal(new Point(x, y));
                DragAndDrop.SourceControl.BringNextToControl(droppedOn, dropPos.X > droppedOn.ActualWidth / 2);
            }
            else
            {
                DragAndDrop.SourceControl.BringToFront();
            }

            return true;
        }

        public override bool DragAndDrop_CanAcceptPackage(Package p)
        {
            if (!AllowReorder)
            {
                return false;
            }

            if (p.Name == "TabButtonMove")
            {
                return true;
            }

            return false;
        }
    }
}
