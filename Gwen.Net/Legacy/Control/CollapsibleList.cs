using System;
using Gwen.Net.Legacy.Control.Layout;
using Gwen.Net.Legacy.Skin;

namespace Gwen.Net.Legacy.Control
{
    /// <summary>
    ///     CollapsibleList control. Groups CollapsibleCategory controls.
    /// </summary>
    public class CollapsibleList : ScrollControl
    {
        private readonly VerticalLayout items;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CollapsibleList" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public CollapsibleList(ControlBase parent)
            : base(parent)
        {
            Padding = Padding.One;

            MouseInputEnabled = true;
            EnableScroll(horizontal: false, vertical: true);
            AutoHideBars = true;

            items = new VerticalLayout(this);
        }

        /// <summary>
        ///     Invoked when an entry has been selected.
        /// </summary>
        public event GwenEventHandler<ItemSelectedEventArgs> ItemSelected;

        /// <summary>
        ///     Invoked when a category collapsed state has been changed (header button has been pressed).
        /// </summary>
        public event GwenEventHandler<EventArgs> CategoryCollapsed;

        // todo: iterator, make this as function? check if works

        /// <summary>
        ///     Selected entry.
        /// </summary>
        public Button GetSelectedButton()
        {
            foreach (ControlBase child in Children)
            {
                var cat = child as CollapsibleCategory;

                if (cat == null)
                {
                    continue;
                }

                Button button = cat.GetSelectedButton();

                if (button != null)
                {
                    return button;
                }
            }

            return null;
        }

        /// <summary>
        ///     Adds a category to the list.
        /// </summary>
        /// <param name="category">Category control to add.</param>
        protected virtual void Add(CollapsibleCategory category)
        {
            category.Parent = items;
            category.Margin = new Margin(left: 1, top: 1, right: 1, bottom: 0);
            category.Selected += OnCategorySelected;
            category.Collapsed += OnCategoryCollapsed;

            Invalidate();
        }

        /// <summary>
        ///     Adds a new category to the list.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="name">Name of the control.</param>
        /// <param name="userData">User data.</param>
        /// <returns>Newly created control.</returns>
        public virtual CollapsibleCategory Add(String categoryName, String name = null, Object userData = null)
        {
            CollapsibleCategory cat = new(this);
            cat.Text = categoryName;
            cat.Name = name;
            cat.UserData = userData;
            Add(cat);

            return cat;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawCategoryHolder(this);
        }

        /// <summary>
        ///     Unselects all entries.
        /// </summary>
        public virtual void UnselectAll()
        {
            foreach (ControlBase child in items.Children)
            {
                var cat = child as CollapsibleCategory;

                if (cat == null)
                {
                    continue;
                }

                cat.UnselectAll();
            }
        }

        /// <summary>
        ///     Handler for ItemSelected event.
        /// </summary>
        /// <param name="control">Event source: <see cref="CollapsibleList" />.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnCategorySelected(ControlBase control, EventArgs args)
        {
            var cat = control as CollapsibleCategory;

            if (cat == null)
            {
                return;
            }

            if (ItemSelected != null)
            {
                ItemSelected.Invoke(this, new ItemSelectedEventArgs(cat));
            }
        }

        /// <summary>
        ///     Handler for category collapsed event.
        /// </summary>
        /// <param name="control">Event source: <see cref="CollapsibleCategory" />.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnCategoryCollapsed(ControlBase control, EventArgs args)
        {
            var cat = control as CollapsibleCategory;

            if (cat == null)
            {
                return;
            }

            if (CategoryCollapsed != null)
            {
                CategoryCollapsed.Invoke(control, EventArgs.Empty);
            }
        }
    }
}
