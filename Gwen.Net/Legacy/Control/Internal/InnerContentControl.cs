using System;

namespace Gwen.Net.Legacy.Control.Internal
{
    public class InnerContentControl : ContentControl
    {
        public InnerContentControl(ControlBase parent)
            : base(parent)
        {
            MouseInputEnabled = false;
            KeyboardInputEnabled = false;
        }

        protected override void OnChildAdded(ControlBase child)
        {
            if (innerPanel == null)
            {
                innerPanel = Children[index: 0];
            }

            base.OnChildAdded(child);
        }

        protected override Size Measure(Size availableSize)
        {
            if (innerPanel != null)
            {
                return innerPanel.DoMeasure(availableSize - Padding) + Padding;
            }

            return Size.Zero;
        }

        protected override Size Arrange(Size finalSize)
        {
            if (innerPanel != null)
            {
                innerPanel.DoArrange(new Rectangle(Padding.Left, Padding.Top, finalSize - Padding));
            }

            return finalSize;
        }

        public override ControlBase FindChildByName(String name, Boolean recursive = false)
        {
            if (innerPanel != null && innerPanel.Name == name)
            {
                return innerPanel;
            }

            return base.FindChildByName(name, recursive);
        }
    }
}
