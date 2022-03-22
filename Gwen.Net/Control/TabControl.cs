using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Control.Layout;
using Gwen.Net.Xml;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Control with multiple tabs that can be reordered and dragged.
    /// </summary>
    [XmlControl(CustomHandler = "XmlElementHandler")]
    public class TabControl : ContentControl
    {
        private readonly ScrollBarButton[] m_Scroll;

        private Padding m_ActualPadding;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TabControl" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public TabControl(ControlBase parent)
            : base(parent)
        {
            TabStrip = new TabStrip(this);
            TabStrip.StripPosition = Dock.Top;

            // Actually these should be inside the TabStrip but it would make things complicated
            // because TabStrip contains only TabButtons. ScrollButtons being here we don't need
            // an inner panel for TabButtons on the TabStrip.
            m_Scroll = new ScrollBarButton[2];

            m_Scroll[0] = new ScrollBarButton(this);
            m_Scroll[0].SetDirectionLeft();
            m_Scroll[0].Clicked += ScrollPressedLeft;
            m_Scroll[0].Size = new Size(BaseUnit);

            m_Scroll[1] = new ScrollBarButton(this);
            m_Scroll[1].SetDirectionRight();
            m_Scroll[1].Clicked += ScrollPressedRight;
            m_Scroll[1].Size = new Size(BaseUnit);

            m_InnerPanel = new TabControlInner(this);
            m_InnerPanel.Dock = Dock.Fill;
            m_InnerPanel.SendToBack();

            IsTabable = false;

            m_ActualPadding = new Padding(left: 6, top: 6, right: 6, bottom: 6);
        }

        /// <summary>
        ///     Determines if tabs can be reordered by dragging.
        /// </summary>
        [XmlProperty] public bool AllowReorder
        {
            get => TabStrip.AllowReorder;
            set => TabStrip.AllowReorder = value;
        }

        /// <summary>
        ///     Currently active tab button.
        /// </summary>
        public TabButton CurrentButton { get; private set; }

        /// <summary>
        ///     Current tab strip position.
        /// </summary>
        [XmlProperty] public Dock TabStripPosition
        {
            get => TabStrip.StripPosition;
            set => TabStrip.StripPosition = value;
        }

        /// <summary>
        ///     Tab strip.
        /// </summary>
        public TabStrip TabStrip { get; }

        /// <summary>
        ///     Number of tabs in the control.
        /// </summary>
        public int TabCount => TabStrip.Children.Count;

        // Ugly way to implement padding but other ways would be more complicated
        [XmlProperty] public override Padding Padding
        {
            get => m_ActualPadding;
            set
            {
                m_ActualPadding = value;

                foreach (ControlBase tab in TabStrip.Children)
                {
                    tab.Margin = (Margin) value;
                }
            }
        }

        protected override void AdaptToScaleChange()
        {
            m_Scroll[0].Size = new Size(BaseUnit);
            m_Scroll[1].Size = new Size(BaseUnit);
        }

        /// <summary>
        ///     Invoked when a tab has been added.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> TabAdded;

        /// <summary>
        ///     Invoked when a tab has been removed.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> TabRemoved;

        /// <summary>
        ///     Adds a new page/tab.
        /// </summary>
        /// <param name="label">Tab label.</param>
        /// <param name="page">Page contents.</param>
        /// <returns>Newly created control.</returns>
        public TabButton AddPage(string label, ControlBase page = null)
        {
            if (null == page)
            {
                page = new DockLayout(this);
            }
            else
            {
                page.Parent = this;
            }

            TabButton button = new(TabStrip);
            button.Text = label;
            button.Page = page;
            button.IsTabable = false;

            AddPage(button);

            return button;
        }

        /// <summary>
        ///     Adds a page/tab.
        /// </summary>
        /// <param name="button">Page to add. (well, it's a TabButton which is a parent to the page).</param>
        internal void AddPage(TabButton button)
        {
            ControlBase page = button.Page;
            page.Parent = this;
            page.IsHidden = true;
            page.Dock = Dock.Fill;
            page.Margin = (Margin) Padding;

            button.Parent = TabStrip;

            if (button.TabControl != null)
            {
                button.TabControl.UnsubscribeTabEvent(button);
            }

            button.TabControl = this;
            button.Clicked += OnTabPressed;

            if (null == CurrentButton)
            {
                button.Press();
            }

            if (TabAdded != null)
            {
                TabAdded.Invoke(this, EventArgs.Empty);
            }

            Invalidate();
        }

        private void UnsubscribeTabEvent(TabButton button)
        {
            button.Clicked -= OnTabPressed;
        }

        /// <summary>
        ///     Handler for tab selection.
        /// </summary>
        /// <param name="control">Event source (TabButton).</param>
        internal virtual void OnTabPressed(ControlBase control, EventArgs args)
        {
            var button = control as TabButton;

            if (null == button)
            {
                return;
            }

            ControlBase page = button.Page;

            if (null == page)
            {
                return;
            }

            if (CurrentButton == button)
            {
                return;
            }

            if (null != CurrentButton)
            {
                ControlBase page2 = CurrentButton.Page;

                if (page2 != null)
                {
                    page2.IsHidden = true;
                }

                CurrentButton.Redraw();
                CurrentButton = null;
            }

            CurrentButton = button;

            page.IsHidden = false;
        }

        protected override Size Arrange(Size finalSize)
        {
            Size size = base.Arrange(finalSize);

            // At this point we know TabStrip location so lets move ScrollButtons
            int buttonSize = m_Scroll[0].Size.Width;

            switch (TabStrip.StripPosition)
            {
                case Dock.Top:
                    m_Scroll[0].SetPosition(TabStrip.ActualRight - 5 - buttonSize - buttonSize, TabStrip.ActualTop + 5);
                    m_Scroll[1].SetPosition(TabStrip.ActualRight - 5 - buttonSize, TabStrip.ActualTop + 5);
                    m_Scroll[0].SetDirectionLeft();
                    m_Scroll[1].SetDirectionRight();

                    break;
                case Dock.Bottom:
                    m_Scroll[0].SetPosition(
                        TabStrip.ActualRight - 5 - buttonSize - buttonSize,
                        TabStrip.ActualBottom - 5 - buttonSize);

                    m_Scroll[1].SetPosition(
                        TabStrip.ActualRight - 5 - buttonSize,
                        TabStrip.ActualBottom - 5 - buttonSize);

                    m_Scroll[0].SetDirectionLeft();
                    m_Scroll[1].SetDirectionRight();

                    break;
                case Dock.Left:
                    m_Scroll[0].SetPosition(
                        TabStrip.ActualLeft + 5,
                        TabStrip.ActualBottom - 5 - buttonSize - buttonSize);

                    m_Scroll[1].SetPosition(TabStrip.ActualLeft + 5, TabStrip.ActualBottom - 5 - buttonSize);
                    m_Scroll[0].SetDirectionUp();
                    m_Scroll[1].SetDirectionDown();

                    break;
                case Dock.Right:
                    m_Scroll[0].SetPosition(
                        TabStrip.ActualRight - 5 - buttonSize,
                        TabStrip.ActualBottom - 5 - buttonSize - buttonSize);

                    m_Scroll[1].SetPosition(
                        TabStrip.ActualRight - 5 - buttonSize,
                        TabStrip.ActualBottom - 5 - buttonSize);

                    m_Scroll[0].SetDirectionUp();
                    m_Scroll[1].SetDirectionDown();

                    break;
            }

            return size;
        }

        /// <summary>
        ///     Handler for tab removing.
        /// </summary>
        /// <param name="button"></param>
        internal virtual void OnLoseTab(TabButton button)
        {
            if (CurrentButton == button)
            {
                CurrentButton = null;
            }

            //TODO: Select a tab if any exist.

            if (TabRemoved != null)
            {
                TabRemoved.Invoke(this, EventArgs.Empty);
            }

            Invalidate();
        }

        protected override void OnBoundsChanged(Rectangle oldBounds)
        {
            var needed = false;

            switch (TabStripPosition)
            {
                case Dock.Top:
                case Dock.Bottom:
                    needed = TabStrip.TotalSize.Width > ActualWidth;

                    break;
                case Dock.Left:
                case Dock.Right:
                    needed = TabStrip.TotalSize.Height > ActualHeight;

                    break;
            }

            m_Scroll[0].IsHidden = !needed;
            m_Scroll[1].IsHidden = !needed;

            base.OnBoundsChanged(oldBounds);
        }

        protected virtual void ScrollPressedLeft(ControlBase control, EventArgs args)
        {
            TabStrip.ScrollOffset--;
        }

        protected virtual void ScrollPressedRight(ControlBase control, EventArgs args)
        {
            TabStrip.ScrollOffset++;
        }

        internal static ControlBase XmlElementHandler(Parser parser, Type type, ControlBase parent)
        {
            TabControl element = new(parent);
            parser.ParseAttributes(element);

            if (parser.MoveToContent())
            {
                foreach (string elementName in parser.NextElement())
                {
                    if (elementName == "TabPage")
                    {
                        string pageLabel = parser.GetAttribute("Text");

                        if (pageLabel == null)
                        {
                            pageLabel = "";
                        }

                        string pageName = parser.GetAttribute("Name");

                        if (pageName == null)
                        {
                            pageName = "";
                        }

                        TabButton button = element.AddPage(pageLabel);
                        button.Name = pageName;

                        ControlBase page = button.Page;
                        parser.ParseContainerContent(page);
                    }
                }
            }

            return element;
        }
    }
}
