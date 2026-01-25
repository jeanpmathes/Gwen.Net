using System;
using Gwen.Net.Legacy.DragDrop;
using Gwen.Net.Legacy.Input;
using Gwen.Net.Legacy.Skin;

namespace Gwen.Net.Legacy.Control.Internal
{
    /// <summary>
    ///     Tab header.
    /// </summary>
    public class TabButton : Button
    {
        private TabControl control;

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
        public Boolean IsActive => Page != null && Page.IsVisible;

        // todo: remove public access
        public TabControl TabControl
        {
            get => control;
            set
            {
                if (value == control)
                {
                    return;
                }

                if (control != null)
                {
                    control.OnLoseTab(this);
                }

                control = value;
            }
        }

        /// <summary>
        ///     Interior of the tab.
        /// </summary>
        public ControlBase Page { get; set; }

        /// <summary>
        ///     Determines whether the control should be clipped to its bounds while rendering.
        /// </summary>
        protected override Boolean ShouldClip => false;

        public override void DragAndDrop_StartDragging(Package package, Int32 x, Int32 y)
        {
            IsCollapsed = true;
        }

        public override void DragAndDrop_EndDragging(Boolean success, Int32 x, Int32 y)
        {
            IsCollapsed = false;
            IsDepressed = false;
        }

        public override Boolean DragAndDrop_ShouldStartDrag()
        {
            return control.AllowReorder;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawTabButton(this, IsActive, control.TabStrip.Dock);
        }

        /// <summary>
        ///     Handler for Down Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyDown(Boolean down)
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
        protected override Boolean OnKeyUp(Boolean down)
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
        protected override Boolean OnKeyRight(Boolean down)
        {
            if (down)
            {
                Int32 count = Parent.Children.Count;
                Int32 me = Parent.Children.IndexOf(this);

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
        protected override Boolean OnKeyLeft(Boolean down)
        {
            if (down)
            {
                Int32 me = Parent.Children.IndexOf(this);

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
                    TextColor = Skin.colors.tabColors.activeColors.disabled;

                    return;
                }

                if (IsDepressed)
                {
                    TextColor = Skin.colors.tabColors.activeColors.down;

                    return;
                }

                if (IsHovered)
                {
                    TextColor = Skin.colors.tabColors.activeColors.hover;

                    return;
                }

                TextColor = Skin.colors.tabColors.activeColors.normal;
            }

            if (IsDisabled)
            {
                TextColor = Skin.colors.tabColors.inactiveColors.disabled;

                return;
            }

            if (IsDepressed)
            {
                TextColor = Skin.colors.tabColors.inactiveColors.down;

                return;
            }

            if (IsHovered)
            {
                TextColor = Skin.colors.tabColors.inactiveColors.hover;

                return;
            }

            TextColor = Skin.colors.tabColors.inactiveColors.normal;
        }
    }
}
