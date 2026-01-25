using System;
using Gwen.Net.Legacy.Control.Layout;
using Gwen.Net.Legacy.DragDrop;

namespace Gwen.Net.Legacy.Control.Internal
{
    /// <summary>
    ///     Tab strip - groups TabButtons and allows reordering.
    /// </summary>
    public class TabStrip : StackLayout
    {
        private Int32 scrollOffset;
        private ControlBase tabDragControl;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TabStrip" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public TabStrip(ControlBase parent)
            : base(parent)
        {
            AllowReorder = false;
            scrollOffset = 0;
        }

        /// <summary>
        ///     Determines whether it is possible to reorder tabs by mouse dragging.
        /// </summary>
        public Boolean AllowReorder { get; set; }

        internal Int32 ScrollOffset
        {
            get => scrollOffset;
            set => SetScrollOffset(value);
        }

        internal Size TotalSize { get; private set; }

        /// <summary>
        ///     Determines whether the control should be clipped to its bounds while rendering.
        /// </summary>
        protected override Boolean ShouldClip => false;

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

        private void SetScrollOffset(Int32 value)
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

            scrollOffset = value;
            scrollOffset = Math.Min(scrollOffset, Children.Count - 1);
            scrollOffset = Math.Max(scrollOffset, val2: 0);

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
                Int32 notFirst = num > 0 ? -1 : 0;

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

        public override void DragAndDrop_HoverEnter(Package p, Int32 x, Int32 y)
        {
            if (tabDragControl != null)
            {
                throw new InvalidOperationException("ERROR! TabStrip::DragAndDrop_HoverEnter");
            }

            tabDragControl = new Highlight(GetCanvas());
            tabDragControl.MouseInputEnabled = false;
            tabDragControl.Size = new Size(width: 3, ActualHeight);
            Invalidate();
        }

        public override void DragAndDrop_HoverLeave(Package p)
        {
            if (tabDragControl != null)
            {
                tabDragControl.Parent.RemoveChild(
                    tabDragControl,
                    dispose: false); // [omeg] need to do that explicitely

                tabDragControl.Dispose();
            }

            tabDragControl = null;
        }

        public override void DragAndDrop_Hover(Package p, Int32 x, Int32 y)
        {
            Point localPos = CanvasPosToLocal(new Point(x, y));

            ControlBase droppedOn = GetControlAt(localPos.X, localPos.Y);

            if (droppedOn != null && droppedOn != this)
            {
                Point dropPos = droppedOn.CanvasPosToLocal(new Point(x, y));
                tabDragControl.BringToFront();
                Int32 pos = droppedOn.ActualLeft - 1;

                if (dropPos.X > droppedOn.ActualWidth / 2)
                {
                    pos += droppedOn.ActualWidth - 1;
                }

                Point canvasPos = LocalPosToCanvas(new Point(pos, y: 0));
                tabDragControl.MoveTo(canvasPos.X, canvasPos.Y);
            }
            else
            {
                tabDragControl.BringToFront();
            }
        }

        public override Boolean DragAndDrop_HandleDrop(Package p, Int32 x, Int32 y)
        {
            Point localPos = CanvasPosToLocal(new Point(x, y));

            if (Parent is TabControl tabControl && DragAndDrop.SourceControl is TabButton button)
            {
                if (button.TabControl != tabControl)
                {
                    // We've moved tab controls!
                    tabControl.AddPage(button);
                }
            }

            ControlBase droppedOn = GetControlAt(localPos.X, localPos.Y);

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

        public override Boolean DragAndDrop_CanAcceptPackage(Package p)
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
