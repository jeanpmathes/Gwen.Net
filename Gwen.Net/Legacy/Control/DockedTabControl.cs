using System;
using Gwen.Net.Legacy.Control.Internal;
using Gwen.Net.Legacy.DragDrop;

namespace Gwen.Net.Legacy.Control
{
    /// <summary>
    ///     Docked tab control.
    /// </summary>
    public class DockedTabControl : TabControl
    {
        private readonly TabTitleBar titleBar;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DockedTabControl" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public DockedTabControl(ControlBase parent)
            : base(parent)
        {
            Dock = Dock.Fill;

            titleBar = new TabTitleBar(this);
            titleBar.Dock = Dock.Top;
            titleBar.IsCollapsed = true;

            AllowReorder = true;
        }

        /// <summary>
        ///     Determines whether the title bar is visible.
        /// </summary>
        public Boolean TitleBarVisible
        {
            get => !titleBar.IsCollapsed;
            set => titleBar.IsCollapsed = !value;
        }

        protected override Size Measure(Size availableSize)
        {
            TabStrip.Collapse(TabCount <= 1, measure: false);
            UpdateTitleBar();

            return base.Measure(availableSize);
        }

        private void UpdateTitleBar()
        {
            if (CurrentButton == null)
            {
                return;
            }

            titleBar.UpdateFromTab(CurrentButton);
        }

        public override void DragAndDrop_StartDragging(Package package, Int32 x, Int32 y)
        {
            base.DragAndDrop_StartDragging(package, x, y);

            IsCollapsed = true;
            // This hiding our parent thing is kind of lousy.
            Parent.IsCollapsed = true;
        }

        public override void DragAndDrop_EndDragging(Boolean success, Int32 x, Int32 y)
        {
            IsCollapsed = false;

            if (!success)
            {
                Parent.IsCollapsed = false;
            }
        }

        public void MoveTabsTo(DockedTabControl target)
        {
            ControlBase[] children = TabStrip.Children.ToArray(); // copy because collection will be modified

            foreach (ControlBase child in children)
            {
                var button = child as TabButton;

                if (button == null)
                {
                    continue;
                }

                target.AddPage(button);
            }

            Invalidate();
        }
    }
}
