using System;
using Gwen.Net.Legacy.DragDrop;
using Gwen.Net.Legacy.Skin;

namespace Gwen.Net.Legacy.Control.Internal
{
    /// <summary>
    ///     Titlebar for DockedTabControl.
    /// </summary>
    public class TabTitleBar : Label
    {
        public TabTitleBar(ControlBase parent) : base(parent)
        {
            MouseInputEnabled = true;
            TextPadding = new Padding(left: 5, top: 2, right: 5, bottom: 2);
            Padding = new Padding(left: 1, top: 2, right: 1, bottom: 2);

            DragAndDrop_SetPackage(draggable: true, "TabWindowMove");
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawTabTitleBar(this);
        }

        public override void DragAndDrop_StartDragging(Package package, Int32 x, Int32 y)
        {
            DragAndDrop.SourceControl = Parent;
            DragAndDrop.SourceControl.DragAndDrop_StartDragging(package, x, y);
        }

        public void UpdateFromTab(TabButton button)
        {
            Text = button.Text;
        }
    }
}
