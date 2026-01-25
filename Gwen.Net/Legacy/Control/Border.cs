using Gwen.Net.Legacy.Skin;

namespace Gwen.Net.Legacy.Control
{
    public enum BorderType
    {
        ToolTip,
        StatusBar,
        MenuStrip,
        Selection,
        PanelNormal,
        PanelBright,
        PanelDark,
        PanelHighlight,
        ListBox,
        TreeControl,
        CategoryList
    }
    
    public class Border : ControlBase
    {
        public Border(ControlBase parent)
            : base(parent)
        {
            BorderType = BorderType.PanelNormal;
        }

        public BorderType BorderType { get; set; }

        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawBorder(this, BorderType);
        }
    }
}
