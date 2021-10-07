using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Xml;

namespace Gwen.Net.Control
{
    [XmlControl]
    public class VerticalSplitter : ControlBase
    {
        private readonly SplitterBar m_HSplitter;
        private readonly ControlBase[] m_Sections;

        private float m_HVal; // 0-1
        private int m_ZoomedSection; // 0-3

        /// <summary>
        ///     Initializes a new instance of the <see cref="CrossSplitter" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public VerticalSplitter(ControlBase parent)
            : base(parent)
        {
            m_Sections = new ControlBase[2];

            m_HSplitter = new SplitterBar(this);
            m_HSplitter.Dragged += OnHorizontalMoved;
            m_HSplitter.Cursor = Cursor.SizeWE;

            m_HVal = 0.5f;

            SetPanel(index: 0, panel: null);
            SetPanel(index: 1, panel: null);

            SplitterSize = 5;
            SplittersVisible = false;

            m_ZoomedSection = -1;
        }

        /// <summary>
        ///     Splitter position (0 - 1)
        /// </summary>
        [XmlProperty] public float Value
        {
            get => m_HVal;
            set => SetHValue(value);
        }

        /// <summary>
        ///     Indicates whether any of the panels is zoomed.
        /// </summary>
        public bool IsZoomed => m_ZoomedSection != -1;

        /// <summary>
        ///     Gets or sets a value indicating whether splitters should be visible.
        /// </summary>
        [XmlProperty] public bool SplittersVisible
        {
            get => m_HSplitter.ShouldDrawBackground;
            set => m_HSplitter.ShouldDrawBackground = value;
        }

        /// <summary>
        ///     Gets or sets the size of the splitter.
        /// </summary>
        [XmlProperty] public int SplitterSize { get; set; }

        /// <summary>
        ///     Invoked when one of the panels has been zoomed (maximized).
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> PanelZoomed;

        /// <summary>
        ///     Invoked when one of the panels has been unzoomed (restored).
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> PanelUnZoomed;

        /// <summary>
        ///     Invoked when the zoomed panel has been changed.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> ZoomChanged;

        /// <summary>
        ///     Centers the panels so that they take even amount of space.
        /// </summary>
        public void CenterPanels()
        {
            m_HVal = 0.5f;
            Invalidate();
        }

        public void SetHValue(float value)
        {
            if (value <= 1f || value >= 0)
            {
                m_HVal = value;
            }

            Invalidate();
        }

        protected void OnHorizontalMoved(ControlBase control, EventArgs args)
        {
            m_HVal = CalculateValueHorizontal();
            Invalidate();
        }

        private float CalculateValueHorizontal()
        {
            return m_HSplitter.ActualLeft / (float)(ActualWidth - m_HSplitter.ActualWidth);
        }

        protected override Size Measure(Size availableSize)
        {
            Size size = Size.Zero;

            m_HSplitter.DoMeasure(new Size(SplitterSize, availableSize.Height));
            size.Width += m_HSplitter.Width;

            int h = (int)((availableSize.Width - SplitterSize) * m_HVal);

            if (m_ZoomedSection == -1)
            {
                if (m_Sections[0] != null)
                {
                    m_Sections[0].DoMeasure(new Size(h, availableSize.Height));
                    size.Width += m_Sections[0].MeasuredSize.Width;
                    size.Height = Math.Max(size.Height, m_Sections[0].MeasuredSize.Height);
                }

                if (m_Sections[1] != null)
                {
                    m_Sections[1].DoMeasure(new Size(availableSize.Width - SplitterSize - h, availableSize.Height));
                    size.Width += m_Sections[1].MeasuredSize.Width;
                    size.Height = Math.Max(size.Height, m_Sections[1].MeasuredSize.Height);
                }
            }
            else
            {
                m_Sections[m_ZoomedSection].DoMeasure(availableSize);
                size = m_Sections[m_ZoomedSection].MeasuredSize;
            }

            return size;
        }

        protected override Size Arrange(Size finalSize)
        {
            int h = (int)((finalSize.Width - SplitterSize) * m_HVal);

            m_HSplitter.DoArrange(new Rectangle(h, y: 0, m_HSplitter.MeasuredSize.Width, finalSize.Height));

            if (m_ZoomedSection == -1)
            {
                if (m_Sections[0] != null)
                {
                    m_Sections[0].DoArrange(new Rectangle(x: 0, y: 0, h, finalSize.Height));
                }

                if (m_Sections[1] != null)
                {
                    m_Sections[1].DoArrange(
                        new Rectangle(h + SplitterSize, y: 0, finalSize.Width - SplitterSize - h, finalSize.Height));
                }
            }
            else
            {
                m_Sections[m_ZoomedSection].DoArrange(new Rectangle(x: 0, y: 0, finalSize.Width, finalSize.Height));
            }

            return finalSize;
        }

        /// <summary>
        ///     Assigns a control to the specific inner section.
        /// </summary>
        /// <param name="index">Section index (0-3).</param>
        /// <param name="panel">Control to assign.</param>
        public void SetPanel(int index, ControlBase panel)
        {
            m_Sections[index] = panel;

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
        public ControlBase GetPanel(int index)
        {
            return m_Sections[index];
        }

        protected override void OnChildAdded(ControlBase child)
        {
            if (!(child is SplitterBar))
            {
                if (m_Sections[0] == null)
                {
                    SetPanel(index: 0, child);
                }
                else if (m_Sections[1] == null)
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

            if (m_ZoomedSection == -1)
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
        public void Zoom(int section)
        {
            UnZoom();

            if (m_Sections[section] != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (i != section && m_Sections[i] != null)
                    {
                        m_Sections[i].IsHidden = true;
                    }
                }

                m_ZoomedSection = section;

                Invalidate();
            }

            OnZoomChanged();
        }

        /// <summary>
        ///     Restores the control so all panels are visible.
        /// </summary>
        public void UnZoom()
        {
            m_ZoomedSection = -1;

            for (int i = 0; i < 2; i++)
            {
                if (m_Sections[i] != null)
                {
                    m_Sections[i].IsHidden = false;
                }
            }

            Invalidate();
            OnZoomChanged();
        }
    }
}