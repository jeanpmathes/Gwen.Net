using System;

namespace Gwen.Net.Legacy.Control.Layout
{
    /// <summary>
    ///     Arrange child controls into a row or a column.
    /// </summary>
    public class StackLayout : ControlBase
    {
        private Boolean horizontal;

        public StackLayout(ControlBase parent)
            : base(parent) {}

        /// <summary>
        ///     If set, arrange child controls into a row instead of a column.
        /// </summary>
        public Boolean Horizontal
        {
            get => horizontal;
            set
            {
                if (horizontal == value)
                {
                    return;
                }

                horizontal = value;
                Invalidate();
            }
        }

        protected override Size Measure(Size availableSize)
        {
            availableSize -= Padding;

            var width = 0;
            var height = 0;

            if (horizontal)
            {
                foreach (ControlBase child in Children)
                {
                    if (child.IsCollapsed)
                    {
                        continue;
                    }

                    Size size = child.DoMeasure(availableSize);
                    availableSize.Width -= size.Width;

                    if (size.Height > height)
                    {
                        height = size.Height;
                    }

                    width += size.Width;
                }
            }
            else
            {
                foreach (ControlBase child in Children)
                {
                    if (child.IsCollapsed)
                    {
                        continue;
                    }

                    Size size = child.DoMeasure(availableSize);
                    availableSize.Height -= size.Height;

                    if (size.Width > width)
                    {
                        width = size.Width;
                    }

                    height += size.Height;
                }
            }

            return new Size(width, height) + Padding;
        }

        protected override Size Arrange(Size finalSize)
        {
            finalSize -= Padding;

            if (horizontal)
            {
                Int32 height = finalSize.Height;
                Int32 x = Padding.Left;

                foreach (ControlBase child in Children)
                {
                    if (child.IsCollapsed)
                    {
                        continue;
                    }

                    child.DoArrange(new Rectangle(x, Padding.Top, child.MeasuredSize.Width, height));
                    x += child.MeasuredSize.Width;
                }

                x += Padding.Right;

                return new Size(x, finalSize.Height + Padding.Top + Padding.Bottom);
            }

            Int32 width = finalSize.Width;
            Int32 y = Padding.Top;

            foreach (ControlBase child in Children)
            {
                if (child.IsCollapsed)
                {
                    continue;
                }

                child.DoArrange(new Rectangle(Padding.Left, y, width, child.MeasuredSize.Height));
                y += child.MeasuredSize.Height;
            }

            y += Padding.Bottom;

            return new Size(finalSize.Width + Padding.Left + Padding.Right, y);
        }
    }
}
