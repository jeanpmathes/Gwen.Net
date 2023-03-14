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
        private Color? hoverColor;
        private Color normalColor;
        private Font normalFont;

        public LinkLabel(ControlBase parent)
            : base(parent)
        {
            hoverColor = null;
            HoverFont = null;

            HoverEnter += OnHoverEnter;
            HoverLeave += OnHoverLeave;
            base.Clicked += OnClicked;
        }

        public string Link { get; set; }

        public Color HoverColor
        {
            get => hoverColor ?? TextColor;
            set => hoverColor = value;
        }

        public Font HoverFont { get; set; }

        public event GwenEventHandler<LinkClickedEventArgs> LinkClicked;

        private void OnHoverEnter(ControlBase control, EventArgs args)
        {
            Cursor = Cursor.Finger;

            normalColor = text.TextColor;
            text.TextColor = HoverColor;

            if (HoverFont != null)
            {
                normalFont = text.Font;
                text.Font = HoverFont;
            }
        }

        private void OnHoverLeave(ControlBase control, EventArgs args)
        {
            text.TextColor = normalColor;

            if (HoverFont != null)
            {
                text.Font = normalFont;
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
