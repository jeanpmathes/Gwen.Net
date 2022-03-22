using Gwen.Net.Control.Internal;
using Gwen.Net.DragDrop;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Docked tab control.
    /// </summary>
    public class DockedTabControl : TabControl
    {
        private readonly TabTitleBar m_TitleBar;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DockedTabControl" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public DockedTabControl(ControlBase parent)
            : base(parent)
        {
            Dock = Dock.Fill;

            m_TitleBar = new TabTitleBar(this);
            m_TitleBar.Dock = Dock.Top;
            m_TitleBar.IsCollapsed = true;

            AllowReorder = true;
        }

        /// <summary>
        ///     Determines whether the title bar is visible.
        /// </summary>
        public bool TitleBarVisible
        {
            get => !m_TitleBar.IsCollapsed;
            set => m_TitleBar.IsCollapsed = !value;
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

            m_TitleBar.UpdateFromTab(CurrentButton);
        }

        public override void DragAndDrop_StartDragging(Package package, int x, int y)
        {
            base.DragAndDrop_StartDragging(package, x, y);

            IsCollapsed = true;
            // This hiding our parent thing is kind of lousy.
            Parent.IsCollapsed = true;
        }

        public override void DragAndDrop_EndDragging(bool success, int x, int y)
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
