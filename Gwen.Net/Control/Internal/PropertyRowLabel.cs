namespace Gwen.Net.Control.Internal
{
    /// <summary>
    ///     Label for PropertyRow.
    /// </summary>
    public class PropertyRowLabel : Label
    {
        private readonly PropertyRow propertyRow;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyRowLabel" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public PropertyRowLabel(PropertyRow parent) : base(parent)
        {
            Alignment = Alignment.Left | Alignment.CenterV;
            propertyRow = parent;
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

            if (propertyRow != null && propertyRow.IsEditing)
            {
                TextColor = Skin.colors.propertiesColors.labelSelected;

                return;
            }

            if (propertyRow != null && propertyRow.IsHovered)
            {
                TextColor = Skin.colors.propertiesColors.labelHover;

                return;
            }

            TextColor = Skin.colors.propertiesColors.labelNormal;
        }
    }
}
