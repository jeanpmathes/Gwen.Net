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
    
    public class ItemSelectedEventArgs<T> : ItemSelectedEventArgs where T : ControlBase
    {
        internal ItemSelectedEventArgs(T selectedItem) : base(selectedItem)
        {
            SelectedItem = selectedItem;
        }

        public new T SelectedItem { get; }
    }
}
