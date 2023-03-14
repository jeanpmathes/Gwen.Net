using System;

namespace Gwen.Net.Control.Internal
{
    public class ScrollArea : InnerContentControl
    {
        private bool canScrollH;
        private bool canScrollV;

        public ScrollArea(ControlBase parent)
            : base(parent)
        {
            canScrollV = true;
            canScrollH = true;
        }

        public Size ViewableContentSize { get; private set; }

        public Size ContentSize => new(innerPanel.ActualWidth, innerPanel.ActualHeight);

        public Point ScrollPosition
        {
            get => innerPanel.ActualPosition;
            set => SetScrollPosition(value.X, value.Y);
        }

        public int VerticalScroll
        {
            get => innerPanel.ActualTop;
            set => innerPanel.SetPosition(Content.ActualLeft, value);
        }

        public int HorizontalScroll
        {
            get => innerPanel.ActualLeft;
            set => innerPanel.SetPosition(value, innerPanel.ActualTop);
        }

        public virtual void EnableScroll(bool horizontal, bool vertical)
        {
            canScrollV = vertical;
            canScrollH = horizontal;
        }

        public void SetScrollPosition(int horizontal, int vertical)
        {
            innerPanel.SetPosition(horizontal, vertical);
        }

        protected override Size Measure(Size availableSize)
        {
            if (innerPanel == null)
            {
                return Size.Zero;
            }

            Size size = innerPanel.DoMeasure(
                new Size(
                    canScrollH ? Util.Infinity : availableSize.Width,
                    canScrollV ? Util.Infinity : availableSize.Height));

            // Let the parent determine the size if scrolling is enabled
            size.Width = canScrollH ? 0 : Math.Min(size.Width, availableSize.Width);
            size.Height = canScrollV ? 0 : Math.Min(size.Height, availableSize.Height);

            return size;
        }

        protected override Size Arrange(Size finalSize)
        {
            if (innerPanel == null)
            {
                return finalSize;
            }

            int scrollAreaWidth = Math.Max(finalSize.Width, innerPanel.MeasuredSize.Width);
            int scrollAreaHeight = Math.Max(finalSize.Height, innerPanel.MeasuredSize.Height);

            innerPanel.DoArrange(new Rectangle(x: 0, y: 0, scrollAreaWidth, scrollAreaHeight));

            ViewableContentSize = new Size(finalSize.Width, finalSize.Height);

            return finalSize;
        }
    }
}
