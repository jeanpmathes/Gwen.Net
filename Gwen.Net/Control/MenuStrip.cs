using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Menu strip.
    /// </summary>
    public class MenuStrip : Menu
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuStrip" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public MenuStrip(ControlBase parent)
            : base(parent)
        {
            Collapse(collapsed: false, measure: false);

            Padding = new Padding(left: 5, top: 0, right: 0, bottom: 0);
            IconMarginDisabled = true;
            EnableScroll(horizontal: true, vertical: false);

            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Top;

            layout.Horizontal = true;
            layout.HorizontalAlignment = HorizontalAlignment.Left;
            layout.VerticalAlignment = VerticalAlignment.Stretch;
        }

        /// <summary>
        ///     Determines whether the menu should open on mouse hover.
        /// </summary>
        protected override bool ShouldHoverOpenMenu => IsMenuOpen();

        /// <summary>
        ///     Closes the current menu.
        /// </summary>
        public override void Close() {}

        /// <summary>
        ///     Renders under the actual control (shadows etc).
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void RenderUnder(SkinBase currentSkin) {}

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawMenuStrip(this);
        }

        /// <summary>
        ///     Add item handler.
        /// </summary>
        /// <param name="item">Item added.</param>
        protected override void OnAddItem(MenuItem item)
        {
            item.TextPadding = new Padding(left: 5, top: 0, right: 5, bottom: 0);
            item.Padding = new Padding(left: 4, top: 4, right: 4, bottom: 4);
            item.HoverEnter += OnHoverItem;
        }
    }
}
