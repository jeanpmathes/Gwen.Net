using System;

namespace Gwen.Net.Legacy.Control.Internal
{
    public class WindowTitleBar : Dragger
    {
        public WindowTitleBar(ControlBase parent)
            : base(parent)
        {
            Title = new Label(this);
            Title.Alignment = Alignment.Left | Alignment.CenterV;

            CloseButton = new CloseButton(this, parent as Window);
            CloseButton.IsTabable = false;
            CloseButton.Name = "closeButton";

            Target = parent;
        }

        public Label Title { get; }

        public CloseButton CloseButton { get; }

        protected override Size Measure(Size availableSize)
        {
            Title.DoMeasure(availableSize);

            if (!CloseButton.IsCollapsed)
            {
                CloseButton.DoMeasure(availableSize);
            }

            return availableSize;
        }

        protected override Size Arrange(Size finalSize)
        {
            Title.DoArrange(new Rectangle(x: 8, y: 0, Title.MeasuredSize.Width, finalSize.Height));

            if (!CloseButton.IsCollapsed)
            {
                Int32 closeButtonSize = finalSize.Height;

                CloseButton.DoArrange(
                    new Rectangle(finalSize.Width - 6 - closeButtonSize, y: 0, closeButtonSize, closeButtonSize));
            }

            return finalSize;
        }
    }
}