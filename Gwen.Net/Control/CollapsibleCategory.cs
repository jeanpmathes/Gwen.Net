using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     CollapsibleCategory control. Used in CollapsibleList.
    /// </summary>
    public class CollapsibleCategory : ControlBase
    {
        private readonly CategoryHeaderButton headerButton;
        private readonly CollapsibleList list;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CollapsibleCategory" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public CollapsibleCategory(CollapsibleList parent) : base(parent)
        {
            headerButton = new CategoryHeaderButton(this);
            headerButton.Text = "Category Title";
            headerButton.Toggled += OnHeaderToggle;

            list = parent;

            Padding = new Padding(left: 1, top: 0, right: 1, bottom: 2);
        }

        /// <summary>
        ///     Header text.
        /// </summary>
        public String Text
        {
            get => headerButton.Text;
            set => headerButton.Text = value;
        }

        /// <summary>
        ///     Determines whether the category is collapsed (closed).
        /// </summary>
        public Boolean IsCategoryCollapsed
        {
            get => headerButton.ToggleState;
            set => headerButton.ToggleState = value;
        }

        /// <summary>
        ///     Invoked when an entry has been selected.
        /// </summary>
        public event GwenEventHandler<ItemSelectedEventArgs> Selected;

        /// <summary>
        ///     Invoked when the category collapsed state has been changed (header button has been pressed).
        /// </summary>
        public event GwenEventHandler<EventArgs> Collapsed;

        /// <summary>
        ///     Gets the selected entry.
        /// </summary>
        public Button GetSelectedButton()
        {
            foreach (ControlBase child in Children)
            {
                var button = child as CategoryButton;

                if (button == null)
                {
                    continue;
                }

                if (button.ToggleState)
                {
                    return button;
                }
            }

            return null;
        }

        /// <summary>
        ///     Handler for header button toggle event.
        /// </summary>
        /// <param name="control">Source control.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnHeaderToggle(ControlBase control, EventArgs args)
        {
            Invalidate();

            if (Collapsed != null)
            {
                Collapsed.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Handler for Selected event.
        /// </summary>
        /// <param name="control">Event source.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnSelected(ControlBase control, EventArgs args)
        {
            var child = control as CategoryButton;

            if (child == null)
            {
                return;
            }

            if (list != null)
            {
                list.UnselectAll();
            }
            else
            {
                UnselectAll();
            }

            child.ToggleState = true;

            if (Selected != null)
            {
                Selected.Invoke(this, new ItemSelectedEventArgs(control));
            }
        }

        /// <summary>
        ///     Adds a new entry.
        /// </summary>
        /// <param name="name">Entry name (displayed).</param>
        /// <returns>Newly created control.</returns>
        public Button Add(String name)
        {
            CategoryButton button = new(this);
            button.Text = name;
            button.Padding = new Padding(left: 5, top: 2, right: 2, bottom: 2);
            button.Clicked += OnSelected;

            Invalidate();

            return button;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawCategoryInner(this, headerButton.ActualHeight, headerButton.ToggleState);
            base.Render(currentSkin);
        }

        /// <summary>
        ///     Unselects all entries.
        /// </summary>
        public void UnselectAll()
        {
            foreach (ControlBase child in Children)
            {
                var button = child as CategoryButton;

                if (button == null)
                {
                    continue;
                }

                button.ToggleState = false;
            }
        }

        protected override Size Measure(Size availableSize)
        {
            Size headerSize = headerButton.DoMeasure(availableSize);

            if (IsCategoryCollapsed)
            {
                return headerSize;
            }

            Int32 width = headerSize.Width;
            Int32 height = headerSize.Height + Padding.Top + Padding.Bottom;

            foreach (ControlBase child in Children)
            {
                var button = child as CategoryButton;

                if (button == null)
                {
                    continue;
                }

                Size size = child.DoMeasure(availableSize);

                if (size.Width > width)
                {
                    width = child.Width;
                }

                height += size.Height;
            }

            width += Padding.Left + Padding.Right;

            return new Size(width, height);
        }

        protected override Size Arrange(Size finalSize)
        {
            headerButton.DoArrange(new Rectangle(x: 0, y: 0, finalSize.Width, headerButton.MeasuredSize.Height));

            if (IsCategoryCollapsed)
            {
                return new Size(finalSize.Width, headerButton.MeasuredSize.Height);
            }

            Int32 y = headerButton.MeasuredSize.Height + Padding.Top;
            Int32 width = finalSize.Width - Padding.Left - Padding.Right;
            var b = true;

            foreach (ControlBase child in Children)
            {
                var button = child as CategoryButton;

                if (button == null)
                {
                    continue;
                }

                button.alt = b;
                button.UpdateColors();
                b = !b;

                child.DoArrange(new Rectangle(Padding.Left, y, width, child.MeasuredSize.Height));
                y += child.MeasuredSize.Height;
            }

            y += Padding.Bottom;

            return new Size(finalSize.Width, y);
        }
    }
}
