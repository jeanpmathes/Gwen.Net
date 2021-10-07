using System;
using Gwen.Net.DragDrop;
using Gwen.Net.Input;
using Gwen.Net.Skin;

namespace Gwen.Net.Control.Internal
{
    /// <summary>
    ///     Tab header.
    /// </summary>
    public class TabButton : Button
    {
        private TabControl m_Control;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TabButton" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public TabButton(ControlBase parent)
            : base(parent)
        {
            DragAndDrop_SetPackage(draggable: true, "TabButtonMove");
            Alignment = Alignment.Top | Alignment.Left;
            TextPadding = new Padding(left: 5, top: 3, right: 3, bottom: 3);
            Padding = Padding.Two;
            KeyboardInputEnabled = true;
        }

        /// <summary>
        ///     Indicates whether the tab is active.
        /// </summary>
        public bool IsActive => Page != null && Page.IsVisible;

        // todo: remove public access
        public TabControl TabControl
        {
            get => m_Control;
            set
            {
                if (value == m_Control)
                {
                    return;
                }

                if (m_Control != null)
                {
                    m_Control.OnLoseTab(this);
                }

                m_Control = value;
            }
        }

        /// <summary>
        ///     Interior of the tab.
        /// </summary>
        public ControlBase Page { get; set; }

        /// <summary>
        ///     Determines whether the control should be clipped to its bounds while rendering.
        /// </summary>
        protected override bool ShouldClip => false;

        public override void DragAndDrop_StartDragging(Package package, int x, int y)
        {
            IsCollapsed = true;
        }

        public override void DragAndDrop_EndDragging(bool success, int x, int y)
        {
            IsCollapsed = false;
            IsDepressed = false;
        }

        public override bool DragAndDrop_ShouldStartDrag()
        {
            return m_Control.AllowReorder;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin)
        {
            skin.DrawTabButton(this, IsActive, m_Control.TabStrip.Dock);
        }

        /// <summary>
        ///     Handler for Down Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnKeyDown(bool down)
        {
            OnKeyRight(down);

            return true;
        }

        /// <summary>
        ///     Handler for Up Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnKeyUp(bool down)
        {
            OnKeyLeft(down);

            return true;
        }

        /// <summary>
        ///     Handler for Right Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnKeyRight(bool down)
        {
            if (down)
            {
                int count = Parent.Children.Count;
                int me = Parent.Children.IndexOf(this);

                if (me + 1 < count)
                {
                    ControlBase nextTab = Parent.Children[me + 1];
                    TabControl.OnTabPressed(nextTab, EventArgs.Empty);
                    InputHandler.KeyboardFocus = nextTab;
                }
            }

            return true;
        }

        /// <summary>
        ///     Handler for Left Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnKeyLeft(bool down)
        {
            if (down)
            {
                int count = Parent.Children.Count;
                int me = Parent.Children.IndexOf(this);

                if (me - 1 >= 0)
                {
                    ControlBase prevTab = Parent.Children[me - 1];
                    TabControl.OnTabPressed(prevTab, EventArgs.Empty);
                    InputHandler.KeyboardFocus = prevTab;
                }
            }

            return true;
        }

        /// <summary>
        ///     Updates control colors.
        /// </summary>
        public override void UpdateColors()
        {
            if (IsActive)
            {
                if (IsDisabled)
                {
                    TextColor = Skin.Colors.Tab.Active.Disabled;

                    return;
                }

                if (IsDepressed)
                {
                    TextColor = Skin.Colors.Tab.Active.Down;

                    return;
                }

                if (IsHovered)
                {
                    TextColor = Skin.Colors.Tab.Active.Hover;

                    return;
                }

                TextColor = Skin.Colors.Tab.Active.Normal;
            }

            if (IsDisabled)
            {
                TextColor = Skin.Colors.Tab.Inactive.Disabled;

                return;
            }

            if (IsDepressed)
            {
                TextColor = Skin.Colors.Tab.Inactive.Down;

                return;
            }

            if (IsHovered)
            {
                TextColor = Skin.Colors.Tab.Inactive.Hover;

                return;
            }

            TextColor = Skin.Colors.Tab.Inactive.Normal;
        }
    }
}