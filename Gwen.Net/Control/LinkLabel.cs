using System;

namespace Gwen.Net.Control
{
    public class LinkClickedEventArgs : EventArgs
    {
        internal LinkClickedEventArgs(string link)
        {
            Link = link;
        }

        public string Link { get; }
    }

    public class LinkLabel : Label
    {
        private Color? m_hoverColor;
        private Color m_normalColor;
        private Font m_normalFont;

        public LinkLabel(ControlBase parent)
            : base(parent)
        {
            m_hoverColor = null;
            HoverFont = null;

            HoverEnter += OnHoverEnter;
            HoverLeave += OnHoverLeave;
            base.Clicked += OnClicked;
        }

        public string Link { get; set; }

        public Color HoverColor
        {
            get => m_hoverColor != null ? (Color)m_hoverColor : TextColor;
            set => m_hoverColor = value;
        }

        public Font HoverFont { get; set; }

        public event GwenEventHandler<LinkClickedEventArgs> LinkClicked;

        private void OnHoverEnter(ControlBase control, EventArgs args)
        {
            Cursor = Cursor.Finger;

            m_normalColor = text.TextColor;
            text.TextColor = HoverColor;

            if (HoverFont != null)
            {
                m_normalFont = text.Font;
                text.Font = HoverFont;
            }
        }

        private void OnHoverLeave(ControlBase control, EventArgs args)
        {
            text.TextColor = m_normalColor;

            if (HoverFont != null)
            {
                text.Font = m_normalFont;
            }
        }

        private void OnClicked(ControlBase control, ClickedEventArgs args)
        {
            if (LinkClicked != null)
            {
                LinkClicked(this, new LinkClickedEventArgs(Link));
            }
        }
    }
}
