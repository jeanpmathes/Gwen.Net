using System;
using Gwen.Net.Legacy.Control.Internal;

namespace Gwen.Net.Legacy.Control
{
    public class HorizontalSplitter : ControlBase
    {
        private readonly ControlBase[] sections;
        private readonly SplitterBar vSplitter;

        private Single vVal; // 0-1
        private Int32 zoomedSection; // 0-1

        /// <summary>
        ///     Initializes a new instance of the <see cref="CrossSplitter" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public HorizontalSplitter(ControlBase parent)
            : base(parent)
        {
            sections = new ControlBase[2];

            vSplitter = new SplitterBar(this);
            vSplitter.Dragged += OnVerticalMoved;
            vSplitter.Cursor = Cursor.SizeNS;

            vVal = 0.5f;

            SetPanel(index: 0, panel: null);
            SetPanel(index: 1, panel: null);

            SplitterSize = 5;
            SplittersVisible = false;

            zoomedSection = -1;
        }

        /// <summary>
        ///     Splitter position (0 - 1)
        /// </summary>
        public Single Value
        {
            get => vVal;
            set => SetVValue(value);
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
            get => vSplitter.ShouldDrawBackground;
            set => vSplitter.ShouldDrawBackground = value;
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
            vVal = 0.5f;
            Invalidate();
        }

        public void SetVValue(Single value)
        {
            if (value <= 1f || value >= 0)
            {
                vVal = value;
            }

            Invalidate();
        }

        protected void OnVerticalMoved(ControlBase control, EventArgs args)
        {
            vVal = CalculateValueVertical();
            Invalidate();
        }

        private Single CalculateValueVertical()
        {
            return vSplitter.ActualTop / (Single) (ActualHeight - vSplitter.ActualHeight);
        }

        protected override Size Measure(Size availableSize)
        {
            Size size = Size.Zero;

            vSplitter.DoMeasure(new Size(availableSize.Width, SplitterSize));
            size.Height += vSplitter.Height;

            var v = (Int32) ((availableSize.Height - SplitterSize) * vVal);

            if (zoomedSection == -1)
            {
                if (sections[0] != null)
                {
                    sections[0].DoMeasure(new Size(availableSize.Width, v));
                    size.Height += sections[0].MeasuredSize.Height;
                    size.Width = Math.Max(size.Width, sections[0].MeasuredSize.Width);
                }

                if (sections[1] != null)
                {
                    sections[1].DoMeasure(new Size(availableSize.Width, availableSize.Height - SplitterSize - v));
                    size.Height += sections[1].MeasuredSize.Height;
                    size.Width = Math.Max(size.Width, sections[1].MeasuredSize.Width);
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
            var v = (Int32) ((finalSize.Height - SplitterSize) * vVal);

            vSplitter.DoArrange(
                new Rectangle(x: 0, v, vSplitter.MeasuredSize.Width, vSplitter.MeasuredSize.Height));

            if (zoomedSection == -1)
            {
                if (sections[0] != null)
                {
                    sections[0].DoArrange(new Rectangle(x: 0, y: 0, finalSize.Width, v));
                }

                if (sections[1] != null)
                {
                    sections[1].DoArrange(
                        new Rectangle(x: 0, v + SplitterSize, finalSize.Width, finalSize.Height - SplitterSize - v));
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
                for (var i = 0; i < 2; i++)
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

            for (var i = 0; i < 2; i++)
            {
                if (sections[i] != null)
                {
                    sections[i].IsHidden = false;
                }
            }

            Invalidate();
            OnZoomChanged();
        }
    }
}
