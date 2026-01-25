using System;
using Gwen.Net.Legacy.Skin;

namespace Gwen.Net.Legacy.Control
{
    /// <summary>
    ///     List box row (selectable).
    /// </summary>
    public class ListBoxRow : TableRow
    {
        private Boolean selected;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ListBoxRow" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ListBoxRow(ControlBase parent)
            : base(parent)
        {
            ListBox = (ListBox) parent;

            MouseInputEnabled = true;
            IsSelected = false;
        }

        public ListBox ListBox { get; }
        
        private Color? selectedColorOverride;
        private Color? normalColorOverride;

        /// <summary>
        /// Set an override color for the selected text.
        /// </summary>
        public Color? SelectedTextOverrideColor
        {
            get => selectedColorOverride;
            set
            {
                selectedColorOverride = value;
                UpdateTextColor();
            }
        }

        /// <summary>
        /// Set an override color for the normal text.
        /// </summary>
        public Color? NormalTextOverrideColor
        {
            get => normalColorOverride;
            set
            {
                normalColorOverride = value;
                UpdateTextColor();
            }
        }
        
        /// <summary>
        ///     Indicates whether the control is selected.
        /// </summary>
        public Boolean IsSelected
        {
            get => selected;
            set
            {
                selected = value;
                UpdateTextColor();
            }
        }

        private void UpdateTextColor()
        {
            SetTextColor(IsSelected 
                ? selectedColorOverride ?? Skin.colors.listBoxColors.textSelected 
                : normalColorOverride ?? Skin.colors.listBoxColors.textNormal);
        }

        /// <inheritdoc/>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawListBoxLine(this, IsSelected, EvenRow);
        }

        /// <summary>
        ///     Handler invoked on mouse click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="down">If set to <c>true</c> mouse button is down.</param>
        protected override void OnMouseClickedLeft(Int32 x, Int32 y, Boolean down)
        {
            base.OnMouseClickedLeft(x, y, down);

            if (down)
            {
                OnRowSelected();
            }
        }
    }
}
