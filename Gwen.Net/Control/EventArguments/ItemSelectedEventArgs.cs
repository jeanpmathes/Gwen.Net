using System;

namespace Gwen.Net.Control
{
    public class ItemSelectedEventArgs : EventArgs
    {
        internal ItemSelectedEventArgs(ControlBase selectedItem)
        {
            SelectedItem = selectedItem;
        }

        public ControlBase SelectedItem { get; }
    }
}
