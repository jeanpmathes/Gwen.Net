using System;
using System.Collections.Generic;
using System.Linq;
using Gwen.Net.Control.Internal;
using Gwen.Net.Control.Layout;
using Gwen.Net.Input;
using Gwen.Net.Skin;
using Gwen.Net.Xml;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Popup menu.
    /// </summary>
    [XmlControl]
    public class Menu : ScrollControl
    {
        protected StackLayout m_Layout;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Menu" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Menu(ControlBase parent)
            : base(parent)
        {
            Padding = Padding.Two;

            Collapse(collapsed: true, measure: false);

            IconMarginDisabled = false;

            AutoHideBars = true;
            EnableScroll(horizontal: false, vertical: true);
            DeleteOnClose = false;

            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;

            m_Layout = new StackLayout(this);
        }

        internal override bool IsMenuComponent => true;

        /// <summary>
        ///     Parent menu item that owns the menu if this is a child of the menu item.
        ///     Real parent of the menu is the canvas.
        /// </summary>
        public MenuItem ParentMenuItem { get; internal set; }

        [XmlProperty] public bool IconMarginDisabled { get; set; }

        /// <summary>
        ///     Determines whether the menu should be disposed on close.
        /// </summary>
        [XmlProperty] public bool DeleteOnClose { get; set; }

        /// <summary>
        ///     Determines whether the menu should open on mouse hover.
        /// </summary>
        protected virtual bool ShouldHoverOpenMenu => true;

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin)
        {
            skin.DrawMenu(this, IconMarginDisabled);
        }

        /// <summary>
        ///     Renders under the actual control (shadows etc).
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void RenderUnder(SkinBase skin)
        {
            base.RenderUnder(skin);
            skin.DrawShadow(this);
        }

        /// <summary>
        ///     Opens the menu.
        /// </summary>
        public void Open()
        {
            Show();
            BringToFront();
            Point mouse = InputHandler.MousePosition;
            SetPosition(mouse.X, mouse.Y);
        }

        protected override Size Measure(Size availableSize)
        {
            availableSize.Height = Math.Min(availableSize.Height, GetCanvas().ActualHeight - Top);

            Size size = base.Measure(availableSize);

            size.Width = Math.Min(Content.MeasuredSize.Width + Padding.Left + Padding.Right, availableSize.Width);
            size.Height = Math.Min(Content.MeasuredSize.Height + Padding.Top + Padding.Bottom, availableSize.Height);

            return size;
        }

        /// <summary>
        ///     Adds a new menu item.
        /// </summary>
        /// <param name="text">Item text.</param>
        /// <returns>Newly created control.</returns>
        public virtual MenuItem AddItem(string text)
        {
            return AddItem(text, string.Empty);
        }

        /// <summary>
        ///     Adds a new menu item.
        /// </summary>
        /// <param name="text">Item text.</param>
        /// <param name="iconName">Icon texture name.</param>
        /// <param name="accelerator">Accelerator for this item.</param>
        /// <returns>Newly created control.</returns>
        public virtual MenuItem AddItem(string text, string iconName, string accelerator = null)
        {
            MenuItem item = new(this);
            item.Padding = Padding.Three;
            item.Text = text;

            if (!string.IsNullOrWhiteSpace(iconName))
            {
                item.SetImage(iconName, ImageAlign.Left | ImageAlign.CenterV);
            }

            if (!string.IsNullOrWhiteSpace(accelerator))
            {
                item.SetAccelerator(accelerator);
            }

            OnAddItem(item);

            return item;
        }

        /// <summary>
        ///     Adds a menu item.
        /// </summary>
        /// <param name="item">Item.</param>
        public virtual void AddItem(MenuItem item)
        {
            item.Parent = this;

            item.Padding = Padding.Three;

            OnAddItem(item);
        }

        public MenuItem AddItemPath(string text)
        {
            return AddItemPath(text, string.Empty);
        }

        public MenuItem AddItemPath(string text, string iconName, string accelerator = null)
        {
            MenuItem item = new(this);
            item.Text = text;
            item.Padding = Padding.Three;

            if (!string.IsNullOrWhiteSpace(iconName))
            {
                item.SetImage(iconName, ImageAlign.Left | ImageAlign.CenterV);
            }

            if (!string.IsNullOrWhiteSpace(accelerator))
            {
                item.SetAccelerator(accelerator);
            }

            AddItemPath(item);

            return item;
        }

        public void AddItemPath(MenuItem item)
        {

            string[] path = item.Text.Split('\\', '/');
            Menu m = this;

            for (var i = 0; i < path.Length - 1; i++)
            {
                MenuItem[] items = m.FindItems(path[i]);

                if (items.Length == 0)
                {
                    m = m.AddItem(path[i]).Menu;
                }
                else if (items.Length == 1)
                {
                    m = items[0].Menu;
                }
                else
                {
                    for (var j = 0; j < items.Length; j++)
                    {
                        if (items[j].Parent == m)
                        {
                            m = items[j].Menu;
                        }
                    }
                }
            }

            item.Text = path.Last();
            m.AddItem(item);
        }

        /// <summary>
        ///     Add item handler.
        /// </summary>
        /// <param name="item">Item added.</param>
        protected virtual void OnAddItem(MenuItem item)
        {
            item.TextPadding = new Padding(IconMarginDisabled ? 0 : 24, top: 0, right: 16, bottom: 0);
            item.Alignment = Alignment.CenterV | Alignment.Left;
            item.HoverEnter += OnHoverItem;
        }

        /// <summary>
        ///     Closes all submenus.
        /// </summary>
        public virtual void CloseAll()
        {
            foreach (ControlBase child in Children)
            {
                if (child is MenuItem)
                {
                    (child as MenuItem).CloseMenu();
                }
            }
        }

        /// <summary>
        ///     Indicates whether any (sub)menu is open.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsMenuOpen()
        {
            return Children.Any(
                child =>
                {
                    if (child is MenuItem)
                    {
                        return (child as MenuItem).IsMenuOpen;
                    }

                    return false;
                });
        }

        /// <summary>
        ///     Mouse hover handler.
        /// </summary>
        /// <param name="control">Event source.</param>
        protected virtual void OnHoverItem(ControlBase control, EventArgs args)
        {
            if (!ShouldHoverOpenMenu)
            {
                return;
            }

            var item = control as MenuItem;

            if (null == item)
            {
                return;
            }

            if (item.IsMenuOpen)
            {
                return;
            }

            CloseAll();
            item.OpenMenu();
        }

        /// <summary>
        ///     Closes the current menu.
        /// </summary>
        public virtual void Close()
        {
            IsCollapsed = true;

            if (DeleteOnClose)
            {
                DelayedDelete();
            }
        }

        /// <summary>
        ///     Finds all items by name in current menu.
        /// </summary>
        public MenuItem[] FindItems(string name)
        {
            List<MenuItem> mi = new();

            for (var i = 0; i < Children.Count; i++)
            {
                if (Children[i] as MenuItem != null)
                {
                    if (((MenuItem) Children[i]).Text == name)
                    {
                        mi.Add(Children[i] as MenuItem);
                    }
                }
            }

            return mi.ToArray();
        }

        /// <summary>
        ///     Closes all submenus and the current menu.
        /// </summary>
        public override void CloseMenus()
        {
            base.CloseMenus();
            CloseAll();
            Close();
        }

        /// <summary>
        ///     Adds a divider menu item.
        /// </summary>
        public virtual void AddDivider()
        {
            MenuDivider divider = new(this);
            divider.Margin = new Margin(IconMarginDisabled ? 0 : 24, top: 0, right: 4, bottom: 0);
        }

        /// <summary>
        ///     Removes all items.
        /// </summary>
        public void RemoveAll()
        {
            m_Layout.DeleteAllChildren();
        }
    }
}
