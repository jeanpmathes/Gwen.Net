namespace Gwen.Net.Control.Internal
{
    /// <summary>
    ///     Header of CollapsibleCategory.
    /// </summary>
    public class CategoryHeaderButton : Button
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CategoryHeaderButton" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public CategoryHeaderButton(ControlBase parent)
            : base(parent)
        {
            ShouldDrawBackground = false;
            IsToggle = true;
            Alignment = Alignment.Center;
            TextPadding = new Padding(left: 3, top: 0, right: 3, bottom: 0);
        }

        /// <summary>
        ///     Updates control colors.
        /// </summary>
        public override void UpdateColors()
        {
            if (IsDepressed || ToggleState)
            {
                TextColor = Skin.colors.categoryColors.headerClosed;
            }
            else
            {
                TextColor = Skin.colors.categoryColors.header;
            }
        }
    }
}
