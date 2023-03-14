using System;

namespace Gwen.Net.Control.Layout
{
    /// <summary>
    ///     Base splitter class.
    /// </summary>
    public class Splitter : ControlBase
    {
        private readonly ControlBase[] panel;
        private readonly bool[] scale;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Splitter" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Splitter(ControlBase parent) : base(parent)
        {
            panel = new ControlBase[2];
            scale = new bool[2];
            scale[0] = true;
            scale[1] = true;
        }

        /// <summary>
        ///     Sets the contents of a splitter panel.
        /// </summary>
        /// <param name="panelIndex">Panel index (0-1).</param>
        /// <param name="newPanel">Panel contents.</param>
        /// <param name="noScale">Determines whether the content is to be scaled.</param>
        public void SetPanel(int panelIndex, ControlBase newPanel, bool noScale = false)
        {
            if (panelIndex < 0 || panelIndex > 1)
            {
                throw new ArgumentException("Invalid panel index", "panelIndex");
            }

            this.panel[panelIndex] = newPanel;
            scale[panelIndex] = !noScale;

            if (null != this.panel[panelIndex])
            {
                this.panel[panelIndex].Parent = this;
            }
        }

        protected override Size Measure(Size availableSize)
        {
            Size size = Size.Zero;

            if (panel[0] != null)
            {
                panel[0].DoMeasure(new Size(availableSize.Width, availableSize.Height / 2));
                size = panel[0].MeasuredSize;
            }

            if (panel[1] != null)
            {
                panel[1].DoMeasure(new Size(availableSize.Width, availableSize.Height / 2));
                size.Width = Math.Max(size.Width, panel[1].MeasuredSize.Width);
                size.Height += panel[1].MeasuredSize.Height;
            }

            return size;
        }

        protected override Size Arrange(Size finalSize)
        {
            var y = 0;

            if (panel[0] != null)
            {
                panel[0].DoArrange(new Rectangle(x: 0, y: 0, finalSize.Width, finalSize.Height / 2));
                y = panel[0].ActualHeight;
            }

            if (panel[1] != null)
            {
                panel[1].DoArrange(new Rectangle(x: 0, y, finalSize.Width, finalSize.Height / 2));
                y += panel[1].ActualHeight;
            }

            return new Size(finalSize.Width, y);
        }
    }
}
