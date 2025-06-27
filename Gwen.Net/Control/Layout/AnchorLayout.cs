using System;

namespace Gwen.Net.Control.Layout
{
    /// <summary>
    ///     Arrange child controls by anchoring them proportionally into the edges of this control.
    /// </summary>
    /// <remarks>
    ///     You can control the anchoring process by setting Anchor and AnchorBounds
    ///     properties of the child control. You must set an AnchorBounds property of this control to
    ///     inform the layout process the default size of the area.
    /// </remarks>
    public class AnchorLayout : ControlBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AnchorLayout" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public AnchorLayout(ControlBase parent)
            : base(parent) {}

        protected override Size Measure(Size availableSize)
        {
            Size size = availableSize - Padding;

            foreach (ControlBase child in Children)
            {
                child.DoMeasure(size);
            }

            return availableSize;
        }

        protected override Size Arrange(Size finalSize)
        {
            Size size = finalSize - Padding;

            Size initialSize = AnchorBounds.Size;

            foreach (ControlBase child in Children)
            {
                Anchor anchor = child.Anchor;
                Rectangle anchorBounds = child.AnchorBounds;

                Int32 left = anchorBounds.Left + ((size.Width - initialSize.Width) * anchor.left / 100);
                Int32 top = anchorBounds.Top + ((size.Height - initialSize.Height) * anchor.top / 100);
                Int32 right = anchorBounds.Right + ((size.Width - initialSize.Width) * anchor.right / 100);
                Int32 bottom = anchorBounds.Bottom + ((size.Height - initialSize.Height) * anchor.bottom / 100);

                child.DoArrange(new Rectangle(left, top, right - left + 1, bottom - top + 1));
            }

            return finalSize;
        }
    }
}
