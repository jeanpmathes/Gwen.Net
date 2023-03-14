using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Group box (container).
    /// </summary>
    public class GroupBox : ContentControl
    {
        private readonly Text text;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GroupBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public GroupBox(ControlBase parent)
            : base(parent)
        {
            text = new Text(this);

            innerPanel = new InnerContentControl(this);
        }

        /// <summary>
        ///     Text.
        /// </summary>
        public virtual string Text
        {
            get => text.String;
            set => text.String = value;
        }

        public override Padding Padding
        {
            get => innerPanel.Padding;
            set => innerPanel.Padding = value;
        }

        protected override Size Measure(Size availableSize)
        {
            Size titleSize = text.DoMeasure(availableSize);

            Size innerSize = Size.Zero;

            if (innerPanel != null)
            {
                innerSize = innerPanel.DoMeasure(
                    new Size(availableSize.Width - 5 - 5, availableSize.Height - titleSize.Height - 5));
            }

            return new Size(
                Math.Max(10 + titleSize.Width + 10, 5 + innerSize.Width + 5),
                titleSize.Height + innerSize.Height + 5);
        }

        protected override Size Arrange(Size finalSize)
        {
            Size size = finalSize;

            text.DoArrange(new Rectangle(x: 10, y: 0, text.MeasuredSize.Width, text.MeasuredSize.Height));

            if (innerPanel != null)
            {
                innerPanel.DoArrange(
                    new Rectangle(
                        x: 5,
                        text.MeasuredSize.Height,
                        finalSize.Width - 5 - 5,
                        finalSize.Height - text.MeasuredSize.Height - 5));
            }

            return size;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawGroupBox(this, textStart: 10, text.ActualHeight, text.ActualWidth);
        }
    }
}
