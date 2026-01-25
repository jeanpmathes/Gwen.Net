using System;
using Gwen.Net.Legacy.Control.Internal;
using Gwen.Net.Legacy.Skin;

namespace Gwen.Net.Legacy.Control
{
    /// <summary>
    ///     Splitter control.
    /// </summary>
    public class CrossSplitter : ControlBase
    {
        private readonly SplitterBar cSplitter;
        private readonly SplitterBar hSplitter;

        private readonly ControlBase[] sections;
        private readonly SplitterBar vSplitter;

        private Single hVal; // 0-1
        private Single vVal; // 0-1

        private Int32 zoomedSection; // 0-3

        /// <summary>
        ///     Initializes a new instance of the <see cref="CrossSplitter" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public CrossSplitter(ControlBase parent)
            : base(parent)
        {
            sections = new ControlBase[4];

            vSplitter = new SplitterBar(this);
            vSplitter.Dragged += OnVerticalMoved;
            vSplitter.Cursor = Cursor.SizeNS;

            hSplitter = new SplitterBar(this);
            hSplitter.Dragged += OnHorizontalMoved;
            hSplitter.Cursor = Cursor.SizeWE;

            cSplitter = new SplitterBar(this);
            cSplitter.Dragged += OnCenterMoved;
            cSplitter.Cursor = Cursor.SizeAll;

            hVal = 0.5f;
            vVal = 0.5f;

            SetPanel(index: 0, panel: null);
            SetPanel(index: 1, panel: null);
            SetPanel(index: 2, panel: null);
            SetPanel(index: 3, panel: null);

            SplitterSize = 5;
            SplittersVisible = false;

            zoomedSection = -1;
        }

        /// <summary>
        ///     Indicates whether any of the panels is zoomed.
        /// </summary>
        public Boolean IsZoomed => zoomedSection != -1;

        /// <summary>
        ///     Gets or sets a value indicating whether splitters should be visible.
        /// </summary>
        public Boolean SplittersVisible
        {
            get => cSplitter.ShouldDrawBackground;
            set
            {
                cSplitter.ShouldDrawBackground = value;
                vSplitter.ShouldDrawBackground = value;
                hSplitter.ShouldDrawBackground = value;
            }
        }

        /// <summary>
        ///     Gets or sets the size of the splitter.
        /// </summary>
        public Int32 SplitterSize { get; set; }

        /// <summary>
        ///     Invoked when one of the panels has been zoomed (maximized).
        /// </summary>
        public event GwenEventHandler<EventArgs> PanelZoomed;

        /// <summary>
        ///     Invoked when one of the panels has been unzoomed (restored).
        /// </summary>
        public event GwenEventHandler<EventArgs> PanelUnZoomed;

        /// <summary>
        ///     Invoked when the zoomed panel has been changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> ZoomChanged;

        /// <summary>
        ///     Centers the panels so that they take even amount of space.
        /// </summary>
        public void CenterPanels()
        {
            hVal = 0.5f;
            vVal = 0.5f;
            Invalidate();
        }

        protected void OnCenterMoved(ControlBase control, EventArgs args)
        {
            CalculateValueCenter();
            Invalidate();
        }

        protected void OnVerticalMoved(ControlBase control, EventArgs args)
        {
            vVal = CalculateValueVertical();
            Invalidate();
        }

        protected void OnHorizontalMoved(ControlBase control, EventArgs args)
        {
            hVal = CalculateValueHorizontal();
            Invalidate();
        }

        private void CalculateValueCenter()
        {
            hVal = cSplitter.ActualLeft / (Single) (ActualWidth - cSplitter.ActualWidth);
            vVal = cSplitter.ActualTop / (Single) (ActualHeight - cSplitter.ActualHeight);
        }

        private Single CalculateValueVertical()
        {
            return vSplitter.ActualTop / (Single) (ActualHeight - vSplitter.ActualHeight);
        }

        private Single CalculateValueHorizontal()
        {
            return hSplitter.ActualLeft / (Single) (ActualWidth - hSplitter.ActualWidth);
        }

        protected override Size Measure(Size availableSize)
        {
            Size size = Size.Zero;

            vSplitter.DoMeasure(new Size(availableSize.Width, SplitterSize));
            hSplitter.DoMeasure(new Size(SplitterSize, availableSize.Height));
            cSplitter.DoMeasure(new Size(SplitterSize, SplitterSize));
            size = new Size(hSplitter.Width, vSplitter.Height);

            var h = (Int32) ((availableSize.Width - SplitterSize) * hVal);
            var v = (Int32) ((availableSize.Height - SplitterSize) * vVal);

            if (zoomedSection == -1)
            {
                if (sections[0] != null)
                {
                    sections[0].DoMeasure(new Size(h, v));
                    size += sections[0].MeasuredSize;
                }

                if (sections[1] != null)
                {
                    sections[1].DoMeasure(new Size(availableSize.Width - SplitterSize - h, v));
                    size += sections[1].MeasuredSize;
                }

                if (sections[2] != null)
                {
                    sections[2].DoMeasure(new Size(h, availableSize.Height - SplitterSize - v));
                    size += sections[2].MeasuredSize;
                }

                if (sections[3] != null)
                {
                    sections[3].DoMeasure(
                        new Size(availableSize.Width - SplitterSize - h, availableSize.Height - SplitterSize - v));

                    size += sections[3].MeasuredSize;
                }
            }
            else
            {
                sections[zoomedSection].DoMeasure(availableSize);
                size = sections[zoomedSection].MeasuredSize;
            }

            return size;
        }

        protected override Size Arrange(Size finalSize)
        {
            var h = (Int32) ((finalSize.Width - SplitterSize) * hVal);
            var v = (Int32) ((finalSize.Height - SplitterSize) * vVal);

            vSplitter.DoArrange(
                new Rectangle(x: 0, v, vSplitter.MeasuredSize.Width, vSplitter.MeasuredSize.Height));

            hSplitter.DoArrange(
                new Rectangle(h, y: 0, hSplitter.MeasuredSize.Width, hSplitter.MeasuredSize.Height));

            cSplitter.DoArrange(new Rectangle(h, v, cSplitter.MeasuredSize.Width, cSplitter.MeasuredSize.Height));

            if (zoomedSection == -1)
            {
                if (sections[0] != null)
                {
                    sections[0].DoArrange(new Rectangle(x: 0, y: 0, h, v));
                }

                if (sections[1] != null)
                {
                    sections[1].DoArrange(
                        new Rectangle(h + SplitterSize, y: 0, finalSize.Width - SplitterSize - h, v));
                }

                if (sections[2] != null)
                {
                    sections[2].DoArrange(
                        new Rectangle(x: 0, v + SplitterSize, h, finalSize.Height - SplitterSize - v));
                }

                if (sections[3] != null)
                {
                    sections[3].DoArrange(
                        new Rectangle(
                            h + SplitterSize,
                            v + SplitterSize,
                            finalSize.Width - SplitterSize - h,
                            finalSize.Height - SplitterSize - v));
                }
            }
            else
            {
                sections[zoomedSection].DoArrange(new Rectangle(x: 0, y: 0, finalSize.Width, finalSize.Height));
            }

            return finalSize;
        }

        /// <summary>
        ///     Assigns a control to the specific inner section.
        /// </summary>
        /// <param name="index">Section index (0-3).</param>
        /// <param name="panel">Control to assign.</param>
        public void SetPanel(Int32 index, ControlBase panel)
        {
            sections[index] = panel;

            if (panel != null)
            {
                panel.Parent = this;
            }

            Invalidate();
        }

        /// <summary>
        ///     Gets the specific inner section.
        /// </summary>
        /// <param name="index">Section index (0-3).</param>
        /// <returns>Specified section.</returns>
        public ControlBase GetPanel(Int32 index)
        {
            return sections[index];
        }

        protected override void OnChildAdded(ControlBase child)
        {
            if (!(child is SplitterBar))
            {
                if (sections[0] == null)
                {
                    SetPanel(index: 0, child);
                }
                else if (sections[1] == null)
                {
                    SetPanel(index: 1, child);
                }
                else if (sections[2] == null)
                {
                    SetPanel(index: 2, child);
                }
                else if (sections[3] == null)
                {
                    SetPanel(index: 3, child);
                }
                else
                {
                    throw new Exception("Too many panels added.");
                }
            }

            base.OnChildAdded(child);
        }

        /// <summary>
        ///     Internal handler for the zoom changed event.
        /// </summary>
        protected void OnZoomChanged()
        {
            if (ZoomChanged != null)
            {
                ZoomChanged.Invoke(this, EventArgs.Empty);
            }

            if (zoomedSection == -1)
            {
                if (PanelUnZoomed != null)
                {
                    PanelUnZoomed.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                if (PanelZoomed != null)
                {
                    PanelZoomed.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Maximizes the specified panel so it fills the entire control.
        /// </summary>
        /// <param name="section">Panel index (0-3).</param>
        public void Zoom(Int32 section)
        {
            UnZoom();

            if (sections[section] != null)
            {
                for (var i = 0; i < 4; i++)
                {
                    if (i != section && sections[i] != null)
                    {
                        sections[i].IsHidden = true;
                    }
                }

                zoomedSection = section;

                Invalidate();
            }

            OnZoomChanged();
        }

        /// <summary>
        ///     Restores the control so all panels are visible.
        /// </summary>
        public void UnZoom()
        {
            zoomedSection = -1;

            for (var i = 0; i < 4; i++)
            {
                if (sections[i] != null)
                {
                    sections[i].IsHidden = false;
                }
            }

            Invalidate();
            OnZoomChanged();
        }

        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawBorder(this, BorderType.TreeControl);
        }
    }
}
