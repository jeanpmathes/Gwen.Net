using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    public class Popup : ScrollControl
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Popup" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Popup(ControlBase parent)
            : base(parent)
        {
            Padding = Padding.Two;

            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;

            AutoHideBars = true;
            EnableScroll(horizontal: false, vertical: false);
            DeleteOnClose = false;
        }

        internal override bool IsMenuComponent => true;

        /// <summary>
        ///     Determines whether the popup should be disposed on close.
        /// </summary>
        public bool DeleteOnClose { get; set; }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin)
        {
            skin.DrawMenu(this, paddingDisabled: true);
        }

        /// <summary>
        ///     Renders under the actual control (shadows etc).
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void RenderUnder(SkinBase skin)
        {
            base.RenderUnder(skin);
            skin.DrawShadow(this);
        }

        /// <summary>
        ///     Opens the popup.
        /// </summary>
        /// <param name="pos">Position where to open.</param>
        public void Open(Point pos)
        {
            IsCollapsed = false;
            BringToFront();
            Left = pos.X;
            Top = pos.Y;
            SetPosition(pos.X, pos.Y);
        }

        /// <summary>
        ///     Closes the current popup.
        /// </summary>
        public virtual void Close()
        {
            IsCollapsed = true;

            if (DeleteOnClose)
            {
                DelayedDelete();
            }
        }

        /// <summary>
        ///     Closes all menus and popups.
        /// </summary>
        public override void CloseMenus()
        {
            base.CloseMenus();
            Close();
        }
    }
}