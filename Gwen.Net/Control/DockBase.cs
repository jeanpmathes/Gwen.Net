using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.DragDrop;
using Gwen.Net.Renderer;
using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Base for dockable containers.
    /// </summary>
    public class DockBase : ControlBase
    {
        private DockBase bottom;

        // Only CHILD dockpanels have a tabcontrol.
        private DockedTabControl dockedTabControl;

        private Boolean drawHover;
        private Boolean dropFar;
        private Rectangle hoverRect;
        private DockBase left;
        private DockBase right;
        private Resizer sizer;
        private DockBase top;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DockBase" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public DockBase(ControlBase parent)
            : base(parent)
        {
            Padding = Padding.One;
            MinimumSize = new Size(width: 30, height: 30);
            MouseInputEnabled = true;
        }

        // todo: dock events?

        /// <summary>
        ///     Control docked on the left side.
        /// </summary>
        public DockBase LeftDock => GetChildDock(Dock.Left);

        /// <summary>
        ///     Control docked on the right side.
        /// </summary>
        public DockBase RightDock => GetChildDock(Dock.Right);

        /// <summary>
        ///     Control docked on the top side.
        /// </summary>
        public DockBase TopDock => GetChildDock(Dock.Top);

        /// <summary>
        ///     Control docked on the bottom side.
        /// </summary>
        public DockBase BottomDock => GetChildDock(Dock.Bottom);

        public TabControl TabControl => dockedTabControl;

        /// <summary>
        ///     Indicates whether the control contains any docked children.
        /// </summary>
        public virtual Boolean IsEmpty
        {
            get
            {
                if (dockedTabControl != null && dockedTabControl.TabCount > 0)
                {
                    return false;
                }

                if (left != null && !left.IsEmpty)
                {
                    return false;
                }

                if (right != null && !right.IsEmpty)
                {
                    return false;
                }

                if (top != null && !top.IsEmpty)
                {
                    return false;
                }

                if (bottom != null && !bottom.IsEmpty)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        ///     Handler for Space keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeySpace(Boolean down)
        {
            // No action on space (default button action is to press)
            return false;
        }

        /// <summary>
        ///     Initializes an inner docked control for the specified position.
        /// </summary>
        /// <param name="pos">Dock position.</param>
        protected virtual void SetupChildDock(Dock pos)
        {
            if (dockedTabControl == null)
            {
                dockedTabControl = new DockedTabControl(this);
                dockedTabControl.TabRemoved += OnTabRemoved;
                dockedTabControl.TabStripPosition = Dock.Bottom;
                dockedTabControl.TitleBarVisible = true;
            }

            Dock = pos;

            Dock sizeDir;

            if (pos == Dock.Right)
            {
                sizeDir = Dock.Left;
            }
            else if (pos == Dock.Left)
            {
                sizeDir = Dock.Right;
            }
            else if (pos == Dock.Top)
            {
                sizeDir = Dock.Bottom;
            }
            else if (pos == Dock.Bottom)
            {
                sizeDir = Dock.Top;
            }
            else
            {
                throw new ArgumentException("Invalid dock", "pos");
            }

            if (sizer != null)
            {
                sizer.Dispose();
            }

            sizer = new Resizer(this);
            sizer.Dock = sizeDir;
            sizer.ResizeDir = sizeDir;

            if (sizeDir == Dock.Left || sizeDir == Dock.Right)
            {
                sizer.Width = 2;
            }
            else
            {
                sizer.Height = 2;
            }
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin) {}

        /// <summary>
        ///     Gets an inner docked control for the specified position.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected virtual DockBase GetChildDock(Dock pos)
        {
            // todo: verify
            DockBase dock = null;

            switch (pos)
            {
                case Dock.Left:
                    if (left == null)
                    {
                        left = new DockBase(this);
                        left.Width = 200;
                        left.SetupChildDock(pos);
                    }

                    dock = left;

                    break;

                case Dock.Right:
                    if (right == null)
                    {
                        right = new DockBase(this);
                        right.Width = 200;
                        right.SetupChildDock(pos);
                    }

                    dock = right;

                    break;

                case Dock.Top:
                    if (top == null)
                    {
                        top = new DockBase(this);
                        top.Height = 200;
                        top.SetupChildDock(pos);
                    }

                    dock = top;

                    break;

                case Dock.Bottom:
                    if (bottom == null)
                    {
                        bottom = new DockBase(this);
                        bottom.Height = 200;
                        bottom.SetupChildDock(pos);
                    }

                    dock = bottom;

                    break;
            }

            if (dock != null)
            {
                dock.IsCollapsed = false;
            }

            return dock;
        }

        /// <summary>
        ///     Calculates dock direction from dragdrop coordinates.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <returns>Dock direction.</returns>
        protected virtual Dock GetDroppedTabDirection(Int32 x, Int32 y)
        {
            Int32 w = ActualWidth;
            Int32 h = ActualHeight;
            Single localTop = y / (Single) h;
            Single localLeft = x / (Single) w;
            Single localRight = (w - x) / (Single) w;
            Single localBottom = (h - y) / (Single) h;
            Single minimum = Math.Min(Math.Min(Math.Min(localTop, localLeft), localRight), localBottom);

            dropFar = minimum < 0.2f;

            if (minimum > 0.3f)
            {
                return Dock.Fill;
            }

            if (localTop == minimum && (null == top || top.IsCollapsed))
            {
                return Dock.Top;
            }

            if (localLeft == minimum && (null == left || left.IsCollapsed))
            {
                return Dock.Left;
            }

            if (localRight == minimum && (null == right || right.IsCollapsed))
            {
                return Dock.Right;
            }

            if (localBottom == minimum && (null == bottom || bottom.IsCollapsed))
            {
                return Dock.Bottom;
            }

            return Dock.Fill;
        }

        public override Boolean DragAndDrop_CanAcceptPackage(Package p)
        {
            // A TAB button dropped 
            if (p.Name == "TabButtonMove")
            {
                return true;
            }

            // a TAB window dropped
            if (p.Name == "TabWindowMove")
            {
                return true;
            }

            return false;
        }

        public override Boolean DragAndDrop_HandleDrop(Package p, Int32 x, Int32 y)
        {
            Point pos = CanvasPosToLocal(new Point(x, y));
            Dock dir = GetDroppedTabDirection(pos.X, pos.Y);

            Invalidate();

            DockedTabControl addTo = dockedTabControl;

            if (dir == Dock.Fill && addTo == null)
            {
                return false;
            }

            if (dir != Dock.Fill)
            {
                DockBase dock = GetChildDock(dir);
                addTo = dock.dockedTabControl;

                if (!dropFar)
                {
                    dock.BringToFront();
                }
                else
                {
                    dock.SendToBack();
                }
            }

            if (p.Name == "TabButtonMove")
            {
                var tabButton = DragAndDrop.SourceControl as TabButton;

                if (null == tabButton)
                {
                    return false;
                }

                addTo.AddPage(tabButton);
            }

            if (p.Name == "TabWindowMove")
            {
                var tabControl = DragAndDrop.SourceControl as DockedTabControl;

                if (null == tabControl)
                {
                    return false;
                }

                if (tabControl == addTo)
                {
                    return false;
                }

                tabControl.MoveTabsTo(addTo);
            }

            return true;
        }

        protected virtual void OnTabRemoved(ControlBase control, EventArgs args)
        {
            DoRedundancyCheck();
            DoConsolidateCheck();
        }

        protected virtual void DoRedundancyCheck()
        {
            if (!IsEmpty)
            {
                return;
            }

            var pDockParent = Parent as DockBase;

            if (null == pDockParent)
            {
                return;
            }

            pDockParent.OnRedundantChildDock(this);
        }

        protected virtual void DoConsolidateCheck()
        {
            if (IsEmpty)
            {
                return;
            }

            if (null == dockedTabControl)
            {
                return;
            }

            if (dockedTabControl.TabCount > 0)
            {
                return;
            }

            if (bottom != null && !bottom.IsEmpty)
            {
                bottom.dockedTabControl.MoveTabsTo(dockedTabControl);

                return;
            }

            if (top != null && !top.IsEmpty)
            {
                top.dockedTabControl.MoveTabsTo(dockedTabControl);

                return;
            }

            if (left != null && !left.IsEmpty)
            {
                left.dockedTabControl.MoveTabsTo(dockedTabControl);

                return;
            }

            if (right != null && !right.IsEmpty)
            {
                right.dockedTabControl.MoveTabsTo(dockedTabControl);
            }
        }

        protected virtual void OnRedundantChildDock(DockBase dock)
        {
            dock.IsCollapsed = true;
            DoRedundancyCheck();
            DoConsolidateCheck();
        }

        public override void DragAndDrop_HoverEnter(Package p, Int32 x, Int32 y)
        {
            drawHover = true;
        }

        public override void DragAndDrop_HoverLeave(Package p)
        {
            drawHover = false;
        }

        public override void DragAndDrop_Hover(Package p, Int32 x, Int32 y)
        {
            Point pos = CanvasPosToLocal(new Point(x, y));
            Dock dir = GetDroppedTabDirection(pos.X, pos.Y);

            if (dir == Dock.Fill)
            {
                if (null == dockedTabControl)
                {
                    hoverRect = Rectangle.Empty;

                    return;
                }

                hoverRect = InnerBounds;

                return;
            }

            hoverRect = RenderBounds;

            Int32 helpBarWidth;

            if (dir == Dock.Left)
            {
                helpBarWidth = (Int32) (hoverRect.Width * 0.25f);
                hoverRect.Width = helpBarWidth;
            }

            if (dir == Dock.Right)
            {
                helpBarWidth = (Int32) (hoverRect.Width * 0.25f);
                hoverRect.X = hoverRect.Width - helpBarWidth;
                hoverRect.Width = helpBarWidth;
            }

            if (dir == Dock.Top)
            {
                helpBarWidth = (Int32) (hoverRect.Height * 0.25f);
                hoverRect.Height = helpBarWidth;
            }

            if (dir == Dock.Bottom)
            {
                helpBarWidth = (Int32) (hoverRect.Height * 0.25f);
                hoverRect.Y = hoverRect.Height - helpBarWidth;
                hoverRect.Height = helpBarWidth;
            }

            if ((dir == Dock.Top || dir == Dock.Bottom) && !dropFar)
            {
                if (left != null && !left.IsCollapsed)
                {
                    hoverRect.X += left.ActualWidth;
                    hoverRect.Width -= left.ActualWidth;
                }

                if (right != null && !right.IsCollapsed)
                {
                    hoverRect.Width -= right.ActualWidth;
                }
            }

            if ((dir == Dock.Left || dir == Dock.Right) && !dropFar)
            {
                if (top != null && !top.IsCollapsed)
                {
                    hoverRect.Y += top.ActualHeight;
                    hoverRect.Height -= top.ActualHeight;
                }

                if (bottom != null && !bottom.IsCollapsed)
                {
                    hoverRect.Height -= bottom.ActualHeight;
                }
            }
        }

        /// <summary>
        ///     Renders over the actual control (overlays).
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void RenderOver(SkinBase currentSkin)
        {
            if (!drawHover)
            {
                return;
            }

            RendererBase render = currentSkin.Renderer;
            render.DrawColor = new Color(a: 20, r: 255, g: 200, b: 255);
            render.DrawFilledRect(RenderBounds);

            if (hoverRect.Width == 0)
            {
                return;
            }

            render.DrawColor = new Color(a: 100, r: 255, g: 200, b: 255);
            render.DrawFilledRect(hoverRect);

            render.DrawColor = new Color(a: 200, r: 255, g: 200, b: 255);
            render.DrawLinedRect(hoverRect);
        }
    }
}
