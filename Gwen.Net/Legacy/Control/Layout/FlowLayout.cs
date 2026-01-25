using System;

namespace Gwen.Net.Legacy.Control.Layout
{
    /// <summary>
    ///     FlowLayout is a layout like <see cref="GridLayout" /> with auto sized columns
    ///     but you don't need to know exact number of columns.
    /// </summary>
    public class FlowLayout : ControlBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FlowLayout" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public FlowLayout(ControlBase parent)
            : base(parent) {}

        protected override Size Measure(Size availableSize)
        {
            availableSize -= Padding;

            var lineWidth = 0;
            var lineHeight = 0;
            var width = 0;
            var height = 0;
            var y = 0;

            foreach (ControlBase child in Children)
            {
                Size size = child.DoMeasure(availableSize);

                if (lineWidth + size.Width > availableSize.Width)
                {
                    y += lineHeight;

                    lineWidth = size.Width;
                    lineHeight = size.Height;
                }
                else
                {
                    lineWidth += size.Width;
                    lineHeight = Math.Max(lineHeight, size.Height);
                }

                width = Math.Max(width, lineWidth);
                height = Math.Max(height, y + size.Height);
            }

            width = Math.Max(width, lineWidth);
            height = Math.Max(height, lineHeight);

            return new Size(width, height) + Padding;
        }

        protected override Size Arrange(Size finalSize)
        {
            finalSize -= Padding;

            var lineHeight = 0;
            var width = 0;
            var height = 0;
            var x = 0;
            var y = 0;

            foreach (ControlBase child in Children)
            {
                if (x + child.MeasuredSize.Width > finalSize.Width)
                {
                    y += lineHeight;
                    x = 0;

                    lineHeight = 0;
                }

                child.DoArrange(
                    new Rectangle(
                        x + Padding.Left,
                        y + Padding.Top,
                        child.MeasuredSize.Width,
                        child.MeasuredSize.Height));

                width = Math.Max(width, x + child.MeasuredSize.Width);
                height = Math.Max(height, y + child.MeasuredSize.Height);

                x += child.MeasuredSize.Width;
                lineHeight = Math.Max(lineHeight, child.MeasuredSize.Height);
            }

            return new Size(width, height) + Padding;
        }
    }
}
