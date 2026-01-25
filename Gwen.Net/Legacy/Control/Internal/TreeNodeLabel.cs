namespace Gwen.Net.Legacy.Control.Internal
{
    /// <summary>
    ///     Tree node label.
    /// </summary>
    public class TreeNodeLabel : Button
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TreeNodeLabel" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public TreeNodeLabel(ControlBase parent)
            : base(parent)
        {
            Alignment = Alignment.Left | Alignment.CenterV;
            ShouldDrawBackground = false;
            TextPadding = new Padding(left: 3, top: 0, right: 5, bottom: 0);
        }

        /// <summary>
        ///     Updates control colors.
        /// </summary>
        public override void UpdateColors()
        {
            if (IsDisabled)
            {
                TextColor = Skin.colors.buttonColors.disabled;

                return;
            }

            if (ToggleState)
            {
                TextColor = Skin.colors.treeColors.selected;

                return;
            }

            if (IsHovered)
            {
                TextColor = Skin.colors.treeColors.hover;

                return;
            }

            TextColor = Skin.colors.treeColors.normal;
        }
    }
}
