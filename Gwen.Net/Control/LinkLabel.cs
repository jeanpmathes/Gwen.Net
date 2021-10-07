using System;
using Gwen.Net.Xml;

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

    [XmlControl]
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

        [XmlProperty] public string Link { get; set; }

        [XmlProperty] public Color HoverColor
        {
            get => m_hoverColor != null ? (Color)m_hoverColor : TextColor;
            set => m_hoverColor = value;
        }

        [XmlProperty] public Font HoverFont { get; set; }

        [XmlEvent] public event GwenEventHandler<LinkClickedEventArgs> LinkClicked;

        private void OnHoverEnter(ControlBase control, EventArgs args)
        {
            Cursor = Cursor.Finger;

            m_normalColor = m_Text.TextColor;
            m_Text.TextColor = HoverColor;

            if (HoverFont != null)
            {
                m_normalFont = m_Text.Font;
                m_Text.Font = HoverFont;
            }
        }

        private void OnHoverLeave(ControlBase control, EventArgs args)
        {
            m_Text.TextColor = m_normalColor;

            if (HoverFont != null)
            {
                m_Text.Font = m_normalFont;
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