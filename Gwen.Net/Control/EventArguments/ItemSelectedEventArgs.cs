using System;

namespace Gwen.Net.Control
{
    public class ItemSelectedEventArgs : EventArgs
    {
        internal ItemSelectedEventArgs(ControlBase selecteditem)
        {
            SelectedItem = selecteditem;
        }

        public ControlBase SelectedItem { get; }
    }
}